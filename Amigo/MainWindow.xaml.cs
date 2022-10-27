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
        Board board;
        public void Start()
        {
            board = new(gameLevel, x, y);
            StartUpdateLoop();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                board.Rotate(activePill, true);
            }
            if (e.Key == Key.A)
            {
                board.Rotate(activePill, false);
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
                if (activePill == null)
                {
                    activePill = new(random.Next(Enum.GetNames(typeof(Color)).Length), random.Next(Enum.GetNames(typeof(Color)).Length));
                    board.Add(new Vector(3,1), activePill.onePiece);
                    board.Add(new Vector(4,1), activePill.twoPiece);
                }

                Grid grid = new Grid();
                grid.ShowGridLines = true;
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
                        TextBlock tb = new TextBlock();
                        tb.Text = "virus";
                        tb.Foreground = ParseColor(tile.color);
                        Vector pos = board.GetPos(tile);
                        Grid.SetColumn(tb, (int)pos.X);
                        Grid.SetRow(tb, (int)pos.Y);

                        grid.Children.Add(tb);
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
                        
                        if (p == activePill.twoPiece)
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
                this.Content = grid;
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

        public Brush ParseColor(Color c)
        {
            switch (c)
            {
                case Color.red:
                    return Brushes.Red;
                case Color.blue:
                    return Brushes.Blue;
                case Color.yellow:
                    return Brushes.Yellow;
                default:
                    return Brushes.White;
            }
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
