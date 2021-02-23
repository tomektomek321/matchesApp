using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess {
    public interface IFileUpdateAccess {
        
        string ConvertMatchupListToStringAndSaveInFile(TournamentModel matches);
        string ConvertMatchupEntriesListToStringAndSaveInFile(TournamentModel matches);



    }
}
