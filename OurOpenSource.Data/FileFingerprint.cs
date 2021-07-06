using System;
using System.IO;
using System.Security.Cryptography;

using OurOpenSource.Utility;

namespace OurOpenSource.Data
{
    /// <summary>
    /// 文件指纹。
    /// File fingerprint.
    /// </summary>
    public class FileFingerprint
    {
        /// <summary>
        /// SHA1类。
        /// Class SHA1.
        /// </summary>
        private static SHA1 sha1 = SHA1.Create();
        /// <summary>
        /// SHA256类。
        /// Class SHA256.
        /// </summary>
        private static SHA256 sha256 = SHA256.Create();
        /// <summary>
        /// MD5类。
        /// Class MD5.
        /// </summary>
        private static MD5 md5 = MD5.Create();

        /// <summary>
        /// 该文件指纹包含的校验值类型。
        /// Checksum types include this file fingerprint.
        /// </summary>
        private FileFingerprintChecksumType fileFingerprintChecksumType;
        /// <summary>
        /// 该文件指纹包含的校验值类型。
        /// Checksum types include this file fingerprint.
        /// </summary>
        public FileFingerprintChecksumType FileFingerprintChecksumType { get { return fileFingerprintChecksumType; } }

        /// <summary>
        /// SHA1校验值。
        /// SHA1 checksum.
        /// </summary>
        private byte[] sha1Code;
        /// <summary>
        /// SHA1校验值。
        /// SHA1 checksum.
        /// </summary>
        public byte[] SHA1Code { get { return sha1Code; } }
        /// <summary>
        /// SHA256校验值。
        /// SHA256 checksum.
        /// </summary>
        private byte[] sha256Code;
        /// <summary>
        /// SHA256校验值。
        /// SHA256 checksum.
        /// </summary>
        public byte[] SHA256Code { get { return sha256Code; } }
        /// <summary>
        /// MD5校验值。
        /// MD5 checksum.
        /// </summary>
        private byte[] md5Code;
        /// <summary>
        /// MD5校验值。
        /// MD5 checksum.
        /// </summary>
        public byte[] MD5Code { get { return md5Code; } }

        /// <summary>
        /// 将文件指纹转化为Base64格式的字符串。
        /// Convert file fingerprint to Base64 string.
        /// </summary>
        /// <param name="fileFingerprint">
        /// 需要被转化的文件指纹。
        /// A file fingerprint need to convert.
        /// </param>
        /// <returns>
        /// 转化后的base64字符串。
        /// Converted Base64 string.
        /// </returns>
        public static string ToBase64(FileFingerprint fileFingerprint)
        {
            byte[] type = BitConverter.GetBytes((ushort)fileFingerprint.FileFingerprintChecksumType);
            int count = type.Length;

            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA1))
            {
                count += 20;
            }
            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA256))
            {
                count += 32;
            }
            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.MD5))
            {
                count += 16;
            }

            byte[] r = new byte[count];
            Array.Copy(type, 0, r, 0, type.Length);
            count = type.Length;
            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA1))
            {
                Array.Copy(fileFingerprint.SHA1Code, 0, r, count, 20);
                count += 20;
            }
            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA256))
            {
                Array.Copy(fileFingerprint.SHA256Code, 0, r, count, 32);
                count += 32;
            }
            if (fileFingerprint.FileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.MD5))
            {
                Array.Copy(fileFingerprint.MD5Code, 0, r, count, 16);
            }

            return Convert.ToBase64String(r);
        }

        /// <summary>
        /// 将可识别为存储有文件指纹内容的Base64字符串转化为文件指纹。
        /// Convert Base64 string which representing file fingerprint to file fingerprint.
        /// </summary>
        /// <param name="base64">
        /// 需要被转化的Base64字符串。
        /// A Base64 string need to onvert.
        /// </param>
        /// <returns>
        /// 转化后的文件指纹。
        /// Converted file fingerprint.
        /// </returns>
        public static FileFingerprint FromBase64(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            byte[] typeBytes = BitConverter.GetBytes(sizeof(ushort));
            Array.Copy(data, 0, typeBytes, 0, 2);
            FileFingerprintChecksumType type = (FileFingerprintChecksumType)BitConverter.ToUInt16(typeBytes);

            byte[] sha1Code;
            byte[] sha256Code;
            byte[] md5Code;
            int i = 2;
            if (type.HasFlag(FileFingerprintChecksumType.SHA1))
            {
                sha1Code = new byte[20];
                Array.Copy(data, i, sha1Code, 0, 20);
                i += 20;
            }
            else
            {
                sha1Code = null;
            }
            if (type.HasFlag(FileFingerprintChecksumType.SHA256))
            {
                sha256Code = new byte[32];
                Array.Copy(data, i, sha256Code, 0, 32);
                i += 32;
            }
            else
            {
                sha256Code = null;
            }
            if (type.HasFlag(FileFingerprintChecksumType.MD5))
            {
                md5Code = new byte[16];
                Array.Copy(data, i, md5Code, 0, 16);
            }
            else
            {
                md5Code = null;
            }

            return new FileFingerprint(sha1Code, sha256Code, md5Code);
        }

        /// <summary>
        /// 用该指纹与另一个文件的指纹进行比较。
        /// Use this file gingerprint to compare with another one of a file.
        /// </summary>
        /// <param name="path">
        /// 另一个文件的路径。
        /// Another file path.
        /// </param>
        /// <param name="fileFingerprint">
        /// 由另一个文件生成的指纹。
        /// Generated file fingerprint of another file.
        /// </param>
        /// <param name="fileFingerprintChecksumType">
        /// 需要比较的文件指纹类型。
        /// Necessary file fingerprint types for comparing.
        /// </param>
        /// <returns>
        /// 两个文件指纹是否相同。
        /// Are two file fingerprint same.
        /// </returns>
        public bool CheckFile(string path, out FileFingerprint fileFingerprint, FileFingerprintChecksumType fileFingerprintChecksumType)
        {
            fileFingerprint = new FileFingerprint(path);
            return IsFileFingerprintSame(this, fileFingerprint, fileFingerprintChecksumType);
        }

        /// /// <summary>
        /// 取两个指纹共同指纹类型进行比较。
        /// Take the intersection of the two file fingerprint to compare.
        /// </summary>
        /// <remarks>
        /// 当没有共同的指纹类型是将抛出`ArgumentException`。
        /// </remarks>
        /// <param name="fileFingerprint1">
        /// 文件指纹1。
        /// File fingerprint 1.
        /// </param>
        /// <param name="fileFingerprint2">
        /// 文件指纹2。
        /// File fingerprint 2.
        /// </param>
        /// <returns>
        /// 两个文件指纹是否相同。
        /// Are two file fingerprint same.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// 当两个文件指纹交集为`FileFingerprintChecksumType.None`时将抛出`ArgumentException`。
        /// Throw `ArgumentException` when the intersection of the two file fingerprint is FileFingerprintChecksumType.None.
        /// <seealso cref="IsFileFingerprintSame(FileFingerprint, FileFingerprint, FileFingerprintChecksumType)"/>
        /// 有些异常不列在此处。
        /// Some exceptions don't list here.
        /// </exception>
        public bool IsFileFingerprintSameMin(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2)
        {
            FileFingerprintChecksumType min = fileFingerprint1.FileFingerprintChecksumType & fileFingerprint2.FileFingerprintChecksumType;
            if((min & FileFingerprintChecksumType.All) == FileFingerprintChecksumType.None)
            {
                throw new ArgumentException("The fileFingerprint1.FileFingerprintChecksumType & fileFingerprint2.FileFingerprintChecksumType == FileFingerprintChecksumType.None");
            }
            return IsFileFingerprintSame(fileFingerprint1, fileFingerprint2, min);
        }

        /// <summary>
        /// 比较两个文件指纹是否相同。
        /// Compare two file fingerprints wheater they are same.
        /// </summary>
        /// <param name="fileFingerprint1">
        /// 文件指纹1。
        /// File fingerprint 1.
        /// </param>
        /// <param name="fileFingerprint2">
        /// 文件指纹2。
        /// File fingerprint 2.
        /// </param>
        /// <param name="fileFingerprintChecksumType">
        /// 需要比较的文件指纹类型。
        /// Necessary file fingerprint types for comparing.
        /// </param>
        /// <returns>
        /// 两个文件指纹是否相同。
        /// Are two file fingerprint same.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// 当两个文件指纹中有至少有一个没有包含所要求的指纹类型或`fileFingerprintChecksumType`为`FileFingerprintChecksumType.None`时将抛出`ArgumentException`。
        /// Throw `ArgumentException` when at least one file fingerprint of two haven't as same fingerprint type as `fileFingerprintChecksumType` or `fileFingerprintChecksumType` is `FileFingerprintChecksumType.None`.
        /// 有些异常不列在此处。
        /// Some exceptions don't list here.
        /// </exception>
        public bool IsFileFingerprintSame(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2, FileFingerprintChecksumType fileFingerprintChecksumType)
        {
            if (fileFingerprintChecksumType == FileFingerprintChecksumType.None)
            {
                throw new ArgumentException("Can't be FileFingerprintChecksumType.None.", "fileFingerprintChecksumType");
            }
            if ((fileFingerprint1.FileFingerprintChecksumType&fileFingerprintChecksumType)!=fileFingerprintChecksumType)
            {
                throw new ArgumentException("FileFingerprint lost some fileFingerType.", "fileFingerprint1");
            }
            if ((fileFingerprint2.FileFingerprintChecksumType & fileFingerprintChecksumType) != fileFingerprintChecksumType)
            {
                throw new ArgumentException("FileFingerprint lost some fileFingerType.", "fileFingerprint2");
            }

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA1))
            {
                if (ComparerMethods.ComapreSameLengthByteArray(fileFingerprint1.sha1Code, fileFingerprint2.sha1Code) != 0)
                {
                    return false;
                }
            }

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA256))
            {
                if (ComparerMethods.ComapreSameLengthByteArray(fileFingerprint1.sha256Code, fileFingerprint2.sha256Code) != 0)
                {
                    return false;
                }
            }

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.MD5))
            {
                if (ComparerMethods.ComapreSameLengthByteArray(fileFingerprint1.md5Code, fileFingerprint2.md5Code) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 直接设置文件指纹的值。
        /// Directly set file fingerprint.
        /// </summary>
        /// <param name="sha1Code">
        /// SHA1的值。
        /// SHA1 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        /// <param name="sha256Code">
        /// SHA256的值。
        /// SHA256 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        /// <param name="md5Code">
        /// MD5的值。
        /// MD5 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        public FileFingerprint(byte[] sha1Code = null, byte[] sha256Code = null, byte[] md5Code = null)
        {
            DirectSet(sha1Code, sha256Code, md5Code);
        }
        /// <summary>
        /// 直接设置文件指纹的值。
        /// Directly set file fingerprint.
        /// </summary>
        /// <param name="sha1Code">
        /// SHA1的值。
        /// SHA1 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        /// <param name="sha256Code">
        /// SHA256的值。
        /// SHA256 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        /// <param name="md5Code">
        /// MD5的值。
        /// MD5 value.
        /// 如果没有可以设置为null。
        /// Please set null if nano.
        /// </param>
        private void DirectSet(byte[] sha1Code, byte[] sha256Code, byte[] md5Code)
        {
            this.fileFingerprintChecksumType = FileFingerprintChecksumType.None;

            if (sha1Code != null)
            {
                if(sha1Code.Length != 20)
                {
                    throw new ArgumentException("SHA1 code length must be 20.", "sha1Code");
                }
                this.fileFingerprintChecksumType |= FileFingerprintChecksumType.SHA1;
            }
            if (sha256Code != null)
            {
                if(sha256Code.Length != 32)
                {
                    throw new ArgumentException("SHA256 code length must be 32.", "sha1Code");
                }
                this.fileFingerprintChecksumType |= FileFingerprintChecksumType.SHA256;
            }
            if (md5Code != null)
            {
                if(md5Code.Length != 16)
                {
                    throw new ArgumentException("MD5 code length must be 16.", "sha1Code");
                }
                this.fileFingerprintChecksumType |= FileFingerprintChecksumType.MD5;
            }
            this.sha1Code = sha1Code;
            this.sha256Code = sha256Code;
            this.md5Code = md5Code;
        }
        /// <summary>
        /// 从文件获取校验值。
        /// Get checksum from file.
        /// </summary>
        /// <param name="path">
        /// 目标文件的路径。
        /// the path of target file.
        /// </param>
        /// <param name="fileFingerprintChecksumType">
        /// 需要的文件指纹类型。
        /// Necessary file fingerprint types.
        /// </param>
        public FileFingerprint(string path, FileFingerprintChecksumType fileFingerprintChecksumType = FileFingerprintChecksumType.All)
        {
            FromFile(path, fileFingerprintChecksumType);
        }
        /// <summary>
        /// 从文件获取校验值。
        /// Get checksum from file.
        /// </summary>
        /// <param name="path">
        /// 目标文件的路径。
        /// the path of target file.
        /// </param>
        /// <param name="fileFingerprintChecksumType">
        /// 需要的文件指纹类型。
        /// Necessary file fingerprint types.
        /// </param>
        private void FromFile(string path, FileFingerprintChecksumType fileFingerprintChecksumType)
        {
            this.fileFingerprintChecksumType = fileFingerprintChecksumType;

            if (fileFingerprintChecksumType == FileFingerprintChecksumType.None)
            {
                throw new ArgumentException("Can't be None.", "fileFingerprintChecksumType");
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

            bool moved = false;

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA1))
            {
                sha1Code = sha1.ComputeHash(fileStream);
                moved = true;
            }
            else
            {
                sha1Code = null;
            }

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.SHA256))
            {
                
                if (moved)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    moved = true;
                }
                sha256Code = sha256.ComputeHash(fileStream);
            }
            else
            {
                sha256Code = null;
            }

            if (fileFingerprintChecksumType.HasFlag(FileFingerprintChecksumType.MD5))
            {
                if (moved)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
                md5Code = md5.ComputeHash(fileStream);
            }
            else
            {
                md5Code = null;
            }

            fileStream.Close();
        }
    }
}
