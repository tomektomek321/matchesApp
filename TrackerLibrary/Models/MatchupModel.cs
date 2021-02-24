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

                int counter = 0;
                int twoNull = 0;
                foreach (MatchupEntryModel me in Entries) {
                    if (me.TeamCompeting != null) {
                        if (output == "") {
                            output = me.TeamCompeting.TeamName;
                        } else {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    } else {
                        twoNull++;
                    }

                    counter++;
                }

                if(twoNull == 2) {
                    output = "Not set yet";
                }else if(counter == 0) {
                    output = "BYE";
                } else if(counter == 1) {
                    output += ": bye";
                }


                return output;
            }
        }

        /*public static explicit operator MatchupModel(List<MatchupModel> v) {
            throw new NotImplementedException();
        }*/
    }

}
