using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   // Methods relation to mods
   static partial class Database
   {
      public static int InsertModerator(Moderator mod)
      {
         var sql = CreateCommand();
         sql.CommandText = @"INSERT INTO moderators (createDt,userName,ircId,slackId,isEnabled) VALUES
                                                 (DATE('now'),@name,@ircId,@slackId,@isEnabled);";

         var p0 = sql.CreateParameter();
         p0.ParameterName = "@name";
         p0.Value = mod.Name;
         sql.Parameters.Add(p0);

         var p1 = sql.CreateParameter();
         p1.ParameterName = "@ircId";
         p1.Value = mod.IrcId;
         sql.Parameters.Add(p1);

         var p2 = sql.CreateParameter();
         p2.ParameterName = "@slackId";
         p2.Value = mod.SlackId;
         sql.Parameters.Add(p2);

         var p3 = sql.CreateParameter();
         p3.ParameterName = "@isEnabled";
         p3.Value = mod.IsEnabled;
         sql.Parameters.Add(p3);

         return ExecuteNonQuery(sql);
      }

      public static int UpdateModerator(Moderator mod)
      {
         var sql = CreateCommand();
         sql.CommandText = @"UPDATE moderators SET updateDt = DATE('now'),userName = ?name,text = ?text,slackId = ?slackId, isEnabled = ?isEnabled) WHERE id = ?id;";

         var pId = sql.CreateParameter();
         pId.ParameterName = "?id";
         pId.Value = mod.Id;
         sql.Parameters.Add(pId);

         var p0 = sql.CreateParameter();
         p0.ParameterName = "?name";
         p0.Value = mod.Name;
         sql.Parameters.Add(p0);

         var p1 = sql.CreateParameter();
         p1.ParameterName = "?ircId";
         p1.Value = mod.IrcId;
         sql.Parameters.Add(p1);

         var p2 = sql.CreateParameter();
         p2.ParameterName = "?slackId";
         p2.Value = mod.SlackId;
         sql.Parameters.Add(p2);

         var p3 = sql.CreateParameter();
         p3.ParameterName = "?isEnabled";
         p3.Value = mod.IsEnabled;
         sql.Parameters.Add(p3);

         return ExecuteNonQuery(sql);
      }

      public static int DeleteModerator(Moderator mod)
      {
         var sql = CreateCommand();
         sql.CommandText = @"UPDATE moderators SET updateDt = DATE('now'),isEnabled = ?isEnabled) WHERE id = ?id;";

         var p0 = sql.CreateParameter();
         p0.ParameterName = "?id";
         p0.Value = mod.Id;
         sql.Parameters.Add(p0);

         return ExecuteNonQuery(sql);
      }

      public static Moderator GetModerator(int id)
      {
         var result = new Moderator();
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            db.Open();
            sql.CommandText = @"SELECT id,userName,ircId,slackId FROM moderators WHERE id = @id LIMIT 1;";

            var pId = sql.CreateParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            sql.Parameters.Add(pId);

            var rs = sql.ExecuteReader();
            if (rs.Read())
            {
               result.Id = rs.GetInt32(0);
               result.Name = rs.GetString(1);
               result.IrcId = rs.GetString(2);
               result.SlackId = rs.GetString(3);
            }
            rs.Close();
            db.Close();

         }
         return result;
      }

      public static Moderator GetModerator(string name)
      {
         var result = new Moderator();
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            sql.CommandText = @"SELECT id,userName,ircId,slackId FROM moderators WHERE userName = @name LIMIT 1;";

            var pId = sql.CreateParameter();
            pId.ParameterName = "@name";
            pId.Value = name;
            sql.Parameters.Add(pId);

            db.Open();
            sql.Prepare();
            var rs = sql.ExecuteReader();

            while (rs.Read())
            {
               result.Id = rs.GetInt32(0);
               result.Name = rs.GetString(1);
               result.IrcId = rs.GetString(2);
               result.SlackId = rs.GetString(3);

            }
            db.Close();
            return result;
         }
      }
   }
}
