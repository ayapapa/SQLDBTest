using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Data;
using System.IO;
using Npgsql;

using BPMSDataBases;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var connString = @"Server=localhost;Port=5432;User Id=postgres;Password=rootroot;Database=postgres";
                //var conn = new NpgsqlConnection(connString);
                //conn.Open();
                DBConnection dbc = new DBConnection();
                dbc.Open();
                var conn = dbc.GetConnection();
                String serverversion;
                NpgsqlCommand command = new NpgsqlCommand("select version()", conn);
                serverversion = (String)command.ExecuteScalar();
                Console.WriteLine("PostgreSQL server version: {0}", serverversion);

                command = new NpgsqlCommand(@"SELECT* FROM pg_tables where not (tablename like 'pg%' or tablename like 'sql_%') order by tablename;", conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Console.WriteLine("{0}", dr.GetString(0));
                    }
                }
                dr.Close();
                //String tables = (String)command.ExecuteScalar();
                //Console.WriteLine("TABLES: ");
                //Console.WriteLine(tables);


                command = new NpgsqlCommand(@"select * from user", conn);
                String users = (String)command.ExecuteScalar();
                Console.WriteLine("USERS: ");
                Console.WriteLine(users);

                // create table
                try
                {
                    command = new NpgsqlCommand(@"CREATE TABLE table1 (id INTEGER, date_tz TIMESTAMP WITH TIME ZONE, description TEXT)", conn);
                    String create_table = (String)command.ExecuteScalar();
                    Console.WriteLine(create_table);
                }
                catch(Exception)
                {
                    Console.WriteLine("すでにtable1は存在しているため作成しませんでした。");
                }

                // テーブル生成戦略
                // オンメモリでテーブルデータを構築
                // そのテーブルをDBへ紐づけるためのアダプタを作成
                // アダプタを使って、テーブルデータをDBへプッシュ（アップデート）

                // オンメモリテーブルDataTable生成
                DataTable table1 = new DataTable("table1");
                table1.Columns.Add(new DataColumn("id", typeof(int)));
                table1.Columns.Add(new DataColumn("date_tz", typeof(DateTime)));
                table1.Columns.Add(new DataColumn("description", typeof(string)));

                // DataAdapter生成
                Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter();
                // SELECTコマンド
                da.SelectCommand = new Npgsql.NpgsqlCommand("SELECT id, date_tz, description FROM table1 ORDER BY id", conn);
                // INSERTコマンド
                da.InsertCommand = new Npgsql.NpgsqlCommand("INSERT INTO table1 (id, date_tz, description) VALUES (:id, :date_tz, :description)", conn);
                da.InsertCommand.Parameters.Add(new Npgsql.NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Integer, 0, "id"));
                da.InsertCommand.Parameters.Add(new Npgsql.NpgsqlParameter("date_tz", NpgsqlTypes.NpgsqlDbType.TimestampTZ, 0, "date_tz"));
                da.InsertCommand.Parameters.Add(new Npgsql.NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text, 0, "description"));
                // UpdateCommandとDeleteCommandは省略

                // データを入れる
                DataRow newRow = table1.NewRow();
                newRow["id"] = 1;
                newRow["date_tz"] =DateTime.Now;// DateTime.Parse("2016-01-01");
                newRow["description"] = "'いや～ん♪ (◇^*)Ξ(*^◇)'";
                table1.Rows.Add(newRow);
                // 保存
                da.Update(table1);

                // データを取得
                DataTable table2 = table1.Clone();
                da.Fill(table2);
                foreach (DataRow row in table2.Rows)
                {
                    var id = row["id"];
                    var date_tz = row["date_tz"];
                    var description = row["description"];
                    Console.WriteLine("id[" + id.GetType().ToString() + "]=" + id.ToString());
                    Console.WriteLine("date_tz[" + date_tz.GetType().ToString() + "]=" + date_tz.ToString());
                    Console.WriteLine("description[" + description.GetType().ToString() + "]=" + description.ToString());
                }

                // データベース切断
                conn.Close();

                Console.ReadKey();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Hello World!");
        }
    }
}