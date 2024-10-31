using System.Text;
using FindMD5HashWithLeadingZeroes;

namespace MD5HashWithLeadingZeroes.Tests
{
    public class PaddingTest
    {
        [Theory]
        [InlineData("They are deterministic", "01010100 01101000 01100101 01111001 00100000 01100001 01110010 01100101 00100000 01100100 01100101 01110100 01100101 01110010 01101101 01101001 01101110 01101001 01110011 01110100 01101001 01100011 10000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 10110000")]

        public void Padding_InputLowerThan512Bits_ReturnTrue(string input, string expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();
            var byteString = md5Hash.ToBinary(md5Hash.ConvertToByteArray(input, Encoding.ASCII));

            //Act
            var actual = md5Hash.AddPadding(byteString);

            //Assert
            Assert.Equal(expectedValue, actual);
        }
    }
}