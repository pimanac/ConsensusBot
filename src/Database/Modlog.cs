using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;

namespace ConsensusBot
{
   public partial class Database
   {
      public static void InsertModlogRecords(IEnumerable<ModAction> log)
      {
         using (var db = CreateConnection())
         using (var sql = db.CreateCommand())
         {

            /*
            "description" : null,
					"mod_id36" : "8937y",
					"created_utc" : 1448759942.0,
					"subreddit" : null,
					"target_permalink" : "/r/politics/comments/3ulxdn/xxxxxxxxxxxxxxxxxxxxxxxxxxxx/yyyyyyyy",
					"details" : "confirm_ham",
					"action" : "approvecomment",
					"target_author" : "xxxxxxx",
					"target_fullname" : "t1_cxfw7gj",
					"sr_id36" : "2cneq",
					"id" : "ModAction_2d05ebaa-9637-11e5-876e-0e44750760a5",
					"mod" : "xxxxxx"
   */
            db.Open();
            sql.CommandText = @"INSERT INTO modlog (description,mod_id36,created_utc,subreddit,target_permalink,
                                                    details,modaction,target_author,target_fullname,sr_id36,
                                                    redditId, mod)
                                            Values (@description,@mod_id36,@created_utc,@subreddit,@target_permalink,
                                                    @details,@modaction,@target_author,@target_fullname,@sr_id36,
                                                    @redditId, @mod;";

            var parameters = new Dictionary<string, object>();

            foreach (var action in log.Take(500))
            {
               parameters.Add("@description", "");
               parameters.Add("@mod_id36", "");
               parameters.Add("@created_utc", action.TimeStamp.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
               parameters.Add("@subreddit", Config.RedditSubreddit);
               parameters.Add("@target_permalink", action.TargetThing.Shortlink);
               parameters.Add("@details", action.Details);
               parameters.Add("@modaction", action.Action);
               parameters.Add("@target_author", action.TargetAuthor);
               parameters.Add("@target_fullname", action.TargetThingFullname);
               parameters.Add("@sr_id36", "");
               parameters.Add("@redditId", action.Id);
               parameters.Add("@mod", action.ModeratorName);

               foreach (var item in parameters)
               {
                  var x = sql.CreateParameter();
                  x.ParameterName = item.Key;
                  x.Value = item.Value;
               }
               sql.Prepare();
               sql.ExecuteNonQuery();
            }

            db.Close();
         }
      }
   }
}
