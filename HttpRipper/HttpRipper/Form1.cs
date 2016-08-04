using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace HttpRipper
{
    public partial class Form1 : Form
    {
        public string rootPath = @"D:\temp\";
        private List<string> downloadedPaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            Download(textBox1.Text, rootPath);
        }

        public void Download(string url, string localpath)
        {
            downloadedPaths.Add(url);

            WebClient client = new WebClient();
            client.Proxy = WebRequest.DefaultWebProxy;
            client.Credentials = System.Net.CredentialCache.DefaultCredentials; ;
            client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string filename = Path.GetFileName(url);
            if (string.IsNullOrEmpty(filename))
            {
                string[] split = url.Split('/');
                filename = split[split.Count() - 1];
                if (string.IsNullOrEmpty(filename) && (split.Count() >1))
                {
                    filename = split[split.Count() - 2];
                }
            }

            if (string.IsNullOrEmpty( filename ))
            {
                return;
            }

            string localfile = Path.Combine(rootPath, filename);
            try
            {
                client.DownloadFile(url, localfile);
            }
            catch
            {
                return;
            }

            List<string> subPaths = new List<string>();


            using (StreamReader sr = new StreamReader(localfile))
            {
                string buffer = sr.ReadToEnd();
                Regex reg = new Regex("href=\"[^\"]*");
                MatchCollection matches = reg.Matches(buffer);
                foreach (Match match in matches)
                {
                    if (!url.Equals(match.Value))
                    {
                        if (!subPaths.Contains(match.Value))
                        {
                            subPaths.Add(match.Value);
                        }
                    }
                }
            }
            listBox1.DataSource = downloadedPaths;
            //mkdir filename
             foreach (string subPath in subPaths)
            {
                string[] split = subPath.Split('\"');
                string fullUrl;
                if (split[1].StartsWith("/"))
                {
                    fullUrl = url + split[1];
                }
                else
                {
                    fullUrl = url;
                }

                if (!downloadedPaths.Contains(split[1]))
                {
                    Download(split[1], Path.Combine(localpath, filename));
                }
            }
             
        }
    }
}
