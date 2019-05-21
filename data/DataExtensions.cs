using penCsharpener.Mail2DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace penCsharpener.ImapData {
    public static class DataExtensions {
        public static DataMessage ToData(this ImapMessage msg, uint emailAccountId) {
            return new DataMessage() {
                Body = msg.Body,
                BodyPlainText = msg.BodyPlainText,
                HasAttachments = msg.HasAttachments,
                InReplyToId = msg.InReplyToId,
                MailFolder = msg.MailFolder,
                MessageTextId = msg.MessageTextId,
                MimeMessageBytes = msg.MimeMessageBytes,
                ReceivedAtLocal = msg.ReceivedAtLocal,
                ReceivedAtUTC = msg.ReceivedAtUTC,
                Subject = msg.Subject,
                FromAddress = msg.From?.EmailAddress,
                UId = msg.UId,
                EmailAccountId = emailAccountId,
            };
        }

        public static DataContact[] ToDataContact(this ImapMessage msg, uint id) {
            var list = new List<DataContact>();
            foreach (var contact in msg.To) {
                list.Add(new DataContact() {
                    ContactName = contact.ContactName,
                    ContactType = ContactTypes.To,
                    EmailAddress = contact.EmailAddress,
                    MessageId = id,
                });
            }
            foreach (var contact in msg.Cc) {
                list.Add(new DataContact() {
                    ContactName = contact.ContactName,
                    ContactType = ContactTypes.Cc,
                    EmailAddress = contact.EmailAddress,
                    MessageId = id,
                });
            }
            list.Add(new DataContact() {
                ContactName = msg.From.ContactName,
                ContactType = ContactTypes.From,
                EmailAddress = msg.From.EmailAddress,
                MessageId = id,
            });
            return list.ToArray();
        }

        public static DataAttachment[] ToAttachment(this ImapMessage msg, uint id) {
            var list = new List<DataAttachment>();
            foreach (var att in msg.Attachments) {
                list.Add(new DataAttachment() {
                    FileContent = att.FileContent,
                    Filename = att.Filename,
                    MessageId = id,
                });
            }
            return list.ToArray();
        }

        public static Credentials ToCreds(this DataAccount account, string decryptKey) {
            return new Credentials() {
                EmailAddress = account.EmailAddress,
                Password = account.PasswordEncrypted.Decrypt(decryptKey),
                Port = account.ImapPort,
                ServerURL = account.ImapServer,
            };
        }

        public static bool MessageExists(this IEnumerable<DataMessage> msgs,
                                         uint uniqueId,
                                         string mailFolder,
                                         uint messageAccount) {
            return msgs.Any(x => x.UId == uniqueId
                              && x.MailFolder.Equals(mailFolder, System.StringComparison.OrdinalIgnoreCase)
                              && x.EmailAccountId == messageAccount);
        }

        public static IEnumerable<string> IntersectCaseIgnore(this IEnumerable<string> first, IEnumerable<string> second) {
            foreach (var f in first) {
                foreach (var s in second) {
                    if (f.Equals(s, StringComparison.OrdinalIgnoreCase)) {
                        yield return f;
                    }
                }
            }
        }
    }
}
