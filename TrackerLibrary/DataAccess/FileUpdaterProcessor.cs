using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.IO;
using System.Configuration;

namespace TrackerLibrary.DataAccess {
    public class FileUpdaterProcessor : IFileUpdateAccess {

        private const string MatchupFile = "MatchupModel.csv";
        private const string MatchupEntryFile = "MatchupEntryModel.csv";

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries) {
            string output = string.Empty;

            if (entries.Count == 0) {
                return "";
            }

            foreach (MatchupEntryModel e in entries) {
                output += $"{ e.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output.Trim('|');
        }

        public static string ConvertMatchupListToString(List<MatchupModel> matchups) {
            string output = string.Empty;

            if (matchups.Count == 0) {
                return "";
            }

            foreach (MatchupModel m in matchups) {
                output += $"{ m.Id }^";
            }

            output = output.Substring(0, output.Length - 1);

            return output.Trim('|');
        }

        public string ConvertMatchupListToStringAndSaveInFile(TournamentModel tournament) {
            List<string> lines = new List<string>();
            
            string winnerek;
            double score1, score2;
            string teamNames;
            int indexik = 0;

            int lastRound = tournament.Rounds.Count;

            foreach (List<MatchupModel> matchups in tournament.Rounds) {
                foreach (MatchupModel m in matchups) {
                    if (m.Winner == null) {
                        winnerek = "";
                    } else {
                        winnerek = m.Winner.Id.ToString();
                    }
                
                    indexik = 0;
                    teamNames = "";
                    foreach (MatchupEntryModel entry in m.Entries) {
                        indexik++;

                        if (indexik == 1) {
                            score1 = entry.Score;
                            
                             teamNames = (entry.Id).ToString();
                            
                        } else if (indexik == 2) {
                            score2 = entry.Score;
                            
                             teamNames += $"|{(entry.Id).ToString()}";
                            
                        }
                    }
                
                    lines.Add($"{ m.Id },{ teamNames },{ winnerek },{ m.MatchupRound }");
                
                }
            }

            File.Delete($"{ ConfigurationManager.AppSettings["filePath"] }\\{ MatchupFile }");
            
            File.WriteAllLines($"{ ConfigurationManager.AppSettings["filePath"] }\\{ MatchupFile }", lines);
            return "";
        }

        public string ConvertMatchupEntriesListToStringAndSaveInFile(TournamentModel tournament) {
            // Id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3   entry
            List<string> linesEntiries = new List<string>();
            string teamNames;
            //int newId;
            int indexik = 0;
            int idCounter = 0;
            string parent;

            foreach (List<MatchupModel> matchups in tournament.Rounds) {
                
                foreach (MatchupModel m in matchups) {
                    int isNew = 0;
                    indexik = 0;
                    teamNames = "";
                    foreach (MatchupEntryModel entry in m.Entries) {
                        isNew++;
                        int score;
                        indexik++;
                        idCounter++;
                        
                        score = int.Parse(entry.Score.ToString());

                        if (entry.TeamCompeting == null || entry.TeamCompeting.Id == 0) {
                            teamNames = "";
                        } else {
                            teamNames = (entry.TeamCompeting.Id).ToString();
                        }

                        if (entry.ParentMatchup == null) {
                            parent = "";
                        } else {
                            parent = entry.ParentMatchup.Id.ToString();
                        }

                        linesEntiries.Add($"{ entry.Id },{ teamNames },{ score },{ parent }");

                    }

                    if(isNew == 0) {
                        linesEntiries.Add($"{ ++idCounter },,0,");
                        
                    } else if(isNew > 0) {


                    }



                }
            }

            File.Delete($"{ ConfigurationManager.AppSettings["filePath"] }\\{ MatchupEntryFile }");

            File.WriteAllLines($"{ ConfigurationManager.AppSettings["filePath"] }\\{ MatchupEntryFile }", linesEntiries);
            
            return "";
        }


    }
}
