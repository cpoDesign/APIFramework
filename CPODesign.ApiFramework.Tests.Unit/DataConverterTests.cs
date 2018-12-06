using ConsoleCPODesign.ApiFrameworkApp1;
using CPODesign.ApiFramework;
using CPODesign.ApiFramework.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConsoleApp1.Tests.Unit
{
    [TestClass]
    public class DataConverterTests
    {
        [TestMethod]
        public void Convert_JSON_PassNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DataConverter().Convert<TestClass>(null));
        }

        [TestMethod]
        public void Convert_JSON_PassEmptyString_ShouldReturnEmptyObject()
        {
            var item = new DataConverter().Convert<TestClass>(string.Empty);
            Assert.AreEqual(typeof(TestClass), item.GetType());
        }

        [TestMethod]
        public void Convert_XML_PassDummyString_ShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => new DataConverter().Convert<TestClass>("Dummy Class Test", ConversionType.XML));
        }

        [TestMethod]
        public void Convert_JSON_ConvertsStringIntoObject()
        {
            string json1 = @"{'Name':'James'}";
            var result = new DataConverter().Convert<TestClass>(json1);
            Assert.AreEqual("James", result.Name);
        }

        [TestMethod]
        public void Convert_JSON_ConvertsStringIntoObject_ShouldSucced()
        {
            string json1 = @"{}";
            var result = new DataConverter().Convert<TestClass>(json1);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Name));
        }

        [TestMethod]
        public void Convert_JSON_ConvertsStringIntoString_ShouldThrowException()
        {
            string json1 = @"test";
            Assert.ThrowsException<ArgumentException>(() => new DataConverter().Convert<string>(json1));

        }
    }
}
