using System;
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
        const string INPUT = "They are deterministic";

        public static void Main(string[] args)
        {
            var md5Hash = new FindMD5Hash();
            var binaryString = md5Hash.ToBinary(md5Hash.ConvertToByteArray(INPUT, Encoding.ASCII));
            var paddedInput = md5Hash.AddPadding(binaryString);
            var hash = md5Hash.ProcessChunks(paddedInput);
            //Console.WriteLine(paddedInput);
        }

        public string AddPadding(string binaryString)
        {
            int inputCount = INPUT.Length;
            int count = binaryString.Length;
            var binaryCount = DecimalToBase(count, 2);

            var stringBuilder = new StringBuilder(binaryString);

            if (inputCount < 448)
            {
                //int remainingZeroes = 448 - 1 - count;
                ////stringBuilder.Append(" ");
                //stringBuilder.Append("1");


                //AddBytesWithZeroes(remainingZeroes, stringBuilder, false);

                stringBuilder.Append("1");
                while (true)
                {
                    bool isDivisible = (stringBuilder.Length + 64) % 512 == 0;

                    if (isDivisible == true)
                    {
                        break;
                    }

                    stringBuilder.Append("0");
                }

                if (binaryCount.Length > 64)
                {
                    while (binaryCount.Length > 64)
                    {
                        binaryCount.Remove(0, 1);
                    }

                    stringBuilder.Append(binaryCount.ToString());
                }

                else
                {
                    AddBytesWithZeroes(64 - binaryCount.Length, stringBuilder, true);
                    stringBuilder.Append(binaryCount.ToString());
                }
                //var chunks = stringBuilder.GetChunks();
            }

            return stringBuilder.ToString();
        }

        public void AddBytesWithZeroes(int remainingZeroes, StringBuilder stringBuilder, bool isPaddedWithZeroes)
        {
            for (int i = 1; i <= remainingZeroes; i++)
            {
                //The first byte when padding will contain leading 1 so it will have one less zero 
                if ((i != 8 && isPaddedWithZeroes == false) || (isPaddedWithZeroes == true))
                    stringBuilder.Append("0");

                //if (i % 8 == 0)
                //{
                //    stringBuilder.Append(" ");
                //}
            }

            //Only do this the first time when adding one in the beginning
            if (isPaddedWithZeroes == false)
            {
                //Adding zero because of the one we put in the begging and we omit 1 zero
                stringBuilder.Append("0");
                //stringBuilder.Append(" ");
            }

        }

        public string ProcessChunks(string paddedInput)
        {
            //Constants
            var k = new long[65];
            InitializeK(k);

            // s specifies the per-round shift amounts
            var s = new int[65];
            InitializeS(s);

            var index = 0;
            long a0 = 0x67452301;
            long b0 = 0xefcdab89;
            long c0 = 0x98badcfe;
            long d0 = 0x10325476;

            var chunksOf512Bits = BreakIntoChunks(paddedInput, 512);
            for (int i = 0; i < chunksOf512Bits.Count; i++)
            {
                long a1 = a0;
                long b1 = b0;
                long c1 = c0;
                long d1 = d0;
                long f = 0;

                var chunksOf32Bits = BreakIntoChunks(chunksOf512Bits[i], 32);
                for (int j = 0; j < chunksOf32Bits.Count; j++)
                {
                    int order = 0;

                    if (index >= 0 && index <= 15)
                    {
                        f = (b1 & c1) | ((~b1) & d1);
                        order = i % 16;
                    }


                    if (index >= 16 && index <= 31)
                    {
                        f = (d1 & b1) | ((~d1) & c1);
                        order = (5 * i + 1) % 16;
                    }


                    if (index >= 32 && index <= 47)
                    {
                        f = b1 ^ c1 ^ d1;
                        order = (3 * i + 5) % 16;
                    }


                    if (index >= 48 && index <= 63)
                    {
                        f = c1 ^ (b1 | (~d1));
                        order = (7 * i) % 16;
                    }

                    var combination = f + a0 + BinaryToDecimal(chunksOf32Bits[order]) + k[index];
                    var rotation = combination << s[index];
                    a1 = d0;
                    b1 = rotation + b0;
                    c1 = b0;
                    d1 = c0;

                    index++;
                }
            }

            var result = new StringBuilder(DecimalToHexadecimal(a0));
            result.Append(DecimalToHexadecimal(b0));
            result.Append(DecimalToHexadecimal(c0));
            result.Append(DecimalToHexadecimal(d0));
            //var result = DecimalToHexadecimal(a0) + DecimalToHexadecimal(b0) +
            //    DecimalToHexadecimal(c0) + DecimalToHexadecimal(d0);
            //var result = new StringBuilder();
            //result.AppendLine(a0.ToString());
            //result.AppendLine(b0.ToString());
            //result.AppendLine(c0.ToString());
            //result.AppendLine(d0.ToString());


            return result.ToString();
        }

        public void InitializeK(long[] k)
        {
            for (int i = 0; i < 64; i++)
            {
                k[i] = (long)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
            }
        }

        // s specifies the per-round shift amounts
        public void InitializeS(int[] s)
        {
            //There are 4 numbers that repeat for each round
            var indexOfNumber = 1;

            for (int i = 0; i < 64; i++)
            {
                if (indexOfNumber == 5)
                {
                    indexOfNumber = 0;
                }

                if (i >= 0 && i <= 15)
                {
                    //These 4 numbers repeat for the whole round
                    switch (indexOfNumber)
                    {
                        case 1:
                            s[i] = 7;
                            break;
                        case 2:
                            s[i] = 12;
                            break;
                        case 3:
                            s[i] = 17;
                            break;
                        case 4:
                            s[i] = 22;
                            break;
                        default:
                            break;
                    }
                }


                if (i >= 16 && i <= 31)
                {
                    //These 4 numbers repeat for the whole round
                    switch (indexOfNumber)
                    {
                        case 1:
                            s[i] = 5;
                            break;
                        case 2:
                            s[i] = 9;
                            break;
                        case 3:
                            s[i] = 14;
                            break;
                        case 4:
                            s[i] = 20;
                            break;
                        default:
                            break;
                    }
                }


                if (i >= 32 && i <= 47)
                {
                    //These 4 numbers repeat for the whole round
                    switch (indexOfNumber)
                    {
                        case 1:
                            s[i] = 4;
                            break;
                        case 2:
                            s[i] = 11;
                            break;
                        case 3:
                            s[i] = 16;
                            break;
                        case 4:
                            s[i] = 23;
                            break;
                        default:
                            break;
                    }
                }


                if (i >= 48 && i <= 63)
                {
                    //These 4 numbers repeat for the whole round
                    switch (indexOfNumber)
                    {
                        case 1:
                            s[i] = 6;
                            break;
                        case 2:
                            s[i] = 10;
                            break;
                        case 3:
                            s[i] = 15;
                            break;
                        case 4:
                            s[i] = 21;
                            break;
                        default:
                            break;
                    }
                }

                indexOfNumber++;
            }
        }

        public List<string> BreakIntoChunks(string paddedInput, int numberOfChunks)
        {
            var chunks = new List<string>();

            if (numberOfChunks == 512)
            {
                for (int i = 0; i < (paddedInput.Length % 512); i++)
                {
                    chunks.Add(paddedInput.Substring(i * 512, 512));
                }
            }

            else
            {
                for (int j = 0; j < 16; j++)
                {
                    chunks.Add(paddedInput.Substring(j * 32, 32));
                }
            }

            return chunks;
        }

        public StringBuilder DecimalToBase(int number, int toBase)
        {
            var binary = new List<int>();

            while (number != 0)
            {
                binary.Add(number % toBase);
                number = number / toBase;
            }

            binary.Reverse();

            var stringBuilder = ConvertToStringBuilder(binary);
            return stringBuilder;
        }

        public long BinaryToDecimal(string binary)
        {
            long converted = 0;
            var reversed = binary.Reverse();

            for (int i = 0; i < reversed.Count(); i++)
            {
                converted += 2 ^ i;
            }

            return converted;
        }

        public string DecimalToHexadecimal(long numberInDecimal)
        {
            var digits = new List<long>();

            while (numberInDecimal > 0)
            {
                var digit = numberInDecimal % 16;
                digits.Add(digit);
                numberInDecimal /= 16;
            }

            digits.Reverse();
            var hexadecimal = new StringBuilder();

            foreach (var digit in digits)
            {
                if (digit < 10)
                    hexadecimal.Append(digit);
                else
                {
                    switch (digit)
                    {
                        case 10:
                            hexadecimal.Append("a");
                            break;
                        case 11:
                            hexadecimal.Append("b");
                            break;
                        case 12:
                            hexadecimal.Append("c");
                            break;
                        case 13:
                            hexadecimal.Append("d");
                            break;
                        case 14:
                            hexadecimal.Append("e");
                            break;
                        case 15:
                            hexadecimal.Append("f");
                            break;
                        default:
                            break;
                    }
                }

            }

            return hexadecimal.ToString();
        }

        public StringBuilder ConvertToStringBuilder(List<int> binary)
        {
            var stringBuilder = new StringBuilder();

            foreach (var bit in binary)
            {
                stringBuilder.Append(bit.ToString());
            }

            return stringBuilder;
        }



        public byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public String ToBinary(Byte[] data)
        {
            return string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        private static List<string> SplitBitsIntoSixteenSegmentsWith36Bits(StringBuilder bits)
        {
            var segments = new List<string>();
            string temporarySegment;
            string bitsConverted = bits.ToString();
            for (int i = 0; i < 16; i++)
            {
                if (i == 0)
                {
                    temporarySegment = bitsConverted.Substring(0, 32);
                }

                else
                {
                    temporarySegment = bitsConverted.Substring(i * 32, 32);
                }

                segments.Add(temporarySegment);
            }

            return segments;
        }
        static StringBuilder GetBits(byte[] input)
        {
            var bits = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                bits.Append(Convert.ToString(input[i], 2));
            }

            return bits;
        }
    }
}

