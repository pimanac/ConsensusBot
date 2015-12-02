using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using ChatSharp.Events;

namespace ConsensusBot.Chat
{
   class IrcProvider : IChatProvider
   {
      private IrcClient client;

      public ChatProviderType Type
      {
         get
         {
            return ChatProviderType.IRC;
         }
      }

      public event CommandHandler CommandReceived;

      public IrcProvider()
      {
         client = new IrcClient(Config.IrcServer, new IrcUser("Consensusbot", "ConsensusBot"));
         client.ConnectAsync();
      }

      private void DoWork()
      {
         client.ConnectionComplete += (sender, args) =>
         {
            client.JoinChannel("#" + Config.IrcRoom);
         };

         client.ChannelMessageRecieved += (sender, args) =>
         {
            // we should never respond to messages from the bot itself
            if (String.Equals(args.PrivateMessage.User.User, "ConsensusBot", StringComparison.InvariantCultureIgnoreCase))
            {
               return;
            }

            var message = args.PrivateMessage.Message;
            var mod = Moderator.GetModerator(args.PrivateMessage.User.User);
            if (message.StartsWith("$vote", StringComparison.InvariantCultureIgnoreCase))
            {
               ChatCommand cmd = null;
               if (ChatCommand.TryParse(ChatProviderType.IRC,message, out cmd))
               {
                  mod.IrcId = args.PrivateMessage.User.Nick;
                  OnCommandReceived(new CommandReceivedEventArgs { Command = cmd, Message = message, Moderator = mod });
               }
            }
         };
      }

      private void Quit()
      {
         client.Quit();
      }

      /// <summary>
      /// Get a list of moderators in IRC
      /// </summary>
      /// <returns></returns>
      public IEnumerable<Moderator> GetUsers()
      {
         var result = new List<Moderator>();
         foreach (IrcUser user in client.Channels["#" + Config.IrcRoom].Users)
         {
            result.Add(Moderator.GetModerator(user.Nick));
         }

         return result;
      }

      /// <summary>
      /// Write a message to the channel
      /// </summary>
      /// <param name="command">message</param>
      public void WriteChannel(string command)
      {
         client.SendMessage(command, new string[] { "#" + Config.IrcRoom });
      }

      /// <summary>
      /// Write a message to a user (PM)
      /// </summary>
      /// <param name="command">message</param>
      /// <param name="user">user nick</param>
      public void WriteUser(string command, Moderator user)
      {
         command.Replace(Environment.NewLine, "||");

         client.SendMessage(command, new string[] { user.IrcId });
      }

      // IDisposable
      public void Dispose()
      {
         client.Quit();
      }

      protected void OnCommandReceived(CommandReceivedEventArgs e)
      {
         CommandReceived(this, e);
      }

      public void Connect()
      {
         DoWork();
      }

      public void Disconnect()
      {
         Quit();
      }

   }


}
