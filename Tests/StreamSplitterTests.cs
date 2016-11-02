using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;

namespace PsUtils.Tests {

    [TestClass()]
    public class StreamSplitterTests {

        private string filePath1 = "..\\..\\Data\\file1.txt";
        private string filePath2 = "..\\..\\Data\\file1_nonewline.txt";
        private string filePath3 = "..\\..\\Data\\file2.txt";

        [TestCategory("StreamSplitter")]
        [TestMethod()]
        public void SplitTest_DefaultDelimiter_File1() {
            using (var stream = new FileStream(filePath1, FileMode.Open)) {
                var split = StreamSplitter.Split(stream);
                Assert.AreEqual(10, split.Count());
            }
        }

        [TestCategory("StreamSplitter")]
        [TestMethod()]
        public void SplitTest_DefaultDelimiter_File2() {

            using (var stream = new FileStream(filePath2, FileMode.Open)) {
                // Check that if there's no new line at end of file, it still flushes the end
                var split = StreamSplitter.Split(stream);
                Assert.AreEqual(10, split.Count());
            }
        }

        [TestCategory("StreamSplitter")]
        [TestMethod()]
        public void SplitTest_CustomDelimiter_File3() {
            using (var stream = new FileStream(filePath3, FileMode.Open)) {
                var delimeter = Encoding.UTF8.GetBytes("</doc>");

                var split = StreamSplitter.Split(stream, delimeter);

                Assert.AreEqual(5, split.Count());
            }
        }

        [TestCategory("StreamSplitter")]
        [TestMethod()]
        public void SplitTest_DelimiterDoesNotExist_File3() {
            using (var stream = new FileStream(filePath3, FileMode.Open)) {
                var delimeter = Encoding.UTF8.GetBytes("empty");

                var split = StreamSplitter.Split(stream, delimeter);

                Assert.AreEqual(1, split.Count());
            }
        }

    }

}