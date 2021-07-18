using System;
using System.IO;

using BenchmarkDotNet.Attributes;

using MetadataExtractor.IO;

namespace MetadataExtractor.Benchmarks
{
    [MemoryDiagnoser]
    public class RASBenchmark
    {
        private readonly MemoryStream _stream;

        private readonly ReaderInfo _reader;
        public RASBenchmark()
        {
            _stream = new MemoryStream();

            // This is the largest JPEG file in this repository
            using var fs = File.OpenRead("../../../../MetadataExtractor.Tests/Data/nikonMakernoteType2b.jpg");
            fs.CopyTo(_stream);

            _reader = new RandomAccessStream(_stream).CreateReader();
        }        

        [Benchmark(Baseline = true)]
        public void RASListBenchmark()
        {
            _stream.Position = 0;

            //var reader = new RandomAccessStream(_stream).CreateReader();
            RunReader(_reader);
        }


        /*
        [Benchmark]]
        public void RASDictionaryBenchmark()
        {
            _stream.Position = 0;

            var reader = new RandomAccessStreamDictionary(_stream).CreateReader();
            RunReader(reader);
        }

        [Benchmark]
        public void RASDictionaryNonseekableBenchmark()
        {
            _stream.Position = 0;

            var reader = new RandomAccessStreamDictionary(new NonSeekableStream(_stream)).CreateReader();
            RunReader(reader);
        }

        [Benchmark]
        public void RASListNonseekableBenchmark()
        {
            _stream.Position = 0;

            var reader = new RandomAccessStream(new NonSeekableStream(_stream)).CreateReader();
            RunReader(reader);
        }
        */
        

        [Benchmark]
        public void IndexedCapturingReaderBenchmark()
        {
            _stream.Position = 0;

            var reader = new IndexedCapturingReader(_stream, 4096);
            RunIndexedReader(reader);
        }

        [Benchmark]
        public void IndexedSeekingReaderBenchmark()
        {
            _stream.Position = 0;

            var reader = new IndexedSeekingReader(_stream);
            RunIndexedReader(reader);
        }

        private void RunReader(ReaderInfo reader)
        {
            int offset = 4 * 1024 + 10; // skip over at least one buffer, just because

            // Nothing mathematical intended here other than jumping around in the file
            for (int i = 0; i < 10; i++)
            {
                var calcoffset2 = GetLongOffset(i, offset, 2);
                var calcoffset3 = GetLongOffset(i, offset, 3);

                
                reader.GetInt16(calcoffset2);
                reader.GetInt16(calcoffset3);
                
                reader.GetInt24(calcoffset2);
                reader.GetInt24(calcoffset3);
                
                reader.GetInt32(calcoffset2);
                reader.GetInt32(calcoffset3);
                
                reader.GetBytes(calcoffset2, 128);
                reader.GetBytes(calcoffset3, 128);
                
                reader.GetInt64(calcoffset2);
                reader.GetInt64(calcoffset3);
                
                for (int j = 0; j < 1000; j++)
                {
                    reader.GetByte(calcoffset2 + j);
                    reader.GetByte(calcoffset3 + j);
                }
                
                reader.GetUInt16(calcoffset2);
                reader.GetUInt16(calcoffset3);

                reader.GetUInt32(calcoffset2);
                reader.GetUInt32(calcoffset3);
                

                //reader.GetUInt64(calcoffset2);
                //reader.GetUInt64(calcoffset3);
            }
        }

        private void RunIndexedReader(IndexedReader reader)
        {
            int offset = 4 * 1024 + 10; // skip over at least one buffer, just because

            // Nothing mathematical intended here other than jumping around in the file
            for (int i = 0; i < 10; i++)
            {
                var calcoffset2 = GetIntOffset(i, offset, 2);
                var calcoffset3 = GetIntOffset(i, offset, 3);

                
                reader.GetInt16(calcoffset2);
                reader.GetInt16(calcoffset3);
                
                reader.GetInt24(calcoffset2);
                reader.GetInt24(calcoffset3);
                
                reader.GetInt32(calcoffset2);
                reader.GetInt32(calcoffset3);
                
                reader.GetBytes(calcoffset2, 128);
                reader.GetBytes(calcoffset3, 128);
                
                reader.GetInt64(calcoffset2);
                reader.GetInt64(calcoffset3);
                
                for (int j = 0; j < 1000; j++)
                {
                    reader.GetByte(calcoffset2 + j);
                    reader.GetByte(calcoffset3 + j);
                }
                
                reader.GetUInt16(calcoffset2);
                reader.GetUInt16(calcoffset3);

                reader.GetUInt32(calcoffset2);
                reader.GetUInt32(calcoffset3);
                

                //reader.GetUInt64(calcoffset2);
                //reader.GetUInt64(calcoffset3);
            }
        }

        private static long GetLongOffset(int i, long offset, int power)
        {
            return (long)(i * offset + Math.Pow(power, i));
        }

        private static int GetIntOffset(int i, long offset, int power)
        {
            return (int)(i * offset + Math.Pow(power, i));
        }
    }
}
