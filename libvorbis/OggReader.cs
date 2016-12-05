namespace libvorbis
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class OggReader : IDisposable
    {
        private readonly BinaryReader reader;

        private libogg.ogg_sync_state syncState;

        public OggReader(Stream stream)
        {
            this.reader = new BinaryReader(stream);

            this.syncState = new libogg.ogg_sync_state();

            var result = libogg.ogg_sync_init(ref this.syncState);

            if (result != 0)
            {
                throw new ArgumentException("Could not initialize libogg sync state.");
            }
        }

        public OggPage ReadPage()
        {
            var oggPage = new libogg.ogg_page();

            while (libogg.ogg_sync_pageout(ref this.syncState, ref oggPage) != 1)
            {
                var bufferPtr = libogg.ogg_sync_buffer(ref this.syncState, 4096);

                var readBytes = this.reader.ReadBytes(4096);

                if (readBytes.Length == 0)
                {
                    throw new InvalidOperationException("No more data in stream.");
                }

                Marshal.Copy(readBytes, 0, bufferPtr, readBytes.Length);

                if (libogg.ogg_sync_wrote(ref this.syncState, readBytes.Length) != 0)
                {
                    throw new Exception("Could not confirm amount of bytes written to libogg.");
                }
            }

            return new OggPage(oggPage);
        }

        public OggPacket ReadPacket()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.reader.Dispose();
            libogg.ogg_sync_clear(ref this.syncState);
        }
    }
}