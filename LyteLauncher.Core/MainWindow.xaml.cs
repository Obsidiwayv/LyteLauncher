using Microsoft.Win32;
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
        public bool GameViewIsHidden = true;

        private List<GameListData>? GameData = DataManager.GetGamesList();
        private GameListData? LoadedGame { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ReloadGames();

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

            foreach (GameListData game in GameData)
            {
                Button buttonComp = new()
                {
                    Content = game.Name,
                    Tag = game.Id,
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
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Guid gameId = (Guid)button.Tag;


            var game = Games.GetFromGUID(gameId);
            if (game == null) return;
            if (GameViewIsHidden) ShowHideGameView(Visibility.Visible);

            LoadedGame = game;

            GameText.Content = game.Name;
            GamePlaytime.Content = Math.ParseTime(game.TotalPlayTime);
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

                var proc = Process.Start(currentGame.ExecutablePath);
                proc.WaitForExit();

                var curentPlaytime = (DateTime.UtcNow - current).TotalSeconds;
                var now = curentPlaytime + Games.GetTimeFromGUID(currentGame.Id);
                Games.UpdateTime(currentGame.Id, now);

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