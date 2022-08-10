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
using IniParser;
using IniParser.Model;
using System.IO;
using eFootball_Editor.Functions;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Windows.Media.Animation;

namespace eFootball_Editor
{
    /// <summary>
    /// Interaction logic for HOME_SECREEN.xaml
    /// </summary>
    public partial class HOME_SECREEN : UserControl
    {
        public HOME_SECREEN()
        {
            InitializeComponent();
        }
        private byte[] unZLIBFile(byte[] filetounzlib)
        {
            byte[] numArray = new byte[0];
            try
            {
                Inflater inflater = new Inflater(false);
                int int32_1 = Convert.ToInt32(BitConverter.ToUInt32(filetounzlib, 8));
                int int32_2 = Convert.ToInt32(BitConverter.ToUInt32(filetounzlib, 12));
                inflater.SetInput(filetounzlib, 16, int32_1);
                byte[] buffer = new byte[int32_2];
                inflater.Inflate(buffer);
                return buffer;
            }
            catch
            {
                throw new Exception("ZLIB Error !!");
            }
        }

        private byte[] ZLIBFile(byte[] inputData)
        {
            uint length1 = (uint)inputData.Length;
            Deflater deflater = new Deflater(9);
            deflater.SetInput(inputData);
            deflater.Finish();
            using (MemoryStream memoryStream1 = new MemoryStream())
            {
                byte[] numArray = new byte[2097152];
                while (!deflater.IsNeedingInput)
                {
                    int count = deflater.Deflate(numArray);
                    memoryStream1.Write(numArray, 0, count);
                    if (deflater.IsFinished)
                        break;
                }
                deflater.Reset();
                uint length2 = (uint)memoryStream1.Length;
                byte[] buffer = new byte[8]
                {
          (byte) 0,
          (byte) 16,
          (byte) 1,
          (byte) 87,
          (byte) 69,
          (byte) 83,
          (byte) 89,
          (byte) 83
                };
                byte[] bytes1 = BitConverter.GetBytes(length2);
                byte[] bytes2 = BitConverter.GetBytes(length1);
                byte[] array = memoryStream1.ToArray();
                MemoryStream memoryStream2 = new MemoryStream();
                memoryStream2.Write(buffer, 0, buffer.Length);
                memoryStream2.Write(bytes1, 0, bytes1.Length);
                memoryStream2.Write(bytes2, 0, bytes2.Length);
                memoryStream2.Write(array, 0, array.Length);
                return memoryStream2.ToArray();
            }
        }


        string playerassignmentbin = globalvars.pesdbfolder + "\\PlayerAssignment.bin";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            TOTAL_SQUAD1.HorizontalContentAlignment = HorizontalAlignment.Center;
            TOTAL_SQUAD2.HorizontalContentAlignment = HorizontalAlignment.Center;
            TEAMS_COMBOBOX.HorizontalContentAlignment= HorizontalAlignment.Center;
            TEAMS_COMBOBOX_Copy.HorizontalContentAlignment= HorizontalAlignment.Center;

           
            byte[] padata = File.ReadAllBytes(playerassignmentbin);
            var pahexx = BitConverter.ToString(padata).Replace("-", "");
            var pahex = "";
            //check and back
            if (pahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(padata);
                pahex = BitConverter.ToString(decompressedfile).Replace("-", "");
                File.WriteAllBytes(playerassignmentbin,decompressedfile );

            }
            else
            {
                pahex = pahexx;
            }
            File.WriteAllBytes(playerassignmentbin + "backup", padata);

            var teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            TEAMS_COMBOBOX.Items.Clear();
            TEAMS_COMBOBOX_Copy.Items.Clear();

            string totalteams = teamsini["Total Teams"]["Total"];
            string teamname = "";
            for (int i = 0; i < int.Parse(totalteams.Trim()); i++)
            {
                teamname = teamsini["Teams Index"][i.ToString()];
                TEAMS_COMBOBOX.Items.Add(teamname);
                TEAMS_COMBOBOX_Copy.Items.Add(teamname);
            }


            string playername = "";
            var playersparser = new FileIniDataParser();
            IniData playersini = playersparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            string totalplayers = playersini["Total Players"]["Total"];
            int indexp = 0;
           
            do
            {
              
             
                    indexp = indexp + 1;
                    playername = playersini["Players Index"][indexp.ToString()];
                   


            }
            while (indexp < int.Parse(totalplayers.Trim()));
        }

        private void TEAMS_COMBOBOX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TEAM1PLAYERLIST.HorizontalContentAlignment = HorizontalAlignment.Center;
            var teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            string asfar = "00000000";
            string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
            string teamassignmenthexid = asfar + hexidteam;

            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);
            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM1PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("Team Offset");
            playersasini.Sections.AddSection("Team Offset");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i +16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM1PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);

            playersasini["Team Offset"].AddKey("From", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);
            TOTAL_SQUAD1.HorizontalContentAlignment = HorizontalAlignment.Center;
            

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, TOTAL_SQUAD1);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { TOTAL_SQUAD1.Content = total.ToString() + " PLAYERS"; };
            storyboard.Begin();
        }

        private void TEAMS_COMBOBOX_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TEAM2PLAYERLIST.HorizontalContentAlignment = HorizontalAlignment.Center;

            var teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            string asfar = "00000000";
            string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
            string teamassignmenthexid = asfar + hexidteam;

            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);

            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM2PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("Team Offset");
            playersasini.Sections.AddSection("Team Offset");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM2PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);
            playersasini["Team Offset"].AddKey("To", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);

            TOTAL_SQUAD2.HorizontalContentAlignment = HorizontalAlignment.Center;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, TOTAL_SQUAD2);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { TOTAL_SQUAD2.Content = total.ToString() + " PLAYERS"; };
            storyboard.Begin();

        }

        private void FROMTEAMB_MouseEnter(object sender, MouseEventArgs e)
        {
            FROMTEAMB.Source = new BitmapImage(new Uri("/Images/fromon.png", UriKind.Relative));
        }

        private void FROMTEAMB_MouseLeave(object sender, MouseEventArgs e)
        {
            FROMTEAMB.Source = new BitmapImage(new Uri("/Images/fromoff.png", UriKind.Relative));

        }

        private void EXCHANGEPLAYERS_MouseEnter(object sender, MouseEventArgs e)
        {
            EXCHANGEPLAYERS.Source = new BitmapImage(new Uri("/Images/swapon.png", UriKind.Relative));

        }

        private void EXCHANGEPLAYERS_MouseLeave(object sender, MouseEventArgs e)
        {
            EXCHANGEPLAYERS.Source = new BitmapImage(new Uri("/Images/swapoff.png", UriKind.Relative));

        }

        private void TOTEAMB_MouseEnter(object sender, MouseEventArgs e)
        {
            TOTEAMB.Source = new BitmapImage(new Uri("/Images/toon.png", UriKind.Relative));

        }

        private void TOTEAMB_MouseLeave(object sender, MouseEventArgs e)
        {
            TOTEAMB.Source = new BitmapImage(new Uri("/Images/tooff.png", UriKind.Relative));

        }

        private void FROMTEAMB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((nationalteama.IsChecked == true) && (nationalteamb.IsChecked == true))
            { MessageBox.Show("You can't transfer between two national teams"); }
            else
            {
                int tpta = TEAM1PLAYERLIST.Items.Count;
                int indexplayerlisteb = TEAM2PLAYERLIST.SelectedIndex;



                var teamaparser = new FileIniDataParser();
                IniData teamaini = teamaparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");

                var teambparser = new FileIniDataParser();
                IniData teambini = teambparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
                string indexplayerlistbingame = teambini["Players Index In Game"][indexplayerlisteb.ToString()];

                byte[] plassdata = File.ReadAllBytes(playerassignmentbin);


                var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                var plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                int indexplayerlistbingame1 = plassdatahex.IndexOf(indexplayerlistbingame);
                string blocklayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 48);

                string lastplayera = teamaini["Players Index In Game"][(tpta - 1).ToString()];
                string teamidp = plassdatahex.Substring(plassdatahex.IndexOf(lastplayera) - 8, 8);

                string baseindex = lastplayera.Substring(0, 3);

                int lpa1 = Convert.ToInt32(baseindex, 16) + 16;
                string lpa2 = lpa1.ToString("x");
                string lpa1hex = lpa2 + lastplayera.Substring(3, 5);



                string newblockpb1 = blocklayerb;
                string oldteamid = newblockpb1.Substring(16, 8);
                string newblockpb = newblockpb1.Replace(oldteamid, teamidp);

                string newas = plassdatahex.Insert(plassdatahex.IndexOf(lastplayera) + 24, newblockpb);

                string newasfinal = "";
                if (nationalteama.IsChecked == true)
                { newasfinal = newas; }
                else
                { newasfinal = newas.Replace(blocklayerb, ""); }


                byte[] newasfinalbytess = hexstringbytes.StringToByteArray(newasfinal);
                byte[] newasfinalbytes = ZLIBFile(newasfinalbytess);
                File.WriteAllBytes(playerassignmentbin, newasfinalbytes);

                //refresh list1
                TEAM1PLAYERLIST.Items.Clear();
                var teamsparser = new FileIniDataParser();
                IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                string asfar = "00000000";
                string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
                string teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);
                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                int i = plassdatahex.IndexOf(teamassignmenthexid);
                int total = 0;

                var playerssparser = new FileIniDataParser();
                IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM1PLAYERLIST.Items.Clear();

                var playersassparser = new FileIniDataParser();
                IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM1PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);

                playersasini["asba"].AddKey("From", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);

                //refresh list2
                TEAM2PLAYERLIST.Items.Clear();

                teamsparser = new FileIniDataParser();
                teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                asfar = "00000000";
                hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
                teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);

                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                i = plassdatahex.IndexOf(teamassignmenthexid);
                total = 0;

                playerssparser = new FileIniDataParser();
                playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM2PLAYERLIST.Items.Clear();

                playersassparser = new FileIniDataParser();
                playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM2PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);
                playersasini["asba"].AddKey("To", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);


                TOTAL_SQUAD1.HorizontalContentAlignment = HorizontalAlignment.Center;
                TOTAL_SQUAD2.HorizontalContentAlignment = HorizontalAlignment.Center;

                var a = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                var storyboard = new Storyboard();

                storyboard.Children.Add(a);
                Storyboard.SetTarget(a, TOTAL_SQUAD1);
                Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
                storyboard.Completed += delegate { TOTAL_SQUAD1.Content = TEAM1PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
                storyboard.Begin();

                var b = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                var sstoryboard = new Storyboard();

                sstoryboard.Children.Add(b);
                Storyboard.SetTarget(b, TOTAL_SQUAD2);
                Storyboard.SetTargetProperty(b, new PropertyPath(OpacityProperty));
                sstoryboard.Completed += delegate { TOTAL_SQUAD2.Content = TEAM2PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
                sstoryboard.Begin();

            }
        }

        private void TOTEAMB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if ((nationalteama.IsChecked == true) && (nationalteamb.IsChecked == true))
            { MessageBox.Show("You can't transfer between two national teams"); }
            else
            {
                int tpta = TEAM2PLAYERLIST.Items.Count;
                int indexplayerlisteb = TEAM1PLAYERLIST.SelectedIndex;



                var teamaparser = new FileIniDataParser();
                IniData teamaini = teamaparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");

                var teambparser = new FileIniDataParser();
                IniData teambini = teambparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
                string indexplayerlistbingame = teambini["Players Index In Game"][indexplayerlisteb.ToString()];

                byte[] plassdata = File.ReadAllBytes(playerassignmentbin);


                var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                var plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                int indexplayerlistbingame1 = plassdatahex.IndexOf(indexplayerlistbingame);
                string blocklayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 48);

                string lastplayera = teamaini["Players Index In Game"][(tpta - 1).ToString()];
                string teamidp = plassdatahex.Substring(plassdatahex.IndexOf(lastplayera) - 8, 8);

                string baseindex = lastplayera.Substring(0, 3);

                int lpa1 = Convert.ToInt32(baseindex, 16) + 16;
                string lpa2 = lpa1.ToString("x");
                string lpa1hex = lpa2 + lastplayera.Substring(3, 5);



                string newblockpb1 = blocklayerb;
                string oldteamid = newblockpb1.Substring(16, 8);
                string newblockpb = newblockpb1.Replace(oldteamid, teamidp);

                string newas = plassdatahex.Insert(plassdatahex.IndexOf(lastplayera) + 24, newblockpb);
                string newasfinal = "";
                if (nationalteamb.IsChecked == true)
                { newasfinal = newas; }
                else
                { newasfinal = newas.Replace(blocklayerb, ""); }


                byte[] newasfinalbytess = hexstringbytes.StringToByteArray(newasfinal);
                byte[] newasfinalbytes = ZLIBFile(newasfinalbytess);
                File.WriteAllBytes(playerassignmentbin, newasfinalbytes);

                //refresh list1
                TEAM1PLAYERLIST.Items.Clear();
                var teamsparser = new FileIniDataParser();
                IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                string asfar = "00000000";
                string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
                string teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);
                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                int i = plassdatahex.IndexOf(teamassignmenthexid);
                int total = 0;

                var playerssparser = new FileIniDataParser();
                IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM1PLAYERLIST.Items.Clear();

                var playersassparser = new FileIniDataParser();
                IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM1PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);

                playersasini["asba"].AddKey("From", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);

                //refresh list2
                TEAM2PLAYERLIST.Items.Clear();

                teamsparser = new FileIniDataParser();
                teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                asfar = "00000000";
                hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
                teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);

                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                i = plassdatahex.IndexOf(teamassignmenthexid);
                total = 0;

                playerssparser = new FileIniDataParser();
                playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM2PLAYERLIST.Items.Clear();

                playersassparser = new FileIniDataParser();
                playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM2PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);
                playersasini["asba"].AddKey("To", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);

                TOTAL_SQUAD1.HorizontalContentAlignment = HorizontalAlignment.Center;
                TOTAL_SQUAD2.HorizontalContentAlignment = HorizontalAlignment.Center;

                var a = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                var storyboard = new Storyboard();

                storyboard.Children.Add(a);
                Storyboard.SetTarget(a, TOTAL_SQUAD1);
                Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
                storyboard.Completed += delegate { TOTAL_SQUAD1.Content = TEAM1PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
                storyboard.Begin();

                var b = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                var sstoryboard = new Storyboard();

                sstoryboard.Children.Add(b);
                Storyboard.SetTarget(b, TOTAL_SQUAD2);
                Storyboard.SetTargetProperty(b, new PropertyPath(OpacityProperty));
                sstoryboard.Completed += delegate { TOTAL_SQUAD2.Content = TEAM2PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
                sstoryboard.Begin();
            }
        }

        private void EXCHANGEPLAYERS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((nationalteama.IsChecked == true) && (nationalteamb.IsChecked == true))
            { MessageBox.Show("You can't transfer between two national teams"); }
            else
            {
                int tpta = TEAM1PLAYERLIST.Items.Count - 1;
                int indexplayerlisteb = TEAM2PLAYERLIST.SelectedIndex;
                int indexplayerlistea = TEAM1PLAYERLIST.SelectedIndex;



                var teamaparser = new FileIniDataParser();
                IniData teamaini = teamaparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");

                var teambparser = new FileIniDataParser();
                IniData teambini = teambparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
                string indexplayerlistbingame = teambini["Players Index In Game"][indexplayerlisteb.ToString()];
                string indexplayerlistaingame = teambini["Players Index In Game"][indexplayerlistea.ToString()];


                byte[] plassdata = File.ReadAllBytes(playerassignmentbin);


                var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");

                var plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                //playerb
                int indexplayerlistbingame1 = plassdatahex.IndexOf(indexplayerlistbingame);
                string blocklayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 48);
                string idplayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 8);
                string lastplayera = teamaini["Players Index In Game"][indexplayerlistea.ToString()];

                string blocplayera = plassdatahex.Substring(plassdatahex.IndexOf(lastplayera) - 24, 48);
                string idplayera = plassdatahex.Substring(plassdatahex.IndexOf(lastplayera) - 24, 8);

                string newblockplayerb = blocklayerb.Replace(idplayerb, idplayera);

                string newblockplayera = blocplayera.Replace(idplayera, idplayerb);




                string newasfinal1 = plassdatahex.Replace(blocklayerb, newblockplayerb);
                string newasfinal = newasfinal1.Replace(blocplayera, newblockplayera);

                var d = TEAM1PLAYERLIST.SelectedItem;
                var dd = TEAM2PLAYERLIST.SelectedItem;
                var indexd = TEAM1PLAYERLIST.SelectedIndex;
                var indexdd = TEAM2PLAYERLIST.SelectedIndex;



                byte[] newasfinalbytess = hexstringbytes.StringToByteArray(newasfinal);
                byte[] newasfinalbytes = ZLIBFile(newasfinalbytess);
                File.WriteAllBytes(playerassignmentbin, newasfinalbytes);

                //refresh list1
                TEAM1PLAYERLIST.Items.Clear();
                var teamsparser = new FileIniDataParser();
                IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                string asfar = "00000000";
                string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
                string teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);
                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                int i = plassdatahex.IndexOf(teamassignmenthexid);
                int total = 0;

                var playerssparser = new FileIniDataParser();
                IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM1PLAYERLIST.Items.Clear();

                var playersassparser = new FileIniDataParser();
                IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM1PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);

                playersasini["asba"].AddKey("From", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);

                //refresh list2
                TEAM2PLAYERLIST.Items.Clear();

                teamsparser = new FileIniDataParser();
                teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
                asfar = "00000000";
                hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
                teamassignmenthexid = asfar + hexidteam;

                plassdata = File.ReadAllBytes(playerassignmentbin);

                plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
                plassdatahex = "";
                //check and back
                if (plassdatahexx.Contains("57455359"))
                {
                    byte[] decompressedfile = unZLIBFile(plassdata);
                    plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

                }
                else
                {
                    plassdatahex = plassdatahexx;
                }

                i = plassdatahex.IndexOf(teamassignmenthexid);
                total = 0;

                playerssparser = new FileIniDataParser();
                playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
                TEAM2PLAYERLIST.Items.Clear();

                playersassparser = new FileIniDataParser();
                playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
                playersasini.Sections.RemoveSection("Players Index In Game");
                playersasini.Sections.AddSection("Players Index In Game");
                playersasini.Sections.RemoveSection("asba");
                playersasini.Sections.AddSection("asba");


                do
                {

                    string p = plassdatahex.Substring(i, 16);
                    if (p == teamassignmenthexid)
                    {
                        playersasini["asba"].AddKey("From", i.ToString());

                        string pidhex = plassdatahex.Substring(i - 8, 8);
                        string playername = playersini["Players Hex ID"][pidhex];
                        string indexplayer = plassdatahex.Substring(i + 16, 8);
                        playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                        TEAM2PLAYERLIST.Items.Add(playername);
                        total = total + 1;
                        i = i + 48;
                    }
                    else { break; }
                }
                while (i < plassdatahex.Length);
                playersasini["asba"].AddKey("To", i.ToString());

                playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);
            }
        }


        private void UP_MouseEnter(object sender, MouseEventArgs e)
        {
            UP.Source = new BitmapImage(new Uri("/Images/fromon.png", UriKind.Relative));

        }

        private void UP_MouseLeave(object sender, MouseEventArgs e)
        {
            UP.Source = new BitmapImage(new Uri("/Images/fromoff.png", UriKind.Relative));

        }

      

        private void DOWN_MouseEnter(object sender, MouseEventArgs e)
        {
            DOWN.Source = new BitmapImage(new Uri("/Images/fromon.png", UriKind.Relative));

        }

        private void DOWN_MouseLeave(object sender, MouseEventArgs e)
        {
            DOWN.Source = new BitmapImage(new Uri("/Images/fromoff.png", UriKind.Relative));

        }

        private void DOWN_Copy_MouseEnter(object sender, MouseEventArgs e)
        {
            DOWN_Copy.Source = new BitmapImage(new Uri("/Images/fromon.png", UriKind.Relative));

        }

        private void DOWN_Copy_MouseLeave(object sender, MouseEventArgs e)
        {
            DOWN_Copy.Source = new BitmapImage(new Uri("/Images/fromoff.png", UriKind.Relative));

        }

        private void UP_Copy_MouseEnter(object sender, MouseEventArgs e)
        {
            UP_Copy.Source = new BitmapImage(new Uri("/Images/fromon.png", UriKind.Relative));

        }

        private void UP_Copy_MouseLeave(object sender, MouseEventArgs e)
        {
            UP_Copy.Source = new BitmapImage(new Uri("/Images/fromoff.png", UriKind.Relative));

        }

        private void UP_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var teamsparser = new FileIniDataParser();
            IniData teamaini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            string indexplayer1 = teamaini["Players Index In Game"][TEAM1PLAYERLIST.SelectedIndex.ToString()];
            string indexplayer2 = teamaini["Players Index In Game"][(TEAM1PLAYERLIST.SelectedIndex - 1).ToString()];
            

            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);
            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }
            int indexpl2 = plassdatahex.IndexOf(indexplayer2)-24;
            string blockplayer1 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            string blockplayer2 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer2) - 24, 48);
            string blockplayer1reserve = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            plassdatahex = plassdatahex.Replace(blockplayer1, blockplayer2);
            plassdatahex = plassdatahex.Remove(indexpl2, 48);
            plassdatahex = plassdatahex.Insert(indexpl2, blockplayer1reserve);
            byte[] savepla = hexstringbytes.StringToByteArray(plassdatahex);
            File.WriteAllBytes(playerassignmentbin,savepla);

            //refresh list1
            TEAM1PLAYERLIST.Items.Clear();
           teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            string asfar = "00000000";
            string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
            string teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);
            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM1PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM1PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);

            playersasini["asba"].AddKey("From", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);
        }

        private void DOWN_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var teamsparser = new FileIniDataParser();
            IniData teamaini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            string indexplayer1 = teamaini["Players Index In Game"][TEAM1PLAYERLIST.SelectedIndex.ToString()];
            string indexplayer2 = teamaini["Players Index In Game"][(TEAM1PLAYERLIST.SelectedIndex +1).ToString()];


            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);
            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }
            int indexpl2 = plassdatahex.IndexOf(indexplayer2) - 24;
            string blockplayer1 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            string blockplayer2 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer2) - 24, 48);
            string blockplayer1reserve = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            plassdatahex = plassdatahex.Replace(blockplayer1, blockplayer2);
            plassdatahex = plassdatahex.Remove(indexpl2, 48);
            plassdatahex = plassdatahex.Insert(indexpl2, blockplayer1reserve);
            byte[] savepla = hexstringbytes.StringToByteArray(plassdatahex);
            File.WriteAllBytes(playerassignmentbin, savepla);

            //refresh list1
            TEAM1PLAYERLIST.Items.Clear();
            teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            string asfar = "00000000";
            string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
            string teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);
            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM1PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM1PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);

            playersasini["asba"].AddKey("From", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);
        }

        private void UP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var teamsparser = new FileIniDataParser();
            IniData teamaini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            string indexplayer1 = teamaini["Players Index In Game"][TEAM2PLAYERLIST.SelectedIndex.ToString()];
            string indexplayer2 = teamaini["Players Index In Game"][(TEAM2PLAYERLIST.SelectedIndex - 1).ToString()];


            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);
            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }
            int indexpl2 = plassdatahex.IndexOf(indexplayer2) - 24;
            string blockplayer1 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            string blockplayer2 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer2) - 24, 48);
            string blockplayer1reserve = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            plassdatahex = plassdatahex.Replace(blockplayer1, blockplayer2);
            plassdatahex = plassdatahex.Remove(indexpl2, 48);
            plassdatahex = plassdatahex.Insert(indexpl2, blockplayer1reserve);
            byte[] savepla = hexstringbytes.StringToByteArray(plassdatahex);
            File.WriteAllBytes(playerassignmentbin, savepla);

            //refresh list2
            TEAM2PLAYERLIST.Items.Clear();

            teamsparser = new FileIniDataParser();
           IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
           var asfar = "00000000";
          var  hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
           var teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);

            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

           int i = plassdatahex.IndexOf(teamassignmenthexid);
          int  total = 0;

          var playerssparser = new FileIniDataParser();
          IniData  playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM2PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
           IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM2PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);
            playersasini["asba"].AddKey("To", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);
        }

        private void DOWN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var teamsparser = new FileIniDataParser();
            IniData teamaini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            string indexplayer1 = teamaini["Players Index In Game"][TEAM2PLAYERLIST.SelectedIndex.ToString()];
            string indexplayer2 = teamaini["Players Index In Game"][(TEAM2PLAYERLIST.SelectedIndex + 1).ToString()];


            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);
            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }
            int indexpl2 = plassdatahex.IndexOf(indexplayer2) - 24;
            string blockplayer1 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            string blockplayer2 = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer2) - 24, 48);
            string blockplayer1reserve = plassdatahex.Substring(plassdatahex.IndexOf(indexplayer1) - 24, 48);
            plassdatahex = plassdatahex.Replace(blockplayer1, blockplayer2);
            plassdatahex = plassdatahex.Remove(indexpl2, 48);
            plassdatahex = plassdatahex.Insert(indexpl2, blockplayer1reserve);
            byte[] savepla = hexstringbytes.StringToByteArray(plassdatahex);
            File.WriteAllBytes(playerassignmentbin, savepla);

            //refresh list2
            TEAM2PLAYERLIST.Items.Clear();

            teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            var asfar = "00000000";
            var hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
            var teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);

            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM2PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM2PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);
            playersasini["asba"].AddKey("To", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int indexplayerlisteb = TEAM1PLAYERLIST.SelectedIndex;



            var teamaparser = new FileIniDataParser();
            IniData teamaini = teamaparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");

           
            string indexplayerlistbingame = teamaini["Players Index In Game"][indexplayerlisteb.ToString()];

            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);


            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int indexplayerlistbingame1 = plassdatahex.IndexOf(indexplayerlistbingame);
            string blocklayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 48);

          



         
            string newasfinal = plassdatahex.Replace(blocklayerb, ""); 

            byte[] newasfinalbytess = hexstringbytes.StringToByteArray(newasfinal);
            byte[] newasfinalbytes = ZLIBFile(newasfinalbytess);
            File.WriteAllBytes(playerassignmentbin, newasfinalbytes);

            //refresh list1
            TEAM1PLAYERLIST.Items.Clear();
            var teamsparser = new FileIniDataParser();
            IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            string asfar = "00000000";
            string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX.SelectedItem.ToString()];
            string teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);
            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int i = plassdatahex.IndexOf(teamassignmenthexid);
            int total = 0;

            var playerssparser = new FileIniDataParser();
            IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM1PLAYERLIST.Items.Clear();

            var playersassparser = new FileIniDataParser();
            IniData playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM1PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);

            playersasini["asba"].AddKey("From", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teama.ini", playersasini);

          

            TOTAL_SQUAD1.HorizontalContentAlignment = HorizontalAlignment.Center;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, TOTAL_SQUAD1);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { TOTAL_SQUAD1.Content = TEAM1PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
            storyboard.Begin();

         

        
    }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            int indexplayerlisteb = TEAM2PLAYERLIST.SelectedIndex;



            var teamaparser = new FileIniDataParser();
            IniData teamaini = teamaparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");


            string indexplayerlistbingame = teamaini["Players Index In Game"][indexplayerlisteb.ToString()];

            byte[] plassdata = File.ReadAllBytes(playerassignmentbin);


            var plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            var plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

            int indexplayerlistbingame1 = plassdatahex.IndexOf(indexplayerlistbingame);
            string blocklayerb = plassdatahex.Substring(indexplayerlistbingame1 - 24, 48);






            string newasfinal = plassdatahex.Replace(blocklayerb, "");

            byte[] newasfinalbytess = hexstringbytes.StringToByteArray(newasfinal);
            byte[] newasfinalbytes = ZLIBFile(newasfinalbytess);
            File.WriteAllBytes(playerassignmentbin, newasfinalbytes);

           

            //refresh list2
            TEAM2PLAYERLIST.Items.Clear();

          var  teamsparser = new FileIniDataParser();
           IniData teamsini = teamsparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
           string asfar = "00000000";
           string hexidteam = teamsini["Teams ID Hex"][TEAMS_COMBOBOX_Copy.SelectedItem.ToString()];
           string teamassignmenthexid = asfar + hexidteam;

            plassdata = File.ReadAllBytes(playerassignmentbin);

            plassdatahexx = BitConverter.ToString(plassdata).Replace("-", "");
            plassdatahex = "";
            //check and back
            if (plassdatahexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(plassdata);
                plassdatahex = BitConverter.ToString(decompressedfile).Replace("-", "");

            }
            else
            {
                plassdatahex = plassdatahexx;
            }

         int   i = plassdatahex.IndexOf(teamassignmenthexid);
          int  total = 0;

            var playerssparser = new FileIniDataParser();
           IniData playersini = playerssparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            TEAM2PLAYERLIST.Items.Clear();

          var  playersassparser = new FileIniDataParser();
          IniData  playersasini = playersassparser.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini");
            playersasini.Sections.RemoveSection("Players Index In Game");
            playersasini.Sections.AddSection("Players Index In Game");
            playersasini.Sections.RemoveSection("asba");
            playersasini.Sections.AddSection("asba");


            do
            {

                string p = plassdatahex.Substring(i, 16);
                if (p == teamassignmenthexid)
                {
                    playersasini["asba"].AddKey("From", i.ToString());

                    string pidhex = plassdatahex.Substring(i - 8, 8);
                    string playername = playersini["Players Hex ID"][pidhex];
                    string indexplayer = plassdatahex.Substring(i + 16, 8);
                    playersasini["Players Index In Game"].AddKey(total.ToString(), indexplayer);
                    TEAM2PLAYERLIST.Items.Add(playername);
                    total = total + 1;
                    i = i + 48;
                }
                else { break; }
            }
            while (i < plassdatahex.Length);
            playersasini["asba"].AddKey("To", i.ToString());

            playersassparser.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teamb.ini", playersasini);


            TOTAL_SQUAD2.HorizontalContentAlignment = HorizontalAlignment.Center;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, TOTAL_SQUAD2);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { TOTAL_SQUAD2.Content = TEAM2PLAYERLIST.Items.Count.ToString() + " PLAYERS"; ; };
            storyboard.Begin();


        }
        
    }
}
