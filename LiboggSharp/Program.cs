namespace Villermen.LiboggSharp
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new OggReader(File.OpenRead("sample.ogg")))
            using (var writer = new OggWriter(File.Open("sample-output.ogg", FileMode.Create), -1502894660))
            {
                var i = 0;
                while (true)
                {
                    var packet = reader.ReadPacket();

                    writer.Write(packet);

                    if (packet.IsEndOfStream)
                    {
                        break;
                    }

                    i++;
                }
            }
        }
    }
}
