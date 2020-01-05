using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace DirektoryAnalyzer.Models
{
    public class OperationOverFolder
    {
        public FolderResultModel ChangesInDirektory(string path)
        {
            if (path == null)
            {
                return new FolderResultModel() { Message = StringOutput.PathNotSet };
            }
            if (!Directory.Exists(path))
            {
                return new FolderResultModel() { Message = StringOutput.DirektoryNotFound };
            }

            List<DirectoryModel> currentData = LoadCurrentDirektory(path);

            List<DirectoryModel> result = new List<DirectoryModel>();
            List<DirectoryModel> saveData = LoadData(path);
            if (saveData != null)
            {
                result = CompareFiles(saveData, currentData);
            }
            else
            {
                SaveDate(currentData, path);
                return new FolderResultModel() { Message = StringOutput.NewFolder };
            }

            SaveDate(currentData, path);

            if (result.Any())
                return new FolderResultModel()
                {
                    Files = result,
                    Result = true
                };
            return new FolderResultModel()
            {
                Message = StringOutput.NotChanges
            };
        }

        private List<DirectoryModel> CompareFiles(List<DirectoryModel> load, List<DirectoryModel> current)
        {
            List<DirectoryModel> files = new List<DirectoryModel>();

            var deleteFiles = load.Where(y => current.Count(z => z.Name == y.Name) == 0).ToList();
            var addedFiles = current.Where(y => load.Count(z => z.Name == y.Name) == 0).ToList();
            var editedFiles = load.Where(y => current.Count(z => z.Name == y.Name && z.LastWrite != y.LastWrite) == 1).ToList();

            //set the current version
            foreach (DirectoryModel loadItem in load)
            {
                foreach (DirectoryModel editItem in editedFiles)
                {
                    if (loadItem.Name == editItem.Name)
                        loadItem.Version += 1;
                }

            }

            //hand over the version of the current list
            foreach (DirectoryModel currentItem in current)
            {
                foreach (DirectoryModel versionItem in load)
                {
                    if (currentItem.Name == versionItem.Name)
                        currentItem.Version = versionItem.Version;
                }
            }


            deleteFiles.ForEach(x => x.Type = FileType.Deleted);
            addedFiles.ForEach(x => x.Type = FileType.Created);
            editedFiles.ForEach(x => x.Type = FileType.Edited);


            files.AddRange(deleteFiles);
            files.AddRange(addedFiles);
            files.AddRange(editedFiles);

            return files;
        }


        private void SaveDate(List<DirectoryModel> list, string fileName)
        {
            string fileNameDirektory = Path.GetFileName(fileName);
            string fileNameFile = fileNameDirektory + StringOutput.SetFormat;
            string pathOfTempDirektory = string.Empty;

            try
            {
                pathOfTempDirektory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StringOutput.NameOfDirektory);
                if (!Directory.Exists(pathOfTempDirektory))
                    Directory.CreateDirectory(pathOfTempDirektory);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            try
            {
                string path2 = Path.Combine(pathOfTempDirektory, fileNameFile);

                string json = JsonConvert.SerializeObject(list.ToArray());

                File.WriteAllText(path2, json);

            }
            catch (Exception e)
            {
                Console.WriteLine(StringOutput.CreateFileErorr, e.Message);
            }
        }

        private List<DirectoryModel> LoadData(string pathOfTempDirektory)
        {
            string fileNameDirektory = Path.GetFileName(pathOfTempDirektory);
            string fileNameFile = fileNameDirektory + StringOutput.SetFormat;

            pathOfTempDirektory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StringOutput.NameOfDirektory);
            string pathToFile = Path.Combine(pathOfTempDirektory, fileNameFile);

            if (File.Exists(Path.Combine(pathOfTempDirektory, fileNameFile)))
            {
                using (StreamReader r = new StreamReader(pathToFile))
                {
                    string json = r.ReadToEnd();
                    List<DirectoryModel> loadList = JsonConvert.DeserializeObject<List<DirectoryModel>>(json);

                    return loadList;
                }

            }

            return null;
        }

        private List<DirectoryModel> LoadCurrentDirektory(string pathOfDiretory)
        {
            List<DirectoryModel> currentData = new List<DirectoryModel>();
            string[] filePaths = Directory.GetFiles(pathOfDiretory, "*", SearchOption.AllDirectories);
            foreach (string FileNames in filePaths)
            {
                var informationAboutFile = new FileInfo(FileNames);
                var lastEdit = informationAboutFile.LastWriteTime;
                string name = informationAboutFile.Name;
                string fileName = informationAboutFile.FullName;
                string DirectoryName = Path.GetFileName(pathOfDiretory);
                string FolderName = new DirectoryInfo(Path.GetDirectoryName(fileName)).Name;
                if (DirectoryName != FolderName)
                {
                    name = FolderName + "/" + name;
                }

                currentData.Add(new DirectoryModel
                {
                    Name = name,
                    Path = fileName,
                    LastWrite = lastEdit
                });

            }
            return currentData;

        }
    }
}