using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SwissTransport;
using System.Windows;

namespace TransportGUI {
    class Navigation {
        private GroupBox[] groupBoxes;
        private ComboBox connectionsTimeComboBox;
        private Button startSiteButton;

        public Navigation(GroupBox[] groupBoxes, ComboBox comboBox, Button button) {
            this.groupBoxes = groupBoxes;
            this.connectionsTimeComboBox = comboBox;
            this.startSiteButton = button;
        }

        public void startUp() {
            switchSite(startSiteButton);
            connectionsTimeComboBoxAddItems();
        }

        public void switchSite(Button activatedButton) {

            foreach (GroupBox groupBox in groupBoxes) {
                if (activatedButton.Name.ToString() == groupBox.Name.ToString() + "Button") {
                    groupBox.Visibility = Visibility.Visible;
                } else {
                    groupBox.Visibility = Visibility.Hidden;
                }
            }
        }

        public void showError(string message) {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void connectionsTimeComboBoxAddItems() {
            for (int i = 00; i <= 23; i++) {
                string timeString = "";
                if (i < 10) {
                    timeString = "0";
                }
                timeString += i + ":00";
                connectionsTimeComboBox.Items.Add(timeString);
            }
        }
    }
}
