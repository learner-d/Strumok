using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strumok_App.Util;
using System;
using System.IO;

namespace Strumok_App.Util.Tests
{
    [TestClass()]
    public class FileUtilsTests
    {
        [TestMethod()]
        public void CanBeReadTest()
        {
            string TestFilePath = @"C:\Users\dmytrogergel\Desktop\Reverso.lnk";
            string TestNoFilePath = @"C:\Users\dmytrogergel\Desktop\Reverso.lnk.empty";
            try
            {
                Assert.IsTrue(FileUtils.CanBeRead(TestFilePath));
                Assert.IsFalse(FileUtils.CanBeRead(TestNoFilePath));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}