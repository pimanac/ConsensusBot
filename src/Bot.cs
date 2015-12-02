using System;
using System.Collections.Generic;
using ConsensusBot;
using ConsensusBot.Chat;
using System.Linq;  
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   public class Bot
   {
      private bool run = false;
      public List<IChatProvider> ChatProviders;

      public Queue<CommandJob> Queue;

      public Bot()
      {
         this.ChatProviders = new List<IChatProvider>();
         this.Queue = new Queue<CommandJob>();
      }

      public void Start()
      {
         run = true;
         foreach (IChatProvider p in ChatProviders)
         {
            p.Connect();
            p.CommandReceived += (sender, args) =>
            {
               Queue.Enqueue(new CommandJob {
                  Command = args.Command,
                  Moderator = args.Moderator
               });
            };
         }
         DoWork();
      }

      public void Stop()
      {
         run = true;
      }

      private void DoWork()
      {
         while (run)
         {
            if (Queue.Count == 0)
            {
               continue;
            }
            
            var item = Queue.Dequeue();
            try
            {
               switch (item.Command.Name)
               {
                  case "cast":
                     CommandCast(item.Command, item.Moderator);
                     break;
                  case "show":
                     // broadcast tot he entire channel
                     CommandShow(item.Command, item.Moderator,true);
                     break;
                  case "info":
                     CommandShow(item.Command, item.Moderator,false);
                     break;
                  case "new":
                     CommandMotion(item.Command, item.Moderator);
                     break;
                  case "help":
                  default:
                     CommandHelp(item.Moderator,item.Command);
                     break;
               }
            }
            catch (Exception ex)
            {
               System.Threading.Thread.Sleep(500);
               WriteChannel("Consensusbot Error");
               Console.WriteLine(ex.Message);
            }
         }
      }

      private void WriteChannel(string message,ChatProviderType provider = ChatProviderType.All)
      {
         List<IChatProvider> providers = new List<IChatProvider>();
         if (provider == ChatProviderType.All)
         {
            foreach (var item in this.ChatProviders)
            {
               providers.Add(item);
            }
         }
         else
         {
            providers = this.ChatProviders.Where(p => p.Type == provider).ToList();
         }


         foreach (IChatProvider p in providers)
         {
            p.WriteChannel(message);
         }
      }

      private void WriteUser(string message, Moderator user, ChatProviderType provider = ChatProviderType.All)
      {
         List<IChatProvider> providers = new List<IChatProvider>();
         if (provider == ChatProviderType.All)
         {
            foreach (var item in this.ChatProviders)
            {
               providers.Add(item);
            }
         }
         else
         {
            providers = this.ChatProviders.Where(p => p.Type == provider).ToList();
         }

         foreach (IChatProvider p in providers)
         {
            p.WriteUser(message, user);
         }
      }

      private void CommandShow(ChatCommand cmd, Moderator mod, bool broadcast = true)
      {
         var usage = "usage: $vote show [ {motion id} | ]";
         var opts = cmd.Args.Split(' ');

         // args empty
         if (opts.GetUpperBound(0) == 0)
         {
            WriteUser(usage, mod);
            return;
         }

         try
         {
            var motion = Motion.GetMotion(Int32.Parse(opts[0]));
            var message = new StringBuilder();
            message.AppendFormat("Motion [{0}]", motion.Id.ToString());
            message.AppendFormat("{{0}/{1}/{2}}", motion.Votes.Yes, motion.Votes.No, motion.Votes.Abstain);
            message.AppendFormat(": {0}  #  {1}", motion.Text, motion.PostUrl);

            if (broadcast)
            {
               WriteChannel(message.ToString(),ChatProviderType.All);
            }
            else
            {
               WriteUser(message.ToString(),mod,cmd.Source);
            }
            
         }
         catch
         {
            WriteUser(usage, mod,cmd.Source);
         }
      }

      private void CommandMotion(ChatCommand cmd,Moderator mod)
      {
         var usage = "usage: $vote motion [text of the motion]";

         if (cmd.Args.Length == 0)
         {
            WriteUser(usage, mod,cmd.Source);
            return;
         }

         WriteUser("Submitting your motion...this make take a second.", mod,cmd.Source);
         var motion = Motion.Create(mod, cmd.Args);

         var result = "Motion entered [" + motion.Id.ToString() + "]: " + motion.PostUrl;

         WriteChannel(result,ChatProviderType.All);
      }

      private void CommandCast(ChatCommand cmd, Moderator mod)
      {
         var usage = "usage: $vote cast [choice]{ yes | no | abstain } [motion id] ";

         if (cmd.Args.Length == 0)
         {
            WriteUser(usage, mod,cmd.Source);
            return;
         }

         var stuff = cmd.Args.Split(' ');

         if (stuff.GetUpperBound(0) < 1)
         {
            WriteUser(usage, mod,cmd.Source);
            return;
         }

         var id = -1;
         try
         {
            id = Int32.Parse(stuff[1]);
         }
         catch
         {
            WriteUser(usage, mod,cmd.Source);
            return;
         }

         Choice c = Choice.Abstain;
         bool error = false;
         switch (stuff[0].ToLower())
         {
            case "yes":
            case "y":
            case "+1":
            case "yay":
            case "yae":
            case "yea":
            case "oui":
            case "si":
            case "ja":
            case "da":
               c = Choice.Yes;
               break;
            case "no":
            case "n":
            case "-1":
            case "nae":
            case "nay":
            case "non":
            case "nein":
            case "neit":
               c = Choice.No;
               break;
            case "abstain":
            case "abs":
            case "0":
               c = Choice.Abstain;
               break;
            default:
               error = true;
               break;
         }

         if (error)
         {
            WriteUser(usage, mod);
            return;
         }

         WriteUser("Submitting ballot.  Please wait.",mod,cmd.Source);

         var motion = Motion.GetMotion(id);
         var b = new Ballot();
         b.Choice = c;
         b.Moderator = mod;
         b.Motion = motion;
         b.Insert();

         WriteUser("Your ballot has been recorded", mod,cmd.Source);
      }

      private void CommandHelp(Moderator mod, ChatCommand cmd = null)
      {
         string context = "";
         if (cmd != null && cmd.Args != null && cmd.Args.Trim().Contains(" "))
         {
            context = cmd.Args.Split(' ')[0];
         }

         switch (context.ToLowerInvariant())
         {
            case "help":
               WriteUser("usage: $vote [ new { text } | cast { id } { yes/no/abstain } | help ]",mod,cmd.Source);
               break;
            case "new":
               WriteUser("usage: $vote new I'd like to do this.  all in favor?",mod,cmd.Source);
               break;
            case "cast":
               WriteUser("usage: $vote cast [ yes / yea / +1 | no / nae / -1 | abstain / abs / 0 ]", mod,cmd.Source);
               break;
            default:
               WriteUser("usage: $vote [ new { text } | cast { id } { yes/no/abstain } | help ]", mod,cmd.Source);
               break;
         }
      }
   }

   public struct CommandJob
   {
      public ChatCommand Command;
      public Moderator Moderator;
   }
}
