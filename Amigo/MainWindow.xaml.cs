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
using System.IO;

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
            gameLevel = 2,
            ups = 60; // game updates per second
        readonly double
            fallSpeed = 1; // seconds to for fall
        Board board;
        public void Start()
        {
            Thread.Sleep(20);
            background.ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\background.png"));

            board = new(gameLevel, x, y);
            StartUpdateLoop();
            StartFallLoop();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (activePill == null)
                return;
            if (e.Key == Key.Z)
            {
                board.Rotate(activePill, false);
            }
            if (e.Key == Key.X)
            {
                board.Rotate(activePill, true);
            }
            if (e.Key == Key.Left)
            {
                board.PillMove(activePill, new Vector(-1, 0));
            }
            if (e.Key == Key.Right)
            {
                board.PillMove(activePill, new Vector(1, 0));
            }
            if (e.Key == Key.Down)
            {
                board.PillFall(activePill);
            }
        }
        Random random = new Random();
        Pill? activePill;
        private void Update(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() => 
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
            }));
        }
        private void Fall(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() =>
            {
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

            }));
        }
            System.Timers.Timer updateLoopTimer;
        public void StartUpdateLoop()
        {
            //make timer
            updateLoopTimer = new System.Timers.Timer(1000 / ups);
            updateLoopTimer.Enabled = true;
            updateLoopTimer.Elapsed += Update;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gameHeight.Height = new GridLength(0.77037037037 * this.Height);
            gameWidth.Width = new GridLength(0.26666666666 * this.Width);
        }

        System.Timers.Timer fallLoopTimer;
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
