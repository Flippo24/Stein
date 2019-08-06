using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using log4net;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using Stein.Common.Configuration;
using Stein.Common.UpdateService;
using Stein.Localization;
using Stein.Presentation;
using Stein.ViewModels;
using Stein.ViewModels.Commands.MainWindowDialogModelCommands;
using Stein.ViewModels.Extensions;

namespace Stein
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string AppTmpFolderPath = Path.Combine(Path.GetTempPath(), Assembly.GetEntryAssembly().GetName().Name);

        private IViewModelService _viewModelService;

        private IDialogService _dialogService;
        
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            System.Windows.Forms.Application.ThreadException += WinFormApplication_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            // https://stackoverflow.com/a/520334
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var kernel = new StandardKernel(new AppModule());
            _dialogService = kernel.Get<IDialogService>();
            _viewModelService = kernel.Get<IViewModelService>(new ConstructorArgument("tmpFolderPath", AppTmpFolderPath));

            var configurationService = kernel.Get<IConfigurationService>();
            var isFirstLaunch = false;
            try
            {
                await configurationService.LoadConfigurationAsync();
            }
            catch (Exception exception)
            {
                Log.Error("Loading configuration failed, will create a new one", exception);
                isFirstLaunch = true;
            }

            var themeService = kernel.Get<IThemeService>();
            themeService.SetTheme(configurationService.Configuration.SelectedTheme);
            
            var mainDialogModel = _viewModelService.CreateViewModel<MainWindowDialogModel>();
            _dialogService.Show(mainDialogModel);

            Task refreshTask = null;
            if (mainDialogModel.TryGetCommand<MainWindowDialogModel, RefreshApplicationsCommand>(out var refreshCommand))
                refreshTask = refreshCommand.ExecuteAsync(null);

            var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            const string repository = "nkristek/Stein";
            var updateService = kernel.Get<IUpdateService>(
                new ConstructorArgument("currentVersion", assemblyVersion),
                new ConstructorArgument("repository", repository));
            var notificationService = kernel.Get<INotificationService>();
            var updateTask = CheckForUpdate(updateService, notificationService, mainDialogModel);

            if (isFirstLaunch)
            {
                var welcomeDialog = _viewModelService.CreateViewModel<WelcomeDialogModel>(mainDialogModel);
                _dialogService.ShowDialog(welcomeDialog);
            }
            
            try
            {
                await updateTask;
            }
            catch (Exception exception)
            {
                Log.Error("Checking for update failed.", exception);
            }

            if (refreshTask != null)
                await refreshTask;
        }

        private async Task CheckForUpdate(IUpdateService updateService, INotificationService notificationService, MainWindowDialogModel mainDialogModel)
        {
            var updateResult = await updateService.IsUpdateAvailable();
            if (updateResult.IsUpdateAvailable)
            {
                var updateDialogModel = _viewModelService.CreateViewModel<UpdateDialogModel>(mainDialogModel);
                updateDialogModel.CurrentVersion = updateResult.CurrentVersion;
                updateDialogModel.UpdateVersion = updateResult.NewestVersion;
                updateDialogModel.UpdateUri = updateResult.NewestVersionUri;

                foreach (var updateAsset in updateResult.UpdateAssets)
                {
                    var updateAssetViewModel = _viewModelService.CreateViewModel<UpdateAssetViewModel>(updateDialogModel);
                    updateAssetViewModel.DownloadUri = updateAsset.DownloadUri;
                    updateAssetViewModel.FileName = updateAsset.FileName;
                    updateAssetViewModel.ReleaseTag = updateResult.ReleaseTag;
                    updateAssetViewModel.IsReadOnly = true;
                    updateAssetViewModel.IsDirty = false;
                    updateDialogModel.UpdateAssets.Add(updateAssetViewModel);
                }

                mainDialogModel.AvailableUpdate = updateDialogModel;

                notificationService.ShowInfo(Strings.UpdateAvailableDescription, () =>
                {
                    if (mainDialogModel.TryGetCommand<MainWindowDialogModel, ShowUpdateDialogCommand>(out var showUpdateDialogCommand))
                        showUpdateDialogCommand.Execute(null);
                });
            }
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DeleteTmpFolder();
        }

        private static void DeleteTmpFolder()
        {
            try
            {
                Directory.Delete(AppTmpFolderPath, true);
            }
            catch (Exception exception)
            {
                Log.Error("Deleting tmp folder failed", exception);
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (exception == null)
                return;

            HandleException(exception);
            e.Handled = true;
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception exception))
                return;

            HandleException(exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (exception == null)
                return;

            HandleException(exception);
            if (!e.Observed)
                e.SetObserved();
        }

        private void WinFormApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (exception == null)
                return;

            HandleException(exception);
        }

        private void HandleException(Exception exception)
        {
            if (exception == null)
                return;

            try
            {
                Log.Error(exception);
                ShowExceptionDialog(exception);
            }
            catch
            {
                // no further escalation
            }
        }

        private bool? ShowExceptionDialog(Exception exception)
        {
            return Dispatcher.Invoke(() =>
            {
                if (_viewModelService == null || _dialogService == null)
                {
                    MessageBox.Show(exception.ToString(), "Error");
                    Environment.Exit(1);
                }
                var exceptionDialogModel = _viewModelService.CreateViewModel<ExceptionDialogModel>(null, exception);
                return _dialogService.ShowDialog(exceptionDialogModel);
            });
        }
    }
}
