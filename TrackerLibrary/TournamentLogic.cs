using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary {

    public static class TournamentLogic {

        // Order our list randomly of teams
        // Check if it is big enough - if not, add in byes - 2*2*2*2 - 2^4
        // Create our first round of matchups
        // Create every round after that - 8 matchups - 4 matchups - 2 matchups - 1 matchup

        private static int matchEntryId = 0;
        private static int matchId = 0;

        public static void CreateRounds(TournamentModel model) {
            
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int[] byes = NumberOfFirstRoundMatchesAndByes(randomizedTeams.Count);

            int[] tabOfByes = createTabOfByes(byes);

            model.Rounds.Add(CreateFirstround(tabOfByes[1], randomizedTeams, tabOfByes));

            CreateOtherRounds(model, rounds);
        }


        public static int[] createTabOfByes(int[] byes_) {

            int numberOfTeams = byes_[0];
            int[] mainTab = new int[numberOfTeams];
            int byes = byes_[1];

            for (int i = 0; i < mainTab.Length; i++) {
                mainTab[i] = 0;
            }

            int equalDistance = byes_[0] / byes;

            int added = 0;
            for (int tabIndex = 0; tabIndex < numberOfTeams; tabIndex++) {

                if (tabIndex % equalDistance == 0) {
                    mainTab[tabIndex] = 1;
                    added++;
                }

                if (added == byes) {
                    break;
                }

            }

            return mainTab;
        }


        private static void CreateOtherRounds(TournamentModel model, int rounds) {

            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel { Id = matchId++ };

            while (round <= rounds) {
                foreach (MatchupModel match in previousRound) {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match, Id = matchEntryId++ });

                    if (currMatchup.Entries.Count > 1) {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel { Id = matchId++ };
                    }
                }

                model.Rounds.Add(currRound);
                previousRound = currRound;

                currRound = new List<MatchupModel>();
                round++;
            }
        }

        private static List<MatchupModel> CreateFirstround(int byes, List<TeamModel> teams, int[] tabOfByes) {

            List<MatchupModel> output = new List<MatchupModel>();


            int addedTeams = 0;
            for(int i=0; i < tabOfByes.Length; i++, i++) {

                MatchupModel curr = new MatchupModel { MatchupRound = 1, Id = matchId++ };

                if (tabOfByes[i] != 1) {
                    curr.Entries.Add(new MatchupEntryModel { TeamCompeting = teams[addedTeams++], Id = matchEntryId++ });
                }

                if (tabOfByes[i + 1] != 1) {
                    curr.Entries.Add(new MatchupEntryModel { TeamCompeting = teams[addedTeams++], Id = matchEntryId++ });
                }


                output.Add(curr);
                /*if (byes > 0) {
                    if(tabOfByes[i] == 1) {
                        curr.Entries.Add(new MatchupEntryModel { TeamCompeting = teams[i] });
                    }
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();
                    if (byes > 0) {
                        byes--;
                    }
                }*/
            }

            return output;
        }

        private static int[] NumberOfFirstRoundMatchesAndByes(/*int rounds, */int numberOfTeams) {

            int numOfTeams = numberOfTeams;

            int firstRoundMatchesCount = 1;

            bool foound = false;
            while (!foound) {
                if (firstRoundMatchesCount < numOfTeams) {
                    firstRoundMatchesCount *= 2;
                } else {
                    foound = true;
                }

            }

            int byes = firstRoundMatchesCount - numOfTeams;

            //firstRoundMatchesCount = firstRoundMatchesCount / 2;

            int[] ret = new int[] { firstRoundMatchesCount, byes };

            return ret;


            /*double powek = double.Parse(2.ToString());
            double x = double.Parse(numberOfTeams.ToString());

            double numberOfMatches = Math.Pow(powek, x);




            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i < rounds; i++) {
                totalTeams *= 2;
            }

            output = Math.Abs(totalTeams - numberOfTeams);

            return output;*/
        }

        private static int FindNumberOfRounds(int teamCount) {
            int output = 1;
            int val = 2;

            while (val < teamCount) {
                output++;
                val *= 2;
                Console.WriteLine(output);
            }

            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams) {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
