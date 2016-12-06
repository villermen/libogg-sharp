namespace Villermen.LiboggSharp
{
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

            this.IsBeginOfStream = liboggPacket.b_o_s == 1;
            this.IsEndOfStream = liboggPacket.e_o_s == 1;
            this.GranulePosition = liboggPacket.granulepos;
            this.PacketNumber = liboggPacket.packetno;
        }

        public libogg.ogg_packet ToLiboggPacket()
        {
            var liboggPacket = new libogg.ogg_packet
            {
                packet = Marshal.AllocHGlobal(this.Data.Length),
                bytes = this.Data.Length,
                b_o_s = this.IsBeginOfStream ? 1 : 0,
                e_o_s = this.IsEndOfStream ? 1 : 0,
                granulepos = this.GranulePosition,
                packetno = this.PacketNumber
            };

            Marshal.Copy(this.Data, 0, liboggPacket.packet, liboggPacket.bytes);

            return liboggPacket;
        }
    }
}