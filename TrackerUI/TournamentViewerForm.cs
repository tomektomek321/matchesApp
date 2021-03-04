using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;
using System.IO;
using TrackerLibrary;

namespace TrackerUI {
    public partial class TournamentViewerForm : Form {

        private TournamentModel tournament;
        int selectedRound = 1;
        List<int> rounds = new List<int>();
        List<MatchupModel> selectedMatchups = new List<MatchupModel>();
        List<TeamModel> entered = new List<TeamModel>();

        IFileUpdateAccess saverObject = new FileUpdaterProcessor();
        IDataConnection textConnector = new TextConnector();


        public TournamentViewerForm(TournamentModel tournamentModel) {
            InitializeComponent();

            tournament = tournamentModel;

            tournamentName.Text = tournament.TournamentName;

            LoadRounds();

            selectedMatchups = tournament.Rounds[selectedRound - 1];

            WireUpLists();

            LoadMatchup(selectedMatchups.First());
        }

        private void LoadFormData() {

        }

        private void WireUpLists() {

            if (unplayedOnlyCheckBox.Checked) {
                selectedMatchups = (List<MatchupModel>)selectedMatchups.Where(x => x.Winner == null).ToList();
            } else {
                selectedMatchups = tournament.Rounds[selectedRound - 1];
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

            roundDropDown.DataSource = rounds;

        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e) {

            selectedRound = roundDropDown.SelectedIndex + 1;

            WireUpLists();
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

            MatchupModel thisMatch = tournament.Rounds[selectedRound - 1][MatchupTextBox.SelectedIndex];

            if (thisMatch.Winner != null) {
                // winnner already
                teamOneScoreValue.Text = thisMatch.Entries[0].Score.ToString();
                teamTwoScoreValue.Text = thisMatch.Entries[1].Score.ToString();
                
            } else if (team1Goal > team2Goal) {
                setWinner(1);
            } else if (team1Goal < team2Goal) {
                setWinner(2);
            }

        }


        private void setWinner(int winnerEntryIndex) {

            MatchupModel thisMatch = tournament.Rounds[selectedRound - 1][MatchupTextBox.SelectedIndex];
            
            WireUpLists();
            
            if (thisMatch.Winner == null) {
                thisMatch.Winner = thisMatch.Entries[winnerEntryIndex - 1].TeamCompeting;
            }
            
            int entryIndex = 0;
            foreach (MatchupEntryModel entry in thisMatch.Entries) {
                if (entryIndex == 0) {
                    entry.Score = double.Parse(teamOneScoreValue.Text);
                    entryIndex++;
                } else if (entryIndex == 1) {
                    entry.Score = double.Parse(teamTwoScoreValue.Text);
                }
            }
            
            if (selectedRound < tournament.Rounds.Count) {  // if next round exists
                
                foreach (MatchupModel matchup_ in tournament.Rounds[selectedRound]) {
                    
                    foreach (MatchupEntryModel entry in matchup_.Entries) {

                        if (entry.ParentMatchup != null  // this match is a parent for next round match
                            && entry.ParentMatchup.Id.ToString() != null 
                            && (thisMatch.Id == entry.ParentMatchup.Id)) {

                            entry.TeamCompeting = thisMatch.Winner; // 2teams, so choose winner
                        }
                        
                    }
                }
            }
            
            GlobalConfig.Connection.UpdateTournament(tournament);

        }
    }
}
