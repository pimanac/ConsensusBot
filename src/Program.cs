using System;
using static System.Console;

namespace ConsensusBot
{
   class Program
   {
      const string DSN = @"Data source=.\consensusBot.db;Version=3;";

      const string IRC_HOST = "localhost";
      const string IRC_ROOM = "";
      const string REDDIT_USER = "";
      const string REDDIT_PASS = "";
      const string REDDIT_SUB = "";
      const string SLACK_TOKEN = "";

      static Bot bot;

      static void Main(string[] args)
      {
         Config.Mods = 28;
         Config.Quorum = 25;
         Config.MajorityPercent = 50.01M;
         Config.SuperMajorityPercent = 75.01M;
         Config.DSN = DSN;
         Config.RedditPass = Properties.Settings.Default.RedditPassword;
         Config.RedditUser = Properties.Settings.Default.RedditUser;
         Config.RedditSubreddit = Properties.Settings.Default.RedditSubreddit;
         Config.IrcPort = 6667;
         Config.IrcServer = Properties.Settings.Default.IrcHost;
         Config.IrcRoom = Properties.Settings.Default.IrcRoom;
         Config.SlackToken = Properties.Settings.Default.SlackToken;

         WriteLine("Starting up.");
         bot = new Bot();


         WriteLine("Loading Chat Providers");
         
         bot.ChatProviders.Add(new Chat.IrcProvider());
         bot.ChatProviders.Add(new Chat.SlackProvider());

         WriteLine("Providers loaded.");
         bot.Start();
      }
   }
}
