using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ManagerSql
{
    /// <summary>
    /// Логика взаимодействия для MyTabItem.xaml
    /// </summary>
    public partial class MyTabItem : UserControl
    {
        private TabItem item;
        public MyTabItem(string sql=null)
        {
            InitializeComponent();
            InitItem();
            if (sql != null)
            {
                TextBoxSql.Text = sql;
            }
        }

        public ModelData GetModelData()
        {
            if (string.IsNullOrWhiteSpace(TextBoxSql.Text.Trim()))
                return null;
            return new ModelData
            {
                Position = RowTextSql.Height.Value,
                Sql = TextBoxSql.Text
            };
        }

        public void SetModelData(ModelData data)
        {
            if (data.Position < 5)
            {
                data.Position = 200;
            }
            TextBoxSql.Text = data.Sql;
            RowTextSql.Height = new GridLength(data.Position);
        }

        private void InitItem()
        {
            item = new TabItem();
            
             var d = new StackPanel
             {
                 Orientation = Orientation.Horizontal,
                 FlowDirection = FlowDirection.RightToLeft,
                 HorizontalAlignment = HorizontalAlignment.Right
             };
             var but = new Button
             { 
                 Margin = new Thickness(0,0,30,0),
                 Height = 20,
                 Width = 20,
                 Content = new Image
                 {
                     Source = new BitmapImage(new Uri("pack://application:,,,/resources/close.png")),
                     VerticalAlignment = VerticalAlignment.Stretch,
                 },
                 Style = (Style)FindResource("TabCloseButton")
             };
             but.Click += (o, args) =>
             {
                 var r = MessageBox.Show("Delete tab?", "",
                     MessageBoxButton.OKCancel,
                     MessageBoxImage.Question,MessageBoxResult.Cancel);
                 if (r == MessageBoxResult.OK)
                 {
                     TabControl control = (TabControl)item.Parent;
                     control.Items.Remove(item);
                 }
             };
             d.Children.Add(but);
             d.Children.Add(new TextBlock
                 {
                     Text = "SQL", FontSize = 20, VerticalAlignment = VerticalAlignment.Center
                 });
             d.MouseEnter += (sender, args) => { d.ToolTip = TextBoxSql.Text; };
             item.Header = d;
             item.Content = this;

             if (RowTextSql.Height.Value < 5)
             {
                 RowTextSql.Height = new GridLength(200);
             }
        }

        public async Task Execute(params Control[] control)
        {
            if (string.IsNullOrWhiteSpace(TextBoxSql.Text)) return;
            
             await Dispatcher.BeginInvoke((Action)(() =>
                 Mouse.OverrideCursor = Cursors.Wait));
             foreach (var control1 in control)
             {
                 control1.IsEnabled = false;
             }
             
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
                 foreach (var control1 in control)
                 {
                     control1.IsEnabled = true;
                 }
                
             }
        }


        public TabItem TabItem => item;

     
    }
}
