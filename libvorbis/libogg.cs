// ReSharper disable All

namespace libvorbis
{
    using System;
    using System.Runtime.InteropServices;

    public static class libogg
    {
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ogg_sync_buffer")]
        public static extern IntPtr ogg_sync_buffer(ref ogg_sync_state oy, long size);


        // [StructLayout(LayoutKind.Sequential)]
        public struct ogg_sync_state
        {
            public IntPtr data;
            public int storage;
            public int fill;
            public int returned;

            public int unsynced;
            public int headerbytes;
            public int bodybytes;
        }
    }
}
