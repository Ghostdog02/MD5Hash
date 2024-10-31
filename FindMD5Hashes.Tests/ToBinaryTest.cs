using FindMD5HashWithLeadingZeroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD5HashWithLeadingZeroes.Tests
{
    public class ToBinaryTest
    {
        [Theory]
        [InlineData("They are deterministic",
            "01010100011010000110010101111001001000000110000101110010011001010010000001100100011001010111010001100101011100100110110101101001011011100110100101110011011101000110100101100011")]

        public void ToBinary_String_ReturnTrue(string input, string expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();

            //Act
            var actual = md5Hash.ToBinary(md5Hash.ConvertToByteArray(input, Encoding.ASCII));

            //Assert
            Assert.Equal(expectedValue, actual);
        }
    }
}
