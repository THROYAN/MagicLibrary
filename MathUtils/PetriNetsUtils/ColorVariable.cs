using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class ColorVariable
    {
        public ColorSet ColorSet { get; set; }

        public string Name { get; set; }

        public ColorVariable(string name, ColorSet colorSet)
        {
            this.ColorSet = colorSet;
            this.Name = name;
        }

        public ColorVariable(string name, ColorSetCollection colors, string color) : this(name, colors[color]) { }
    }
}
