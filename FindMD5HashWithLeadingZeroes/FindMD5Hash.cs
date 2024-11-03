﻿using System;
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

        public static void Main(string[] args)
        {
            var input = "They are deterministic";
            var md5Hash = new FindMD5Hash();
            //var binaryString = md5Hash.ToBinary(md5Hash.ConvertToByteArray(input, Encoding.ASCII));
            //var paddedInput = md5Hash.AddPadding(binaryString, input);
            //var hash = md5Hash.ProcessChunks(paddedInput);
            //Console.WriteLine(paddedInput);
            var data = md5Hash.ConvertToByteArray(input, Encoding.ASCII);
            var paddedData = md5Hash.AddPadding(data);
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
            paddedData[lengthOfData] = 1;
            var dataLengthInBits = BitConverter.GetBytes(data.Length * 8);
            dataLengthInBits = dataLengthInBits.Reverse().ToArray();
            var remainingZeroes = 8 - dataLengthInBits.Length;
            Array.Copy(dataLengthInBits, 0, paddedData, paddedData.Length - remainingZeroes, remainingZeroes);

            return paddedData;
        }

        //public string AddPadding(string binaryString, string input)
        //{
        //    //int inputCount = input.Length;
        //    int binaryCount = binaryString.Length;
        //    var countInBinary = DecimalToBase(binaryCount, 2);

        //    var stringBuilder = new StringBuilder(binaryString);

        //    stringBuilder.Append("1");
        //    var countOfZeroes = 0;
        //    while (true)
        //    {
        //        bool isDivisible = (stringBuilder.Length + 64) % 512 == 0;

        //        if (isDivisible == true)
        //        {
        //            break;
        //        }

        //        stringBuilder.Append("0");
        //        countOfZeroes++;
        //    }

        //    if (countInBinary.Length > 64)
        //    {
        //        while (binaryCount > 64)
        //        {
        //            countInBinary.Remove(0, 1);
        //        }

        //        stringBuilder.Append(countInBinary.ToString());
        //    }

        //    else
        //    {
        //        AddBytesWithZeroes(64 - countInBinary.Length, stringBuilder, true);
        //        stringBuilder.Append(countInBinary.ToString());
        //    }

        //    return stringBuilder.ToString();
        //}

        public void AddBytesWithZeroes(int remainingZeroes, StringBuilder stringBuilder, bool isPaddedWithZeroes)
        {
            for (int i = 1; i <= remainingZeroes; i++)
            {
                stringBuilder.Append("0");
            }
        }

        public string ProcessChunks(string paddedInput)
        {
            //Constants
            var k = new uint[64];
            InitializeK(k);

            // s specifies the per-round shift amounts
            var s = new int[64];
            InitializeS(s);

            //var index = 0;
            //uint a0 = 0x67425301;
            //uint b0 = 0xEDFCBA45;
            //uint c0 = 0x98CBADFE;
            //uint d0 = 0x13DCE476;
            uint a0 = 0x67452301;   // A
            uint b0 = 0xefcdab89;   // B
            uint c0 = 0x98badcfe;   // C
            uint d0 = 0x10325476;   // D

            var chunksOf512Bits = BreakIntoChunks(paddedInput, 512);
            for (int i = 0; i < chunksOf512Bits.Count; i++)
            {
                uint a1 = a0;
                uint b1 = b0;
                uint c1 = c0;
                uint d1 = d0;

                var chunksOf32Bits = BreakIntoChunks(chunksOf512Bits[i], 32);
                for (uint index = 0; index < 64; index++)
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

                    //uint firstStep = (uint)(f % (Math.Pow(2, 32)) + a1);
                    //uint secondStep = BinaryToDecimal(chunksOf32Bits[order]) + firstStep;
                    //uint thirdStep = secondStep + k[index];
                    //f = f + a1 + BinaryToDecimal(chunksOf32Bits[order]) + k[index];
                    //var rotation = f << s[index];
                    //var rotation = long.RotateLeft(f, s[index]);
                    //(x << n) | (x >> (32 - n))
                    //uint lastStep = (uint)((thirdStep << s[index]) % (Math.Pow(2, 32)));
                    //uint lastStep = (uint)((thirdStep << s[index]) | (thirdStep >> (32 - s[index])));

                    var tempData = d1;
                    b1 = b1 + LeftRotate((a1 + BinaryToDecimal(chunksOf32Bits[(int)order]) + k[index]), s[index]);
                    c1 = b1;
                    d1 = c1;
                    a1 = tempData;
                }

                a0 += a1;
                b0 += b1;
                c0 += c1;
                d0 += d1;
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

            //char[] chars = new char[16];

            return result.ToString();
        }

        public void InitializeK(uint[] k)
        {
            for (int i = 0; i < 64; i++)
            {
                k[i] = (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
            }
        }

        public static uint LeftRotate(uint x, int c)
        {
            return (x << c) | (x >> (32 - c));
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
                    indexOfNumber = 1;
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
                    //If it is five than reset it to the first pair for the round
                    //if (indexOfNumber == 5)
                    //{
                    //    indexOfNumber = 5;
                    //}

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
                    //If it is five than reset it to the first pair for the round
                    //if (indexOfNumber == 5)
                    //{
                    //    indexOfNumber = 4;
                    //}

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
                    //If it is five than reset it to the first pair for the round
                    //if (indexOfNumber == 5)
                    //{
                    //    indexOfNumber = 6;
                    //}

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
                for (int i = 0; i < (paddedInput.Length / 512); i++)
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

            while ((toBase == 2) && (binary.Count % 4 != 0))
            {
                binary.Insert(0, 0);
            }

            var stringBuilder = ConvertToStringBuilder(binary);
            return stringBuilder;
        }

        public uint BinaryToDecimal(string binary)
        {
            uint converted = 0;
            var reversed = binary.Reverse().ToArray();

            for (int i = 0; i < reversed.Count(); i++)
            {
                //var x = BigInteger.Parse(reversed[i].ToString());
                //converted += BigInteger.Multiply(BigInteger.Pow(2, i), BigInteger.Parse(reversed[i].ToString()));
                converted += (uint.Parse(reversed[i].ToString()) * (uint)Math.Pow(2, i));

            }

            return converted;
        }

        public string DecimalToHexadecimal(uint numberInDecimal)
        {
            var digits = new List<uint>();

            while (numberInDecimal > 0)
            {
                uint digit = numberInDecimal % 16;
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

        //private static List<string> SplitBitsIntoSixteenSegmentsWith36Bits(StringBuilder bits)
        //{
        //    var segments = new List<string>();
        //    string temporarySegment;
        //    string bitsConverted = bits.ToString();
        //    for (int i = 0; i < 16; i++)
        //    {
        //        if (i == 0)
        //        {
        //            temporarySegment = bitsConverted.Substring(0, 32);
        //        }

        //        else
        //        {
        //            temporarySegment = bitsConverted.Substring(i * 32, 32);
        //        }

        //        segments.Add(temporarySegment);
        //    }

        //    return segments;
        //}

        //static StringBuilder GetBits(byte[] input)
        //{
        //    var bits = new StringBuilder();
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        bits.Append(Convert.ToString(input[i], 2));
        //    }

        //    return bits;
        //}
    }
}

