// ReSharper disable All

// TODO: Add documentation to undocumented methods


namespace Villermen.LiboggSharp
{
    using System;
    using System.Runtime.InteropServices;

    public static class libogg
    {
        #region Structures

        public struct ogg_int64_t
        {
            private int value;

            private ogg_int64_t(int val)
            {
                value = val;
            }

            public static implicit operator ogg_int64_t(int val)
            {
                return new ogg_int64_t(val);
            }

            public static implicit operator int(ogg_int64_t val)
            {
                return val.value;
            }
        }

        /* TODO: Implement remaining typedefs
         * typedef __int32 ogg_int32_t;
         * typedef unsigned __int32 ogg_uint32_t;
         * typedef __int16 ogg_int16_t;
         * typedef unsigned __int16 ogg_uint16_t;
         */

        /// <summary>
        /// The <see cref="oggpack_buffer"/> struct is used with libogg's bitpacking functions. You should never need to directly access anything in this structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct oggpack_buffer
        {
            public int endbyte;

            public int endbit;

            /// <summary>
            /// Pointer to data being manipulated.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr buffer;

            /// <summary>
            /// Location pointer to mark which data has been read.
            /// 
            /// byte*
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Size of buffer.
            /// </summary>
            public int storage;
        }

        /// <summary>
        /// The ogg_page struct encapsulates the data for an Ogg page.
        /// 
        /// Ogg pages are the fundamental unit of framing and interleave in an ogg bitstream.
        /// They are made up of packet segments of 255 bytes each.
        /// There can be as many as 255 packet segments per page, for a maximum page size of a little under 64 kB.
        /// This is not a practical limitation as the segments can be joined across page boundaries allowing packets of arbitrary size.
        /// In practice many applications will not completely fill all pages because they flush the accumulated packets periodically order to bound latency more tightly.
        /// 
        /// For a complete description of ogg pages and headers, please refer to the <a href="https://www.xiph.org/ogg/doc/framing.html">framing document</a>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_page
        {
            /// <summary>
            /// Pointer to the page header for this page.
            /// The exact contents of this header are defined in the framing spec document.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr header;

            /// <summary>
            /// Length of the page header in bytes.
            /// </summary>
            public int header_len;

            /// <summary>
            /// Pointer to the data for this page.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr body;

            /// <summary>
            /// Length of the body data in bytes.
            /// </summary>
            public int body_len;
        }

        /// <summary>
        /// The <see cref="ogg_stream_state"/> struct tracks the current encode/decode state of the current logical bitstream.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_stream_state
        {
            /// <summary>
            /// Pointer to data from packet bodies.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr body_data;

            /// <summary>
            /// Storage allocated for bodies in bytes (filled or unfilled).
            /// </summary>
            public int body_storage;

            /// <summary>
            /// Amount of storage filled with stored packet bodies.
            /// </summary>
            public int body_fill;

            /// <summary>
            /// Number of elements returned from storage.
            /// </summary>
            public int body_returned;

            /// <summary>
            /// String of lacing values for the packet segments within the current page.
            /// Each value is a byte, indicating packet segment length.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr lacing_vals;

            /// <summary>
            /// Pointer to the lacing values for the packet segments within the current page.
            /// 
            /// ogg_int64_t[]*
            /// </summary>
            public IntPtr granule_vals;

            /// <summary>
            /// Total amount of storage (in bytes) allocated for storing lacing values.
            /// </summary>
            public int lacing_storage;

            /// <summary>
            /// Fill marker for the current vs. total allocated storage of lacing values for the page.
            /// </summary>
            public int lacing_fill;

            /// <summary>
            /// Lacing value for current packet segment.
            /// </summary>
            public int lacing_packet;

            /// <summary>
            /// Number of lacing values returned from <see cref="lacing_storage"/>.
            /// </summary>
            public int lacing_returned;

            /// <summary>
            /// Temporary storage for page header during encode process, while the header is being created.
            /// 
            /// byte[282]
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 282)]
            public byte[] header;

            /// <summary>
            /// Fill marker for header storage allocation. Used during the header creation process.
            /// </summary>
            public int header_fill;

            /// <summary>
            /// Marker set when the last packet of the logical bitstream has been buffered.
            /// </summary>
            public int e_o_s;

            /// <summary>
            /// Marker set after we have written the first page in the logical bitstream.
            /// </summary>
            public int b_o_s;

            /// <summary>
            /// Serial number of this logical bitstream.
            /// </summary>
            public int serialno;

            /// <summary>
            /// Number of the current page within the stream.
            /// </summary>
            public int pageno;

            /// <summary>
            /// Number of the current packet.
            /// </summary>
            public ogg_int64_t packetno;

            /// <summary>
            /// Exact position of decoding/encoding process.
            /// 
            /// ogg_int64_t*
            /// </summary>
            public ogg_int64_t granulepos;
        }

        /// <summary>
        /// The ogg_packet struct encapsulates the data for a single raw packet of data and is used to transfer data between the ogg framing layer and the handling codec.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_packet
        {
            /// <summary>
            /// Pointer to the packet's data.
            /// This is treated as an opaque type by the ogg layer.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr packet;

            /// <summary>
            /// Indicates the size of the packet data in bytes.
            /// Packets can be of arbitrary size.
            /// </summary>
            public int bytes;

            /// <summary>
            /// Flag indicating whether this packet begins a logical bitstream.
            /// 1 indicates this is the first packet, 0 indicates any other position in the stream.
            /// </summary>
            public int b_o_s;

            /// <summary>
            /// Flag indicating whether this packet ends a bitstream.
            /// 1 indicates the last packet, 0 indicates any other position in the stream.
            /// </summary>
            public int e_o_s;

            /// <summary>
            /// A number indicating the position of this packet in the decoded data.
            /// This is the last sample, frame or other unit of information ('granule') that can be completely decoded from this packet.
            /// </summary>
            public ogg_int64_t ganulepos;

            /// <summary>
            /// Sequential number of this packet in the ogg bitstream.
            /// </summary>
            public ogg_int64_t packetno;
        }

        /// <summary>
        /// The <see cref="ogg_sync_state"/> struct tracks the synchronization of the current page.
        /// It is used during decoding to track the status of data as it is read in, synchronized, verified, and parsed into pages beinting to the various logical bistreams in the current physical bitstream link.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_sync_state
        {
            /// <summary>
            /// Pointer to buffered stream data.
            /// 
            /// byte[]*
            /// </summary>
            public IntPtr data;

            /// <summary>
            /// Current allocated size of the stream buffer held in *data.
            /// </summary>
            public int storage;

            /// <summary>
            /// The number of valid bytes currently held in *data; functions as the buffer head pointer.
            /// </summary>
            public int fill;

            /// <summary>
            /// The number of bytes at the head of *data that have already been returned as pages; functions as the buffer tail pointer.
            /// </summary>
            public int returned;

            /// <summary>
            /// Synchronization state flag; nonzero if sync has not yet been attained or has been lost.
            /// </summary>
            public int unsynced;

            /// <summary>
            /// If synced, the number of bytes used by the synced page's header.
            /// </summary>
            public int headerbytes;

            /// <summary>
            /// If synced, the number of bytes used by the synced page's body.
            /// </summary>
            public int bodybytes;
        }

        #endregion

        #region Bitpacking

        /// <summary>
        /// This function initializes an <see cref="oggpack_buffer"/> for writing using the Ogg bitpacking functions. 
        /// </summary>
        /// <param name="b">
        /// Buffer to be used for writing.
        /// This is an ordinary data buffer with some extra markers to ease bit navigation and manipulation.
        /// </param>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writeinit")]
        public static extern void oggpack_writeinit(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writecheck")]
        public static extern int oggpack_writecheck(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_reset")]
        public static extern void oggpack_reset(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writetrunc")]
        public static extern void oggpack_writetrunc(ref oggpack_buffer b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writealign")]
        public static extern void oggpack_writealign(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writecopy")]
        public static extern void oggpack_writecopy(ref oggpack_buffer b, IntPtr source, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writeclear")]
        public static extern void oggpack_writeclear(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_readinit")]
        public static extern void oggpack_readinit(ref oggpack_buffer b, IntPtr buffer, int bytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_write")]
        public static extern void oggpack_write(ref oggpack_buffer b, uint value, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_look")]
        public static extern int oggpack_look(ref oggpack_buffer b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_look1")]
        public static extern int oggpack_look1(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_adv")]
        public static extern void oggpack_adv(ref oggpack_buffer b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_adv1")]
        public static extern void oggpack_adv1(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_read")]
        public static extern int oggpack_read(ref oggpack_buffer b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_read1")]
        public static extern int oggpack_read1(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_bytes")]
        public static extern int oggpack_bytes(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_bits")]
        public static extern int oggpack_bits(ref oggpack_buffer b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_get_buffer")]
        public static extern IntPtr oggpack_get_buffer(ref oggpack_buffer b);

        #endregion

        #region Decoding

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_init")]
        public static extern int ogg_sync_init(ref ogg_sync_state oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_check")]
        public static extern int ogg_sync_check(ref ogg_sync_state oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_clear")]
        public static extern int ogg_sync_clear(ref ogg_sync_state oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_destroy")]
        public static extern int ogg_sync_destroy(ref ogg_sync_state oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_reset")]
        public static extern int ogg_sync_reset(ref ogg_sync_state oy);

        /// <summary>
        /// This function is used to provide a properly-sized buffer for writing.
        /// 
        /// Buffer space which has already been returned is cleared, and the buffer is extended as necessary by the size plus some additional bytes.Within the current implementation, an extra 4096 bytes are allocated, but applications should not rely on this additional buffer space.
        /// The buffer exposed by this function is empty internal storage from the ogg_sync_state struct, beginning at the fill mark within the struct.
        /// A pointer to this buffer is returned to be used by the calling application. 
        /// </summary>
        /// <param name="oy">Pointer to a previously declared <see cref="ogg_sync_state"/> struct.</param>
        /// <param name="size">Size of the desired buffer. The actual size of the buffer returned will be this size plus some extra bytes (currently 4096).</param>
        /// <returns>Returns a pointer to the newly allocated buffer or NULL on error</returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_buffer")]
        public static extern IntPtr ogg_sync_buffer(ref ogg_sync_state oy, int size);

        /// <summary>
        /// This function is used to tell the <see cref="ogg_sync_state"/> struct how many bytes we wrote into the buffer.
        /// 
        /// The general proceedure is to request a pointer into an internal ogg_sync_state buffer by calling <see cref="ogg_sync_buffer"/>.
        /// The buffer is then filled up to the requested size with new input, and <see cref="ogg_sync_wrote"/> is called to advance the fill pointer by however much data was actually available.
        /// </summary>
        /// <param name="oy">Pointer to a previously declared <see cref="ogg_sync_state"/> struct.</param>
        /// <param name="bytes">Number of bytes of new data written.</param>
        /// <returns>
        /// -1 if the number of bytes written overflows the internal storage of the ogg_sync_state struct or an internal error occurred.
        /// 0 in all other cases.
        /// </returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_wrote")]
        public static extern int ogg_sync_wrote(ref ogg_sync_state oy, int bytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_pageseek")]
        public static extern int ogg_sync_pageseek(ref ogg_sync_state oy, ref ogg_page og);

        /// <summary>
        /// This function takes the data stored in the buffer of the ogg_sync_state struct and inserts them into an ogg_page.
        /// 
        /// In an actual decoding loop, this function should be called first to ensure that the buffer is cleared.
        /// The example code below illustrates a clean reading loop which will fill and output pages.
        /// 
        /// <strong>Caution</strong>: This function should be called before reading into the buffer to ensure that data does not remain in the <see cref="ogg_sync_state"/> struct.
        /// Failing to do so may result in a memory leak.See the example code below for details.
        /// </summary>
        /// <param name="oy">
        /// Pointer to a previously declared <see cref="ogg_sync_state"/> struct.
        /// Normally, the internal storage of this struct should be filled with newly read data and verified using <see cref="ogg_sync_wrote"/>.
        /// </param>
        /// <param name="og">Pointer to a page struct filled by this function.</param>
        /// <returns>
        /// -1 returned if stream has not yet captured sync (bytes were skipped).
        /// 0 returned if more data needed or an internal error occurred.
        /// 1 indicated a page was synced and returned.
        /// </returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_pageout")]
        public static extern int ogg_sync_pageout(ref ogg_sync_state oy, ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pagein")]
        public static extern int ogg_stream_pagein(ref ogg_stream_state os, ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetout")]
        public static extern int ogg_stream_packetout(ref ogg_stream_state os, ref ogg_packet op);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetpeek")]
        public static extern int ogg_stream_packetpeek(ref ogg_stream_state os, ref ogg_packet op);

        #endregion

        #region Encoding

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetin")]
        public static extern int ogg_stream_packetin(ref ogg_stream_state os, ref ogg_packet op);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pageout")]
        public static extern int ogg_stream_pageout(ref ogg_stream_state os, ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pageout_fill")]
        public static extern int ogg_stream_pageout_fill(ref ogg_stream_state os, ref ogg_page og, int fillbytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_flush")]
        public static extern int ogg_stream_flush(ref ogg_stream_state os, ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_flush_fill")]
        public static extern int ogg_stream_flush_fill(ref ogg_stream_state os, ref ogg_page og, int fillbytes);

        #endregion

        #region General

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_init")]
        public static extern int ogg_stream_init(ref ogg_stream_state os, int serialno);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_check")]
        public static extern int ogg_stream_check(ref ogg_stream_state os);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_clear")]
        public static extern int ogg_stream_clear(ref ogg_stream_state os);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_reset")]
        public static extern int ogg_stream_reset(ref ogg_stream_state os);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_reset_serialno")]
        public static extern int ogg_stream_reset_serialno(ref ogg_stream_state os, int serialno);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_destroy")]
        public static extern int ogg_stream_destroy(ref ogg_stream_state os);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_version")]
        public static extern int ogg_page_version(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_continued")]
        public static extern int ogg_page_continued(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_packets")]
        public static extern int ogg_page_packets(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_bos")]
        public static extern int ogg_page_bos(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_eos")]
        public static extern int ogg_page_eos(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_granulepos")]
        public static extern ogg_int64_t ogg_page_granulepos(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_serialno")]
        public static extern int ogg_page_serialno(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_pageno")]
        public static extern int ogg_page_pageno(ref ogg_page og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_packet_clear")]
        public static extern void ogg_packet_clear(ref ogg_packet op);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_page_checksum_set")]
        public static extern int ogg_page_checksum_set(ref ogg_page og);

        #endregion
    }
}
