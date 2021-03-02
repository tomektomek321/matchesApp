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
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester {

        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        //List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        List<PrizeModel> selectedPrizes = GlobalConfig.Connection.GetPrize_All();


        public CreateTournamentForm() {
            InitializeComponent();

            WireUpLists();
        }

        private void WireUpLists() {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";



            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void createPrizeButton_Click(object sender, EventArgs e) {
            // Call the CreatePrizeForm
            CreatePrizeForm frm = new CreatePrizeForm(this);

            frm.Show();
        }

        public void PrizeComplete(PrizeModel model) {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        private void CreateTournamentForm_Load(object sender, EventArgs e) {

        }

        private void addTeamButton_Click(object sender, EventArgs e) {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null) {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }

        public void TeamComplete(TeamModel model) {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        private void removeSelectedPlayersButton_Click(object sender, EventArgs e) {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (t != null) {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                WireUpLists();
            }
        }

        private void removeSelectedPrizesButton_Click(object sender, EventArgs e) {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null) {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e) {
            // Validate data
            decimal fee = 0;

            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out fee);

            if (!feeAcceptable) {
                MessageBox.Show("You need to enter a valid Entry Fee.",
                    "Invalid Fee",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Create our Tournament
            TournamentModel tm = new TournamentModel {
                TournamentName = tournamentNameValue.Text,
                EntryFee = fee,
                Prizes = selectedPrizes,
                EnteredTeams = selectedTeams
            };

            // TODO - Wire our Matchups
            TournamentLogic.CreateRounds(tm);

            // Create Tournament entry
            // Create all of the Prize entries
            // Create all of the Team entries
            GlobalConfig.Connection.CreateTournament(tm);
        }

        private void CreateTournamentForm_Load_1(object sender, EventArgs e) {

        }
    }
}
