using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidColorSetAttributesException:Exception
    {
        public InvalidColorSetAttributesException(string colorName, string attrs)
            :base(String.Format("Error in the definition of color '{0}'.", colorName, attrs))
        {

        }
    }
}
