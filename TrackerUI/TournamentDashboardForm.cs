using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI {
    public partial class TournamentDashboardForm : Form {

        //List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournament_All();
        List<TournamentModel> tournamentsBasic = GlobalConfig.Connection.GetTournament_AllBasics();

        public TournamentDashboardForm() {

            tescik();

            InitializeComponent();

            WireUpLists();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e) {
            TournamentModel tm = (TournamentModel)loadExistingTournamentDropDown.SelectedItem;

            if (tm.Rounds.Count == 0) {
                GlobalConfig.Connection.getRoundsOfTournament(tm);
            }


            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
        }

        private void createTournamentButton_Click(object sender, EventArgs e) {
            CreateTournamentForm frm = new CreateTournamentForm();
            frm.Show();
        }

        private void WireUpLists() {
            loadExistingTournamentDropDown.DataSource = tournamentsBasic;
            loadExistingTournamentDropDown.DisplayMember = "TournamentName";
        }

        public static void tescik() {

            int numOfTeams = 4;

            int firstRoundMatchesCount = 1;

            bool foound = false;
            while(!foound) {
                if(firstRoundMatchesCount < numOfTeams) {
                    firstRoundMatchesCount *= 2;               
                } else {
                    foound = true;
                }
                
            }

            int byes = firstRoundMatchesCount - numOfTeams;
            
            firstRoundMatchesCount = firstRoundMatchesCount / 2;

            


            /*double numOfTeams = 17;

            double rounds = 1;
            int xtempDoubler = 2;

            while (xtempDoubler < numOfTeams) {
                rounds++;
                xtempDoubler *= 2;
            }

            double Xpowek = double.Parse(2.ToString());

            double numberOfMatches = Math.Pow(Xpowek, rounds);

            double byes = numberOfMatches - numOfTeams;

            int[] mainTab = new int[(int)numberOfMatches];

            for (int i = 0; i < mainTab.Length; i++) {
                mainTab[i] = 0;
            }

            double space = numberOfMatches / byes;
            
            int equalDistance = (int)numberOfMatches / (int)byes;

            double neededIterateForMorePause = ((numberOfMatches / byes) - equalDistance);

            neededIterateForMorePause = Math.Round(Math.Abs(1 / neededIterateForMorePause));

            int added = 0;
            for (int tabIndex = 0; tabIndex < (numberOfMatches); tabIndex++) {

                if(tabIndex  % equalDistance == 0) {
                    mainTab[tabIndex] = 1;
                    added++;
                }

                if(added == (int)byes) {
                    break;
                }
                
            }*/


            
        }



    }


}
