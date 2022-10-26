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
        PillPiece player;
        public void Start()
        {
            board = new(gameLevel, x, y);
            player = new(1);
            board.Add(new Vector(2, 2), player);
            StartUpdateLoop();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                Vector vec = board.GetPos(player);
                vec.X++;
                board.Remove(board.GetPos(player));
                board.Add(vec, player);
            }
            if (e.Key == Key.A)
            {
                Vector vec = board.GetPos(player);
                vec.X--;
                board.Remove(board.GetPos(player));
                board.Add(vec, player);
            }
        }
        private void Update(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() => 
            {
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
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\test.png"));
                        Vector pos = board.GetPos(tile);
                        Grid.SetColumn(img, (int)pos.X);
                        Grid.SetRow(img, (int)pos.Y);
                        grid.Children.Add(img);
                    }
                }
                this.Content = grid;
            }));
        }
        
        System.Timers.Timer loopTimer;
        public void StartUpdateLoop()
        {
            //make timer
            loopTimer = new System.Timers.Timer(1000 / ups);
            loopTimer.Enabled = true;
            loopTimer.Elapsed += Update;
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
    }
}
