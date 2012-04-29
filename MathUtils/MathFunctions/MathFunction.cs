using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.MathUtils.Functions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class MathFunction : IMathFunction
    {
        public int ParamsCount { get; set; }

        public MathFunction(string name, int pCount, Func<FunctionElement[], FunctionElement> func, string formatString = "")
        {
            this.ParamsCount = pCount;
            this.FunctionName = name;
            this.Function = func;
            if (formatString == "")
                this.ToStringFormat = this.FunctionName + "({0})";
            else
                this.ToStringFormat = formatString;
        }

        public MathFunction(string func)
        {
            var mf = MathFunction.FromString(func);
            this.FunctionName = mf.FunctionName;
            this.Function = mf.Function;
            this.func = mf.func;
            this.ParamsCount = mf.ParamsCount;
            this.ToStringFormat = mf.ToStringFormat;
        }

        private Function func { get; set; }

        public static MathFunction FromString(string func)
        {
            string mask = @"^\s*? (?<name>\w+?\.*?) \s* \((?<params>.*?)\)\s*=\s*(?<body>.+)$";
            var m = Regex.Match(func, mask, RegexOptions.IgnorePatternWhitespace);

            if (!m.Success)
            {
                throw new Exception();
            }

            Function f = null;
            try
            {
                f = new Function(m.Groups["body"].Value);
            }
            catch
            {
                throw new Exception();
            }

            string[] vars = m.Groups["params"].Value.Split(',');
            string[] fVars = f.Variables;
            if (vars.Length != fVars.Length)
            {
                throw new Exception();
            }
            foreach (var var in fVars)
            {
                if (!vars.Contains(var))
                {
                    throw new Exception();
                }
            }
            return new MathFunction(m.Groups["name"].Value, vars.Length, delegate(FunctionElement[] d)
                {
                    Dictionary<string, FunctionElement> bindVars = new Dictionary<string, FunctionElement>();
                    for (int i = 0; i < vars.Length; i++)
                    {
                        if (!d[i].IsConstant())
                        {
                            var temp = d[0].Clone() as FunctionElement;
                            temp.ForceAddFunction(m.Groups["name"].Value, d.ToList().GetRange(1, vars.Length - 1).ToArray());
                            return temp;
                        }
                        bindVars[vars[i]] = d[i];
                    }
                    return f.SetVariablesValues(bindVars);

                }, m.Groups["name"] + "{0}");
        }

        public string FunctionName { get; protected set; }

        public FunctionElement Calculate(params FunctionElement[] parameters)
        {
            try
            {
                return this.Function(parameters);
            }
            catch (InvalidMathFunctionParameters)
            {
                throw new InvalidMathFunctionParameters(this.FunctionName);
            }
        }

        public string ToStringFormat { get; set; }

        public Func<FunctionElement[], FunctionElement> Function { get; set; }

        public IMathFunction ReverseFunction { get; set; }

        public virtual object Clone()
        {
            return new MathFunction(this.FunctionName, this.ParamsCount, this.Function, this.ToStringFormat) { ReverseFunction = this.ReverseFunction };
        }

        public virtual string ToStringML(string name, params FunctionElement[] _params)
        {
            List<string> els = new List<string>();
            els.Add(name);
            els.AddRange(_params.Select(p => p.ToMathML()));
            return String.Format(this.ToStringFormat, els.ToArray());
        }

        public virtual string ToString(string name, params FunctionElement[] _params)
        {
            List<string> els = new List<string>();
#warning ЧУШЬ блять!!!
            var f = new Function(name);
            if (f.IsNeedBrackets())
            {
                els.Add(String.Format("({0})", name));
            }
            else
            {
                els.Add(name);
            }
            els.AddRange(_params.Select(p => p.ToString()));
            return String.Format(Regex.Unescape(this.ToStringFormat), els.ToArray());
        }
    }
}
