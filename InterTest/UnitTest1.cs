using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMatch1()
        {
            MessageDeal1.MessageDeal1 m1 = new MessageDeal1.MessageDeal1();
            m1.DealFriendMessage("@问：你好 答：嗯嗯好呀 呆呵");
        }
    }
}
