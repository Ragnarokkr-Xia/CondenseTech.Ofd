using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace CondenseTech.Ofd.Manager
{
    public class OfdContainer : IDisposable
    {
        private ZipFile zipFile;
        private Stream ofdFileStream;

        public OfdContainer(byte[] ofdData)
        {
            ReadOfd(new MemoryStream(ofdData));
        }

        public OfdContainer(string ofdFilename) : this(File.ReadAllBytes(ofdFilename))
        {
        }

        private void ReadOfd(Stream stream)
        {
            ofdFileStream = stream;
            zipFile = new ZipFile(stream);
        }

        public byte[] this[string index]
        {
            get
            {
                ZipEntry zipEntry = zipFile?.GetEntry(index);
                if (zipEntry != null)
                {
                    var zipStream = zipFile.GetInputStream(zipEntry);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        zipStream.CopyTo(stream);
                        zipStream.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        return data;
                    }
                }

                return null;
            }
        }

        public void Dispose()
        {
            zipFile?.Close();
            ofdFileStream?.Dispose();
        }
    }
}