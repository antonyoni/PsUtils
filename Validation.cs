using System.IO;
using System.Management.Automation;

namespace PsUtils.Validation {

    public class ValidatePathAttribute : ValidateArgumentsAttribute {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics) {
            string filePath = arguments as string;
            if (filePath == null || !File.Exists(filePath)) {
                throw new FileNotFoundException("The specified Path does not exist.");
            }
        }
    }

}
