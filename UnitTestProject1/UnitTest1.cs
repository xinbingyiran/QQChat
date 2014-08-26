using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGTK()
        {
            var result = WebQQ2.Extends.QQHelper.GetPassword("841473232", "thisis", "abcd");
            Assert.AreEqual(result,"");
        }
    }
}
