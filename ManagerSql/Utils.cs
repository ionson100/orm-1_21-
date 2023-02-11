using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ManagerSql
{
    static class Utils
    {
        public static string GetTotalConnection(int typeBase, string connectionString)
        {
            return $"{connectionString}   {typeBase}";
        }

        public static void UpdateLastConnects(int typeBase, string connectionString)
        {
            var l = SettingsMy.Default.Connects;
            if (l == null)
            {
                l = new StringCollection();
            }

            string t = GetTotalConnection(typeBase, connectionString);
            {
                if (l.Count > 9)
                {
                    l.RemoveAt(0);
                }

                if (l.Contains(t) == false)
                {
                    l.Add(t);
                }
                SettingsMy.Default.Save();
            }

        }

        public static (int, string)? GetParamConnect(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            int last = s.LastIndexOf("   ", StringComparison.CurrentCulture);
            if (last == -1) return null;
            string c = s.Substring(0, last).Trim();
            string iss = s.Substring(last).Trim();
            int res;
            if (int.TryParse(iss, out res))
            {
                return (res, c);
            }

            return null;
        }
        public static void InitMenu(MenuItem itemRoot, RoutedEventHandler action)
        {
            var conList = SettingsMy.Default.Connects;

            if (conList != null)
            {
                foreach (string header in conList)
                {
                    ContextMenu contextMenu = new ContextMenu();
                    MenuItem delete = new MenuItem
                    {
                        Header = "Delete",
                        Tag = header
                    };
                    delete.Click += (sender, args) =>
                    {
                        if (MessageBox.Show("Delete connection string from menu?", "Delete connection", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                            return;
                        var sm = ((MenuItem)sender).Tag.ToString();
                        List<MenuItem> deItems = new List<MenuItem>();
                        foreach (object itemRootItem in itemRoot.Items)
                        {
                            var sm1 = ((MenuItem)itemRootItem).Header.ToString();
                            if (sm1 == sm)
                            {
                                deItems.Add((MenuItem)itemRootItem);
                            }
                        }
                        deItems.ForEach(a =>
                        {
                            SettingsMy.Default.Connects.Remove(sm);
                            itemRoot.Items.Remove(a);
                        });
                        SettingsMy.Default.Save();
                    };
                    contextMenu.Items.Add(delete);


                    var item = new MenuItem
                    {
                        Header = header,
                        ContextMenu = contextMenu,
                        Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,/Resources/database.png")), }
                    };

                    item.Click += action;
                    itemRoot.Items.Add(item);
                }
            }
        }

        public static int GetHashStr(string str)
        {
            return  str.Replace(" ","").ToUpper().GetHashCode();
        }
    }
}