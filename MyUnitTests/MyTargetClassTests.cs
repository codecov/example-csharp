using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestTargetProject;

namespace MyUnitTests
{
    [TestClass]
    public class MyTargetClassTests
    {
        [TestMethod]
        public void VerifySummation()
        {
            int intFirstNumber = 15, intsecondNumber = 10;
            MyTargetClass objTargetClass = new MyTargetClass();
            int intResult = objTargetClass.AddNumbers(intFirstNumber, intsecondNumber);
            Assert.AreEqual(15 + 10, intResult, "Summation method is not written well.");
        }

        [TestMethod]
        public void VerifySubtraction()
        {
            int intFirstNumber = 15, intsecondNumber = 10;
            MyTargetClass objTargetClass = new MyTargetClass();
            int intResult = objTargetClass.SubractNumbers(intFirstNumber, intsecondNumber);
            Assert.AreEqual(15 - 10, intResult, "Subtraction method is not written well.");
        }
    }
}
