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
    /// データベース操作を行うクラス
    ///   暫定でPostgreを利用
    /// </summary>
    class DBHandler
    {
        /// <summary>
        /// データベースコネクター
        /// シングルトンクラスであり、一度作成したDB接続オブジェクトは
        /// 本クラスが削除されるまで接続状態のまま保持される
        /// </summary>
        public class Connection: IDisposable
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
            public Connection(String server, String port, String user, String pass, String dbName)
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
            ~Connection()
            {
                // データベース切断
                Close();
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
                    _conn.Close();
            }

            /// <summary>
            /// DB接続オブジェクトを取得する
            /// </summary>
            /// <returns> DB接続オブジェクト</returns> 
            public NpgsqlConnection GetConnection()
            {
                return _conn;
            }

            //
            // 概要:
            //     Performs application-defined tasks associated with freeing, releasing, or resetting
            //     unmanaged resources.
            public void Dispose()
            {
                this.Close();
                GC.SuppressFinalize(this);
            }

            //private static Connection _singleInstance = new Connection();   // シングルトン
            private  NpgsqlConnection _conn = null;                // DB接続オブジェクト

            private String SelectDBParamItem(String item, String defaultItem)
            {
                return String.IsNullOrEmpty(item) ? defaultItem : item;
            }

            private String defaultServer    = @"localhost";
            private String defaultPort      = @"5432";
            private String defaultDBName    = @"testdb";
            private String defaultUser      = @"test";
            private String defaultPass      = @"test";
        }
    }
}
