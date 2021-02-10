using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cymbal;

namespace CymbalStudentLoaderTest
{
    [TestClass]
    public class StudentFileProcessorTest
    {
        [TestMethod]
        public void TestProcessCsvFile()
        {
            StudentFileProcessor processor = new StudentFileProcessor();
            int records = processor.ProcessCsvFile("students.csv");
            Assert.AreEqual<int>(3, records);
        }
    }
}
