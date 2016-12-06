namespace Villermen.LiboggSharp
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new OggReader(File.OpenRead("sample.ogg")))
            using (var writer = new OggWriter(File.OpenWrite("sample-written.ogg")))
            {
                while (true)
                {
                    var page = reader.ReadPage();

                    writer.Write(page);

                    if (page.IsEndOfStream)
                    {
                        break;
                    }
                }

                writer.Flush();
            }
        }
    }
}
