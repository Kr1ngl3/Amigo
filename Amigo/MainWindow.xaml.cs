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

namespace Amigo
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        public MainWindow()
        {
            InitializeComponent();

            Grid grid = new Grid();
            grid.ShowGridLines = true;
            
            for (int i = 0; i < 8; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < 13; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            Board board = new Board(5);

            foreach (Tile tile in board.board)
            { 
                if (tile != null)
                {
                    if (tile.pos == null)
                        break;
                    TextBlock tb = new TextBlock();
                    tb.Text = "virus";
                    tb.Foreground = ParseColor(tile.color);
                    Grid.SetColumn(tb, tile.pos.x);
                    Grid.SetRow(tb, tile.pos.y);

                    grid.Children.Add(tb);
                }
            }
            this.Content = grid;
        }
    }
}
