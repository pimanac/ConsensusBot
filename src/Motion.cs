using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;

namespace ConsensusBot
{
   public enum MotionStatus
   {
      Voting,
      Passed,
      Failed,
      OutOfOrder
   }

   [Serializable()]
   public class Motion : IEquatable<Motion>
   {
      public int Id;
      public string Text;
      public Moderator Sponsor;
      public MotionStatus Status;
      public BallotCollection Votes;
      public DateTime Created;
      public bool RequiresSuperMajority = false;
      public string PostUrl;

      public string StatusText;

      public Motion()
      {
         this.Votes = new BallotCollection();
         this.Text = "";
         this.PostUrl = "";
      }

      public void Insert()
      {
         this.Id = Database.InsertMotion(this);
         RedditClient.PostMotion(this);
         Database.UpdateMotion(this);
      }

      public void Update()
      {
         Database.UpdateMotion(this);
      }

      public void CalculateVotes()
      {
         // do we have a quorum, exlcuding those who abstain?
         if (Votes.Count < Config.Quorum || Votes.Count - Votes.Abstain < Config.Quorum)
         {
            Status = MotionStatus.OutOfOrder;
            StatusText = "Quorum is not present.";
            return;
         }

         // ties are failures
         if (Votes.No >= Votes.Yes)
         {
            Status = MotionStatus.Failed;
            return;
         }

         if (this.RequiresSuperMajority)
         {
            if (Votes.Yes < Config.SuperMajorityOfQuorum)
            {
               Status = MotionStatus.Failed;
               return;
            }
         }

         Status = MotionStatus.Passed;
         return;
      }

      public static Motion Create(Moderator sponsor, string text)
      {
         var result = new Motion();
         result.Text = text;
         result.Sponsor = sponsor;
         result.Created = DateTime.Now;
         result.Insert();

         return result;
      }

      public static Motion GetMotion(int id)
      {
         return Database.GetMotion(id);
      }

      // IEquatable
      public bool Equals(Motion other)
      {
         if (this.Id == other.Id)
         {
            return true;
         }

         if (this.Sponsor.Id == other.Sponsor.Id && this.Text == other.Text)
         {
            return true;
         }
         return false;
      }

      public override string ToString()
      {
         var result = "";
         result += "Motion: " + this.Text + Environment.NewLine + Environment.NewLine;
         result += "**********" + Environment.NewLine;
         result += "Sponsor: " + this.Sponsor.Name + Environment.NewLine + Environment.NewLine;
         result += "Entered: " + this.Created.ToUniversalTime() + " (UTC)";
         result += "Id:      " + this.Id.ToString();
         result += Environment.NewLine;
         result += "Votes:" + Environment.NewLine;
         result += "Yea:     " + Votes.Yes.ToString() + Environment.NewLine;
         result += "Nay:     " + Votes.No.ToString() + Environment.NewLine;
         result += "Abstain: " + Votes.Abstain.ToString() + Environment.NewLine;

         return result;
      }


   }
}
