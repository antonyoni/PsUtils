using PsUtils.Validation;
using System;
using System.Management.Automation;
using Text = System.Text;

namespace PsUtils {

    [Cmdlet(VerbsCommon.Split, "Stream")]
    [OutputType(typeof(PSObject))]
    public class SplitStreamCmdlet : Cmdlet {

        #region Parameters

        [Parameter(Mandatory = true,
                   ValueFromPipeline = true,
                   Position = 1)]
        [ValidatePath()]
        public string Path {
            get { return this.path; }
            set { this.path = value; }
        }
        private string path;

        [Parameter(Mandatory = false)]
        public Text.Encoding Encoding {
            get { return this.encoding; }
            set { this.encoding = value; }
        }
        private Text.Encoding encoding = new Text.UTF8Encoding();

        [Parameter(Mandatory = false)]
        public string BlockDelimiter {
            get { return this.blockDelimiter; }
            set { this.blockDelimiter = value; }
        }
        private string blockDelimiter = "#DRE";

        #endregion

    }

}
