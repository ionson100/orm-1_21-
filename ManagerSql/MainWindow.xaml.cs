using ORM_1_21_;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

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
            InitBase(SettingsMy.Default.TypeBase, SettingsMy.Default.ConnctionString, true);
            ColumnTree.Width = new GridLength(SettingsMy.Default.Sp1);
            ComboBoxTypeBase.SelectedIndex = SettingsMy.Default.TypeBase;
            SettingsMy.Default.Connects.Remove("start");
            Utils.InitMenu(MenuLastConnects, (sender, args) =>
            {
                var s = ((MenuItem)args.Source).Header;
                var par = Utils.GetParamConnect(s.ToString());
                if (par == null) return;
                if (par.Value.Item1 > 3) return;
                ComboBoxTypeBase.SelectedIndex = par.Value.Item1;
                TextBoxConnectionString.Text = par.Value.Item2;
                ButtonBase_OnClickRefreshBase(null, null);
            });
            this.Closing += SaveToBase;
        }

        private void SaveToBase(object sender, CancelEventArgs es)
        {
            var res = new SqliteModel();
            res.BaseName = ComboBoxTypeBase.SelectedIndex;
            res.HashStr=Utils.GetHashStr(TextBoxConnectionString.Text);
            foreach (var item in TabControlToot.Items)
            {
                TabItem i = (TabItem)item;
                MyTabItem mi = (MyTabItem)i.Content;
                ModelData data = mi.GetModelData();
                if (data == null) continue;
                res.list.Add(data);
            }

            using (var ses = Configure.GetSession<MyDataProvider>())
            {
                var t = ses.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                   var e= ses.Query<SqliteModel>().Delete(a=>a.BaseName==ComboBoxTypeBase.SelectedIndex&&a.HashStr== Utils.GetHashStr(TextBoxConnectionString.Text));
                    var ee = ses.Query<SqliteModel>().ToList();
                    _ = ses.Save(res);
                    t.Commit();
                }
                catch (Exception e)
                {
                    t.Rollback();
                    MessageBox.Show(e.ToString());
                }
            }
        }


        void InitBase(int typeBase, string conStr, bool isStart = false)
        {
            if (string.IsNullOrWhiteSpace(conStr)) return;
            if(isStart==false)
             SaveToBase(null, null);
            TabControlToot.Items.Clear();
            var listTable = InitConfigure.InitConfigureCore(typeBase, conStr);
            if (listTable == null) return;
            TreeViewTables.Items.Clear();
            foreach (var tableName in listTable)
            {
                StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };
                stack.Children.Add(new Image
                {
                    Source = new BitmapImage
                        (new Uri("pack://application:,,/Resources/table.png")),
                    Width = 25,
                    Height = 25
                });
                stack.Children.Add(new Label { Content = tableName });
                var s = new TreeViewItem() { Header = stack, Tag = tableName };
                var tc = Configure.Session.GetTableColumns(tableName).ToList().OrderByDescending(a => a.IsPk);
                foreach (var c in tc)
                {
                    StackPanel stack1 = new StackPanel { Orientation = Orientation.Horizontal };
                    stack1.Children.Add(new Image
                    {
                        Source = c.IsPk == false
                            ? new BitmapImage(new Uri("pack://application:,,/Resources/column.png"))
                            : new BitmapImage(new Uri("pack://application:,,/Resources/key.png")),
                        Width = 15,
                        Height = 15
                    });
                    stack1.Children.Add(new Label { Content = $"{c.ColumnName} ({c.ColumnType})" });
                    s.Items.Add(new TreeViewItem() { Header = stack1 });
                }

                TreeViewTables.Items.Add(s);
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

            Utils.UpdateLastConnects(typeBase, TextBoxConnectionString.Text);
            using (var ses = Configure.GetSession<MyDataProvider>())
            {
                if (ses.TableExists<SqliteModel>() == false)
                {
                    ses.TableCreate<SqliteModel>();
                }
                var m = ses.Query<SqliteModel>().Where(a => a.BaseName == typeBase && a.HashStr == Utils.GetHashStr(conStr)).ToList();
                if (m == null) return;

               // if (m != null)
               // {
               //     foreach (var sqliteModel in m.list)
               //     {
               //         var ti = new MyTabItem();
               //         ti.SetModelData(sqliteModel);
               //         TabControlToot.Items.Add(ti.TabItem);
               //     }
               // }
                

                if (TabControlToot.Items.Count > 0)
                {
                    TabControlToot.SelectedIndex = 0;
                }
            }
        }

        private void ButtonBase_OnClickRefreshBase(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxConnectionString.Text))
            {
                MessageBox.Show("Строка соединения с базой отсутствует!");
                return;
            }

            InitBase(ComboBoxTypeBase.SelectedIndex, TextBoxConnectionString.Text.Trim());
        }

        private async void ButtonBase_OnClickExecuteSql(object sender, RoutedEventArgs e)
        {
            await Execute();
        }

        private async Task Execute()
        {
            var selectedItem = TabControlToot.SelectedItem;
            if (selectedItem == null) return;
            MyTabItem myTabItem = (MyTabItem)((TabItem)selectedItem).Content;
            await myTabItem.Execute(ButtonExecute, ButtonInit, TextBoxConnectionString, ButtonAdd, TreeViewTables);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Thumb_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SettingsMy.Default.Sp1 = ColumnTree.Width.Value;
            SettingsMy.Default.Save();
        }

        private async void TreeViewTables_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeViewTables.SelectedItem == null) return;
            TreeViewItem item = (TreeViewItem)TreeViewTables.SelectedItem;
            if (item.Tag == null) return;
            string table = (string)item.Tag;
            string sql;

            switch (ComboBoxTypeBase.SelectedIndex)
            {
                case 3: //Sqlite
                    sql = $"SELECT  * FROM {table} LIMIT 10";
                    break;
                case 2: //MSSQL
                    sql = $"SELECT TOP 1000 * FROM [{table}]";
                    break;
                case 1: //MYSQL
                    sql = $"SELECT * FROM `{table}` LIMIT 0,10";
                    break;
                case 0: //POSTGRESQL
                    sql = $"SELECT * FROM \"{table}\" LIMIT 10";
                    break;
                default:
                    {
                        sql = $"SELECT  * FROM {table} LIMIT 10";
                        break;
                    }
            }

            TabControlToot.Items.Add(new MyTabItem(sql).TabItem);
            TabControlToot.SelectedIndex = TabControlToot.Items.Count - 1;
            await Execute();
        }

        private void ButtonBase_OnClickAdd(object sender, RoutedEventArgs e)
        {
            TabControlToot.Items.Add(new MyTabItem().TabItem);
            TabControlToot.SelectedIndex = TabControlToot.Items.Count - 1;
        }
    }
}
