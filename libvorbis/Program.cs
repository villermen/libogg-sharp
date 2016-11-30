namespace libvorbis
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    class Program
    {
        static void Main(string[] args)
        {
            libogg.ogg_sync_state syncState = new libogg.ogg_sync_state();
            var result = libogg.ogg_sync_init(ref syncState);

            if (result != 0)
            {
                throw new Exception("Could not init sync state.");
            }

            var oggPage = new libogg.ogg_page();

            // Read into the buffer
            using (var sampleFileReader = new BinaryReader(File.OpenRead("sample.ogg")))
            {
                while (libogg.ogg_sync_pageout(ref syncState, ref oggPage) != 1)
                {
                    var bufferPtr = libogg.ogg_sync_buffer(ref syncState, 4096);

                    var readBytes = sampleFileReader.ReadBytes(4096);

                    if (readBytes.Length == 0)
                    {
                        break;
                    }

                    Marshal.Copy(readBytes, 0, bufferPtr, readBytes.Length);

                    result = libogg.ogg_sync_wrote(ref syncState, readBytes.Length);

                    if (result != 0)
                    {
                        throw new Exception("Error while calling ogg_sync_wrote");
                    }
                }

                var header = new byte[oggPage.header_len];
                Marshal.Copy(oggPage.header, header, 0, oggPage.header_len);

                var body = new byte[oggPage.body_len];
                Marshal.Copy(oggPage.body, body, 0, oggPage.body_len);
            }
        }
    }
}
