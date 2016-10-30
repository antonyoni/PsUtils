using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PsUtils {

    public class StreamSplitter {

        private static Encoding defaultEncoding = new UTF8Encoding();
        private static int defaultBufferSize = 128 * 1024;

        public static IEnumerable<byte[]> Split(Stream stream) {
            var delimiter = defaultEncoding.GetBytes(Environment.NewLine);
            return Split(stream, delimiter);
        }

        public static IEnumerable<byte[]> Split(Stream stream, byte[] delimiter) {

            MemoryStream ms = new MemoryStream(defaultBufferSize * 2); // DocBuffer
            var buffer = new byte[defaultBufferSize];
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
                        ms = new MemoryStream(defaultBufferSize * 2);
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

    }

}
