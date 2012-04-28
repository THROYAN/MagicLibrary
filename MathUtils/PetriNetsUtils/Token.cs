using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class Token
    {
        public uint Count { get; set; }
        public string Value { get; set; }

        public Token(string value, uint count)
        {
            this.Count = count;
            this.Value = value;
        }

        public Token(string initFunc)
        {
            string[] masks = new string[]{
                @"^\s*(?<count>\d+)`\((?<value>.+)\)\s*",
                @"^\s*(?<count>\d+)`(?<value>.+)\s*",
                @"^\s*(?<value>.+)\s*"
            };
            Match m = null;
            foreach (var mask in masks)
            {
                m = Regex.Match(initFunc, mask);
                if (m.Success)
                {
                    break;
                }
            }

            if (m != null && m.Success)
            {
                if (String.IsNullOrEmpty(m.Groups["count"].Value))
                {
                    this.Count = 1;
                }
                else
                {
                    this.Count = UInt32.Parse(m.Groups["count"].Value);
                }

                this.Value = m.Groups["value"].Value;
            }
            else
            {
                throw new InvalidTokenException(this.Value, "<?>");
            }
        }

        public string ToString(ColorSet color)
        {
            return String.Format("{0}`{1}", this.Count, color.ShowToken(this));
        }

        public override string ToString()
        {
            return String.Format("{0}`{1}", this.Count, this.Value);
        }
    }
}
