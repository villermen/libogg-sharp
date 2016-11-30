// ReSharper disable All

namespace libvorbis
{
    using System;
    using System.Runtime.InteropServices;

    public static class libogg
    {
        #region Structures

        public struct ogg_int64_t
        {
            private long value;

            private ogg_int64_t(long val)
            {
                value = val;
            }

            public static implicit operator ogg_int64_t(long val)
            {
                return new ogg_int64_t(val);
            }

            public static implicit operator long(ogg_int64_t val)
            {
                return val.value;
            }
        }

         //typedef __int32 ogg_int32_t;
         //typedef unsigned __int32 ogg_uint32_t;
         //   typedef __int16 ogg_int16_t;
         //typedef unsigned __int16 ogg_uint16_t;

        /// <summary>
        /// The <see cref="oggpack_buffer"/> struct is used with libogg's bitpacking functions. You should never need to directly access anything in this structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct oggpack_buffer
        {
            public long endbyte;

            public int endbit;

            /// <summary>
            /// Pointer to data being manipulated.
            /// </summary>
            public unsafe byte* buffer;

            /// <summary>
            /// Location pointer to mark which data has been read.
            /// </summary>
            public unsafe byte* ptr;

            /// <summary>
            /// Size of buffer.
            /// </summary>
            public long storage;
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
            /// Pointer to the page header for this page. The exact contents of this header are defined in the framing spec document.
            /// </summary>
            public unsafe byte* header;

            /// <summary>
            /// Length of the page header in bytes.
            /// </summary>
            public long header_len;

            /// <summary>
            /// Pointer to the data for this page.
            /// </summary>
            public unsafe byte* body;

            /// <summary>
            /// Length of the body data in bytes.
            /// </summary>
            public long body_len;
        }

        /// <summary>
        /// The <see cref="ogg_stream_state"/> struct tracks the current encode/decode state of the current logical bitstream.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_stream_state
        {
            /// <summary>
            /// Pointer to data from packet bodies.
            /// </summary>
            public IntPtr body_data;

            /// <summary>
            /// Storage allocated for bodies in bytes (filled or unfilled).
            /// </summary>
            public long body_storage;

            /// <summary>
            /// Amount of storage filled with stored packet bodies.
            /// </summary>
            public long body_fill;

            /// <summary>
            /// Number of elements returned from storage.
            /// </summary>
            public long body_returned;

            /// <summary>
            /// String of lacing values for the packet segments within the current page.
            /// Each value is a byte, indicating packet segment length.
            /// </summary>
            public unsafe byte* lacing_vals;

            /// <summary>
            /// Pointer to the lacing values for the packet segments within the current page.
            /// </summary>
            public unsafe ogg_int64_t* granule_vals;

            /// <summary>
            /// Total amount of storage (in bytes) allocated for storing lacing values.
            /// </summary>
            public long lacing_storage;

            /// <summary>
            /// Fill marker for the current vs. total allocated storage of lacing values for the page.
            /// </summary>
            public long lacing_fill;

            /// <summary>
            /// Lacing value for current packet segment.
            /// </summary>
            public long lacing_packet;

            /// <summary>
            /// Number of lacing values returned from <see cref="lacing_storage"/>.
            /// </summary>
            public long lacing_returned;

            /// <summary>
            /// Temporary storage for page header during encode process, while the header is being created.
            /// </summary>
            public unsafe fixed byte header[282];

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
            public long serialno;

            /// <summary>
            /// Number of the current page within the stream.
            /// </summary>
            public int pageno;

            /// <summary>
            /// Number of the current packet.
            /// </summary>
            public unsafe ogg_int64_t* packetno;

            /// <summary>
            /// Exact position of decoding/encoding process.
            /// </summary>
            public unsafe ogg_int64_t* granulepos;
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
            /// </summary>
            public unsafe byte* packet;

            /// <summary>
            /// Indicates the size of the packet data in bytes.
            /// Packets can be of arbitrary size.
            /// </summary>
            public long bytes;

            /// <summary>
            /// Flag indicating whether this packet begins a logical bitstream.
            /// 1 indicates this is the first packet, 0 indicates any other position in the stream.
            /// </summary>
            public long b_o_s;

            /// <summary>
            /// Flag indicating whether this packet ends a bitstream.
            /// 1 indicates the last packet, 0 indicates any other position in the stream.
            /// </summary>
            public long e_o_s;

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
        /// It is used during decoding to track the status of data as it is read in, synchronized, verified, and parsed into pages belonging to the various logical bistreams in the current physical bitstream link.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ogg_sync_state
        {
            /// <summary>
            /// Pointer to buffered stream data.
            /// </summary>
            public unsafe byte* data;

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
        public static unsafe extern void oggpack_writeinit(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writecheck")]
        public static unsafe extern int oggpack_writecheck(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_reset")]
        public static unsafe extern void oggpack_reset(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writetrunc")]
        public static unsafe extern void oggpack_writetrunc(oggpack_buffer* b, long bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writealign")]
        public static unsafe extern void oggpack_writealign(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writecopy")]
        public static unsafe extern void oggpack_writecopy(oggpack_buffer* b, void* source, long bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_writeclear")]
        public static unsafe extern void oggpack_writeclear(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_readinit")]
        public static unsafe extern void oggpack_readinit(oggpack_buffer* b, byte* buffer, int bytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_write")]
        public static unsafe extern void oggpack_write(oggpack_buffer* b, ulong value, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_look")]
        public static unsafe extern long oggpack_look(oggpack_buffer* b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_look1")]
        public static unsafe extern long oggpack_look1(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_adv")]
        public static unsafe extern void oggpack_adv(oggpack_buffer* b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_adv1")]
        public static unsafe extern void oggpack_adv1(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_read")]
        public static unsafe extern long oggpack_read(oggpack_buffer* b, int bits);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_read1")]
        public static unsafe extern long oggpack_read1(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_bytes")]
        public static unsafe extern long oggpack_bytes(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_bits")]
        public static unsafe extern long oggpack_bits(oggpack_buffer* b);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "oggpack_get_buffer")]
        public static unsafe extern byte* oggpack_get_buffer(oggpack_buffer* b);

        #endregion

        #region Decoding

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_init")]
        public static extern unsafe int ogg_sync_init(ogg_sync_state* oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_check")]
        public static extern unsafe int ogg_sync_check(ogg_sync_state* oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_clear")]
        public static extern unsafe int ogg_sync_clear(ogg_sync_state* oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_destroy")]
        public static extern unsafe int ogg_sync_destroy(ogg_sync_state* oy);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_reset")]
        public static extern unsafe int ogg_sync_reset(ogg_sync_state* oy);

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
        public static extern IntPtr ogg_sync_buffer(ref ogg_sync_state oy, long size);

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
        public static extern int ogg_sync_wrote(ref ogg_sync_state oy, long bytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_pageseek")]
        public static extern unsafe int ogg_sync_pageseek(ogg_sync_state* oy, ogg_page* og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_pageout")]
        public static extern unsafe int ogg_sync_pageout(ogg_sync_state* oy, ogg_page* og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pagein")]
        public static extern unsafe int ogg_stream_pagein(ogg_stream_state* os, ogg_page* og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetout")]
        public static extern unsafe int ogg_stream_packetout(ogg_stream_state* os, ogg_packet* op);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetpeek")]
        public static extern unsafe int ogg_stream_packetpeek(ogg_stream_state* os, ogg_packet* op);

        #endregion

        #region Encoding

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_packetin")]
        public static extern unsafe int ogg_stream_packetin(ogg_stream_state* os, ogg_packet* op);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pageout")]
        public static extern unsafe int ogg_stream_pageout(ogg_stream_state* os, ogg_page* og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_pageout_fill")]
        public static extern unsafe int ogg_stream_pageout_fill(ogg_stream_state* os, ogg_page* og, int fillbytes);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_flush")]
        public static extern unsafe int ogg_stream_flush(ogg_stream_state* os, ogg_page* og);

        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_stream_flush_fill")]
        public static extern unsafe int ogg_stream_flush_fill(ogg_stream_state* os, ogg_page* og, int fillbytes);

        #endregion

        #region General

        #endregion
    }
}
