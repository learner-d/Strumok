using StrumokLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrumokApp.Util
{
    public static class StrumokCrypterUtils
    {
        public const int ENCRYPTION_UNIT_SIZE = 1 * 1024 * 1024; // 1 MB
        public static async Task DoCryption(string inputFilePath, string outputFilePath, ulong[] key, ulong[] iv, IProgress<double> progress, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[ENCRYPTION_UNIT_SIZE];
            byte[] output = new byte[ENCRYPTION_UNIT_SIZE];
            using (Stream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StrumokCrypter crypter = new StrumokCrypter(key, iv))
            using (Stream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                for (long cryptedBytes = 0; cryptedBytes < inputStream.Length;)
                {
                    int bufferSize = (int)new long[] { ENCRYPTION_UNIT_SIZE, inputStream.Length - cryptedBytes, int.MaxValue }.Min();
                    // Прочитати файл
                    await inputStream.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false);
                    // Шифрувати вміст
                    await crypter.CryptAsync(buffer, output, bufferSize, cancellationToken).ConfigureAwait(false);
                    // Записати до кінцевого файлу
                    await outputStream.WriteAsync(output, 0, bufferSize, cancellationToken).ConfigureAwait(false);
                    cryptedBytes += bufferSize;
                    double progressVal = (double)cryptedBytes / inputStream.Length;
                    progress?.Report(progressVal);
                }
            }
        }
    }
}
