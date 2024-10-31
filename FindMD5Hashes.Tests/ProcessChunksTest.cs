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
        [InlineData("The quick brown fox jumps over the lazy dog", "9e107d9d372bb6826bd81d3542a419d6")]

        public void ToBinary_String_ReturnTrue(string input, string expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();
            var binaryString = md5Hash.ToBinary(md5Hash.ConvertToByteArray(input, Encoding.ASCII));
            var paddedInput = md5Hash.AddPadding(binaryString, input);
            
            //Act
            var actual = md5Hash.ProcessChunks(paddedInput).ToString();

            //Assert
            Assert.Equal(expectedValue, actual);
        }
    }
}
