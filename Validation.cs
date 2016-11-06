using System.IO;
using System.Management.Automation;

namespace PsUtils.Validation {

    public class ValidatePathAttribute : ValidateArgumentsAttribute {

        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics) {

            string path = arguments as string;

            if (path == null)
                throw new ValidationMetadataException("Invalid path parameter.");

            if (!Path.IsPathRooted(path)) {
                path = Path.Combine(
                    engineIntrinsics.SessionState.Path.CurrentFileSystemLocation.Path,
                    path
                );
            }
            
            if (!(File.Exists(path) || Directory.Exists(path))) {
                throw new FileNotFoundException("The specified Path does not exist.");
            }

        }

    }

}
