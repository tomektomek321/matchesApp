using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrackerLibrary.Models;
using System.Configuration;

namespace TrackerLibrary.DataAccess.TextHelpers {

    public static class TextConnectorProcessor {


        private static List<string> matchupsLines;
        private static List<string> matchupsEntryLines;
        
                                                                                
        public static string FullFilePath(this string fileName) {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        public static List<string> LoadFile(this string file) {
            if (!File.Exists(file)) {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines) {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel {
                    Id = int.Parse(cols[0]),
                    PlaceNumber = int.Parse(cols[1]),
                    PlaceName = cols[2],
                    PrizeAmount = decimal.Parse(cols[3]),
                    PrizePercentage = double.Parse(cols[4])
                };

                output.Add(p);
            }

            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines) {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel {
                    Id = int.Parse(cols[0]),
                    FirstName = cols[1],
                    LastName = cols[2],
                    EmailAddress = cols[3],
                    CellPhoneNumber = cols[4]
                };

                output.Add(p);
            }

            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName) {
            //Id, Team Name, list of IDs separated by pipe
            //3, Fred's Team, 1|3|5
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel {
                    Id = int.Parse(cols[0]),
                    TeamName = cols[1],
                };

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds) {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> GetTournament_AllBasics(this List<string> lines, string teamFileName, string peopleFileName, string prizeFileName) {
            // Id = 0  TournamentName = 1  EntryFee = 2
            // EnteredTeams = 3  Prizes = 4 Rounds = 5
            //Id, TournamentName, EntryFee, (Id|Id|Id - Entered Teams), (Id|Id|Id - Prizes), (Rounds - Id^Id^Id^|Id^Id^Id|Id^Id^Id)

            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            //List<MatchupModel> matchups;
            //List<MatchupEntryModel> matchupsEntry = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);
                
                output.Add(tm);
            }

            return output;
        }

        public static void getRoundsOfTournament(this List<string> lines, TournamentModel model, string teamFileName, string peopleFileName, string prizeFileName) {

            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            
            TournamentModel output = new TournamentModel();
            
            foreach (string line in lines) {
                string[] cols = line.Split(',');
                
                if (int.Parse(cols[0]) == model.Id) {
                    
                    string newMatchupFile = GlobalConfig.MatchupFile.Substring(0, 12) + "" + model.Id + ".csv";

                    List<MatchupModel> matchups = newMatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels(model.Id);

                    string newMatchupEntryFile = GlobalConfig.MatchupEntryFile.Substring(0, 17) + "" + model.Id + ".csv";

                    List<MatchupEntryModel> matchupsEntries = newMatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

                    string[] teamIds = cols[3].Split('|');

                    foreach (string Id in teamIds) {
                        model.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(Id)).First());
                    }

                    if (cols[4].Length > 0) {
                        string[] prizeIds = cols[4].Split('|');

                        foreach (string Id in prizeIds) {
                            model.Prizes.Add(prizes.Where(x => x.Id == int.Parse(Id)).First());
                        }
                    }
                    
                    string[] rounds = cols[5].Split('|');

                    foreach(string round in rounds) {
                        
                        string[] matches = round.Split('^');

                        List<MatchupModel> tempMatchups = new List<MatchupModel>();

                        foreach (string matchIdText in matches) {

                            MatchupModel tempMatchup = matchups.Where(x => x.Id == int.Parse(matchIdText)).First();
                            
                            tempMatchups.Add(tempMatchup);
                        }

                        model.Rounds.Add(tempMatchups);
                    }

                    break;
                }
                
            }
            
        }
        
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, string teamFileName, string peopleFileName, string prizeFileName) {
            // Id = 0  TournamentName = 1  EntryFee = 2
            // EnteredTeams = 3  Prizes = 4  Rounds = 5
            //Id, TournamentName, EntryFee, (Id|Id|Id - Entered Teams), (Id|Id|Id - Prizes), (Rounds - Id^Id^Id^|Id^Id^Id|Id^Id^Id)

            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups;
            //List<MatchupEntryModel> matchupsEntry = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string Id in teamIds) {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(Id)).First());
                }

                if (cols[4].Length > 0) {
                    string[] prizeIds = cols[4].Split('|');

                    foreach (string Id in prizeIds) {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(Id)).First());
                    }
                }

                string newMatchupFile = GlobalConfig.MatchupFile.Substring(0, 12) + "" + tm.Id + ".csv";
                matchups = newMatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels(tm.Id);

                string[] rounds = cols[5].Split('|');

                foreach (string round in rounds) {
                    string[] msText = round.Split('^');
                    List<MatchupModel> ms = new List<MatchupModel>();
                    
                    foreach (string matchupModelTextId in msText) {
                        
                        MatchupModel a = matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First();
                        
                        ms.Add(a);
                        
                    }

                    tm.Rounds.Add(ms);
                }

                output.Add(tm);
            }

            return output;
        }

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName) {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models) {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellPhoneNumber }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName) {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models) {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName) {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models) {
                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile) {

            matchupsLines = new List<string>();
            matchupsEntryLines = new List<string>();

            //int fileId = model.Id;
            
            foreach (List<MatchupModel> round in model.Rounds) {
                foreach (MatchupModel matchup in round) {
                    matchup.convertMatchUpToString(matchupFile, matchupEntryFile);
                }
            }
            
            File.WriteAllLines(matchupFile.FullFilePath(), matchupsLines);
            File.WriteAllLines(matchupEntryFile.FullFilePath(), matchupsEntryLines);
        }

        public static void convertMatchUpToString(this MatchupModel matchup, string matchupFile, string matchupEntryFile) {

            foreach (MatchupEntryModel entry in matchup.Entries) {
                entry.convertEntryToString(matchupEntryFile);
            }

            List<string> lines = new List<string>();

            string winner = (matchup.Winner != null) ? matchup.Winner.Id.ToString() : "";
            
            matchupsLines.Add($"{ matchup.Id },{ ConvertMatchupEntryListToString(matchup.Entries) },{ winner },{ matchup.MatchupRound }");
            
        }

        public static void convertEntryToString(this MatchupEntryModel entry, string matchupEntryFile) {

            string parent = (entry.ParentMatchup != null) ? entry.ParentMatchup.Id.ToString() : null;
            string teamCompeting = (entry.TeamCompeting != null) ? entry.TeamCompeting.Id.ToString() : null;

            matchupsEntryLines.Add($"{ entry.Id },{ teamCompeting },{ entry.Score },{ parent }");

        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines) {
            // Id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3

            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                MatchupEntryModel me = new MatchupEntryModel();

                me.Id = int.Parse(cols[0]);

                if (cols[1].Length == 0) {
                    me.TeamCompeting = null;
                } else {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }

                me.Score = double.Parse(cols[2]);

                int parentId = 0;
                if (int.TryParse(cols[3], out parentId)) {
                    me.ParentMatchup = LookupMatchupById(parentId);
                } else {
                    me.ParentMatchup = null;
                }

                output.Add(me);
            }

            return output;
        }

        private static List<MatchupEntryModel> ConvertStingToMatchupEntryModels(string input, int modelId) {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            string newMatchupEntryFile = GlobalConfig.MatchupEntryFile.Substring(0, 17) + "" + modelId + ".csv";

            List<string> entries = newMatchupEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids) {
                foreach (string entry in entries) {
                    string[] cols = entry.Split(',');

                    if (cols[0] == id) {
                        matchingEntries.Add(entry);
                    }
                }
            }

            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static TeamModel LookupTeamById(int id) {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();

            foreach (string team in teams) {
                string[] cols = team.Split(',');

                if (cols[0] == id.ToString()) {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }

            return null;
        }

        private static MatchupModel LookupMatchupById(int id) {
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups) {
                string[] cols = matchup.Split(',');

                if (cols[0] == id.ToString()) {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines, int modelId = 0) {
            // Id = 0, Entries = 1(pipe delimited by Id), Winner = 2, MatchupRound = 3

            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines) {
                string[] cols = line.Split(',');

                MatchupModel m = new MatchupModel();

                m.Id = int.Parse(cols[0]); ;
                m.Entries = ConvertStingToMatchupEntryModels(cols[1], modelId);

                if (cols[2].Length == 0) {
                    m.Winner = null;
                } else {
                    m.Winner = LookupTeamById(int.Parse(cols[2]));
                }

                m.MatchupRound = int.Parse(cols[3]);

                output.Add(m);
            }

            return output;
        }
        
        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName) {
            // Id = 0
            // TournamentName = 1
            // EntryFee = 2
            // EnteredTeams = 3
            // Prizes = 4
            // Rounds = 5 (Id^Id^Id^|Id^Id^Id|Id^Id^Id)

            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models) {
                lines.Add($"{ tm.Id },{ tm.TournamentName },{ tm.EntryFee },{ ConvertTeamListToString(tm.EnteredTeams) },{ ConvertPrizeListToString(tm.Prizes) },{ ConvertRoundListToString(tm.Rounds) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds) {
            // (Id^Id^Id^|Id^Id^Id|Id^Id^Id)

            string output = string.Empty;

            if (rounds.Count == 0) {
                return "";
            }

            foreach (List<MatchupModel> r in rounds) {
                output += $"{ ConvertMatchupListToString(r) }|";
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
        
        private static string ConvertPrizeListToString(List<PrizeModel> prizes) {
            string output = string.Empty;

            if (prizes.Count == 0) {
                return "";
            }

            foreach (PrizeModel p in prizes) {
                output += $"{ p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output.Trim('|');
        }

        private static string ConvertTeamListToString(List<TeamModel> teams) {
            string output = string.Empty;

            if (teams.Count == 0) {
                return "";
            }

            foreach (TeamModel t in teams) {
                output += $"{ t.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output.Trim('|');
        }

        private static string ConvertPeopleListToString(List<PersonModel> people) {
            string output = string.Empty;

            if (people.Count == 0) {
                return "";
            }

            foreach (PersonModel p in people) {
                output += $"{ p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output.Trim('|');
        }

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


    }
}
