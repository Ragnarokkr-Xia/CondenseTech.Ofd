using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CondenseTech.Ofd.BasicType
{
    public class ST_Array<T>
    {
        public ST_Array()
        {
        }

        public ST_Array(string raw)
        {
            this.Raw = raw;
        }

        public string Raw
        {
            get => string.Join(" ", List);
            set
            {
                value = value.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Type targetType = typeof(T);
                    string[] contents = value.Split(' ');
                    try
                    {
                        foreach (var content in contents)
                        {
                            try
                            {
                                T element = (T)Convert.ChangeType(content, targetType);
                                List.Add(element);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidCastException($"Cannot convert {content} to {targetType.FullName}, refer to the inner exception for more information.", ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Cannot convert \"{value}\" to an ST_Array, refer to the inner exception for more information.", ex);
                    }
                }
            }
        }

        public List<T> List { get; } = new List<T>();

        public static implicit operator List<T>(ST_Array<T> stArray)
        {
            return stArray.List;
        }

        public static implicit operator string(ST_Array<T> stArray)
        {
            return stArray.Raw;
        }

        public static string NormalizeDeltaArrayString(string gArrayString)
        {
            if (!string.IsNullOrWhiteSpace(gArrayString))
            {
                Regex gRegex = new Regex("g ([0-9]+) ([0-9.]+)");
                while (true)
                {
                    var gMatch = gRegex.Match(gArrayString);
                    if (!gMatch.Success)
                        break;
                    int gCount = int.Parse(gMatch.Groups[1].Value);
                    string gDelta = gMatch.Groups[2].Value;
                    string gReplacement = string.Join(" ", Enumerable.Repeat(gDelta, gCount).ToArray());
                    gArrayString = gRegex.Replace(gArrayString, gReplacement, 1);
                }

                return gArrayString;
            }

            return null;
        }
    }
}