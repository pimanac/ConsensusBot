using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   [Serializable()]
   public class Moderator : IEquatable<Moderator>
   {
      public int Id;
      public string Name;
      public string IrcId;
      public string SlackId;
      public bool IsEnabled;

      public void CastBallot(Motion motion, Choice choice)
      {
         var b = new Ballot();
         b.Choice = choice;
         b.Motion = motion;
         b.Moderator = this;
         motion.Votes.Add(b);
         b.Insert();
      }

      public Motion MakeMotion(string text, bool supermajorityRequired = false)
      {
         return Motion.Create(this, text);
      }

      public bool Equals(Moderator other)
      {
         if (this.Id == other.Id)
         {
            return true;
         }

         if (this.Name == other.Name && this.IrcId == other.IrcId && this.SlackId == other.SlackId)
         {
            return true;
         }

         return false;
      }

      public override string ToString()
      {
         return this.Name;
      }

      public static Moderator GetModerator(int id)
      {
         return Database.GetModerator(id);
      }

      public static Moderator GetModerator(string name)
      {
         return Database.GetModerator(name);
      }
   }
}
