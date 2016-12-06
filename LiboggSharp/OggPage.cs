﻿namespace Villermen.LiboggSharp
{
    using System.Runtime.InteropServices;

    public class OggPage
    {
        private libogg.ogg_page backingPage;

        private byte[] body;

        public byte[] Body
        {
            get
            {
                if (this.body == null)
                {
                    this.body = new byte[this.backingPage.body_len];
                    Marshal.Copy(this.backingPage.body, this.body, 0, this.backingPage.body_len);
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
                    this.header = new byte[this.backingPage.header_len];
                    Marshal.Copy(this.backingPage.header, this.header, 0, this.backingPage.header_len);
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

        public static explicit operator libogg.ogg_page(OggPage page)
        {
            var liboggPage = new libogg.ogg_page
            {
                body_len = page.Body.Length,
                body = Marshal.AllocHGlobal(page.Body.Length),
                header_len = page.Header.Length,
                header = Marshal.AllocHGlobal(page.Header.Length)
            };

            Marshal.Copy(page.Body, 0, liboggPage.body, page.Body.Length);
            Marshal.Copy(page.Header, 0, liboggPage.header, page.Header.Length);

            return liboggPage;
        }
    }
}