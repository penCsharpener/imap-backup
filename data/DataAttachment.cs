using penCsharpener.Mail2DB;
using SqlKata;
using System.IO;

namespace penCsharpener.ImapData {
    public class DataAttachment : SqlBase<DataAttachment>, IDbClass {

        public uint Id { get; set; }
        public uint MessageId { get; set; }
        public string Filename { get; set; }
        [Ignore]
        public long Filesize => FileContent.LongLength;
        public byte[] FileContent { get; set; }
        private string _sha256Hash;
        public string Sha256Hash { get => FileContent?.ToSha256(); set => _sha256Hash = value; }
        public string Subfolder { get; protected set; }
        [Ignore]
        public string FullPath { get; set; }
        [Ignore]
        public FileInfo FileInfo => GetFileInfo();

        private FileInfo GetFileInfo() {
            if (File.Exists(FullPath)) {
                return new FileInfo(FullPath);
            }
            return null;
        }

        public override string ToString() {
            return $"{Filename} {Filesize} bytes";
        }
    }
}
