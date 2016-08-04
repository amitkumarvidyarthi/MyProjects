using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace GroupImageFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<DateTime,List<string>> data = new Dictionary<DateTime, List<string>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if( dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath.Content = dialog.SelectedPath;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnOrganize_Click(object sender, RoutedEventArgs e)
        {
            if(!Directory.Exists( SelectedPath.Content.ToString()))
            {
                return;
            }

            string[] files = Directory.GetFiles(SelectedPath.Content.ToString());
            foreach (string file in files)
            {
                var dateTime = File.GetCreationTime(file);
                if(data.ContainsKey(dateTime.Date))
                {
                    data[dateTime.Date].Add(file);

                }
                else
                {
                    var list = new List<string> {file};
                    data.Add(dateTime.Date, list);
                    var item = new KeyValuePair<DateTime, List<string>>(dateTime, list);
                    ListView1.Items.Add(item);
                }
            }

            //GridView1.DataContext = data;
            //GridView1.DataBind();

        }

        private void CreateDynamicGridView()
        {
            // Create a GridView 
            var grdView = new GridView();
            grdView.AllowsColumnReorder = true;
            grdView.ColumnHeaderToolTip = "Authors";

            var nameColumn = new GridViewColumn();
            nameColumn.DisplayMemberBinding = new System.Windows.Data.Binding("Key");
            nameColumn.Header = "Author Name";
            nameColumn.Width = 120;
            grdView.Columns.Add(nameColumn);

            var ageColumn = new GridViewColumn();
            ageColumn.DisplayMemberBinding = new System.Windows.Data.Binding("Value.ToString()");
            ageColumn.Header = "Age";
            ageColumn.Width = 30;
            grdView.Columns.Add(ageColumn);

            ListView1.View = grdView;
        }

        private void CreateIt_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<DateTime, List<string>> item in data)
            {
                
                string yearPath = System.IO.Path.Combine(SelectedPath.Content.ToString(), item.Key.Year.ToString("yyyy MM dd", new CultureInfo("en-US")));
                if(!Directory.Exists(yearPath))
                {
                    Directory.CreateDirectory(yearPath);
                }
                foreach (string filePathName in item.Value)
                {
                    string fileName = System.IO.Path.GetFileName(filePathName);
                    if (fileName != null)
                    {
                        File.Move(filePathName, System.IO.Path.Combine(yearPath, fileName));
                    }
                }
            }
        }

    }
}
