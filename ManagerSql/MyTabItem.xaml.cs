using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManagerSql
{
    /// <summary>
    /// Логика взаимодействия для MyTabItem.xaml
    /// </summary>
    public partial class MyTabItem : UserControl
    {
        public MyTabItem()
        {
            InitializeComponent();
        }

        public TabItem TabItem
        {
            get
            {
                TabItem item = new TabItem
                {
                    Content = this
                };
                return item;
            }
        }

        private void Thumb_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            
        }

        private void DataGridSql_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }
    }
}
