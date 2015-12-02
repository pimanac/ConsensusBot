using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot.Chat
{
   public class ChatCommand
   {
      private static string[] COMMANDS = { "help", "show", "display", "new", "cast" };

      public string Name;
      public string Args;
      public Moderator Moderator;
      public ChatProviderType Source;

      public static bool TryParse(ChatProviderType type, string str, out ChatCommand cmd)
      {
         
         string name = "";
         string args = "";

         cmd = new ChatCommand();
         cmd.Source = type;
         if (!str.StartsWith("$vote", StringComparison.InvariantCultureIgnoreCase))
         {
            return false;
         }

         // remove $vote 
         str = str.Remove(0, 5).Trim();

         if (str.Length == 0)
         {
            cmd.Name = "help";
            cmd.Args = "";
            return true;
         }

         if (!str.Contains(" "))
         {
            cmd.Name = str;
            cmd.Args = "";
            return true;
         }

         var arr = str.Split(new char[] { ' ' }, 2);
         name = arr[0];
         args = arr[1];

         if (!COMMANDS.Contains(name))
         {
            cmd.Name = "help";
            cmd.Args = "";
            return true;
         }

         cmd.Name = name;
         cmd.Args = args;

         return true;
      }

   }
}
