namespace libvorbis
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class OggReader : IDisposable
    {
        private readonly BinaryReader reader;

        private libogg.ogg_sync_state syncState;

        private bool streamStateInitialized = false;

        private libogg.ogg_stream_state streamState;

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

        /// <summary>
        /// Reads a page from the stream.
        /// </summary>
        /// <param name="submitToPacketBuffer">
        /// Whether to submit this page to the packet buffer.
        /// Will prevent future packets from being read correctly if false, but causes overhead if set to true.
        /// </param>
        /// <exception cref="EndOfStreamException">I wonder when this exception occurs.</exception>
        /// <exception cref="InvalidOperationException">When the amount of bytes written could not be confirmed to libogg.</exception>
        /// <returns></returns>
        public OggPage ReadPage(bool submitToPacketBuffer = false)
        {
            return new OggPage(this.ReadLiboggPage());
        }

        private libogg.ogg_page ReadLiboggPage()
        {
            var liboggPage = new libogg.ogg_page();

            while (libogg.ogg_sync_pageout(ref this.syncState, ref liboggPage) != 1)
            {
                var bufferPtr = libogg.ogg_sync_buffer(ref this.syncState, 4096);

                var readBytes = this.reader.ReadBytes(4096);

                if (readBytes.Length == 0)
                {
                    throw new EndOfStreamException("No more data in stream.");
                }

                Marshal.Copy(readBytes, 0, bufferPtr, readBytes.Length);

                if (libogg.ogg_sync_wrote(ref this.syncState, readBytes.Length) != 0)
                {
                    throw new InvalidOperationException("Could not confirm amount of bytes written to libogg.");
                }
            }

            return liboggPage;
        }

        public OggPacket ReadPacket()
        {
            this.InitializeStreamState();

            if ()

            var liboggPage = this.ReadLiboggPage();




            throw new NotImplementedException();
        }

        private void InitializeStreamState()
        {
            if (!this.streamStateInitialized)
            {
                var result = libogg.ogg_stream_init(ref this.streamState, new Random().Next());

                if (result != 0)
                {
                    throw new InvalidOperationException("Could not initialize libogg stream.");
                }

                this.streamStateInitialized = true;
            }
        }

        public void Dispose()
        {
            this.reader.Dispose();
            libogg.ogg_sync_clear(ref this.syncState);
        }
    }
}