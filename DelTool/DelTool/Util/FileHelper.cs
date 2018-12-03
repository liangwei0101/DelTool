using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using DelTool.Models;

namespace DelTool.Util
{
    public class FileHelper
    {
        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            var fileName = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1, (path.LastIndexOf(".", StringComparison.Ordinal) - path.LastIndexOf("\\", StringComparison.Ordinal) - 1)); //文件名
            return fileName;
        }

        /// <summary>
        /// 删除文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteFolder(string directoryPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(directoryPath))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);     //删除文件   
                }
                else
                    DeleteFolder(d);    //删除文件夹
            }
            Directory.Delete(directoryPath);    //删除空文件夹
        }

        /// <summary>
        /// 创建文件夹树
        /// </summary>
        /// <param name="srcPath"></param>
        public static void CreateDirTree(string srcPath)
        {
            try
            {
                TreeModel treeModel = new TreeModel();
                treeModel.Nodes = new ObservableCollection<TreeModel>();

                DirectoryInfo dir = new DirectoryInfo(srcPath);
                treeModel.NodeName = dir.Name;
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录

                foreach (FileSystemInfo i in fileinfo)
                {
                    treeModel.Nodes.Add(new TreeModel
                    {
                        NodeName = i.FullName
                    });

                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        CreateDirTree(i.FullName);
                    }
                }


                var aa = treeModel;
            }
            catch (Exception e)
            {
                throw;
            }
        }



        /// <summary>
        /// 获得指定路径下所有子目录名
        /// </summary>
        /// <param name="fileObservableCollection"></param>
        /// <param name="path"></param>
        /// <param name="indent"></param>
        public static void GetDirectory(ObservableCollection<TreeModel> fileObservableCollection, string path)
        {
            var root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                var treeModel = new TreeModel
                {
                    NodeName = f.Name,
                    CurrNodeName = f.Name,
                    Type = ResourcesType.File,
                    Nodes = new ObservableCollection<TreeModel>()
                };
                fileObservableCollection.Add(treeModel);
            }

            var directories = root.GetDirectories();

            if (directories.Length == 0)
                return;

            foreach (DirectoryInfo d in root.GetDirectories())
            {
                var treeModel = new TreeModel
                {
                    NodeName = d.FullName,
                    CurrNodeName = d.Name,
                    Type = ResourcesType.Directory,
                    Nodes = new ObservableCollection<TreeModel>()
                };
                fileObservableCollection.Add(treeModel);
                var firstOrDefault = fileObservableCollection.FirstOrDefault(s => s.NodeName == d.FullName);
                if (firstOrDefault != null)
                    GetDirectory(firstOrDefault.Nodes, d.FullName);
            }
        }

        /// <summary>
        /// 查找文件或者文件夹
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fileObservableList"></param>
        /// <returns></returns>
        public static TreeModel FindSameDirectoryOrFile(string key, ObservableCollection<TreeModel> fileObservableList)
        {
            if (fileObservableList.Count > 0)
            {
                foreach (var item in fileObservableList)
                {
                    if (key == item.CurrNodeName)
                        return item;

                    if (item.Nodes.Count> 0)
                    {
                        var treeModel = FindSameDirectoryOrFile(key, item.Nodes);
                        if (treeModel != null)
                            return treeModel;
                    }   
                }
            }             
            return null;
        }
    }
}