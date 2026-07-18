using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace sudoku_win
{
    public partial class Form1 : Form
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
		private Button[] buttons , inputButtons;
		private Label[] numshows;
		private int click = 0;
		private bool debugMode = false;

		//0題目 1答案 2錯的答案
		//0選取 1高亮 2普通
		private static Color[][] blockColer =
		{
			new[] { Color.FromArgb(  0, 114, 255), Color.FromArgb( 70, 160, 255), Color.FromArgb(172, 206, 255) },	//題目
			new[] { Color.FromArgb(150, 150, 150), Color.FromArgb(180, 180, 180), Color.FromArgb(255, 255, 255) },	//答案
			new[] { Color.FromArgb(230, 160, 140), Color.FromArgb(230, 190, 170), Color.FromArgb(255, 200, 180) }	//錯誤
		};

		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) //視窗初始化
        {
			load_item_field();
			tableLayoutPanel11.Enabled = false;

			try //嘗試讀檔
			{
				game = FileCtr.load();
                showGame(-1);
                tableLayoutPanel11.Enabled = true;
            }
			catch (Exception ex) { }

			if ( !debugMode )
			{
				listBox1.Visible = false;
			}

			Form1_Resize( sender , e );
		}

		private void load_item_field() //初始化按鈕
        {
			try
			{
				buttons = new Button[81];
				inputButtons = new Button[10];
				numshows = new Label[9];
				for ( int t = 0; t != 81; t++ )
					buttons[t] = (Button)typeof(Form1).GetField("AsNum" + t, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
				for (int t = 0; t != 10; t++)
					inputButtons[t] = (Button)typeof(Form1).GetField("inputButton" + (t), BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
				for (int t = 0; t != 9; t++)
					numshows[t] = (Label)typeof(Form1).GetField("numShow" + (t + 1), BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
			}
			catch ( Exception e ) { }

            for (int t = 0; t != 81; t++)
            {
                buttons[t].Text = " ";
                buttons[t].Dock = DockStyle.Fill;
                buttons[t].Margin = new System.Windows.Forms.Padding(1);
            }
        }

		private void start_Click(object sender, EventArgs e) //開始按鈕
        {
			game = new Sudoku(3,3,(int)numericUpDown1.Value);
            showGame(-1);
            FileCtr.save(game);
            tableLayoutPanel11.Enabled = true;
		}

		private void showGame( int pick ) //顯示
        {
			int[][][] fullArr = game.getFullGame();
			int[] cw = game.checkWin();
			bool[] cg = game.checkGame();
			for ( int t = 0; t != 81; t ++)
			{
				if (fullArr[0][t / 9][ t % 9] != 0) //數字顯示
					buttons[t].Text = fullArr[0][t / 9][ t % 9].ToString();
				else
					buttons[t].Text = "";

				int c1 = 1, c2 = 2;
				if (fullArr[2][t / 9][ t % 9] != 0) //檢查是否為題目
					c1 = 0;
				if (t == pick) //是否為選取格
					c2 = 0;
				else
					if ((t / 9 == pick / 9 || t % 9 == pick % 9) && pick >= 0) //是否在十字線上
						c2 = 1;
				if (!cg[t] && buttons[t].Text != "" && c1 != 0) //是否為錯誤格
				{
					c1 = 2;
					buttons[t].ForeColor = Color.Red;
				}
				else
					buttons[t].ForeColor = Color.Black;
				buttons[t].BackColor = blockColer[c1][c2];
			}
			for ( int t = 0; t != 9; t ++ ) //數量顯示
            {
				numshows[t].ForeColor = Color.Black;
				numshows[t].Text = cw[t + 1].ToString();
				if (cw[t + 1] > 8)
					if (cw[t + 1] > 9)
						numshows[t].ForeColor = Color.Red;
					else
						numshows[t].ForeColor = Color.Blue;
            }

			if ( game.start == false )
            {
                listBox1.Items.Add( "-WIN-" );
                tableLayoutPanel11.Enabled = false;
			}
        }

        private void as_Click(object sender, EventArgs e) //選取格
		{
			click = Array.IndexOf(buttons,sender);
			showGame(click );
        }

		private void input_Click(object sender, EventArgs e) //輸入數字 UI
		{
			KeyInAns(Array.IndexOf(inputButtons, sender));
		}

		private void KeyP(object sender, KeyPressEventArgs e) //鍵盤輸入事件
		{
			if (e.KeyChar >= 48 && e.KeyChar <= 57 && tableLayoutPanel11.Enabled)
				KeyInAns((int)e.KeyChar - 48);
		}

		private void KeyInAns ( int input ) //輸入
		{
			listBox1.Items.Add(input);
			game.inputAns(click / 9, click % 9, input);
            showGame(click);
            FileCtr.save(game);
        }

		private void Form1_Resize(object sender, EventArgs e) //視窗尺寸改變
        {
			tableLayoutPanel11.Size = new Size( panel1.Size.Width -24 , ( panel1.Size.Width -24 ) + 80 ) ;
			Font resize = new Font("新細明體", (int)(buttons[5].Width * 0.5));
			for ( int t = 0; t != 81; t ++ )
                buttons[t].Font =  resize;
		}
	}
}
