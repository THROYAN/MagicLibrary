using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MagicLibrary.Exceptions;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    /// <summary>
    /// Класс для создания различных цветов.
    /// </summary>
    [Serializable]
    public class ColorSet
    {
        public static Random r = new Random();

        public const string RegexMaskOfIndexes = "^{0}\\[(?<index>-?\\d+)\\]";

        private ColorSetType type;
        /// <summary>
        /// Тип цвета
        /// </summary>
        public ColorSetType Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
                this.ParseAttributes();
            }
        }

        public string Name { get; set; }

        private string attrs;
        /// <summary>
        /// Строка аттрибутов, заданных пользователем
        /// </summary>
        public string Attributes
        {
            get { return this.attrs; }
            set
            {
                this.attrs = value;
                this.ParseAttributes();
            }
        }

        public ColorSetCollection Collection { get; private set; }

#warning Запретить изменение распарсенных аттрибутов!
        /// <summary>
        /// Аттрибуты, выделенные из строки аттрибутов
        /// </summary>
        public Dictionary<string, string> ParsedAttributes { get; private set; }

        #region Consts
        public const string EmptyAttribute = "";

        public const string MaskOfIndexes = "{0}[{1}]";
        
        public const string NameAttribute = "Name";
        public const string MinValueAttribute = "MinValue";
        public const string MaxValueAttribute = "MaxValue";
        public const string FromStringAttribute = "FromString";
        public const string ToStringAttribute = "ToString";
        public const string MinLengthAttribute = "MinLength";
        public const string MaxLengthAttribute = "MaxLength";
        public const string TrueStringAttribute = "TrueString";
        public const string FalseStringAttribute = "FalseString";
        public const string FromAttribute = "From";
        public const string ToAttribute = "To";
        public const string ElementsColor = "OfColor";

        public const string UnitDefault = "'()'";
        public const string TrueDefault = "true";
        public const string FalseDefault = "false";
        #endregion

        public static Dictionary<string, ColorSetType> Types = new Dictionary<string, ColorSetType>()
            {
                { "unit", ColorSetType.Unit },
                { "int", ColorSetType.Int },
                { "bool", ColorSetType.Bool },
                { "string", ColorSetType.String },
                { "index", ColorSetType.Index },
                { "record", ColorSetType.Record },
                { "enum", ColorSetType.Enum },
                { "list", ColorSetType.List },
            };

        public ColorSet(string name, ColorSetType type, ColorSetCollection colors, string attrs = "")
        {
            this.Name = name;
            this.type = type;
            this.attrs = attrs;
            this.Collection = colors;
            
            this.ParseAttributes();
            //this.Collection.AddColorSet(this);
        }

        public ColorSet(string colorSetDescription, ColorSetCollection colors)
        {
            string[] masks = new string[]
            {
                //@"^(?<name>\w.*)\s*=\s*(?<type>\w+)$",
                @"^\s*(?<name>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)\s*=\s*(?<type>\w+)\((?<attrs>(.|\n)*)\)\s*$"
            };

            Match m = null;

            foreach (var mask in masks)
            {
                m = Regex.Match(colorSetDescription, mask);
                if (m.Success)
                {
                    break;
                }
            }

            if (m == null || !m.Success || !ColorSet.Types.ContainsKey(m.Groups["type"].Value))
            {
                throw new InvalidColorSetAttributesException("NewColorSet()", colorSetDescription);
            }
            this.Name = m.Groups["name"].Value.Trim();
            this.type = ColorSet.Types[m.Groups["type"].Value];
            this.attrs = m.Groups["attrs"].Value;
            this.Collection = colors;
            
            this.ParseAttributes();
            //this.Collection.AddColorSet(this);
        }

        public bool IsInt(string value)
        {
            int i;
            return Int32.TryParse(value, out i);
        }

        public bool IsArrayAttributes()
        {
            return this.ParsedAttributes.ContainsKey(Function.KeyOfIndex(1));
        }

        /// <summary>
        /// Проверка на присутствие у цвета заданных аттрибутов.
        /// </summary>
        /// <param name="attrsNames">Перечисление аттрибутов</param>
        /// <returns></returns>
        public bool HasAttributes(params string[] attrsNames)
        {
            foreach (var attr in attrsNames)
            {
                if (!this.ParsedAttributes.ContainsKey(attr))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверка на присутствие у цвета определённое количества аттрибутов, 
        /// и в таком случае задание остальным пустого значения.
        /// </summary>
        /// <param name="count">Нужное количество совпадений</param>
        /// <param name="attrsNames">Перечисление аттрибутов</param>
        /// <returns>Имеется ли заданное количество аттрибутов из перечисленных</returns>
        public bool FillNoneAttributes(int count, params string[] attrsNames)
        {
            int i = 0;
            foreach (var attr in attrsNames)
            {
                if (this.HasAttributes(attr))
                {
                    i++;
                }
            }
            if (count > i)
            {
                return false;
            }

            foreach (var attr in attrsNames)
            {
                if (!this.HasAttributes(attr))
                {
                    this.ParsedAttributes[attr] = ColorSet.EmptyAttribute;
                }
            }

            return true;
        }

        /// <summary>
        /// Обновление аттрибутов для текущего цвета.
        /// </summary>
        public void ParseAttributes()
        {
            try
            {
                this.ParsedAttributes = Function.ParseAttributes(this.Attributes);
                this.ConvertAttributes();
            }
            catch (InvalidAttributesException)
            {
                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
            }
        }

        /// <summary>
        /// Преобразует аттрибуты соотвественно единому стандарту (текущему цвету).
        /// </summary>
        public void ConvertAttributes()
        {
            switch (this.Type)
            {
                case ColorSetType.Unit:

                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 0:
                            this.ParsedAttributes[ColorSet.NameAttribute] = ColorSet.EmptyAttribute;
                            break;
                        case 1:
                            // Если аттрибут без имени, то это и есть новое значение
                            if (this.IsArrayAttributes())
                            {
                                this.ParsedAttributes[ColorSet.NameAttribute] = this.ParsedAttributes[Function.KeyOfIndex(1)];
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                            }
                            else if (!this.HasAttributes(ColorSet.NameAttribute))
                            {
                                // Если есть имя, то оно должно совпадать с ColorSet.NameAttribute
                                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                            }
                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }

                    break;
                case ColorSetType.Int:

                    #region Тяжелые штуки для выделения баундов инта
                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 0:

                            // default bounds
                            this.ParsedAttributes[ColorSet.MinValueAttribute] = ColorSet.EmptyAttribute;
                            this.ParsedAttributes[ColorSet.MaxValueAttribute] = ColorSet.EmptyAttribute;

                            break;
                        case 1:

                            // Если 1 аттрибут, то это должен быть промежуток в виде <число>..<число>
                            if (this.IsArrayAttributes())
                            {
                                // Выделяем промежуток
                                var m = Regex.Match(this.ParsedAttributes[Function.KeyOfIndex(1)], @"^'(?<min>-?\d+)\.\.(?<max>-?\d+)'$");

                                if (m.Success)
                                {
                                    // Если задан не верный промежуток
                                    if (Convert.ToInt32(m.Groups["min"].Value) > Convert.ToInt32(m.Groups["max"].Value))
                                    {
                                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                    }
                                    this.ParsedAttributes[ColorSet.MinValueAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.MaxValueAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            else
                            {
                                // Проверка на один из дозволенных аттрибутов
                                if (!this.FillNoneAttributes(1, ColorSet.MinValueAttribute, ColorSet.MaxValueAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            break;
                        case 2:

                            // Если заданы 2 параметра без имён, то это минимальное и максимальное значения
                            if (this.IsArrayAttributes())
                            {
                                // Опять же, если не верно задан промежуток
                                if (!this.IsInt(this.ParsedAttributes[Function.KeyOfIndex(1)]) ||
                                    !this.IsInt(this.ParsedAttributes[Function.KeyOfIndex(2)]) ||
                                    Convert.ToInt32(this.ParsedAttributes[Function.KeyOfIndex(1)]) > Convert.ToInt32(this.ParsedAttributes[Function.KeyOfIndex(2)]))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }

                                this.ParsedAttributes[ColorSet.MinValueAttribute] = this.ParsedAttributes[Function.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.MaxValueAttribute] = this.ParsedAttributes[Function.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(2));
                            }
                            else
                            {
                                // Если у аттрибутов есть имена, то они должны быть правильными
                                if (!(this.HasAttributes(ColorSet.MinValueAttribute) && this.HasAttributes(ColorSet.MaxValueAttribute)))
                                //if(!this.FillNoneAttributes(2, ColorSet.MaxValueAttribute, ColorSet.MinValueAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }

                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }
                    #endregion

                    break;
                case ColorSetType.String:

                    #region Магическое разбираение не менее магических аттрибутов строк
                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 0:
                            this.ParsedAttributes[ColorSet.FromStringAttribute] = ColorSet.EmptyAttribute;
                            this.ParsedAttributes[ColorSet.ToStringAttribute] = ColorSet.EmptyAttribute;
                            this.ParsedAttributes[ColorSet.MinLengthAttribute] = ColorSet.EmptyAttribute;
                            this.ParsedAttributes[ColorSet.MaxLengthAttribute] = ColorSet.EmptyAttribute;
                            break;
                        case 1:

                            // 1 параметр - это либо строковые баунды строки, либо именованный аттрибут
                            if (this.IsArrayAttributes())
                            {
                                var m = Regex.Match(this.ParsedAttributes[Function.KeyOfIndex(1)], @"^'(?<min>.+)\.\.(?<max>.+)'$");
                                if (m.Success)
                                {
                                    this.ParsedAttributes[ColorSet.FromStringAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToStringAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes[ColorSet.MinLengthAttribute] = ColorSet.EmptyAttribute;
                                    this.ParsedAttributes[ColorSet.MaxLengthAttribute] = ColorSet.EmptyAttribute;
                                    this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            else
                            {
                                if (!this.FillNoneAttributes(1, ColorSet.FromStringAttribute, ColorSet.ToStringAttribute, ColorSet.MinLengthAttribute, ColorSet.MaxLengthAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }

                            break;
                        case 2:
                            // 2 параметра - баунды по строкам и длине либо именованные аттрибуты
                            if (this.IsArrayAttributes())
                            {
                                var m = Regex.Match(this.ParsedAttributes[Function.KeyOfIndex(1)], @"^'(?<min>.+)\.\.(?<max>.+)'$");
                                if (m.Success)
                                {
                                    this.ParsedAttributes[ColorSet.FromStringAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToStringAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }

                                m = Regex.Match(this.ParsedAttributes[Function.KeyOfIndex(2)], @"^'(?<min>\d+)\.\.(?<max>\d+)'$");
                                if (m.Success)
                                {
                                    if (Int32.Parse(m.Groups["min"].Value) > Int32.Parse(m.Groups["max"].Value))
                                    {
                                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                    }

                                    this.ParsedAttributes[ColorSet.MinLengthAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.MaxLengthAttribute] = m.Groups["max"].Value;

                                    this.ParsedAttributes.Remove(Function.KeyOfIndex(2));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            else
                            {
                                if (!this.FillNoneAttributes(2, ColorSet.FromStringAttribute, ColorSet.ToStringAttribute, ColorSet.MinLengthAttribute, ColorSet.MaxLengthAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            break;
                        case 3:
                        case 4:
                            // Должны быть заданы только именованные аттрибуты
                            if (!this.FillNoneAttributes(this.ParsedAttributes.Keys.Count, ColorSet.FromStringAttribute, ColorSet.ToStringAttribute, ColorSet.MinLengthAttribute, ColorSet.MaxLengthAttribute))
                            {
                                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                            }
                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }
                    #endregion

                    break;
                case ColorSetType.Bool:

                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 0:
                        case 1:
                            // Проверка на заданный 1 параметр и установка остальных значений в пустые
                            if (!this.FillNoneAttributes(this.ParsedAttributes.Keys.Count, ColorSet.TrueStringAttribute, ColorSet.FalseStringAttribute))
                            {
                                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                            }
                            break;
                        case 2:
                            if (this.IsArrayAttributes())
                            {
                                this.ParsedAttributes[ColorSet.TrueStringAttribute] = this.ParsedAttributes[Function.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.FalseStringAttribute] = this.ParsedAttributes[Function.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(2));
                            }
                            else
                            {
                                if (!this.HasAttributes(ColorSet.TrueStringAttribute, ColorSet.FalseStringAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }

                    break;
                case ColorSetType.Enum:

                    // Энум всегда просто Энум
                    if (!this.IsArrayAttributes())
                    {
                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }
                    for (int i = 0; i < this.ParsedAttributes.Count; i++)
                    {
                        var item = this.ParsedAttributes.ElementAt(i);
                        if (!Regex.IsMatch(item.Value, @"^\'.*\'$"))
                        {
                            this.ParsedAttributes[item.Key] = String.Format("'{0}'", item.Value);
                        }
                    }

                    break;
                case ColorSetType.Index:

                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 1: // Только промежуток (1..5)
                            if (this.IsArrayAttributes())
                            {
                                // Выделяем промежуток
                                var m = Regex.Match(this.ParsedAttributes[Function.KeyOfIndex(1)], @"^'(?<min>-?\d+)\.\.(?<max>-?\d+)'$");

                                if (m.Success)
                                {
                                    // Если задан не верный промежуток
                                    if (Convert.ToInt32(m.Groups["min"].Value) > Convert.ToInt32(m.Groups["max"].Value))
                                    {
                                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                    }
                                    this.ParsedAttributes[ColorSet.FromAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }
                            else
                            {
                                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                            }
                            break;
                        case 2:
                            if (this.IsArrayAttributes())
                            {
                                if (Convert.ToInt32(this.ParsedAttributes[Function.KeyOfIndex(1)]) > Convert.ToInt32(this.ParsedAttributes[Function.KeyOfIndex(2)]))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                                this.ParsedAttributes[ColorSet.FromAttribute] = this.ParsedAttributes[Function.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.ToAttribute] = this.ParsedAttributes[Function.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(Function.KeyOfIndex(2));
                            }
                            else
                            {
                                if (!this.HasAttributes(ColorSet.FromAttribute, ColorSet.ToAttribute))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                            }

                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }

                    break;
                case ColorSetType.Record:

                    if (this.ParsedAttributes.Keys.Count < 2)
                    {
                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }
                    foreach (var attr in this.ParsedAttributes)
                    {
                        if (!this.Collection.ContainsColorSet(attr.Value))
                        {
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                        }
                    }

                    break;
                case ColorSetType.List:

                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 1:
                            if (this.IsArrayAttributes())
                            {
                                this.ParsedAttributes[ColorSet.ElementsColor] = this.ParsedAttributes[Function.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.MinLengthAttribute] = ColorSet.EmptyAttribute;
                                this.ParsedAttributes[ColorSet.MaxLengthAttribute] = ColorSet.EmptyAttribute;

                                this.ParsedAttributes.Remove(Function.KeyOfIndex(1));
                            }
                            else
                            {
                                if (!this.ParsedAttributes.ContainsKey(ColorSet.ElementsColor))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                                this.FillNoneAttributes(1, ColorSet.ElementsColor, ColorSet.MinLengthAttribute, ColorSet.MaxLengthAttribute);
                            }
                            break;
                        case 2: case 3:

                            if (!this.ParsedAttributes.ContainsKey(ColorSet.ElementsColor))
                            {
                                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                            }
                            this.FillNoneAttributes(this.ParsedAttributes.Keys.Count, ColorSet.ElementsColor, ColorSet.MinLengthAttribute, ColorSet.MaxLengthAttribute);

                            break;
                        default:
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }
                    if (this.Collection.GetColorSet(this.ParsedAttributes[ColorSet.ElementsColor]) == null)
                    {
                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                    }

                    break;
            }
        }

        /// <summary>
        /// Валидация значения фишки по отношению к правилам текущего цвета.
        /// </summary>
        /// <param name="tokenF">функция - представление 1 фишки</param>
        /// <returns></returns>
        public bool IsLegal(Function tokenF)
        {
            var v = tokenF.ToLeaf();
            if (v is Variable && (v as Variable).Name != "")
            {
                if (!this.Collection.HasVariable(v.Name))
                {
                    return false;
                }

                var var = this.Collection.GetVariable(v.Name);
                if (var.ColorSet.Type == ColorSetType.Record)
                {
                    var curC = var.ColorSet;
                    for (int i = 0; i < tokenF.MathFunctions.Count; i++)
                    {
                        if (!(tokenF.MathFunctions[i].Item1 == RecordVariable.RecordField))
                        {
                            break;
                        }
                        var s = tokenF.MathFunctions[i].Item2[0].ToLeaf() as StringVariable;
                        if (!curC.ParsedAttributes.ContainsKey(s.Value))
                        {
                            return false;
                        }
                        curC = this.Collection.GetColorSet(curC.ParsedAttributes[s.Value]);
                    }
                    return curC == this;
                }
                if (var.ColorSet.Type == ColorSetType.List &&
                    v.MathFunctions.Count > 0 &&
                    v.MathFunctions[0].Item1 == Function.GetMathFunction("index"))
                {
                    if (var.ColorSet == this && this.Type == ColorSetType.Index)
                    {
                        return true;
                    }

                    return this.Collection.GetColorSet(var.ColorSet.ParsedAttributes[ColorSet.ElementsColor]) == this;
                }

                return this.Collection.GetVariable(v.Name).ColorSet == this;
            }

            switch (this.Type)
            {
                case ColorSetType.Unit:

                    var s = v as StringVariable;
                    if (s == null)
                    {
                        return false;
                    }
                    
                    return s.ToString().Equals(ColorSet.UnitDefault) || s.ToString().Equals(this.ParsedAttributes[ColorSet.NameAttribute]);

                case ColorSetType.Int:

                    if (!tokenF.IsDouble())
                    {
                        var vars = tokenF.Variables;

                        foreach (var var in vars)
                        {
                            if (!this.Collection.HasVariable(var) || !(this.Collection.GetVariable(var).ColorSet == this))
                            {
                                return false;
                            }
                        }
                        try
                        {
                            // Можно ли будет посчитать?
                            tokenF.ToDouble();
                        }
                        catch (NotImplementedException)
                        {
                            return false;
                        }
                        catch
                        {
                            return true;
                        }
                    }

                    int val = (int)tokenF.ToDouble();
                    string min = this.ParsedAttributes[ColorSet.MinValueAttribute];
                    string max = this.ParsedAttributes[ColorSet.MaxValueAttribute];

                    return (min.Equals(ColorSet.EmptyAttribute) || Convert.ToInt32(min) <= val) &&
                            (max.Equals(ColorSet.EmptyAttribute) || Convert.ToInt32(max) >= val);

                case ColorSetType.Bool:

                    // оно сюда никогда не должно попасть, потому что true и false должны быть как переменные в ColorSetCollection и их значения для текущего цвета!
                    // разве что если будет выражение с логическими операциями
                    return tokenF.ToString().Equals(ColorSet.TrueDefault) || tokenF.ToString().Equals(ColorSet.FalseDefault) ||
                        tokenF.ToString().Equals(this.ParsedAttributes[ColorSet.TrueStringAttribute]) || tokenF.ToString().Equals(this.ParsedAttributes[ColorSet.FalseStringAttribute]);

                case ColorSetType.String:

                    s = tokenF.ToLeaf() as StringVariable;

                    if (s == null)
                    {
                        return false;
                    }

                    bool f = true;

                    if (this.ParsedAttributes[ColorSet.MinLengthAttribute] != ColorSet.EmptyAttribute)
                    {
                        f = f && s.Value.Length >= Convert.ToInt32(this.ParsedAttributes[ColorSet.MinLengthAttribute]);
                    }
                    if (this.ParsedAttributes[ColorSet.MaxLengthAttribute] != ColorSet.EmptyAttribute)
                    {
                        f = f && s.Value.Length <= Convert.ToInt32(this.ParsedAttributes[ColorSet.MaxLengthAttribute]);
                    }
                    if (this.ParsedAttributes[ColorSet.FromStringAttribute] != ColorSet.EmptyAttribute)
                    {
                        bool f2 = true;
                        foreach (var c in s.Value)
                        {
                            if (this.ParsedAttributes[ColorSet.FromStringAttribute][0] > c ||
                                this.ParsedAttributes[ColorSet.ToStringAttribute][0] < c)
                            {
                                f2 = false;
                                break;
                            }
                        }

                        f = f && f2;
                    }
                    if (this.ParsedAttributes[ColorSet.ToStringAttribute] != ColorSet.EmptyAttribute)
                    {
                        f = f && s.Value.CompareTo(this.ParsedAttributes[ColorSet.ToStringAttribute]) <= 0;
                    }

                    return f;
                case ColorSetType.Enum:
                    s = v as StringVariable;

                    if (s == null)
                    {
                        return false;
                    }

                    // Сюда тоже не должен дойти
                    foreach (var item in this.ParsedAttributes)
                    {
                        if (item.Value.Equals(s.ToString()))
                            return true;
                    }

                    return false;

                case ColorSetType.Index:

                    s = tokenF.ToLeaf() as StringVariable;

                    if (s == null)
                    {
                        return false;
                    }

                    var m = Regex.Match(s.Value, String.Format(ColorSet.RegexMaskOfIndexes, this.Name));

                    if (m.Success)
                    {
                        return Convert.ToInt32(m.Groups["index"].Value) >= Convert.ToInt32(this.ParsedAttributes[ColorSet.FromAttribute]) &&
                            Convert.ToInt32(m.Groups["index"].Value) <= Convert.ToInt32(this.ParsedAttributes[ColorSet.ToAttribute]);
                    }

                    return false;

                case ColorSetType.Record:

                    var r = tokenF.ToLeaf() as RecordVariable;

                    if (r == null)
                    {
                        return false;
                    }

                    if (r.Values.ContainsKey(Function.KeyOfIndex(1)))
                    {
                        if (r.Values.Count != this.ParsedAttributes.Count)
                        {
                            return false;
                        }

                        for (int i = 0; i < this.ParsedAttributes.Count; i++)
                        {
                            if (!this.Collection[this.ParsedAttributes.ElementAt(i).Value].IsLegal(r.Values[Function.KeyOfIndex(i + 1)] as Function))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                    foreach (var item in this.ParsedAttributes)
                    {
                        if (!r.Values.ContainsKey(item.Key))
                        {
                            return false;
                        }

                        if (!this.Collection[item.Value].IsLegal(r.Values[item.Key] as Function))
                        {
                            return false;
                        }
                    }

                    return true;
                case ColorSetType.List:

                    var l = tokenF.ToLeaf() as ListVariable;

                    if (l == null)
                    {
                        return false;
                    }

                    foreach (var value in l.Values)
                    {
#warning Осторожно, тут такое четкое приведение типов, шо я не могу
                        if (!this.Collection.GetColorSet(this.ParsedAttributes[ColorSet.ElementsColor]).IsLegal(value as Function))
                        {
                            return false;
                        }
                    }

                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ShowToken(Token token)
        {
            if (!this.IsLegal(token.Function))
            {
                throw new InvalidTokenException(token.Value, this.Name);
            }

            var attrs = Function.ParseAttributes(token.Value);
            switch (this.Type)
            {
                case ColorSetType.Unit:

                    return this.ParsedAttributes[ColorSet.NameAttribute];

                case ColorSetType.Int:

                    int val;
                    // не обязательная проверка, но всё же
                    if (!Int32.TryParse(token.Value, out val))
                    {
                        throw new InvalidTokenException(token.Value, this.Name);
                    }
                    return val.ToString();

                case ColorSetType.Bool:

                    if (token.Value.Equals(this.ParsedAttributes[ColorSet.TrueStringAttribute]) ||
                        token.Value.Equals(ColorSet.TrueDefault))
                    {
                        return this.ParsedAttributes[ColorSet.TrueStringAttribute];
                    }
                    else
                    {
                        // Если isLegal проходит, значит можно точно вернуть это значение
                        return this.ParsedAttributes[ColorSet.FalseStringAttribute];
                    }

                case ColorSetType.Enum:
                case ColorSetType.Index:

                    return token.Value;

                case ColorSetType.String:

                    return String.Format("'{0}'", token.Value);

                case ColorSetType.Record:

                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    var r = token.Function.ToLeaf() as RecordVariable;
                    
                    foreach (var item in r.Values)
                    {
                        i++;
                        ColorSet color = this.Collection[this.ParsedAttributes[item.Key]];

                        Token t = new Token(new Function(item.Value), color);
                        sb.AppendFormat("[{0}] => {1}, ", item.Key, t.ToString());
                    }
                    sb.Remove(sb.Length - 2, 2);

                    return String.Format("({0})", sb.ToString());

                case ColorSetType.List:

                    return token.Value;

                default:

                    throw new InvalidTokenException(token.Value, this.Name);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}({2})", this.Name, this.GetTypeName(), this.Attributes);
        }

        /// <summary>
        /// Строковое представление типа цвета
        /// </summary>
        /// <returns></returns>
        public string GetTypeName()
        {
            foreach (var item in ColorSet.Types)
            {
                if (item.Value.Equals(this.Type))
                {
                    return item.Key;
                }
            }
            return "<undefined type>";
        }

        public List<Token> GetTokensFromInitFunction(string initFunc)
        {
            List<Token> tokens = new List<Token>();

            var sepTokens = initFunc.Split(new[] { "++" }, StringSplitOptions.None);
            foreach (var token in sepTokens)
            {
                tokens.Add(new Token(token, this));
            }

            return tokens;
        }

        public Function GetRandomValue()
        {
            switch (this.Type)
            {
                case ColorSetType.Int:

                    if (this.ParsedAttributes[ColorSet.MinValueAttribute] == ColorSet.EmptyAttribute ||
                        this.ParsedAttributes[ColorSet.MaxValueAttribute] == ColorSet.EmptyAttribute)
                    {
                        throw new Exception();
                    }
                    return new Function(r.Next(Int32.Parse(this.ParsedAttributes[ColorSet.MinValueAttribute]),
                                            Int32.Parse(this.ParsedAttributes[ColorSet.MaxValueAttribute])));
                    
                case ColorSetType.String:

                    throw new Exception();

                case ColorSetType.Enum:

                    return new Function(this.ParsedAttributes[Function.KeyOfIndex(r.Next(1, this.ParsedAttributes.Count))]);

                case ColorSetType.Bool:

                    return new Function(r.Next(0, 1) == 0 ? ColorSet.TrueDefault : ColorSet.FalseDefault);

                case ColorSetType.Unit:

                    return new Function(ColorSet.UnitDefault);

                case ColorSetType.Index:

                    return new Function(String.Format("{0}[{1}]", this.Name, r.Next(Int32.Parse(ColorSet.FromAttribute), Int32.Parse(ColorSet.ToAttribute))));

                case ColorSetType.Record:

                    StringBuilder sb = new StringBuilder();
                    foreach (var attr in this.ParsedAttributes)
                    {
                        sb.AppendFormat("{0}, ", this.Collection.GetColorSet(attr.Value).GetRandomValue());
                    }
                    sb = sb.Remove(sb.Length - 2, 2);
                    return new Function(String.Format("{1} {0} {2}", sb.ToString(), "{", "}"));

                case ColorSetType.List:

                    if (this.ParsedAttributes[ColorSet.MinLengthAttribute] == ColorSet.EmptyAttribute ||
                        this.ParsedAttributes[ColorSet.MaxLengthAttribute] == ColorSet.EmptyAttribute)
                    {
                        throw new Exception();
                    }
                    int count = r.Next(
                        Int32.Parse(this.ParsedAttributes[ColorSet.MinLengthAttribute]),
                        Int32.Parse(this.ParsedAttributes[ColorSet.MaxLengthAttribute]));

                    FunctionElement[] elements = new FunctionElement[count];

                    for (int i = 0; i < count; i++)
                    {
                        elements[i] = this.Collection.GetColorSet(this.ParsedAttributes[ColorSet.ElementsColor]).GetRandomValue();
                    }
                    return new Function(new ListVariable(elements));

                default:
                    throw new Exception();
            }
        }
    }
}
