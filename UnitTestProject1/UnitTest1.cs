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
            string input = "@e3y041ltb";
            string need = "344993453";
            string result = WebQQ2.Extends.QQHelper.getGTK(input);
            Assert.AreEqual(result,need);
        }
    }
}
