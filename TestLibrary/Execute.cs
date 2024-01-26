﻿using Newtonsoft.Json;
using ORM_1_21_;
using ORM_1_21_.Extensions;
using ORM_1_21_.geo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable All

namespace TestLibrary
{

    public class ExeGeo
    {
        public static void Run()
        {

           //  TestJsonIon100<JPosO, MyDbPostgresGeo>(ProviderName.PostgreSql);
           //  TestJsonIon100<JMyO, MyDbMySql>(ProviderName.MySql);
           //  TestJsonIon100<JMsO, MyDbMsSql>(ProviderName.MsSql);
           //  TestJsonIon100<JSqliteO, MyDbSqlite>(ProviderName.SqLite);
           // 
           // 
           // TestJsonObjec<JPosS, MyDbPostgresGeo>(ProviderName.PostgreSql);
           // TestJsonObjec<JMyS, MyDbMySql>(ProviderName.MySql);
           // TestJsonObjec<JMsS, MyDbMsSql>(ProviderName.MsSql);
           // TestJsonObjec<JSqliteS, MyDbSqlite>(ProviderName.SqLite);
            
            
           // TestGeoExtension<GeoPos, MyDbPostgresGeo>(ProviderName.PostgreSql);
           // TestGeoExtension<GeoMs, MyDbMsSql>(ProviderName.MsSql);
           // TestGeoExtension<GeoMy, MyDbMySql>(ProviderName.MySql);
           // ////
           // ////
           // ////
           // TestGeoObject<GeoPos, MyDbPostgresGeo>(ProviderName.PostgreSql);
           // TestGeoObject<GeoMs, MyDbMsSql>(ProviderName.MsSql);
           // TestGeoObject<GeoMy, MyDbMySql>(ProviderName.MySql);
           // ////
           // TestUpdate<UpPost, MyDbPostgresGeo>(ProviderName.PostgreSql);
           // TestUpdate<UpMs, MyDbMsSql>(ProviderName.MsSql);
           // TestUpdate<UpMys, MyDbMySql>(ProviderName.MySql);


           




        }

        public static async Task RunAsync()
        {
            //await TestCillGeoExtensionAsync<GeoPos, MyDbPostgresGeo>(ProviderName.PostgreSql);
            // await TestGeoExtensionAsync<GeoPos, MyDbPostgresGeo>(ProviderName.PostgreSql);

            TestUpdateSqlObjec<JPosO, MyDbPostgresGeo>(ProviderName.PostgreSql);

        }

        public static void TestUpdateSqlObjec<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new()
            where T : JsonBaseO, new()
        {
            Console.WriteLine($"************************Test UpdateSql as Object {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {
                ses.TruncateTable<T>();
                ses.InsertBulk(new List<T>
                {
                    new T { Name = "1", Ion100 = new Ion100 { Name = "1", Age = 1 } },
                    new T { Name = "2", Ion100 = new Ion100 { Name = "2", Age = 2 } },
                    new T { Name = "3", Ion100 = new Ion100 { Name = "3", Age = 3 } },
                    new T { Name = "4", Ion100 = new Ion100 { Name = "4", Age = 4 } },
                });
                var er = ses.Query<T>().Where(s => s.Name == "1").UpdateSqlE(a =>$"{a.Name}={DateTime.Now:D}");
                //var er = ses.Query<T>().Where(s=>s.Name=="1").UpdateSqlE(a => ""+a.Name+" = '122',"+a.Ion100+" = null");

                List<T> list = ses.Query<T>().ToList();
                // int res=ses.Query<T>().Where(s=>s.Name=="1").Update(a => new Dictionary<object, object>
                //{
                //    { a.Name, "33" }
                //});
                //list = ses.Query<T>().ToList();
                int res2 = ses.Query<T>().Where(s=>s.Name=="1").UpdateSql($" {ses.ColumnName<T>(a=>a.Name)} = '33'");
                list = ses.Query<T>().ToList();

            }
        }

        static string AddString()
        {
            return "\"Name\"='asas'";
        }




        public static async Task TestCillGeoExtensionAsync<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new()
            where T : GeoBase, new()
        {
            Console.WriteLine($"************************ Test cill geo extension {providerName}*************************");
            var geo = FactoryGeo.CreateGeo("LINESTRING(50 100, 50 150)");
            IGeoShape geo2 = FactoryGeo.CreateGeo("LINESTRING(50 50, 50 200)").SetSession(Configure.GetSession<TD>()).StDifference(geo);
            var sddd=geo2.StDifference(geo);
            using (var ses = await Configure.GetSessionAsync<TD>())
            {
                
                geo2 = await FactoryGeo.CreateGeo("LINESTRING(50 50, 50 200)").SetSession(ses).StDifferenceAsync(geo);
                var o = await geo2.StDifferenceAsync(geo);

            }
            var o1 = await geo2.StDifferenceAsync(geo);

        }

        public static async Task TestGeoExtensionAsync<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new()
            where T : GeoBase, new()
        {
            Console.WriteLine($"************************ Test geo extension {providerName}*************************");
            using (var ses = await Configure.GetSessionAsync<TD>())
            {
                await ses.TruncateTableAsync<T>();

                #region StIsValidReasonAsync

                {
                    Console.WriteLine("******* StIsValidReasonAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StIsValidReasonAsync();
                }
                #endregion

                #region StMakeValidAsync

                {
                    Console.WriteLine("******* StMakeValidAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StMakeValidAsync();
                }
                #endregion

                #region StAsGeoJSONAsync

                {
                    Console.WriteLine("******* StAsGeoJSONAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StAsGeoJsonAsync();
                }
                #endregion

                #region StPointOnSurfaceAsync

                {
                    Console.WriteLine("******* StPointOnSurfaceAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StPointOnSurfaceAsync();
                }
                #endregion

                #region StInteriorRingNAsync

                {
                    Console.WriteLine("******* StInteriorRingNAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StInteriorRingNAsync(1);
                }
                #endregion

                #region StXAsync

                {
                    Console.WriteLine("******* StXAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)").SetSession(ses).StXAsync();
                }
                #endregion

                #region StYAsync

                {
                    Console.WriteLine("******* StYAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)").SetSession(ses).StYAsync();
                }
                #endregion

                #region StTransform

                {
                    Console.WriteLine("******* StTransform");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StTransformAsync(4326);
                }
                #endregion

                #region StAsLatLonTextAsync

                {
                    Console.WriteLine("******* StAsLatLonTextAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)").SetSession(ses).StAsLatLonTextAsync("D°M''S.SSS\"");
                }
                #endregion

                #region StReverseAsync

                {
                    Console.WriteLine("******* StReverseAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StReverseAsync();
                }
                #endregion

                #region StPerimeterAsync

                {
                    Console.WriteLine("******* StPerimeterAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StPerimeterAsync();
                }
                #endregion

                #region StTranslateAsync

                {
                    Console.WriteLine("******* StTranslateAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StTranslateAsync(1,3);
                }
                #endregion

                #region StConvexHullAsync

                {
                    Console.WriteLine("******* StConvexHullAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StConvexHullAsync();
                }
                #endregion

                #region StPointN

                {
                    Console.WriteLine("******* StNumInteriorRingAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StPointNAsync(1);
                }
                #endregion

                #region StNumInteriorRingAsync

                {
                    Console.WriteLine("******* StNumInteriorRingAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StNumInteriorRingAsync();
                }
                #endregion

                #region StNumPointsAsync

                {
                    Console.WriteLine("******* StNumPointsAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StNumPointsAsync();
                }
                #endregion

                #region StIsClosedAsync

                {
                    Console.WriteLine("******* StIsClosedAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StIsClosedAsync();
                }
                #endregion

                #region StLengthAsync

                {
                    Console.WriteLine("******* StLengthAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StLengthAsync();
                }
                #endregion

                #region StIsValidAsync

                {
                    Console.WriteLine("******* StIsValidAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StIsValidAsync();
                }
                #endregion

                #region StIsSimpleAsync

                {
                    Console.WriteLine("******* StIsSimpleAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StIsSimpleAsync();
                }
                #endregion

                #region StNumGeometriesAsync

                {
                    Console.WriteLine("******* StNumGeometriesAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StNumGeometriesAsync();
                }
                #endregion

                #region StTouchesAsync

                {
                    Console.WriteLine("******* StTouchesAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StTouchesAsync(geo);
                }
                #endregion

                #region StOverlapsAsync

                {
                    Console.WriteLine("******* StOverlapsAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StOverlapsAsync(geo);
                }
                #endregion

                #region StIntersectsAsync

                {
                    Console.WriteLine("******* StIntersectsAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StIntersectsAsync(geo);
                }

                #endregion

                #region StEqualsAsync

                {
                    Console.WriteLine("******* StEqualsAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StEqualsAsync(geo);
                }

                #endregion

                #region StDistanceAsync

                {
                    Console.WriteLine("******* StDistanceAsync");
                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = await FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)").SetSession(ses).StDistanceAsync(geo);
                }

                #endregion

                #region StDisjointAsync

                {
                    Console.WriteLine("******* StDisjointAsync");
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");

                    var o = await geo.SetSession(ses).StDisjointAsync(geo2);
                }

                #endregion

                #region StDimensionAsync

                {
                    Console.WriteLine("******* StDimensionAsync");
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                  
                    var o = await geo.SetSession(ses).StDimensionAsync();

                }

                #endregion

                #region StCrossesAsync

                {
                    Console.WriteLine("******* StDifferenceAsync");
                    var geo = FactoryGeo.CreateGeo("LINESTRING(50 100, 50 150)");
                    IGeoShape geo2 = await FactoryGeo.CreateGeo("LINESTRING(50 50, 50 200)").SetSession(ses).StDifferenceAsync(geo);
                }

                #endregion

                #region StCrossesAsync

                {
                    Console.WriteLine("******* StCrossesAsync");
                    var geo = FactoryGeo.LineString(0,0,1,4);
                    var geo2 = await FactoryGeo.CreateGeo("point(1 4)").SetSession(ses).StBuffer(50).SetSession(ses).StCrossesAsync(geo);
                }

                #endregion

                #region StContainsAsync

                {
                    Console.WriteLine("******* StContainsAsync");
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = await FactoryGeo.CreateGeo("point(1 4)").SetSession(ses).StContainsAsync(geo);
                }

                #endregion

                #region StUnionAsync

                {
                    Console.WriteLine("******* StUnionAsync");
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = await FactoryGeo.CreateGeo("point(1 4)").SetSession(ses).StUnionAsync(geo); 
                }

                #endregion

                #region StSymDifferenceAsync

                {
                    Console.WriteLine("******* StSymDifferenceAsync");
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = await FactoryGeo.CreateGeo("point(1 4)").SetSession(ses).StSymDifferenceAsync(geo);
                }

                #endregion

                #region StStartPointAsync

                {
                    Console.WriteLine("******* StStartPointAsync");
                    var shape = FactoryGeo.CreateGeo("LINESTRING(1 1, 2 2, 3 3)");
                    var res = await shape.SetSession(ses).StStartPointAsync();
                }

                #endregion

                #region StEnvelopeAsync

                {
                    Console.WriteLine("******* StEnvelopeAsync");
                    var shape = FactoryGeo.CreateGeo("POLYGON((0 0, 0 1, 1.0000001 1, 1.0000001 0, 0 0))");
                    var res = await shape.SetSession(ses).StEnvelopeAsync();
                }

                #endregion

                #region StEndPointAsync

                {
                    Console.WriteLine("******* StEndPointAsync");
                    var shape = FactoryGeo.CreateGeo("LINESTRING(1 1, 2 2, 3 3)");
                    var res = await shape.SetSession(ses).StEndPointAsync();
                }

                #endregion

                #region StCentroidAsync

                {
                    {
                        Console.WriteLine("******* StCentroidAsync");
                        var shape = FactoryGeo.CreateGeo("MULTIPOINT ( -1 0, -1 2, -1 3, -1 4, -1 7, 0 1, 0 3, 1 1, 2 0, 6 0, 7 8, 9 8, 10 6 )");
                        var res = await shape.SetSession(ses).StCentroidAsync();
                    }
                }

                #endregion

                #region StBufferAsync

                {
                    {
                        Console.WriteLine("******* StBufferAsync");
                        var shape = FactoryGeo.CreateGeo("POINT(100 90)");
                        var res = await shape.SetSession(ses).StBufferAsync(50);
                    }
                }

                #endregion

                #region StBoundaryAsync

                {
                    Console.WriteLine("******* StBoundaryAsync");
                    var shape = FactoryGeo.CreateGeo("LINESTRING(100 150,50 60, 70 80, 160 170)");
                    var res = await shape.SetSession(ses).StBoundaryAsync();
                }

                #endregion

                #region StGeometryTypeAsync

                {
                    Console.WriteLine("******* StGeometryTypeAsync");
                    var shape = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07, 77.42 29.26, 77.27 29.31, 77.29 29.07)");
                    var res = await shape.SetSession(ses).StGeometryTypeAsync();
                } 
                #endregion

                #region StAreaAsync

                {
                    Console.WriteLine("******* StAreaAsync");
                    var shape = FactoryGeo.CreateGeo("POLYGON((743238 2967416,743238 2967450,\r\n\t\t\t\t 743265 2967450,743265.625 2967416,743238 2967416))").SetSrid(2249);
                    var res = await shape.SetSession(ses).StAreaAsync();
                }



                #endregion

                #region StWithinAsync

                {
                    {
                        Console.WriteLine("******* StWithinAsync");
                        var shape = FactoryGeo.CreateGeo("POLYGON((1 1, 1 2, 2 2, 2 1, 1 1))");
                        var res = await shape.SetSession(ses).StWithinAsync(FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0))"));
                    }
                }

                #endregion

                #region StAsBinaryAsync

                {
                    {
                        Console.WriteLine("******* StAsBinaryAsync");
                        var shape = FactoryGeo.CreateGeo("POLYGON((1 1, 1 2, 2 2, 2 1, 1 1))");
                        var res = await shape.SetSession(ses).StAsBinaryAsync();
                    }
                }

                #endregion


            }
        }


        public static void TestJsonObjec<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new() where T : JsonBaseS, new()
        {
            Console.WriteLine($"************************Test json as Object {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {
                ses.DropTableIfExists<T>();
                ses.TableCreate<T>();
                T jsonPos = new T
                {
                    Name = "1'",
                    Ion100 = JsonConvert.SerializeObject(new Ion100 { Age = 10, Name = "10" })
                };
                ses.Insert(jsonPos);
                T jsonPos2 = new T
                {
                    Name = "2222'",
                    Ion100 = new { Age = 20, Name = "20" }
                };
                ses.InsertBulk(new List<T> { jsonPos2 });

                var list = ses.Query<T>().ToList();
                var pos = list.First();
                pos.Name = "123";
                pos.Ion100 = new Ion100 { Age = 100, Name = "100" };
                ses.Update(pos);
                pos.Ion100 = new { name = "asas", Age = 100 };
                ses.Update(pos);

                var t = new Ion100 { Age = 120, Name = "120" };
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, JsonConvert.SerializeObject(t) }
                });
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, JsonConvert.SerializeObject(new{bb=1}) }
                });

                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, new Ion100() }
                });
                var s = ses.Query<T>().Select(a => a.Ion100).ToList();
                var sn = ses.Query<T>().Select(a => new { n = a.Name, d = a.Ion100 }).ToList();
                list = ses.Query<T>().ToList();

                var sql = $"select {ses.StarSql<T>()} from {ses.TableName<T>()}";
                list = ses.FreeSql<T>(sql).ToList();
                ses.TruncateTable<T>();
                ses.Insert(new T() { Name = "2", Ion100 = new { Age = 100, Name = "100" } });
                ses.Insert(new T() { Name = "2", Ion100 = new Ion100 { Age = 100, Name = "100" } });
                ses.Insert(new T() { Name = "2", Ion100 = new Ion100 { Age = 200, Name = "200" } });
                list = ses.FreeSql<T>(sql).ToList();
                if (providerName == ProviderName.MsSql)
                {
                    var o = ses.Query<T>().WhereSql("JSON_VALUE(ion100, '$.Age') = 100").Select(a => a.Ion100).ToList();
                }
                if (providerName == ProviderName.PostgreSql)
                {
                    var o = ses.Query<T>().WhereSql("ion100 @> '{\"Age\":100}'").Select(a => a.Ion100).ToList();
                }
            }
        }

        public static void TestGeoObject<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new()
            where T : GeoBase, new()
        {
            Console.WriteLine($"************************ Test geo object {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {

                #region GeometryCollection

                {
                    Console.WriteLine("********** GeometryCollection");
                    ses.TruncateTable<T>();
                    var col1 = FactoryGeo.GeometryCollection(
                        "GEOMETRYCOLLECTION (MULTIPOLYGON (((10 30, 30 30, 30 10, 10 10, 10 30))), POINT (2 3), LINESTRING (2 3, 3 4), POLYGON ((0 0, 1 0, 1 1, 0 1, 0 0)), POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0), (1 1, 1 2, 2 2, 2 1, 1 1)))");
                    var r = col1.GetGeoJson();
                    var reoJson = JsonConvert.SerializeObject(r);
                    var rd = FactoryGeo.GetGeometryFromGeoJson(reoJson);
                    var g1 = FactoryGeo.Point(2, 4);
                    var g2 = FactoryGeo.MultiPoint(new GeoPoint(4, 6), new GeoPoint(6, 7));
                    var g3 = FactoryGeo.LineString(new double[] { 3, 5, 7, 8, 10, 14 });
                    var g4 = FactoryGeo.MultiLineString(
                        "MULTILINESTRING((10 60, 60 20), (20 40, 60 120), (20 40, 80 20))");
                    var g5 = FactoryGeo.CreateGeo("POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))");
                    var g6 = FactoryGeo.CreateGeo("POLYGON((0 0, 10 0, 10 10, 0 10, 0 0),(1 1, 1 2, 2 2, 2 1, 1 1))");
                    var g7 = FactoryGeo.CreateGeo("MULTIPOLYGON (((1 5, 5 5, 5 1, 1 1, 1 5)), ((6 5, 9 1, 6 1, 6 5)))");
                    var g8 = col1;

                    var col3 = FactoryGeo.CreateGeometryCollection(g1, g2, g3, g4, g5, g6, g7);





                    var gs21 = JsonConvert.SerializeObject(col3.GetGeoJson());
                    var listGeo = FactoryGeo.GetGeometryFromGeoJson(gs21);

                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { col1, col3 })
                    {
                        var ss = shape.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true);
                        if (providerName == ProviderName.PostgreSql)
                        {
                            Console.WriteLine(shape.SetSession(ses).StIsValidReason());
                        }
                    }


                    var list1 = new List<T>()
                    {
                        new T { Name = "2", MyGeoObject = col3 },
                        new T { Name = "1", MyGeoObject = col1 },
                    };
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.GeometryCollection);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(),
                        Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, (res.MultiGeoShapes.Count == 7 || res.MultiGeoShapes.Count == 5) && res.ListGeoPoints.Count == 0 && res.GeoType == GeoType.GeometryCollection);



                }






                #endregion


                #region PolygonWithHole

                {
                    Console.WriteLine("********** PolygonWithHole");
                    ses.TruncateTable<T>();
                    var p1 = FactoryGeo.PolygonWithHole(
                        "POLYGON ( (0 0, 10 0, 10 10, 0 10, 0 0) ,( 1 1, 1 2, 2 2, 2 1, 1 1),( 1 1, 1 2, 2 2, 2 1, 1 1) )");
                    var p2 = FactoryGeo.PolygonWithHole(new Double[] { 0, 0, 10, 0, 10, 10, 0, 10, 0, 0 },
                        new double[] { 1, 1, 1, 2, 2, 2, 2, 1, 1, 1 });
                    var p3 = FactoryGeo.PolygonWithHole(FactoryGeo.Polygon(0, 0, 10, 0, 10, 10, 0, 10, 0, 0),
                        FactoryGeo.Polygon(1, 1, 1, 2, 2, 2, 2, 1, 1, 1));

                    var list1 = new List<T>()
                    {
                        new T { Name = "1", MyGeoObject = p1 },
                        new T { Name = "2", MyGeoObject = p2 },
                        new T { Name = "2", MyGeoObject = p3 },

                    };
                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { p1, p2, p3 })
                    {
                        var ss = shape.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true && (shape.ListGeoPoints.Count == 0) && shape.MultiGeoShapes.Count == 2);
                        if (providerName == ProviderName.PostgreSql)
                        {
                            Console.WriteLine(shape.SetSession(ses).StIsValidReason());
                        }
                    }
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.PolygonWithHole);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(),
                        Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, (res.MultiGeoShapes.Count == 3 || res.MultiGeoShapes.Count == 2) && res.ListGeoPoints.Count == 0 && res.GeoType == GeoType.PolygonWithHole);
                }

                #endregion


                #region MultiLineString

                {
                    Console.WriteLine("********** MultiLineString");
                    ses.TruncateTable<T>();
                    var p1 = FactoryGeo.MultiLineString("MULTILINESTRING  ( ( 0 0,1 1.6767, 1 2),  (2.9 3,3 2,5 4) )");
                    var p2 = FactoryGeo.MultiLineString(FactoryGeo.LineString(0, 0.455, 1, 1, 1, 2),
                        FactoryGeo.LineString(2, 3, 3.455, 2, 5, 4));

                    var list1 = new List<T>()
                    {
                        new T { Name = "1", MyGeoObject = p1 },
                        new T { Name = "2", MyGeoObject = p2 },

                    };

                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { p1, p2 })
                    {
                        var ss = shape.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true && (shape.ListGeoPoints.Count == 0) && shape.MultiGeoShapes.Count == 2);
                        if (providerName == ProviderName.PostgreSql)
                        {
                            Console.WriteLine(shape.SetSession(ses).StIsValidReason());
                        }
                    }
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.MultiLineString);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(),
                        Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 2 && res.ListGeoPoints.Count == 0 && res.GeoType == GeoType.MultiLineString);
                }

                #endregion

                #region LineString

                {
                    Console.WriteLine("********** LineString");
                    ses.TruncateTable<T>();
                    var p1 = FactoryGeo.LineString(10, 30, 30, 30.45, 30, 10, 10, 10);
                    var p2 = FactoryGeo.LineString("LINESTRING      " +
                                                   " (10 30, 30.34 30, 30 10,  10  10 )");
                    var p3 = FactoryGeo.LineString(
                        new GeoPoint(10, 30),
                        new GeoPoint(30.45, 30),
                        new GeoPoint(30.56, 10),
                        new GeoPoint(10, 10.56));
                    var list1 = new List<T>()
                    {
                        new T { Name = "1", MyGeoObject = p1 },
                        new T { Name = "2", MyGeoObject = p2 },
                        new T { Name = "2", MyGeoObject = p3 },

                    };
                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { p1, p2, p3 })
                    {
                        var ss = shape.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true && (shape.ListGeoPoints.Count == 4) && shape.MultiGeoShapes.Count == 0);
                        if (providerName == ProviderName.PostgreSql)
                        {
                            Console.WriteLine(shape.SetSession(ses).StIsValidReason());
                        }
                    }
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.LineString);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(),
                        Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 0 && res.ListGeoPoints.Count == 4 && res.GeoType == GeoType.LineString);
                }

                #endregion

                #region MultiPolygon

                {
                    Console.WriteLine("********** MultiPolygon");
                    ses.TruncateTable<T>();
                    var p0 = FactoryGeo.MultiPolygon("MULTIPOLYGON(((10 30,30 30,30 10,10 10,10 30)))");
                    var p1 = FactoryGeo.MultiPolygon(
                        FactoryGeo.Polygon(10, 30, 30, 30, 30, 10, 10, 10, 10, 30),
                        FactoryGeo.Polygon(10, 30, 30, 30, 30, 10, 10.56, 10, 10, 30));


                    var list1 = new List<T>()
                    {
                        new T { Name = "2", MyGeoObject = p1 },
                        new T { Name = "1", MyGeoObject = p0 },


                    };

                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { p0, p1 })
                    {
                        var ss = shape.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true && (shape.ListGeoPoints.Count == 0) && shape.MultiGeoShapes.Count == 1);
                        if (providerName == ProviderName.PostgreSql)
                        {
                            Console.WriteLine(shape.SetSession(ses).StIsValidReason());
                        }
                    }
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.MultiPolygon);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(),
                        Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 2 && res.ListGeoPoints.Count == 0 && res.GeoType == GeoType.MultiPolygon);
                }

                #endregion

                #region Polygon

                {
                    Console.WriteLine("********** Polygon");
                    ses.TruncateTable<T>();
                    var p0 = FactoryGeo.Polygon(30, 40, 25.09, 40, 25, 60.344, 30, 60, 30, 40);
                    var p1 = FactoryGeo.Polygon("POLYGON((30 40, 25.54545 40.545545, 25.5455 60, 30 60, 30 40))");
                    var p2 = FactoryGeo.Polygon(
                        new GeoPoint { X = 30, Y = 40 },
                        new GeoPoint { X = 25.3434, Y = 40.45455 },
                        new GeoPoint { X = 25.3434, Y = 60.3434 },
                        new GeoPoint { X = 30.3434, Y = 60 },
                        new GeoPoint { X = 30, Y = 40 }
                    );
                    int i = 0;
                    foreach (IGeoShape shape in new IGeoShape[] { p0, p1, p2 })
                    {
                        var ss = p0.SetSession(ses).StIsValid();
                        Execute.Log(++i, ss == true && (p0.ListGeoPoints.Count == 5) && p0.MultiGeoShapes.Count == 0);
                    }


                    var list1 = new List<T>()
                    {
                        new T { Name = "1", MyGeoObject = p0 },
                        new T { Name = "2", MyGeoObject = p1 },
                        new T { Name = "3", MyGeoObject = p2 }
                    };
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(dd => dd.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.Polygon);
                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(), Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 0 && res.ListGeoPoints.Count == 5 && res.GeoType == GeoType.Polygon);
                }


                #endregion

                #region Point

                {
                    Console.WriteLine("********** Point");
                    ses.TruncateTable<T>();
                    var gs = FactoryGeo.Point(75, 30);
                    var gs1 = FactoryGeo.Point(new GeoPoint { X = 60.2323, Y = 30.2323 });
                    var gs2 = FactoryGeo.Point("POINT(75.56 30.67)");
                    List<T> list1 = new List<T>
                    {
                        new T() { Name = "p1", MyGeoObject = gs },
                        new T() { Name = "p2", MyGeoObject = gs1 },
                        new T() { Name = "p3", MyGeoObject = gs2 }
                    };
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.Point);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(), Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 0 && res.ListGeoPoints.Count == 1 && res.GeoType == GeoType.Point);

                }



                #endregion

                #region MultiPoint

                {
                    Console.WriteLine("********** MultiPoint");
                    ses.TruncateTable<T>();
                    var mp = FactoryGeo.MultiPoint(new GeoPoint { X = 30, Y = 40 }, new GeoPoint { X = 31, Y = 41 });
                    var mp2 = FactoryGeo.MultiPoint(FactoryGeo.Point(30, 40), FactoryGeo.Point(31, 32));
                    IGeoShape mp3 = FactoryGeo.MultiPoint("MultiPoint ((30 40), (31 32))");
                    IGeoShape mp4 = FactoryGeo.MultiPoint(new double[] { 23.4, 5.566, 45.066, 34.344 });
                    var list1 = new List<T>()
                    {
                        new T { Name = "1", MyGeoObject = mp },
                        new T { Name = "2", MyGeoObject = mp2 },
                        new T { Name = "3", MyGeoObject = mp3 },
                        new T { Name = "3", MyGeoObject = mp4 }
                    };
                    ses.InsertBulk(list1);
                    var sasList = ses.Query<T>().Where(a => a.MyGeoObject != null).ToList();
                    var pF = sasList.FirstOrDefault(s => s.MyGeoObject.GeoType == GeoType.MultiPoint);

                    var pJson = JsonConvert.SerializeObject(pF.MyGeoObject.GetGeoJson(), Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(pJson);
                    var res = FactoryGeo.GetGeometryFromGeoJson(pJson);
                    Execute.Log(1, res.MultiGeoShapes.Count == 2 && res.ListGeoPoints.Count == 2 && res.GeoType == GeoType.MultiPoint);
                }




                #endregion
















            }
        }

        public static void TestUpdate<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new()
            where T : BaseUpdate, new()
        {

            Console.WriteLine($"************************ Test update {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {

                ses.DropTableIfExists<T>();
                ses.TableCreate<T>();
                ses.InsertBulk(new List<T> { new T(), new T() });
                ses.Insert(new T());
                //var list = ses.Query<T>().ToList();
                //var c = list[0];
                //c.Name = "asas";
                //c.Ion100 = new Ion100() { Age = 12, Name = "23" };
                //c.Geo = FactoryGeo.Point(3.45, 6.7888);
                //ses.Update(c);
                //list = ses.Query<T>().ToList();
                List<object> sas = ses.Query<T>().Select(a => a.Ion100).ToList();
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Ion100,JsonConvert.SerializeObject(new{s='a',d=34,r="asas"}) },


                });
                // list = ses.Query<T>().ToList();
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Date,new DateTime(1923, 12, 1) },
                    {a.decimal3,new decimal(34d)},

                });
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Ion100,new Ion100()}
                });
                int r = ses.Query<T>().Where(s => s.Name != null).Update(a => new Dictionary<object, object>
                {
                    { a.Name, "222" },
                    { a.Guid, new Guid(Guid.NewGuid().ToString()) },
                    {a.Geo,null},
                    {a.Ion100,JsonConvert.SerializeObject(DateTime.MaxValue)}

                });
                // list = ses.Query<T>().ToList();
            }
        }

      
        public static void TestGeoExtension<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new() where T : GeoBase, new()
        {
            Console.WriteLine($"************************ Test geo extension {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {

               
                ses.TruncateTable<T>();
                {
                    ses.Insert(new T { MyGeoObject = FactoryGeo.Point(1, 2) });
                    ses.Query<T>().Update(a => new Dictionary<object, object>
                    {
                        { a.MyGeoObject, FactoryGeo.CreateGeo(a.MyGeoObject.StAsText()) }
                    });
                    var l = ses.Query<T>().ToList();
                }



                #region StAsLatLonText

                {
                       Console.WriteLine("***** StAsLatLonText");
                       if (providerName == ProviderName.PostgreSql)
                       {
                           ses.TruncateTable<T>();
                           var geo = FactoryGeo.CreateGeo("POINT(-71.01 42.37)");
                    
                           ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    
                           var o = geo.SetSession(ses).StAsLatLonText();
                           o = geo.SetSession(ses).StAsLatLonText("");
                           //var sas = ses.Query<T>().Select(a => a.MyGeoObject.StAsLatLonText(null)).ToList();
                           //var sas3 = ses.Query<T>().Select(a => a.MyGeoObject.StAsLatLonText("D degrees, M minutes, S seconds to the C")).ToList();
                           ses.Query<T>().Select(a => a.MyGeoObject.StAsLatLonText("D°M''S.SSS\"")).ForEach(a => Console.WriteLine(a));
                    
                       }
                }

                #endregion


                #region StAsGeoJSON

                {
                    Console.WriteLine("***** StAsGeoJSON");
                    ses.TruncateTable<T>();
                    if (providerName == ProviderName.PostgreSql)
                    {
                        var shape = FactoryGeo.CreateGeo("POLYGON ((0 0, 1 0, 1 1, 0 1, 0 0))");
                        string res = shape.SetSession(ses).StAsGeoJson();
                        ses.Insert(new T { MyGeoObject = shape });
                        List<string> o = ses.Query<T>().Select(a => a.MyGeoObject.StAsGeoJson()).ToList();
                    }
                }



                #endregion

                #region StMakeValid

                {
                    Console.WriteLine("***** StMakeValid");
                    ses.TruncateTable<T>();
                    if (providerName == ProviderName.PostgreSql)
                    {
                        var shape = FactoryGeo.CreateGeo("LINESTRING(0 0, 0 0)");
                        var res = shape.SetSession(ses).StMakeValid();
                        ses.Insert(new T { MyGeoObject = shape });
                        var o = ses.Query<T>().Select(a => a.MyGeoObject.StMakeValid()).ToArray();
                    }
                }



                #endregion

                #region StGeometryType
                Console.WriteLine("***** StGeometryType");
                ses.TruncateTable<T>();
                {
                    var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 1 0, 1 1, 0 1, 0 0))");
                    var str = geo.SetSession(ses).StGeometryType();
                    Execute.Log(1, str == GeoType.Polygon.GetTypeName(providerName), str);
                    ses.Insert(new T { MyGeoObject = geo });
                }

                {
                    var geo = FactoryGeo.Empty(GeoType.GeometryCollection);
                    var str = geo.SetSession(ses).StGeometryType();
                    Execute.Log(2, str == GeoType.GeometryCollection.GetTypeName(providerName), str);
                    ses.Insert(new T { MyGeoObject = geo });
                }
                ses.Insert(new T() { MyGeoObject = null });
                {
                    var list = ses.Query<T>().Select(a => a.MyGeoObject.StGeometryType()).ToList();
                    Execute.Log(3, list.Count == 3, JsonConvert.SerializeObject(list));
                }
                {
                    var list = ses.Query<T>().Select(a => a.MyGeoObject).ToList();
                    list = ses.Query<T>().Where(a => a.MyGeoObject != null).Select(a => a.MyGeoObject).ToList();
                    var tt = ses.Query<T>().Where(s => s.MyGeoObject != null).Select(a => new { name = a.MyGeoObject.StGeometryType() }).ToList();

                }
                {
                    var list = ses.Query<T>().Where(s => s.MyGeoObject != null).Select(a => new { name = a.MyGeoObject.StGeometryType() }).ToList();
                    Execute.Log(4, list.Count == 2);
                }
                {
                    var sql = $"select {ses.StarSql<T>()} from {ses.TableName<T>()}";
                    var list = ses.FreeSql<T>(sql).ToList();
                    Execute.Log(5, list.Count == 3);
                }

                #endregion

                #region TestSrid
                Console.WriteLine("***** TestSrid");
                {
                    ses.DropTableIfExists<T>();
                    ses.TableCreate<T>();
                    ses.InsertBulk(new List<T>() { new T() });
                    var sass = ses.Query<T>().First();
                    Execute.Log(121, sass.MyGeoObject.StSrid() == FactoryGeo.DefaultSrid);
                    if (providerName == ProviderName.PostgreSql || providerName == ProviderName.MsSql)
                    {
                        sass.MyGeoObject.SetSession(ses).StSetSRID(0);
                    }

                    sass.MyGeoObject.SetSrid(0);

                    ses.Update(sass);
                    sass = ses.Query<T>().First();
                    Execute.Log(122, sass.MyGeoObject.StSrid() == 0);
                    ses.TruncateTable<T>();
                    ses.Insert(new T());
                    sass = ses.Query<T>().First();
                    Execute.Log(123, sass.MyGeoObject.StSrid() == FactoryGeo.DefaultSrid);
                    sass.MyGeoObject.SetSrid(0);
                    ses.Update(sass);
                    sass = ses.Query<T>().First();
                    Execute.Log(124, sass.MyGeoObject.StSrid() == 0);
                    ses.TruncateTable<T>();
                    ses.InsertBulk(new List<T> { new T() { MyGeoObject = null } });
                    ses.Insert(new T { MyGeoObject = null });
                    var list = ses.Query<T>().ToList();
                    Execute.Log(125, list.Count == 2);

                    ses.Query<T>().Update(a => new Dictionary<object, object>
                    {
                        { a.MyGeoObject, FactoryGeo.Point(2.2323, 4.232323).SetSrid(0) },
                        { a.MyGeoObject2, FactoryGeo.Point(2.2323, 4.232323).SetSrid(0) }
                    });
                    list = ses.Query<T>().ToList();

                }

                #endregion

                #region StCollect

                {
                    Console.WriteLine("***** StCollect");

                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("point(1 2)");
                        var geo2 = FactoryGeo.CreateGeo("point(1 4)");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });

                        //ses.Insert(new T { Name = "p", MyGeoObject = null });
                        // var o = geo.SetSession(ses).StUnion(geo2);
                        var sasr = ses.Query<T>().ToList();
                        var asss = FactoryGeo.Point(3, 4);
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StCollect(asss)).ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StCollect(FactoryGeo.Point(3, 4)).StAsText()).ToList();
                    }

                }

                #endregion

                #region TestWhereIn

                {
                    Console.WriteLine("***** TestWhereIn");

                    ses.Insert(new T { Name = "1" });
                    ses.Insert(new T { Name = "2" });
                    ses.Insert(new T { Name = "3" });
                    var res = ses.Query<T>().WhereIn(a => a.Name, "1", "2").ToList();
                    Execute.Log(112, res.Count == 2, "where in");
                    res = ses.Query<T>().WhereNotIn(a => a.Name, "1", "2").ToList();
                    Execute.Log(113, res.Count == 1, "where in");
                }

                #endregion

                #region TestSelectSql

                {
                    Console.WriteLine("***** TestSelectSql");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POLYGON((743238 2967416,743238 2967450,743265 2967450,743265.625 2967416,743238 2967416))").SetSrid(2249);
                        var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var res = ses.Query<T>()
                            .SelectSql<string>("(ST_AsLatLonText('POINT (-3.2342342 -2.32498)', 'D°M''S.SSS\"C'))")
                            .ToList();
                        Execute.Log(1, res.Count == 3, "selectSql");
                        res = ses.Query<T>()
                            .SelectSql<string>($"(ST_AsLatLonText('POINT (-3.2342342 -2.32498)',{ses.SymbolParam}k1 ))",
                                new SqlParam($"{ses.SymbolParam}k1", "D°M''S.SSS\"C"))
                            .ToList();
                        Execute.Log(2, res.Count == 3, "selectSql par");
                    }


                }

                #endregion

                #region StReverse

                {
                    Console.WriteLine("***** StReverse");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("LINESTRING(0 0, 1 1, 2 2)");
                        var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StReverse();
                        var sasr = ses.Query<T>().ToList();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StReverse()).ToList();

                    }

                }


                #endregion

                

                #region StSetSRID

                {
                    Console.WriteLine("***** StSetSRID");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POLYGON((-71.1776848522251 42.3902896512902,-71.1776843766326 42.3903829478009,-71.1775844305465 42.3903826677917,-71.1775825927231 42.3902893647987,-71.177684\r\n8522251 42.3902896512902))").SetSrid(4326);
                        var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        // ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        // ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StSetSRID(2249);
                        var sasr = ses.Query<T>().ToList();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StSetSRID(2249)).ToList();
                        sasr = ses.Query<T>().ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StSetSRID(2249).StAsText()).ToList();
                    }

                }

                #endregion

                #region StTransform

                {
                    Console.WriteLine("***** StTransform");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POLYGON((743238 2967416,743238 2967450,743265 2967450,743265.625 2967416,743238 2967416))").SetSrid(2249);
                        var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StTransform(4326);
                        var sasr = ses.Query<T>().ToList();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StTransform(4326)).ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StTransform(4326).StAsText()).ToList();
                    }

                }

                #endregion

                #region StTranslate

                {
                    Console.WriteLine("***** StTranslate");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POINT(-71.01 42.37)");
                        var geo2 = FactoryGeo.CreateGeo("LINESTRING(-71.01 42.37,-71.11 42.38)");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });

                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StTranslate(1.4f, 0.8f);
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StTranslate(1, 0)).ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StTranslate(1, 0).StAsText()).ToList();
                    }

                }

                #endregion

                #region StY

                {
                    Console.WriteLine("***** StY");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POINT(2.56 4.4545)");
                    var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    //ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    //ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StY();
                    var sasr = ses.Query<T>().ToList();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StY()).ToList();
                    //var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StY() > 0).ToList();
                }

                #endregion

                #region StX

                {
                    Console.WriteLine("***** StX");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POINT(2.56 4.4545)");
                    var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    //ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    //ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StX();
                    var sasr = ses.Query<T>().ToList();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StX()).ToList();
                    //var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StX()>0).ToList();
                }

                #endregion

                #region StInteriorRingN

                {
                    Console.WriteLine("***** StInteriorRingN");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0), (1 1, 1 2, 2 2, 2 1, 1 1))");
                    var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StInteriorRingN(1);
                    var sasr = ses.Query<T>().ToList();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StInteriorRingN(1)).ToList();
                    var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StInteriorRingN(1).StAsText()).ToList();
                }

                #endregion

                #region StPointOnSurface

                {
                    Console.WriteLine("***** StPointOnSurface");
                    if (providerName == ProviderName.PostgreSql || providerName == ProviderName.MsSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0), (1 1, 1 2, 2 2, 2 1, 1 1))");
                        var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StPointOnSurface();
                        var sasr = ses.Query<T>().ToList();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StPointOnSurface()).ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StPointOnSurface().StAsText()).ToList();
                    }

                }

                #endregion

                #region StPointN

                {
                    Console.WriteLine("***** StPointN");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(0 0, 1 1, 2 2)");
                    var geo2 = FactoryGeo.CreateGeo($"{GeoType.GeometryCollection} EMPTY");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StPointN(2);
                    var sasr = ses.Query<T>().ToList();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StPointN(2)).ToList();
                    var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StPointN(2).StAsText()).ToList();
                }

                #endregion

                #region StConvexHull

                {
                    Console.WriteLine("***** StConvexHull");
                    if (providerName == ProviderName.PostgreSql || providerName == ProviderName.MsSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("MULTILINESTRING((10 19,10 8),(15 10, 20 30))");
                        var geo2 = FactoryGeo.CreateGeo("MULTILINESTRING((10 19,10 8),(15 10, 20 30))");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });

                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StConvexHull();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StConvexHull()).ToList();
                        var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StConvexHull().StAsText()).ToList();
                    }

                }

                #endregion

                #region StPerimeter

                {
                    Console.WriteLine("***** StPerimeter");
                    if (providerName == ProviderName.PostgreSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POLYGON((743238 2967416,743238 2967450,743265 2967450,\r\n743265.625 2967416,743238 2967416))").SetSrid(2249);
                        var geo2 = FactoryGeo.CreateGeo("MULTIPOLYGON(((763104.471273676 2949418.44119003," +
                                                        "763104.477769673 2949418.42538203," +
                                                        "763104.189609677 2949418.22343004,763104.471273676 2949418.44119003))," +
                                                        "((763104.471273676 2949418.44119003,763095.804579742 2949436.33850239," +
                                                        "763086.132105649 2949451.46730207,763078.452329651 2949462.11549407," +
                                                        "763075.354136904 2949466.17407812,763064.362142565 2949477.64291974," +
                                                        "763059.953961626 2949481.28983009,762994.637609571 2949532.04103014," +
                                                        "762990.568508415 2949535.06640477,762986.710889563 2949539.61421415," +
                                                        "763117.237897679 2949709.50493431,763235.236617789 2949617.95619822," +
                                                        "763287.718121842 2949562.20592617,763111.553321674 2949423.91664605,\r\n763104.471273676 2949418.44119003)))").SetSrid(2249);
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });

                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StPerimeter();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StPerimeter()).ToList();
                    }

                }

                #endregion

                #region StUnion

                {
                    Console.WriteLine("***** StUnion");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = FactoryGeo.CreateGeo("point(1 4)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });

                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StUnion(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StUnion(geo2)).ToList();
                    var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StUnion(geo2).StAsText()).ToList();
                }

                #endregion

                #region StDifference

                {
                    Console.WriteLine("***** StDifference");

                    if (providerName == ProviderName.MySql || providerName == ProviderName.MsSql)
                    {
                        ses.TruncateTable<T>();
                        var p = FactoryGeo.Polygon("Point(1 1)");
                        var geo = FactoryGeo.CreateGeo("Point(2 2)");
                        var ee = geo.SetSession(ses).StDifference(p);
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var sas = ses.Query<T>().Select(a =>
                            a.MyGeoObject.StDifference(p).StAsText()).ToList();
                    }
                    else
                    {
                        ses.TruncateTable<T>();
                        var p = FactoryGeo.Polygon("LINESTRING(50 100, 50 200)");
                        var geo = FactoryGeo.CreateGeo("LINESTRING(50 50, 50 150)");
                        var ee = geo.SetSession(ses).StDifference(p);
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var sas = ses.Query<T>().Select(a =>
                            a.MyGeoObject.StDifference(p).StAsText()).ToList();
                    }





                }


                #endregion

                #region StDimension

                Console.WriteLine("***** StDimension");
                {
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StDimension();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StDimension()).ToList();
                }

                #endregion

                #region StCentroid

                {
                    Console.WriteLine("***** StCentroid");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("MULTIPOINT ( -1 0, -1 2, -1 3, -1 4, -1 7, 0 1, 0 3, 1 1, 2 0, 6 0, 7 8, 9 8, 10 6 )").SetSrid(0);
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)").SetSrid(0);
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StCentroid();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StCentroid()).ToList();
                    if (providerName != ProviderName.MySql)
                    {
                        var saws = ses.Query<T>().Select(a => new
                        {
                            s = a.MyGeoObject.StCentroid().StAsText(),
                            b = a.MyGeoObject.StSrid()
                        }).ToList();
                    }

                }

                #endregion

                #region StStartPoint

                {
                    Console.WriteLine("***** StStartPoint");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StStartPoint();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StStartPoint().StAsText()).ToList();


                }

                #endregion

                #region StBoundary

                {
                    Console.WriteLine("***** StBoundary");
                    if (providerName != ProviderName.MySql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("LINESTRING(90 76,50 60, 70 80, 77 90)");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StAsText().Substring(0, 7)).ToList();
                        var res = ses.Query<T>().Select(a => a.MyGeoObject.StBoundary()).ToList();
                    }


                }


                #endregion

                #region StBuffer

                {
                    Console.WriteLine("***** StBuffer");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = FactoryGeo.CreateGeo("point(1 4)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });

                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StBuffer(12);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StBuffer(12)).ToList();
                    var sas2 = ses.Query<T>().Select(a => a.MyGeoObject.StBuffer(12).StAsText()).ToList();
                }

                #endregion

                #region StSymDifference

                {
                    Console.WriteLine("***** StSymDifference");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("point(1 2)");
                    var geo2 = FactoryGeo.CreateGeo("point(1 4)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });

                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StSymDifference(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StSymDifference(geo2)).ToList();
                }

                #endregion

                #region StEnvelope

                {
                    Console.WriteLine("***** StEnvelope");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("point(1 2)").SetSrid(0);
                    var geo2 = FactoryGeo.CreateGeo("point(1 4)").SetSrid(0);
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StEnvelope();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StEnvelope()).ToList();
                }

                #endregion

                #region StEndPoint

                {
                    Console.WriteLine("***** StEndPoint");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StEndPoint();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StEndPoint()).ToList();
                }

                #endregion

                #region StAsBinary

                {
                    Console.WriteLine("***** StAsBinary");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StAsBinary();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StAsBinary()).ToList();

                }


                #endregion

                #region StNumPoints

                {
                    Console.WriteLine("***** StNumPoints");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31,77.29 29.07)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(77.29 29.07,77.42 29.26,77.27 29.31)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StNumPoints();
                    var sas = ses.Query<T>().Where(a => a.MyGeoObject.StNumPoints() == 4).ToList();

                }

                #endregion

                #region StIsClosed

                {
                    Console.WriteLine("***** StIsClosed");
                    ses.TruncateTable<T>();
                    if (providerName == ProviderName.PostgreSql || providerName == ProviderName.MsSql)
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                        var geo2 = FactoryGeo.CreateGeo("POINT(0 0)");
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StIsClosed();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StIsClosed()).ToList();
                    }
                    else
                    {
                        ses.TruncateTable<T>();
                        var geo = FactoryGeo.CreateGeo("LineString(1 1,2 2,3 3,1 1)").SetSrid(0);
                        var geo2 = FactoryGeo.CreateGeo("LineString(1 1,2 2,3 3,1 1)").SetSrid(0);
                        ses.Insert(new T { Name = "p", MyGeoObject = geo });
                        ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                        ses.Insert(new T { Name = "p", MyGeoObject = null });
                        var o = geo.SetSession(ses).StIsClosed();
                        var sas = ses.Query<T>().Select(a => a.MyGeoObject.StIsClosed()).ToList();
                    }

                }

                #endregion

                #region StLength

                {
                    Console.WriteLine("***** StLength");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.1240 42.45666, -72.123 42.1546)").SetSrid(26986);

                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StLength();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StLength()).ToList();
                }

                #endregion

                #region StIsValid

                {
                    Console.WriteLine("***** StIsValid");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("LINESTRING(0 0, 1 1)");
                    var geo2 = FactoryGeo.CreateGeo("POLYGON((0 0, 1 1, 1 2, 1 1, 0 0))");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = geo2 });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StIsValid();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StIsValid()).ToList();
                }

                #endregion

                #region StIsSimple

                {
                    Console.WriteLine("***** StIsSimple");
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0), (1 1, 1 2, 2 2, 2 1, 1 1))");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StIsSimple();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StIsSimple()).ToList();

                }

                #endregion

                #region StNumInteriorRing

                {
                    Console.WriteLine("***** StNumInteriorRing");
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0), (1 1, 1 2, 2 2, 2 1, 1 1))");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StNumInteriorRing();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StNumInteriorRing()).ToList();
                }

                #endregion

                #region StArea
                Console.WriteLine("***** StArea");
                {
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POLYGON((743238 2967416,743238 2967450,743265 2967450,743265.625 2967416,743238 2967416))").SetSrid(2249);
                    var res = geo.SetSession(ses).SetSession(ses).StArea();//928.625
                    Execute.Log(6, res == 928.625d, "StArea");
                }
                #endregion

                #region StWithin
                Console.WriteLine("***** StWithin");
                {
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POLYGON((1 1, 1 2, 2 2, 2 1, 1 1))");
                    var res = geo.SetSession(ses).StWithin(FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0))"));

                }

                #endregion

                #region StContains
                Console.WriteLine("***** StContains");
                {
                    ses.TruncateTable<T>();
                    var geo = FactoryGeo.CreateGeo("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0))");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });


                    var res = geo.SetSession(ses).StContains(FactoryGeo.Point(1, 1));
                    Execute.Log(6, res == true, "StContains");
                    res = geo.SetSession(ses).StContains(FactoryGeo.Point(-10, -10));
                    Execute.Log(7, res == false, "StContains");
                    var o = ses.Query<T>().Where(a => a.MyGeoObject.StContains(FactoryGeo.Point(1, 1)) == true)
                        .ToList();
                    Execute.Log(8, o.Count == 1, "StContains");
                    o = ses.Query<T>().Where(a => a.MyGeoObject.StContains(FactoryGeo.Point(-1, -1)) == true)
                        .ToList();
                    Execute.Log(9, o.Count == 0, "StContains");

                }



                #endregion

                #region StNumGeometries
                Console.WriteLine("***** StNumGeometries");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("GEOMETRYCOLLECTION(MULTIPOINT((-2 3),(-2 2)),LINESTRING(5 5 ,10 10),POLYGON((-7 4.2,-7.1 5,-7.1 4.3,-7 4.2)))");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StNumGeometries();
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StNumGeometries()).ToList();
                }

                #endregion

                #region StTouches
                Console.WriteLine("***** StTouches");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StTouches(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StTouches(geo2)).ToList();
                }

                #endregion

                #region StOverlaps
                Console.WriteLine("***** StOverlaps");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StOverlaps(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StOverlaps(geo2)).ToList();
                }

                #endregion

                #region StEquals
                Console.WriteLine("***** StEquals");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StEquals(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StEquals(geo2)).ToList();
                }

                #endregion

                #region StCrosses
                Console.WriteLine("***** StCrosses");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StCrosses(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StCrosses(geo2)).ToList();
                }

                #endregion

                #region StDisjoint
                Console.WriteLine("***** StDisjoint");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(0 0)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING ( 2 0, 0 2 )");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StDisjoint(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StDisjoint(geo2)).ToList();
                }


                #endregion

                #region StIntersects
                Console.WriteLine("***** StIntersects");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("LINESTRING(-43.23456 72.4567,-43.23456 72.4568)");
                    var geo2 = FactoryGeo.CreateGeo("POINT(-43.23456 72.4567772)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StIntersects(geo2);
                    //var sas = ses.Query<T>().Select(a => a.MyGeoObject.StIntersects(geo2).ToList();
                }


                #endregion

                #region StDistance
                Console.WriteLine("***** StDistance");
                {
                    ses.TruncateTable<T>();


                    var geo = FactoryGeo.CreateGeo("POINT(-72.1235 42.3521)");
                    var geo2 = FactoryGeo.CreateGeo("LINESTRING(-72.1260 42.45, -72.123 42.1546)");
                    ses.Insert(new T { Name = "p", MyGeoObject = geo });
                    ses.Insert(new T { Name = "p", MyGeoObject = null });
                    var o = geo.SetSession(ses).StDistance(geo2);
                    var sas = ses.Query<T>().Select(a => a.MyGeoObject.StDistance(geo2)).ToList();

                }



                #endregion



            }
        }
        public static void TestJsonIon100<T, TD>(ProviderName providerName) where TD : IOtherDataBaseFactory, new() where T : JsonBaseO, new()
        {
            Console.WriteLine($"************************Test json as Ion100 {providerName}*************************");
            using (var ses = Configure.GetSession<TD>())
            {
                ses.DropTableIfExists<T>();
                ses.TableCreate<T>();
                T jsonPos = new T
                {
                    Name = "1'",
                    Ion100 = new Ion100 { Age = 10, Name = "10" }
                };
                ses.Insert(jsonPos);
                T jsonPos2 = new T
                {
                    Name = "2222'",
                    Ion100 = new Ion100 { Age = 10, Name = "10" }
                };
                ses.InsertBulk(new List<T> { jsonPos2 });

                var list = ses.Query<T>().ToList();
                var pos = list.First();
                pos.Name = "123";
                pos.Ion100 = new Ion100 { Age = 100, Name = "100" };
                ses.Update(pos);
                pos.Ion100 = new Ion100 { Age = 10, Name = "10" };
                ses.Update(pos);

                var t = new Ion100 { Age = 120, Name = "120" };
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, JsonConvert.SerializeObject(t) }
                });
                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, JsonConvert.SerializeObject(new{bb=1}) }
                });

                ses.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, new Ion100() }
                });
                var s = ses.Query<T>().Select(a => a.Ion100).ToList();
                var sn = ses.Query<T>().Select(a => new { n = a.Name, d = a.Ion100 }).ToList();
                list = ses.Query<T>().ToList();

                var sql = $"select {ses.StarSql<T>()} from {ses.TableName<T>()}";
                list = ses.FreeSql<T>(sql).ToList();
                ses.TruncateTable<T>();
                ses.Insert(new T() { Name = "2", Ion100 = new Ion100 { Age = 10, Name = "10" } });
                ses.Insert(new T() { Name = "2", Ion100 = new Ion100 { Age = 100, Name = "100" } });
                ses.Insert(new T() { Name = "2", Ion100 = new Ion100 { Age = 200, Name = "200" } });
                list = ses.FreeSql<T>(sql).ToList();
                if (providerName == ProviderName.MsSql)
                {
                    var o = ses.Query<T>().WhereSql("JSON_VALUE(ion100, '$.Age') = 100").Select(a => a.Ion100).ToList();
                }
                if (providerName == ProviderName.PostgreSql)
                {
                    var o = ses.Query<T>().WhereSql("ion100 @> '{\"Age\":100}'").Select(a => a.Ion100).ToList();
                }
            }
        }
    }
    [MapTable("mygeo")] public class GeoPos : GeoBase { }
    [MapTable("mygeo")] public class GeoMy : GeoBase { }
    [MapTable("mygeo")] public class GeoMs : GeoBase { }

    [MapTable("mupdate")] public class UpPost : BaseUpdate { }
    [MapTable("mupdate")] public class UpMys : BaseUpdate { }
    [MapTable("mupdate")] public class UpMs : BaseUpdate { }
    [MapTable("mupdate")] public class UpSql : BaseUpdate { }

    public class BaseUpdate
    {
        [MapPrimaryKey("id", Generator.Assigned)] public Guid Id { get; set; } = Guid.NewGuid();

        [MapColumn("my_geo")]
        public IGeoShape Geo { get; set; } = FactoryGeo.Empty(GeoType.GeometryCollection);

        [MapColumn("json")]
        [MapColumnTypeJson]
        public object Ion100 { get; set; } = new Ion100();

        [MapColumn("name")] public string Name { get; set; }

        [MapColumn("guid")] public Guid Guid { get; set; }

        [MapColumn("dat")] public DateTime Date { get; set; } = DateTime.Now;
        [MapColumn("det")] public decimal? decimal3 { get; set; }
    }
    public class GeoBase
    {
        [MapPrimaryKey("id", Generator.Assigned)] public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumn("name")] public string Name { get; set; }

        [MapColumn("my_geo")]
        //[MapDefaultValue(" NOT NULL ")]
        public IGeoShape MyGeoObject { get; set; } = FactoryGeo.Empty(GeoType.GeometryCollection);
        [MapColumn("my_geo2")] public IGeoShape MyGeoObject2 { get; set; } = FactoryGeo.Empty(GeoType.GeometryCollection);
    }
    [MapTable("myjson")] public class JPosO : JsonBaseO { }
    [MapTable("myjson")] public class JMyO : JsonBaseO { }
    [MapTable("myjson")] public class JMsO : JsonBaseO { }
    [MapTable("myjson")] public class JSqliteO : JsonBaseO { }

    [MapTable("myjson")] public class JPosS : JsonBaseS { }
    [MapTable("myjson")] public class JMyS : JsonBaseS { }
    [MapTable("myjson")] public class JMsS : JsonBaseS { }
    [MapTable("myjson")] public class JSqliteS : JsonBaseS { }
    public class JsonBaseO
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
        //[MapIndex]
        [MapColumn("ion100")]
        [MapColumnTypeJson()]
        public Ion100 Ion100 { get; set; }

        [MapColumn]
        public string Name { get; set; } = "name";
    }

    public class JsonBaseS
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
        //[MapIndex]
        [MapColumn("ion100")]
        [MapColumnTypeJson(TypeReturning.AsString)]
        public object Ion100 { get; set; }

        [MapColumn]
        public string Name { get; set; } = "name";
    }

    public class Ion100
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Execute
    {
        private static void Running(int i)
        {
            while (true)
            {
                using (var ses = Configure.Session)
                {
                    var ts = ses.BeginTransaction();
                    MyClass c = new MyClass(1);
                    ses.Insert(c);
                    var s = Configure.Session.Query<MyClass>().Count();
                    Console.WriteLine($"{i} -- " + s);
                    ts.Commit();
                }
            }
        }

        private static void InnerRunOtherSession(int i)
        {

            while (true)
            {
                using (var ses = Configure.GetSession<MyDbPostgres>())
                {
                    using (var t = ses.BeginTransaction())
                    {
                        var ee = ses.Query<MyClass>().JoinCore(ses.Query<MyClass>(), a => a.Age, b => b.Age,
                      (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();
                        var l = ses.Query<MyClass>().ToList();
                        var e1e = ses.Query<MyClass>().JoinCore(l, a => a.Age, b => b.Age,
                            (aa, bb) => new { name1 = aa.Name, name2 = bb.Name });

                    }
                    Console.WriteLine($"{i}--");
                }
            }
        }

        public static void RunOtherSession()
        {
            Task.Run(() => { InnerRunOtherSession(1); });
            Task.Run(() => { InnerRunOtherSession(2); });
            Task.Run(() => { InnerRunOtherSession(4); });
            Task.Run(() => { InnerRunOtherSession(5); });
            Task.Run(() => { InnerRunOtherSession(6); });
            Task.Run(() => { InnerRunOtherSession(7); });
            Task.Factory.StartNew(() => { InnerRunOtherSession(3); }, TaskCreationOptions.LongRunning);
        }

        public static void RunThread()
        {
            Task.Factory.StartNew(() => { Running(1); }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(2); }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(3); }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(4); }, TaskCreationOptions.LongRunning);
        }

        public static void TotalTest()
        {
            NewExe<MyClassPostgres, MyDbPostgres>();
            NewExe<MyClassMysql, MyDbMySql>();
            NewExe<MyClassMsSql, MyDbMsSql>();
            NewExe<MyClassSqlite, MyDbSqlite>();
        }

        public static void TotalTestNull()
        {
            NewExeNull<ClassNullPostgres, MyDbPostgres>();
            NewExeNull<ClassNullMysql, MyDbMySql>();
            NewExeNull<ClassNullMsSql, MyDbMsSql>();
            NewExeNull<ClassNullSqlite, MyDbSqlite>();
        }

        private static void NewExe<T, Tb>() where T : MyClassBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<Tb>();


            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();

            var dt = DateTime.Now;
            var myClass = new T
            {
                DateTime = dt,
                Age = 12,
                Description = "simple",
                Name = "name",
                MyEnum = MyEnum.First
            };
            session.Insert(myClass);
            List<T> res = null;
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Year == dt.Year).ToList();
            Log(1, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Log(2, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Log(3, res.Count == 1);


            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Log(4, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Log(5, res.Count == 1, "minutes may not match");

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Log(6, res.Count == 1, "seconds may not match");

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Log(7, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Log(8, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.AddYears(1).Year)
                .ToList();
            Log(9, res.Count == 1);
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.AddMonths(1).Month)
                .ToList();
            Log(10, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.AddHours(1).Hour)
                .ToList();
            Log(11, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.AddDays(1).Day)
                .ToList();
            Log(12, res.Count == 1);
            res = session.Query<T>()
                .Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.AddMinutes(1).Minute)
                .ToList();
            Log(13, res.Count == 1, "minutes may not match");

            res = session.Query<T>()
                .Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.AddSeconds(1).Second)
                .ToList();
            Log(14, res.Count == 1, "seconds may not match");


            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa")
                .ToList();
            Log(15, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Log(16, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Log(17, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Log(18, res.Count == 1);


            if (s.GetProviderName() == ProviderName.MsSql)
            {
                T my1 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Update(my1);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Log(19, res.Count == 1);

                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Log(20, res.Count == 1);

                var sss = session.Query<T>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Log(21, res.Count == 1);
            }
            else
            {
                T my2 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                var ss = session.Update(my2);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Log(22, res.Count == 1);

                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Log(20, res.Count == 1);


                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Log(21, res.Count == 1);
            }

            T my3 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Update(my3);
            var err = session.Query<T>().Select(a => new { sd = a.Name.Length }).ToList();
            res = session.Query<T>().Where(a => a.Name.Length == "name".Length).ToList();
            Log(22, res.Count == 1);

            res = session.Query<T>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Log(23, res.Count == 1);

            res = session.Query<T>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Log(241, res.Count == 1);


            List<T> list = new List<T>
            {
                new T { Name = "MyName1", Age = 10 },
                new T { Name = "MyName2", Age = 20 },
                new T { Name = "MyName3", Age = 30 }
            };
            var i = session.InsertBulk(list);
            Log(242, i == 3, "/1 InsertBulk");


            var count = session.Query<T>().Count();
            Log(25, count == 4);


            var o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault();
            Log(26, o != null && o.Age == 10);

            Console.WriteLine("Test transaction");
            session.TruncateTable<T>();
            count = session.Query<T>().Count();
            Log(27, count == 0);


            IsolationLevel? level = null;
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Insert(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                }
                finally
                {
                    ses.Dispose();
                }

                count = session.Query<T>().Count();
                Log(28, count == 3);
            }
            session.TruncateTable<T>();
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Insert(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Commit();
                    ses.Insert(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                }
                finally
                {
                    ses.Dispose();
                }

                count = session.Query<T>().Count();
                Log(28, count == 6);

                session.TruncateTable<T>();
                session.Insert(new T
                {
                    Name = "name",
                    Age = 12
                });
                res = session.Query<T>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                Log(30, res.Count == 1);

                res = session.Query<T>().Where(a => string.IsNullOrEmpty(a.Description)).ToList();
                Log(31, res.Count == 1);

                o = session.Query<T>().Where(a => a.Age == 12).Single();
                Log(32, o != null);

                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).Single();
                    Log(32, false);
                }
                catch (Exception e)
                {
                    Log(32, true);
                }

                o = session.Query<T>().Where(a => a.Age == 14).SingleOrDefault();
                Log(33, o == null);

                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).First();
                    Log(34, false);
                }
                catch (Exception e)
                {
                    Log(34, true);
                }

                o = session.Query<T>().Where(a => a.Age == 14).FirstOrDefault();
                Log(34, o == null);

                session.TruncateTable<T>();
                count = session.InsertBulk(new List<T>
                {
                    new T { Age = 40, Name = "name" },
                    new T { Age = 20, Name = "name1" },
                    new T { Age = 30, Name = "name1" },
                    new T { Age = 50, Name = "name1" },
                    new T { Age = 60, Name = "name" },
                    new T { Age = 10, Name = "name" }
                });
                var ob = session.Query<T>().Select(a => new { ass = a.Age, asss = string.Concat(a.Name, a.Age) })
                    .ToList();
                Log(35, ob.Count() == 6);

                count = session.Query<T>().Where(a => a.Name == "name").OrderBy(r => r.Age).ToList().Sum(a => a.Age);
                Log(36, count == 110);



                o = session.Query<T>().OrderBy(a => a.Age).First();
                Log(38, o.Age == 10);

                o = session.Query<T>().OrderByDescending(a => a.Age).First();
                Log(39, o.Age == 60);

                count = session.Query<T>().Where(a => a.Age < 100).OrderBy(ds => ds.Age).ToListAsync().Result
                    .Sum(a => a.Age);
                Log(40, count == 210);

                var sCore = session.Query<T>().Where(a => a.Name.Contains("1")).Distinct(a => a.Name);
                Log(41, sCore.Count() == 1);

                count = session.Query<T>().Where(sw => sw.Age == 10).Update(d => new Dictionary<object, object>
                {
                    { d.Name, string.Concat(d.Name, d.Age) },
                   // { d.DateTime, DateTime.Now }
                });

                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Log(42, res.Count() == 1);

                session.Query<T>().Delete(a => a.Name == "name10");
                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Log(43, res.Count() == 0);

                session.Query<T>().Where(a => a.Age == 10).Delete();
                count = session.Query<T>().Where(a => a.Age == 10).Count();
                Log(44, count == 0);

                if (s.GetProviderName() == ProviderName.MySql)
                {
                    res = session.FreeSql<T>(
                            $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = ?1",
                            40)
                        .ToList();
                    Log(45, res.Count() == 1);
                }
                else
                {
                    res = session.FreeSql<T>(
                            $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = @1",
                            40)
                        .ToList();
                    Log(45, res.Count() == 1);
                }


                var anon1 = session.Query<T>().Where(a => a.Age == 40).Select(d => new { age = d.Age, name = d.Name })
                    .ToList();
                Log(46, anon1.Count() == 1);


                dynamic di = session.FreeSql<dynamic>($"select age, name from {session.TableName<T>()}");
                Log(47, di.Count == 5);

                if (s.GetProviderName() == ProviderName.SqLite)
                {
                    var anon = TempSql(new { age = 3L, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Log(48, anon.Count() == 5);
                }
                else
                {
                    var anon = TempSql(new { age = 3, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Log(48, anon.Count() == 5);
                }

                var tempFree = session.FreeSql<MyFreeSql>($"select {session.ColumnName<T>(a => a.Id)}," +
                                                          $"name,age,enum from {session.TableName<T>()}");

                Log(49, tempFree.Count() == 5);





                session.Query<T>().Where(a => a.Age == 20).Update(f => new Dictionary<object, object>
                {
                    { f.Age, 400 }
                });

                var ano = session.Query<T>().Where(a => a.Age < 500).Select(f =>
                    new { e = f.MyEnum, c = f.DateTime }).ToList();
                Log(54, ano.Count() == 5);
                var ano1 = session.Query<T>().Distinct(a => a.Age);
                Log(55, ano1.Count() == 5);
                var ano2 = session.Query<T>().Distinct(a => new { ago = a.Age, date = a.DateTime });
                Log(56, ano2.Count() == 5);


                List<T> list22 = new List<T>();
                for (int iz = 0; iz < 2; iz++)
                {
                    list22.Add(new T { Age = 30, Name = "name1" });
                    list22.Add(new T { Age = 10, Name = "name2" });
                }


                var guid = new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e");
                var data = new DateTime(2023, 3, 4);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Age, new int() }
                });
                session.Query<T>().Delete(a => a.Age == new int());
                res = session.Query<T>().ToListAsync().Result;
                Log(57, res.Count() == 0);

                Console.WriteLine($"{Environment.NewLine}/*--------------mytest-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);


                Console.WriteLine($"{Environment.NewLine} /*--------------guid-------------*/{Environment.NewLine}");

                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.ValGuid, guid }
                });
                session.Query<T>().Delete(a => a.ValGuid == guid);
                res = session.Query<T>().ToListAsync().Result;
                Log(60, res.Count() == 0);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.ValGuid, new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e") }
                });
                session.Query<T>().Delete(a => a.ValGuid == new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e"));
                res = session.Query<T>().ToListAsync().Result;
                Log(61, res.Count() == 0);

                Console.WriteLine($"{Environment.NewLine}/*--------------date-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.DateTime, data }
                });
                session.Query<T>().Delete(a => a.DateTime == data);
                res = session.Query<T>().ToListAsync().Result;
                Log(62, res.Count() == 0);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.DateTime, new DateTime(2023, 3, 4) }
                });
                session.Query<T>().Delete(a => a.DateTime == new DateTime(2023, 3, 4));
                res = session.Query<T>().ToListAsync().Result;
                Log(63, res.Count() == 0);
                session.TruncateTable<T>();
                list22.Clear();
                for (int j = 0; j < 10; j++)
                {
                    list22.Add(new T { Age = j * 10, Name = "name1" });
                }

                session.InsertBulk(list22);
                var rBases = session.Query<T>().OrderBy(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderBy(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Log(64, count == 50);

                rBases = session.Query<T>().OrderByDescending(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Log(65, count == 130);

                Console.WriteLine(
                    $"{Environment.NewLine}/*-------------------------test serialize-----------------------------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                list22.Clear();
                list22.Add(new T { });
                session.InsertBulk(list22);
                o = session.Query<T>().First();
                var b = true;
                Log(66, b, "test serialize");
                o.TestUser = "A";
                session.Update(o);
                o = session.Query<T>().First();
                Log(67, o.TestUser == "A");
                session.TruncateTable<T>();
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                session.InsertBulk(list22);
                var intVals = session.Query<T>().Where(a => a.ValInt4 == 20).Select(f => f.ValInt4).ToListAsync()
                    .Result;
                Log(68, intVals.Count == 4, "test select");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                    session.Insert(new T { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now });
                }

                count = session.Query<T>().Distinct(a => a.Age).ToList().Sum();
                Log(69, count == 450, "test distinct");
                var r1 = session.Query<T>().Distinct(a => new { age = a.Age, mane = a.Name });
                Log(70, r1.Count() == 10, "test distinct");

                Console.WriteLine(
                    $"{Environment.NewLine}/*----------------------------params------------------------------*/{Environment.NewLine}");
                string sql = "";
                if (s.GetProviderName() == ProviderName.MySql)
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=?assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =?assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = ?age";
                    count = (int)session.ExecuteScalar(sql, "name1", 123, 10);
                    Log(72, count == 10, "test params");
                }
                else if (s.GetProviderName() == ProviderName.MsSql)
                {
                    var ssp = session.GetCommand().CreateParameter();
                    ssp.Precision = 18;
                    ssp.Scale = 2;
                    ssp.ParameterName = "@2";
                    ssp.DbType = DbType.Decimal;
                    ssp.Value = 123.3;
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=@1 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} = @2 and" +
                          $" {session.ColumnName<T>(d => d.Age)} = @3";
                    //count = (int)session.ExecuteScalar("select [age] from [my_class5] where  [name]=@1 and [test5] = '123.3' and [age] = '10'", "name1");
                    count = (int)session.ExecuteScalar(sql, "name1", new decimal(123), 10);
                    Log(72, count == 10, "test params");
                }
                else
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=@assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =@assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = @age";
                    var ssT = session.ExecuteScalar(sql, "name1", 123, 10);
                    Log(72, ssT.ToString() == "10", "test params");
                }

                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                }

                res = session.Query<T>().OrderBy(a => a.Age).Limit(0, 1).ToList();
                Log(73, res.Count == 1 && res.First().Age == 0, "test limit");
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(0, 1).ToList();
                Log(74, res.Count == 1 && res.First().Age == 9);


                Console.WriteLine(
                    $"{Environment.NewLine}/*----------test ElementAtOrDefault-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(0);
                Log(75, o.Age == 0, "test ElementAtOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(9);
                Log(76, o.Age == 9);

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(10);
                Log(77, o == null);

                Console.WriteLine(
                    $"{Environment.NewLine}/*----------test ElementAt-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(0);
                Log(78, o.Age == 0, "test ElementAt");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(9);
                Log(79, o.Age == 9);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).ElementAt(10);
                    Log(80, 1 != 1);
                }
                catch (Exception)
                {
                    Log(80, 1 == 1);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}/*-----------test FirstOrDefault------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == 0);
                Log(81, o.Age == 0, "test FirstOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == -10);
                Log(82, o == null);

                Console.WriteLine($"{Environment.NewLine} /*------------test first-----------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == 0);
                Log(83, o.Age == 0);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == -10);
                    Log(84, false);
                }
                catch (Exception)
                {
                    Log(84, true);
                }

                Console.WriteLine($"{Environment.NewLine} /*-----------test last------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == 0);
                Log(85, o.Age == 0);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == -10);
                    Log(86, false);
                }
                catch (Exception)
                {
                    Log(86, true);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}*------------test LastOrDefault-----------*{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == 0);
                Log(87, o.Age == 0);

                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == -10);
                Log(88, o == null);
                Console.WriteLine(
                    $"{Environment.NewLine} /*------------test test SingleOrDefault-----------*/{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == 0);
                Log(87, o.Age == 0);

                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == -10);
                Log(88, o == null);
                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault();
                    Log(89, false);
                }
                catch (Exception)
                {
                    Log(89, true);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}/*------------test test Single-----------*/{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).Single(d => d.Age == 0);
                Log(90, o.Age == 0);


                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Single(a => a.Age == -10);
                    Log(91, false);
                }
                catch (Exception)
                {
                    Log(91, true);
                }

                Console.WriteLine($"{Environment.NewLine}/*------------test Any-----------*/{Environment.NewLine}");
                bool bll = session.Query<T>().Where(a => a.Age == 1).Any();
                Log(92, bll);
                bll = session.Query<T>().Any(a => a.Age == 1);
                Log(93, bll);
                bll = session.Query<T>().Any(a => a.Age == -1);
                Log(94, !bll);

                Console.WriteLine($"{Environment.NewLine}/*------------test All-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10, Name = "name1" });
                }

                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 20, Name = "name1" });
                }

                bll = session.Query<T>().Where(d => d.Name.StartsWith("name")).All(a => a.Age == 10);
                Log(94, bll == false);
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10, Name = "name1" });
                }

                bll = session.Query<T>().Where(d => d.Name.StartsWith("name")).All(a => a.Age == 10);
                Log(94, bll == true);
                bll = session.Query<T>().Where(d => d.Name.Contains("name")).All(a => a.Age == 10);
                Log(95, bll == true);
                bll = session.Query<T>().Where(d => d.Name.EndsWith("e1")).All(a => a.Age == 11);
                Log(96, bll != true);

                Console.WriteLine($"{Environment.NewLine}/*------------test skip-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = j, Name = "name1" });
                }

                count = session.Query<T>().Count();

                res = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Skip(2).ToListAsync().Result;
                Log(97, res.Count == 8 && res.First().Age == 2);
                Console.WriteLine($"{Environment.NewLine}/*------------test select-----------*/{Environment.NewLine}");
                var listInt = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d, ir) => ir)
                    .ToList();
                Log(98, listInt.Count == 10);
                i = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d, index) => index).ToList()
                    .Sum();
                Log(98, i == 55);
                var listOb = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age)
                    .Select((d, index) => new { Age = d.Age, Name = string.Concat(index, "-", d.Name) }).ToListAsync()
                    .Result;
                Log(98, listOb.Count == 10);
                session.TruncateTable<T>();
                session.Insert(new T());
                session.Insert(new T());
                session.Insert(new T());
                count = session.Query<T>().Count();
                Log(99, count == 3);
                session.Query<T>().ForEach(a => session.Delete(a));
                count = session.Query<T>().Count();
                Log(100, count == 0);

            }
        }

        public static IEnumerable<Ts> TempSql<Ts>(Ts t, ISession session, string sql)
        {
            return session.FreeSql<Ts>(sql);
        }

        private static void NewExeNull<T, Tb>() where T : MyClassNullBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();

            session.Insert(new T());
            var tttest = session.Query<T>().First();

            if (s.GetProviderName() == ProviderName.PostgreSql || s.GetProviderName() == ProviderName.MsSql)
            {
                var res = (tttest.V5 == null
                           || tttest.V6 == null
                           || tttest.V8 == null
                           || tttest.V9 == null
                           || tttest.V10 == null
                           || tttest.V11 == null
                           || tttest.V12 == null
                           || tttest.V13 == null
                           || tttest.V15 == null
                           || tttest.V16 == null);
                Log(1, res);
                tttest.V5 = true;
                tttest.V6 = 1;
                tttest.V8 = DateTime.Now;
                tttest.V9 = new decimal(1.22);
                tttest.V10 = 1D;
                tttest.V11 = 1;
                tttest.V12 = 1;
                tttest.V13 = 1L;
                tttest.V15 = 1.5F;
                tttest.V16 = Guid.Empty;
                session.Insert(tttest);
            }

            if (s.GetProviderName() == ProviderName.MySql)
            {
                ClassNullMysql tttest1 = tttest as ClassNullMysql;
                var res = (
                    tttest1.V5 == null
                    || tttest1.V6 == null
                    || tttest1.V8 == null
                    || tttest1.V9 == null
                    || tttest1.V10 == null
                    || tttest1.V11 == null
                    || tttest1.V12 == null
                    || tttest1.V13 == null
                    || tttest1.V15 == null
                    || tttest1.V16 == null
                    || tttest1.V2 == null
                    || tttest1.V3 == null
                    || tttest1.V4 == null
                    || tttest1.V14 == null);
                Log(1, res);
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22);
                ;
                tttest1.V10 = 1.3;
                tttest1.V11 = 1;
                tttest1.V12 = 1;
                tttest1.V13 = 1L;
                tttest1.V15 = 1.5F;
                tttest1.V16 = Guid.Empty;
                tttest1.V2 = 1;
                tttest1.V3 = 1;
                tttest1.V4 = 1;
                tttest1.V14 = 1;
                session.Insert(tttest1);
            }

            if (s.GetProviderName() == ProviderName.SqLite)
            {
                ClassNullSqlite tttest1 = tttest as ClassNullSqlite;
                var res = (tttest1.V5 == null
                           || tttest1.V6 == null
                           || tttest1.V8 == null
                           || tttest1.V9 == null
                           || tttest1.V10 == null
                           || tttest1.V11 == null
                           || tttest1.V12 == null
                           || tttest1.V13 == null
                           || tttest1.V15 == null
                           || tttest1.V16 == null
                           || tttest1.V2 == null
                           || tttest1.V3 == null
                           || tttest1.V4 == null
                           || tttest1.V14 == null);
                Log(1, res);
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22);
                ;
                tttest1.V10 = 1.4;
                tttest1.V11 = 1;
                tttest1.V12 = 1;
                tttest1.V13 = 1L;
                tttest1.V15 = 1.5F;
                tttest1.V16 = Guid.Empty;
                tttest1.V2 = 1;
                tttest1.V3 = 1;
                tttest1.V4 = 1;
                tttest1.V14 = 1;
                session.Insert(tttest1);
            }

            tttest = session.Query<T>().First();

            if (s.GetProviderName() == ProviderName.SqLite)
            {
                ClassNullSqlite tttest1 = tttest as ClassNullSqlite;
                var res = (tttest1.V5 != null
                           || tttest1.V6 != null
                           || tttest1.V8 != null
                           || tttest1.V9 != null
                           || tttest1.V10 != null
                           || tttest1.V11 != null
                           || tttest1.V12 != null
                           || tttest1.V13 != null
                           || tttest1.V15 != null
                           || tttest1.V16 != null
                           || tttest1.V2 != null
                           || tttest1.V3 != null
                           || tttest1.V4 != null
                           || tttest1.V14 != null);
                Log(2, res);
            }

            if (s.GetProviderName() == ProviderName.MySql)
            {
                ClassNullMysql tttest1 = tttest as ClassNullMysql;
                var res = (tttest1.V5 != null
                           || tttest1.V6 != null
                           || tttest1.V8 != null
                           || tttest1.V9 != null
                           || tttest1.V10 != null
                           || tttest1.V11 != null
                           || tttest1.V12 != null
                           || tttest1.V13 != null
                           || tttest1.V15 != null
                           || tttest1.V16 != null
                           || tttest1.V2 != null
                           || tttest1.V3 != null
                           || tttest1.V4 != null
                           || tttest1.V14 != null);
                Log(2, res);
            }

            if (s.GetProviderName() == ProviderName.PostgreSql || s.GetProviderName() == ProviderName.MsSql)
            {
                var res = (tttest.V5 != null
                           || tttest.V6 != null
                           || tttest.V8 != null
                           || tttest.V9 != null
                           || tttest.V10 != null
                           || tttest.V11 != null
                           || tttest.V12 != null
                           || tttest.V13 != null
                           || tttest.V15 != null
                           || tttest.V16 != null);
                Log(2, res);
            }
        }

        public static void TestNativeInsert()
        {
            TestNativeInser<TiPostgresNative, MyDbPostgres>();
            TestNativeInser<TiMysqlNative, MyDbMySql>();
            TestNativeInser<TiMsSqlNative, MyDbMsSql>();
            TestNativeInser<TiSqliteNative, MyDbSqlite>();
        }

        static void TestNativeInser<T, Tb>() where Tb : IOtherDataBaseFactory, new()
            where T : TestInsertBaseNative, new()
        {
            IOtherDataBaseFactory s = Activator.CreateInstance<Tb>();
            Console.WriteLine($@"*********************NativeInser {s.GetProviderName()} **********************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();
            T t = new T();
            var i = session.Insert(t);
            Log(1, i == 1);
            Log(2, t.Id == 1);
            // Log(3, session.IsPersistent(t));
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Log(4, i == 3);
            // i = list.Where(a => session.IsPersistent(a)).Count();
            // Log(5, i == 3);
            i = session.Query<T>().Count();
            Log(6, i == 4);
        }

        public static void TestAssignetInsert()
        {
            TestAssignetInser<TiPostgresAssignet, MyDbPostgres>();
            TestAssignetInser<TiMysqlAssignet, MyDbMySql>();
            TestAssignetInser<TiMsSqlAssignet, MyDbMsSql>();
            TestAssignetInser<TiSqliteAssignet, MyDbSqlite>();
        }

        static void TestAssignetInser<T, Tb>() where Tb : IOtherDataBaseFactory, new()
            where T : TestInsertBaseAssignet, new()
        {
            IOtherDataBaseFactory s = Activator.CreateInstance<Tb>();
            Console.WriteLine($@"********************* AssignetInser {s.GetProviderName()} **********************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();
            T t = new T();
            var i = session.Insert(t);
            Log(1, i == 1);

            // Log(3, session.IsPersistent(t));
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Log(4, i == 3);
            // i = list.Where(a => session.IsPersistent(a)).Count();
            // Log(5, i == 3);
            i = session.Query<T>().Count();
            Log(6, i == 4);
        }

        public static void Log(int i, bool b, string appen = null)
        {
            if (b)
            {
                Console.ForegroundColor
                    = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor
                    = ConsoleColor.Red;
            }

            Console.WriteLine($"{i} {b} {appen}", Console.ForegroundColor);
            Console.ForegroundColor
                = ConsoleColor.White;
        }

        [MapReceiverFreeSql]
        class MyFreeSql
        {
            public MyFreeSql(Guid idGuid, string name, int age, MyEnum @enum)
            {
                IdGuid = idGuid;
                Name = name;
                Age = age;
                MyEnum = (MyEnum)@enum;
            }

            public Guid IdGuid { get; }
            public string Name { get; }
            public int Age { get; }
            public MyEnum MyEnum { get; }
        }

    }
    public class ExecAdd
    {

        public static async Task Run()
        {
            await NewExe<AddClassPostgres, MyDbPostgres>();
            await NewExe<AddClassMysql, MyDbMySql>();
            await NewExe<AddClassMsSql, MyDbMsSql>();
            await NewExe<AddClassSqlite, MyDbSqlite>();
        }
        private static async Task NewExe<T, Tb>() where T : MyClassBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = await Configure.GetSessionAsync<Tb>();


            if (await session.TableExistsAsync<T>())
            {
                await session.DropTableAsync<T>();
            }

            await session.TableCreateAsync<T>();
            T ts = new T { Age = 12, Name = "simple name" };
            bool r = session.IsPersistent(ts);
            Execute.Log(1, r == false);
            var res = session.Insert(ts);
            r = session.IsPersistent(ts);
            Execute.Log(2, r == true && res == 1);
            List<T> list = new List<T>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new T { Age = i, Name = i.ToString() });
            }

            res = session.InsertBulk(list);
            r = session.IsPersistent(list.First());
            Execute.Log(3, r == true && res == 10);
            var tt = session.Get<T>(list.First().Id);
            Execute.Log(4, tt.Id == list[0].Id);
            tt = await session.GetAsync<T>(list.First().Id);
            Execute.Log(5, tt.Id == list[0].Id);
            tt = await session.Query<T>().SingleAsync(a => a.Id == list[0].Id);
            r = session.IsPersistent(ts);
            Execute.Log(6, r == true);
            tt = new T() { Age = 100 };
            res = session.Save(tt);
            r = session.IsPersistent(ts);
            Execute.Log(7, r == true && res == 1);
            tt = session.Get<T>(tt.Id);
            tt.Name = "100";
            res = session.Save(tt);
            r = session.IsPersistent(ts);
            Execute.Log(8, r == true && res == 1);
            tt = session.Get<T>(tt.Id);
            r = session.IsPersistent(tt);
            Execute.Log(9, tt.Name == "100" && r == true);
            tt = await session.Query<T>().SingleAsync(a => a.Name == "100");
            r = session.IsPersistent(tt);
            Execute.Log(10, tt.Name == "100" && r == true);
            var ee = session.Query<T>().Where(a => a.Age > 3 && a.Name == "100").OrderBy(ws => ws.Age)
                .CastCore<MyClassBase>().Count();
            Execute.Log(11, ee == 1);
            var sres = await session.Query<T>().Where(a => a.Age > 3 && a.Name == "100").OrderBy(ws => ws.Age)
                .CastCoreAsync<MyClassBase>();
            ee = sres.Count();
            Execute.Log(12, ee == 1);
            session.TruncateTable<T>();
            var to = new T();
            var si = session.Insert(to);
            Execute.Log(13, si == 1);
            si = session.Delete(to);
            Execute.Log(14, si == 1);

            si = session.Insert(to);
            Execute.Log(15, si == 1);
            si = await session.DeleteAsync(to);
            Execute.Log(16, si == 1);

            var count = session.Query<T>().Count();

            Execute.Log(17, count == 0);



        }

        [MapTable]
        [MapUsagePersistent]
        public class AddClassSqlite : TestLibrary.MyClassBase
        {
        }
        [MapTable]
        [MapUsagePersistent]
        public class AddClassMsSql : MyClassBase
        {
        }
        [MapTable]
        [MapUsagePersistent]
        public class AddClassMysql : MyClassBase
        {
        }
        [MapTable]
        [MapUsagePersistent]
        public class AddClassPostgres : MyClassBase
        {
        }
    }
}