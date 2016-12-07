namespace Villermen.LiboggSharpTests
{
    using System;
    using System.IO;
    using System.Linq;
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
            this.writer.Write(this.reader.First(), true);

            // Comment and setup headers
            this.writer.Write(this.reader.Take(2), true);

            var processedPackets = 3;

            foreach (var packet in this.reader)
            {
                this.writer.Write(packet);

                processedPackets++;
            }

            Assert.Equal(10718, processedPackets);
        }

        public void Dispose()
        {
            this.reader.Dispose();
            this.writer.Dispose();
        }
    }
}
