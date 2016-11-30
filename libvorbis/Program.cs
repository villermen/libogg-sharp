namespace libvorbis
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new OggReader(File.OpenRead("sample.ogg")))
            {
                while (true)
                {
                    var page = reader.ReadPage();

                    if (page == null)
                    {
                        break;
                    }
                }
            }
        }
    }
}
