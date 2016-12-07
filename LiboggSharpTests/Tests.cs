namespace Villermen.LiboggSharpTests
{
    using System;
    using System.IO;
    using Villermen.LiboggSharp;
    using Xunit;

    public class Tests : IDisposable
    {
        private readonly OggReader reader;

        private readonly OggWriter writer;

        public Tests()
        {
            this.reader = new OggReader(File.Open("sample.ogg", FileMode.Open, FileAccess.Read, FileShare.Read));
            this.writer = new OggWriter(File.Open("sample-out.ogg", FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        [Fact]
        public void TestTransferPackets()
        {
            // Identification header
            this.writer.Write(this.reader.ReadPacket());
            this.writer.Flush(true);

            // Comment and setup headers
            this.writer.Write(this.reader.ReadPacket());
            this.writer.Write(this.reader.ReadPacket());
            this.writer.Flush(true);

            var processedPackets = 3;

            while (true)
            {
                var packet = this.reader.ReadPacket();

                this.writer.Write(packet);
                if (packet.IsEndOfStream)
                {
                    break;
                }

                processedPackets++;
            }

            this.writer.Flush();

            Assert.Equal(10717, processedPackets);
        }

        public void Dispose()
        {
            this.reader.Dispose();
            this.writer.Dispose();
        }
    }
}
