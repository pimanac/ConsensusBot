using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackConnector;
using SlackConnector.Models;


namespace ConsensusBot.Chat
{
   class SlackProvider : IChatProvider
   {
      public event CommandHandler CommandReceived;
      SlackConnector.SlackConnector client;
      ISlackConnection connection;

      public ChatProviderType Type
      {
         get
         {
            return ChatProviderType.Slack;
         }
      }

      public SlackProvider()
      {
         this.client = new SlackConnector.SlackConnector();
      }

      public async void Connect()
      {
         // we don't have to join channels - just choose the channel when sending the message
         connection = await client.Connect(Config.SlackToken);
         connection.OnMessageReceived += Connection_OnMessageReceived;
      }

      private async Task Connection_OnMessageReceived(SlackMessage message)
      {
         ProcessMessage(message);
      }

      public void Disconnect()
      {
         connection.Disconnect();
      }

      public void Dispose()
      {
         Disconnect();
      }

      public IEnumerable<Moderator> GetUsers()
      {
         var result = new List<Moderator>();

         foreach (var user in GetSlackUserData().Result.members)
         {
            result.Add(Moderator.GetModerator(user.name));
         }
         return result;
      }

      private async Task<SlackAPI.UserListResponse> GetSlackUserData()
      {
         var client = new SlackAPI.SlackTaskClient(Config.SlackToken);

         return await client.GetUserListAsync();
      }
      public async void WriteChannel(string command)
      {
         SlackChatHub channel = connection.ConnectedChannels().Where(x => x.Name == "#general").FirstOrDefault();
         var message = new BotMessage();
         message.ChatHub = channel;
         message.Text = command;
         await connection.Say(message);
      }

      public async void WriteUser(string command, Moderator user)
      {
         var client = new SlackAPI.SlackTaskClient(Config.SlackToken);
         if (!client.ConnectAsync().Result.ok)
         {
            return;
         }

         string userId = "";

         foreach (var x in client.UserLookup)
         {
            if (String.Equals(x.Value.name, user.Name, StringComparison.InvariantCultureIgnoreCase))
            {
               userId = x.Value.id;
               break;
            }
         }

         var channel = await client.JoinDirectMessageChannelAsync(userId);

         var result = await client.PostMessageAsync(channel.channel.id, command, "Consensusbot");
         if (result.ok)
         {
            return;
         }
         else
         {
            Console.Write("something went wrong");
         }
      }
      
      protected void OnCommandReceived(CommandReceivedEventArgs e)
      {
         CommandReceived(this, e);
      }

      private void ProcessMessage(SlackMessage message)
      {
         // we should never respond to messages from the bot itself
         if (String.Equals(message.User.Name, "ConsensusBot", StringComparison.InvariantCultureIgnoreCase) &&
            !String.Equals(message.ChatHub.Name,"general",StringComparison.InvariantCulture))
         {
            return;
         }

         var mod = Moderator.GetModerator(message.User.Name);
         if (message.Text.StartsWith("$vote", StringComparison.InvariantCultureIgnoreCase))
         {
            ChatCommand cmd = null;
            if (ChatCommand.TryParse(ChatProviderType.Slack,message.Text, out cmd))
            {
               OnCommandReceived(new CommandReceivedEventArgs { Command = cmd, Message = message.Text, Moderator = mod });
            }
         }
      }

   }
}
