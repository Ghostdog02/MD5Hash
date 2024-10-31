using System;
using System.Diagnostics.Metrics;
using System.Numerics;
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
            //Console.WriteLine(paddedInput);
        }

        public string AddPadding(string binaryString)
        {
            int inputCount = INPUT.Length;            
            int count = binaryString.Length;
            var binaryCount = IntegerToBase(count, 2);

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

                    stringBuilder.AppendLine(binaryCount.ToString());
                }

                else
                {
                    AddBytesWithZeroes(64 - binaryCount.Length, stringBuilder, true);
                    stringBuilder.AppendLine(binaryCount.ToString());
                }
            }

            return stringBuilder.ToString().Replace("\r\n", "");
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

        public StringBuilder IntegerToBase(int number, int toBase)
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

        public StringBuilder ConvertToStringBuilder(List<int> binary)
        {
            var stringBuilder = new StringBuilder();
            int index = 1;

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

        //public void ConvertToMD5Hash(string input)
        //{
        //    byte[] bytes = new byte[byte.MaxValue];
        //    bytes = Encoding.ASCII.GetBytes(input);
        //    var bits = GetBits(bytes);
        //    //List<StringBuilder> blocksOfBits = [bits];
        //    //StringBuilder bits = new StringBuilder();
        //    //bits.Append(bytes);
        //    AddPadding(bits);
        //    //ProcessMessage(bits);

        //}

        //static void AddPadding(StringBuilder bits)
        //{
        //    var inputLenght = bits.Length;
        //    bits.Append("1");
        //    int zeroesCount = 448 - bits.Length;
        //    PaddZeroes(bits, zeroesCount);

        //    //Convert inputLenght in bits
        //    var lenghtInBits = Convert.ToString(inputLenght, 2);
        //    var bitsCount = lenghtInBits.Length;
        //    var zeroes = 64 - bitsCount;
        //    PaddZeroes(bits, zeroes);
        //    bits.Append(lenghtInBits);
        //}

        private static void PaddZeroes(StringBuilder bits, int zeroesCount)
        {
            for (int i = 0; i < zeroesCount; i++)
            {
                bits.Append("0");
            }
        }

        //static void ProcessMessage(StringBuilder bits)
        //{
        //    // s specifies the per-round shift amounts
        //    int[] shifter = new int[64];
        //    int indexOfShiftAmounts = 0;
        //    List<int> shiftAmounts = new List<int>();
        //    InitializeShifter(shifter, shiftAmounts, indexOfShiftAmounts);
        //    uint[] k = new uint[64];
        //    InitializeK(k);
        //    BigInteger a0 = 0x67452301;   // A
        //    BigInteger b0 = 0xefcdab89;   // B
        //    BigInteger d0 = 0x10325476;   // D
        //    BigInteger c0 = 0x98badcfe;   // C
        //    int currentbit = 0;

        //    /*nt[] m = new int[16];*/
        //    // Initialize hash value for this chunk:
        //    BigInteger a = a0;
        //    BigInteger b = b0;
        //    BigInteger d = d0;
        //    BigInteger c = c0;
        //    var segments = SplitBitsIntoSixteenSegmentsWith36Bits(bits);
        //    for (int i = 0; i < 64; i++)
        //    {
        //        BigInteger f;
        //        int g;

        //        if (i >= 0 && i <= 15)
        //        {
        //            f = (b & c) | ((~b) & d);
        //            g = i;
        //        }

        //        else if (i >= 16 && i <= 31)
        //        {
        //            f = (d & b) | ((~d) & c);
        //            g = (5 * i + 1) % 16;
        //        }

        //        else if (i >= 32 && i <= 47)
        //        {
        //            f = b ^ c ^ d;
        //            g = (3 * i + 5) % 16;
        //        }

        //        else
        //        {
        //            f = c ^ (b ^ (~d));
        //            g = (7 * i + 1) % 16;
        //        }

        //        var number = Convert.ToInt64(segments[g], 2);
        //        f = f + a + k[i] + number;
        //        a = d;
        //        d = c;
        //        c = b;
        //        b = b + (f << shifter[i]);
        //    }

        //    a0 += a;
        //    b0 += b;
        //    d0 += d;
        //    c0 += c;

        //    string digest = $"{a}{b}{c}{d}";
        //    var bigInt = BigInteger.Parse(digest);
        //    var hexString = bigInt.ToString("x");
        //}

        //private static void InitializeShifter(int[] shifter, List<int> shiftAmounts, int indexOfShiftAmounts)
        //{
        //    for (int i = 0; i < 64; i++)
        //    {
        //        if (i >= 0 && i <= 15)
        //        {
        //            if (i == 0)
        //            {
        //                shiftAmounts.Add(7);
        //                shiftAmounts.Add(12);
        //                shiftAmounts.Add(17);
        //                shiftAmounts.Add(22);
        //            }

        //        }

        //        else if (i >= 16 && i <= 31)
        //        {
        //            if (i == 16)
        //            {
        //                shiftAmounts.Clear();
        //                shiftAmounts.Add(5);
        //                shiftAmounts.Add(9);
        //                shiftAmounts.Add(14);
        //                shiftAmounts.Add(20);
        //            }

        //        }

        //        else if (i >= 32 && i <= 47)
        //        {
        //            if (i == 32)
        //            {
        //                shiftAmounts.Clear();
        //                shiftAmounts.Add(4);
        //                shiftAmounts.Add(11);
        //                shiftAmounts.Add(16);
        //                shiftAmounts.Add(23);
        //            }

        //        }

        //        else
        //        {
        //            if (i == 48)
        //            {
        //                shiftAmounts.Clear();
        //                shiftAmounts.Add(6);
        //                shiftAmounts.Add(10);
        //                shiftAmounts.Add(15);
        //                shiftAmounts.Add(21);
        //            }

        //        }

        //        shifter[i] = shiftAmounts[indexOfShiftAmounts];
        //        indexOfShiftAmounts++;
        //        indexOfShiftAmounts = (indexOfShiftAmounts == 4) ? 0 : indexOfShiftAmounts;
        //    }
        //}

        //private static void InitializeK(uint[] k)
        //{
        //    // Use binary integer part of the sines of integers (Radians) as constants:
        //    for (int i = 0; i < 64; i++)
        //    {
        //        k[i] = (uint)Math.Floor(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
        //    }

        //}

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
