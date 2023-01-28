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
               // name = ProviderName.MsSql;
               // conString =
               //     @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                _ = new Configure(conString, name, null);
                return Configure.Session.GetTableNames().OrderBy(a => a).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

            return null;

        }

      
       
    }
}
