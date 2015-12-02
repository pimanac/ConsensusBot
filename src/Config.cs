using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   [Serializable()]
   public static class Config
   {
      public static string SlackToken;
      public static int Mods;
      public static int Quorum;

      public static decimal SuperMajorityPercent = 75.00M;
      public static decimal MajorityPercent = 50.01M;

      public static string DSN;
      public static string RedditUser;
      public static string RedditPass;
      public static string RedditSubreddit;

      public static string IrcServer;
      public static UInt32 IrcPort;
      public static string IrcRoom;
   
      public static int SuperMajorityOfMods
      {
         get
         {
            return (int)Math.Ceiling((Mods * (SuperMajorityPercent / 100)));
         }
      }

      public static int SuperMajorityOfQuorum
      {
         get
         {
            return (int)Math.Ceiling((Quorum * (SuperMajorityPercent / 100)));
         }
      }

      public static int MajorityOfMods
      {
         get
         {
            return (int)Math.Ceiling((Mods * (MajorityPercent / 100)));
         }
      }

      public static int MajorityOfQuorum
      {
         get
         {
            return (int)Math.Ceiling((Quorum * (MajorityPercent / 100)));
         }
      }
   }
}
