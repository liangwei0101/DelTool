using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelTool.Util
{
    public class StringOperation
    {
        /// <summary>
        /// 文件路径+名称
        /// </summary>
        /// <returns></returns>
        public static string FilePthAndName(string path ,out bool isHasParentDirectoryName)
        {
            var fileType = Path.GetExtension(path);

            if (fileType != null)
            {
                var substring = path.Substring(0, path.Length - fileType.Length);

                // 解压出来有一个文件夹
                if (Directory.Exists(substring))
                {
                    isHasParentDirectoryName = false;
                    return substring;
                }
                else // 解压出来直接是文件
                {
                    isHasParentDirectoryName = true;
                    return path;
                }
   
            }
            else
            {
                isHasParentDirectoryName = false;
                return string.Empty;
            }
        }
    }
}
