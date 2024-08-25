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
				if (n > size || n < 1)  //防呆
					return false;
				if (x > size || x < 1)  //防笨
					return false;
				if (y > size || y < 1)  //防傻
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
			public int[] readdata() //讀取資料 ( )
			{
				return new int[] { n, m, level, size };
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

		sudoku game;
		Button[] buttons;

		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			
			//NOTHING
			//load_button();
			load_button_field();

			for ( int t = 0; t != 81; t++)
			{
				buttons[t].Text = " ";
				buttons[t].Dock = DockStyle.Fill;
				buttons[t].Margin = new System.Windows.Forms.Padding(1);
				buttons[t].Enabled = false;
				listBox1.Items.Add("AsNum" + t);
			}
			//*/
		}
		private void load_button()
		{
			int t = 0;
			buttons = new Button[81];
			buttons[t++] = AsNum0;
			buttons[t++] = AsNum1;
			buttons[t++] = AsNum2;
			buttons[t++] = AsNum3;
			buttons[t++] = AsNum4;
			buttons[t++] = AsNum5;
			buttons[t++] = AsNum6;
			buttons[t++] = AsNum7;
			buttons[t++] = AsNum8;
			////////////////////////////
			buttons[t++] = AsNum9;
			buttons[t++] = AsNum10;
			buttons[t++] = AsNum11;
			buttons[t++] = AsNum12;
			buttons[t++] = AsNum13;
			buttons[t++] = AsNum14;
			buttons[t++] = AsNum15;
			buttons[t++] = AsNum16;
			buttons[t++] = AsNum17;
			////////////////////////////
			buttons[t++] = AsNum18;
			buttons[t++] = AsNum19;
			buttons[t++] = AsNum20;
			buttons[t++] = AsNum21;
			buttons[t++] = AsNum22;
			buttons[t++] = AsNum23;
			buttons[t++] = AsNum24;
			buttons[t++] = AsNum25;
			buttons[t++] = AsNum26;
			////////////////////////////
			buttons[t++] = AsNum27;
			buttons[t++] = AsNum28;
			buttons[t++] = AsNum29;
			buttons[t++] = AsNum30;
			buttons[t++] = AsNum31;
			buttons[t++] = AsNum32;
			buttons[t++] = AsNum33;
			buttons[t++] = AsNum34;
			buttons[t++] = AsNum35;
			////////////////////////////
			buttons[t++] = AsNum36;
			buttons[t++] = AsNum37;
			buttons[t++] = AsNum38;
			buttons[t++] = AsNum39;
			buttons[t++] = AsNum40;
			buttons[t++] = AsNum41;
			buttons[t++] = AsNum42;
			buttons[t++] = AsNum43;
			buttons[t++] = AsNum44;
			////////////////////////////
			buttons[t++] = AsNum45;
			buttons[t++] = AsNum46;
			buttons[t++] = AsNum47;
			buttons[t++] = AsNum48;
			buttons[t++] = AsNum49;
			buttons[t++] = AsNum50;
			buttons[t++] = AsNum51;
			buttons[t++] = AsNum52;
			buttons[t++] = AsNum53;
			////////////////////////////
			buttons[t++] = AsNum54;
			buttons[t++] = AsNum55;
			buttons[t++] = AsNum56;
			buttons[t++] = AsNum57;
			buttons[t++] = AsNum58;
			buttons[t++] = AsNum59;
			buttons[t++] = AsNum60;
			buttons[t++] = AsNum61;
			buttons[t++] = AsNum62;
			////////////////////////////
			buttons[t++] = AsNum63;
			buttons[t++] = AsNum64;
			buttons[t++] = AsNum65;
			buttons[t++] = AsNum66;
			buttons[t++] = AsNum67;
			buttons[t++] = AsNum68;
			buttons[t++] = AsNum69;
			buttons[t++] = AsNum70;
			buttons[t++] = AsNum71;
			////////////////////////////
			buttons[t++] = AsNum72;
			buttons[t++] = AsNum73;
			buttons[t++] = AsNum74;
			buttons[t++] = AsNum75;
			buttons[t++] = AsNum76;
			buttons[t++] = AsNum77;
			buttons[t++] = AsNum78;
			buttons[t++] = AsNum79;
			buttons[t++] = AsNum80;
			////////////////////////////
		}
		private void load_button_field()
        {
			try
			{
				/*
				FieldInfo[] GetButton = typeof(Form1).GetFields(BindingFlags.NonPublic | BindingFlags.Instance) ;
				for (int t = 0; t != GetButton.Length; t++)
					listBox1.Items.Add( "Name:" + GetButton[t].Name  +"\n" );
				//*/
				//FieldInfo[] GetButton;
				for ( int t = 0; t != 81; t++ )
                {
					listBox1.Items.Add("AsNum" + t);
					buttons[t] = (Button)typeof(Form1).GetField("AsNum" + t, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
				}
			}
			catch ( Exception e )
            {
				load_button();
            }
		}
		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void start_Click(object sender, EventArgs e)
        {
			game = new sudoku(3,3,(int)numericUpDown1.Value);
			showGame(game.readgame());
        }

		private void showGame( int[,] show )
        {
			for ( int t = 0; t != 81; t ++)
            {
				buttons[t].Text = show[t/9,t%9].ToString() ;
				if (buttons[t].Text != "0")
					buttons[t].Enabled = false;
				else
					buttons[t].Enabled = true;
			}
        }


        private void AsNum48_Click(object sender, EventArgs e)
        {
        }

        private void as_Click(object sender, EventArgs e)
		{
			int click = Array.IndexOf(buttons,e);
		}

        private void Form1_Resize(object sender, EventArgs e)
        {
			//tableLayoutPanel1.Size = new Size( Form1.Size.Width , );
        }

    }
}
