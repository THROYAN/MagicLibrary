using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.Exceptions;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    [Serializable]
    public class Token
    {
        private Function _value { get; set; }
        public string Value { get { return this._value.ToString(); } }
        public Function Function { get { return this._value; } }

        public ColorSet ColorSet { get; private set; }

        public Token(string value, ColorSet colorSet)
        {
            this._value = new Function(value);
            this.ColorSet = colorSet;
        }

        public Token(Function initFunc, ColorSet colorSet)
        {
            this._value = initFunc;
            this.ColorSet = colorSet;
        }

        //public Token(string initFunc, ColorSet colorSet)
        //{
        //    string[] masks = new string[]{
        //        @"^\s*(?<count>\d+)`\((?<value>.+)\)\s*",
        //        @"^\s*(?<count>\d+)`(?<value>.+)\s*",
        //        @"^\s*(?<value>.+)\s*"
        //    };
        //    Match m = null;
        //    foreach (var mask in masks)
        //    {
        //        m = Regex.Match(initFunc, mask);
        //        if (m.Success)
        //        {
        //            break;
        //        }
        //    }

        //    if (m != null && m.Success)
        //    {
        //        if (String.IsNullOrEmpty(m.Groups["count"].Value))
        //        {
        //            this.Count = 1;
        //        }
        //        else
        //        {
        //            this.Count = UInt32.Parse(m.Groups["count"].Value);
        //        }

        //        this.Value = m.Groups["value"].Value;
        //    }
        //    else
        //    {
        //        throw new InvalidTokenException(this.Value, "<?>");
        //    }

        //    this.ColorSet = colorSet;
        //}

        public Token(string value, ColorSetCollection colors, string colorName)
            : this(value, colors[colorName])
        { }

        public override string ToString()
        {
           return String.Format("{0}", this.ColorSet.ShowToken(this));
        }

        public bool IsLegal()
        {
            return this.ColorSet.IsLegal(this.Function);
        }
    }
}
