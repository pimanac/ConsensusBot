using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsensusBot
{
   static class Extensions
   {
      public static String ToRedditMarkdown(this Motion motion)
      {
         var result = "";
         result += "## Motion: " + motion.Text + Environment.NewLine + Environment.NewLine;
         result += "***"+ Environment.NewLine;
         result += "**Sponsor:** " + motion.Sponsor.Name + Environment.NewLine + Environment.NewLine;
         result += "**Entered:** " + motion.Created.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + @"\(UTC\)";
         result += Environment.NewLine;
         result += "### Votes:" + Environment.NewLine;
         result += "Vote | Count " + Environment.NewLine;
         result += " - | - " + Environment.NewLine;
         result += "Yea | " + motion.Votes.Yes.ToString() + Environment.NewLine;
         result += "Nay | " + motion.Votes.No.ToString() + Environment.NewLine;
         result += "Abstain | " + motion.Votes.Abstain.ToString() + Environment.NewLine;
         result += " ***** " + Environment.NewLine;
         result += "    " + motion.ToJson();
         return result;
      }

      public static String ToJson(this Motion motion)
      {
         var s = new Newtonsoft.Json.JsonSerializer();
         var sr = new System.IO.StringWriter();

         s.Serialize(sr, motion);

         return sr.ToString();
      }

      public static String ToJson(this Moderator mod)
      {
         var json = new Newtonsoft.Json.JsonSerializer();
         var sw = new System.IO.StringWriter();
         json.Serialize(sw, json);

         return sw.ToString();
      }
      
      public static String ToRedditMarkdown(this string str)
      {
         var result = str.Replace("(", @"\(");
         result = result.Replace(")", @"\)");

         return result;
      }
   }
}
