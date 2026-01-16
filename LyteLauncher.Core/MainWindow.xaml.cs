using Microsoft.Win32;
using StellaBootstrapper;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;



namespace LyteLauncher.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Logger Logger { get; } = new();
        public static RobloxHandler RobloxH { get; } = new();

        public bool GameViewIsHidden = true;

        private List<GameListData>? GameData = DataManager.GetGamesList();
        private LoadedVirtualGameCard? LoadedGame { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ReloadGames();

            DataManager.InitDirectories();

            ShowHideGameView(Visibility.Hidden);
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = "C:\\",
                Filter = "Executable/Java Files|*.exe;*.jar"
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    GameListData game = new()
                    {
                        ExecutablePath = file,
                        Name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                        IconSrc = "",
                        TotalPlayTime = 0.0,
                        Id = Guid.NewGuid()
                    };
                    DataManager.WriteGame(game);
                }
                GameData = DataManager.GetGamesList();

                Dispatcher.Invoke(ReloadGames);
            }
        }

        private void ReloadGames()
        {
            if (GameData == null) return;
            // Remove all the games in the list first
            if (GamesList.Items.Count != 0)
            {
                GamesList.Items.Clear();
            }

            AddGame("Roblox With Stella", RobloxHandler.Tag);

            foreach (GameListData game in GameData)
            {
                AddGame(game.Name, game.Id);
            }
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            LoadedVirtualGameCard? virtualGameCard;

            if ((string)button.Tag == RobloxHandler.Tag)
            {
                virtualGameCard = new()
                {
                    ExecutablePath = RobloxHandler.RobloxPlayerExecutable,
                    Id = new Guid(),
                    Name = "Roblox With StellaStrap",
                    IconSrc = "",
                    TotalPlayTime = RobloxHandler.GetTimings(),
                    Type = GameType.Roblox
                };
            } else
            {
                Guid gameId = (Guid)button.Tag;
                var game = Games.GetFromGUID(gameId);
                if (game == null) return;
                virtualGameCard = LoadedVirtualGameCard.FromList(game);
            }
            if (GameViewIsHidden) ShowHideGameView(Visibility.Visible);

            LoadedGame = virtualGameCard;

            GameText.Content = virtualGameCard.Name;
            GamePlaytime.Content = Math.ParseTime(virtualGameCard.TotalPlayTime);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedGame is null) return;
            var currentGame = LoadedGame;

            Task.Run(() =>
            {
                var gamelogger = Logger.UseSector(currentGame.ExecutablePath);

                gamelogger.Write("Started game process");
                var current = DateTime.UtcNow;

                if (currentGame.Type != GameType.Roblox)
                {
                    var proc = Process.Start(currentGame.ExecutablePath);
                    proc.WaitForExit();
                }
                if (currentGame.Type == GameType.Roblox)
                {
                    var testPreset = new BootstrapPreset()
                    {
                        BackgroundImage = new Uri($"{Directory.GetCurrentDirectory()}/Presets/Fnaf/Springtrap.png", UriKind.Absolute),
                        //FontFace = new Uri(""),
                        Sound = new Uri("Presets/Fnaf/fnaf3-start.mp3", UriKind.Relative)
                    };
                    Dispatcher.Invoke(() => 
                    {
                        var robloxBootstrapper = new BootStrapperWindow(testPreset, DataManager.RobloxAppDir);
                        robloxBootstrapper.Show();
                        robloxBootstrapper.ProgressTrigger += (time) => MasterProgressBar.Value = time;
                        Task.Run(() => robloxBootstrapper.DownloadClient(
                                DataManager.RobloxZipFileCacheDir));
                    });
                    //robloxBootstrapper.StartClient();
                }

                double now;
                var curentPlaytime = (DateTime.UtcNow - current).TotalSeconds;
                if (LoadedGame.Type == GameType.Roblox) 
                {
                    now = curentPlaytime + RobloxHandler.GetTimings();
                    RobloxHandler.UpdateTiming(now);
                } else
                {
                    now = curentPlaytime + Games.GetTimeFromGUID(currentGame.Id);
                    Games.UpdateTime(currentGame.Id, now);
                }

                gamelogger.Write("ended execution");
                Dispatcher.BeginInvoke(() =>
                {
                    GamePlaytime.Content = Math.ParseTime(now);
                    GamePlaytimeCurrent.Visibility = Visibility.Visible;
                    TimeCurrent.Visibility = Visibility.Visible;
                    GamePlaytimeCurrent.Content = Math.ParseTime(curentPlaytime);
                });
            });
        }

        private void ShowHideGameView(Visibility visibility)
        {
            GameText.Visibility = visibility;
            GamePlaytime.Visibility = visibility;
            PlayButton.Visibility = visibility;
            TimeLabel.Visibility = visibility;

            if (visibility == Visibility.Hidden)
            {
                GameViewIsHidden = true;
            } else
            {
                GameViewIsHidden = false;
            }
        }

        private void AddGame<T>(string name, T tag)
        {
            Button buttonComp = new()
            {
                Content = name,
                Tag = tag,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = GamesList.Background,
                BorderBrush = GamesList.BorderBrush,
                Foreground = System.Windows.Media.Brushes.White
            };

            buttonComp.Click += SelectionChanged;
            buttonComp.Width = GamesList.Width;
            buttonComp.Height += 5;

            GamesList.Items.Add(buttonComp);
        }

        //private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowState = WindowState.Minimized;
        //}

        // Probably Fixed in the future 
        //private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        WindowState = WindowState.Normal;
        //        MaximizeIcon.Data = (Geometry)FindResource("MaximizeIcon");
        //    }
        //    else
        //    {
        //        WindowState = WindowState.Maximized;
        //        MaximizeIcon.Data = (Geometry)FindResource("RestoreIcon");
        //    }
        //}

        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        //private void TitleBarMovable_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //    {
        //        try
        //        {
        //            DragMove();
        //        }
        //        catch
        //        { }
        //    }
        //}
    }
}