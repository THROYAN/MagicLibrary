﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public class Equation
    {
        public Function LeftPart { get; set; }
        public Function RightPart { get; set; }
        private int sign = 0;
        public string Sign
        {
            get
            {
                switch (this.sign)
                {
                    case -1:
                        return "<=";
                    case -2:
                        return "<";
                    case 1:
                        return ">=";
                    case 2:
                        return ">";
                    default:
                        return "=";
                }
            }
            set
            {
                switch (value)
                {
                    case "<=":
                        this.sign = -1;
                        break;
                    case "<":
                        this.sign = -2;
                        break;
                    case ">=":
                        this.sign = 1;
                        break;
                    case ">":
                        this.sign = 2;
                        break;
                    case "=":
                        this.sign = 0;
                        break;
                }
            }
        }

        public Equation(Function leftPart, Function rightPart, string sign = "=")
        {
            this.LeftPart = leftPart.Clone() as Function;
            this.RightPart = rightPart.Clone() as Function;
            this.Sign = sign;
        }

        public Equation(Function leftPart, string sign = "=")
        {
            this.LeftPart = leftPart.Clone() as Function;
            this.RightPart = new Function(0);
            this.Sign = sign;
        }

        public void InverseSign()
        {
            this.sign *= -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {2} {1}", this.LeftPart.ToString(), this.RightPart.ToString(), this.Sign);
            return sb.ToString();
        }

        public Function AllToLeft()
        {
            return this.LeftPart - this.RightPart;
        }
    }
}
