namespace Villermen.LiboggSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class OggReader : IDisposable, IEnumerable<OggPacket>
    {
        private readonly BinaryReader reader;

        private libogg.ogg_sync_state syncState;

        private bool isStreamStateInitialized = false;

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

        private void BufferPage()
        {
            var liboggPage = new libogg.ogg_page();

            while (libogg.ogg_sync_pageout(ref this.syncState, ref liboggPage) != 1)
            {
                var bufferPtr = libogg.ogg_sync_buffer(ref this.syncState, 8192);

                var readBytes = this.reader.ReadBytes(8192);

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

            // Initialize a stream state with the same serial number as the page we just read
            if (!this.isStreamStateInitialized)
            {
                var result = libogg.ogg_stream_init(ref this.streamState, libogg.ogg_page_serialno(ref liboggPage));

                if (result != 0)
                {
                    throw new InvalidOperationException("Failed to initialize libogg stream state.");
                }

                this.isStreamStateInitialized = true;
            }

            // Submit page to stream
            if (libogg.ogg_stream_pagein(ref this.streamState, ref liboggPage) != 0)
            {
                throw new InvalidOperationException("Adding page to packet buffer failed due to a serial or page number mismatch, or an internal error occurred.");
            }
        }

        /// <summary>
        /// Reads a packet from the stream.
        /// Internally this reads a page whenever no full packet can be read.
        /// Note that the granule position of the returned packet is -1 when it is not the last packet that ended on a page.
        /// </summary>
        /// <returns></returns>
        public OggPacket ReadPacket()
        {
            var liboggPacket = new libogg.ogg_packet();
            int packetOutResult;

            // Retry without new page if it is out of sync and has a gap (-1)
            do
            {
                packetOutResult = libogg.ogg_stream_packetout(ref this.streamState, ref liboggPacket);

                // Read a new page into the stream when there is insufficient data available
                if (packetOutResult == 0)
                {
                    this.BufferPage();
                }
            }
            while (packetOutResult != 1);

            return new OggPacket(liboggPacket);
        }

        public void Dispose()
        {
            this.reader.Dispose();
            libogg.ogg_sync_clear(ref this.syncState);
            libogg.ogg_stream_clear(ref this.streamState);
        }

        public IEnumerator<OggPacket> GetEnumerator()
        {
            OggPacket packet;

            do
            {
                packet = this.ReadPacket();

                yield return packet;
            }
            while (packet.IsEndOfStream == false);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}