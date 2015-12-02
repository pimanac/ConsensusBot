using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   // Methods relation to motions
   static partial class Database
   {
      public static int InsertMotion(Motion motion)
      {
         var result = -1;
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            db.Open();
            sql.CommandText = @"INSERT INTO motion (createDt,moderatorId,text,status,redditLink,isInOrder) VALUES
                                                 (DATE('now'),@sponsor,@text,@status,@redditLink,@isInOrder);";

            var p0 = sql.CreateParameter();
            p0.ParameterName = "@sponsor";
            p0.Value = motion.Sponsor.Id;
            sql.Parameters.Add(p0);

            var p1 = sql.CreateParameter();
            p1.ParameterName = "@text";
            p1.Value = motion.Text;
            sql.Parameters.Add(p1);

            var p2 = sql.CreateParameter();
            p2.ParameterName = "@status";
            p2.Value = (int)motion.Status;
            sql.Parameters.Add(p2);

            var p3 = sql.CreateParameter();
            p3.ParameterName = "@redditLink";
            p3.Value = motion.PostUrl;
            sql.Parameters.Add(p3);

            var p4 = sql.CreateParameter();
            p4.ParameterName = "@isInOrder";
            p4.Value = 0;
            sql.Parameters.Add(p4);

            sql.Prepare();
            sql.ExecuteNonQuery();
            sql.CommandText = "SELECT last_insert_rowid()";
            result = Int32.Parse(sql.ExecuteScalar().ToString()); sql.ExecuteNonQuery();

            db.Close();
         }
         return result;
      }

      public static int UpdateMotion(Motion motion)
      {
         var sql = CreateCommand();
         sql.CommandText = @"UPDATE motion SET updateDt = DATE('now'),moderatorId = @sponsor,text = @text,status = @status,
                                    redditLink = @redditLink,isInOrder = @isInOrder WHERE id = @id;";

         var pId = sql.CreateParameter();
         pId.ParameterName = "@id";
         pId.Value = motion.Id;
         sql.Parameters.Add(pId);

         var p0 = sql.CreateParameter();
         p0.ParameterName = "@sponsor";
         p0.Value = motion.Sponsor.Id;
         sql.Parameters.Add(p0);

         var p1 = sql.CreateParameter();
         p1.ParameterName = "@text";
         p1.Value = motion.Text;
         sql.Parameters.Add(p1);

         var p2 = sql.CreateParameter();
         p2.ParameterName = "@status";
         p2.Value = (int)motion.Status;
         sql.Parameters.Add(p2);

         var p3 = sql.CreateParameter();
         p3.ParameterName = "@redditLink";
         p3.Value = motion.PostUrl;
         sql.Parameters.Add(p3);

         var p4 = sql.CreateParameter();
         p4.ParameterName = "@isInOrder";
         p4.Value = 0;
         sql.Parameters.Add(p4);


         return ExecuteNonQuery(sql);
      }

      public static Motion GetMotion(int id)
      {
         var result = new Motion();
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {
            db.Open();
            sql.CommandText = @"SELECT id,moderatorId,text,status,redditLink,isInOrder,createDt FROM motion WHERE id = @id LIMIT 1;";

            var pId = sql.CreateParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            sql.Parameters.Add(pId);

            var rs = sql.ExecuteReader();

            if (rs.Read())
            {
               result.Id = rs.GetInt32(0);
               result.Sponsor = GetModerator(rs.GetInt32(1));
               result.Text = rs.GetString(2);
               result.Status = (MotionStatus)rs.GetInt32(3);
               result.PostUrl = (string)rs.GetValue(4);
               result.Created = DateTime.Parse(rs.GetString(6));
            }
            rs.Close();
            db.Close();
         }
         return result;
      }
   }
}
