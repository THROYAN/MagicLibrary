using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;
using System.Text.RegularExpressions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.Functions
{
    public class Function : FunctionElement
    {
        #region Basic function
        /// <summary>
        /// All math function which you can use in your functions
        /// </summary>
        private static List<IMathFunction> _allFunctions = new List<IMathFunction>()
        {
            new MathOperator("plus", (f1, f2) => f1 + f2, "\\+"),
            new MathOperator("minus", (f1, f2) => f1 - f2, "-"),
            new MathOperator("multiply", (f1, f2) => f1 * f2, "\\*"),
            new MathOperator("divide", (f1, f2) => f1 / f2, "/"),

            new MathOperator("power", delegate(FunctionElement func, FunctionElement power)
                {
                    try{
                        func.ToDouble();
                        power.ToDouble();
                    }
                    catch(NotImplementedException)
                    {
                        // Если не определена функция перевода в дабл, то врятли когда-нибудь можно будет возвести эту штуку в степень
                        throw new InvalidMathFunctionParameters();
                    }
                    catch
                    {
                        // Остальные исключения не из-за типов, поэтому пропускаем
                    }

                    var f = func.Clone() as FunctionElement;

                    bool powerIsConstant = power.IsDouble();
                    // степень 1
                    if (powerIsConstant && power.ToDouble() == 1)
                    {
                        return f;
                    }
                    if (powerIsConstant && power.ToDouble() == 0)
                    {
                        return new Function(1);
                    }
                    // Если возводим число в степень число
                    if (powerIsConstant && f.IsDouble())
                    {
                        return new Function(Math.Pow(f.ToDouble(), power.ToDouble()));
                    }

                    var last = f.GetLastFunction();
                    if (last != null && last.Item1.Equals(Function.GetMathFunction("power")))
                    {
                        // Берем последнюю степень, удаляем её, перемножаем её с добавляемой степенью
                        // и применяем её опять (для пересёта)
                        f.MathFunctions.Remove(last);
                        last.Item2[0] *= power;

                        return f.ApplyFunction(last.Item1.FunctionName, last.Item2);
                    }

                    f.ForceAddFunction("power", power);
                    return f;
                    
                }, "\\^"),
                new OneParameterMathFunction("cos", delegate(FunctionElement func)
                    {
                        var f = func.Clone() as FunctionElement;

                        if(f.IsDouble())
                        {
                            return new Function(Math.Cos(f.ToDouble()));
                        }

                        f.ForceAddFunction("cos");
                        return f;

                    }, "cos{0}"),
                #region Relations
                new MathOperator("equals", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("true");
                            }
                        }

                        if (v1.IsConstant() && v2.IsConstant())
                        {
                            return new Function(v1.Equals(v2) ? "true" : "false");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("equals", e2.Clone() as FunctionElement);
                        return temp;

                    }, "=="),
                    new MathOperator("less then", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("false");
                            }
                        }

                        if (v1.IsDouble() && v2.IsDouble())
                        {
                            return new Function(v1.ToDouble() < v2.ToDouble() ? "true" : "false");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("less then", e2.Clone() as FunctionElement);
                        return temp;

                    }, "<"),
                    new MathOperator("greater then", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("false");
                            }
                        }

                        if (v1.IsDouble() && v2.IsDouble())
                        {
                            return new Function(v1.ToDouble() > v2.ToDouble() ? "true" : "false");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("greater then", e2.Clone() as FunctionElement);
                        return temp;

                    }, ">"),
                    new MathOperator("less or equals", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("true");
                            }
                        }

                        if (v1.IsDouble() && v2.IsDouble())
                        {
                            return new Function(v1.ToDouble() <= v2.ToDouble() ? "true" : "false");
                        }

                        if (v1.IsConstant() && v2.IsConstant())
                        {
                            return new Function(v1.Equals(v2) ? "true" : "false");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("less or equals", e2.Clone() as FunctionElement);
                        return temp;

                    }, "<="),
                    new MathOperator("greater or equals", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("true");
                            }
                        }

                        if (v1.IsDouble() && v2.IsDouble())
                        {
                            return new Function(v1.ToDouble() >= v2.ToDouble() ? "true" : "false");
                        }

                        if (v1.IsConstant() && v2.IsConstant())
                        {
                            return new Function(v1.Equals(v2) ? "true" : "false");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("greater or equals", e2.Clone() as FunctionElement);
                        return temp;

                    }, ">="),
                    new MathOperator("not equals", delegate(FunctionElement e1, FunctionElement e2)
                    {
                        var v1 = e1.ToLeaf() as FunctionElement;
                        var v2 = e2.ToLeaf() as FunctionElement;

                        if (v1 is Variable || v2 is Variable)
                        {
                            if (v1 is Variable && v2 is Variable && (v1 as Variable).Name.Equals(v2))
                            {
                                return new Function("false");
                            }
                        }

                        if (v1.IsConstant() && v2.IsConstant())
                        {
                            return new Function(v1.Equals(v2) ? "false" : "true");
                        }

                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("not equals", e2.Clone() as FunctionElement);
                        return temp;

                    }, "!="),
                    #endregion
        };
        #endregion

        //private static List<IMathFunction> _allVariables = new List<IMathFunction>()
        //{
        //    new OneParameterMathFunction("var", delegate(FunctionElement e)
        //        {
        //            return new Variable("");

        //        }, "\\{0}"),
        //};

        public static IMathFunction GetMathFunction(string name)
        {
            return Function._allFunctions.Find(mf => mf.FunctionName.Equals(name));
        }

        public static void RegisterMathFunction(IMathFunction func)
        {
            if (Function.GetMathFunction(func.FunctionName) != null)
            {
                throw new Exception(String.Format("It is already exist function with name '{0}'", func.FunctionName));
            }
            Function._allFunctions.Add(func);
        }

        public static bool IsArrayAttributes(string attrs)
        {
            return Function.ParseAttributes(attrs).ContainsKey(Function.KeyOfIndex(1));
        }

        public static bool IsArrayAttributes(Dictionary<string, string> attrs)
        {
            return attrs.ContainsKey(Function.KeyOfIndex(1));
        }

        private const string _stringMask = "[_s{0}]";
        private const string _innerAttrsMask = "[_iA{0}]";
        private const string _funcMask = "[_f{0}]";
        private const string _paramsMask = "[_p{0}]";

        public const string MaskOfArrayAttributes = "[{0}]";
        
        private const string _openBracer = "{";
        private const string _closeBracer = "}";

        /// <summary>
        /// Разбиение строки аттрибутов, на строки, содержащие по одному аттрибуту
        /// </summary>
        /// <param name="attrs">Строка аттрибутов</param>
        /// <returns>Массив строк аттрибутов</returns>
        private static string[] parseAttributes(string attributes, List<string> ass)
        {
            string attrs = attributes;
            string mask = String.Format(@".*(?<all>{0}(?<innerAttrs>.*?){1})", Regex.Escape(Function._openBracer), Regex.Escape(Function._closeBracer));

            var m = Regex.Match(attrs, mask);

            while (m.Success)
            {
                string innerAttrs = m.Groups["innerAttrs"].Value;
                string[] ss2 = Function.parseAttributes(innerAttrs, ass);
                attrs = attrs.Replace(m.Groups["all"].Value, String.Format(Function._innerAttrsMask, ass.Count));

                StringBuilder sb = new StringBuilder();
                foreach (var s in ss2)
                {
                    sb.AppendFormat("{0},", s);
                }
                if (sb.Length > 0)
                {
                    sb = sb.Remove(sb.Length - 1, 1);
                }
                ass.Add(sb.ToString());

                m = Regex.Match(attrs, mask);
            };

            var ss = attrs.Split(',');

            //for (int i = 0; i < ass.Count; i++)
            //{
            //    for (int j = 0; j < ss.Length; j++)
            //    {
            //        ss[j] = ss[j].Replace(String.Format("[_iA{0}]", i), ass[i]);
            //    }
            //}
            return ss;
        }

        /// <summary>
        /// Парсинг аттрибутов из строки аттрибутов.
        /// Аттрибуты записываются через ','.
        /// </summary>
        /// <param name="attrs">Аттрибуты в виде строки</param>
        /// <returns>Словарь, где ключи - это имена аттрибутов, а значения - это значения аттрибутов.</returns>
        public static Dictionary<string, string> ParseAttributes(string attributes)
        {
            string attrs = attributes;
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (Regex.IsMatch(attrs, @"^\s?$"))
                return d;

            // Вложенные аттрибуты
            List<string> ass = new List<string>();
            // строки на замену
            List<string> ss = new List<string>();
            string mask = @".*?(?<str>\'.*?\')";

            var m = Regex.Match(attrs, mask);

            while (m.Success)
            {
                attrs = attrs.Replace(m.Groups["str"].Value, String.Format("[_s{0}]", ss.Count));
                ss.Add(m.Groups["str"].Value);

                m = Regex.Match(attrs, mask);
            }

            var pAttrs = parseAttributes(attrs, ass);

            int i = 0, j = 0;
            foreach (var attr in pAttrs)
            {
                var pAttr = parseAttribute(attr, ss, ass);

                // array attrs
                if (pAttr.Key == "")
                {
                    // Если где-то в середине был именованный аттрибут, то это плохо
                    if (i != j)
                    {
                        throw new InvalidAttributesException(attrs, "<?>");
                    }
                    d.Add(Function.KeyOfIndex(++i), pAttr.Value);
                }
                else
                {
                    // Если был массив, а тут появился именованный аттрибут
                    if (i != 0)
                    {
                        throw new InvalidAttributesException(attrs, "<?>");
                    }

                    // Повторяющиеся аттрибуты - это лажа
                    if (d.ContainsKey(pAttr.Key))
                    {
                        throw new InvalidAttributesException(attrs, "<?>");
                    }

                    d.Add(pAttr.Key, pAttr.Value);
                }
                j++;
            }

            return d;
        }

        /// <summary>
        /// Парсинг аттрибута из строки, содержащей 1 аттрибут.
        /// 1) Имя=Значение
        /// 2) Значение
        /// 3) Имя = 'Значение'
        /// 4) Имя: "Значение"
        /// ...
        /// </summary>
        /// <param name="attr">Аттрибут, в виде строки</param>
        /// <returns>Пара - имя и значение аттрибута</returns>
        private static KeyValuePair<string, string> parseAttribute(string attribute, List<string> ss, List<string> ass)
        {
            string attr = attribute;

            //for (int i = 0; i < ss.Count; i++)
            //{
            //    attr = attr.Replace(String.Format(ColorSet._stringMask, i), ss[i]);
            //}

            if (Regex.IsMatch(attr, @"^\s?$"))
            {
                return new KeyValuePair<string, string>("", "");
            }
            string[] patterns = new string[] {
                //"^\\s*(?<name>\\w.*)\\s*[=:]\\s*\"(?<value>.+)\"\\s*$", // Name ="+asd"
                @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*[=:]\s*(?<value>'.+')\s*$", // name = '2asd'
                @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*[=:]\s*(?<value>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*$", // Name = asd2
                @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*[=:]\s*(?<value>-?\d+)\s*$", // MinLength = -12
                String.Format(
                        @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*[=:]\s*{0}\s*$",
                        Regex.Escape(String.Format(Function._innerAttrsMask, "```")).Replace("```", "(?<iAIndex>\\d+)")
                ),
                String.Format(
                        @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*[=:]\s*{0}\s*$",
                        Regex.Escape(String.Format(Function._stringMask, "```")).Replace("```", "(?<sIndex>\\d+)")
                ),
                //"^\\s*\"(?<value>.+)\"\\s*$", // "asd2"
                @"^\s*(?<value>'.+')\s*$", // 'asd2'
                String.Format(
                        @"^\s*{0}\s*$",
                        Regex.Escape(String.Format(Function._innerAttrsMask, "```")).Replace("```", "(?<iAIndex>\\d+)")
                ),
                String.Format(
                        @"^\s*{0}\s*$",
                        Regex.Escape(String.Format(Function._stringMask, "```")).Replace("```", "(?<sIndex>\\d+)")
                ),
                @"^\s*(?<value>-?\d+)\s*$", // 15
                @"^\s*(?<value>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*$", // asd2
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
                throw new InvalidAttributesException(attr, "<?>");
            }

            string name = m.Groups["name"].Value.Trim();

            if (name == null)
            {
                name = "";
            }
            string value = "";
            if (m.Groups["iAIndex"].Success)
            {
                value = ass[Int32.Parse(m.Groups["iAIndex"].Value)];
                var temp = Function.parseAttributes(value, ass);
                StringBuilder sb = new StringBuilder();
                foreach (var temp2 in temp)
                {
                    var a = Function.parseAttribute(temp2, ss, ass);
                    sb.AppendFormat("{0}={1},", a.Key, a.Value);
                }
                sb = sb.Remove(sb.Length - 1, 1);
                value = String.Format("{1}{0}{2}", sb.ToString(), Function._openBracer, Function._closeBracer);
            }
            else
            {
                if (m.Groups["sIndex"].Success)
                {
                    value = ss[Int32.Parse(m.Groups["sIndex"].Value)];
                }
                else
                {
                    value = m.Groups["value"].Value.Trim();
                }
            }


            return new KeyValuePair<string, string>(name, value);
        }

        /// <summary>
        /// Имя индексного аттрибута (массива)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string KeyOfIndex(int index)
        {
            return String.Format(Function.MaskOfArrayAttributes, index);
        }

        private List<VariablesMulriplication> variables;
        private List<VariablesMulriplication> notNullVariables { get { return this.variables.Where(vs => vs.Constant != 0).ToList(); } }

        private void _initProperties()
        {
            this.MathFunctions = new List<Tuple<IMathFunction, FunctionElement[]>>();
            this.variables = new List<VariablesMulriplication>();
            this._isModified = true;
        }

        public Function(double constant = 0, string varName = "")
        {
            this._initProperties();
            this.AddVariablesMul(new VariablesMulriplication(new Variable(varName), constant));
        }

        public Function(string func)
        {
            this.ReplaceThisWithElement(Function.FromString(func));
        }

        public Function(VariablesMulriplication v)
        {
            this._initProperties();

            this.AddVariablesMul(v);
        }

        public Function(VariablesMulriplication[] variables)
        {
            this._initProperties();

            foreach (var v in variables)
            {
                this.AddVariablesMul(v);
            }
        }

        public Function(FunctionElement el)
        {
            this.ReplaceThisWithElement(el);
        }

        public void ReplaceThisWithElement(FunctionElement el)
        {
            this._initProperties();
            this._isModified = true;

            if (el is Function)
            {
                this.variables = new List<VariablesMulriplication>((el as Function).variables);
                this.CopyFunctions(el);
            }
            else
            {
                this.AddVariablesMul(new VariablesMulriplication(el));
                this.CopyFunctions(el);
            }
        }

        public static Function FromString(string func)
        {
            List<Function> fs = new List<Function>();

            try
            {
                return Function._fromString(func, fs);
            }
            catch(InvalidFunctionStringException)
            {
                throw new InvalidFunctionStringException(func);
            }
        }

        private static Function _fromString(string func, List<Function> fs)
        {
            string f = func;

            // Ищем все строки в одинарных кавычках
            List<string> ss = new List<string>();
            string mask = @".*?(?<str>\'.*?\')";

            var m = Regex.Match(f, mask);

            while (m.Success)
            {
                // скрываем их, чтобы не мешали своими скобками и запятыми
                f = f.Replace(m.Groups["str"].Value, String.Format(Function._stringMask, ss.Count));
                ss.Add(m.Groups["str"].Value);

                m = Regex.Match(f, mask);
            }

            // теперь ищем все параметры функций (больше 1-го параметра), тоесть что-то в скобках и через запятую
            mask = @".*(?<all>\(\s*(?<params>.*,.*?)\))";

            m = Regex.Match(f, mask);

            List<string> ps = new List<string>();

            while (m.Success)
            {
                // тоже скрываем их
                f = f.Replace(m.Groups["all"].Value, String.Format(Function._paramsMask, ps.Count));
                ps.Add(m.Groups["params"].Value);

                m = Regex.Match(f, mask);
            }

            // теперь разбираем все выражения в скобках
            mask = @".*(?<all>\(\s*(?<function>.+?)\))";

            m = Regex.Match(f, mask);
            // Все скобки заменяем на ссылки на функции
            while (m.Success)
            {
                string function = m.Groups["function"].Value;

                // выбрасываем обратно параметры, потому что они могут быть в этих скобках
                for (int i = 0; i < ps.Count; i++)
                {
                    function = function.Replace(String.Format(Function._paramsMask, i), String.Format("({0})", ps[i]));
                }

                // и строки выбрасываем, чтобы не тащить их за собой повсюду... хотя не знаю что лучше
                for (int i = 0; i < ss.Count; i++)
                {
                    function = function.Replace(String.Format(Function._stringMask, i), ss[i]);
                }

                // парсим внутренность этих скобок, результат сохраняем в fs
                fs.Add(Function._fromString(function, fs));
                // и скрываем всю скобку
                f = f.Replace(m.Groups["all"].Value, String.Format(Function._funcMask, fs.Count - 1));

                m = Regex.Match(f, mask);
            }
            
            // когда осталось "голое" выражение (без скобок)
            // пересчитываем все параметры, т.е. каждое выражение через запятую
            for (int i = 0; i < ps.Count; i++)
            {
                // если эти параметры - это параметры функции, которая была в скобках, то их пересчитывать уже не нужно
                if (f.IndexOf(String.Format(Function._paramsMask, i)) != -1)
                {
                    var pss = ps[i].Split(',');
                    string res = "";
                    // разбираем каждый параметр отдельно
                    foreach (var p in pss)
                    {
                        string p1 = p;
                        // возвращаем все строки в параметр
                        for (int j = 0; j < ss.Count; j++)
                        {
                            p1 = p1.Replace(String.Format(Function._stringMask, j), ss[j]);
                        }

                        // и сейчас вот послушайте... еще в каждом параметре могут быть другие параметры....яебал
                        for (int g = 0; g < ps.Count; g++)
                        {
                            p1 = p1.Replace(String.Format(Function._paramsMask, g), String.Format("({0})", ps[g]));
                        }

                        // результат сохраняем в fs
                        fs.Add(Function._fromString(p1, fs));
                        // вместо параметра записываем ссылку на его функцию в fs
                        res += String.Format(Function._funcMask, fs.Count - 1) + ",";
                    }

                    res = res.Remove(res.Length - 1, 1);

                    // заменяем все параметры на их ссылки + скобки
                    f = f.Replace(String.Format(Function._paramsMask, i), String.Format("({0})", res));
                }
            }

            for (int i = 0; i < ss.Count; i++)
            {
                f = f.Replace(String.Format(Function._stringMask, i), ss[i]);
            }

            return Function._fromStringWitoutBracers(f, fs);
        }

        private static Function _fromStringWitoutBracers(string func, List<Function> fs)
        {
            string f = func.Trim();

            string _fMask = String.Format("^{0}$", Regex.Escape(String.Format(Function._funcMask, "```")).Replace("```", "(?<index>\\d+)"));
            string mask = _fMask;//@"^\[_f(?<index>\d+)\]$";
            var m = Regex.Match(f, mask);

            if (m.Success)
            {
                var index = Int32.Parse(m.Groups["index"].Value);
                if (index >= fs.Count)
                {
                    throw new InvalidFunctionStringException(func);
                }
                return fs[index];
            }

            // check math functions
            foreach (var mf in Function._allFunctions)
            {
                string[] _paramsIndexes = new string[mf.ParamsCount];
                for (int i = 0; i < mf.ParamsCount; i++)
                {
                    _paramsIndexes[i] = String.Format(@"\s*(?<p{0}>.+)\s*", i);
                }

                mask = String.Format(mf.ToStringFormat, _paramsIndexes);

                m = Regex.Match(f, mask);

                if (m.Success)
                {
                    try
                    {
                        FunctionElement[] _params = new FunctionElement[mf.ParamsCount];
                        for (int i = 0; i < mf.ParamsCount; i++)
                        {
                            _params[i] = Function._fromStringWitoutBracers(m.Groups[String.Format("p{0}", i)].Value, fs);
                        }

                        try
                        {
                            return new Function(mf.Calculate(_params));
                        } // Skip this, because it can be other function with same signature.. Блять, прям перегрузка функций
                        catch (InvalidMathFunctionParameters)
                        { }
                    }
                    catch (InvalidFunctionStringException)
                    { }
                }
            }

            // Digit
            mask = @"^\s*(?<value>\d+([\.\,]\d+)?)\s*$";

            m = Regex.Match(f, mask);

            if (m.Success)
            {
                return new Function(Double.Parse(m.Groups["value"].Value));
            }

            // Variable
            try
            {
                var v = new Variable("");
                v.ParseFromString(f);
                return new Function(v);
            }
            catch { }

            try
            {
                var s = new StringVariable("");
                s.ParseFromString(f);
                return new Function(s);
            }
            catch { }

            try
            {
                if (f.Contains('{'))
                {
                    for (int i = 0; i < fs.Count; i++)
                    {
                        if (f.IndexOf(String.Format(Function._funcMask, i)) != -1)
                        {
                            f = f.Replace(String.Format(Function._funcMask, i), String.Format("({0})", fs[i].ToString()));
                        }
                    }
                    var v = new RecordVariable(f);
                    return new Function(v);
                }
            }
            catch { }

            throw new InvalidFunctionStringException(func);
        }

        public void AddVariablesMul(VariablesMulriplication variablesMulriplication)
        {
            this._isModified = true;

            var temp = variablesMulriplication.ToString();
            if (temp == "0" || temp == "")
                return;
            var v = this.variables.Find(vs => vs.Equals(variablesMulriplication));
            if (v != null)
            {
                v.Constant += variablesMulriplication.Constant;
            }
            else
            {
                if (variablesMulriplication.IsFunction())
                {
                    //this.variables.Add(variablesMulriplication);
                    //(this + variablesMulriplication.ToFunction()).CopyTo(this);
                    var func = variablesMulriplication.ToFunction();
                    if (func.MathFunctions.Count != 0)
                    {
                        this.variables.Add(variablesMulriplication);
                    }
                    else
                    {
                        func.variables.ForEach(vs => this.AddVariablesMul(vs));
                    }
                }
                else
                {
                    this.variables.Add(variablesMulriplication);
                }
            }
        }

        private static Function _add(Function f1, Function f2)
        {
            Function f = new Function();

            foreach (VariablesMulriplication vars in f1.variables)
            {
                if (vars.Constant != 0)
                {
                    f.AddVariablesMul(vars.Clone() as VariablesMulriplication);
                }
            }

            foreach (VariablesMulriplication vars in f2.variables)
            {
                if (vars.Constant != 0)
                {
                    f.AddVariablesMul(vars.Clone() as VariablesMulriplication);
                }
            }

            return f;
        }

        private static Function _mul(Function f1, Function f2)
        {
            Function f = new Function();

            foreach (VariablesMulriplication vars1 in f1.variables)
            {
                foreach (VariablesMulriplication vars2 in f2.variables)
                {
                    f += vars1 * vars2;
                }
            }

            return f;
        }

        public Function ForceMul(FunctionElement e)
        {
            if (e.IsDouble() && e.ToDouble() == 1)
            {
                return this.Clone() as Function;
            }
            if (this.MathFunctions.Count != 0)
            {
                return this * e;
            }
            Function f = new Function();

            this.variables.ForEach(vs => f += vs * e);
            f.CopyFunctions(this);
            return f;
        }
/*
        #region operator +
        //public static Function operator +(Function f1, Function f2)
        //{
        //    return Function._add(f1, f2);
        //}

        //public static Function operator +(Function f, VariablesMulriplication vs)
        //{
        //    return Function._add(f, new Function(vs));
        //}

        //public static Function operator +(VariablesMulriplication vs, Function f)
        //{
        //    return f + vs;
        //}

        //public static Function operator +(Function f, FunctionElement v)
        //{
        //    return f + new Function(v);
        //}

        //public static Function operator +(FunctionElement v, Function f)
        //{
        //    return f + v;
        //}

        //public static Function operator +(Function f, double d)
        //{
        //    return f + new Function(d);
        //}

        //public static Function operator +(double d, Function f)
        //{
        //    return f + d;
        //}

        #endregion

        #region operator -
        //public static Function operator -(Function f1, Function f2)
        //{
        //    return f1 + f2 * -1;
        //}

        //public static Function operator -(Function f, VariablesMulriplication vs)
        //{
        //    return f + vs * -1;
        //}

        //public static Function operator -(VariablesMulriplication vs, Function f)
        //{
        //    return new Function(vs) - f;
        //}

        //public static Function operator -(Function f, FunctionElement v)
        //{
        //    return f - new Function(v);
        //}

        //public static Function operator -(FunctionElement v, Function f)
        //{
        //    return new Function(v) - f;
        //}

        //public static Function operator -(Function f, double d)
        //{
        //    return f + new Function(-d);
        //}

        //public static Function operator -(double d, Function f)
        //{
        //    return new Function(d) - f;
        //}
        #endregion

        #region operator *
        //public static Function operator *(Function f1, Function f2)
        //{
        //    return Function._mul(f1, f2);
        //}

        //public static Function operator *(Function f, VariablesMulriplication vs)
        //{
        //    return f * new Function(vs);
        //}

        //public static Function operator *(VariablesMulriplication vs, Function f)
        //{
        //    return f * vs;
        //}

        //public static Function operator *(Function f, FunctionElement v)
        //{
        //    return f * new Function(v);
        //    if ((v is Variable || v.IsConstant()) && f.Degree == 1)
        //    {
        //        Function newF = new Function();
        //        foreach (VariablesMulriplication vars in f.variables)
        //        {
        //            newF += vars * v;
        //        }
        //        return newF;
        //    }
        //    else
        //    {
        //        return new Function(new VariablesMulriplication(new FunctionElement[] { f, v }));
        //    }
        //}

        //public static Function operator *(FunctionElement v, Function f)
        //{
        //    return f * v;
        //}

        //public static Function operator *(Function f, double d)
        //{
        //    if (f.Degree != 1)
        //    {
        //        Function F = new Function();
        //        F.AddVariablesMul(new VariablesMulriplication(d));
        //        F.AddVariablesMul(new VariablesMulriplication(f));
        //        return F;
        //    }
        //    Function newF = new Function();
        //    foreach (VariablesMulriplication vars in f.variables)
        //    {
        //        newF += vars * d;
        //    }
        //    return newF;
        //}

        //public static Function operator *(double d, Function f)
        //{
        //    return f * d;
        //}
        #endregion

        #region operator /
//        public static Function operator /(Function f1, Function f2)
//        {
//#warning Сделать деление функций
//            if (f2.IsConstant())
//            {
//                return f1 / f2.ToDouble();
//            }

//            if (f2.VarsCount > 1 || (f2.VarsCount == 0 && f2.ToDouble() == 0))
//            {
//                Function f = f1.Clone() as Function;
//                Function f22 = f2.Clone() as Function;
//                f22.Degree *= -1;
//                f.variables.Add(new VariablesMulriplication(f2));
//                return f;
//                throw new Exception("Я еще не умею так делить.");
//            }
//            Function res = new Function();
//            VariablesMulriplication d = f2.variables.Find(v => v.VarsCount != 0);

//            foreach (var v in f1.variables)
//            {
//                res += v / d;
//            }
//            return res;
//        }

//        public static Function operator /(Function f, VariablesMulriplication vs)
//        {
//            Function newF = new Function();
//            foreach (var v in f.variables)
//            {
//                newF += v / vs;
//            }
//            return newF;
//        }

//        public static Function operator /(VariablesMulriplication vs, Function f)
//        {
//            return new Function(vs) / f;
//        }

//        public static Function operator /(Function f, FunctionElement v)
//        {
//            Function newF = new Function();
//            foreach (var vs in f.variables)
//            {
//                newF += vs / v;
//            }
//            return newF;
//        }

//        public static Function operator /(FunctionElement v, Function f)
//        {
//            return new Function(new VariablesMulriplication(v)) / f;
//        }

//        public static Function operator /(Function f, double d)
//        {
//            Function newF = new Function();
//            foreach (var vs in f.variables)
//            {
//                newF += vs / d;
//            }
//            return newF;
//        }

//        public static Function operator /(double d, Function f)
//        {
//            return new Function(d) / f;
//        }

        #endregion
        */
        public override object Clone()
        {
            List<VariablesMulriplication> vars = new List<VariablesMulriplication>();
            foreach (var vs in this.variables)
            {
                if (vs.Constant != 0)
                {
                    vars.Add(vs.Clone() as VariablesMulriplication);
                }
            }
            
            var temp = new Function(vars.ToArray());
            temp.MathFunctions.Clear();
            temp.ForceAddFunctions(this);
            return temp;
        }

        public Function Inverse()
        {
            return this * -1;
        }

        public override FunctionElement Pow(double power)
        {
            /*if (this.IsConstant())
            {
                return new Function(Math.Pow(this.ToDouble(), power));
            }*/
            return this.ApplyFunction("power", new Function(power));
        }

        public FunctionElement Sqrt()
        {
            return this.Pow(0.5);
        }

        public override double ToDouble()
        {
            if (this.variables.Count(vs => vs.Constant != 0) == 1)
            {
                var v = this.variables.Find(vs => vs.Constant != 0);
                return v.Constant * v[0].ToDouble();
            }
            if (this.variables.Count(vs => vs.Constant != 0) == 0)
            {
                return 0;
            }
            //var v = this.variables.Find(vs => vs.VarsCount == 0 && vs.Constant != 0);
            throw new Exception();
        }

        public FunctionElement SetVariablesValues(Dictionary<string, double> dict)
        {
            Function e = this.Clone() as Function;

            foreach (var item in dict)
            {
                e = e.SetVariableValue(item.Key, item.Value) as Function;
            }

            if (e.IsDouble())
            {
                return new Function(e.Calculate(e.ToDouble()));
            }

            return e;
        }

        public Equation EquationSolutionByVariable(string name, Function equationAnswer = null)
        {
            if (equationAnswer == null)
                equationAnswer = new Function(0);
            Equation equation = new Equation(
                this - equationAnswer,
                new Function(0)
            );

            Function curX, nextX = new Function(1);
            double eps = 0.0001;
            Function derivative = new Function(equation.LeftPart.Derivative());
            do
            {
                curX = nextX;
                var up = equation.LeftPart.SetVariableValue(name, curX);
                var down = derivative.SetVariableValue(name, curX);
                nextX = curX - up / down;
            }
            while (Math.Abs((curX - nextX).ToDouble()) > eps);
            return new Equation(new Function(1, name), nextX);

#warning Посидим пока без решений уравнений
            #region Как в школе учили
            /*
            double degree = equation.LeftPart.Degree * equation.LeftPart.variables.Max(delegate(VariablesMulriplication vs)
            {
                if (vs.HasVariable(name))
                {
                    return vs.GetVariableByName(name).Degree;
                }
                return 0;
            });

            switch (degree.ToString())
            {
                case "1":
                    equation.LeftPart.variables.ForEach(delegate(VariablesMulriplication vs)
                    {
                        if (vs.HasVariable(name))
                        {
                            if (vs.Constant < 0)
                            {
                                equation.InverseSign();
                            }
                            var newV = vs.GetVariableByName(name);
                            //leftPart.AddVariablesMul(newV);
                            equation.LeftPart *= newV / vs;
                            equation.RightPart *= newV / vs;
                        }
                        else
                        {
                            //rightPart = rightPart.Mul(new EquationMatrixElement(new VariablesMulriplication(1) / vs)) as EquationMatrixElement;
                            equation.RightPart -= vs;
                        }
                    });
                    equation.LeftPart = new Function(1, name);
                    break;
                // Квадратное уравнение
                case "2":
                    // Всё умножения, содержащие нужную переменную в 1 степени
                    var v = equation.LeftPart.GetElementsWithVariable(name, 1);
                    // И во 2
                    var v2 = equation.LeftPart.GetElementsWithVariable(name, 2);
                    // Если есть только 2 степень
                    if (v.VarsCount == 0)
                    {
                        // берём коэффициенты при 2 степени, т.е. делим всё, что есть со 2 степенью на эту переменную во второй степени
                        var elements = v2 / new Variable(name, 2);

                        // правая часть - это всё без переменной во 2 степени с минусом и делённое на коэффициент при переменной во 2 степеи
                        var right = equation.LeftPart.GetElementsWithoutVariable(name, 2) / elements * -1;

                        // берём корень
                        equation.RightPart = new Function(right.Sqrt());
                        equation.LeftPart = new Function(1, name);
                        // всё
                        return equation;
                    }
                    else
                    {
                        // коэффциент при второй степени
                        Function a = v2 / new Variable(name, 2);
                        // при первой
                        Function b = v / new Variable(name, 1);
                        // и всё, что осталось
                        Function c = equation.LeftPart - (v + v2);

                        // доделываем всё это дело до квадрата суммы

                        // Находим какое должно быть c, чтобы это было квадрат суммы
                        Function neededC = new Function((b / (a.Sqrt() * 2)).Pow(2));

                        // находим остаток, например нужно c = 5, а есть c = -4, делаем 5 + (-4 - 5) = -4
                        var rightC = c - neededC;
                        if (rightC.VarsCount > 0 || rightC.ToDouble() <= 0)
                        {
                            // Переносим вправо и берём корень
                            equation.RightPart = rightC.Inverse().Sqrt() as Function;

                            // узнаём ли у нас квадрат суммы или разности

                            // Слева остаётся квадрат суммы, типа взяли корень, осталось sqrt(a) +(-) sqrt(neededC) = sqrt(-rightC)
                            // Делаем так, чтобы осталось только наша переменная
                            equation.RightPart -= b / (a.Sqrt() * 2);
                            equation.RightPart /= a.Sqrt();


                            equation.LeftPart = new Function(1, name);
                            return equation;
                        }

                        // Если это не сумма квадратов, решаем квадратное уравнение
                        Function d = b.Pow(2) - a * c * 4;
                        if (d.VarsCount > 0 || d.ToDouble() >= 0)
                        {
                            Function x1 = b.Inverse() + d.Sqrt() / (a * 2);
                            Function x2 = b.Inverse() - d.Sqrt() / (a * 2);
                            equation.RightPart = x1;
                            equation.LeftPart = new Function(1, name);
                            return equation;
                        }
                        else
                        {
                            throw new Exception("Нет решений. Я еще не знаю комплексных чисел. =)");
                        }
                    }
                default:
                    break;
            }

            return equation;*/
            #endregion
        }

        public Function GetElementsWithVariable(string name)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (vs.HasVariable(name))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }
        public Function GetElementsWithVariable(string name, double degree)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (vs.HasVariable(name, degree))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        public Function GetElementsWithoutVariable(string name)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (!vs.HasVariable(name))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        public Function GetElementsWithoutVariable(string name, double degree)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (!vs.HasVariable(name, degree))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        /// <summary>
        /// Set variables in equation.
        /// If use vector, variables are set by adding order.
        /// Using valuesVector, when first col is variables name and second is values, solve uncertainly.
        /// If variables repeated, it sets only first value.
        /// </summary>
        /// <param name="valuesVector">Vector or valuesVector.</param>
        /// <returns></returns>
        public FunctionElement SetVariablesValues(Matrix valuesVector)
        {
            if (valuesVector.Cols == 1)
            {
                //if (valuesVector.Rows > this.VarsCount)
                //{
                //    throw new InvalidMatrixSizeException("Too much values");
                //}
                string[] vars = this.Variables;
                Dictionary<string, IMatrixElement> dict = new Dictionary<string, IMatrixElement>();
                for (int i = 0; i < valuesVector.Rows && i < vars.Length; i++)
                {
                    //this.SetVariableValue(vars[i], valuesVector[i, 0].ToDouble());
                    dict.Add(vars[i], valuesVector[i, 0]);
                }
                return this.SetVariablesValues(dict);
            }
            if (valuesVector.Cols == 2)
            {
                Dictionary<string, IMatrixElement> dict = new Dictionary<string, IMatrixElement>();
                for (int i = 0; i < valuesVector.Rows; i++)
                {
                    //this.SetVariableValue(valuesVector[i, 0].ToString(), valuesVector[i, 1].ToDouble());
                    dict.Add(valuesVector[i, 0].ToString(), valuesVector[i, 1]);
                }
                return this.SetVariablesValues(dict);
            }
            throw new InvalidMatrixSizeException("Vector must have only one col, or I can use 2 col matrix, when first col is variables names");
        }

        public FunctionElement SetVariablesValuesWithFixedOrder(Matrix valuesVector, Matrix variablesOrder)
        {
            if (valuesVector.Rows != variablesOrder.Rows)
                throw new Exception("Вектора значений и переменных должны иметь одинаковое количество строк");

            if (variablesOrder.Cols != 1 || variablesOrder.Cols != 1)
                throw new Exception("Вектор должен содержать только 1 столбик");

            Matrix newValuesVector = new Matrix(valuesVector.Rows, 2, new FunctionMatrixElement());
            for (int i = 0; i < valuesVector.Rows; i++)
            {
                // Первый столбик будет содержать имена переменных
                newValuesVector[i, 0] = variablesOrder[i, 0];
                // Второй столбик - значения
                newValuesVector[i, 1] = valuesVector[i, 0];
            }

            return this.SetVariablesValues(newValuesVector);
        }

        public Function SetVariablesValues(Dictionary<string, IMatrixElement> dict)
        {
            Function e = this.Clone() as Function;

            foreach (var item in dict)
            {
                e = e.SetVariableValue(item.Key, item.Value);
            }

            if (e.IsDouble())
            {
                return new Function(e.Calculate(e.ToDouble()));
            }
            return e;
        }

        public Function SetVariablesValues(Dictionary<string, FunctionElement> dict)
        {
            Function e = this.Clone() as Function;

            foreach (var item in dict)
            {
                e = e.SetVariableValue(item.Key, item.Value) as Function;
            }

            if (e.IsDouble())
            {
                return new Function(e.Calculate(e.ToDouble()));
            }
            return e;
        }

        private Function SetVariableValue(string name, IMatrixElement value)
        {
            if (value is FunctionMatrixElement)
            {
                return new Function(this.SetVariableValue(name, (value as FunctionMatrixElement).Function));
            }
            if (value is DoubleMatrixElement)
            {
                return new Function(this.SetVariableValue(name, (value as DoubleMatrixElement).D));
            }
            throw new Exception("Unsupported matrix element type");
        }

        /// <summary>
        /// Set value instead of the variable with the specify name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override FunctionElement SetVariableValue(string name, double value)
        {
            return this.SetVariableValue(name, new Function(value));
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement e)
        {
            Function newF = new Function();
            this.variables.ForEach(v => newF += v.SetVariableValue(name, e));
            //newF.Degree = this.Degree;
            //newF.Functions = new List<MathFunctions.IMathFunction>(this.Functions);
            newF.ForceAddFunctions(this);
            foreach (var mf in newF.MathFunctions)
            {
                for (int i = 0; i < mf.Item2.Length; i++)
                {
                    mf.Item2[i] = mf.Item2[i].SetVariableValue(name, e);
                }
            }

            return newF.Recalc();
        }

        public override VariablesMulriplication Derivative(string name)
        {
            VariablesMulriplication newVS = new VariablesMulriplication();
            Function e = new Function();
            
            this.variables.ForEach(vs => e.AddVariablesMul(vs.Derivative(name)));
            newVS.AddVariable(e);

#warning И без производных
            /*if (this.Degree != 1)
            {
                Function predF = this.Clone() as Function;
                predF.Degree--;
                newVS.AddVariable(predF);
                newVS.Constant *= this.Degree;
            }*/

            return newVS;
        }

        public Matrix Gradient()
        {
            string[] vars = this.Variables;

            Matrix m = new Matrix(vars.Length, 1);

            for (int i = 0; i < vars.Length; i++)
            {
                m[i, 0] = new FunctionMatrixElement(new Function(this.Derivative(vars[i])));
            }

            return m;
        }

//        public override string ToString()
//        {
//#warning варнинг
//            /* Функции и так должны всё это пересчитывать, но на всякий случай тут варнинг
//             * if (this.Degree == 0)
//                return "1";
//            if (this.Degree == 1)
//                return this.Name;*/
//            /*if (this.Degree == 0.5)
//            {
//                return String.Format("sqrt({0})", this.Name);
//            }*/
//            if (this.Degree < 0)
//            {
//                if (this.IsNeedBracketsForPower())
//                {
//                    return String.Format("({0})^({1})", this.Name, this.Degree);
//                }
//                return String.Format("{0}^({1})", this.Name, this.Degree);
//            }

//            if (this.IsNeedBracketsForPower())
//            {
//                return String.Format("({0})^{1}", this.Name, this.Degree);
//            }
//            return String.Format("{0}^{1}", this.Name, this.Degree);
//        }

        public override string ToMathMLShort()
        {
            StringBuilder sb = new StringBuilder();

            if (this.IsDouble())
            {
                return String.Format("<math><mrow><mi>{0}</mi></mrow></math>", this.ToDouble());
            }

            bool first = true;
            foreach (var vs in this.variables)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (vs.Constant > 0)
                    {
                        sb.AppendFormat("<mo>+</mo>");
                    }
                }
                sb.AppendFormat("<mrow>{0}</mrow>",vs.ToMathML());
            }
            
            return String.Format("<math><mrow>{0}</mrow></math>", this.ShowMLFunctions(sb.Length == 0 ? "0" : sb.ToString()));
            //return String.Format("<mrow>{0}</mrow>", this.ShowMLFunctions(sb.Length == 0 ? "0" : sb.ToString()));
        }

        public int VarsCount
        {
            get
            {
                return this.Variables.Length;
            }
        }

        public int VariablesMulsWithVariablesCount
        {
            get
            {
                return this.variables.Count(vs => vs.VarsCount != 0);
            }
        }

        public int VariablesMulsWithoutVariablesCount
        {
            get
            {
                return this.variables.Count(vs => vs.Constant != 0 && vs.VarsCount == 0);
            }
        }

        private string[] varNames = null;
        public string[] Variables
        {
            get
            {
                if (!this._isModified && this.varNames != null)
                {
                    return this.varNames;
                }
                
                List<string> vars = new List<string>();
                this.variables.ForEach(delegate(VariablesMulriplication vs)
                {
                    if (vs.Constant != 0)
                    {
                        foreach (string var in vs.Variables)
                        {
                            if (!vars.Contains(var))
                            {
                                vars.Add(var);
                            }
                        }
                    }
                });
                foreach (var mf in this.MathFunctions)
                {
                    foreach (var f in mf.Item2)
                    {
                        if (f is Function)
                        {
                            foreach (var var in (f as Function).Variables)
                            {
                                if (!vars.Contains(var))
                                {
                                    vars.Add(var);
                                }
                            }
                        }
                        else
                        {
                            if (!f.IsConstant() && !vars.Contains(f.Name))
                            {
                                vars.Add(f.Name);
                            }
                        }
                    }
                }

                this._isModified = false;
                this.varNames = vars.ToArray();

                return this.varNames;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Function)
            {
                Function f = (obj as Function).Clone() as Function;

                if (this.notNullVariables.Count != f.notNullVariables.Count || !this.SameMathFunctionsWith(f))
                {
                    return false;
                }

                foreach (var vs in this.notNullVariables)
                {
                    var vs2 = f.variables.Find(v => v.Equals(vs));
                    if (vs2 == null)
                    {
                        return false;
                    }
                    f.variables.Remove(vs2);
                }
                return f.notNullVariables.Count == 0;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Matrix GesseMatrix()
        {
            string[] vars = this.Variables;
            Matrix m = new Matrix(vars.Length, vars.Length);
            for (int i = 0; i < vars.Length; i++)
            {
                for (int j = 0; j < vars.Length; j++)
                {
                    m[i, j] = new FunctionMatrixElement(new Function(this.Derivative(vars[i]).Derivative(vars[j])));
                }
            }
            return m;
        }

        private string name = null;
        public override string Name
        {
            get
            {
                if (!this._isModified && !String.IsNullOrEmpty(this.name))
                {
                    return this.name;
                }

                if (this.variables.Count == 0)
                    return "0";
                
                StringBuilder sb = new StringBuilder();

                bool first = true;

                foreach (var vars in this.variables)
                {
                    double constant = vars.Constant;
                    if (constant != 0 || this.variables.Count == 1)
                    {
                        if (!first && constant > 0)
                            sb.Append(" + ");
                        if (constant != 0)
                            sb.Append(vars.ToString());
                        first = false;
                    }
                }

                this._isModified = false;
                this.name = sb.ToString();
                return this.name;
            }
        }

        public override bool IsDouble()
        {
            if (this.MathFunctions.Count != 0)
            {
                return false;
            }
            foreach (var vs in this.variables)
            {
                if (!vs.IsDouble())
                {
                    return false;
                }
            }

            return true;
        }

        public override VariablesMulriplication Derivative()
        {
            if (Variables.Length == 0)
            {
                return new VariablesMulriplication(0);
            }
            var variable = this.Variables[0];
            return this.Derivative(variable);
        }

        public override bool HasVariable(string name)
        {
            return this.Variables.Contains(name);
        }

        public void CopyTo(Function func)
        {
            func.variables = new List<VariablesMulriplication>();
            this.variables.ForEach(vs => func.variables.Add(vs));
            func.CopyFunctions(this);
        }

        public Function OpenAllBrackets()
        {
            Function f = new Function();

#warning Сделать что-то такое

            return f;
        }

        public bool IsNeedBrackets()
        {
            //if (Math.Abs(this.Degree) != 1)
            //    return false;
            return this.IsNeedBracketsForPower();
        }

        public bool IsNeedBracketsForPower()
        {
            int count = 0;
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].Constant != 0)
                    count++;

                if (count > 1)
                    return true;

                if (this.variables[i].Count > 1)
                    return true;
            }

            if (this. IsVariableMultiplication())
            {
                var temp = this.ToVariableMultiplication();
                if (temp.IsFunction())
                {
                    return temp.ToFunction().IsNeedBracketsForPower();
                }
            }
            return false;
        }

        public override bool IsVariableMultiplication()
        {
            if (this.MathFunctions.Count != 0)
                return false;
            int count = 0;
            foreach (var vs in this.variables)
            {
                if (vs.Constant != 0 && vs.ToString() != "0")
                {
                    count++;
                }
                if (count > 1)
                    return false;
            }
            return true;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            if (this.IsVariableMultiplication())
            {
                var v = this.variables.Find(vs => vs.Constant != 0 && vs.ToString() != "0");
                if (v != null)
                {
                    return v;
                }
                else
                {
                    return new VariablesMulriplication(0);
                }
            }
            return new VariablesMulriplication(this);
        }

        public override bool IsLeaf()
        {
            if (this.variables.Count(vs => vs.Constant != 0) == 1)
            {
                return this.variables.Find(vs => vs.Constant != 0).IsLeaf();
            }
            return false;
        }

        public override FunctionElement ToLeaf()
        {
            if (this.IsLeaf())
            {
                return this.variables[0].ToLeaf();
            }
            return null;
        }

        public override bool IsConstant()
        {
            foreach (var vs in this.variables)
            {
                if (!vs.IsConstant())
                {
                    return false;
                }
            }
            return true;
        }

        public override void ParseFromString(string func)
        {
            this.ReplaceThisWithElement(Function.FromString(func));
        }
    }
}
