using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess {
    public class TextConnector : IDataConnection {

        private const string PrizesFile = "PrizeModel.csv";
        private const string PeopleFile = "PersonModel.csv";
        private const string TeamFile = "TeamModel.csv";
        private const string TournamentFile = "TournamentModel.csv";
        private const string MatchupFile = "MatchupModel.csv";
        private const string MatchupEntryFile = "MatchupEntryModel.csv";
        
        public PersonModel CreatePerson(PersonModel model) {
            // Load the text file
            // Convert the text to a List<PersonModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            int currentId = 1;

            if (people.Count > 0) {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            // Add the new record with the new ID
            people.Add(model);

            // Convert the people to List<string>
            // Save the list<string> to the text file
            people.SaveToPeopleFile(PeopleFile);

            return model;
        }

        public List<MatchupModel> getMatchupObject() {
            return MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();
        }

        public List<MatchupEntryModel> getMatchupEntriesObject() {
            return MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
        }
        
        public TeamModel CreateTeam(TeamModel model) {
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            // Find the ID
            int currentId = 1;

            if (teams.Count > 0) {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile(TeamFile);

            return model;
        }
        
        public PrizeModel CreatePrize(PrizeModel model) {
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the ID
            int currentId = 1;

            if (prizes.Count > 0) {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            // Add the new record with the new ID
            prizes.Add(model);

            // Convert the prizes to List<string>
            // Save the list<string> to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }
        
		public List<PersonModel> GetPerson_All() {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All() {
            return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        public List<PrizeModel> GetPrize_All() {
            return PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
        }

        public void CreateTournament(TournamentModel model) {
            List<TournamentModel> tournaments = TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
                // id, name, fee, teams, prizeId, 
            int currentId = 1;

            if (tournaments.Count > 0) {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            string newMatchupFile = MatchupFile.Substring(0, 12) + "" + currentId + ".csv";
            string newMatchupEntryFile = MatchupEntryFile.Substring(0, 17) + "" + currentId + ".csv";

            model.Id = currentId;

            model.SaveRoundsToFile(newMatchupFile, newMatchupEntryFile);

            tournaments.Add(model);

            tournaments.SaveToTournamentFile(TournamentFile);
        }

        public List<TournamentModel> GetTournament_All() {
            return TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
                //.ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
        }

        public List<TournamentModel> GetTournament_AllBasics() {
            return TournamentFile
                .FullFilePath()
                .LoadFile()
                .GetTournament_AllBasics(TeamFile, PeopleFile, PrizesFile);
            //.ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
        }

        public void getRoundsOfTournament(TournamentModel model) {
            TournamentFile
                .FullFilePath()
                .LoadFile()
                .getRoundsOfTournament(model, TeamFile, PeopleFile, PrizesFile);
            //.ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
        }
        
    }
}
