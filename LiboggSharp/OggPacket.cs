namespace Villermen.LiboggSharp
{
    using System;
    using System.Runtime.InteropServices;

    public class OggPacket
    {
        public byte[] Data { get; set; }

        public bool IsBeginOfStream { get; }

        public bool IsEndOfStream { get; }

        public long GranulePosition { get; set; }

        public long PacketNumber { get; set; }

        public OggPacket(libogg.ogg_packet liboggPacket)
        {
            this.Data = new byte[liboggPacket.bytes];
            Marshal.Copy(liboggPacket.packet, this.Data, 0, liboggPacket.bytes);

            this.IsBeginOfStream = liboggPacket.b_o_s != 0;
            this.IsEndOfStream = liboggPacket.e_o_s != 0;
            this.GranulePosition = liboggPacket.granulepos;
            this.PacketNumber = liboggPacket.packetno;
        }
    }
}