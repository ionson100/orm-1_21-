using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ORM_1_21_.geo
{
    static class UtilsGeo
    {
        public static List<GeoPoint> GetListPoint(GeoType type, string str,List<IGeoShape> geoShapes)
        {
            List<GeoPoint> list = new List<GeoPoint>();
            switch (type)
            {
                case GeoType.None:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Geo type empty");
                case GeoType.Point:
                {
                    str = str.Replace("POINT", "").Replace(")", "").Replace("(", "");
                    var s = str.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    list.Add(new GeoPoint { X = double.Parse(s[0], CultureInfo.InvariantCulture), Y = double.Parse(s[1], CultureInfo.InvariantCulture) });
                    
                    break;
                }
                case GeoType.LineString:
                {
                    str = str.Replace("LINESTRING", "").Replace(")", "").Replace("(", "");
                    var s = str.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s1 in s)
                    {
                        var ss = s1.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        list.Add(new GeoPoint
                        {
                            X = double.Parse(ss[0], CultureInfo.InvariantCulture),
                            Y = double.Parse(ss[1], CultureInfo.InvariantCulture)
                        });
                    }
                    break;
                }
                case GeoType.CircularString:
                {
                    str = str.Replace("CIRCULARSTRING", "").Replace(")", "").Replace("(", "");
                    var s = str.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s1 in s)
                    {
                        var ss = s1.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        list.Add(new GeoPoint
                        {
                            X = double.Parse(ss[0], CultureInfo.InvariantCulture),
                            Y = double.Parse(ss[1], CultureInfo.InvariantCulture)
                        });
                    }
                    break;
                }
                case GeoType.MultiPoint:
                {
                    Regex regex = new Regex(@"\(([^)]+)\)");
                    MatchCollection matches = regex.Matches(str);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            var rr = match.Value.Trim('(', ')', ' ');
                            string[] d = rr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in d)
                            {
                                string[] ds = s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                var sp = new GeoPoint
                                    { X = double.Parse(ds[0]), Y = double.Parse(ds[1]) };
                                list.Add(sp);
                                geoShapes.Add(new GeoObject(GeoType.Point,sp));
                            }
                            

                        }

                    }
                    else
                    {
                        throw new Exception("Совпадений не найдено");
                    }
                    break;
                        
                }
                case GeoType.Polygon:
                {
                    str = str.Replace("POLYGON", "").Replace(")", "").Replace("(", "");
                    var s = str.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s1 in s)
                    {
                        var ss = s1.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        list.Add(new GeoPoint
                        {
                            X = double.Parse(ss[0], CultureInfo.InvariantCulture),
                            Y = double.Parse(ss[1], CultureInfo.InvariantCulture)
                        });
                    }
                    break;
                }
                case GeoType.MultiLineString:
                {
                    //str = str.Trim("MULTILINESTRING ()".ToCharArray());

                    Regex regex = new Regex(@"\(([^)]+)\)");
                    MatchCollection matches = regex.Matches(str);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            var rr = match.Value.Trim('(',')',' ');
                            geoShapes.Add(new GeoObject($"LINESTRING({rr})"));
                            
                        }
                            
                    }
                    else
                    {
                        throw new Exception("Совпадений не найдено");
                    }
                    break;
                }
                case GeoType.MultiPolygon:
                {
                    //str = str.Trim("MULTILINESTRING ()".ToCharArray());

                    Regex regex = new Regex(@"\(([^)]+)\)");
                    MatchCollection matches = regex.Matches(str);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            var rr = match.Value.Trim('(', ')', ' ');
                            geoShapes.Add(new GeoObject($"POLYGON(({rr}))"));

                        }

                    }
                    else
                    {
                        throw new Exception("Совпадений не найдено");
                    }
                    break;
                }
                case GeoType.PolygonWithHole:
                {
                    //str = str.Trim("MULTILINESTRING ()".ToCharArray());

                    Regex regex = new Regex(@"\(([^)]+)\)");
                    MatchCollection matches = regex.Matches(str);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            var rr = match.Value.Trim('(', ')', ' ');
                            geoShapes.Add(new GeoObject($"POLYGON({rr})"));

                        }

                    }
                    else
                    {
                        throw new Exception("Совпадений не найдено");
                    }
                    break;
                }
                case GeoType.GeometryCollection:
                {
                    
                   
                    foreach (var s1 in FactoryGeo.ParseGeoCollection(str))
                    {
                        geoShapes.Add(new GeoObject(s1));
                    }

                    break;
                   
                }
                case GeoType.Empty:
                {
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return list;
        }
    }
}

//POLYGON\(([^)]+)\)\)|POINT\(([^)]+)\)|LINESTRING\(([^)]+)\)|GEOMETRYCOLLECTION\(([^)]+)\)\)|MULTIPOINT\(\((.+?)\)\)|MULTIPOLYGON\(\((.+?)\)\)\)|MULTILINESTRING\(\((.+?)\)\)