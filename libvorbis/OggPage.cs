namespace libvorbis
{
    using System.Runtime.InteropServices;

    public class OggPage
    {
        private libogg.ogg_page backingPage;

        internal libogg.ogg_page BackingPage => this.backingPage;

        private byte[] body;

        public byte[] Body
        {
            get
            {
                if (this.body == null)
                {
                    this.body = new byte[this.BackingPage.body_len];
                    Marshal.Copy(this.BackingPage.body, this.body, 0, this.BackingPage.body_len);
                }

                return this.body;
            }
        }

        private byte[] header;

        public byte[] Header
        {
            get
            {
                if (this.header == null)
                {
                    this.header = new byte[this.BackingPage.header_len];
                    Marshal.Copy(this.BackingPage.header, this.header, 0, this.BackingPage.header_len);
                }

                return this.header;
            }
        }

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
        }
    }
}