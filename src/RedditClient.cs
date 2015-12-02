using System;
using System.Linq;
using RedditSharp.Things;
using RedditSharp;

namespace ConsensusBot
{
   static class RedditClient
   {
      public static Reddit GetReddit()
      {
         return new Reddit(Config.RedditUser, Config.RedditPass, true);
      }

      public static void PostMotion(Motion motion)
      {
         var reddit = GetReddit();
         var subreddit = reddit.GetSubreddit(Config.RedditSubreddit);

         Post post;
         if (motion.PostUrl != "")
         {
            post = reddit.GetPost(new Uri(motion.PostUrl));
            post.EditText(motion.ToRedditMarkdown());
         }
         else
         {
            post = subreddit.SubmitTextPost("[" + motion.Id + "] " + motion.Text, motion.ToRedditMarkdown());
         }
         post.Save();
         motion.PostUrl = post.Url.AbsoluteUri;
      }

      public static void GetModLog(string subreddit)
      {
         var reddit = GetReddit();
         reddit.RateLimit = WebAgent.RateLimitMode.Pace;
         
         var sub = reddit.GetSubreddit(subreddit);
         
         var log = sub.GetModerationLog().ToList();

#if DEBUG
         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine(log.Count + " INSERTING NOW");

         Console.ForegroundColor = ConsoleColor.White;
#endif
         Database.InsertModlogRecords(log);

         foreach (var item in log)
         {
            Console.WriteLine(item.Action.ToString() + "   " + item.Id);
         }
      }

      public static void PostBallot(Ballot ballot)
      {
         var motion = ballot.Motion;
         if (motion.PostUrl == "")
         {
            return;
         }


         var reddit = new RedditSharp.Reddit(Config.RedditUser, Config.RedditPass, true);
         var post = reddit.GetPost(new Uri(motion.PostUrl));
         post.Comment("###" + ballot.Moderator.Name + "###" + Environment.NewLine
            + "*****" + Environment.NewLine
            + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + @" \(UTC\) : *" + ballot.Choice.ToString() + "*" + Environment.NewLine
            );

      }
   }
}
