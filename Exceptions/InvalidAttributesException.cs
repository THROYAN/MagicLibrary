using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidAttributesException : Exception
    {
        public InvalidAttributesException(string attrs, string objName = "")
            : base(
                    String.Format("Error occured while parsing attributes '{0}'{1}.",
                        attrs,
                        objName != "" ? String.Format(" for object '{0}'", objName) : "")){}
    }
}
