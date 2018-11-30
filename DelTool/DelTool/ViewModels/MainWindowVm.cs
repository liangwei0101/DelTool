using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DelTool.Models;
using DelTool.Util;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        private ObservableCollection<TreeModel> _fileTreeList;
        public ObservableCollection<TreeModel> FileTreeList
        {
            get { return _fileTreeList; }
            set
            {
                _fileTreeList = value;
                OnPropertyChanged("FileTreeList");
            }
        }

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
            FileTree.NodeName = nodeName;
            FileTree.Type = ResourcesType.Directory;
            FileTree.Nodes = new ObservableCollection<TreeModel>();
        }

        private void InitData()
        {
            FileTree = new TreeModel();
            FileTreeList = new ObservableCollection<TreeModel>();
        }

        private void SelectionPathAction()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };
            var aa = dialog.ShowDialog();

            var fileNames = dialog.FileNames;

            //var ofd = new OpenFileDialog
            //{
            //    Title = "请选择文件夹",
            //    Multiselect = true
            //};
            //ofd.ShowDialog();
            //var fileNames = ofd.FileNames;

            var enumerable = fileNames as string[] ?? fileNames.ToArray();
            FilePathShow(enumerable);
            FileUnZipOrRar(enumerable);

        }

        private void FilePathShow(IEnumerable<string> fileNames)
        {
            FilePath = "";
            foreach (var item in fileNames)
            {
                FilePath += item + ";" + "\n";
            }
        }

        private void FileUnZipOrRar(IEnumerable<string> fileNames)
        {
            foreach (var item in fileNames)
            {
                DirectoryInfo root = new DirectoryInfo(item);
                FileInfo[] files = root.GetFiles();

                foreach (var file in files)
                {
                    // 解压文件
                    RarClass.UnRar(file.FullName, item);
                    // 删除原来的文件
                    File.Delete(file.FullName);
                    // 获取：路径+文件名称
                    var folder = StringOperation.FilePthAndName(item);

                    if (item == folder) // 说明直接解压出来的文件，没有新建一个文件夹
                    {
                        // 压缩
                        //RarClass.Rar(rarPath, folderPath);
                    }
                    else // 解压后，有一个文件夹
                    {
                        // 压缩
                        RarClass.Rar(folder, item);
                    }
                    // 获取文件名称
                    var directoryName = FileHelper.GetFileName(item);
                    // 创建一级树
                    CreateOneLevelTree(directoryName);
                    // 创建文件树
                    FileHelper.GetDirectory(FileTree.Nodes, folder, 2);
                    // 创建树集合
                    FileTreeList.Add(FileTree);
                    // 删除文件
                    //FileHelper.DeleteFolder(folder);
                }
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
