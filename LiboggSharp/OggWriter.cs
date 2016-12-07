namespace Villermen.LiboggSharp
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

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

        public void Write(OggPacket packet)
        {
            var liboggPacket = new libogg.ogg_packet
            {
                packet = Marshal.AllocHGlobal(packet.Data.Length),
                bytes = packet.Data.Length,
                b_o_s = packet.IsBeginOfStream ? 1 : 0,
                e_o_s = packet.IsEndOfStream ? 1 : 0,
                granulepos = packet.GranulePosition,
                packetno = packet.PacketNumber
            };

            Marshal.Copy(packet.Data, 0, liboggPacket.packet, packet.Data.Length);

            var result = libogg.ogg_stream_packetin(ref this.streamState, ref liboggPacket);

            Marshal.FreeHGlobal(liboggPacket.packet);

            if (result != 0)
            {
                throw new Exception("Internal error occurred when trying to submit a packet to the libogg stream.");
            }

            this.Flush(false);
        }

        public void Flush(bool force = true)
        {
            var liboggPage = new libogg.ogg_page();
            int result;

            // Output pages until no more are available using the chosen method
            do
            {
                // Try to obtain a page, or squeeze it out if forcing
                result = force
                    ? libogg.ogg_stream_flush(ref this.streamState, ref liboggPage)
                    : libogg.ogg_stream_pageout(ref this.streamState, ref liboggPage);

                // Write page data to stream if a page was accumulated
                if (result != 0)
                {
                    this.Write(liboggPage);
                }
            }
            while (result != 0);
        }

        private void Write(libogg.ogg_page page)
        {
            var header = new byte[page.header_len];
            var body = new byte[page.body_len];

            Marshal.Copy(page.header, header, 0, page.header_len);
            Marshal.Copy(page.body, body, 0, page.body_len);

            this.writer.Write(header);
            this.writer.Write(body);
        }

        public void Dispose()
        {
            this.Flush(true);
            this.writer.Dispose();
            libogg.ogg_stream_clear(ref this.streamState);
        }
    }
}