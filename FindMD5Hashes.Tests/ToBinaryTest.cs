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
            "01010100 01101000 01100101 01111001 00100000 01100001 01110010 01100101 00100000 01100100 01100101 01110100 01100101 01110010 01101101 01101001 01101110 01101001 01110011 01110100 01101001 01100011")]

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
