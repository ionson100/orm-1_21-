using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ORM_1_21_;

namespace ManagerSql
{
    class ExecutorSql
    {
        public async Task Execute(string sql, DataGrid dataGrid)
        {

            _ = dataGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGrid.DataContext = null;
            }));

            var r = await GetDataTableTask(sql);
            _ = dataGrid.Dispatcher.BeginInvoke(new Action(() =>
              {
                  dataGrid.DataContext = r;
              }));
        }

        private async Task<DataTable> GetDataTableTask(string sql)
        {
          
           //await Task.Delay(3000);
            return await Task.Run(() => Configure.Session.GetDataTable(sql, 0));
        }
    }
}
