using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    /// Интерфейс для  обращение к чужой базе данных
    /// </summary>
    public interface IOtherDataBaseFactory
    {
        /// <summary>
        /// Тип базы данных 
        /// </summary>
        ProviderName GetProviderName();

        /// <summary>
        /// Получение Провайдера для выбранной базы данных
        /// </summary>
        DbProviderFactory GetDbProviderFactories();

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        string GetConnectionString();
    }
}
