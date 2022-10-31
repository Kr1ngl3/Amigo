using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Amigo
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            gameState = GameState.title;
            gameLevel = 1;
            gameSpeed = 1;
            musicType = 1;
            titlestate = 0;

            InitializeComponent();
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\title.png"));
            this.Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\test.png"));

            SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\sol_badguy.wav");
            player.Load();
            player.Play();
            updateOptionText();

        }
        readonly int
            x = 8,
            y = 13;
        
        int gameLevel;
        double gameSpeed;
        int musicType;
        int titlestate;

        GameState gameState;
        readonly double
            fallSpeed = .5; // seconds to for fall
        Board board;
        public double points = 0;
        public double topscore = 0;
        public MediaPlayer le_sound_player;
        public void Start(int gameLevel)

        {
            gameState = GameState.playing;
            updateOptionText();
            this.gameLevel = gameLevel;
            SoundPlayer player;
            switch (musicType)
            {
                case 1:

                    player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\sound.wav");
                    player.Load();
                    player.PlayLooping();
                    break;
                case 2:
                    player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\aaaaa.wav");
                    player.Load();
                    player.PlayLooping();
                    break;
                case 3:
                    player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\type_3.wav");
                    player.Load();
                    player.PlayLooping();
                    break;
                case 4:
                    player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\type_4.wav");
                    player.Load();
                    player.PlayLooping();
                    break;
            }
            






            bool soundFinished = true;

            mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario1.png"));
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\background.png"));

            board = new(gameLevel, x, y, this);
            topscore = ReadScore(Directory.GetCurrentDirectory() + @"\score.txt");
        }

        public void SaveScore(string FileName)
        {
            topscore = points;
            StreamWriter sw = new StreamWriter(FileName, false);
            sw.WriteLine(points);
            sw.Close();
        }

        public double ReadScore(string FileName) {
            StreamReader sr = new StreamReader(FileName);
            

            string score = sr.ReadLine();
            
            if (score == null)
            {
                return 0;

            } else
            {
                double final_score = Convert.ToDouble(score);
                return final_score;
            }

        }

        public void updateOptionText()
        {
            if (gameState != GameState.title)
            {
                optionText.Text = "";
                return;
            }



            switch (titlestate)
            {
                case 0:
                    optionText.Text = "<Virus Level: " + gameLevel + ">\n" + "Game Speed: " + Get_GameSpeed_String() + "\n" + "Music Type: Type " + musicType + "\n" + "Press enter to start";
                    break;
                case 1:
                    optionText.Text = "Virus Level: " + gameLevel + "\n" + "<Game Speed: " + Get_GameSpeed_String() + ">\n" + "Music Type: Type " + musicType + "\n" + "Press enter to start";
                    break;
                case 2:
                    optionText.Text = "Virus Level: " + gameLevel + "\n" + "Game Speed: " + Get_GameSpeed_String() + "\n" + "<Music Type: Type " + musicType + ">\n" + "Press enter to start";
                    break;
            }
            

        }

        public string Get_GameSpeed_String()
        {
            string GameSpeedText = "";

            switch (gameSpeed)
            {
                case 0.5:
                    GameSpeedText = "Low";
                    break;
                case 1:
                    GameSpeedText = "Mid";
                    break;
                case 2:
                    GameSpeedText = "High";
                    break;
                case 4:
                    GameSpeedText = "Turbo";
                    break;
            }
            
            return GameSpeedText;

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Enter)
            {
                if (gameState == GameState.win)
                {
                    Start(++gameLevel);
                }
                if (gameState == GameState.gameOver)
                {
                    points = 0;
                    gameLevel = 1;
                    Start(gameLevel);
                }
                if (gameState == GameState.title)
                {
                    Start(gameLevel);
                    StartFallLoop();
                }
            }
            if (e.Key == Key.Left)
            {
                switch (gameState)
                {
                    case GameState.title:

                        switch (titlestate)
                        {
                            case 0:
                                if (gameLevel > 1){
                                    gameLevel--;
                                }
                                break;
                            case 1:

                                if (gameSpeed > 0.5)
                                {
                                    gameSpeed /= 2.0;

                                }
                                break;

                            case 2:
                                if (musicType > 1)
                                {
                                    musicType--;
                                }
                                break;
                        }
                        updateOptionText();
                        break;
                    case GameState.playing:
                        if (activePill != null) { 
                        board.PillMove(activePill, new Vector(-1, 0));
                        }
                        break;

                }

            }


            if (e.Key == Key.Right)
            {
                switch (gameState)
                {
                    case GameState.title:

                        
                        switch (titlestate)
                        {
                            case 0:
                                if (gameLevel < 5)
                                {
                                    gameLevel++;
                                }
                                break;
                            case 1:

                                if (gameSpeed < 4)
                                {
                                    gameSpeed *= 2;
                                }
                                break;
                            case 2:
                                if (musicType < 4)
                                {
                                    musicType++;
                                }
                                break; ;

                        }
                        updateOptionText();
                        break;
                    
                    case GameState.playing:
                        if (activePill != null)
                        {
                            board.PillMove(activePill, new Vector(1, 0));
                            
                        }
                        break;
                }                    
            }
            if (e.Key == Key.Down)
            {

                switch (gameState)
                {
                    case GameState.title:

                        titlestate = (titlestate + 1) % 3;
                        updateOptionText();
                        break;
                    case GameState.playing:
                        if (activePill != null)
                        {

                            board.PillFall(activePill);
                        }
                        break;
                }
            }
            if (e.Key == Key.Up) {
                {
                titlestate--;

                if (titlestate < 0)
                {
                    titlestate = 2;
                }
                updateOptionText();
                }

            }
            if (activePill == null)
                return;

            if (e.Key == Key.Z) {
                board.Rotate(activePill, false);
            }
            if (e.Key == Key.X) {
                board.Rotate(activePill, true);
            }


            Update();
        }
        Random random = new Random();
        Pill? activePill;

        public int Update()
        {
            int virusCount = 0;
            scoreText.Text = "     Points: " + points.ToString() + "\n\n     Top: " + topscore.ToString();
            Grid grid = new Grid();
            for (int i = 0; i < x; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < y; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            foreach (Tile tile in board.Values)
            {
                if (tile == null)
                    continue;
                if (tile.state == State.virus)
                {
                    virusCount++;
                    Vector pos = board.GetPos(tile);
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\" + ParseColorToString(tile.color) + "Virus.png"));
                    Grid.SetColumn(img, (int)pos.X);
                    Grid.SetRow(img, (int)pos.Y);

                    grid.Children.Add(img);
                }
                if (tile.state == State.pill)
                {
                    PillPiece p = (PillPiece)tile;
                    Image img = new Image();
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\" + ParseColorToString(p.color) + "Pill.png");
                    bi.Rotation = p.rotation;
                    bi.EndInit();
                    img.Source = bi;

                    if (p.isTwoPiece)
                    {
                        img.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                        ScaleTransform flipTrans = new ScaleTransform();
                        flipTrans.ScaleY = -1;
                        img.RenderTransform = flipTrans;
                    }

                    Vector pos = board.GetPos(p);
                    Grid.SetColumn(img, (int)pos.X);
                    Grid.SetRow(img, (int)pos.Y);
                    grid.Children.Add(img);
                }
            }
            game.Content = grid;
            infoText.Text = "\n\n     Game Level: \n      " + gameLevel + "\n" + "     Game Speed: \n      " + Get_GameSpeed_String()  + "\n     Virus Count: \n      " + virusCount;


            return virusCount;

        }

        int loop = 0;
        Image img1;
        Image img2;
        Pill? previewPill;
        private void Fall(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() =>
            {
                if (Update() == 0)
                {

                    if (gameState != GameState.win)
                    {

                        SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\ay_lamo.wav");
                        player.Load();
                        player.Play();

                        le_sound_player = new MediaPlayer();
                        le_sound_player.Open(new Uri(Directory.GetCurrentDirectory() + @"\wow.wav"));
                       
                        le_sound_player.Play();
                        points += (1000 * gameLevel);
                    }
                    gameState = GameState.win;

                }
                    if (gameState == GameState.win)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\gameWin.png"));
                    game.Content = img;
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\marioWin.png"));

                    preview.Children.Remove(img1);
                    preview.Children.Remove(img2);
                    return;
                }
                if (gameState == GameState.gameOver)
                {
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\marioDead.png"));
                    preview.Children.Remove(img1);
                    preview.Children.Remove(img2);
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\gameOver.png"));
                    game.Content = img;
                    return;
                }
                if (activePill == null && (board.ContainsKey(new Vector(3, 0)) || board.ContainsKey(new Vector(4, 0))))
                {
                    if (gameState != GameState.gameOver)
                    {

                        SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\gameover.wav");
                        player.Load();
                        player.Play();

                        le_sound_player = new MediaPlayer();
                        le_sound_player.Open(new Uri(Directory.GetCurrentDirectory() + @"\VineBoom.wav"));

                        le_sound_player.Play();

                        if (points > topscore)
                        {
                            SaveScore(Directory.GetCurrentDirectory() + @"\score.txt");
                        }

                    }
                    gameState = GameState.gameOver;
                    return;
                }
                if (loop == 1 && activePill == null)
                {
                    activePill = previewPill;
                    previewPill = null;
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario1.png"));
                    preview.Children.Remove(img1);
                    preview.Children.Remove(img2);
                    board.Add(new Vector(3, 0), activePill.onePiece);
                    board.Add(new Vector(4, 0), activePill.twoPiece);
                    loop--;
                    Update();
                    return;
                }
                if (previewPill == null)
                {
                    previewPill = new(random.Next(Enum.GetNames(typeof(Color)).Length), random.Next(Enum.GetNames(typeof(Color)).Length));
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario2.png"));

                    PillPiece p1 = (PillPiece)previewPill.onePiece;
                    PillPiece p2 = (PillPiece)previewPill.twoPiece;
                    img1 = new Image();
                    img2 = new Image();
                    BitmapImage bi1 = new BitmapImage();
                    BitmapImage bi2 = new BitmapImage();
                    bi1.BeginInit();
                    bi1.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\" + ParseColorToString(p1.color) + "Pill.png");
                    bi1.Rotation = p1.rotation;
                    bi1.EndInit();
                    img1.Source = bi1;
                    img1.HorizontalAlignment = HorizontalAlignment.Left;
                    bi2.BeginInit();
                    bi2.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\" + ParseColorToString(p2.color) + "Pill.png");
                    bi2.Rotation = p2.rotation;
                    bi2.EndInit();
                    img2.Source = bi2;
                    img2.HorizontalAlignment = HorizontalAlignment.Right;
                    img2.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                    ScaleTransform flipTrans = new ScaleTransform();
                    flipTrans.ScaleY = -1;
                    img2.RenderTransform = flipTrans;


                    Grid.SetColumn(img1, 1);
                    Grid.SetRow(img1, 0);
                    preview.Children.Add(img1);
                    Grid.SetColumn(img2, 1);
                    Grid.SetRow(img2, 0);
                    preview.Children.Add(img2);
                    loop++;
                    Update();
                }
                if (activePill != null)
                {
                    bool test = board.PillFall(activePill);
                    if (!test)
                    {
                        activePill = null;
                        board.TestForConnections();
                    }
                    Update();
                }


            }));
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gameHeight1.Height = new GridLength(0.77037037037 * this.Height * 5.0/13.0);
            gameHeight2.Height = new GridLength(0.77037037037 * this.Height * 4.0/13.0);
            gameHeight3.Height = new GridLength(0.77037037037 * this.Height * 4.0/13.0);
            gameWidth.Width = new GridLength(0.26666666666 * this.Width);

            this.FontSize = 60 * (this.Height / 1080);
            infoText.FontSize = 45 * (this.Height / 1080);
            optionText.FontSize = 50 * (this.Height / 1080);
        }

        public System.Timers.Timer fallLoopTimer;
        public void StartFallLoop()
        {
            //make timer
            fallLoopTimer = new System.Timers.Timer((1000 * fallSpeed)/gameSpeed);
            fallLoopTimer.Enabled = true;
            fallLoopTimer.Elapsed += Fall;
        }
        public string ParseColorToString(Color c)
        {
            switch (c)
            {
                case Color.red:
                    return "red";
                case Color.blue:
                    return "blue";
                case Color.yellow:
                    return "yellow";
                default:
                    return "";
            }
        }

    }
    enum GameState
    { 
        playing,
        gameOver,
        win,
        title
    }


}
