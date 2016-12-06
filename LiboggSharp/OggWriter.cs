﻿namespace Villermen.LiboggSharp
{
    using System;
    using System.IO;

    public class OggWriter : IDisposable
    {
        private libogg.ogg_stream_state streamState;

        private BinaryWriter writer;

        public int SerialNumber { get; }

        public OggWriter(Stream stream)
            : this(stream, new Random().Next())
        { 
        }

        public OggWriter(Stream stream, int serialNumber)
        {
            this.writer = new BinaryWriter(stream);
            this.streamState = new libogg.ogg_stream_state();
            this.SerialNumber = serialNumber;

            var result = libogg.ogg_stream_init(ref this.streamState, this.SerialNumber);

            if (result != 0)
            {
                throw new ArgumentException("Could not initialize libogg sync state.");
            }
        }

        public void Write(OggPage page)
        {
            var liboggPage = page.ToLiboggPage();
            var result = libogg.ogg_stream_pagein(ref this.streamState, ref liboggPage);

            if (result != 0)
            {
                throw new Exception("Page to write does not match the serial number of the bitstream, the page version is incorrect or libogg encountered an internal error.");
            }

            this.Flush(false);
        }

        public void Write(OggPacket packet)
        {
            var liboggPacket = packet.ToLiboggPacket();
            var result = libogg.ogg_stream_packetin(ref this.streamState, ref liboggPacket);

            if (result != 0)
            {
                throw new Exception("Internal error occurred when trying to submit a packet to the libogg stream.");
            }

            this.Flush(false);
        }

        public void Flush(bool force = true)
        {
            var backingPage = new libogg.ogg_page();
            var result = libogg.ogg_stream_pageout(ref this.streamState, ref backingPage);

            // Write page data to stream if a page was accumulated
            if (result != 0)
            {
                this.Write(backingPage);
            }

            // Write remaining page data to stream if forcing
            if (force)
            {
                result = libogg.ogg_stream_flush(ref this.streamState, ref backingPage);

                if (result != 0)
                {
                    this.Write(backingPage);   
                }
            }
        }

        private void Write(libogg.ogg_page page)
        {
            var wrappedPage = new OggPage(page);
            this.writer.Write(wrappedPage.Header);
            this.writer.Write(wrappedPage.Body);
        }

        public void Dispose()
        {
            this.Flush();
            this.writer.Dispose();
            libogg.ogg_stream_destroy(ref this.streamState);
        }
    }
}