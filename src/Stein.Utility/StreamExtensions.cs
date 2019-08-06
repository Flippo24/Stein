﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Stein.Utility
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Asynchronously copy data from the <paramref name="source"/> <see cref="Stream"/> to the <paramref name="destination"/> <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">Source <see cref="Stream"/>.</param>
        /// <param name="destination">Destination <see cref="Stream"/>.</param>
        /// <param name="bufferSize">Size of the buffer to use.</param>
        /// <param name="progress">Optional progress reporter.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>Asynchronous <see cref="Task"/> which copies the data.</returns>
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize = 81920, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            cancellationToken.ThrowIfCancellationRequested();
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
