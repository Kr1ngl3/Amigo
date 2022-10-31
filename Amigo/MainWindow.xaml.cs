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

            InitializeComponent();
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\title.png"));
        }
        readonly int
            x = 8,
            y = 13;
        
        int gameLevel;

        GameState gameState;
        readonly double
            fallSpeed = .5; // seconds to for fall
        Board board;
        public double points = 0;
        public void Start(int gameLevel)
        {
            gameState = GameState.playing;
            this.gameLevel = gameLevel;
            SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\sound.wav");
            player.Load();
            player.Play();
            bool soundFinished = true;

            mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario1.png"));
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\background.png"));

            board = new(gameLevel, x, y, this);
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
                    Start(gameLevel);
                }
                if (gameState == GameState.title)
                {
                    Start(gameLevel);
                    StartFallLoop();
                }
            }
            if (activePill == null)
                return;

            if (e.Key == Key.Z)
                board.Rotate(activePill, false);

            if (e.Key == Key.X)
                board.Rotate(activePill, true);

            if (e.Key == Key.Left)
                board.PillMove(activePill, new Vector(-1, 0));

            if (e.Key == Key.Right)
                board.PillMove(activePill, new Vector(1, 0));

            if (e.Key == Key.Down)
                board.PillFall(activePill);
            Update();
        }
        Random random = new Random();
        Pill? activePill;

        public int Update()
        {
            int virusCount = 0;
            scoreText.Text = "     Points: " + points.ToString() + "\n\n     Top: amogus";
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
                        img.RenderTransformOrigin = new Point(0.5, 0.5);
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
            infoText.Text = "\n\n     Game Level: " + gameLevel + "\n\n" + "     Game Speed: 69" + "\n\n     Virus Count: " + virusCount;
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
                    gameState = GameState.win;
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
                    img2.RenderTransformOrigin = new Point(0.5, 0.5);
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

        }

        public System.Timers.Timer fallLoopTimer;
        public void StartFallLoop()
        {
            //make timer
            fallLoopTimer = new System.Timers.Timer(1000 * fallSpeed);
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
