namespace libvorbis
{
    public class OggPage
    {
        public byte[] Body { get; set; }
        public byte[] Header { get; set; }

        public OggPage(byte[] header, byte[] body)
        {
            this.Header = header;
            this.Body = body;
        }
    }
}