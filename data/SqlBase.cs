using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace penCsharpener.ImapData {
    public class SqlBase<T> {

        public static QueryFactory Db;
        public static string TableName;

        public async Task<R> WriteToDb<R>() {
            return await Db.Query(TableName).InsertGetIdAsync<R>(this);
        }

        public static async Task<IEnumerable<T>> GetList() {
            return await Db.Query(TableName).GetAsync<T>();
        }


    }
}
