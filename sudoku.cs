using System;
using System.IO;
using System.Xml.Serialization;

namespace sudoku_win
{
	public class Sudoku
	{
        private static readonly string filePath = "Save.xml"; //存檔位置

        private int Size => n * m;

        private int n, m, level;		
		private int[][] game, ans, qs; //遊戲中數獨 答案 題目
        private Random rand = new Random(DateTime.Now.Millisecond);

        public Sudoku(int n, int m, int level) //初始建構子 (目前只能3*3)
		{
			this.n = n;
			this.m = m;
			this.level = level;
			
			do
            {
                this.ans = SudokuSub();
            }
			while (ans == null);

			game = MakeGame();
			qs = CopyArr(game);
		}

        private Sudoku(int n, int m, int level, int[][] game, int[][] ans, int[][] qs) //讀檔用建構子
        {
            this.n = n;
            this.m = m;
            //this.size = m * n;
            this.level = level;
            this.game = game;
            this.ans = ans;
            this.qs = qs;
        }

        public bool InputAns(int x, int y, int n) //輸入答案 ( 位置x , 位置y , 數字)
		{
			if (n > Size || n < 0)  //防呆
				return false;

			if (x > Size || x < 0)  //防笨
				return false;

			if (y > Size || y < 0)  //防傻
				return false;

			if (qs[x][ y] != 0)      //防低能
				return false;

			game[x][ y] = n;
			return true;
		}

        public int[] CheckWin() //回傳各值數量與是否勝利 //第一個值為0即為勝利
		{
			int[] ch = new int[Size + 1];
			int ts = Size * Size;

			for (int t = 0; t != ts; t++)
			{
				ch[game[t / Size][ t % Size]]++;
				if (game[t / Size][ t % Size] != ans[t / Size][ t % Size])
					ch[0]++;
			}

			return ch;
		}

        /// <summary>
        /// 讀取正在進行的數獨
        /// </summary>
		public int[][] ReadGame() => game;

        /// <summary>
        /// 讀取答案
        /// </summary>
		public int[][] ReadAnswer() => ans;

        /// <summary>
        /// 讀取題目
        /// </summary>
		public int[][] ReadQuestion() => qs;

		public int[] ReadData() //讀取資料
		{
			return new int[] { n, m, level, Size };
		}

        public bool[] CheckGame() //檢查勝利
		{
			bool[] cg = new bool[81];
			for (int t = 0; t != 81; t++)
			{
				int buff = game[t / Size][ t % Size];
				game[t / Size][ t % Size] = 0;
				cg[t] = IsTrue(game, t, buff);
				game[t / Size][ t % Size] = buff;
			}
			return cg;
		}

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                
                serializer.Serialize(fs, new GameData(this));
            }
        }

        public static Sudoku Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                GameData data = (GameData)serializer.Deserialize(fs);
                return new Sudoku(data.n, data.m, data.level, data.game, data.ans, data.qs);
            }
        }

        public static void Delete()
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e) { }
        }

        //------------------------------------------------後端---------------------------------------------------------
        private bool IsTrue(int[][] arr, int xy, int num) //檢查是否合理
		{
			if (arr[xy / Size][ xy % Size] != 0)
				return false;

			for (int t = 0; t != Size; t++)
				if (arr[(xy / Size / 3) * 3 + t / 3][ (xy % Size / 3) * 3 + t % 3] == num)
					return false;

			for (int t = 0; t != Size; t++)
			{
				if (xy % Size != t)
					if (arr[xy / Size][t] == num)
						return false;

				if (xy / Size != t)
					if (arr[t][xy % Size] == num)
						return false;
			}
			return true;
		}

        private int[][] SudokuSub() //模組化生成數獨
		{
			int[][] arr = new int[Size][];
			for (int t = 0; t != Size; t++)
				arr[t] = new int[Size]; 

			int xyb, res;
			for (int t = 0; t != Size * Size; t++)
				arr[t / Size][t % Size] = 0;

			for (int t = 0; t != Size; t++)
			{
				for (int t2 = 0; t2 != Size; t2++) ;
				//xy[t] = -1;

				for (int t2 = 0; t2 != Size; t2++)
				{
					res = 0;
					bool s = false;
					xyb = rand.Next() % (Size - 1);
					Console.Write(t + " " + t2 + "\n");
					do
					{
						if (res > Size)                             //用下面的方法這裡要改大,上面的用9就可
						{
							//cout << t << " " << t2 << endl;
							return null;
						}
						xyb++; if (xyb > Size - 1) xyb = 0;         //上下兩種二擇一
																	//xyb = rand() % 9;							//此為另一種方法
																	//s = !isTrue(arr, ((xyb / m) + (t2 / n * m)) * size + (xyb % n) + (t2 % m * n), t + 1);
						s = !IsTrue(arr, (xyb / n * Size) + (xyb % n) /*內格*/ + (t2 / n * m) * Size + (t2 % m * n) /*外格*/, t + 1);
						res++;
					} while (s);
					//xy[t2] = xyb;
					arr[(xyb / m) + (t2 / n * m)][(xyb % n) + (t2 % m * n)] = t + 1;
				}
			}
			return arr;
		}

        private int[][] MakeGame() //挖空格
		{

			int[][] arr = new int[n * m][];
            for (int t = 0; t != n * m; t++)
                arr[t] = new int[m * n ];

            do
			{
				arr = CopyArr(ans);
				for (int t = 0; t != level;)
				{
					int r = rand.Next() % 81;
					if (arr[r / 9][r % 9] != 0)
					{
						t++;
						arr[r / 9][r % 9] = 0;
					}
				}
			}
			while (AsSuduku(arr, 0, 0) > 2);
			return arr;
		}

        private int[][] CopyArr(int[][] arr) //複製陣列
		{
			int[][] newArr = new int[Size][];
            for (int t = 0; t != Size; t++)
                newArr[t] = new int[Size];

            for (int t = 0; t != Size; t++)
				for (int t2 = 0; t2 != Size; t2++)
					newArr[t][t2] = arr[t][t2];

			return newArr;
		}

		private int AsSuduku(int[][] arr, int xy, int asn) //檢查唯一解
		{
			if (xy >= 81)
				return asn + 1;

			if (arr[xy / 9][xy % 9] != 0)
            {
                asn = AsSuduku(arr, xy + 1, asn);
            }
			else
            {
                for (int t = 1; t != 10; t++)
                {
                    if (IsTrue(arr, xy, t))
                    {
                        arr[xy / 9][xy % 9] = t;
                        asn = AsSuduku(arr, xy + 1, asn);
                        arr[xy / 9][xy % 9] = 0;

                        if (asn > 1)
                            return asn;
                    }
                }
            }

			return asn;
		}

        public class GameData
        {
            public int n;
            public int m;
            public int level;
            public int[][] game; // 遊戲中數獨
            public int[][] ans;  // 答案
            public int[][] qs;   // 題目

            public GameData(Sudoku data)
            {
                int[] buff = data.ReadData();
                n = buff[0];
                m = buff[1];
                level = buff[2];

                game = data.ReadGame();
                ans = data.ReadAnswer();
                qs = data.ReadQuestion();
            }

            private GameData() { }
        }
    }
}
