using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class ColorSet
    {
        public ColorSetType Type { get; set; }
        public string Name { get; set; }
        public string Attributes { get; set; }
        public Dictionary<string, string> ParsedAttributes { get; private set; }

        public ColorSet(string name, ColorSetType type, string attrs = "")
        {
            this.Name = name;
            this.Type = type;
            this.Attributes = attrs;
            this.ParsedAttributes = this.ParseAttributes(this.Attributes);
        }

        KeyValuePair<string, string> parseAttribute(string attr)
        {
            if (Regex.IsMatch(attr, @"^\s?$"))
            {
                return new KeyValuePair<string,string>("", "");
            }
            string[] patterns = new string[] {
                @"^\s*(?<name>\w.*)\s*[=:]\s*(?<value>\w.*)\s*$", // Name = asd2
                @"^\s*(?<name>\w.*)\s*[=:]\s*(?<value>\d+)\s*$", // MinLength = 12
                "^\\s*(?<name>\\w.*)\\s*[=:]\\s*\"(?<value>.+)\"\\s*$", // Name ="+asd"
                @"^\s*(?<name>\w.*)\s*[=:]\s*'(?<value>.+)'\s*$", // name = '2asd'
                @"^\s*(?<value>\w.*)\s*$", // asd2
                @"^\s*'(?<value>.+)'\s*$", // 'asd2'
                "^\\s*\"(?<value>.+)\"\\s*$", // "asd2"
                @"^\s*(?<value>-?\d+)\s*$", // 15
            };
            Match m = null;
            foreach (var pattern in patterns)
            {
                m = Regex.Match(attr, pattern);
                if (m.Success)
                {
                    break;
                }
            }

            if (m == null || !m.Success)
            {
                throw new InvalidAttributesException(attr, this.Name);
            }

            string name = m.Groups["name"].Value.Trim();

            if (name == null)
            {
                name = "";
            }

            return new KeyValuePair<string, string>(name, m.Groups["value"].Value.Trim());
        }

        public void ParseAttributes()
        {
            this.ParsedAttributes = ParseAttributes(this.Attributes);
        }

        public Dictionary<string, string> ParseAttributes(string attrs)
        {
            if (attrs == null)
            {
                attrs = this.Attributes;
            }
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (Regex.IsMatch(attrs, @"^\s?$"))
                return d;
            /*switch (this.Type)
            {
                case ColorSetType.Unit:
                    var m = Regex.Match(attrs, @"^\s?[Nn]ame\s?=\s?(?<name>.+)\s?$");

                    if (m.Success)
                    {
                        d["Name"] = m.Groups["name"].Value;
                    }
                    else
                    {
                        m = Regex.Match(attrs, @"^\s?(?<name>.+)\s?$");
                        if (m.Success)
                        {
                            d["Name"] = m.Groups["name"].Value;
                        }
                        else
                        {
                            throw new InvalidColorSetAttributesException(this.Name, attrs);
                        }
                    }
                    break;
            }
            return d;*/

            var pAttrs = parseAttributes(attrs);

            int i = 0;
            foreach (var attr in pAttrs)
            {
                var pAttr = parseAttribute(attr);

                // array attrs
                if (pAttr.Key == "")
                {
                    d.Add(String.Format("[{0}]", ++i), pAttr.Value);
                }
                else
                {
                    d.Add(pAttr.Key, pAttr.Value);
                }
            }

            return d;
        }

        private string[] parseAttributes(string attrs)
        {
            return attrs.Split(',');
        }

        public bool IsLegal(Token token)
        {
            return true;
        }
        
        public string Constructor(Token token)
        {
            if (this.IsLegal(token))
            {
                switch (this.Type)
                {
                }
            }

            throw new InvalidTokenException(token.Value, this.Name);
        }
    }

    public enum ColorSetType { Int, String, Enum, Bool, Unit, Index, Record }
}
