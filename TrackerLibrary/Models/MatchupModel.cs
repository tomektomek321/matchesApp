using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models {

    /// <summary>
    /// Represents one match in the tournament.
    /// </summary>
    public class MatchupModel {
        /// <summary>
        /// The unique identifier for the match.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The set of teams that were involved in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// The ID from the database that will be used to identify the winner.
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// The winner of the match.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Which round this match is part of.
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName {

            get {
                string output = "";

                if(Entries.Count == 2) {
                    string temp1team = "";
                    string temp2team = "";
                    
                    if(Entries[0].TeamCompeting != null) {
                        temp1team = Entries[0].TeamCompeting.TeamName;
                    }

                    if (Entries[1].TeamCompeting != null) {
                        temp2team = Entries[1].TeamCompeting.TeamName;
                    }

                    if (temp1team != "" && temp2team != "") {
                        output = Entries[0].TeamCompeting.TeamName;
                        output += " vs ";
                        output += Entries[1].TeamCompeting.TeamName;
                    } else if (temp1team != "" && temp2team == "") {
                        output = temp1team + " vs (??)";
                    } else if (temp1team == "" && temp2team != "") {
                        output = temp2team + " vs (??)";
                    } else {
                        output = "NOT SET YET";
                    }
                    
                } else if (Entries.Count == 1) {
                    output = Entries[0].TeamCompeting.TeamName;
                    output += " (BYE)";
                } else {
                    output = "NOT SET YET";
                }
                
                return output;
            }
        }

    }

}
