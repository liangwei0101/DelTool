using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using DelTool.Models;
using DelTool.Util;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DelTool.ViewModels
{
    using System;
    using GalaSoft.MvvmLight;

    // Base class for MainWindow's ViewModels. All methods must be virtual. Default constructor must exist.
    //  Using this Base Class will allow xaml to bind variables to a concrete View Model at compile time
    public class MainWindowVm : ViewModelBase, IDisposable, INotifyPropertyChanged
    {

        public MainWindowVm()
        {
            InitData();
        }

        public virtual void Dispose() { }

        #region 公有变量

        private TreeModel _fileTree;
        public TreeModel FileTree
        {
            get { return _fileTree; }
            set
            {
                _fileTree = value;
                OnPropertyChanged("FileTree");
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        #endregion

        #region 命令

        public ICommand SelectionPathCommand => new RelayCommand(SelectionPathAction);

        #endregion

        #region 私有函数

        private void CreateOneLevelTree(string nodeName)
        {
            var pathInfo = new DirectoryInfo(nodeName);
            if (pathInfo.Parent != null)
            {
                string newPath = pathInfo.Parent.FullName;
                FileTree.NodeName = newPath;
                FileTree.Type = ResourcesType.Directory;
                FileTree.Nodes = new ObservableCollection<TreeModel>();
            }
        }

        private void InitData()
        {
            FileTree = new TreeModel();
        }

        private void SelectionPathAction()
        {
            var ofd = new OpenFileDialog
            {
                Title = "请选择文件",
                Multiselect = true
            };
            ofd.ShowDialog();
            var fileNames = ofd.FileNames;
            FilePathShow(fileNames);
            FileUnZipOrRar(fileNames);
              
        }

        private void FilePathShow(string[] fileNames)
        {
            FilePath = "";
            foreach (var item in fileNames)
            {
                FilePath += item + ";" + "\n";
            }
        }

        private void FileUnZipOrRar(string[] fileNames)
        {
            foreach (var item in fileNames)
            {
                TreeModel treeModel = new TreeModel();

                // 获取当前的路径
                var folderPath =  Path.GetDirectoryName(item);
                // 解压文件
                RarClass.UnRar(item, folderPath);
                // 删除原来的文件
                File.Delete(item);
                // 获取：路径+文件名称
                var folder = StringOperation.FilePthAndName(item);
                // 压缩
                RarClass.Rar(folder, folderPath);
                // 创建一级树
                CreateOneLevelTree(folder);
                // 创建树
                FileHelper.GetDirectory(FileTree.Nodes, folder, 2);

                var aa = FileTree;
                // 删除文件
                FileHelper.DeleteFolder(folder);
            }
        }

        #endregion

        #region 属性变更

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
