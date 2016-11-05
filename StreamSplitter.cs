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
            return Split(stream, delimiter, defaultBufferSize);
        }

        public static IEnumerable<byte[]> Split(Stream stream, byte[] delimiter, int bufferSize) {

            MemoryStream ms = new MemoryStream(bufferSize * 2); // DocBuffer
            var buffer = new byte[bufferSize];
            int readLength = 0;

#if DEBUG
            var bufStr = "";
            var msStr = "";
#endif

            while ((readLength = stream.Read(buffer, 0, buffer.Length)) > 0) {

                var splitEnds = KMP.FindAll(buffer, delimiter);

#if DEBUG
                bufStr = Encoding.UTF8.GetString(buffer);
                msStr = Encoding.UTF8.GetString(ms.ToArray());
#endif

                if (ms.Length > 0 && delimiter.Length > 1) {
                    var searchBuffer = new byte[delimiter.Length * 2 - 2];
                    int end;
                    if (delimiter.Length - 1 <= ms.Length) {
                        end = delimiter.Length - 1;
                    } else {
                        end = (int)ms.Length;
                    }
                    ms.Seek(-end, SeekOrigin.Current);
                    ms.Read(searchBuffer, 0, end);
                    Array.Copy(buffer, 0, searchBuffer, end, end);
                    var found = KMP.Find(searchBuffer, delimiter);
                    if (found > -1) {
                        var newSplitEnds = new int[splitEnds.Length + 1];
                        newSplitEnds[0] = found - end;
                        Array.Copy(splitEnds, 0, newSplitEnds, 1, splitEnds.Length);
                        splitEnds = newSplitEnds;
                    }
                }

                var startPointer = 0;

                for (int i = 0; i < splitEnds.Length; i++) {
                    var endPointer = splitEnds[i];
                    if (endPointer >= readLength)
                        break;
                    var copyLength = endPointer - startPointer + delimiter.Length;
                    byte[] doc;
                    if (ms.Length > 0) {
                        ms.Write(buffer, startPointer, copyLength);
                        doc = ms.ToArray();
                        ms = new MemoryStream(bufferSize * 2);
                    } else {
                        doc = new byte[copyLength];
                        Array.Copy(buffer, startPointer, doc, 0, copyLength);
                    }
#if DEBUG
                    var docStr = Encoding.UTF8.GetString(doc);
#endif
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
