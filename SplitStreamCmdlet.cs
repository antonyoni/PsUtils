using Microsoft.PowerShell.Commands;
using PsUtils.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Text = System.Text;

namespace PsUtils {

    [Cmdlet(VerbsCommon.Split, "Stream",
            DefaultParameterSetName = "Stream")]
    [OutputType(typeof(PSObject))]
    public class SplitStreamCmdlet : Cmdlet {

        #region Parameters

        [Parameter(Mandatory = true,
                   ParameterSetName = "Stream",
                   ValueFromPipeline = true,
                   Position = 1)]
        [Alias("FileStream")]
        public Stream Stream {
            get { return this.stream; }
            set { this.stream = value; }
        }
        private Stream stream;

        [Parameter(Mandatory = true,
                   ParameterSetName = "FilePath",
                   ValueFromPipeline = true,
                   Position = 1)]
        [ValidatePath()]
        public string Path {
            get { return this.path; }
            set { this.path = value; }
        }
        private string path;

        [Parameter(Mandatory = false,
                   Position = 2)]
        public string Delimiter {
            get { return this.delimiter; }
            set { this.delimiter = value; }
        }
        private string delimiter = Environment.NewLine;

        [Parameter(Mandatory = false)]
        public FileSystemCmdletProviderEncoding Encoding {
            get { return this.psEncoding; }
            set { this.psEncoding = value; }
        }
        private FileSystemCmdletProviderEncoding psEncoding = FileSystemCmdletProviderEncoding.Default;
        private Text.Encoding encoding;

        [Parameter(Mandatory = false)]
        public int BufferSize {
            get { return this.bufferSize; }
            set { this.bufferSize = value; }
        }
        private int bufferSize = 128 * 1024;

        #endregion

        #region Flow Functions

        protected override void BeginProcessing() {
            encoding = CmdletEncoding.Convert(Encoding);
        }

        protected override void ProcessRecord() {

            if (stream == null) {
                try {
                    stream = new FileStream(
                        Path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        BufferSize,
                        FileOptions.SequentialScan
                    );
                } catch (Exception e) {
                    WriteError(new ErrorRecord(e, "OpenStreamError", ErrorCategory.OpenError, Path));
                    return;
                }
            }

            // Discard encoding preamble
            var encPreamble = encoding.GetPreamble();
            if (encPreamble != null) {
                var paLen = encPreamble.Length;
                var paBuf = new byte[paLen];
                stream.Read(paBuf, 0, paLen);
                if (Array.Equals(encPreamble, paBuf)) {
                    WriteWarning(String.Format("BOM found ({0}) bytes). It will be ignored.", paLen));
                } else {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            var delimBytes = encoding.GetBytes(Delimiter);

            MemoryStream ms = new MemoryStream(BufferSize*2); // DocBuffer
            var buffer = new byte[BufferSize];
            int readLength = 0;

            while ((readLength = stream.Read(buffer, 0, buffer.Length)) > 0) {

                var splitEnds = KMP.FindAll(buffer, delimBytes);
                var startPointer = 0;

                for (int i = 0; i < splitEnds.Length; i++) {
                    var endPointer = splitEnds[i];
                    if (endPointer > readLength)
                        break;
                    var copyLength = endPointer - startPointer + delimBytes.Length;
                    byte[] doc;
                    if (ms.Length > 0) {
                        ms.Write(buffer, startPointer, copyLength);
                        doc = ms.ToArray();
                        ms = new MemoryStream(BufferSize * 2);
                    } else {
                        doc = new byte[copyLength];
                        Array.ConstrainedCopy(buffer, startPointer, doc, 0, copyLength);
                    }
                    WriteObject(doc);
                    startPointer = endPointer + delimBytes.Length;
                }

                // handle remaining bytes
                if (startPointer < readLength) {
                    var copyLength = readLength - startPointer;
                    ms.Write(buffer, startPointer, copyLength);
                }

            }

            // handle anything left in the DocBuffer
            if (ms.Length > 0) {
                WriteObject(ms.ToArray());
                ms = null;
            }

        }

        protected override void StopProcessing() {
            base.StopProcessing();
        }

        #endregion

        #region Helpers



        #endregion

    }

}
