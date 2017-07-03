using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;


namespace Rename_File_Names
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetFiles_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            var path = "";

            if (browser.ShowDialog() == DialogResult.OK)
            {
                path = browser.SelectedPath; // prints path
                Console.WriteLine(path);
            }

            //lets you create/move and enumerate files from a path
            var directoryInfo = new DirectoryInfo(path);
            //gets list of the actual files in that folder
            var fileInfo = directoryInfo.GetFiles();

            foreach (var file in fileInfo)
            {
                var fileSplit = file.FullName.Split('.');
                if (fileSplit[1].Equals("mp4") || fileSplit[1].Equals("avi"))
                {
                    Console.WriteLine("can not be formatted as already contains formatting");
                }
                else
                {
                    var newFileName = path + "\\" + fileSplit[0] + " " + fileSplit[1] + " (" + fileSplit[2] + ")" + file.Extension;
                
                    File.Move(file.FullName,newFileName);
                }
            }

            //json
            var json = "";
            using (var wc = new WebClient())
            {
                var name = "12 feet deep";
                json = wc.DownloadString("http://www.theimdbapi.org/api/find/movie?title="+ name);
            }

            var deserializedProduct = JsonConvert.DeserializeObject<List<RootObject>>(json);//List<RootObject>
 
            Console.WriteLine(deserializedProduct[0].title);
            Console.WriteLine(deserializedProduct[0].year);

        }
    }
}
