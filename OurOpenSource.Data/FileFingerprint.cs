using System;
using System.IO;
using System.Security.Cryptography;

namespace OurOpenSource.Data
{
    public class FileFingerprint
    {
        public enum FileFingerprintType
        {
            None = 0x0000_0000,

            MD5 = 0x0000_0001,
            SHA1 = 0x0000_0010,
            SHA256 = 0x0000_0100,

            All = MD5 | SHA1 | SHA256
        }

        private byte[] sha1Code;
        public byte[] SHA1Code { get { return sha1Code; } }
        private byte[] sha256Code;
        public byte[] SHA256Code { get { return sha256Code; } }
        private byte[] md5Code;
        public byte[] MD5Code { get { return md5Code; } }

        public bool CheckFile(string path, out FileFingerprint fileFingerprint)
        {
            fileFingerprint = new FileFingerprint(path);
            return IsFileFingerprintSame(this, fileFingerprint);
        }

        public bool CheckFile(string path, out FileFingerprint fileFingerprint, FileFingerprintType fileFingerprintType)
        {
            fileFingerprint = new FileFingerprint(path);
            return IsFileFingerprintSame(this, fileFingerprint, fileFingerprintType);
        }

        public bool IsFileFingerprintSame(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2)
        {
            return fileFingerprint1.sha1Code == fileFingerprint2.sha1Code &&
                fileFingerprint1.sha256Code == fileFingerprint2.sha256Code &&
                fileFingerprint1.md5Code == fileFingerprint2.md5Code;
        }

        public bool IsFileFingerprintSame(FileFingerprint fileFingerprint1, FileFingerprint fileFingerprint2, FileFingerprintType fileFingerprintType)
        {
            if (fileFingerprintType == FileFingerprintType.None)
            {
                throw new ArgumentException("Can't be None.", "fileFingerprintType");
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

        public FileFingerprint()
        {
            sha1Code = null;
            sha256Code = null;
            md5Code = null;
        }

        public FileFingerprint(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

            md5Code = HMACMD5.Create().ComputeHash(fileStream);

            fileStream.Seek(0, SeekOrigin.Begin);
            sha1Code = HMACSHA1.Create().ComputeHash(fileStream);

            fileStream.Seek(0, SeekOrigin.Begin);
            sha256Code = HMACSHA256.Create().ComputeHash(fileStream);

            fileStream.Close();
        }

        public FileFingerprint(string path, FileFingerprintType fileFingerprintType)
        {
            if (fileFingerprintType == FileFingerprintType.None)
            {
                throw new ArgumentException("Can't be None.", "fileFingerprintType");
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

            if (fileFingerprintType.HasFlag(FileFingerprintType.MD5))
            {
                md5Code = HMACMD5.Create().ComputeHash(fileStream);
            }
            else
            {
                md5Code = null;
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA1))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                sha1Code = HMACSHA1.Create().ComputeHash(fileStream);
            }
            else
            {
                sha1Code = null;
            }

            if (fileFingerprintType.HasFlag(FileFingerprintType.SHA256))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
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
