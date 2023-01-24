using System;
using System.Collections.Generic;
using System.Data;
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
using ORM_1_21_;

namespace ManagerSql
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitBase(SettingsMy.Default.TypeBase,SettingsMy.Default.ConnctionString,true);
            TextBoxSql.Text = SettingsMy.Default.LastSql;
            ColumnTree.Width = new GridLength(SettingsMy.Default.Sp1);
            RowTextSql.Height = new GridLength(SettingsMy.Default.Sp2);
            ComboBoxTypeBase.SelectedIndex = SettingsMy.Default.TypeBase;
        }

        void InitBase(int typeBase,string conStr,bool isStart=false)
        {
            if(string.IsNullOrWhiteSpace(conStr)) return;
            var listTable = InitConfigure.InitConfigureCore(typeBase,conStr);
            if (listTable == null) return;
            TreeViewTables.Items.Clear();
            foreach (var tableName in listTable)
            {
                TreeViewTables.Items.Add(new TreeViewItem() { Header = tableName });
            }

            if (isStart)
            {
                TextBoxConnectionString.Text = conStr;
            }
            else
            {
                SettingsMy.Default.ConnctionString = conStr;
                SettingsMy.Default.TypeBase = ComboBoxTypeBase.SelectedIndex;
                SettingsMy.Default.Save();
            }

            
        }

        private void ButtonBase_OnClickRefreshBase(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxConnectionString.Text))
            {
                MessageBox.Show("Строка соединения с базой отсутствует!");
                return;
            }
            InitBase(ComboBoxTypeBase.SelectedIndex,TextBoxConnectionString.Text.Trim());
        }

        private async void ButtonBase_OnClickExecuteSql(object sender, RoutedEventArgs e)
        {
            await Execute();
        }

        private async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(TextBoxSql.Text)) return;

            await Dispatcher.BeginInvoke((Action)(() =>
                Mouse.OverrideCursor = Cursors.Wait));
            ButtonExecute.IsEnabled = ButtonInit.IsEnabled = TextBoxConnectionString.IsEnabled = false;
            try
            {
                await new ExecutorSql().Execute(TextBoxSql.Text, DataGridSql);

                TextBoxError.Visibility = Visibility.Hidden;
                SettingsMy.Default.LastSql = TextBoxSql.Text.Trim();
                SettingsMy.Default.Save();
            }
            catch (Exception ex)
            {
                DataGridSql.DataContext = null;
                TextBoxError.Visibility = Visibility.Visible;
                TextBoxError.Text = ex.Message;
            }
            finally
            {
                await Dispatcher.BeginInvoke((Action)(() => Mouse.OverrideCursor = null));
                ButtonExecute.IsEnabled = ButtonInit.IsEnabled = TextBoxConnectionString.IsEnabled = true;
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Thumb_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            GridSplitter gridSplitter = (GridSplitter)sender;
            switch (gridSplitter.Name)
            {
                case "Splitter1":
                {
                    SettingsMy.Default.Sp1 =ColumnTree.Width.Value;
                    SettingsMy.Default.Save();
                    break;
                }
                case "Splitter2":
                {
                    SettingsMy.Default.Sp2 = RowTextSql.Height.Value;
                    SettingsMy.Default.Save();
                        break;
                }
            }
            
        }

        private async void TextBoxSql_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await Execute();
            }
        }

        private async void TreeViewTables_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeViewTables.SelectedItem == null) return;
            TreeViewItem item = (TreeViewItem)TreeViewTables.SelectedItem;
            string table = (string)item.Header;
            string sql;
            
            switch (ComboBoxTypeBase.SelectedIndex)
            {
                case 3://Sqlite
                    sql = $"SELECT  * FROM {table} LIMIT 10";
                    break;
                case 2://MSSQL
                    sql = $"SELECT TOP 1000 * FROM [{table}]";
                    break;
                case 1://MYSQL
                    sql = $"SELECT * FROM `{table}` LIMIT 0,10";
                    break;
                case 0://POSTGRESQL
                    sql = $"SELECT * FROM \"{table}\" LIMIT 10";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                
            }

            TextBoxSql.Text = sql;
            await Execute();
        }
    }
}
