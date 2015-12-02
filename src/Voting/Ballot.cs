using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   [Serializable()]
   public class Ballot : IEquatable<Ballot>
   {
      public Choice Choice;
      public Moderator Moderator;

      [NonSerialized()]
      public Motion Motion;

      public int Id;

      public void Insert()
      {
         // remove my existing vote

         var existing = Motion.Votes.Where(x => x.Moderator == Moderator).ToList();

         foreach (var item in existing.ToList())
         {
            existing.Remove(item);
         }

         Motion.Votes.Add(this);
         Database.InsertBallot(this);
         RedditClient.PostBallot(this);
         RedditClient.PostMotion(Motion);
      }

      public void Delete()
      {
         Database.DeleteBallot(this);
      }

      public bool Equals(Ballot other)
      {
         return (this.Moderator == other.Moderator && this.Motion == other.Motion);
      }
   }

   public class BallotCollection : List<Ballot>
   {
      
      public int Yes
      {
         get
         {
            return this.Where(x => x.Choice == Choice.Yes).Count();
         }
      }

      public int No
      {
         get
         {
            return this.Where(x => x.Choice == Choice.No).Count();
         }
      }

      public int Abstain
      {
         get
         {
            return this.Where(x => x.Choice == Choice.Abstain).Count();
         }
      }

   }

   public enum Choice
   {
      Yes = 1,
      No = -1,
      Abstain = 0
   }
}
