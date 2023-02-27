using System.Data.Common;

namespace ORM_1_21_
{
    /// <summary>
    /// Interface for accessing a foreign database
    /// </summary>
    public interface IOtherDataBaseFactory
    {
        /// <summary>
        /// Type database
        /// </summary>
        ProviderName GetProviderName();

        /// <summary>
        /// Getting the DbProviderFactory for the selected database
        /// </summary>
        DbProviderFactory GetDbProviderFactories();

        /// <summary>
        /// Database connection string
        /// </summary>
        string GetConnectionString();
    }
}
