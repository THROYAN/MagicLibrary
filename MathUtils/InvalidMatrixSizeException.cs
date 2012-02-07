using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public class InvalidMatrixSizeException : ApplicationException
    {
        public InvalidMatrixSizeException()
        {
        }

        public InvalidMatrixSizeException(string message)
            : base(message)
        {
        }

        public InvalidMatrixSizeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
