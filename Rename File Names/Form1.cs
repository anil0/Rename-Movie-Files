using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;
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
                RenameFiles(file,path);
            }
        }

        public MovieInformation GetMovieInformation(string constructedNameFromParts)
        {
            //json parsing of movie data
            var json = "";
            using (var wc = new WebClient())
            {
                // var name = "12 feet deep";
                json = wc.DownloadString("http://www.theimdbapi.org/api/find/movie?title=" + constructedNameFromParts);
            }

            var deserializedProduct = JsonConvert.DeserializeObject<List<RootObject>>(json);//List<RootObject>

            var title = deserializedProduct[0].title;
            var year = deserializedProduct[0].year;

            return new MovieInformation(){Title = title, Year = year};
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            var path = "";

            if (browser.ShowDialog() == DialogResult.OK)
            {
                path = browser.SelectedPath; // prints path
                Console.WriteLine(path);
            }

            //gets all folders within a parent folder
            var directories = Directory.GetDirectories(path);

            //show all paths of those folders
            foreach (var directory in directories)
            {
                Console.WriteLine("Directory ---");
                Console.WriteLine(directory);
                //do name changing for the folder here

                //access the files of that folder
                //lets you create/move and enumerate files from a path
                var directoryInfo = new DirectoryInfo(directory);
                //gets list of the actual files in that folder
                var fileInfo = directoryInfo.GetFiles();

                foreach (var file in fileInfo)
                {
                    Console.WriteLine("File --------");
                    Console.WriteLine(file.FullName);

                    //check to see if inside the folder there is a .mp4,.avi or .mkv file if not then look for another folder which would contain said file

                }
            }
            
        }

        public void RenameFiles(FileInfo file, string path)
        {
            var fileSplit = file.FullName.Split('.');
            var fileSplitByName = file.Name.Split('.');

            var constructedNameFromParts = "";

            foreach (var s in fileSplitByName)
            {
                //if year then stop and grab only name

                if (Regex.IsMatch(s, "^(19|20)[0-9][0-9]"))
                {
                    break;
                }

                constructedNameFromParts += s + " ";
            }

            if (fileSplit[1].Equals("mp4") || fileSplit[1].Equals("avi"))
            {
                Console.WriteLine("Error: can not be formatted as already contains formatting");
            }
            else
            {
                var movieInformation = GetMovieInformation(constructedNameFromParts);

                //formatting new file name
                var newFileName = path + "\\" + movieInformation.Title + " (" + movieInformation.Year + ")" + file.Extension;
                //rename the file from original to new
                File.Move(file.FullName, newFileName);
            }
        }

    }//end class
}
