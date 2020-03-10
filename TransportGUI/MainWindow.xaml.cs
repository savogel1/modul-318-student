using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Windows.Shapes;
using SwissTransport;
namespace TransportGUI {
   
    public partial class MainWindow : Window {
        private List<GroupBox> groupBoxes = new List<GroupBox>();
        public MainWindow() {
            InitializeComponent();
            groupBoxes.Add(connectionsGroupBox);
            groupBoxes.Add(stationsGroupBox);
            groupBoxes.Add(stationBoardGroupBox);
            switchSite(connectionsGroupBoxButton);
        }

        private void connectionsSearchButton_Click(object sender, RoutedEventArgs e) {
            connectionsListBox.Items.Clear();
            Transport transport = new Transport();
            List<Station> fromStations = transport.GetStations(connectionsStartComboBox.Text).StationList;
            List<Station> toStations = transport.GetStations(connectionsEndComboBox.Text).StationList;
            foreach (Station station in fromStations) {
                connectionsStartComboBox.Items.Add(station.Name.ToString());
            }
            foreach (Station station in toStations) {
                connectionsEndComboBox.Items.Add(station.Name.ToString());
            }

            List<Connection> connections = transport.GetConnections(connectionsStartComboBox.Text, connectionsEndComboBox.Text).ConnectionList;
            foreach (Connection connection in connections) {
                connectionsListBox.Items.Add(createConnectionOutput(connection));
            }
        }
        private void stationsSearchButton_Click(object sender, RoutedEventArgs e) {
            stationsListBox.Items.Clear();
            Transport transport = new Transport();
            List<Station> stations = transport.GetStations(stationsTextBox.Text).StationList;

            if (isExecutable(stations)) {
                foreach (Station station in stations) {
                    stationsListBox.Items.Add(station.Name.ToString());
                }
            }
        }

        private void stationBoardSearchButton_Click(object sender, RoutedEventArgs e) { 
        
        }

        private string createConnectionOutput(Connection connection) {
            string listBoxOutput = "";
            listBoxOutput += "Von: " + connectionsStartComboBox.Text;
            listBoxOutput += "      ---->       ";
            listBoxOutput += "Nach: " + connectionsEndComboBox.Text;
            listBoxOutput += "      Zeit: " + connection.Duration.ToString();
            return listBoxOutput;
        }

        private bool isExecutable(List<Connection> connection, ComboBox comboBox) {
            if (comboBox.Text.Length < 3) {
                MessageBox.Show("Geben Sie mindestens drei Buchstaben zum Suchen einer Station ein!");
                return false;
            }
            if (connection.Count == 0) {
                comboBox.Items.Add("Es wurden keine Suchergebnisse gefunden!");
                return false;
            }
            return true;
        }


        private bool isExecutable(List<Station> stations) {
            if (stationsTextBox.Text.Length < 3) {
                MessageBox.Show("Geben Sie mindestens drei Buchstaben zum Suchen einer Station ein!");
                return false;
            }
            if (stations.Count == 0) {
                stationsListBox.Items.Add("Es wurden keine Suchergebnisse gefunden!");
                return false;
            }
            return true;
        }

        private void switchSite(Button activatedButton) { 
            foreach(GroupBox groupBox in groupBoxes) {
                if (activatedButton.Name.ToString() == groupBox.Name.ToString() + "Button") {
                    groupBox.Visibility = Visibility.Visible;
                } else {
                    groupBox.Visibility = Visibility.Hidden;
                }
            }
        }

        private void stationsGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            switchSite(stationsGroupBoxButton);
        }

        private void connectionsGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            switchSite(connectionsGroupBoxButton);
        }

        private void stationBoardGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            switchSite(stationBoardGroupBoxButton);
        }
    }
}
