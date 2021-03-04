using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess {
    public interface IDataConnection {
        PrizeModel CreatePrize(PrizeModel model);
        PersonModel CreatePerson(PersonModel model);
        List<PersonModel> GetPerson_All();
        TeamModel CreateTeam(TeamModel model);
        void CreateTournament(TournamentModel model);
        List<TeamModel> GetTeam_All();
        List<PrizeModel> GetPrize_All();
        List<TournamentModel> GetTournament_All();
        List<TournamentModel> GetTournament_AllBasics();
        List<MatchupModel> getMatchupObject();
        List<MatchupEntryModel> getMatchupEntriesObject();
        void getRoundsOfTournament(TournamentModel model);
        void UpdateTournament(TournamentModel model);

    }
}
