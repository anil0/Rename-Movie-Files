using System;
using System.IO;
using System.Windows.Forms;
using System.Web;


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
                var newFileName = fileSplit[0] + " " + fileSplit[1] + " (" + fileSplit[2] + ")" + file.Extension;
                
                File.Move(file.FullName,newFileName);
            }
        }
    }
}
