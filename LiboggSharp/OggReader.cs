namespace Villermen.LiboggSharp
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
            return new OggPage(this.ReadLiboggPage(submitToPacketBuffer));
        }

        private libogg.ogg_page ReadLiboggPage(bool submitToPacketBuffer)
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

            if (submitToPacketBuffer)
            {
                this.SubmitLiboggPageToPacketBuffer(liboggPage);
            }

            return liboggPage;
        }

        private void SubmitLiboggPageToPacketBuffer(libogg.ogg_page liboggPage)
        {
            if (libogg.ogg_stream_pagein(ref this.streamState, ref liboggPage) != 0)
            {
                throw new InvalidOperationException("Adding page to packet buffer failed due to a serial or page number mismatch, or an internal error occurred.");
            }
        }

        public OggPacket ReadPacket()
        {
            this.EnsureStreamStateInitialized();

            var liboggPacket = new libogg.ogg_packet();
            int packetOutResult;

            // Retry without new page if it is out of sync and has a gap (-1)
            do
            {
                packetOutResult = libogg.ogg_stream_packetout(ref this.streamState, ref liboggPacket);

                // Read a new page into the stream when there is insufficient data available
                if (packetOutResult == 0)
                {
                    this.ReadLiboggPage(true);
                }
            }
            while (packetOutResult != 1);

            return new OggPacket(liboggPacket);
        }

        private void EnsureStreamStateInitialized()
        {
            if (!this.streamStateInitialized)
            {
                // Read a page to determine the serial number to use
                var liboggPage = this.ReadLiboggPage(false);

                var serialNumber = libogg.ogg_page_serialno(ref liboggPage);

                // Actually initialize the stream state
                var result = libogg.ogg_stream_init(ref this.streamState, serialNumber);

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