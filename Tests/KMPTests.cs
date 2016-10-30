﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace PsUtils.Tests {

    [TestClass()]
    public class KMPTests {

        [TestMethod()]
        public void NewFailureFunction_ShortString_ReturnsValidTable() {

            string pattern = "ABCDABD";
            var bytes = Encoding.ASCII.GetBytes(pattern);
            // what the array for pattern should look like:
            var failFunction = new int[] {-1,0,0,0,0,1,2};

            var table = KMP.NewFailureFunction(bytes);

            Assert.AreEqual(-1, table[0]); // first value always -1
            Assert.AreEqual(0, table[1]);  // second value always 0
            CollectionAssert.AreEqual(failFunction, table);

        }

        [TestMethod()]
        public void NewFailureFunction_LongString_ReturnsValidTable() {

            string pattern = "PARTICIPATE IN PARACHUTE";
            var bytes = Encoding.ASCII.GetBytes(pattern);
            var failFunction = new int[] {-1,0,0,0,0,0,0,0,1,2,0,0,0,0,0,0,1,2,3,0,0,0,0,0};

            var table = KMP.NewFailureFunction(bytes);

            Assert.AreEqual(-1, table[0]); // first value always -1
            Assert.AreEqual(0, table[1]);  // second value always 0
            CollectionAssert.AreEqual(failFunction, table);

        }

        [TestMethod()]
        public void Find_BytePattern_SingleInstance() {

            string input = "ABCDABD";
            string pattern = "CDA";

            var inputBytes = Encoding.ASCII.GetBytes(input);
            var patternBytes = Encoding.ASCII.GetBytes(pattern);

            var found = KMP.FindAll(inputBytes, patternBytes);

            Assert.AreEqual(1, found.Length);
            Assert.AreEqual(2, found[0]);

        }

        [TestMethod()]
        public void Find_StringPattern_SingleInstance() {

            string input = "ABCDABD";
            string pattern = "CDA";

            var found = KMP.FindAll(input, pattern);

            Assert.AreEqual(1, found.Length);
            Assert.AreEqual(2, found[0]);

        }

        [TestMethod()]
        public void Find_StringPattern_SingleInstance_MultiByteEncoding() {

            string input = "ABCDABD";
            string pattern = "CDA";

            var found = KMP.FindAll(input, pattern, Encoding.Unicode);

            Assert.AreEqual(1, found.Length);
            Assert.AreEqual(2, found[0]);

        }

        [TestMethod()]
        public void Find_StringPattern_MultipleInstances() {

            string input = "AABAACAADAABAAABAA";
            string pattern = "AABA";
            var shouldFind = new int[] {0,9,13};

            var found = KMP.FindAll(input, pattern);

            CollectionAssert.AreEqual(shouldFind, found);

        }

        [TestMethod()]
        public void Find_StringPattern_SingleCharacter() {

            string input = "AAABAAA";
            string pattern = "A";
            var shouldFind = new int[] {0,1,2,4,5,6};

            var found = KMP.FindAll(input, pattern);

            CollectionAssert.AreEqual(shouldFind, found);

        }

        [TestMethod()]
        public void Find_StringPattern_SingleCharacter_MultiByteEncoding() {

            string input = "AAABAAA";
            string pattern = "A";
            var shouldFind = new int[] {0,1,2,4,5,6};

            var found = KMP.FindAll(input, pattern, Encoding.Unicode);

            CollectionAssert.AreEqual(shouldFind, found);

        }

        [TestMethod()]
        public void Find_StringPattern_TwoCharacters() {

            string input = "AAABAAA";
            string pattern = "AA";
            var shouldFind = new int[] {0,4};

            var found = KMP.FindAll(input, pattern);

            CollectionAssert.AreEqual(shouldFind, found);

        }

        [TestMethod()]
        public void Find_StringPattern_TwoCharacters_MultiByteEncoding() {

            string input = "AAABAAA";
            string pattern = "AA";
            var shouldFind = new int[] {0,4};

            var found = KMP.FindAll(input, pattern, Encoding.Unicode);

            CollectionAssert.AreEqual(shouldFind, found);

        }

        [TestMethod()]
        public void Find_StringPattern_TwoCharacters2() {

            string input = "AAAABAAAA";
            string pattern = "AA";
            var shouldFind = new int[] {0,2,5,7};

            var found = KMP.FindAll(input, pattern);

            CollectionAssert.AreEqual(shouldFind, found);

        }

    }

}