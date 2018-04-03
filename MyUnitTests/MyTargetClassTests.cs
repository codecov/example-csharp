using UnitTestTargetProject;
using Xunit;

namespace MyUnitTests
{
    public class MyTargetClassTests
    {
        [Fact]
        public void VerifySummation()
        {
            const int intFirstNumber = 15;
            const int intsecondNumber = 10;
            var objTargetClass = new MyTargetClass();
            var intResult = objTargetClass.AddNumbers(intFirstNumber, intsecondNumber);
            Assert.Equal(15 + 10, intResult);
        }

        [Fact]
        public void VerifySubtraction()
        {
            const int intFirstNumber = 15;
            const int intsecondNumber = 10;
            var objTargetClass = new MyTargetClass();
            var intResult = objTargetClass.SubractNumbers(intFirstNumber, intsecondNumber);
            Assert.Equal(15 - 10, intResult);
        }

        [Fact]
        public void VerifyDivision()
        {
            const int intFirstNumber = 15;
            const int intsecondNumber = 10;
            var objTargetClass = new MyTargetClass();
            var intResult = objTargetClass.DivideNumbers(intFirstNumber, intsecondNumber);
            Assert.Equal(15/10, intResult);
        }
    }
}