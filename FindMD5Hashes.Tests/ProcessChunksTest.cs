using FindMD5HashWithLeadingZeroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD5HashWithLeadingZeroes.Tests
{
    public class ProcessChunksTest
    {
        [Theory]
        [InlineData("They are deterministic", "23db6982caef9e9152f1a5b2589e6ca3")]

        public void ToBinary_String_ReturnTrue(string input, string expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();
            var binaryString = md5Hash.ToBinary(md5Hash.ConvertToByteArray(input, Encoding.ASCII));
            var paddedInput = md5Hash.AddPadding(binaryString);
            
            //Act
            var actual = md5Hash.ProcessChunks(paddedInput).ToString();

            //Assert
            Assert.Equal(expectedValue, actual);
        }
    }
}
