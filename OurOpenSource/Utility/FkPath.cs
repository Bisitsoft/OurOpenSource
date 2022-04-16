using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OurOpenSource.Utility
{
#warning 移到OurOpenSource里

    /// <remarks>
    /// 结尾的空格也算作路径的一部分。
    /// DirectorySeparator忽略出现意外值的情况，默认为Backslash。
    /// 仅支持本地路径。
    /// </remarks>
    public static class FkPath
    {
        public enum DirectorySeparator
        {
            /// <summary>
            /// 不变或者使用默认。
            /// </summary>
            DoNotCare = 0,
            /// <summary>
            /// \
            /// </summary>
            Slash = 1, //'\'
            /// <summary>
            /// /
            /// </summary>
            Backslash = 2, //'/'
        }

        /// <remark>
        /// 如果无法判断则返回原值
        /// </remark>
        public static string GetRelativePath(string referenceAbsolutePath, string absolutePath, DirectorySeparator directorySeparator)
        {
            if (!IsExists(referenceAbsolutePath))
            {
                throw new PathNotFoundException(String.Format("\"{0}\" not found.", referenceAbsolutePath));
            }
            if (!IsAbsolutePath(referenceAbsolutePath, true))
            {
                throw new PathNotFoundException(String.Format("\"{0}\" isn't an absolute path.", referenceAbsolutePath));
            }
            if (!IsAbsolutePath(absolutePath, true))
            {
                throw new PathNotFoundException(String.Format("\"{0}\" isn't an absolute path.", absolutePath));
            }

            //获取短路径来保证接下来的比对。
            //这里的_referenceAbsolutePath是文件夹路径，不是文件路径。
            //#warning ReplaceDirectorySeparator()是否需要？
            //string _referenceAbsolutePath = NormalizeHeader(GetDirectory(ReplaceDirectorySeparator(GetShortPath(referenceAbsolutePath, true), directorySeparator)));
            //string _absolutePath = NormalizeHeader(ReplaceDirectorySeparator(GetShortPath(absolutePath, true), directorySeparator));
#warning NormalizeHeader()是否需要？
            string _referenceAbsolutePath = NormalizeHeader(GetDirectory(GetShortPath(referenceAbsolutePath, true)));
            string _absolutePath = NormalizeHeader(GetShortPath(absolutePath, true));

            string _referenceAbsolutePathRoot = GetRootPath(_referenceAbsolutePath);
            string _absolutePathRoot = GetRootPath(_absolutePath);

            if(String.Equals(_referenceAbsolutePathRoot, _absolutePathRoot)) //完全比较，类似二进制比较，不同于String.Compare的带有CultureInfo的比较。
            {
                _referenceAbsolutePathRoot = GetOriginRootPath(_referenceAbsolutePath);
                _absolutePathRoot = GetOriginRootPath(_absolutePath);

                string _rap_part, _ap_part;
                int _rap_i = _referenceAbsolutePathRoot.Length, _ap_i = _absolutePathRoot.Length;

                while (true)
                {
                    if (_rap_i >= _referenceAbsolutePath.Length || _ap_i >= _absolutePath.Length)
                    {
                        break;
                    }
                    _rap_part = GetNextPart(_referenceAbsolutePath, _rap_i);
                    _ap_part = GetNextPart(_absolutePath, _ap_i);
                    if (!IsDirectoryNameEquals(_rap_part, _ap_part))
                    {
                        break;
                    }
                    _rap_i += _rap_part.Length;
                    _ap_i += _ap_part.Length;
                }

                return ReplaceDirectorySeparator(GetRelativePath(_referenceAbsolutePath, _rap_i, _absolutePath, _ap_i), directorySeparator);
            }
            else
            {
                return ReplaceDirectorySeparator(absolutePath,directorySeparator);
            }
        }
        private static string GetNextPart(string _path, int _i)
        {
            string r = Regex.Match(_path.Substring(_i), "^(.*?)[\\\\/]").Value;
            if (r == "")
            {
                r = _path.Substring(_i);
            }
            //if (r == "")
            //{
            //    return null;
            //}
            return r;
        }
        /// <remarks>
        /// 仅考虑单个目录名“a”或“a/”不考虑“a//”。不过显然“C://”==“C:/”，但是“C:/dsadasd//”!=“D:/dsadasd//”，所以无法判断“dsadasd//”。
        /// </remarks>
        private static bool IsDirectoryNameEquals(string x, string y)
        {
            char xEnd = x[x.Length - 1], yEnd = y[y.Length - 1];
            string _x = (xEnd == '\\' || xEnd == '/') ? x.Substring(0, x.Length - 1) : x;
            string _y = (yEnd == '\\' || yEnd == '/') ? y.Substring(0, y.Length - 1) : y;
            return String.Equals(_x, _y);
        }
        private static string GetRelativePath(string referenceAbsolutePath, int _rap_i, string absolutePath, int _ap_i)
        {
            int i;

            if (_rap_i == referenceAbsolutePath.Length && _ap_i == absolutePath.Length)
            {
                if (IsFile(absolutePath))
                {
                    return GetLastPart(absolutePath);
                }
                else
                {
                    return "./";
                }
            }

            StringBuilder r = new StringBuilder();
            int rap_partCount = GetPartAmount(referenceAbsolutePath,_rap_i);
            for (i = 0; i < rap_partCount; i++)
            {
                r.Append("../");
            }
            string ap_diff = absolutePath.Substring(_ap_i);
            r.Append(ap_diff);

            return r.ToString();
        }
        private static int GetPartAmount(string absolutePath, int begin)
        {
            int i, count;

            char temp = absolutePath[absolutePath.Length - 1];
            if (temp == '\\' || temp == '/')
            {
                i = absolutePath.Length - 2;
            }
            else
            {
                i = absolutePath.Length - 1;
            }

            count = 0;
            for (; i >= begin; i--)
            {
                temp = absolutePath[i];

                if (temp == '\\' || temp == '/')
                {
                    count++;
                }
            }
            if (begin < absolutePath.Length)
            {
                temp = absolutePath[begin];
                if (temp != '\\' || temp != '/')
                {
                    count++;
                }
            }

            if (IsFile(absolutePath))
            {
                count--;
            }

            return count;
        }
        public static string GetLastPart(string path)
        {
            char lastChar = path[path.Length - 1], temp;
            int i;

            if(lastChar=='\\'|| lastChar == '/')
            {
                i = path.Length - 2;
            }
            else
            {
                i = path.Length - 1;
            }
            for (; i >= 0; i--)
            {
                temp = path[i];
                if (temp == '\\' || temp == '/')
                {
                    break;
                }
            }
            return path.Substring(i + 1);
        }

        public static string NormalizeHeader(string path)
        {
            Regex regex = new Regex("^[A-Za-z]:[\\\\/]([\\\\/]+)");

            if (IsAbsolutePath(path, false))
            {
                Match match = regex.Match(path);
                if (match.Success)
                {
                    return path.Remove(3, match.Groups[1].Value.Length); //将“C://”替换为“C:/”
                }
            }

            return path;
        }

        public static string GetAbsolutePath(string basicAbsolutePath, string relativePath, DirectorySeparator directorySeparator, bool carePathExists)
        {
            if (carePathExists && (!IsExists(basicAbsolutePath)))
            {
                throw new PathNotFoundException(String.Format("\"{0}\" not found.", basicAbsolutePath));
            }
            if (!IsAbsolutePath(basicAbsolutePath, true))
            {
                throw new PathNotFoundException(String.Format("\"{0}\" isn't an absolute path.", basicAbsolutePath));
            }

            //string _basicAbsolutePath;
            //if (IsFile(basicAbsolutePath))
            //{
            //    _basicAbsolutePath = GetDirctionary(basicAbsolutePath);
            //}
            //else
            //{
            //    _basicAbsolutePath = basicAbsolutePath;
            //}
            string _basicAbsolutePath = GetDirectory(basicAbsolutePath);

            return Combine(_basicAbsolutePath, relativePath, directorySeparator);
        }

        public static string GetDirectory(string path)
        {
            if (IsFile(path))
            {
                //int i, lastSlashIndex = 0;
                //
                //for (i = 0; i < path.Length; i++)
                //{
                //    if (path[i] == '\\' || path[i] == '/')
                //    {
                //        lastSlashIndex = i;
                //    }
                //}
                //
                //return path.Substring(0, lastSlashIndex + 1);
                int i;
                
                for (i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i] == '\\' || path[i] == '/')
                    {
                        return path.Substring(0, i + 1);
                    }
                }

                return "";
            }

            return path;
        }

        /// <remarks>
        /// 视parentPath最后一项为文件夹，无论是否为文件。
        /// </remarks>
        public static string Combine(string parentPath, string childPath, DirectorySeparator directorySeparator)
        {
            if(IsAbsolutePath(childPath, true))
            {
                return ReplaceDirectorySeparator(childPath, directorySeparator);
            }

            char lastChar = parentPath[parentPath.Length - 1];
            StringBuilder r = new StringBuilder(parentPath, parentPath.Length + childPath.Length + 1); //最多加一个连接符

            if (lastChar != '\\' && lastChar != '/')
            {
                if (directorySeparator == DirectorySeparator.DoNotCare)
                {
                    r.Append(System.IO.Path.DirectorySeparatorChar);
                }
                else
                {
                    if (directorySeparator == DirectorySeparator.Slash)
                    {
                        r.Append('\\');
                    }
                    else
                    {
                        r.Append('/');
                    }
                }
            }

            r.Append(childPath);

            return ReplaceDirectorySeparator(r.ToString(), directorySeparator);
        }

        /// <summary>
        /// 处理../。
        /// </summary>
        /// <remarks>
        /// 如果文件不存在或者IsIsAbsolutePath的结果为false则返回原path。
        /// </remarks>
        public static string GetShortPath(string path, bool dependOnOperatingSystem)
        {
            if (!IsAbsolutePath(path, dependOnOperatingSystem))
            {
                return path;
            }

            if (IsDirectory(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return directoryInfo.FullName;
            }
            else if (IsFile(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.FullName;
            }
            else
            {
                return path;
            }
        }

        public static string GetRootPath(string path)
        {
            char firstChar = path[0];

            if (firstChar == '\\' || firstChar == '/')
            {
                return new string(firstChar, 1);
            }

            if (Regex.IsMatch(path, "^[A-Za-z]:")) //不考虑如"C:Dir"这样的错误
            {
                return String.Format("{0}:/", firstChar);
            }

            return null;
        }

        public static string GetOriginRootPath(string path)
        {
            return Regex.Match(path, "^([A-Za-z]:|[\\\\/])[\\\\/]*").Value;
        }

        /// <remarks>
        /// false不代表完全否定，可能是因为找不到路径。
        /// </remarks>
        public static bool IsDirectory(string path)
        {
            char lastChar = path[path.Length - 1];

            if (lastChar == '\\' || lastChar == '/')
            {
                return true;
            }

            return Directory.Exists(path);
        }

        /// <remarks>
        /// false不代表完全否定，可能是因为找不到路径。
        /// </remarks>
        public static bool IsFile(string path)
        {
            char lastChar = path[path.Length - 1];

            if (lastChar == '\\' || lastChar == '/')
            {
                return false;
            }

            return File.Exists(path);
        }

        /// <summary>
        /// 是否存在目录或文件。
        /// </summary>
        public static bool IsExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static bool IsAbsolutePath(string path, bool dependOnOperatingSystem)
        {
            char firstChar = path[0];

            if (firstChar == '\\' || firstChar == '/')
            {
                return true;
            }
            if(Regex.IsMatch(path,"^[A-Za-z]:")) //不考虑如"C:Dir"这样的错误
            {
                if (dependOnOperatingSystem)
                {
                    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public static string ReplaceDirectorySeparator(string path, DirectorySeparator directorySeparator)
        {
            if (directorySeparator == DirectorySeparator.DoNotCare)
            {
                return path;
            }
            else
            {
                if (directorySeparator == DirectorySeparator.Slash)
                {
                    return path.Replace('/', '\\');
                }
                else
                {
                    return path.Replace('\\', '/');
                }
            }
        }

        public static bool IsDirectorySeparatorChar(char value)
        {
            return value == '\\' || value == '/';
        }
    }
}
