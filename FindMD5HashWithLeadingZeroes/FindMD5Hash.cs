using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace FindMD5HashWithLeadingZeroes
{
    public class FindMD5Hash
    {
        //static void Main(string[] args)
        //{
        //    string input = "ckczppom";
        //    ConvertToMD5Hash(input);
        //}       
        //const string INPUT = "They are deterministic";

        static int[] S = new int[64] {
            7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
        };

        /*
         * Constant K Values
         */
        static uint[] K = new uint[64] {
            0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
            0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
            0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
            0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
            0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
            0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
            0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
            0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
            0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
            0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
            0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
            0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
            0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
            0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
            0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
            0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
        };

        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            // Run your code here
            var input = "ckczppom";
            var md5Hash = new FindMD5Hash();
            var result = md5Hash.SearchForRightNumber(input);
            Console.WriteLine(result);
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            Console.WriteLine(elapsed.Seconds);
        }

        public long SearchForRightNumber(string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var number = 0;

            while (true)
            {
                var numberBytes = Encoding.ASCII.GetBytes(number.ToString());
                var data = new byte[numberBytes.Length + inputBytes.Length];
                Array.Copy(inputBytes, data, inputBytes.Length);
                Array.Copy(numberBytes, 0, data, inputBytes.Length, numberBytes.Length);
                //var inputWithNumber = new StringBuilder(input);
                //inputWithNumber.Append(number);
                //var data = ConvertToByteArray(inputWithNumber.ToString(), Encoding.ASCII);
                var paddedData = AddPadding(data);
                var md5HashResult = ProcessChunks(paddedData);
                var fiveZeroes = "000000";
                //var firstFiveCharacters = md5HashResult.ToString().Substring(0, 5);

                if (md5HashResult.StartsWith(fiveZeroes))
                {
                    break;
                }

                number++;
            }

            return number;
        }

        public byte[] AddPadding(byte[] data)
        {
            bool isDivisible;
            var lengthOfData = data.Length;

            //The target = (length - 64) that has to be reached by padding with zeroes 
            var targetLength = 0;

            while (isDivisible = ((targetLength + lengthOfData * 8) + 64) % 512 != 0)
            {
                targetLength++;
            }


            var paddedData = new byte[lengthOfData + targetLength / 8 + 8];
            Array.Copy(data, paddedData, lengthOfData);
            paddedData[lengthOfData] = 0x80;
            var dataLengthInBits = BitConverter.GetBytes(data.Length * 8);
            //dataLengthInBits = dataLengthInBits.Reverse().ToArray();
            var remainingZeroes = 8 - dataLengthInBits.Length;
            Array.Copy(dataLengthInBits, 0, paddedData, paddedData.Length - 8, 4);

            return paddedData;
        }

        public string ProcessChunks(byte[] paddedData)
        {
            uint a0 = 0x67452301;   // A
            uint b0 = 0xefcdab89;   // B
            uint c0 = 0x98badcfe;   // C
            uint d0 = 0x10325476;   // D

            var chunksOf512Bits = BreakIntoChunks(paddedData, 512);
            for (int i = 0; i < chunksOf512Bits.Count(); ++i)
            {
                uint a1 = a0;
                uint b1 = b0;
                uint c1 = c0;
                uint d1 = d0;

                var chunksOf32Bits = BreakIntoChunks(chunksOf512Bits[i], 32);
                for (uint index = 0; index < 64; ++index)
                {
                    uint f = 0;
                    uint order = 0;

                    if (index >= 0 && index <= 15)
                    {
                        f = (b1 & c1) | ((~b1) & d1);
                        order = index;
                    }


                    if (index >= 16 && index <= 31)
                    {
                        f = (d1 & b1) | ((~d1) & c1);
                        order = (5 * index + 1) % 16;
                    }


                    if (index >= 32 && index <= 47)
                    {
                        f = b1 ^ c1 ^ d1;
                        order = (3 * index + 5) % 16;
                    }


                    if (index >= 48 && index <= 63)
                    {
                        f = c1 ^ (b1 | (~d1));
                        order = (7 * index) % 16;
                    }

                    var tempData = d1;
                    var numberFromChunk = BitConverter.ToUInt32(chunksOf32Bits[order]);
                    d1 = c1;
                    c1 = b1;
                    b1 = b1 + LeftRotate((a1 + f + numberFromChunk + K[index]), S[index]);
                    a1 = tempData;
                }

                a0 += a1;
                b0 += b1;
                c0 += c1;
                d0 += d1;
            }

            var result = new StringBuilder(GetByteString(a0));
            result.Append(GetByteString(b0));
            result.Append(GetByteString(c0));
            result.Append(GetByteString(d0));
            return result.ToString();
        }

        private static string GetByteString(uint x)
        {
            return String.Join("", BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
        }

        //public void InitializeK(uint[] k)
        //{
        //    for (int i = 0; i < 64; i++)
        //    {
        //        k[i] = (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
        //    }
        //}

        public static uint LeftRotate(uint x, int c)
        {
            return (x << c) | (x >> (32 - c));
        }

        public byte[][] BreakIntoChunks(byte[] paddedData, int typeOfChunks)
        {
            if (typeOfChunks == 512)
            {
                int numberOfChunks = (int)Math.Floor((decimal)((paddedData.Length * 8) / 512));
                var chunks = new byte[numberOfChunks][];
                if (paddedData.Length > 64)
                {

                    for (int i = 0; i < numberOfChunks; i++)
                    {
                        chunks[i] = new byte[64];
                        //Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
                        Array.Copy(paddedData, i * 64, chunks[i], 0, paddedData.Length - i * 64);
                    }
                }

                else
                {
                    chunks[0] = new byte[64];
                    Array.Copy(paddedData, chunks[0], paddedData.Length);
                }

                return chunks;
            }

            else
            {
                var chunks = new byte[16][];

                for (int j = 0; j < 16; j++)
                {
                    chunks[j] = new byte[4];
                    Array.Copy(paddedData, j * 4, chunks[j], 0, 4);
                }

                return chunks;
            }
        }

        //public StringBuilder DecimalToBase(int number, int toBase)
        //{
        //    var binary = new List<int>();

        //    while (number != 0)
        //    {
        //        binary.Add(number % toBase);
        //        number = number / toBase;
        //    }

        //    binary.Reverse();

        //    while ((toBase == 2) && (binary.Count % 4 != 0))
        //    {
        //        binary.Insert(0, 0);
        //    }

        //    var stringBuilder = ConvertToStringBuilder(binary);
        //    return stringBuilder;
        //}

        //public uint BinaryToDecimal(string binary)
        //{
        //    uint converted = 0;
        //    var reversed = binary.Reverse().ToArray();

        //    for (int i = 0; i < reversed.Count(); i++)
        //    {
        //        //var x = BigInteger.Parse(reversed[i].ToString());
        //        //converted += BigInteger.Multiply(BigInteger.Pow(2, i), BigInteger.Parse(reversed[i].ToString()));
        //        converted += (uint.Parse(reversed[i].ToString()) * (uint)Math.Pow(2, i));

        //    }

        //    return converted;
        //}

        //public string DecimalToHexadecimal(uint numberInDecimal)
        //{
        //    var digits = new List<uint>();

        //    while (numberInDecimal > 0)
        //    {
        //        uint digit = numberInDecimal % 16;
        //        digits.Add(digit);
        //        numberInDecimal /= 16;
        //    }

        //    digits.Reverse();
        //    var hexadecimal = new StringBuilder();

        //    foreach (var digit in digits)
        //    {
        //        if (digit < 10)
        //            hexadecimal.Append(digit);
        //        else
        //        {
        //            switch (digit)
        //            {
        //                case 10:
        //                    hexadecimal.Append("a");
        //                    break;
        //                case 11:
        //                    hexadecimal.Append("b");
        //                    break;
        //                case 12:
        //                    hexadecimal.Append("c");
        //                    break;
        //                case 13:
        //                    hexadecimal.Append("d");
        //                    break;
        //                case 14:
        //                    hexadecimal.Append("e");
        //                    break;
        //                case 15:
        //                    hexadecimal.Append("f");
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //    }

        //    return hexadecimal.ToString();
        //}

        //public StringBuilder ConvertToStringBuilder(List<int> binary)
        //{
        //    var stringBuilder = new StringBuilder();

        //    foreach (var bit in binary)
        //    {
        //        stringBuilder.Append(bit.ToString());
        //    }

        //    return stringBuilder;
        //}



        public byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        //public String ToBinary(Byte[] data)
        //{
        //    return string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        //}
    }
}

