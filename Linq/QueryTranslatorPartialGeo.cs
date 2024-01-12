using ORM_1_21_.geo;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ORM_1_21_.Utils;


namespace ORM_1_21_.Linq
{
    internal sealed partial class QueryTranslator<T> : ExpressionVisitor, ITranslate
    {
        private static readonly HashSet<string> HashSetMethods = new HashSet<string>
        {
            "SetSrid","ListGeoPoints","GetGeoJson",
        };

        bool GeoMethodCallExpression(MethodCallExpression m)
        {

            if (HashSetMethods.Contains(m.Method.Name))
            {
                throw new Exception(
                    $"The {m.Method.Name} method cannot be used in an expression tree to build an sql query");
            }
            var mem = m.Object as MemberExpression;
            if (mem == null)
            {
                return false;

            }
            

            if (m.Method.Name == "StGeometryType")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STGeometryType()");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_GeometryType({nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_GeometryType({nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;

            }

            if (m.Method.Name == "StAsText")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"{nameColumn}.STAsText()");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_AsText({nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_AsText({nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;

            }

            if (m.Method.Name == "StArea")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STArea()");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Area({nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Area({nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StWithin")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape) Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
               
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
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STAsBinary()");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_AsBinary({nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_AsBinary({nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;
            }

            if (m.Method.Name == "StBoundary")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STBoundary()).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Boundary({nameColumn}))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Boundary({nameColumn}))) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;
            }

            if (m.Method.Name == "StBuffer")
            {

                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STBuffer(");
                        Visit(m.Arguments[0]);
                        StringB.Append(")).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Buffer(ST_GeomFromText(ST_AsText({nameColumn})), ");
                        Visit(m.Arguments[0]);
                        StringB.Append("))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Buffer({nameColumn}, ");
                        Visit(m.Arguments[0]);
                        StringB.Append("))) ");
                       
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;
            }

            if (m.Method.Name == "StCentroid")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STCentroid()).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Centroid(ST_GeomFromText(ST_AsText({nameColumn}))))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Centroid({nameColumn}))) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

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
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';', ({nameColumn}.STDifference(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($",{geoShape.StSrid()})).STAsText()))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Difference({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Difference({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
            }

            if (m.Method.Name == "StDimension")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STDimension()");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Dimension({nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Dimension({nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

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
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STEndPoint()).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_EndPoint(ST_GeomFromText(ST_AsText({nameColumn}))))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_EndPoint({nameColumn}))) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StEnvelope")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STEnvelope()).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Envelope(ST_GeomFromText(ST_AsText({nameColumn}))))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Envelope({nameColumn}))) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

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

            if (m.Method.Name == "StOverlapsContra")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append("(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()})).STOverlaps({nameColumn})");
                        break;
                    case ProviderName.MySql:
                        StringB.Append("ST_Overlaps(ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}),{nameColumn})");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append("ST_Overlaps(ST_GeomFromText("); 
                        AddParameter(geoShape.StAsText());
                         StringB.Append($", {geoShape.StSrid()}),{nameColumn})");
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
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STSrid as srid ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Srid({nameColumn}) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Srid({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StStartPoint")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';',({nameColumn}.STStartPoint()).STAsText()) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_StartPoint(ST_GeomFromText(ST_AsText({nameColumn}))))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($"CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_StartPoint({nameColumn}))) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StWithinContra")
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
                        StringB.Append($" ST_Within(ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}), {nameColumn})");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Within(ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}), {nameColumn})");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }
                return true;

            }

            if (m.Method.Name == "StSymDifference")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';', ({nameColumn}.STSymDifference(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($",{geoShape.StSrid()})).STAsText()))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_SymDifference({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_SymDifference({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
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
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STNumGeometries() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_NumGeometries({nameColumn}) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_NumGeometries({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StNumInteriorRing")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STNumInteriorRing() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_NumInteriorRing({nameColumn}) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_NumInteriorRing({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StIsSimple")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STIsSimple() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_IsSimple({nameColumn}) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_IsSimple({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StLength")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STLength() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Length({nameColumn}) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Length({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StIsClosed")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STIsClosed() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_IsClosed(ST_GeomFromText(ST_AsText({nameColumn}))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_IsClosed({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StNumPoints")
            {
                string nameColumn = GetColumnName(mem.Member.Name);
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STNumPoints() ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_NumPoints(ST_GeomFromText(ST_AsText({nameColumn}))) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_NumPoints({nameColumn}) ");
                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
                }

                return true;
            }

            if (m.Method.Name == "StUnion")
            {
                CheckArgument(m);
                string nameColumn = GetColumnName(mem.Member.Name);
                var geoShape = (IGeoShape)Expression.Lambda<Func<object>>(m.Arguments[0]).Compile()();
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($"CONCAT('SRID=',{nameColumn}.STSrid,';', ({nameColumn}.STUnion(geometry::STGeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($",{geoShape.StSrid()})).STAsText()))");
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Union({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");
                        break;
                    case ProviderName.PostgreSql:

                        StringB.Append($" CONCAT('SRID=',ST_Srid({nameColumn}),';',ST_AsText(ST_Union({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.StAsText());
                        StringB.Append($", {geoShape.StSrid()}))))");

                        break;
                    case ProviderName.SqLite:
                        UtilsCore.ErrorAlert();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");

                }
                return true;
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

       

    }
    
}
