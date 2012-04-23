using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace VersionOne.ServiceHost.Tests.Utility {
    public static class ResourceLoader {
        private static Stream LoadStream(string resourcename, Assembly asm) {
            if(asm == null) {
                throw (new ArgumentNullException("asm"));
            }

            var resourcestream = asm.GetManifestResourceStream(resourcename);

            if(resourcestream == null) {
                throw new MissingManifestResourceException(resourcename);
            }

            return resourcestream;
        }

        public static Stream LoadClassStream(string name, Type classtype) {
            if(classtype == null) {
                throw (new ArgumentNullException("classtype"));
            }

            var resourcename = classtype.Namespace + "." + name;
            var asm = classtype.Assembly;
            return LoadStream(resourcename, asm);
        }
    }
}