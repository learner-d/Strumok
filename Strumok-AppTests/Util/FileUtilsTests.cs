using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrumokApp.Util;
using System;
using System.IO;

namespace StrumokApp.Util.Tests
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