using MySql.Data.MySqlClient;
using penCsharpener.ImapData;
using penCsharpener.Mail2DB;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace penCsharpener.ImapBackup {
    public class ModelImapBackup {

        public DbCredentials ConnectionString { get; private set; }
        public bool IsWorking { get; private set; }
        private string[] mailFolders = { "inbox", "sent", "archive" };
        private QueryFactory db;

        public ModelImapBackup() {

        }

        public async Task<QueryFactory> GetQueryFactory() {
            ConnectionString ??= await InfoGetter.GetDbCredentials();
            var dbCon = new MySqlConnection(ConnectionString.ToString());
            var compiler = new MySqlCompiler();
            var db = new QueryFactory(dbCon, compiler);

            SqlBase<DataMessage>.TableName = "imap_backup.messages";
            SqlBase<DataAttachment>.TableName = "imap_backup.attachments";
            SqlBase<DataContact>.TableName = "imap_backup.contacts";
            SqlBase<DataAccount>.TableName = "imap_backup.mail_accounts";

            return db;
        }

        public async Task GetMail() {
            IsWorking = true;
            db = await GetQueryFactory();
            // get decryption key for passwords
            var decryptKey = await File.ReadAllTextAsync(".decryptkey");
            // get all accounts
            var accounts = await db.Query(SqlBase<DataAccount>.TableName).GetAsync<DataAccount>();
            //var encPwd = accounts.FirstOrDefault(x => x.Id == 5);
            //    encPwd.PasswordEncrypted = "".Encrypt(decryptKey);
            //await db.Query(SqlBase<DataAccount>.TableName).Where(nameof(encPwd.Id), encPwd.Id).UpdateAsync(new { encPwd.PasswordEncrypted });
            foreach (var account in accounts.Where(x => x.Id > 1)) {
                var imapCreds = account.ToCreds(decryptKey);
                await ProcessAccount(imapCreds, account);
            }
            IsWorking = false;
        }

        private bool InvalidMailfolder(string mailFolder) {
            return mailFolder == "REPLY" || mailFolder == "[Google Mail]" || mailFolder == "[Gmail]"
                    || mailFolder == "Deleted" || mailFolder == "Drafts" || mailFolder == "Junk" || mailFolder == "Outbox";
        }

        private async Task ProcessAccount(Credentials imapCreds, DataAccount account) {
            var mail2db = new Client(imapCreds);
            var converter = new MailTypeConverter(mail2db);
            var availableFolders = await mail2db.GetMailFolders();
            var folders = mailFolders.IntersectCaseIgnore(availableFolders).ToList();
            var filter = new ImapFilter();
            foreach (var mailFolder in folders) {
                if (InvalidMailfolder(mailFolder)) continue;

                try {
                mail2db.SetMailFolder(mailFolder);
                var countInFolder = await mail2db.GetTotalMailCount();
                if (mail2db.OpenedMailFolder.IsNullOrEmpty()) {
                    continue;
                }
                Console.WriteLine($"{account.EmailAddress}  mail folder: '{mail2db.OpenedMailFolder}'");
                Console.WriteLine($"Total Count: {countInFolder}");
                    var savedMessages = await db.Query(SqlBase<DataMessage>.TableName)
                                    .Select(nameof(DataMessage.EmailAccountId), nameof(DataMessage.UId), nameof(DataMessage.MailFolder))
                                    .Where(new { EmailAccountId = account.Id, MailFolder = mailFolder })
                                    .GetAsync<DataMessage>();

                    Console.WriteLine("Existing Uids: {0}", savedMessages.Count());
                    Console.WriteLine("===================\n");

                    converter.UIdsToExclude = savedMessages.Select(x => x.UId).ToArray();

                    async Task ProcessImapMessage(ImapMessage item) {
                        //var TA = db.Connection.BeginTransaction();
                        Console.WriteLine("Saving: {0}\t{1}\"{2}\"", item?.ReceivedAtLocal,
                                                                     SpacePadding(item?.From?.EmailAddress),
                                                                     item?.Subject);
                        try {
                            if (!savedMessages.MessageExists(item.UId, item.MailFolder, account.Id)) {
                                var msgId = await db.Query(SqlBase<DataMessage>.TableName).InsertGetIdAsync<uint>(item.ToData(account.Id));
                                foreach (var contact in item.ToDataContact(msgId)) {
                                    await db.Query(SqlBase<DataContact>.TableName).InsertAsync(contact);
                                }
                                if (item.HasAttachments) {
                                    foreach (var file in item.ToAttachment(msgId)) {
                                        Console.WriteLine("Saving Attachment: {0}", file.Filename);
                                        await db.Query(SqlBase<DataAttachment>.TableName).InsertAsync(file);
                                    }
                                }
                            }
                            //TA.Commit();
                        } catch (Exception ex) {
                            //TA.Rollback();
                            Console.WriteLine(ex.Message);
                        }
                    }

                    await converter.GetMessagesAsync(ProcessImapMessage, filter);

                } catch (Exception ex2) {
                    Console.WriteLine(ex2.Message);
                }
                GC.Collect();
                Console.WriteLine("\n");
            }
        }

        private string SpacePadding(string mailaddress) {
            return mailaddress.PadRight(40, ' ');
        }
    }
}
