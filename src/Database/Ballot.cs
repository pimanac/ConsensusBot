using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   static partial class Database
   {
      public static int InsertBallot(Ballot b)
      {
         var sql = CreateCommand();
         sql.CommandText = @"INSERT INTO ballots (createDt,motionId,moderatorId,choice) VALUES
                                                 (DATE('now'),@motionId,@moderatorId,@choice);";

         var p0 = sql.CreateParameter();
         p0.ParameterName = "@motionId";
         p0.Value = b.Motion.Id;
         sql.Parameters.Add(p0);

         var p1 = sql.CreateParameter();
         p1.ParameterName = "@moderatorId";
         p1.Value = b.Moderator.Id;
         sql.Parameters.Add(p1);

         var p2 = sql.CreateParameter();
         p2.ParameterName = "@choice";
         p2.Value = (int)b.Choice;
         sql.Parameters.Add(p2);

         b.Id = ExecuteNonQuery(sql);
         return b.Id;
      }

      public static int DeleteBallot(Ballot b)
      {
         var sql = CreateCommand();
         sql.CommandText = @"DELETE FROM ballots WHERE id = ?id";

         var p0 = sql.CreateParameter();
         p0.ParameterName = "?id";
         p0.Value = b.Id;
         sql.Parameters.Add(p0);

         return ExecuteNonQuery(sql);
      }

      public static Ballot GetBallot(int id)
      {
         var result = new Ballot();

         var sql = CreateCommand();
         sql.CommandText = @"SELECT id,motionId,moderatorId,choice FROM ballots WHERE id = ?id";

         var pId = sql.CreateParameter();
         pId.ParameterName = "?id";
         pId.Value = id;
         sql.Parameters.Add(pId);

         var rs = ExecuteReader(sql);

         result.Choice = (Choice)rs.GetInt32(3);
         result.Id = rs.GetInt32(0);
         result.Moderator = GetModerator(rs.GetInt32(2));
         result.Motion = GetMotion(rs.GetInt32(1));

         return result;
      }

      public static BallotCollection GetBallots(Motion motion)
      {
         var result = new BallotCollection();

         var sql = CreateCommand();
         sql.CommandText = @"SELECT id,motionId,moderatorId,choice FROM ballots WHERE motionId = ?motionId";

         var pId = sql.CreateParameter();
         pId.ParameterName = "?motionId";
         pId.Value = motion.Id;
         sql.Parameters.Add(pId);

         using (var db = CreateConnection())
         {
            db.Open();
            sql.Connection = db;
            sql.Prepare();
            var rs = sql.ExecuteReader();

            while (rs.Read())
            {
               result.Add(new Ballot
               {
                  Id = rs.GetInt32(0),
                  Motion = GetMotion(rs.GetInt32(1)),
                  Moderator = GetModerator(rs.GetInt32(2)),
                  Choice = (Choice)rs.GetInt32(3)
               });
            }
            db.Close();
         }
         return result;
      }
   }
}
