using ORM_1_21_;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                  r.Dispose();
              }));
        }

        private async Task<DataTable> GetDataTableTask(string sql)
        {

            //await Task.Delay(3000);
            return await Task.Run(() => Configure.Session.GetDataTable(sql, 0));
        }
    }
}
