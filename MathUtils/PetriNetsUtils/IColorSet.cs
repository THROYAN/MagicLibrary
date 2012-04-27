using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public interface IColorSet
    {
        string Constructor(string value);
        bool IsLegal(string value);
        string ShowToken(string value);
    }
}
