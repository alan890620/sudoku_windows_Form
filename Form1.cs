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
		//Random rand = new Random(DateTime.Now.Millisecond);
		public class sudoku
		{
			private int n, m, level, size;
			private int[,] game, ans, qs; //遊戲中數獨 答案 題目
			Random rand = new Random(DateTime.Now.Millisecond);

			public sudoku(int n, int m, int level) //初始建構子 (目前只能3*3)
			{
				this.n = n;
				this.m = m;
				this.size = m * n;
				this.level = level;
				//this.game = new int[n * m, m * n];
				do
					this.ans = sudoku_sub();
				while (ans == null);
				game = makegame();
				qs = copyarr(game);
			}
			public sudoku(int n, int m, int level, int[,] game, int[,] ans, int[,] qs) //讀檔用建構子
			{
				this.n = n;
				this.m = m;
				this.size = m * n;
				this.level = level;
				this.game = game;
				this.ans = ans;
				this.qs = qs;
			}
			public bool inputAns(int x, int y, int n) //輸入答案 ( 位置x , 位置y , 數字)
			{
				if (n > size || n < 0)  //防呆
					return false;
				if (x > size || x < 0)  //防笨
					return false;
				if (y > size || y < 0)  //防傻
					return false;
				if (qs[x, y] != 0)      //防低能
					return false;

				game[x, y] = n;
				return true;
			}
			public int[] chickWin() //回傳各值數量與是否勝利 //第一個值為0即為勝利
			{
				int[] ch = new int[size + 1];
				int ts = size * size;
				for (int t = 0; t != ts; t++)
				{
					ch[game[t / size, t % size]]++;
					if (game[t / size, t % size] != ans[t / size, t % size])
						ch[0]++;
				}
				return ch;
			}
			public int[,] readgame() //讀取正在進行的數獨
			{
				return game;
			}
			public int[,] readans() //讀取答案
			{
				return ans;
			}
			public int[,] readqs() //讀取題目
			{
				return qs;
			}
			public int[] readdata() //讀取資料
			{
				return new int[] { n, m, level, size };
			}
			public bool[] chickgame()
            {
				bool[] cg = new bool[81];
				for ( int t = 0; t != 81; t++ )
					cg[t] = isTrue(game, t, game[t / size, t % size]);
				return cg;
            }

			//------------------------------------------------後端---------------------------------------------------------
			private bool isTrue(int[,] arr, int xy, int num) //檢查是否合理
			{
				if (arr[xy / size, xy % size] != 0)
					return false;

				for (int t = 0; t != size; t++)
					if (arr[(xy / size / 3) * 3 + t / 3, (xy % size / 3) * 3 + t % 3] == num)
						return false;

				for (int t = 0; t != size; t++)
				{
					if (xy % size != t)
						if (arr[xy / size, t] == num)
							return false;
					if (xy / size != t)
						if (arr[t, xy % size] == num)
							return false;
				}
				return true;
			}
			private int[,] sudoku_sub() //模組化生成數獨
			{
				int[,] arr = new int[size, size];
				//int[] xy = new int[size];
				int xyb, res;
				for (int t = 0; t != size * size; t++)
					arr[t / size, t % size] = 0;

				for (int t = 0; t != size; t++)
				{
					for (int t2 = 0; t2 != size; t2++) ;
					//xy[t] = -1;

					for (int t2 = 0; t2 != size; t2++)
					{
						res = 0;
						bool s = false;
						xyb = rand.Next() % (size - 1);
						Console.Write(t + " " + t2 + "\n");
						do
						{
							if (res > size)                             //用下面的方法這裡要改大,上面的用9就可
							{
								//cout << t << " " << t2 << endl;
								return null;
							}
							xyb++; if (xyb > size - 1) xyb = 0;         //上下兩種二擇一
																		//xyb = rand() % 9;							//此為另一種方法
																		//s = !isTrue(arr, ((xyb / m) + (t2 / n * m)) * size + (xyb % n) + (t2 % m * n), t + 1);
							s = !isTrue(arr, (xyb / n * size) + (xyb % n) /*內格*/ + (t2 / n * m) * size + (t2 % m * n) /*外格*/, t + 1);
							res++;
						} while (s);
						//xy[t2] = xyb;
						arr[(xyb / m) + (t2 / n * m), (xyb % n) + (t2 % m * n)] = t + 1;
					}
				}
				return arr;
			}
			private int[,] makegame() //挖空格
			{

				int[,] arr = new int[n * m, m * n];

				do
				{
					arr = copyarr(ans);
					for (int t = 0; t != level;)
					{
						int r = rand.Next() % 81;
						if (arr[r / 9, r % 9] != 0)
						{
							t++;
							arr[r / 9, r % 9] = 0;
						}
						//cout << t << endl ;
					}
				}
				while (asSuduku(arr, 0, 0) > 2);
				return arr;
			}
			private int[,] copyarr(int[,] arr) //複製陣列
			{
				int[,] newArr = new int[size, size];
				for (int t = 0; t != size; t++)
					for (int t2 = 0; t2 != size; t2++)
					{
						newArr[t, t2] = arr[t, t2];
					}
				return newArr;
			}

			private int asSuduku(int[,] arr, int xy, int asn) //檢查唯一解
			{
				//cout << xy << endl ;
				if (xy >= 81)
				{
					//asn += 1;
					//draw(arr, 9, 3);
					return asn + 1;
				}
				if (arr[xy / 9, xy % 9] != 0)
					asn = asSuduku(arr, xy + 1, asn);
				else
					for (int t = 1; t != 10; t++)
					{
						if (isTrue(arr, xy, t))
						{
							arr[xy / 9, xy % 9] = t;
							asn = asSuduku(arr, xy + 1, asn);
							arr[xy / 9, xy % 9] = 0;
							if (asn > 1)
								return asn;
						}
					}
				//draw(arr, 9, 3);
				//cout << "error" << endl;
				return asn;
			}
		}

		/*
		
		┼┴┬┤├─│┌┐└┘ ═╞╪╡╔╦╗╠╬╣╚╩╝╒╤╕╘╧╛╓╥╖╟╫╢╙╨╜║

		┌ n * m ┐
		m		m
		*		*
		n		n
		└ n * m ┘

		*/

		//------------------------------------------------前端---------------------------------------------------------

		sudoku game;
		Button[] buttons , inputButtons;
		Label[] numshows;
		int click = -1;

		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			
			//NOTHING
			load_item_field();

			for ( int t = 0; t != 81; t++)
			{
				buttons[t].Text = " ";
				buttons[t].Dock = DockStyle.Fill;
				buttons[t].Margin = new System.Windows.Forms.Padding(1);
			}
			//*/
			tableLayoutPanel11.Enabled = false;

			Form1_Resize( sender , e );
		}
		private void load_item_field()
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
			catch ( Exception e )
            {

            }
		}

		private void start_Click(object sender, EventArgs e) //開始按鈕
        {
			game = new sudoku(3,3,(int)numericUpDown1.Value);
			showGame(game.readgame() , game.readqs() , -1 , game.chickWin(), game.chickgame());
			tableLayoutPanel11.Enabled = true;
		}

		Color normal_as = Color.FromArgb(255, 255, 255) //答案普通
			, normal_he = Color.FromArgb(200, 200, 255) //題目普通
			, pick_as = Color.FromArgb(200, 200, 200)   //答案選取
			, pick_he = Color.FromArgb(150, 150, 200)   //題目選取
			, high_as = Color.FromArgb(220, 220, 220)   //答案高亮
			, high_he = Color.FromArgb(180, 180, 230)   //題目高亮
			, wrong_he = Color.FromArgb(255, 200, 200)  //答案錯誤
			, wrong_hight_he = Color.FromArgb(230, 180, 180);  //高亮答案錯誤


        private void showGame( int[,] game , int[,] qs , int pick , int[] cw , bool[] cg) //顯示
        {
			for ( int t = 0; t != 81; t ++)
			{
				buttons[t].ForeColor = Color.Black;
				if (game[t / 9, t % 9] != 0) //數字顯示
					buttons[t].Text = game[t / 9, t % 9].ToString();
				else
					buttons[t].Text = "";

				if (qs[t / 9, t % 9] != 0) //背景色 檢查是否為題目
					if (t == pick) //是否為選取格
						buttons[t].BackColor = pick_he;//題目
					else
						if ((t / 9 == pick / 9 || t % 9 == pick % 9) && pick >= 0) //是否在十字線上
							buttons[t].BackColor = high_he;
						else
							buttons[t].BackColor = normal_he;
				else
				{
					if (t == pick)
						buttons[t].BackColor = pick_as;
					else
						if ((t / 9 == pick / 9 || t % 9 == pick % 9) && pick >= 0)
							buttons[t].BackColor = high_as;
						else
							buttons[t].BackColor = normal_as;
					if (cg[t])
						buttons[t].ForeColor = Color.Black;
					else
						buttons[t].ForeColor = Color.Red;
				}
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
			//listBox1.Items.Add( pick );
        }

        private void as_Click(object sender, EventArgs e) //選取格
		{
			click = Array.IndexOf(buttons,sender);
			showGame(game.readgame(), game.readqs(), click, game.chickWin() , game.chickgame());
			//listBox1.Items.Add(click.ToString());
		}
		private void input_Click(object sender, EventArgs e) //輸入數字 UI
		{
			int input = Array.IndexOf(inputButtons, sender) ;
			listBox1.Items.Add(input);
			game.inputAns(click / 9, click % 9, input);
			showGame(game.readgame(), game.readqs(), click, game.chickWin(), game.chickgame());
		}
		private void KeyP(object sender, KeyPressEventArgs e) //輸入數字 鍵盤
		{
			if (e.KeyChar >= 48 && e.KeyChar <= 57)
			{
				int buffer = (int)e.KeyChar - 47;
			}
		}

		private void Form1_Resize(object sender, EventArgs e) //視窗尺寸改變
        {
			tableLayoutPanel11.Size = new Size( panel1.Size.Width -24 , ( panel1.Size.Width -24 ) + 80 ) ;

		}

	}
}
