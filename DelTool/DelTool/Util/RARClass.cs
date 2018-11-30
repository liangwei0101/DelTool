using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DelTool.Util
{
    /// <summary>
    /// rar文件
    /// </summary>
    public class RarClass
    {
        /// <summary>  
        /// 获取WinRAR.exe路径  
        /// </summary>  
        /// <returns>为空则表示未安装WinRAR</returns>  
        public static string ExistsRar()
        {
            var regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");

            if (regkey != null)
            {
                string strkey = regkey.GetValue("").ToString();
                regkey.Close();
                return strkey;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>  
        /// 解压RAR文件  
        /// </summary>  
        /// <param name="rarFilePath">要解压的文件路径</param>  
        /// <param name="unrarDestPath">解压路径（绝对路径）</param>  
        public static void UnRar(string rarFilePath, string unrarDestPath)
        {
            string rarexe = ExistsRar();
            if (String.IsNullOrEmpty(rarexe))
            {
                throw new Exception("未安装WinRAR程序。");
            }
            try
            {
                //组合出需要shell的完整格式  
                var shellArguments = $"x -o+ \"{rarFilePath}\" \"{unrarDestPath}\\\"";

                //用Process调用  
                using (Process unrar = new Process())
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    startinfo.FileName = rarexe;
                    startinfo.Arguments = shellArguments;               //设置命令参数  
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;  //隐藏 WinRAR 窗口  

                    unrar.StartInfo = startinfo;
                    unrar.Start();
                    unrar.WaitForExit();//等待解压完成  

                    unrar.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>  
        ///  压缩为RAR文件  
        /// </summary>  
        /// <param name="filePath">要压缩的文件路径（绝对路径）</param>  
        /// <param name="rarfilePath">压缩到的路径（绝对路径）</param>  
        public static void Rar(string filePath, string rarfilePath, string otherPara = "")
        {
            Rar(filePath, rarfilePath, "", "", otherPara);
        }

        /// <summary>  
        ///  压缩为RAR文件  
        /// </summary>  
        /// <param name="filePath">要压缩的文件路径（绝对路径）</param>  
        /// <param name="rarfilePath">压缩到的路径（绝对路径）</param>  
        /// <param name="rarName">压缩后压缩包名称</param>
        /// <param name="otherPara"></param>  
        public static void Rar(string filePath, string rarfilePath, string rarName, string otherPara = "")
        {
            Rar(filePath, rarfilePath, "", rarName, otherPara);
        }

        /// <summary>  
        ///  压缩为RAR文件  
        /// </summary>  
        /// <param name="filePath">要压缩的文件路径（绝对路径）</param>  
        /// <param name="rarfilePath">压缩到的路径（绝对路径）</param>  
        /// <param name="rarName">压缩后压缩包名称</param>  
        /// <param name="password">解压密钥</param>  
        public static void Rar(string filePath, string rarfilePath, string password, string rarName, string otherPara = "")
        {
            var rarexe = ExistsRar();
            if (string.IsNullOrEmpty(rarexe))
            {
                throw new Exception("未安装WinRAR程序。");
            }

            if (!Directory.Exists(filePath))
            {
                throw new Exception("文件不存在！");  
            }

            if (String.IsNullOrEmpty(rarName))
            {
                rarName = Path.GetFileNameWithoutExtension(filePath) + ".rar";
            }
            else
            {
                if (Path.GetExtension(rarName).ToLower() != ".rar")
                {
                    rarName += ".rar";
                }
            }

            try
            {
                //Directory.CreateDirectory(rarfilePath);  
                //压缩命令，相当于在要压缩的文件夹(path)上点右键->WinRAR->添加到压缩文件->输入压缩文件名(rarName)  
                string shellArguments;
                if (String.IsNullOrEmpty(password))
                {
                    shellArguments = $"a -ep1 \"{rarName}\" \"{filePath}\" -r";
                }
                else
                {
                    shellArguments = $"a -ep1 \"{rarName}\" \"{filePath}\" -r -p\"{password}\"";
                }
                if (!string.IsNullOrEmpty(otherPara))
                {
                    shellArguments = shellArguments + " " + otherPara;
                }

                using (Process rar = new Process())
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    startinfo.FileName = rarexe;
                    startinfo.Arguments = shellArguments;               //设置命令参数  
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;  //隐藏 WinRAR 窗口  
                    startinfo.WorkingDirectory = rarfilePath;

                    rar.StartInfo = startinfo;
                    rar.Start();
                    rar.WaitForExit(); //无限期等待进程 winrar.exe 退出  
                    rar.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
