using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class UnitColorSet : IColorSet
    {
        public const string DefaultToken = "()";
        private string valueString;

        public UnitColorSet(string valueString = UnitColorSet.DefaultToken)
        {
            this.valueString = valueString;
        }

        public bool IsLegal(string value)
        {
            return value == UnitColorSet.DefaultToken || value == this.valueString;
        }

        public string Constructor(string value)
        {
            if (this.IsLegal(value))
            {
                return UnitColorSet.DefaultToken;
            }
            throw new InvalidTokenException(value, "Unit");
        }

        public string ShowToken(string value)
        {
            return this.valueString;
        }
    }
}
