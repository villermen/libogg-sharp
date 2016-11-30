namespace libvorbis
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;

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

            while (libogg.ogg_sync_pageout(ref syncState, ref oggPage) != 1)
            {
                var bufferPtr = libogg.ogg_sync_buffer(ref syncState, 4096);

                var readBytes = this.reader.ReadBytes(4096);

                if (readBytes.Length == 0)
                {
                    break;
                }

                Marshal.Copy(readBytes, 0, bufferPtr, readBytes.Length);

                if (libogg.ogg_sync_wrote(ref this.syncState, readBytes.Length) != 0)
                {
                    throw new Exception("Could not confirm amount of bytes written to libogg.");
                }
            }

            var header = new byte[oggPage.header_len];
            Marshal.Copy(oggPage.header, header, 0, oggPage.header_len);

            var body = new byte[oggPage.body_len];
            Marshal.Copy(oggPage.body, body, 0, oggPage.body_len);

            return new OggPage(header, body);
        }

        public void Dispose()
        {
            this.reader.Dispose();
            libogg.ogg_sync_clear(ref this.syncState);
        }
    }
}