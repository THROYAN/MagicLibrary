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
            this.ParamsCount = mf.ParamsCount;
            this.ToStringFormat = mf.ToStringFormat;
        }

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
                throw new InvalidFunctionStringException(func);
            }

            string name = m.Groups["name"].Value;
            string[] tempVars = m.Groups["params"].Value.Split(',');
            string[] vars = new string[tempVars.Length];

            string formatString = "";
            for (int i = 0; i < tempVars.Length; i++)
            {
                formatString += "{" + i + "},";
                vars[i] = tempVars[i].Trim();
            }
            if (vars.Length > 0)
            {
                formatString = formatString.Remove(formatString.Length - 1, 1);
                if (vars.Length > 1)
                {
                    formatString = String.Format("\\({0}\\)", formatString);
                }
            }

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
            return new MathFunction(name, vars.Length, delegate(FunctionElement[] d)
                {
                    Dictionary<string, FunctionElement> bindVars = new Dictionary<string, FunctionElement>();
                    for (int i = 0; i < vars.Length; i++)
                    {
                        if (!d[i].IsDouble())
                        {
                            var temp = d[0].Clone() as FunctionElement;
                            temp.ForceAddFunction(m.Groups["name"].Value, d.ToList().GetRange(1, vars.Length - 1).ToArray());
                            return temp;
                        }
                        bindVars[vars[i]] = d[i];
                    }
                    return f.SetVariablesValues(bindVars);

                }, name + formatString);
        }

        public static MathFunction MultiLineFunction(string str)
        {
            string mask = @"^(\n\r\t)*\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*\((?<params>.*)\)\s*=\s*(?<body>(.|\n)*?)\s*$";

            var m = Regex.Match(str, mask, RegexOptions.IgnorePatternWhitespace);

            if (!m.Success)
            {
                throw new Exception();
            }

            string name = m.Groups["name"].Value;
            //var funcs = Regex.Split(m.Groups["body"].Value, "");
            var funcs = m.Groups["body"].Value.Split(new[] { ';', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            var tempPs = MagicLibrary.MathUtils.Functions.Function.ParseAttributes(m.Groups["params"].Value);
            if (!MagicLibrary.MathUtils.Functions.Function.IsArrayAttributes(tempPs))
            {
                throw new InvalidFunctionStringException(str);
            }
            string[] ps = new string[tempPs.Count];
            for (int i = 0; i < tempPs.Count; i++)
            {
                ps[i] = tempPs.ElementAt(i).Value;
            }

            string formatString = "";
            for (int i = 0; i < ps.Length; i++)
            {
                formatString += "{" + i + "},";
                ps[i] = ps[i].Trim();
            }

            if (ps.Length > 0)
            {
                formatString = formatString.Remove(formatString.Length - 1, 1);
                if (ps.Length > 1)
                {
                    formatString = String.Format("\\({0}\\)", formatString);
                }
            }

            Dictionary<string, Function> parameters = new Dictionary<string, Function>();
            
            int pCount = ps.Length;

            var j = 0;
            foreach (var func in funcs)
            {
                j++;
                mask = @"\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*=\s*(?<body>.*)\s*";
                m = Regex.Match(func, mask, RegexOptions.IgnorePatternWhitespace);

                if (!m.Success)
                {
                    mask = @"\s*return\s+(?<body>.*)\s*";
                    m = Regex.Match(func, mask, RegexOptions.IgnorePatternWhitespace);

                    if (!m.Success)
                    {
                        throw new InvalidFunctionStringException(func);
                    }
                }

                if (ps.Contains(m.Groups["name"].Value))
                {
                    throw new Exception();
                }

                Function f = new Function(m.Groups["body"].Value);

                foreach (var var in f.Variables)
                {
                    if (!parameters.ContainsKey(var) && !ps.Contains(var))
                    {
                        throw new InvalidFunctionStringException(func);
                    }
                }

                parameters.Add(m.Groups["name"].Value, f);

                if (String.IsNullOrEmpty(m.Groups["name"].Value))
                {
                    if (j != funcs.Length)
                    {
                        throw new InvalidFunctionStringException(func);
                    }

                    return new MathFunction(name, pCount, delegate(FunctionElement[] d)
                        {
                            Dictionary<string, FunctionElement> bindVars = new Dictionary<string, FunctionElement>();
                            
                            for (int i = 0; i < ps.Length; i++)
                            {
                                if (!d[i].IsDouble())
                                {
                                    var temp = d[0].Clone() as FunctionElement;
                                    temp.ForceAddFunction(name, d.ToList().GetRange(1, ps.Length - 1).ToArray());
                                    return temp;
                                }
                                bindVars[ps[i]] = d[i];
                            }

                            foreach (var item in parameters)
                            {
                                if (String.IsNullOrEmpty(item.Key))
                                {
                                    return item.Value.SetVariablesValues(bindVars);
                                }
                                bindVars.Add(item.Key, item.Value.SetVariablesValues(bindVars));
                            }
                            throw new Exception();
                        }, name + String.Format("{0}", formatString));
                }
            }

            throw new Exception();
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
#warning ЧУШЬ блять!!! ага, полная! ересь блять
            //var f = new Function(name);
            //if (f.IsNeedBrackets())
            //{
            //    els.Add(String.Format("({0})", name));
            //}
            //else
            {
                els.Add(name);
            }
            els.AddRange(_params.Select(p => p.ToString()));
            return String.Format(Regex.Unescape(this.ToStringFormat), els.ToArray());
        }
    }
}
