using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace InterTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMatch1()
        {
            MessageDeal1.MyDeal m1 = new MessageDeal1.MyDeal();
            m1.DealFriendMessage(
                new Dictionary<string, object> {
                {"uin",0},
                {"nick","nick"},
                {"mark","mark"}
                }, "@问：你好 答：嗯嗯好呀 呆呵");
        }
    }
}
