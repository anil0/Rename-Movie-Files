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

            if(deserializedProduct == null)
            {
                return null;
            }

            var title = deserializedProduct[0].title;
            var year = deserializedProduct[0].year;

            return new MovieInformation(){Title = title, Year = year};
        }

        public MovieInformation GetMovieInformationWhatIsMyMovieApi(string constructedNameFromParts)
        {
            

            //json parsing of movie data
            var json = "";
            using (var wc = new WebClient())
            {
                // var name = "12 feet deep";
                json = wc.DownloadString("https://api.whatismymovie.com/1.0/?api_key=jPvyBrw6iWCD2sgU&text=" + constructedNameFromParts);
            }

            var deserializedProduct = JsonConvert.DeserializeObject<List<RootObject>>(json);//List<RootObject>

            if (deserializedProduct == null)
            {
                return null;
            }

            var title = deserializedProduct[0].title;
            var year = deserializedProduct[0].year;

            return new MovieInformation() { Title = title, Year = year };
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
                string originalDirectory = directory;
                Console.WriteLine("Directory ---");
                Console.WriteLine(directory);

                //do name changing for the folder here
                var movieInformation = RenameFolder(directory);

                var indexOfLastBackSlash = directory.LastIndexOf('\\');
                var folderPath = directory.Substring(0, indexOfLastBackSlash);
                var folderName = directory.Substring(indexOfLastBackSlash + 1);
                var renamedDirectory = folderPath + "\\" + movieInformation.Title + " (" + movieInformation.Year + ")";

                ///////////////////////////////////////////////////////////////
                //does the current folder contain another folder or more than one?
                var subDirectories = Directory.GetDirectories(renamedDirectory);

                if (subDirectories.Length > 0)
                {
                    Console.WriteLine("Yes contains another folder! -----");
                    //list all the inner folders
                    foreach (var subDirectory in subDirectories)
                    {
                        Console.WriteLine(subDirectory);//inner folder path
                        //send to rename folder and go into this directory and rename file from folder name
                    }
                    
                }
                ////////////////////////////////////////////////////////

                //access the files of that folder
                //lets you create/move and enumerate files from a path
                var directoryInfo = new DirectoryInfo(renamedDirectory);
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

            if (Regex.IsMatch(file.Name, "(\\((19|20)[0-9][0-9]\\))")) //Already Formatted //HAS A VALID YEAR = ^(19|20)[0-9][0-9] 
            {
                Console.WriteLine("Error: can not be formatted as already contains formatting");
            }
            else
            {
                //contains dot formatting in name
                if (fileSplitByName.Length > 2)
                {
                    //contains dot formatting in name
                    Console.WriteLine("Contains Dot Formatting");

                    SetupRenameFilePath(fileSplitByName, constructedNameFromParts, path, file);
                    
                }
                //contains space formatting
                else
                {
                    var fileSplitBySpace = file.Name.Split(' ');

                    Console.WriteLine("contains space formatting");

                    SetupRenameFilePath(fileSplitBySpace,constructedNameFromParts,path,file);

                }
            }
        }

        public MovieInformation RenameFolder(string path)
        {
            var indexOfLastBackSlash = path.LastIndexOf('\\');
            var folderPath = path.Substring(0, indexOfLastBackSlash);
            var folderName = path.Substring(indexOfLastBackSlash + 1);
            var fileSplitByName = folderName.Split('.');

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

            var movieInformation = GetMovieInformation(constructedNameFromParts);

            //formatting new file name
            var newFolderName = folderPath + "\\" + movieInformation.Title + " (" + movieInformation.Year + ")";

            //rename the file from original to new
            Directory.Move(path, newFolderName);

            return movieInformation;

        }

        public void SetupRenameFilePath(string[] fileSplitBySpace, string constructedNameFromParts, string path, FileInfo file)
        {
            int i = 0;
            foreach (var s in fileSplitBySpace)
            {
                i++;
                //if year then stop and grab only name
                if (Regex.IsMatch(s, "^(19|20)[0-9][0-9]")) //HAS A VALID YEAR = ^(19|20)[0-9][0-9] 
                {
                    if (i < 2) //starts with a year
                    {
                        foreach (var s1 in fileSplitBySpace)
                        {
                            constructedNameFromParts += s1 + " ";
                        }
                    }

                    break;
                }

                constructedNameFromParts += s + " ";
            }

            var movieInformation = GetMovieInformation(constructedNameFromParts);

            if (movieInformation != null)
            {
                //formatting new file name
                var newFileName = path + "\\" + movieInformation.Title + " (" + movieInformation.Year + ")" +
                                  file.Extension;
                //rename the file from original to new
                File.Move(file.FullName, newFileName);
            }
            else
            {
                Console.WriteLine("ERROR: No such movie exists to get the information for: " + constructedNameFromParts);
                Console.WriteLine("Retrying From Another Source...");
                var movieInfo = GetMovieInformationWhatIsMyMovieApi(constructedNameFromParts);

                //formatting new file name
                var newFileName = path + "\\" + movieInfo.Title + " (" + movieInfo.Year + ")" +
                                  file.Extension;
                //rename the file from original to new
                File.Move(file.FullName, newFileName);

            }
        }

    }//end class
}
