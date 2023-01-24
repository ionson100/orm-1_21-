using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ORM_1_21_;

namespace ManagerSql
{
   // <TextBlock>PostgreSql</TextBlock>
   //     <TextBlock>MySql</TextBlock>
   //     <TextBlock>MSSql</TextBlock>
   //     <TextBlock>Sqlite</TextBlock>
    static class InitConfigure
    {
        public static List<string> InitConfigureCore(int typeBase, string conString)
        {
            ProviderName name = ProviderName.Postgresql;
            switch (typeBase)
            {
                case 0:
                {
                    name = ProviderName.Postgresql;
                    break;
                }
                case 1:
                {
                    name = ProviderName.MySql;
                        break;
                }
                case 2:
                {
                    name = ProviderName.MsSql;
                    break;
                }
                case 3:
                {
                    name = ProviderName.Sqlite;
                    break;
                }
            }
            try
            {
                _ = new Configure(conString, name, null);
                return Configure.GetSession().GetTableNames().OrderBy(a => a).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

            return null;

        }
    }
}
