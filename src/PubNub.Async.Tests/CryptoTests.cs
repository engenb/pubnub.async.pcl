using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PubNub.Async.Tests
{
    public class CryptoTests
    {
        [Fact]
        public void Decrypt__Given_PubNubEncryptedMessage__Then_DecryptDecodeContent()
        {
            var expectedResult = "{\"text\":\"Hello World!\"}";

            var cipher = "TEST";
            
            var encryptedMsg = "fhqbfIebqFs1rIzlMGNanS03azaP5nqBa16PbfnXkm8=";

            var result = Crypto.Decrypt(cipher, encryptedMsg);

            Assert.Equal(expectedResult, result);
        }
    }
}
