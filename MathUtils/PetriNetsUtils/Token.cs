using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class Token
    {
        public uint Count { get; set; }
        public string Value { get; set; }

        public Token(string value, uint count = 1)
        {
            this.Count = count;
            this.Value = value;
        }

        public string ToString(IColorSet color)
        {
            return String.Format("{0}`{1}", this.Count, color.ShowToken(this.Value));
        }
    }
}
