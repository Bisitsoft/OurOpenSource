using System;
using System.IO;
using System.Security.Cryptography;

namespace OurOpenSource.Data
{
    public enum FileFingerprintType : ushort
    {
        None = 0x0000,

        MD5 = 0x0001,
        SHA1 = 0x0010,
        SHA256 = 0x0100,

        All = MD5 | SHA1 | SHA256
    }

    public class FileFingerprint
    {
        private FileFingerprintType fileFingerprintType;
        public FileFingerprintType FileFingerprintType { get { return fileFingerprintType; } }

        private byte[] sha1Code;
        public byte[] SHA1Code { get { return sha1Code; } }
        private byte[] sha256Code;
        public byte[] SHA256Code { get { return sha256Code; } }
        private byte[] md5Code;
        public byte[] MD5Code { get { return md5Code; } }

        public static string ToBase64(FileFingerprint fileFingerprint)
        {
            int count = 2;

            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.SHA1))
            {
                count += 20;
            }
            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.SHA256))
            {
                count += 32;
            }
            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.MD5))
            {
                count += 16;
            }

            byte[] r = new byte[count];
            byte[] type = BitConverter.GetBytes((ushort)fileFingerprint.FileFingerprintType);
            Array.Copy(type, 0, r, 0, type.Length);
            count = 2;
            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.SHA1))
            {
                Array.Copy(fileFingerprint.SHA1Code, 0, r, count, 20);
                count += 20;
            }
            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.SHA256))
            {
                Array.Copy(fileFingerprint.SHA256Code, 0, r, count, 32);
                count += 32;
            }
            if (fileFingerprint.FileFingerprintType.HasFlag(FileFingerprintType.MD5))
            {
                Array.Copy(fileFingerprint.MD5Code, 0, r, count, 16);
            }

            return Convert.ToBase64String(r);
        }

        public static FileFingerprint FromBase64(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            byte[] typeBytes = new byte[2];
            Array.Copy(data, 0, typeBytes, 0, 2);
            FileFingerprintType type = (FileFingerprintType)BitConverter.ToUInt16(typeBytes);

            byte[] sha1Code = null;
            byte[] sha256Code = null;
            byte[] md5Code = null;
            int i = 2;
            if (type.HasFlag(FileFingerprintType.SHA1))
            {
                Array.Copy(data, i, sha1Code, 0, 20);
                i += 20;
            }
            if (type.HasFlag(FileFingerprintType.SHA256))
            {
                Array.Copy(data, i, sha256Code, 0, 32);
                i += 32;
            }
            if (type.HasFlag(FileFingerprintType.MD5))
            {
                Array.Copy(data, i, md5Code, 0, 16);
            }

            return new FileFingerprint(sha1Code, sha256Code, md5Code);
        }

        /// <summary>
        /// 用该指纹与另一个文件的指纹进行比较。
        /// </summary>
        /// <param name="path">另一个文件的路径。</param>
        /// <param name="fileFingerprint">由另一个文件生成的指纹。</param>
        /// <param name="fileFingerprintType">比较的指纹类型。</param>
        public bool CheckFile(string path, out FileFingerprint fileFingerprint, FileFingerprintType fileFingerprintType)
        {
            fileFingerprint = new FileFingerprint(path);
            return IsFileFingerprintSame(this, fileFingerprint, fileFingerprintType);
        }

        /// <summary>
        /// 取两个指纹共同指纹类型进行比较。
        /// </summary>
        /// <remarks>
        /// 当没有共同的指纹类型是将抛出`ArgumentException`。
        /// </remarks>
        public bool IsFileFingerprintSameMin(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2)
        {
            FileFingerprintType min = fileFingerprint1.FileFingerprintType & fileFingerprint2.FileFingerprintType;
            if((min& FileFingerprintType.All) == FileFingerprintType.None)
            {
                throw new ArgumentException("The fileFingerprint1.FileFingerprintType & fileFingerprint2.FileFingerprintType == FileFingerprintType.None");
            }
            return IsFileFingerprintSame(fileFingerprint1, fileFingerprint2, min);
        }

        /// <remarks>
        /// 当没有共同的指纹类型是将抛出`ArgumentException`。
        /// </remarks>
        public bool IsFileFingerprintSame(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2, FileFingerprintType fileFingerprintType)
        {
            if (fileFingerprintType == FileFingerprintType.None)
            {
                throw new ArgumentException("Can't be FileFingerprintType.None.", "fileFingerprintType");
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.MD5) && fileFingerprint1.md5Code == fileFingerprint2.md5Code)
            {
                if (fileFingerprint1.md5Code != fileFingerprint2.md5Code)
                {
                    return false;
                }
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA1))
            {
                if (fileFingerprint1.sha1Code != fileFingerprint2.sha1Code)
                {
                    return false;
                }
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA256))
            {
                if (fileFingerprint1.sha256Code != fileFingerprint2.sha256Code)
                {
                    return false;
                }
            }

            return true;
        }

        public FileFingerprint(byte[] sha1Code = null, byte[] sha256Code = null, byte[] md5Code = null)
        {
            DirectSet(sha1Code, sha256Code, md5Code);
        }
        private void DirectSet(byte[] sha1Code, byte[] sha256Code, byte[] md5Code)
        {
            this.fileFingerprintType = FileFingerprintType.None;

            if (sha1Code != null)
            {
                if(sha1Code.Length != 20)
                {
                    throw new ArgumentException("SHA1 code length must be 20.", "sha1Code");
                }
                this.fileFingerprintType |= FileFingerprintType.SHA1;
            }
            if (sha256Code != null)
            {
                if(sha256Code.Length != 32)
                {
                    throw new ArgumentException("SHA256 code length must be 32.", "sha1Code");
                }
                this.fileFingerprintType |= FileFingerprintType.SHA256;
            }
            if (md5Code != null)
            {
                if(md5Code.Length != 16)
                {
                    throw new ArgumentException("MD5 code length must be 16.", "sha1Code");
                }
                this.fileFingerprintType |= FileFingerprintType.MD5;
            }
            this.sha1Code = sha1Code;
            this.sha256Code = sha256Code;
            this.md5Code = md5Code;
        }
        public FileFingerprint(string path, FileFingerprintType fileFingerprintType = FileFingerprintType.All)
        {
            FromFile(path, fileFingerprintType);
        }
        private void FromFile(string path, FileFingerprintType fileFingerprintType)
        {
            this.fileFingerprintType = fileFingerprintType;

            if (fileFingerprintType == FileFingerprintType.None)
            {
                throw new ArgumentException("Can't be None.", "fileFingerprintType");
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

            bool moved = false;

            if (fileFingerprintType.HasFlag(FileFingerprintType.MD5))
            {
                md5Code = HMACMD5.Create().ComputeHash(fileStream);
                moved = true;
            }
            else
            {
                md5Code = null;
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA1))
            {
                if (moved)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    moved = true;
                }
                sha1Code = HMACSHA1.Create().ComputeHash(fileStream);
            }
            else
            {
                sha1Code = null;
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA256))
            {
                if (moved)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
                sha256Code = HMACSHA256.Create().ComputeHash(fileStream);
            }
            else
            {
                sha256Code = null;
            }

            fileStream.Close();
        }
    }
}
