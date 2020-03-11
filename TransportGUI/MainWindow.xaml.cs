using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Diagnostics;
using System.Windows.Shapes;
using SwissTransport;
namespace TransportGUI {
   
    public partial class MainWindow : Window {
        private GroupBox[] groupBoxes;
        private Transport transport = new Transport();
        private Navigation navigation;

        public MainWindow() {
            InitializeComponent();
            groupBoxes = new GroupBox[3] { stationsGroupBox, connectionsGroupBox, stationBoardGroupBox };
            navigation = new Navigation(groupBoxes, connectionsTimeComboBox, connectionsGroupBoxButton);
            navigation.startUp();
        }

        private void connectionsSearchButton_Click(object sender, RoutedEventArgs e) {
            connectionsListBox.Items.Clear();
            string selectedDate = "";
            
            if (connectionsDatePicker.SelectedDate != null && connectionsTimeComboBox.SelectedItem != null) {
                string str = connectionsDatePicker.SelectedDate.ToString().Replace("00:00:00", "");
                selectedDate = str + connectionsTimeComboBox.SelectedItem.ToString() + ":00";
            }
            
            List<Connection> connections = transport.GetConnections(connectionsStartComboBox.Text, connectionsEndComboBox.Text).ConnectionList;

            foreach (Connection connection in connections) {
                ConnectionPoint connectionPointFrom = connection.From;
                if (selectedDate == "" || isAfterSelectedDate(selectedDate, connectionPointFrom)) {
                    string listBoxOutput = "";
                    listBoxOutput += "Von: " + connectionsStartComboBox.Text;
                    listBoxOutput += "      ---->       ";
                    listBoxOutput += "Nach: " + connectionsEndComboBox.Text;
                    listBoxOutput += "      Zeit: " + connectionPointFrom.Departure;

                    connectionsListBox.Items.Add(listBoxOutput);
                }
            }
        }

        private bool isAfterSelectedDate(string selectedDate, ConnectionPoint connectionPoint) {
            DateTime selectedDateTime = DateTime.Parse(selectedDate);
            DateTime departureDateTime = DateTime.Parse(connectionPoint.Departure);

            return selectedDateTime <= departureDateTime;
        }

        private void stationsSearchButton_Click(object sender, RoutedEventArgs e) {
            stationsListBox.Items.Clear();
            List<Station> stations = transport.GetStations(stationsTextBox.Text).StationList;

            if (isExecutable(stations)) {
                foreach (Station station in stations) {
                    stationsListBox.Items.Add(station.Name.ToString());
                }
                if (stationsListBox.Items.Count == 0) {
                    stationsListBox.Items.Add("Es wurden keine Suchergebnisse gefunden!");
                }
            }
        }

        private bool isExecutable(List<Station> stations) {
            if (stationsTextBox.Text.Length < 3) {
                MessageBox.Show("Geben Sie mindestens drei Buchstaben zum Suchen einer Station ein!");
                return false;
            }
            return true;
        }

        private void onSelect(object sender, RoutedEventArgs e) {
            if (stationsListBox.SelectedItem != null) { 
                stationsTextBox.Text = stationsListBox.SelectedItem.ToString();
            }
        }

        private void stationsOpenMapButton_Click(object sender, RoutedEventArgs e) {
            Process.Start("https://www.google.ch/maps/search/?api=1&query=" + getCoordinates());
        }

        private string getCoordinates() {
            List<Station> stations = transport.GetStations(stationsTextBox.Text).StationList;
            Station station = stations.Find(x => x.Name == stationsTextBox.Text);
            station.Coordinate.XCoordinate.ToString().Replace(",", ".");
            station.Coordinate.YCoordinate.ToString().Replace(",", ".");

            return station.Coordinate.XCoordinate.ToString() + ", " + station.Coordinate.YCoordinate.ToString();
        }

        private void stationBoardSearchButton_Click(object sender, RoutedEventArgs e) {
            stationBoardListBox.Items.Clear();
            StationBoardRoot stationBoardRoot = transport.GetStationBoard(stationBoardTextBox.Text, "");

            foreach (StationBoard stationBoard in stationBoardRoot.Entries) {
                string listBoxOutput = "";
                listBoxOutput += stationBoard.Name.ToString() + "   ";
                listBoxOutput += "   ---->   ";
                listBoxOutput += stationBoard.To.ToString();

                stationBoardListBox.Items.Add(listBoxOutput);
            }
        }

        private void toCompleteStartStation(object sender, KeyEventArgs e) {
            connectionsStartComboBox.Items.Clear();
            List<Station> stations = transport.GetStations(connectionsStartComboBox.Text).StationList;

            foreach (Station station in stations) {
                connectionsStartComboBox.Items.Add(station.Name.ToString());
            }
            connectionsStartComboBox.IsDropDownOpen = true;
        }

        private void toCompleteEndStation(object sender, KeyEventArgs e) {
            connectionsEndComboBox.Items.Clear();
            List<Station> stations = transport.GetStations(connectionsEndComboBox.Text).StationList;

            foreach (Station station in stations) {
                connectionsEndComboBox.Items.Add(station.Name.ToString());
            }
            connectionsEndComboBox.IsDropDownOpen = true;
        }

        private void stationsGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            navigation.switchSite(stationsGroupBoxButton);
        }

        private void connectionsGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            navigation.switchSite(connectionsGroupBoxButton);
        }

        private void stationBoardGroupBoxButton_Click(object sender, RoutedEventArgs e) {
            navigation.switchSite(stationBoardGroupBoxButton);
        }
    }
}
