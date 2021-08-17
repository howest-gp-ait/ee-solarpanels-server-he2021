using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ait.SolarPanels.Core.Services;
using Ait.SolarPanels.Core.Entities;
using Ait.SolarPanels.Core.SocketHelpers;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Ait.SolarPanels.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        PanelService panelService;
        Socket mainSocket;
        IPEndPoint myEndpoint;
        bool serverOnline;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // bij de opstart van de server wordt het aantal panelen random bepaald.
            panelService = new PanelService();
            Random rnd = new Random();
            int panelCount = rnd.Next(3, 11);  //we beschikken over 3 tot 10 panelen
            for(int i=1; i <= panelCount;i++)
            {
                panelService.AddPanel(i);
            }
            lstPanels.ItemsSource = panelService.Panels;
            StartupConfig();
            tblCommunication.Text = HandleInstruction("GETDATA|3") + tblCommunication.Text;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnStopServer_Click(null, null);
        }
        public static void DoEvents()
        {
            try
            {
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch (Exception fout)
            {
                System.Windows.Application.Current.Dispatcher.DisableProcessing();
            }
        }
        private void StartupConfig()
        {
            cmbIPs.ItemsSource = IPv4.GetActiveIP4s();
            for (int port = 49200; port <= 49500; port++)
            {
                cmbPorts.Items.Add(port);
            }
            Config.GetConfig(out string savedIP, out int savedPort);
            try
            {
                cmbIPs.SelectedItem = savedIP;
            }
            catch
            {
                cmbIPs.SelectedItem = "127.0.0.1";
            }
            try
            {
                cmbPorts.SelectedItem = savedPort;
            }
            catch
            {
                cmbPorts.SelectedItem = 49200;
            }
            btnStartServer.Visibility = Visibility.Visible;
            btnStopServer.Visibility = Visibility.Hidden;
        }
        private void CmbIPs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveConfig();        }

        private void CmbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveConfig();
        }
        private void SaveConfig()
        {
            if (cmbIPs.SelectedItem == null) return;
            if (cmbPorts.SelectedItem == null) return;

            string ip = cmbIPs.SelectedItem.ToString();
            int port = int.Parse(cmbPorts.SelectedItem.ToString());
            Config.WriteConfig(ip, port);
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.Visibility = Visibility.Hidden;
            btnStopServer.Visibility = Visibility.Visible;
            cmbIPs.IsEnabled = false;
            cmbPorts.IsEnabled = false;
            tblCommunication.Text = "";

            StartTheServer();
            StartListening();

        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.Visibility = Visibility.Visible;
            btnStopServer.Visibility = Visibility.Hidden;
            cmbIPs.IsEnabled = true;
            cmbPorts.IsEnabled = true;

            CloseTheServer();
        }
        private void CloseTheServer()
        {
            serverOnline = false;
            try
            {
                if (mainSocket != null)
                    mainSocket.Close();
            }
            catch
            { }
            mainSocket = null;
            myEndpoint = null;
        }
        private void StartTheServer()
        {
            serverOnline = true;
        }
        private void StartListening()
        {
            IPAddress ip = IPAddress.Parse(cmbIPs.SelectedItem.ToString());
            int port = int.Parse(cmbPorts.SelectedItem.ToString());
            myEndpoint = new IPEndPoint(ip, port);
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                mainSocket.Bind(myEndpoint);
                mainSocket.Listen(int.MaxValue);
                while (serverOnline)
                {
                    DoEvents();
                    if (mainSocket != null)
                    {
                        if (mainSocket.Poll(200000, SelectMode.SelectRead))
                        {
                            HandleClientCall(mainSocket.Accept());
                        }
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void HandleClientCall(Socket clientCall)
        {
            byte[] clientRequest = new Byte[1024];
            string instruction = null;

            while (true)
            {

                int numByte = clientCall.Receive(clientRequest);
                instruction += Encoding.ASCII.GetString(clientRequest, 0, numByte);
                if (instruction.IndexOf("##EOM") > -1)
                    break;
            }
            string serverResponseInText = HandleInstruction(instruction);
            tblCommunication.Text = "======================\n" + tblCommunication.Text;
            tblCommunication.Text = serverResponseInText + "\n" + tblCommunication.Text;



            if (serverResponseInText != "")
            {
                byte[] serverResponse = Encoding.ASCII.GetBytes(serverResponseInText);
                clientCall.Send(serverResponse);
            }
            clientCall.Shutdown(SocketShutdown.Both);
            clientCall.Close();
        }
        private string HandleInstruction(string instruction)
        {
            instruction = instruction.Replace("##EOM", "").Trim().ToUpper();
            if(instruction.Length >= 3 && instruction.Substring(0,3) == "ADD")
            {
                string[] parts = instruction.Split('|');
                if (parts.Length != 3)
                    return "";
                int surface;
                int maxPowerPerSquareMeter;
                int.TryParse(parts[1], out surface);
                int.TryParse(parts[2], out maxPowerPerSquareMeter);
                int id = panelService.Panels.Count + 1;
                panelService.AddPanel(id, surface, maxPowerPerSquareMeter);

                tblCommunication.Text = $"New panel added : ID = {id} - Surface = {surface}m² - Pw/m² = {maxPowerPerSquareMeter}W\n" + tblCommunication.Text;
                tblCommunication.Text = $"Sending new panelset to controller ...\n" + tblCommunication.Text;

                StringBuilder sb = new StringBuilder();
                foreach (Core.Entities.Panel panel in panelService.Panels)
                {
                    sb.Append(panel.ID.ToString() + "|");
                }
                string retour = sb.ToString();
                if (retour.Length > 0)
                    retour = retour.Substring(0, retour.Length - 1);
                return retour;

            }
            else if (instruction.Length >= 6 && instruction.Substring(0, 6) == "STATUS")
            {
                string[] parts = instruction.Split('|');
                if (parts.Length != 3)
                    return "";
                Suncondition suncondition;
                if (parts[1] == "1")
                    suncondition = Suncondition.Zwaarbewolkt;
                else if (parts[1] == "2")
                    suncondition = Suncondition.Lichtbewolkt;
                else if (parts[1] == "3")
                    suncondition = Suncondition.OverwegendZon;
                else if (parts[1] == "4")
                    suncondition = Suncondition.VolleZon;
                else
                    return "";

                StringBuilder sb = new StringBuilder();
                int numberOffPanels = panelService.Panels.Count;
                if (parts[2] == "0")
                    sb.AppendLine($"PANEL STATUS REPORT FOR {numberOffPanels} PANEL(S)");
                else
                    sb.AppendLine($"PANEL STATUS REPORT FOR 1 PANEL(S)");

                sb.AppendLine($"\tSun conditions : {suncondition}");
                sb.AppendLine($"\tTime : {DateTime.Now.ToString("HH:mm")}");
                if (parts[2] == "0")
                {
                    foreach (Core.Entities.Panel panel in panelService.Panels)
                    {
                        sb.AppendLine($"\tPANEL ID : {panel.ID.ToString()}");
                        sb.AppendLine($"\t    Surface : {panel.Surface.ToString()}m²");
                        sb.AppendLine($"\t    Max Power : {panel.MaxPower.ToString()}W");
                        sb.AppendLine($"\t    Current Power : {panel.GetCurrentPower(suncondition)}W");
                        sb.AppendLine($"\n");
                    }
                }
                else
                {
                    foreach (Core.Entities.Panel panel in panelService.Panels)
                    {
                        if (panel.ID.ToString() == parts[2])
                        {
                            sb.AppendLine($"\tPANEL ID : {panel.ID.ToString()}");
                            sb.AppendLine($"\t    Surface : {panel.Surface.ToString()}m²");
                            sb.AppendLine($"\t    Max Power : {panel.MaxPower.ToString()}W");
                            sb.AppendLine($"\t    Current Power : {panel.GetCurrentPower(suncondition)}W");
                            sb.AppendLine($"\n");
                        }
                    }
                }
                return sb.ToString();
            }
            else if (instruction.Length >= 7 && instruction.Substring(0, 7) == "CONNECT")
            {
                StringBuilder sb = new StringBuilder();
                foreach(Core.Entities.Panel panel in panelService.Panels)
                {
                    sb.Append(panel.ID.ToString() + "|");
                }
                string retour = sb.ToString();
                if(retour.Length > 0)
                    retour = retour.Substring(0, retour.Length - 1);
                return retour;
            }
            else
                return "";
        }


    }
}
