using FindMD5HashWithLeadingZeroes;

namespace MD5HashWithLeadingZeroes.Tests
{
    public class InitializeConstants
    {
        [Theory]
        [InlineData(0, 0xd76aa478)]
        [InlineData(7, 0xfd469501)]
        [InlineData(12, 0x6b901122)]
        [InlineData(23, 0xe7d3fbc8)]
        [InlineData(25, 0xc33707d6)]

        public void IntializeK_WithCorrectNumbers_ReturnTrue(int index, uint expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();
            uint[] k = new uint[64];

            //Act
            md5Hash.InitializeK(k);

            //Assert
            Assert.Equal(expectedValue, k[index]);
        }

        [Theory]
        [InlineData(0, 7)]
        [InlineData(16, 5)]
        [InlineData(48, 6)]
        [InlineData(51, 21)]
        [InlineData(63, 21)]

        public void IntializeS_WithCorrectNumbers_ReturnTrue(int index, int expectedValue)
        {
            //Arrange
            var md5Hash = new FindMD5Hash();
            int[] s = new int[64];

            //Act
            md5Hash.InitializeS(s);

            //Assert
            Assert.Equal(expectedValue, s[index]);
        }
    }
}
