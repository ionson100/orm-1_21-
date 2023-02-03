using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    /// Обращение к чужой базе данных
    /// </summary>
    public interface IOtherDataBaseFactory
    {
        /// <summary>
        /// Получение IDbCommand
        /// </summary>
        /// <returns></returns>
        IDbCommand GetDbCommand();

        /// <summary>
        /// Тип базы данных 
        /// </summary>
        /// <returns></returns>
        ProviderName GetProviderName();

        /// <summary>
        /// Получение IDbConnection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        string GetConnectionString();
    }
}
