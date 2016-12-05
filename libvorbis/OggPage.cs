namespace libvorbis
{
    using System.Runtime.InteropServices;

    public class OggPage
    {
        private libogg.ogg_page backingPage;

        public byte[] Body {get; }

        public byte[] Header { get; }

        public bool IsBeginOfStream => libogg.ogg_page_bos(ref this.backingPage) > 0;

        public bool IsEndOfStream => libogg.ogg_page_eos(ref this.backingPage) > 0;

        public long GranulePos => libogg.ogg_page_granulepos(ref this.backingPage);

        public int PageNumber => libogg.ogg_page_pageno(ref this.backingPage);

        public int SerialNumber => libogg.ogg_page_serialno(ref this.backingPage);

        public bool IsContinued => libogg.ogg_page_continued(ref this.backingPage) == 1;

        public int PacketsCompletedOnPage => libogg.ogg_page_packets(ref this.backingPage);

        public int Version => libogg.ogg_page_version(ref this.backingPage);

        public OggPage(libogg.ogg_page backingPage)
        {
            this.backingPage = backingPage;

            var header = new byte[backingPage.header_len];
            Marshal.Copy(backingPage.header, header, 0, backingPage.header_len);

            var body = new byte[backingPage.body_len];
            Marshal.Copy(backingPage.body, body, 0, backingPage.body_len);
        }
    }
}