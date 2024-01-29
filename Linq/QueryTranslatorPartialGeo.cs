using ORM_1_21_.geo;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace ORM_1_21_.Linq
{
    internal sealed partial class QueryTranslator<T>
    {
        private static readonly HashSet<string> HashSetMethods = new HashSet<string>
        {
            "SetSrid","ListGeoPoints","GetGeoJson,StGeometryTypeAsync","StAreaAsync"
        };

        bool GeoMethodCallExpression(MethodCallExpression m)
        {
            if (CurrentEvolution == Evolution.Update) return false;
            if (HashSetMethods.Contains(m.Method.Name))
            {
                throw new Exception(
                    $"The {m.Method.Name} method cannot be used in an expression tree to build an sql query");
            }

            if (!(m.Object is MemberExpression mem))
            {
                return false;

            }


            if (m.Method.Name == "StGeometryType")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StGeometryType");

                return true;

            }

            if (m.Method.Name == "StAsText")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StAsText");


                return true;

            }

            if (m.Method.Name == "StArea")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StArea");


                return true;
            }

            if (m.Method.Name == "StWithin")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();

                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STWithin(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Within({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Within({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;
            }



            if (m.Method.Name == "StAsBinary")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StAsBinary");
                return true;
            }

            if (m.Method.Name == "StBoundary")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StBoundary");
                return true;
            }



            if (m.Method.Name == "StBuffer")
            {

                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StBuffer", m.Arguments[0]);

                return true;
            }

            if (m.Method.Name == "StTranslate")
            {

                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StTranslate", m.Arguments[0], m.Arguments[1]);

                return true;
            }



            if (m.Method.Name == "StCentroid")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StCentroid");
                return true;

            }

            if (m.Method.Name == "StContains")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STContains(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Contains({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Contains({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
            }

            if (m.Method.Name == "StCrosses")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STCrosses(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Crosses({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Crosses({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
            }

            if (m.Method.Name == "StDifference")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                ExecuteMethodIGeoShapeParamGeo(nameColumn, "StDifference", geoShape);
                return true;
            }

            if (m.Method.Name == "StDimension")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StDimension");
                return true;

            }

            if (m.Method.Name == "StDisjoint")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STDisjoint(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Disjoint({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Disjoint({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;

            }

            if (m.Method.Name == "StDistance")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STDistance(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Distance({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Distance({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StEndPoint")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StEndPoint");
                return true;
            }

            if (m.Method.Name == "StEnvelope")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StEnvelope");
                return true;

            }

            if (m.Method.Name == "StEquals")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STEquals(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Equals({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Equals({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;

            }

            if (m.Method.Name == "StIntersects")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STIntersects(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Intersects({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Intersects({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
            }

            if (m.Method.Name == "StOverlaps")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STOverlaps(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Overlaps({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" ST_Overlaps({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
            }

            if (m.Method.Name == "StSrid")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StSrid");

                return true;
            }

            if (m.Method.Name == "StStartPoint")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StStartPoint");
                return true;
            }

            if (m.Method.Name == "StSymDifference")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                ExecuteMethodIGeoShapeParamGeo(nameColumn, "StSymDifference", geoShape);

                return true;
            }

            if (m.Method.Name == "StTouches")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();

                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STTouches(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Touches({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Touches({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;
            }

            if (m.Method.Name == "StNumGeometries")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StNumGeometries");

                return true;
            }

            if (m.Method.Name == "StNumInteriorRing")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StNumInteriorRing");

                return true;
            }

            if (m.Method.Name == "StIsSimple")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StIsSimple");
                return true;
            }

            if (m.Method.Name == "StIsValid")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StIsValid");
                return true;
            }

            if (m.Method.Name == "StLength")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StLength");
                return true;
            }

            if (m.Method.Name == "StIsClosed")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StIsClosed");


                return true;
            }

            if (m.Method.Name == "StNumPoints")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StNumPoints");


                return true;
            }

            if (m.Method.Name == "StPerimeter")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StPerimeter");
            }

            if (m.Method.Name == "StUnion")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                ExecuteMethodIGeoShapeParamGeo(nameColumn, "StUnion", geoShape);

                return true;
            }

            if (m.Method.Name == "StConvexHull")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StConvexHull");
            }

            if (m.Method.Name == "StCollect")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape[])Expression.Lambda<Func<object>>(m.Arguments[0]).Compile().Invoke();
                ExecuteMethodIGeoShapeParamGeoCollection(nameColumn, "StCollect", geoShape);
            }

            if (m.Method.Name == "StPointN")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StPointN", m.Arguments[0]);

                return true;
            }

            if (m.Method.Name == "StPointOnSurface")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StPointOnSurface");
            }

            if (m.Method.Name == "StInteriorRingN")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StInteriorRingN", m.Arguments[0]);
            }

            if (m.Method.Name == "StX")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StX");
            }

            if (m.Method.Name == "StY")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StY");
            }

            if (m.Method.Name == "StTransform")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StTransform", m.Arguments[0]);
            }

            if (m.Method.Name == "StSetSRID")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeParam(nameColumn, "StSetSRID", m.Arguments[0]);
            }

            if (m.Method.Name == "StAsLatLonText")
            {
                if (_providerName != ProviderName.PostgreSql)
                {
                    throw new Exception("Only for Postgres");
                }
                string nameColumn = GetColumnName(mem.Member.Name);
                if (m.Arguments[0] is ConstantExpression t)
                {
                    if (t.Value == null)
                    {
                        StringB.Append($" ST_AsLatLonText({nameColumn})");
                    }
                    else
                    {
                        StringB.Append($" ST_AsLatLonText({nameColumn}, ");
                        Visit(m.Arguments[0]);
                        StringB.Append(")");
                    }
                }
                else
                {
                    StringB.Append($" ST_AsLatLonText({nameColumn}, ");
                    Visit(m.Arguments[0]);
                    StringB.Append(")");
                }

            }

            if (m.Method.Name == "StReverse")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StReverse");
            }

            if (m.Method.Name == "StIsValidReason")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StIsValidReason");
            }

            if (m.Method.Name == "StMakeValid")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteMethodIGeoShapeNoParam(nameColumn, "StMakeValid");
            }

            if (m.Method.Name == "StAsGeoJson")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                ExecuteSimple(nameColumn, "StAsGeoJSON");
            }



            return false;
        }

        static void CheckArgument(MethodCallExpression m)
        {
            if (m.Arguments.Count == 0)
            {
                throw new Exception($"The argument to the function {m.Method.Name} is empty or is null");
            }
            if (m.Arguments[0] == null)
            {
                throw new Exception($"The argument to the function {m.Method.Name} is empty or is null");
            }
        }

        void AddSimpleSql(string methodName, string nameColumn)
        {

            switch (_providerName)
            {
                case ProviderName.MsSql:
                    if (methodName == "STSrid")
                    {
                        StringB.Append($"{nameColumn}.STSrid");
                    }
                    else
                    {
                        StringB.Append($"{nameColumn}.{methodName}()");
                    }

                    break;
                case ProviderName.MySql:
                    StringB.Append($"{methodName}({nameColumn})");
                    break;
                case ProviderName.PostgreSql:
                    StringB.Append($"{methodName}({nameColumn})");
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
            }
        }

        void ExecuteMethodIGeoShapeNoParam(string nameColumn, string methodName)
        {
            var s = _currentMethodSelect;
            var w = _currentMethodWhere;
            methodName = GetNameMethod(methodName, _providerName);

            if (s != null)
            {
                if (_currentMethodType != null && _currentMethodType != typeof(IGeoShape))
                {
                    AddSimpleSql(methodName, nameColumn);

                }
                else
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            if (methodName == "STSrid")
                            {
                                StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STSrid).STAsText()) ");
                            }
                            else
                            {
                                StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.{methodName}()).STAsText()) ");
                            }

                            break;
                        case ProviderName.MySql:
                            StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}(ST_GeomFromText(ST_AsText({nameColumn}))))) ");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn}))) ");
                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                    }
                }

            }
            else if (w != null)
            {
                AddSimpleSql(methodName, nameColumn);
            }
            else
            {
                throw new Exception("Смотри сюда");
            }
        }

        void ExecuteMethodIGeoShapeParam(string nameColumn, string methodName, params Expression[] param)
        {
            var s = _currentMethodSelect;
            var w = _currentMethodWhere;
            methodName = GetNameMethod(methodName, _providerName);

            if (s != null)
            {
                if (_currentMethodType != null && _currentMethodType != typeof(IGeoShape))
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:

                            StringB.Append($"{nameColumn}.{methodName}(");
                            NewFunction();
                            StringB.Append(")");

                            break;
                        case ProviderName.MySql:
                            StringB.Append($"{methodName}({nameColumn}, ");
                            NewFunction();
                            StringB.Append(")");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append($"{methodName}({nameColumn}, ");
                            NewFunction();
                            StringB.Append(")");
                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                    }

                }
                else
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:

                            StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.{methodName}(");
                            NewFunction();
                            StringB.Append(")).STAsText())");

                            break;
                        case ProviderName.MySql:
                            StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},");
                            NewFunction();
                            StringB.Append(")))");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},");
                            NewFunction();
                            StringB.Append(")))");
                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                    }
                }

            }
            else if (w != null)
            {
                switch (_providerName)
                {
                    case ProviderName.MsSql:

                        StringB.Append($"{nameColumn}.{methodName}(");
                        NewFunction();
                        StringB.Append(")");

                        break;
                    case ProviderName.MySql:
                        StringB.Append($"{methodName}({nameColumn}, ");
                        NewFunction();
                        StringB.Append(")");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"{methodName}({nameColumn}, ");
                        NewFunction();
                        StringB.Append(")");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
            }
            else
            {
                throw new Exception("Смотри сюда");
            }

            void NewFunction()
            {
                for (var i = 0; i < param.Length; i++)
                {
                    Visit(param[i]);
                    if (i != param.Length - 1)
                    {
                        StringB.Append(" , ");
                    }
                }
            }
        }

        void ExecuteMethodIGeoShapeParamGeo(string nameColumn, string methodName, IGeoShape geoShape)
        {
            var s = _currentMethodSelect;
            var w = _currentMethodWhere;
            methodName = GetNameMethod(methodName, _providerName);



            if (s != null)
            {
                if (_currentMethodType != null && _currentMethodType != typeof(IGeoShape))
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:

                            StringB.Append($"{nameColumn}.{methodName}(geometry::STGeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($",{geoShape.StSrid()}))");
                            break;
                        case ProviderName.MySql:
                            StringB.Append($"{methodName}({nameColumn},ST_GeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($", {geoShape.StSrid()}))");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append($"{methodName}({nameColumn},ST_GeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($", {geoShape.StSrid()}))");
                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                    }

                }
                else
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';', ({nameColumn}.{methodName}(geometry::STGeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($",{geoShape.StSrid()})).STAsText()))");
                            break;
                        case ProviderName.MySql:
                            StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},ST_GeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($", {geoShape.StSrid()}))))");
                            break;
                        case ProviderName.PostgreSql:

                            StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},ST_GeomFromText(");
                            AddParameter(geoShape.StAsText());
                            StringB.Append($", {geoShape.StSrid()}))))");

                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                    }
                }

            }
            else if (w != null)
            {
                switch (_providerName)
                {
                    case ProviderName.MsSql:

                        StringB.Append($"{nameColumn}.{methodName}(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($",{geoShape.StSrid()}))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"{methodName}({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"{methodName}({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
            }
            else
            {
                throw new Exception("Смотри сюда");
            }



        }

        void ExecuteSimple(string nameColumn, string methodName)
        {
            methodName = GetNameMethod(methodName, _providerName);
            switch (_providerName)
            {
                case ProviderName.MsSql:
                    if (methodName == "STSrid")
                    {
                        StringB.Append($" {nameColumn}.STSrid");
                    }
                    else if (methodName == "STX")
                    {
                        StringB.Append($" {nameColumn}.STX");
                    }
                    else if (methodName == "STY")
                    {
                        StringB.Append($" {nameColumn}.STY");
                    }
                    else
                    {
                        StringB.Append($" {nameColumn}.{methodName}()");
                    }

                    break;
                case ProviderName.MySql:
                    StringB.Append($" {methodName}(ST_GeomFromText(ST_AsText({nameColumn})))");
                    break;
                case ProviderName.PostgreSql:
                    StringB.Append($" {methodName}({nameColumn})");
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
            }
        }

        void ExecuteAsBuffer(string nameColumn, string methodName, Expression param)
        {
            methodName = GetNameMethod(methodName, _providerName);
            switch (_providerName)
            {
                case ProviderName.MsSql:
                    StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.{methodName}(");
                    Visit(param);
                    StringB.Append(")).STAsText()) ");
                    break;
                case ProviderName.MySql:
                    StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}(ST_GeomFromText(ST_AsText({nameColumn})), ");
                    Visit(param);
                    StringB.Append("))) ");
                    break;
                case ProviderName.PostgreSql:
                    StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn}, ");
                    Visit(param);
                    StringB.Append("))) ");

                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
            }
        }






        void ExecuteMethodIGeoShapeParamGeoCollection(string nameColumn, string methodName, params IGeoShape[] geoShape)
        {
            void Simple()
            {
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"{nameColumn}.{methodName}(");
                        FuncMsSql();
                        StringB.Append($")");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"{methodName}({nameColumn},");
                        FuncPostgres();
                        StringB.Append($")");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"{methodName}({nameColumn},");
                        FuncPostgres();
                        StringB.Append($")");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
            }

            void FuncPostgres()
            {
                for (var i = 0; i < geoShape.Length; i++)
                {
                    StringB.Append("ST_GeomFromText(");
                    AddParameter(geoShape[i].StAsText());
                    StringB.Append(", ");
                    StringB.Append($"{geoShape[i].StSrid()})");
                    if (i != geoShape.Length - 1)
                    {
                        StringB.Append(" , ");
                    }
                }
            }
            void FuncMsSql()
            {
                for (var i = 0; i < geoShape.Length; i++)
                {
                    StringB.Append("geometry::STGeomFromText(");
                    AddParameter(geoShape[i].StAsText());
                    StringB.Append(", ");
                    StringB.Append($"{geoShape[i].StSrid()})");
                    if (i != geoShape.Length - 1)
                    {
                        StringB.Append(" , ");
                    }
                }
            }

            var s = _currentMethodSelect;
            var w = _currentMethodWhere;
            methodName = GetNameMethod(methodName, _providerName);



            if (s != null)
            {
                if (_currentMethodType != null && _currentMethodType != typeof(IGeoShape))
                {
                    Simple();
                }
                else
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';', ({nameColumn}.{methodName}(");
                            FuncMsSql();
                            StringB.Append($")))");
                            break;
                        case ProviderName.MySql:
                            StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},");
                            FuncPostgres();
                            StringB.Append($")))");
                            break;
                        case ProviderName.PostgreSql:

                            StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText({methodName}({nameColumn},");
                            FuncPostgres();
                            StringB.Append($")))");

                            break;
                        case ProviderName.SqLite:
                            UtilsCore.ErrorAlert();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                    }
                }

            }
            else if (w != null)
            {
                Simple();
            }
            else
            {
                throw new Exception("Смотри сюда");
            }



        }



    }


    internal sealed partial class QueryTranslator<T>
    {
        private static readonly HashSet<string> NameAllowed = new HashSet<string> { "StAsBinary", "StAsText", "StSrid", "StLength" };
        bool GeoMethodCallExpression2(MethodCallExpression m)
        {
            if (CurrentEvolution == Evolution.Update) return false;
            if (HashSetMethods.Contains(m.Method.Name))
            {
                throw new Exception(
                    $"The {m.Method.Name} method cannot be used in an expression tree to build an sql query");
            }

            if (NameAllowed.Contains(m.Method.Name))
            {
                var name = GetNameMethod(m.Method.Name, _providerName);
                {
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            break;
                        case ProviderName.MySql:
                            StringB.Append($"({name}(");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append($"({name}(");
                            break;
                        case ProviderName.SqLite:
                            break;

                    }
                    Visit(m.Object);
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            if (name == "STSrid")
                            {
                                StringB.Append($".{name}");
                            }
                            else
                            {
                                StringB.Append($".{name}()");
                            }

                            break;
                        case ProviderName.MySql:
                            StringB.Append("))");
                            break;
                        case ProviderName.PostgreSql:
                            StringB.Append("))");
                            break;
                        case ProviderName.SqLite:
                            break;
                    }
                }
                return true;
            }

            return false;

        }
        public static string GetNameMethod(string name, ProviderName providerName)
        {
            string n1 = name.Substring(0, 2);
            string n2 = name.Substring(2);
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return $"{n1.ToUpper()}{n2}";
                case ProviderName.MySql:
                    return $"{n1.ToUpper()}_{n2}";

                case ProviderName.PostgreSql:
                    return $"{n1.ToUpper()}_{n2}";
                case ProviderName.SqLite:
                    throw new Exception("12-23");
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerName), providerName, null);
            }
        }
    }


}
