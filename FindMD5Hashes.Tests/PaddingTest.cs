using System.Text;
using FindMD5HashWithLeadingZeroes;

namespace MD5HashWithLeadingZeroes.Tests
{
    public class PaddingTest
    {
        [Theory]
        [InlineData("They are deterministic", "01010100011010000110010101111001001000000110000101110010011001010010000001100100011001010111010001100101011100100110110101101001011011100110100101110011011101000110100101100011100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010110000")]

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