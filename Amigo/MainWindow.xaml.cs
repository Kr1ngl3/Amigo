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
            InitializeComponent();
            Start();
        }
        readonly int
            x = 8,
            y = 13,
            gameLevel = 1;
        readonly double
            fallSpeed = 1; // seconds to for fall
        Board board;
        double points = 0;
        public void Start()
        {
            SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\sound.wav");
            player.Load();
            player.Play();
            bool soundFinished = true;

            mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario1.png"));
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\background.png"));

            board = new(gameLevel, x, y, points, this);
            StartFallLoop();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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

        public void Update()
        {
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

        }
        bool marioBool = false;
        private void Fall(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() =>
            {
                if (marioBool)
                {
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario1.png"));
                }
                else
                {
                    mario.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\mario3.png"));
                }
                marioBool = !marioBool;


                if (activePill != null)
                {
                    bool test = board.PillFall(activePill);
                    if (!test)
                    {
                        activePill = null;
                        board.TestForConnections();
                    }
                }
                else
                {
                    activePill = new(random.Next(Enum.GetNames(typeof(Color)).Length), random.Next(Enum.GetNames(typeof(Color)).Length));
                    board.Add(new Vector(3, 0), activePill.onePiece);
                    board.Add(new Vector(4, 0), activePill.twoPiece);
                }
                Update();

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
}
