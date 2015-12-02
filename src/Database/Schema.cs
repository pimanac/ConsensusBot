using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusBot
{
   static partial class Database
   {
      public static string Schema
      {
         get
         {
            return @"
BEGIN TRANSACTION;
CREATE TABLE ""motion"" (
	`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`createDt`	TEXT,
	`updateDt`	TEXT,
	`moderatorId`	INTEGER NOT NULL,
	`text`	TEXT,
	`status`	INTEGER,
	`redditLink`	TEXT,
	`isInOrder`	INTEGER
);
CREATE TABLE ""moderators"" (
	`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`createDt`	TEXT,
	`updateDt`	TEXT,
	`userName`	TEXT NOT NULL,
	`ircId`	TEXT,
	`slackId`	TEXT,
	`isEnabled`	INTEGER
);
CREATE TABLE ""ballots"" (
	`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`createDt`	TEXT,
	`motionId`	INTEGER,
	`moderatorId`	INTEGER,
	`choice`	INTEGER
);
COMMIT;
";
         }
      }
   }
}
