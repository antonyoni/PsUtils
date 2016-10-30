using System.Text;
using Microsoft.PowerShell.Commands;

namespace PsUtils {

    public class CmdletEncoding {

        // From https://github.com/PowerShell/PowerShell/blob/5cbf1b43de52fe9025bf756990b1cc6eb4e694e9/src/System.Management.Automation/engine/Utils.cs
        public static Encoding Convert(FileSystemCmdletProviderEncoding encoding) {

            Encoding result = Encoding.Unicode;

            switch (encoding) {
                case FileSystemCmdletProviderEncoding.String:
                    result = new UnicodeEncoding();
                    break;

                case FileSystemCmdletProviderEncoding.Unicode:
                    result = new UnicodeEncoding();
                    break;

                case FileSystemCmdletProviderEncoding.BigEndianUnicode:
                    result = new UnicodeEncoding(true, false);
                    break;

                case FileSystemCmdletProviderEncoding.UTF8:
                    result = new UTF8Encoding();
                    break;

                case FileSystemCmdletProviderEncoding.UTF7:
                    result = new UTF7Encoding();
                    break;

                case FileSystemCmdletProviderEncoding.UTF32:
                    result = new UTF32Encoding();
                    break;
                /*
                case FileSystemCmdletProviderEncoding.BigEndianUTF32:
                    result = new UTF32Encoding(true, false);
                    break;
                */
                case FileSystemCmdletProviderEncoding.Ascii:
                    result = new ASCIIEncoding();
                    break;

                case FileSystemCmdletProviderEncoding.Default:
                    result = Encoding.Default;
                    break;

                case FileSystemCmdletProviderEncoding.Oem:
                    // Proper implementation done via PInvoke
                    result = Encoding.Default;
                    break;

                default:
                    // Default to unicode encoding
                    result = new UnicodeEncoding();
                    break;
            }

            return result;
        }

    }

}
