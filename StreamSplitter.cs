using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PsUtils {

    public class StreamSplitter : IEnumerable<byte[]> {

        private static Encoding defaultEncoding = new UTF8Encoding();
        private static int defaultBufferSize = 128 * 1024;
        private static byte[] defaultDelimiter = defaultEncoding.GetBytes(Environment.NewLine);

        #region Static Methods

        public static IEnumerable<byte[]> Split(Stream stream) {
            var delimiter = defaultEncoding.GetBytes(Environment.NewLine);
            return Split(stream, delimiter);
        }

        public static IEnumerable<byte[]> Split(Stream stream, byte[] delimiter) {
            return Split(stream, delimiter, defaultBufferSize);
        }

        public static IEnumerable<byte[]> Split(Stream stream, byte[] delimiter, int bufferSize) {

            MemoryStream ms = new MemoryStream(bufferSize * 2); // DocBuffer
            var buffer = new byte[bufferSize];
            int readLength = 0;

            while ((readLength = stream.Read(buffer, 0, buffer.Length)) > 0) {

                var splitEnds = KMP.FindAll(buffer, delimiter);
                var startPointer = 0;

                for (int i = 0; i < splitEnds.Length; i++) {
                    var endPointer = splitEnds[i];
                    if (endPointer > readLength)
                        break;
                    var copyLength = endPointer - startPointer + delimiter.Length;
                    byte[] doc;
                    if (ms.Length > 0) {
                        ms.Write(buffer, startPointer, copyLength);
                        doc = ms.ToArray();
                        ms = new MemoryStream(bufferSize * 2);
                    } else {
                        doc = new byte[copyLength];
                        Array.ConstrainedCopy(buffer, startPointer, doc, 0, copyLength);
                    }
                    yield return doc;
                    startPointer = endPointer + delimiter.Length;
                }

                // handle remaining bytes
                if (startPointer < readLength) {
                    var copyLength = readLength - startPointer;
                    ms.Write(buffer, startPointer, copyLength);
                }

            }

            // handle anything left in the DocBuffer
            if (ms.Length > 0) {
                yield return ms.ToArray();
                ms = null;
            }

        }

        #endregion

        #region Static - Helpers

        private static Stream createFileStream(string filePath, int bufferSize) {
            return new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.SequentialScan
            );
        }

        #endregion

        #region Fields

        IEnumerable<byte[]> enumerator;
        //private Stream stream;
        //private byte[] delimiter;
        //private Encoding encoding;
        //private int bufferSize;

        #endregion

        #region Constructors

        public StreamSplitter(string filePath)
            : this(filePath, defaultDelimiter) { }

        public StreamSplitter(string filePath, byte[] delimeter)
            : this(filePath, delimeter, defaultEncoding) { }

        public StreamSplitter(string filePath, byte[] delimeter, Encoding encoding)
            : this(filePath, delimeter, encoding, defaultBufferSize) { }

        public StreamSplitter(string filePath, byte[] delimeter, int bufferSize)
            : this(filePath, delimeter, defaultEncoding, bufferSize) { }

        public StreamSplitter(string filePath, byte[] delimiter, Encoding encoding, int bufferSize)
            : this(createFileStream(filePath, bufferSize), delimiter, encoding, bufferSize) { }

        public StreamSplitter(Stream stream)
            : this(stream, defaultDelimiter) { }

        public StreamSplitter(Stream stream, byte[] delimeter)
            : this(stream, delimeter, defaultEncoding) { }

        public StreamSplitter(Stream stream, byte[] delimeter, Encoding encoding)
            : this(stream, delimeter, encoding, defaultBufferSize) { }

        public StreamSplitter(Stream stream, byte[] delimeter, int bufferSize)
            : this(stream, delimeter, defaultEncoding, bufferSize) { }

        public StreamSplitter(Stream stream, byte[] delimiter, Encoding encoding, int bufferSize) {

            var encPreamble = encoding.GetPreamble();

            if (encoding is UTF8Encoding) {
                // by default utf8 without bom, check regardless
                encPreamble = Encoding.UTF8.GetPreamble();
            }
            if (encPreamble.Length > 0) {
                var paLen = encPreamble.Length;
                var paBuf = new byte[paLen];
                stream.Read(paBuf, 0, paLen);
                if (encPreamble.SequenceEqual(paBuf)) {
                    System.Diagnostics.Trace.TraceWarning(
                        String.Format("BOM found ({0} bytes). It will be ignored.", paLen)
                    );
                } else {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            enumerator = StreamSplitter.Split(stream, delimiter, bufferSize);
        }

        #endregion

        #region Methods - Interface Implementation

        public IEnumerator<byte[]> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        #endregion

    }

}

