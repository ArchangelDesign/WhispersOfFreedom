using Microsoft.VisualStudio.TestTools.UnitTesting;
using WofEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WofEngine.Exceptions;

namespace WofEngine.Tests
{
    [TestClass()]
    public class WofGameObjectTests
    {
        [TestMethod()]
        public void EnterServerTest()
        {
            WofGameObject go = new WofGameObject();
            go.EnterServer("aadmin@wof", "Admin123");
            Assert.AreEqual(WOF_STATE.LOBBY, go.State);
        }
    }
}