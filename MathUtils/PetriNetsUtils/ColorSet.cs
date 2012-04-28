using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    /// <summary>
    /// Класс для создания различных цветов.
    /// </summary>
    public class ColorSet
    {
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

        public const string MaskOfArrayAttributes = "[{0}]";
        public const string MaskOfIndexes = "{0}({1})";
        public const string RegexMaskOfIndexes = "^{0}\\((?<index>-?\\d+)\\)";

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

        public const string UnitDefault = "()";
        public const string TrueDefault = "true";
        public const string FalseDefault = "false";
        #endregion

        private static Dictionary<string, ColorSetType> typeNames = new Dictionary<string, ColorSetType>()
            {
                { "unit", ColorSetType.Unit },
                { "int", ColorSetType.Int },
                { "bool", ColorSetType.Bool },
                { "string", ColorSetType.String },
                { "index", ColorSetType.Index },
                { "record", ColorSetType.Record },
                { "enum", ColorSetType.Enum },
            };

        public ColorSet(string name, ColorSetType type, ColorSetCollection colors, string attrs = "")
        {
            this.Name = name;
            this.type = type;
            this.attrs = attrs;
            this.Collection = colors;
            
            this.ParseAttributes();
            this.Collection.AddColorSet(this);
        }

        public ColorSet(string colorSetDescription, ColorSetCollection colors)
        {
            string[] masks = new string[]
            {
                //@"^(?<name>\w.*)\s*=\s*(?<type>\w+)$",
                @"^\s*(?<name>\w.*)\s*=\s*(?<type>\w+)\((?<attrs>.*)\)\s*$"
            };

            Match m = null;

            foreach (var mask in masks)
            {
                m = Regex.Match(colorSetDescription, mask);
            }
            if (m == null || !m.Success || !ColorSet.typeNames.ContainsKey(m.Groups["type"].Value))
            {
                throw new InvalidColorSetAttributesException("NewColorSet()", colorSetDescription);
            }
            this.Name = m.Groups["name"].Value.Trim();
            this.type = ColorSet.typeNames[m.Groups["type"].Value];
            this.attrs = m.Groups["attrs"].Value;
            this.Collection = colors;
            
            this.ParseAttributes();
            this.Collection.AddColorSet(this);
        }

        /// <summary>
        /// Имя индексного аттрибута (массива)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string KeyOfIndex(int index)
        {
            return String.Format(ColorSet.MaskOfArrayAttributes, index);
        }

        public bool IsInt(string value)
        {
            int i;
            return Int32.TryParse(value, out i);
        }

        public bool IsArrayAttributes()
        {
            return this.ParsedAttributes.ContainsKey(this.KeyOfIndex(1));
        }

        public bool IsArrayAttributes(string attrs)
        {
            return this.ParseAttributes(attrs).ContainsKey(this.KeyOfIndex(1));
        }

        public bool IsArrayAttributes(Dictionary<string, string> attrs)
        {
            return attrs.ContainsKey(this.KeyOfIndex(1));
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
        /// Парсинг аттрибута из строки, содержащей 1 аттрибут.
        /// 1) Имя=Значение
        /// 2) Значение
        /// 3) Имя = 'Значение'
        /// 4) Имя: "Значение"
        /// ...
        /// </summary>
        /// <param name="attr">Аттрибут, в виде строки</param>
        /// <returns>Пара - имя и значение аттрибута</returns>
        KeyValuePair<string, string> parseAttribute(string attr)
        {
            if (Regex.IsMatch(attr, @"^\s?$"))
            {
                return new KeyValuePair<string, string>("", "");
            }
            string[] patterns = new string[] {
                "^\\s*(?<name>\\w.*)\\s*[=:]\\s*\"(?<value>.+)\"\\s*$", // Name ="+asd"
                @"^\s*(?<name>\w.*)\s*[=:]\s*'(?<value>.+)'\s*$", // name = '2asd'
                @"^\s*(?<name>\w.*)\s*[=:]\s*(?<value>\w.*)\s*$", // Name = asd2
                @"^\s*(?<name>\w.*)\s*[=:]\s*(?<value>-?\d+)\s*$", // MinLength = -12
                "^\\s*\"(?<value>.+)\"\\s*$", // "asd2"
                @"^\s*'(?<value>.+)'\s*$", // 'asd2'
                @"^\s*(?<value>-?\d+)\s*$", // 15
                @"^\s*(?<value>.+)\s*$", // asd2
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

        /// <summary>
        /// Разбиение строки аттрибутов, на строки, содержащие по одному аттрибуту
        /// </summary>
        /// <param name="attrs">Строка аттрибутов</param>
        /// <returns>Массив строк аттрибутов</returns>
        private string[] parseAttributes(string attrs)
        {
            var ss = Regex.Split(attrs, @"\\,");

            if (ss.Length == 1)
            {
                return attrs.Split(',');
            }

            List<string> result = new List<string>();

            for (int i = 0; i < ss.Length; i++)
            {
                var ss2 = this.parseAttributes(ss[i]);

                if (i != 0)
                {
                    result[result.Count - 1] = result.Last() + "," + ss2[0];
                    for (int j = 1; j < ss2.Length; j++)
                    {
                        result.Add(ss2[j]);
                    }
                }
                else
                {
                    result.AddRange(ss2);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Обновление аттрибутов для текущего цвета.
        /// </summary>
        public void ParseAttributes()
        {
            try
            {
                this.ParsedAttributes = ParseAttributes(this.Attributes);
            }
            catch (InvalidAttributesException ae)
            {
                throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
            }

            this.ConvertAttributes();
        }

        /// <summary>
        /// Парсинг аттрибутов из строки аттрибутов.
        /// Аттрибуты записываются через ','.
        /// </summary>
        /// <param name="attrs">Аттрибуты в виде строки</param>
        /// <returns>Словарь, где ключи - это имена аттрибутов, а значения - это значения аттрибутов.</returns>
        public Dictionary<string, string> ParseAttributes(string attrs)
        {   
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (Regex.IsMatch(attrs, @"^\s?$"))
                return d;
            
            var pAttrs = parseAttributes(attrs);

            int i = 0, j = 0;
            foreach (var attr in pAttrs)
            {
                var pAttr = parseAttribute(attr);

                // array attrs
                if (pAttr.Key == "")
                {
                    // Если где-то в середине был именованный аттрибут, то это плохо
                    if (i != j)
                    {
                        throw new InvalidAttributesException(attrs, this.Name);
                    }
                    d.Add(this.KeyOfIndex(++i), pAttr.Value);
                }
                else
                {
                    // Если был массив, а тут появился именованный аттрибут
                    if (i != 0)
                    {
                        throw new InvalidAttributesException(attrs, this.Name);
                    }

                    // Повторяющиеся аттрибуты - это лажа
                    if(d.ContainsKey(pAttr.Key))
                    {
                        throw new InvalidAttributesException(attrs, this.Name);
                    }

                    d.Add(pAttr.Key, pAttr.Value);
                }
                j++;
            }

            return d;
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
                                this.ParsedAttributes[ColorSet.NameAttribute] = this.ParsedAttributes[this.KeyOfIndex(1)];
                                this.ParsedAttributes.Remove(this.KeyOfIndex(1));
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
                                var m = Regex.Match(this.ParsedAttributes[this.KeyOfIndex(1)], @"^(?<min>-?\d+)\.\.(?<max>-?\d+)$");

                                if (m.Success)
                                {
                                    // Если задан не верный промежуток
                                    if (Convert.ToInt32(m.Groups["min"].Value) > Convert.ToInt32(m.Groups["max"].Value))
                                    {
                                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                    }
                                    this.ParsedAttributes[ColorSet.MinValueAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.MaxValueAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(this.KeyOfIndex(1));
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
                                if (Convert.ToInt32(this.ParsedAttributes[this.KeyOfIndex(1)]) > Convert.ToInt32(this.ParsedAttributes[this.KeyOfIndex(2)]))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }

                                this.ParsedAttributes[ColorSet.MinValueAttribute] = this.ParsedAttributes[this.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.MaxValueAttribute] = this.ParsedAttributes[this.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(this.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(this.KeyOfIndex(2));
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

                    #region Магическое разбираение магических аттрибутов строк
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
                                var m = Regex.Match(this.ParsedAttributes[this.KeyOfIndex(1)], @"^(?<min>.+)\.\.(?<max>.+)$");
                                if (m.Success)
                                {
                                    this.ParsedAttributes[ColorSet.FromStringAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToStringAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes[ColorSet.MinLengthAttribute] = ColorSet.EmptyAttribute;
                                    this.ParsedAttributes[ColorSet.MaxLengthAttribute] = ColorSet.EmptyAttribute;
                                    this.ParsedAttributes.Remove(this.KeyOfIndex(1));
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
                                var m = Regex.Match(this.ParsedAttributes[this.KeyOfIndex(1)], @"^(?<min>.+)\.\.(?<max>.+)$");
                                if (m.Success)
                                {
                                    this.ParsedAttributes[ColorSet.FromStringAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToStringAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(this.KeyOfIndex(1));
                                }
                                else
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }

                                m = Regex.Match(this.ParsedAttributes[this.KeyOfIndex(2)], @"^(?<min>\d+)\.\.(?<max>\d+)$");
                                if (m.Success)
                                {
                                    this.ParsedAttributes[ColorSet.MinLengthAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.MaxLengthAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(this.KeyOfIndex(2));
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
                                this.ParsedAttributes[ColorSet.TrueStringAttribute] = this.ParsedAttributes[this.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.FalseStringAttribute] = this.ParsedAttributes[this.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(this.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(this.KeyOfIndex(2));
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

                    break;
                case ColorSetType.Index:

                    switch (this.ParsedAttributes.Keys.Count)
                    {
                        case 1: // Только промежуток (1..5)
                            if (this.IsArrayAttributes())
                            {
                                // Выделяем промежуток
                                var m = Regex.Match(this.ParsedAttributes[this.KeyOfIndex(1)], @"^(?<min>-?\d+)\.\.(?<max>-?\d+)$");

                                if (m.Success)
                                {
                                    // Если задан не верный промежуток
                                    if (Convert.ToInt32(m.Groups["min"].Value) > Convert.ToInt32(m.Groups["max"].Value))
                                    {
                                        throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                    }
                                    this.ParsedAttributes[ColorSet.FromAttribute] = m.Groups["min"].Value;
                                    this.ParsedAttributes[ColorSet.ToAttribute] = m.Groups["max"].Value;
                                    this.ParsedAttributes.Remove(this.KeyOfIndex(1));
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
                                if (Convert.ToInt32(this.ParsedAttributes[this.KeyOfIndex(1)]) > Convert.ToInt32(this.ParsedAttributes[this.KeyOfIndex(2)]))
                                {
                                    throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                                }
                                this.ParsedAttributes[ColorSet.FromAttribute] = this.ParsedAttributes[this.KeyOfIndex(1)];
                                this.ParsedAttributes[ColorSet.ToAttribute] = this.ParsedAttributes[this.KeyOfIndex(2)];
                                this.ParsedAttributes.Remove(this.KeyOfIndex(1));
                                this.ParsedAttributes.Remove(this.KeyOfIndex(2));
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
                        if (!this.Collection.Contains(attr.Value))
                        {
                            throw new InvalidColorSetAttributesException(this.Name, this.Attributes);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Валидация значения фишки по отношению к правилам текущего цвета.
        /// </summary>
        /// <param name="token">Фишка</param>
        /// <returns></returns>
        public bool IsLegal(Token token)
        {
            switch (this.Type)
            {
                case ColorSetType.Unit:

                    return token.Value.Equals(ColorSet.UnitDefault) || token.Value.Equals(this.ParsedAttributes[ColorSet.NameAttribute]);

                case ColorSetType.Int:

                    int val;
                    if (!Int32.TryParse(token.Value, out val))
                    {
                        return false;
                    }
                    string min = this.ParsedAttributes[ColorSet.MinValueAttribute];
                    string max = this.ParsedAttributes[ColorSet.MaxValueAttribute];
                    
                    return (min.Equals(ColorSet.EmptyAttribute) || Convert.ToInt32(min) <= val) &&
                            (max.Equals(ColorSet.EmptyAttribute) || Convert.ToInt32(max) >= val);

                case ColorSetType.Bool:

                    return token.Value.Equals(ColorSet.TrueDefault) || token.Value.Equals(ColorSet.FalseDefault);

                case ColorSetType.String:

                    bool f = true;

                    if (this.ParsedAttributes[ColorSet.MinLengthAttribute] != ColorSet.EmptyAttribute)
                    {
                        f = f && token.Value.Length >= Convert.ToInt32(this.ParsedAttributes[ColorSet.MinLengthAttribute]);
                    }
                    if (this.ParsedAttributes[ColorSet.MaxLengthAttribute] != ColorSet.EmptyAttribute)
                    {
                        f = f && token.Value.Length <= Convert.ToInt32(this.ParsedAttributes[ColorSet.MaxLengthAttribute]);
                    }
                    if (this.ParsedAttributes[ColorSet.FromStringAttribute] != ColorSet.EmptyAttribute)
                    {
                        bool f2 = true;
                        foreach (var c in token.Value)
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
                        f = f && token.Value.CompareTo(this.ParsedAttributes[ColorSet.ToStringAttribute]) <= 0;
                    }

                    return f;
                case ColorSetType.Enum:

                    foreach (var item in this.ParsedAttributes)
                    {
                        if (item.Value.Equals(token.Value))
                            return true;
                    }

                    return false;

                case ColorSetType.Index:

                    var m = Regex.Match(token.Value, String.Format(ColorSet.RegexMaskOfIndexes, this.Name));

                    if (m.Success)
                    {
                        return Convert.ToInt32(m.Groups["index"].Value) >= Convert.ToInt32(this.ParsedAttributes[ColorSet.FromAttribute]) &&
                            Convert.ToInt32(m.Groups["index"].Value) <= Convert.ToInt32(this.ParsedAttributes[ColorSet.ToAttribute]);
                    }

                    return false;

                case ColorSetType.Record:

                    var parsedToken = this.ParseAttributes(token.Value);

                    foreach (var item in this.ParsedAttributes)
                    {
                        if (!parsedToken.ContainsKey(item.Key))
                        {
                            return false;
                        }

                        if (!this.Collection[item.Value].IsLegal(new Token(parsedToken[item.Key])))
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
            if (!this.IsLegal(token))
            {
                throw new InvalidTokenException(token.Value, this.Name);
            }

            var attrs = this.ParseAttributes(token.Value);
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
                    foreach (var item in this.ParseAttributes(token.Value))
                    {
                        i++;
                        Token t = new Token(item.Value);
                        ColorSet color = this.Collection[this.ParsedAttributes[item.Key]];

                        sb.AppendFormat("[{0}] => {1}, ", item.Key, color.ShowToken(t));
                    }
                    sb.Remove(sb.Length - 2, 2);

                    return String.Format("({0})", sb.ToString());

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
            foreach (var item in ColorSet.typeNames)
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
                tokens.Add(new Token(token));
            }

            return tokens;
        }
    }
}
