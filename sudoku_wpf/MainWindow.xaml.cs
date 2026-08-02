using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
using Xceed.Wpf.Toolkit;

namespace sudoku_wpf
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
		
		┼┴┬┤├─│┌┐└┘ ═╞╪╡╔╦╗╠╬╣╚╩╝╒╤╕╘╧╛╓╥╖╟╫╢╙╨╜║

		┌ n * m ┐
		m		m
		*		*
		n		n
		└ n * m ┘

		*/

        private Sudoku game;
        private Button[] buttons, inputButtons;
        private Label[] numshows;
        private int click = 0;


        //0題目 1答案 2錯的答案
        //0選取 1高亮 2普通
        private static Color[][] blockColer =
        {
            new[] { Color.FromRgb(  0, 114, 255), Color.FromRgb( 70, 160, 255), Color.FromRgb(172, 206, 255) },	//題目
			new[] { Color.FromRgb(150, 150, 150), Color.FromRgb(180, 180, 180), Color.FromRgb(255, 255, 255) },	//答案
			new[] { Color.FromRgb(230, 160, 140), Color.FromRgb(230, 190, 170), Color.FromRgb(255, 200, 180) }	//錯誤
		};

        public MainWindow() //視窗初始化
        {
            InitializeComponent();

            load_item_field();


            try //嘗試讀檔
            {
                game = FileCtr.load();
                showGame(-1);
                SetEnable( true );
            }
            catch (Exception ex) 
            {
                SetEnable( false);
            }

            //Form1_Resize(sender, e);

        }

        private void load_item_field() //初始化按鈕
        {
            buttons = new Button[ 81 ];
            inputButtons = new Button[10];
            numshows = new Label[9];

            for (int t = 0; t != 9; t++)//建立九宮格內框
            {
                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int t2 = 0; t2 != 9; t2++)//建立九宮格按鈕
                {
                    int buttonNo = (t * 3 + (t / 3 * 18) + t2 + (t2 / 3 * 6));
                    buttons[ buttonNo ] = new Button();
                    buttons[buttonNo].Content = /*"btn" + buttonNo*/ "";
                    buttons[buttonNo].Click += as_Click;
                    Grid.SetRow(buttons[buttonNo], t2 / 3);
                    Grid.SetColumn(buttons[buttonNo], t2 % 3);
                    grid.Children.Add(buttons[buttonNo]);
                }
                Grid.SetRow(grid, t / 3);
                Grid.SetColumn(grid, t % 3);
                bigSudokuBox.Children.Add(grid);
            }


            inputButtons[0] = new Button();
            inputButtons[0].Content = "清除";
            inputButtons[0].Click += input_Click;
            inputButtons[0].ClipToBounds = false;
            inputButtons[0].Width = 100;
            inputButtons[0].HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(inputButtons[0], 2);
            Grid.SetColumn(inputButtons[0], 0);
            Grid.SetColumnSpan(inputButtons[0], 9);
            inputGrid.Children.Add(inputButtons[0]);

            for (int t = 0; t != 9; t++)//建立輸入區
            {
                inputButtons[t + 1] = new Button();
                inputButtons[t + 1].Content = ( t + 1 ).ToString();
                inputButtons[t + 1].Click += input_Click;
                Grid.SetRow(inputButtons[t + 1], 0 );
                Grid.SetColumn(inputButtons[t + 1], t );
                inputGrid.Children.Add(inputButtons[t + 1]);

                numshows[t] = new Label();
                numshows[t].HorizontalContentAlignment  = HorizontalAlignment.Center;
                numshows[t].VerticalContentAlignment = VerticalAlignment.Center;
                Grid.SetRow(numshows[t], 1);
                Grid.SetColumn(numshows[t], t);
                inputGrid.Children.Add(numshows[t]);
            }

        }

        private void Start_Click(object sender, RoutedEventArgs e)//開始按鈕
        {
            game = new Sudoku(3, 3, (int)NumLevel.Value);
            showGame(-1);
            FileCtr.save(game);
            SetEnable(true);
        }

        private void showGame(int pick) //顯示
        {
            int[][][] fullArr = game.getFullGame();
            int[] cw = game.checkWin();
            bool[] cg = game.checkGame();
            for (int t = 0; t != 81; t++)
            {
                if (fullArr[0][t / 9][t % 9] != 0) //數字顯示
                    buttons[t].Content = fullArr[0][t / 9][t % 9].ToString();
                else
                    buttons[t].Content = "";

                int c1 = 1, c2 = 2;
                if (fullArr[2][t / 9][t % 9] != 0) //檢查是否為題目
                    c1 = 0;
                if (t == pick) //是否為選取格
                    c2 = 0;
                else
                    if ((t / 9 == pick / 9 || t % 9 == pick % 9) && pick >= 0) //是否在十字線上
                        c2 = 1;
                if (!cg[t] && buttons[t].Content != "" && c1 != 0) //是否為錯誤格
                {
                    c1 = 2;
                    buttons[t].Foreground = Brushes.Red;
                }
                else
                    buttons[t].Foreground = Brushes.Black;
                buttons[t].Background = new SolidColorBrush(blockColer[c1][c2]);
            }
            for (int t = 0; t != 9; t++) //數量顯示
            {
                numshows[t].Foreground = Brushes.Black;
                numshows[t].Content = cw[t + 1].ToString();
                if (cw[t + 1] > 8)
                    if (cw[t + 1] > 9)
                        numshows[t].Foreground = Brushes.Red;
                    else
                        numshows[t].Foreground = Brushes.Blue;
            }

            if (game.start == false)
            {
                listBox1.Items.Add("-WIN-");
                SetEnable(false);
            }//*/
        }//*/

        private void as_Click(object sender, EventArgs e) //選取格
        {
            click = Array.IndexOf(buttons, sender);
            showGame(click);
        }

        private void input_Click(object sender, EventArgs e) //輸入數字 UI
        {
            KeyInAns(Array.IndexOf(inputButtons, sender));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) //鍵盤輸入事件
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 && bigSudokuBox.IsEnabled)
                KeyInAns((int)( e.Key - Key.D0 ));
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 && bigSudokuBox.IsEnabled)
                KeyInAns((int)(e.Key - Key.NumPad0));
        }//*/

        private void KeyInAns(int input) //輸入
        {
            listBox1.Items.Add(input);
            game.inputAns(click / 9, click % 9, input);
            showGame(click);
            FileCtr.save(game);
        }

        private void SetEnable(bool flag) //設定UI狀態
        {
            bigSudokuBox.IsEnabled = flag;
            inputGrid.IsEnabled = flag;
        }
    }

}

