using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

// PostgreDB
using Npgsql;


namespace BPMSDataBases
{

    /// <summary>
    /// データベースコネクター
    /// シングルトンクラスであり、一度作成したDB接続オブジェクトは
    /// 本クラスが削除されるまで接続状態のまま保持される
    /// </summary>
    public class DBConnection : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// データベースに接続する
        /// </summary>
        /// <returns>リストのEnumerator</returns>
        /// <param name="server">データベースサーバー名またはIPアドレス</param>
        /// <param name="port">ポート番号</param>
        /// <param name="user">ログインユーザー名</param>
        /// <param name="pass">ログインパスワード</param>
        /// <param name="dbName">データベース名</param>
        public DBConnection(String server = null, String port = null, String user = null, String pass = null, String dbName = null)
        {

            //データベース接続
            //var connString = @"Server=localhost;Port=5432;User Id=postgres;Password=rootroot;Database=postgres";
            //var connString = @"Server=localhost;Port=5432;User Id=test;Password=test;Database=testdb";
            var connString = "Server=" + SelectDBParamItem(server, defaultServer) + ";"
                           + "Port=" + SelectDBParamItem(port, defaultPort) + ";"
                           + "User Id=" + SelectDBParamItem(user, defaultUser) + ";"
                           + "Password=" + SelectDBParamItem(user, defaultPass) + ";"
                           + "Database=" + SelectDBParamItem(dbName, defaultDBName);
            _conn = new NpgsqlConnection(connString);
        }

        /// <summary>
        /// デストラクタ
        /// データベースを切断する
        /// </summary>
        ~DBConnection()
        {
            // データベース切断
            Dispose(false);
        }

        /// <summary>
        /// DBを接続する
        /// </summary>
        /// <returns>なし</returns>
        public void Open()
        {
            _conn.Open();
        }

        /// <summary>
        /// データベースを切断する
        /// </summary>
        public void Close()
        {
            // データベース切断
            if (_conn != null && _conn.State != ConnectionState.Closed)
            {
                _conn.Close();
            }
        }

        /// <summary>
        /// DB接続オブジェクトを取得する
        /// </summary>
        /// <returns> DB接続オブジェクト</returns> 
        public NpgsqlConnection GetConnection()
        {
            return _conn;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        // 概要:
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// データベース接続オブジェクトを破棄する
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            Close();
            if (disposing)
            {
                if (_conn != null)
                {
                    _conn.Dispose();
                    _conn = null;
                }
            }

        }

        private String SelectDBParamItem(String item, String defaultItem)
        {
            return String.IsNullOrEmpty(item) ? defaultItem : item;
        }

        //private static DBConnection _singleInstance = new DBConnection();   // シングルトン
        private NpgsqlConnection _conn = null;                // DB接続オブジェクト

        private String defaultServer = @"localhost";
        private String defaultPort = @"5432";
        private String defaultDBName = @"testdb";
        private String defaultUser = @"test";
        private String defaultPass = @"test";
    }

    /// <summary>
    /// データベース操作を行うクラス
    ///   暫定でPostgreを利用
    /// </summary>
    class DBHandler
    {
    }
}
