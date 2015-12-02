using System;
using System.Collections.Generic;


namespace ConsensusBot.Chat
{
   public delegate void CommandHandler(object sender, CommandReceivedEventArgs args);

   public enum ChatProviderType
   {
      IRC,
      Slack,
      All = 99
   }

   public interface IChatProvider : IDisposable
   {
      ChatProviderType Type
      {
         get;
      }

      /// <summary>
      /// On command received
      /// </summary>
      event CommandHandler CommandReceived;

      /// <summary>
      /// Write something to the chat channel.
      /// </summary>
      void WriteChannel(string command);

      /// <summary>
      /// Write something as a PM
      /// </summary>
      /// <param name="command"></param>
      /// <param name="user"></param>
      void WriteUser(string command, Moderator user);


      void Connect();

      void Disconnect();

      /// <summary>
      /// Get a list of moderators in the Chat system
      /// </summary>
      /// <returns></returns>
      IEnumerable<Moderator> GetUsers();

      void Dispose();
   }

   public class CommandReceivedEventArgs : EventArgs
   {
      public ChatCommand Command;
      public Moderator Moderator;
      public string Message;

   }



}
