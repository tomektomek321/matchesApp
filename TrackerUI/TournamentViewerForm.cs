using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;

namespace TrackerUI {
    public partial class TournamentViewerForm : Form {

        private TournamentModel tournament;
        int selectedRound;
        List<int> rounds = new List<int>();
        List<MatchupModel> selectedMatchups = new List<MatchupModel>();
        List<TeamModel> entered = new List<TeamModel>();


        public TournamentViewerForm(TournamentModel tournamentModel) {
            InitializeComponent();
            
            tournament = tournamentModel;

            tournamentName.Text = tournament.TournamentName;

            selectedRound = 1;
            
            selectedMatchups = tournament.Rounds[selectedRound - 1];
            
            LoadRounds();

            WireUpLists();

            LoadMatchup(selectedMatchups.First());
        }

        private void LoadFormData() {
            
        }

        private void WireUpLists() {

            if (unplayedOnlyCheckBox.Checked) {
                selectedMatchups = (List<MatchupModel>)selectedMatchups.Where(x => x.Winner == null).ToList();
            }


            roundDropDown.DataSource = rounds;
            MatchupTextBox.DataSource = selectedMatchups;
            MatchupTextBox.DisplayMember = "DisplayName";
        }

        private void LoadRounds() {
            rounds.Clear();

            rounds.Add(1);
            int currRound = 1;

            foreach (List<MatchupModel> matchups in tournament.Rounds) {
                if (matchups.First().MatchupRound > currRound) {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                }
            }

            //LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e) {
            selectedMatchups = tournament.Rounds[(int)roundDropDown.SelectedItem - 1];
            WireUpLists();

            //LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void LoadMatchups(int round) {
            /*foreach (List<MatchupModel> matchups in tournament.Rounds) {
                if (matchups.First().MatchupRound == round) {
                    selectedMatchups.Clear();
                    foreach (MatchupModel m in matchups) {
                        selectedMatchups.Add(m);
                    }
                }
            }

            LoadMatchup(selectedMatchups.First());*/
        }

        private void LoadMatchup(MatchupModel m) {
            for (int i = 0; i < m.Entries.Count; i++) {
                if (i == 0) {
                    if (m.Entries[0].TeamCompeting != null) {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        teamTwoName.Text = "<bye>";
                        teamTwoScoreValue.Text = "0";

                    } else {
                        teamOneName.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }
                }

                if (i == 1) {
                    if (m.Entries[1].TeamCompeting != null) {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[1].Score.ToString();

                    } else {
                        teamTwoName.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e) {
            
            LoadMatchup((MatchupModel)MatchupTextBox.SelectedItem);
        }

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e) {
            WireUpLists();
        }

        private void scoreButton_Click(object sender, EventArgs e) {
            int.TryParse(teamOneScoreValue.Text, out int team1Goal);
            int.TryParse(teamTwoScoreValue.Text, out int team2Goal);

            if(team1Goal > team2Goal) {
                setWinner(1);
            } else {
                setWinner(2);
            }






        }


        private void setWinner(int team) {
            int f;
            TeamModel x = tournament.Rounds[selectedRound - 1][MatchupTextBox.SelectedIndex].Entries[team - 1].TeamCompeting;
            
            tournament.Rounds[selectedRound - 1][MatchupTextBox.SelectedIndex].Winner = x;

            WireUpLists();

        }
    }
}
