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
            try {
                connectionsListBox.Items.Clear();
                string selectedDate = "";

                if (connectionsDatePicker.SelectedDate != null && connectionsTimeComboBox.SelectedItem != null) {
                    string str = connectionsDatePicker.SelectedDate.ToString().Replace("00:00:00", "");
                    selectedDate = str + connectionsTimeComboBox.SelectedItem.ToString() + ":00";
                }
                if (connectionsStartComboBox.Text == "" || connectionsEndComboBox.Text == "") {
                    throw new Exception();
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
            } catch (WebException) {
                navigation.showError("Sie haben keine Internetverbindung, verbinden Sie sich mit dem Internet.");
            } catch (Exception) {
                navigation.showError("Geben Sie erst zwei gültige Stationen in die Eingabefelder ein, bevor Sie die Suche starten.");
            }

        }

        private bool isAfterSelectedDate(string selectedDate, ConnectionPoint connectionPoint) {
            DateTime selectedDateTime = DateTime.Parse(selectedDate);
            DateTime departureDateTime = DateTime.Parse(connectionPoint.Departure);

            return selectedDateTime <= departureDateTime;
        }

        private void stationsSearchButton_Click(object sender, RoutedEventArgs e) {
            bool isExceptionThrown = false;
            try {
                stationsListBox.Items.Clear();
                
                if (stationsTextBox.Text == "") {
                    isExceptionThrown = true;
                    throw new Exception();
                }
                List<Station> stations = transport.GetStations(stationsTextBox.Text).StationList;

                foreach (Station station in stations) {
                    stationsListBox.Items.Add(station.Name.ToString());
                }
            } catch (WebException) {
                navigation.showError("Sie haben keine Internetverbindung, verbinden Sie sich mit dem Internet.");
            } catch (Exception) {
                navigation.showError("Geben Sie erst etwas ins Eingabefeld ein, bevor Sie die Suche starten.");
            }
            if (stationsListBox.Items.Count == 0  && isExceptionThrown == false) {
                stationsListBox.Items.Add("Es wurden keine Suchergebnisse gefunden!");
            }
        }

        private void onSelect(object sender, RoutedEventArgs e) {
            if (stationsListBox.SelectedItem != null) {
                stationsTextBox.Text = stationsListBox.SelectedItem.ToString();
            }
        }

        private void stationsOpenMapButton_Click(object sender, RoutedEventArgs e) {
            try {
                string coordinates = getCoordinates();
                if (coordinates != "") {
                    Process.Start("https://www.google.ch/maps/search/?api=1&query=" + coordinates);
                } 
            } catch (WebException) {
                navigation.showError("Sie haben keine Internetverbindung, verbinden Sie sich mit dem Internet.");
            }
        }

        private string getCoordinates() {
            try {
                List<Station> stations = transport.GetStations(stationsTextBox.Text).StationList;
                Station station = stations.Find(x => x.Name == stationsTextBox.Text);
                station.Coordinate.XCoordinate.ToString().Replace(",", ".");
                station.Coordinate.YCoordinate.ToString().Replace(",", ".");

                return station.Coordinate.XCoordinate.ToString() + ", " + station.Coordinate.YCoordinate.ToString();
            } catch (NullReferenceException) {
                navigation.showError("Erst eine Station anklicken, bevor die Karte geöffnet werden kann.");
            }
            return "";
        }

        private void stationBoardSearchButton_Click(object sender, RoutedEventArgs e) {
            try {
                stationBoardListBox.Items.Clear();
                StationBoardRoot stationBoardRoot = transport.GetStationBoard(stationBoardTextBox.Text, "");

                foreach (StationBoard stationBoard in stationBoardRoot.Entries) {
                    string listBoxOutput = "";
                    listBoxOutput += stationBoard.Name.ToString() + "   ";
                    listBoxOutput += "   ---->   ";
                    listBoxOutput += stationBoard.To.ToString();

                    stationBoardListBox.Items.Add(listBoxOutput);
                }
            } catch (WebException) {
                navigation.showError("Sie haben keine Internetverbindung, verbinden Sie sich mit dem Internet.");
            } catch (Exception) {
                navigation.showError("Geben Sie erst etwas ins Eingabefeld ein, bevor Sie die Suche starten.");
            }
        }


        private void toCompleteStartStation(object sender, KeyEventArgs e) {
            toCompleteStation(connectionsStartComboBox);
        }

        private void toCompleteEndStation(object sender, KeyEventArgs e) {
            toCompleteStation(connectionsEndComboBox);
        }

        private void toCompleteStation(ComboBox comboBox) {
            try {
                comboBox.Items.Clear();
                List<Station> stations = transport.GetStations(comboBox.Text).StationList;

                foreach (Station station in stations) {
                    comboBox.Items.Add(station.Name.ToString());
                }
                comboBox.IsDropDownOpen = true;
            } catch (WebException) {
                navigation.showError("Sie haben keine Internetverbindung, verbinden Sie sich mit dem Internet.");
            }
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
