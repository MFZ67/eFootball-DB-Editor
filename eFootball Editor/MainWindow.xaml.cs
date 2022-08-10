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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IniParser;
using IniParser.Model;
using Ookii.Dialogs.Wpf;
using System.IO;
using eFootball_Editor.Functions;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace eFootball_Editor
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


        private void home_Click(object sender, RoutedEventArgs e)
        {
            evoprogress.Visibility = Visibility.Hidden;
            loadingtext.Visibility = Visibility.Hidden;
            loadingtext_Copy.Visibility = Visibility.Hidden;
            loadingtext_Copy1.Visibility = Visibility.Hidden;
            loadingtext_Copy2.Visibility = Visibility.Hidden;
            loadingtext_Copy3.Visibility = Visibility.Hidden;

            where_usercontrol_loads.Children.Clear();
            UserControl home = new HOME_SECREEN();
            where_usercontrol_loads.Children.Add(home);
        }


        private void loadfiles_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();

            dialog.Description = "Select PESDB folder. Your files will have backup, enjoy !";

            dialog.ShowDialog();

            evoprogress.Visibility = Visibility.Visible;
            evoprogress.Value = 0;
           

            globalvars.pesdbfolder = dialog.SelectedPath;
             var pesdbfolder = dialog.SelectedPath;
            globalvars.pesdbfolder = dialog.SelectedPath;

            //Competitions
            var competitionbin = pesdbfolder + "\\CompetitionUnit.bin";
            var parsercompetition = new FileIniDataParser();

            IniData inicompetition = parsercompetition.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\competitions.ini");
            inicompetition.Sections.RemoveSection("Competitions");
            inicompetition.Sections.AddSection("Competitions");

            byte[] competitiondata = File.ReadAllBytes(competitionbin);
            var competitionhexx = BitConverter.ToString(competitiondata).Replace("-", "");
            var competitionhex = "";
            //check and back
            if (competitionhexx.Contains("57455359"))
            {
                byte[] decompressedfile = unZLIBFile(competitiondata);
                competitionhex = BitConverter.ToString(decompressedfile).Replace("-", "");
            }
            else
            {
                competitionhex = competitionhexx;
            }
            File.WriteAllBytes(competitionbin + "backup", competitiondata);

            int i = 4;

            do
            {
                
                var hexidcompetiition = competitionhex.Substring(i + 16, 4);
                var hexidcompetitionT = hexidcompetiition.Substring(2, 2) + hexidcompetiition.Substring(0, 2);
                var idcompetiton = Convert.ToInt32(hexidcompetitionT, 16);
                var englishhexname = competitionhex.Substring(i + 3300, 92);
                var englishnormaname = hexstringbytes.FromHexString(englishhexname.Replace("00", ""));
                inicompetition["Competitions"].AddKey(englishnormaname, hexidcompetiition);

                i = i + 4696;

            }
            while (i < competitionhex.Length);


            //Save competitions to ini file
            parsercompetition.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\competitions.ini", inicompetition);
           
            loadingtext_Copy.Visibility = Visibility.Visible;
            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, loadingtext_Copy);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { loadingtext_Copy.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();

            //Country
            var countrybin = pesdbfolder + "\\Country.bin";
            var parsercountry = new FileIniDataParser();

            IniData inicountry = parsercountry.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\country.ini");
            inicountry.Sections.RemoveSection("Country");
            inicountry.Sections.AddSection("Country");

            byte[] countrydata = File.ReadAllBytes(countrybin);
            var countryhexx = BitConverter.ToString(countrydata).Replace("-", "");
            var countryhex = "";
            //check and back
            if (countryhexx.Contains("57455359"))
            {
                byte[] countryfile = unZLIBFile(countrydata);
                countryhex = BitConverter.ToString(countryfile).Replace("-", "");
            }
            else
            {
                countryhex = countryhexx;
            }
            File.WriteAllBytes(countrybin + "backup", countrydata);


            i = 0;
            int index = 0;
            do
            {
                var countryhexname = countryhex.Substring(i + 992, 128);
                var countryenglishname = hexstringbytes.FromHexString(countryhexname.Replace("00", ""));
                index = index + 1;
                inicountry["Country"].AddKey(index.ToString(), countryenglishname);
                i = i + 2840;

            }
            while (i < countryhex.Length);
            //Save comuntry to ini file
            parsercountry.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\country.ini",inicountry);
           
            loadingtext_Copy1.Visibility = Visibility.Visible;
             a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
             storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, loadingtext_Copy1);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { loadingtext_Copy1.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();


            //Teams
            var teambin = pesdbfolder + "\\Team.bin";
            var parserteam = new FileIniDataParser();

            IniData initeam = parserteam.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini");
            initeam.Sections.RemoveSection("Total Teams");
            initeam.Sections.RemoveSection("Teams Index");
            initeam.Sections.RemoveSection("Teams ID Hex");
            
            initeam.Sections.AddSection("Total Teams");
            initeam.Sections.AddSection("Teams Index");
            initeam.Sections.AddSection("Teams ID Hex");

            byte[] teamdata = File.ReadAllBytes(teambin);
            var teamhexx = BitConverter.ToString(teamdata).Replace("-", "");
            var teamhex = "";
            //check and back
            if (teamhexx.Contains("57455359"))
            {
                byte[] teamfile = unZLIBFile(teamdata);
                teamhex = BitConverter.ToString(teamfile).Replace("-", "");
            }
            else
            {
                teamhex = teamhexx;
            }
            File.WriteAllBytes(teambin + "backup", teamdata);

            i = 0;
            var totalteams = 0;
            do
            {
                var hexteamid = teamhex.Substring(i + 24, 8);
                var hexteamidT = hexteamid.Substring(6, 2) + hexteamid.Substring(4, 2) + hexteamid.Substring(2, 2) + hexteamid.Substring(0, 2);
                var idteam = Convert.ToInt32(hexteamidT, 16);
                var englishhexname = teamhex.Substring(i + 736, 96);
                var englishnormaname = hexstringbytes.FromHexString(englishhexname.Replace("00", ""));
                totalteams = totalteams + 1;
                initeam["Teams Index"].AddKey(totalteams.ToString(), englishnormaname);
                
                initeam["Teams ID Hex"].AddKey(englishnormaname, hexteamid);
                i = i + 3040;

            }
            while (i < teamhex.Length);

            //Save teams to ini file
            initeam["Total Teams"].AddKey("Total", totalteams.ToString());
            parserteam.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\teams.ini", initeam);

           
            loadingtext_Copy2.Visibility = Visibility.Visible;
            a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(1.5),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, loadingtext_Copy2);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { loadingtext_Copy2.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();

            //Players
            var playerbin = pesdbfolder + "\\Player.bin";
            var parserplayer = new FileIniDataParser();

            IniData iniplayer = parserplayer.ReadFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini");
            iniplayer.Sections.RemoveSection("Total Players");
            iniplayer.Sections.RemoveSection("Players Index");
            iniplayer.Sections.RemoveSection("Players ID Hex");
            iniplayer.Sections.RemoveSection("Players Hex ID");

            iniplayer.Sections.AddSection("Total Players");
            iniplayer.Sections.AddSection("Players Index");
            iniplayer.Sections.AddSection("Players ID Hex");
            iniplayer.Sections.AddSection("Players Hex ID");

            byte[] playerdata = File.ReadAllBytes(playerbin);
            var playerhexx = BitConverter.ToString(playerdata).Replace("-", "");
            var playerhex = "";
             //check and back
            if (playerhexx.Contains("57455359"))
            {
                byte[] playerfile = unZLIBFile(playerdata);
                playerhex = BitConverter.ToString(playerfile).Replace("-", "");
            }
            else
            {
                playerhex = playerhexx;
            }
            File.WriteAllBytes(playerbin + "backup", playerdata);

            i = 16;
            var totalplayers = 0;
            do
            {
                var hexplayerid = playerhex.Substring(i, 8);
                var hexplayeridT = hexplayerid.Substring(6, 2) + hexplayerid.Substring(4, 2) + hexplayerid.Substring(2, 2) + hexplayerid.Substring(0, 2);
                var idplayer = Convert.ToInt32(hexplayeridT, 16);
                var englishhexname = playerhex.Substring(i + 624, 96);
                var englishnormaname = hexstringbytes.FromHexString(englishhexname.Replace("00", ""));
                totalplayers = totalplayers + 1;
                iniplayer["Players Index"].AddKey(totalplayers.ToString(), englishnormaname);

                iniplayer["Players ID Hex"].AddKey(englishnormaname, hexplayerid);
                iniplayer["Players Hex ID"].AddKey(hexplayerid, englishnormaname) ;


                i = i + 784;

            }
            while (i < playerhex.Length);
            iniplayer["Total Players"].AddKey("Total", totalplayers.ToString());
            parserplayer.WriteFile(System.AppDomain.CurrentDomain.BaseDirectory + "Config\\players.ini", iniplayer);


            //

            a = new DoubleAnimation
            {
                From = 0.0,
                To = 100.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0.1),
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, evoprogress);
            Storyboard.SetTargetProperty(a, new PropertyPath(ProgressBar.ValueProperty));
            storyboard.Completed += delegate { evoprogress.Value=100; };
            storyboard.Begin();

            loadingtext_Copy3.Visibility = Visibility.Visible;
            a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(2),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, loadingtext_Copy3);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { loadingtext_Copy3.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();

            //welcome text
            a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, loadingtext);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { loadingtext.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();

            a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, evoprogress);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { evoprogress.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();
        }



        private void loadfiles_MouseMove(object sender, MouseEventArgs e)
        {
            loadfiles.BorderBrush = Brushes.White;
            loadfiles.Foreground = Brushes.Black;
            loadfiles.Background = Brushes.White;
          
        }

        private void loadfiles_MouseLeave(object sender, MouseEventArgs e)
        {
            loadfiles.BorderBrush = Brushes.Black;
            
            loadfiles.Foreground = Brushes.White;

            loadfiles.Background = Brushes.Black;
        }

        private void home_MouseLeave(object sender, MouseEventArgs e)
        {
            home.BorderBrush = Brushes.Black;

            home.Foreground = Brushes.White;
            home.Background = Brushes.Black;
        }

        private void home_MouseMove(object sender, MouseEventArgs e)
        {
            home.BorderBrush = Brushes.White;
            home.Foreground = Brushes.Black;
            home.Background = Brushes.White;
        }

        private void minimizeapp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void closeapp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void maximizeapp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            { this.WindowState = WindowState.Normal;}
            else { this.WindowState = WindowState.Maximized; }
        }

       
    }
}
