using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary {

    public static class TournamentLogic {
        
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
                    if (round == 2 && match.Entries.Count == 1) {
                        match.Winner = match.Entries[0].TeamCompeting;
                        currMatchup.Entries.Add(new MatchupEntryModel {
                            ParentMatchup = match,
                            Id = matchEntryId++,
                            TeamCompeting = match.Winner
                        });
                    } else {
                        currMatchup.Entries.Add(new MatchupEntryModel {
                            ParentMatchup = match,
                            Id = matchEntryId++
                        });
                    }
                    
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
            bool matchHasBye = false;
            int[] tabOfMatchesWithByes = new int[tabOfByes.Length];
            int tabIndex = 0;

            for(int i=0; i < tabOfByes.Length; i++, i++) {

                MatchupModel curr = new MatchupModel { MatchupRound = 1, Id = matchId++ };

                if (tabOfByes[i] != 1) {
                    curr.Entries.Add(new MatchupEntryModel { TeamCompeting = teams[addedTeams++], Id = matchEntryId++ });
                    
                } else {
                    matchHasBye = true;
                }

                if (tabOfByes[i + 1] != 1) {
                    curr.Entries.Add(new MatchupEntryModel { TeamCompeting = teams[addedTeams++], Id = matchEntryId++ });
                    
                } else {
                    matchHasBye = true;
                }

                if(matchHasBye) {
                    tabOfMatchesWithByes[tabIndex++] = matchId - 1;
                }


                matchHasBye = false;
                output.Add(curr);
            }

            return output;
        }

        private static int[] NumberOfFirstRoundMatchesAndByes(int numberOfTeams) {

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

            int[] ret = new int[] { firstRoundMatchesCount, byes };

            return ret;
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
