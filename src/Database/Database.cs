using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Data;

namespace ConsensusBot
{
   static partial class Database
   {
      static IDbConnection CreateConnection()
      {
         return new SQLiteConnection(Config.DSN);
      }

      static IDbCommand CreateCommand()
      {
         return new SQLiteCommand();
      }

      static IDataReader ExecuteReader(IDbCommand sql)
      {
         IDataReader result = null;
         using (var db = CreateConnection())
         {
            db.Open();
            sql.Connection = db;
            sql.Prepare();
            result = sql.ExecuteReader();
            db.Close();
         }

         return result;
      } // ExecuteReader

      static IDataReader ExecuteReader(string commandText)
      {

         IDataReader result = null;
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            sql.CommandText = commandText;
            db.Open();
            sql.Connection = db;
            sql.Prepare();
            result = sql.ExecuteReader();
            db.Close();
         }

         return result;
      } // ExecuteReader

      static int ExecuteNonQuery(IDbCommand sql)
      {
         var result = -1;
         using (var db = CreateConnection())
         {
            db.Open();
            sql.Connection = db;
            sql.Prepare();
            sql.ExecuteNonQuery();
            sql.CommandText = "SELECT last_insert_rowid()";
            result = Int32.Parse(sql.ExecuteScalar().ToString());
            db.Close();
         }
         return result;
      } // ExecuteNonQuery

      static int ExecuteNonQuery(string commandText)
      {
         var result = -1;
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            sql.CommandText = commandText;
            db.Open();
            sql.Connection = db;
            sql.Prepare();
            result = sql.ExecuteNonQuery();
            db.Close();
         }
         return result;
      } // ExecuteNonQuery

      static string TestMe()
      {
         string result = "";

         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            db.Open();
            sql.CommandText = "select name from moderators;";
            var rs = sql.ExecuteReader();

            while (rs.Read())
            {
               result = rs.GetString(0);
            }
            db.Close();
            rs.Dispose();
         }

            return result;
      }
   }
}
