using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttpClientRebirth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientRebirth.Tests
{
    [TestClass()]
    public class NNTests
    {
        [TestMethod()]
        public void StartTest() {
            //{
            //    NNDataReceiving.watch.Start();
            //    NNDataReceiving.KeyBoardInfoReceiving();
            NNDataReceiving.KeyBoardInfoReceiving();
        }
    }
}