using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.MathUtils.Functions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.MathFunctions
{
    [Serializable]
    public class MathFunction : IMathFunction
    {
        public bool IsNeedBracers { get; set; }

        public int ParamsCount { get; set; }

        public MathFunction(string name, int pCount, Func<FunctionElement[], FunctionElement> func,
            string formatString = "", bool isNeedBracers = false)
        {
            this.ParamsCount = pCount;
            this.FunctionName = name;
            this.MainFunction = func;
            this.IsNeedBracers = isNeedBracers;
            if (formatString == "")
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < pCount; i++)
                {
                    sb.Append("{0},");
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                this.ToStringFormat = String.Format("{0}({1})", this.FunctionName, sb.ToString());
            }
            else
            {
                this.ToStringFormat = formatString;
            }
        }

        public MathFunction(string func)
        {
            var mf = MathFunction.FromString(func);
            this.FunctionName = mf.FunctionName;
            this.MainFunction = mf.MainFunction;
            this.ParamsCount = mf.ParamsCount;
            this.ToStringFormat = mf.ToStringFormat;
            this.IsNeedBracers = false;
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
            string mask = @"^(\n\r\t)*\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*\((?<params>.*)\)(\s|\n|\r|\t)*\{(?<body>(.|\n)*?)\}(\s|\n|\r|\t)*$";

            var m = Regex.Match(str, mask, RegexOptions.IgnorePatternWhitespace);

            if (!m.Success)
            {
                throw new InvalidFunctionStringException(str);
            }

            string name = m.Groups["name"].Value;
            //var funcs = Regex.Split(m.Groups["body"].Value, "");
            
            var funcs = m.Groups["body"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            for (int i = 0; i < funcs.Length; i++)
			{
                funcs[i] = Function.NormalizeString(funcs[i]);
                if (!String.IsNullOrWhiteSpace(funcs[i]))
                {
                    count++;
                }
			}

            Dictionary<string, string> tempPs;
            try
            {
                tempPs = MagicLibrary.MathUtils.Functions.Function.ParseAttributes(m.Groups["params"].Value);
            }
            catch (InvalidAttributesException)
            {
                throw new InvalidFunctionStringException(str);
            }

            if (!MagicLibrary.MathUtils.Functions.Function.IsArrayAttributes(tempPs))
            {
                throw new InvalidFunctionStringException(str);
            }
            string[] ps = new string[tempPs.Count];
            for (int i = 0; i < tempPs.Count; i++)
            {
                if(ps.Contains(tempPs.ElementAt(i).Value))
                {
                    throw new InvalidFunctionStringException(str);
                }
                ps[i] = tempPs.ElementAt(i).Value;
            }

            string formatString = "";
            string fullName = "";
            for (int i = 0; i < ps.Length; i++)
            {
                formatString += "{" + i + "},";
                ps[i] = ps[i].Trim();
                fullName += String.Format("{0}, ", ps[i]);
            }

            if (ps.Length > 0)
            {
                formatString = formatString.Remove(formatString.Length - 1, 1);
                fullName = String.Format("{0}({1})", name, fullName.Remove(fullName.Length - 2, 2));
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
                if (!String.IsNullOrWhiteSpace(func))
                {
                    j++;
                }
                mask = @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*=\s*(?<body>.*)\s*$";
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

                if (ps.Contains(m.Groups["name"].Value) || parameters.ContainsKey(m.Groups["name"].Value))
                {
                    throw new InvalidFunctionStringException(func);
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
                    if (j != count)
                    {
                        throw new InvalidFunctionStringException(func);
                    }

                    return new MathFunction(fullName, pCount, delegate(FunctionElement[] d)
                        {
                            Dictionary<string, FunctionElement> bindVars = new Dictionary<string, FunctionElement>();
                            
                            for (int i = 0; i < ps.Length; i++)
                            {
                                if (!d[i].IsConstant())
                                {
                                    var temp = d[0].Clone() as FunctionElement;
                                    temp.ForceAddFunction(fullName, d.ToList().GetRange(1, ps.Length - 1).ToArray());
                                    return temp;
                                }
                                bindVars[ps[i]] = d[i];
                            }

                            foreach (var item in parameters)
                            {
                                if (String.IsNullOrEmpty(item.Key))
                                {
                                    try
                                    {
                                        return item.Value.SetVariablesValues(bindVars);
                                    }
                                    catch
                                    {
                                        var temp = d[0].Clone() as FunctionElement;
                                        temp.ForceAddFunction(fullName, d.ToList().GetRange(1, ps.Length - 1).ToArray());
                                        return temp;
                                    }
                                }
                                bindVars.Add(item.Key, item.Value.SetVariablesValues(bindVars));
                            }
                            throw new InvalidMathFunctionParameters();
                        }, name + String.Format("{0}", formatString));
                }
            }

            throw new InvalidFunctionStringException(str);
        }

        public string FunctionName { get; protected set; }

        public FunctionElement Calculate(params FunctionElement[] parameters)
        {
            try
            {
                return this.MainFunction(parameters);
            }
            catch (InvalidMathFunctionParameters)
            {
                throw new InvalidMathFunctionParameters(this.FunctionName);
            }
        }

        public FunctionElement CalculateReverse(params FunctionElement[] parameters)
        {
            try
            {
                if (this.HasReverse())
                {
                    return this.ReverseFunction(parameters);
                }
                else
                {
                    throw new InvalidMathFunctionParameters(this.FunctionName);
                }
            }
            catch (InvalidMathFunctionParameters)
            {
                throw new InvalidMathFunctionParameters(this.FunctionName);
            }
        }

        public string ToStringFormat { get; set; }

        public Func<FunctionElement[], FunctionElement> MainFunction { get; set; }

        public Func<FunctionElement[], FunctionElement> ReverseFunction { get; set; }

        public bool HasReverse()
        {
            return this.ReverseFunction != null;
        }

        public virtual object Clone()
        {
            return new MathFunction(this.FunctionName, this.ParamsCount, this.MainFunction, this.ToStringFormat) { ReverseFunction = this.ReverseFunction };
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
            
            els.Add(name);
            
            els.AddRange(_params.Select(p => String.Format(new Function(p).IsNeedBrackets() ? "({0})" : "{0}", p.ToString())));

            for (int i = 0; i < els.Count; i++)
            {
                els[i] = String.Format(" {0} ", els[i]);
            }
            els[0] = els[0].Remove(0, 1);
            els[els.Count - 1] = els[els.Count - 1].Remove(els[els.Count - 1].Length - 1, 1);

            return String.Format(String.Format(this.IsNeedBracers ? "({0})" : "{0}", Regex.Unescape(this.ToStringFormat)), els.ToArray());
        }
    }
}
