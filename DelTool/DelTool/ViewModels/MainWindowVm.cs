using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DelTool.Models;
using DelTool.Util;
using GalaSoft.MvvmLight.Command;
using log4net;
using MaterialDesignThemes.Wpf;
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
        private string _delStr;
        private string _newFolderStr;

        public MainWindowVm()
        {
            InitData();
        }

        public virtual void Dispose() { }

        #region 公有变量

        private Visibility _delVisibility;
        public Visibility DelVisibility
        {
            get { return _delVisibility; }
            set
            {
                _delVisibility = value;
                OnPropertyChanged("DelVisibility");
            }
        }

        private string _sameTextShow;
        public string SameTextShow
        {
            get { return _sameTextShow; }
            set
            {
                _sameTextShow = value;
                OnPropertyChanged("SameTextShow");
            }
        }


        private Visibility _sameFileOrDirPanelVisibility;
        public Visibility SameFileOrDirPanelVisibility
        {
            get { return _sameFileOrDirPanelVisibility; }
            set
            {
                _sameFileOrDirPanelVisibility = value;
                OnPropertyChanged("SameFileOrDirPanelVisibility");
            }
        }

        private Visibility _panelVisibility;
        public Visibility PanelVisibility
        {
            get { return _panelVisibility; }
            set
            {
                _panelVisibility = value;
                OnPropertyChanged("PanelVisibility");
            }
        }

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

        private ObservableCollection<TreeModel> _sameFileOrDirTreeList;
        public ObservableCollection<TreeModel> SameFileOrDirTreeList
        {
            get { return _sameFileOrDirTreeList; }
            set
            {
                _sameFileOrDirTreeList = value;
                OnPropertyChanged("SameFileOrDirTreeList");
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

        private string _searchStr;
        public string SearchStr
        {
            get { return _searchStr; }
            set
            {
                _searchStr = value;
                OnPropertyChanged("SearchStr");
            }
        }

        #endregion

        #region 命令

        public ICommand DelCommand => new RelayCommand(DelAction);

        public ICommand SearchDirectoryOrFileCommand => new RelayCommand(SearchDirectoryOrFileAction);

        public ICommand SelectionPathCommand => new RelayCommand(SelectionPathAction);

        #endregion

        #region 私有函数

        private void DelAction()
        {
            if (string.IsNullOrWhiteSpace(_delStr))
                return;
            //try
            //{
            foreach (var item in FileTreeList)
            {
                var fileOrDir = FileHelper.FindSameDirectoryOrFile(_delStr, item.Nodes);
                if (fileOrDir != null)
                {
                    if (fileOrDir.Type == ResourcesType.Directory)
                    {
                        FileHelper.DeleteFolder(fileOrDir.NodeName);
                    }
                    else
                    {
                        File.Delete(fileOrDir.NodeName);
                    }
                }              
            }

            foreach (var item in FileTreeList)
            {
                // 压缩
                RarClass.Rar(item.CurrNodeName, FilePath);
                // 删除文件
                FileHelper.DeleteFolder(item.CurrNodeName);

                //if (!isHasParentDirectoryName) // 解压出来又一级目录
                //{
                //    // 压缩
                //    RarClass.Rar(folder, item);
                //}
                //else // 直接解压，没有目录
                //{
                //    var dir = Path.GetDirectoryName(file.FullName);
                //    var d = new DirectoryInfo(dir);
                //    var fsinfos = d.GetFileSystemInfos();

                //    var path = Path.GetDirectoryName(file.FullName);
                //    // 压缩
                //    foreach (var fileInfo in fsinfos)
                //    {
                //        RarClass.Rar(fileInfo.FullName, path);
                //    }
                //}
            }

            MessageBox.Show("操作成功！");
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("操作失败！");
            //    throw;
            //}      
        }

        /// <summary>
        /// 文件或者文件夹查询
        /// </summary>
        private void SearchDirectoryOrFileAction()
        {
            if (!string.IsNullOrWhiteSpace(SearchStr.Trim()))
            {
                SameFileOrDirTreeList = new ObservableCollection<TreeModel>();
                foreach (var item in FileTreeList)
                {
                    var treeModel = FileHelper.FindSameDirectoryOrFile(SearchStr, item.Nodes);
                    if (treeModel != null)
                    {
                        SameFileOrDirTreeList.Add(treeModel);
                    }
                }
                if (SameFileOrDirTreeList.Count > 0)
                {
                    // 增加一项
                    var sampList = new ObservableCollection<TreeModel>();
                    var firstOrDefault = SameFileOrDirTreeList.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        _delStr = firstOrDefault.CurrNodeName;
                        var treeModel = new TreeModel
                        {
                            NodeName = "相同项：" + firstOrDefault.CurrNodeName,
                            CurrNodeName = firstOrDefault.CurrNodeName,
                            Nodes = new ObservableCollection<TreeModel>(SameFileOrDirTreeList)
                        };
                        sampList.Add(treeModel);
                    }
                    SameFileOrDirTreeList = new ObservableCollection<TreeModel>(sampList);

                    SameTextShow = "相同项";
                    DelVisibility = Visibility.Visible;
                    SameFileOrDirPanelVisibility = Visibility.Visible;
                }
                else
                {
                    SameTextShow = "没有相同项";
                    SameFileOrDirTreeList.Clear();
                    SameFileOrDirPanelVisibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("字段不能为空！");
            }
        }

        private void CreateOneLevelTree(string fullName, string name)
        {
            FileTree = new TreeModel();
            FileTree.NodeName = name;
            FileTree.CurrNodeName = fullName;
            FileTree.Type = ResourcesType.Directory;
            FileTree.Nodes = new ObservableCollection<TreeModel>();
        }

        private void InitData()
        {
            SameTextShow = "相同项";
            DelVisibility = Visibility.Collapsed;
            PanelVisibility = Visibility.Collapsed;
            DelVisibility = Visibility.Collapsed;
            SameFileOrDirPanelVisibility = Visibility.Collapsed;
            FileTreeList = new ObservableCollection<TreeModel>();
        }

        private void SelectionPathAction()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            dialog.ShowDialog();
            var fileNames = dialog.FileNames;

            if (fileNames != null)
            {
                var enumerable = fileNames as string[] ?? fileNames.ToArray();
                FilePathShow(enumerable);
                FileUnZipOrRar(enumerable);
            }
        }

        private void FilePathShow(IEnumerable<string> fileNames)
        {
            FilePath = "";
            foreach (var item in fileNames)
            {
                FilePath += item;
            }
        }

        private void FileUnZipOrRar(IEnumerable<string> fileNames)
        {
            var treeModelTemp = new ObservableCollection<TreeModel>();

            foreach (var item in fileNames)
            {
                DirectoryInfo root = new DirectoryInfo(item);
                FileInfo[] files = root.GetFiles();

                foreach (var file in files)
                {
                    // 新建一个文件夹
                    var folderStr = FileHelper.CrateFolder(file.FullName);
                    _newFolderStr = folderStr;

                    // 解压文件
                    RarClass.UnRar(file.FullName, folderStr);

                    // 删除原来的文件
                    File.Delete(file.FullName);

                    bool isHasParentDirectoryName = false;

                    // 获取：获取解压后的文件名称
                    var folder = StringOperation.FilePthAndName(file.FullName, out isHasParentDirectoryName);

                    // 创建一级树
                    CreateOneLevelTree(folderStr, file.Name);
                    // 创建文件树
                    FileHelper.GetDirectory(FileTree.Nodes, folder);
                    // 创建树集合
                    treeModelTemp.Add(FileTree);

                }
            }

            if (treeModelTemp.Count > 0)
            {
                FileTreeList = new ObservableCollection<TreeModel>(treeModelTemp);
                PanelVisibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("没有压缩文件！");
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
