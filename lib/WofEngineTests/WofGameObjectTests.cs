using Microsoft.VisualStudio.TestTools.UnitTesting;
using WofEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WofEngine.Exceptions;
using WofEngine.NetworkCommand;
using System.Threading;

namespace WofEngine.Tests
{
    [TestClass()]
    public class WofGameObjectTests
    {
        [TestMethod()]
        public void EnterServerTest()
        {
            WofGameObject.Game.EnterServer("aadmin@wof", "Admin123");
            // TODO: use callback or wait properly
            Thread.Sleep(2000);
            Assert.AreEqual(WOF_STATE.LOBBY, WofGameObject.Game.State);
        }

        [TestMethod()]
        public void SerializeIdentificationCommand()
        {
            GenericNetworkCommand cmd = new IdentificationCommand("1234");
            string c = cmd.ToJson();
            GenericNetworkCommand deserialized = GenericNetworkCommand.FromJson(c);
            Assert.AreEqual(cmd.ClientId, deserialized.ClientId);

        }
    }
}