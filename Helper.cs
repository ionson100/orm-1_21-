using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    ///     Расширения для построения запроса
    /// </summary>
    public static class Helper
    {


        /// <summary>
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<object> DistinctCore<T>(this IQueryable<T> coll, Expression<Func<T, object>> exp)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.DistinctCastom });
            return (IEnumerable<object>)coll.Provider.Execute<object>(coll.Expression);
        }

        /// <summary>
        /// Удаление обьекта без вытаскивания данных на клиента
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="exp">передикат на удаление</param>
        /// <typeparam name="T">Тип проекции таблицы</typeparam>
        /// <returns></returns>
        public static int Delete<T>(this IQueryable<T> coll, Expression<Func<T, bool>> exp = null) where T : class
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.Delete });
            return coll.Provider.Execute<int>(coll.Expression);
        }


        /// <summary>
        ///     LIMIT всегда ставится в конце предложения LIMIT ( начало позиции с учетом нуля, количество в выборке)
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="start">Начало позиции</param>
        /// <param name="length">Количество записей</param>
        /// <typeparam name="T">Тип проекции таблицы</typeparam>
        /// <returns></returns>
        public static IQueryable<T> Limit<T>(this IQueryable<T> coll, int start, int length)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { TypeRevalytion = Evolution.Limit, ParamList = new List<object> { start, length } });
            return coll;
        }


        /// <summary>
        ///     Преобразование массива байт в картинку
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static Image GetImage(this byte[] coll)
        {
            return Utils.ImageFromByte(coll);
        }

        /// <summary>
        ///     Разбиение на массивы
        /// </summary>
        /// <param name="source"></param>
        /// <param name="chunkLength">количество массивов</param>
        /// <typeparam name="T">Тип проекции таблицы</typeparam>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkLength)
        {
            var enumerable = source as IList<T> ?? source.ToList();
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext()) yield return InnerSplit(enumerator, chunkLength);
            }
        }

        /// <summary>
        ///разбиение на перечисления
        /// </summary>
        /// <param name="coll">исходное перечисление</param>
        /// <param name="splitSize">количество перечислений на выходе</param>
        /// <typeparam name="T">Тип проекции таблицы</typeparam>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> SplitQueryable<T>(this IQueryable<T> coll, int splitSize)
        {
            var enumerable = coll;
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext()) yield return InnerSplit(enumerator, splitSize);
            }
        }

        private static IEnumerable<T> InnerSplit<T>(IEnumerator<T> enumerator, int splitSize)
        {
            var count = 0;
            do
            {
                count++;
                yield return enumerator.Current;
            } while (count % splitSize != 0
                     && enumerator.MoveNext());
        }


        /// <summary>
        /// Обновление таблицы без вытаскивания данных на клиента
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="parametr">Словарь поле - значение</param>
        /// <typeparam name="T">Тип проекции таблицы</typeparam>
        /// <typeparam name="TKey">Свойство - поле</typeparam>
        /// <typeparam name="TValue">Значение</typeparam>
        /// <returns></returns>
        public static int Update<T, TKey, TValue>(this IQueryable<T> coll,
            Expression<Func<T, Dictionary<TKey, TValue>>> parametr) where T : class
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = parametr, TypeRevalytion = Evolution.Update });
            return coll.Provider.Execute<int>(coll.Expression);
        }







        /// <summary>
        ///     Выполенение произвольного запроса с параметрами
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Запрос</param>
        /// <param name="par">Параметры запроса</param>
        /// <typeparam name="TResult">Тип единицы Результата</typeparam>
        /// <returns>IEnumerableTResult</returns>
        public static IEnumerable<TResult> FreeSql<TResult>(this ISession ses, string sql, params Parameter[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (par != null)
            {
                return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, par);
            }
            return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).Execute<TResult>(callExpr);
        }


        /// <summary>
        ///     Выполенение произвольного запроса с параметрами асинхронно
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Запрос</param>
        /// <param name="par">Параметры запроса</param>
        /// <typeparam name="TResult">Тип единицы Результата</typeparam>
        /// <returns>IEnumerableTResult</returns>
        public static Task<IEnumerable<TResult>> FreeSqlAsunc<TResult>(this ISession ses, string sql, params Parameter[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (par != null)
            {
                return Task.FromResult((IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, par));
            }
            return Task.FromResult((IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).Execute<TResult>(callExpr));
        }

        /// <summary>
        /// произвольный запрос к чужой базе
        /// </summary>
        /// <param name="ses"></param>
        /// <param name="dataReader"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> FreeSqlMonster<TResult>(this ISession ses, IDataReader dataReader)
        {

            return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteMonster<TResult>(dataReader);
        }

        /// <summary>
        ///     Вызов хранимой процедуры
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Текст запроса</param>
        /// <typeparam name="TResult">Тип перечисления</typeparam>
        /// <returns>IEnumerable(TResult)</returns>
        public static IEnumerable<TResult> ProcedureCall<TResult>(this ISession ses, string sql)
        {

            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCall<TResult>(callExpr);
        }

        /// <summary>
        ///     Вызов хранимой процедуры с параметрами
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Текст процедуры</param>
        /// <param name="par">Праметры</param>
        /// <typeparam name="TResult">Тип еденицы перчисления</typeparam>
        /// <returns>IEnumerable(TResult)</returns>
        public static IEnumerable<TResult> ProcedureCallParam<TResult>(this ISession ses, string sql,
            params ParameterStoredPr[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCallParam<TResult>(callExpr, par);
        }

        /// <summary>
        /// Асинхронное выполнение запроса
        /// </summary>
        /// <param name="coll"></param>
        /// <returns>Task&lt;TResult&gt;</returns>

        public static Task<List<TResult>> ToListAsync<TResult>(this IQueryable<TResult> coll)
        {
            try
            {
                IInnerList d = (IInnerList)coll;
                var list = d.GetInnerList();
                return Task.FromResult((List<TResult>)list);
            }
            catch(Exception ex)
            {
                Configure.WriteLogFile($"{ex}{Environment.NewLine}sql error: {coll}");
                throw;
            }
            
        }

    }
}