using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku_wpf
{
    public class Sudoku
    {
        public bool start = false;
        private int n, m, level;
        private int size => n * m;
        private int[][] game, ans, ques; //遊戲中數獨 答案 題目
        Random rand = new Random(DateTime.Now.Millisecond);

        public Sudoku(int n, int m, int level) //初始建構子 (目前只能3*3)
        {
            start = true;
            this.n = n;
            this.m = m;
            this.level = level;
            do
                this.ans = Sudoku_sub();
            while (ans == null);
            game = createGame();
            ques = copyArr(game);
        }

        public Sudoku(int n, int m, int level, int[][] game, int[][] ans, int[][] ques) //讀檔用建構子
        {
            start = true;
            this.n = n;
            this.m = m;
            this.level = level;
            this.game = game;
            this.ans = ans;
            this.ques = ques;
        }

        public bool inputAns(int x, int y, int n) //輸入答案 ( 位置x , 位置y , 數字)
        {
            if (n > size || n < 0)  //防呆
                return false;
            if (x > size || x < 0)  //防笨
                return false;
            if (y > size || y < 0)  //防傻
                return false;
            if (ques[x][y] != 0)   //防低能
                return false;
            game[x][y] = n;
            return true;
        }

        public int[] checkWin() //回傳各值數量與是否勝利 //第一個值為0即為勝利
        {
            int[] ch = new int[size + 1];
            int ts = size * size;
            for (int t = 0; t != ts; t++)
            {
                ch[game[t / size][t % size]]++;
                if (game[t / size][t % size] != ans[t / size][t % size])
                    ch[0]++;
            }
            if (ch[0] == 0)
                start = false;
            return ch;
        }
        public bool[] checkGame() //檢查每格是否正確
        {
            bool[] cg = new bool[81];
            for (int t = 0; t != 81; t++)
            {
                int buff = game[t / size][t % size];
                game[t / size][t % size] = 0;
                cg[t] = isTrue(game, t, buff);
                game[t / size][t % size] = buff;
            }
            return cg;
        }

        public int[][][] getFullGame() => new[] { game, ans, ques }; //讀取遊戲盤面
        public int[] readData() => new int[] { n, m, level, size }; //讀取資料


        //------------------------------------------------後端---------------------------------------------------------
        private bool isTrue(int[][] arr, int xy, int num) //檢查是否合理
        {
            if (arr[xy / size][xy % size] != 0)
                return false;

            for (int t = 0; t != size; t++)
                if (arr[(xy / size / 3) * 3 + t / 3][(xy % size / 3) * 3 + t % 3] == num)
                    return false;

            for (int t = 0; t != size; t++)
            {
                if (xy % size != t)
                    if (arr[xy / size][t] == num)
                        return false;
                if (xy / size != t)
                    if (arr[t][xy % size] == num)
                        return false;
            }
            return true;
        }

        private int[][] Sudoku_sub() //模組化生成數獨
        {
            int[][] arr = new int[size][];
            for (int t = 0; t != size; t++)
                arr[t] = new int[size];

            int xyb, res;
            for (int t = 0; t != size * size; t++)
                arr[t / size][t % size] = 0;

            for (int t = 0; t != size; t++)
            {

                for (int t2 = 0; t2 != size; t2++)
                {
                    res = 0;
                    bool s = false;
                    xyb = rand.Next() % (size - 1);
                    Console.Write(t + " " + t2 + "\n");
                    do
                    {
                        if (res > size)                             //用下面的方法這裡要改大,上面的用9就可
                            return null;
                        xyb++; if (xyb > size - 1) xyb = 0;         //上下兩種二擇一
                                                                    //xyb = rand() % 9;							//此為另一種方法
                                                                    //s = !isTrue(arr, ((xyb / m) + (t2 / n * m)) * size + (xyb % n) + (t2 % m * n), t + 1);
                        s = !isTrue(arr, (xyb / n * size) + (xyb % n) /*內格*/ + (t2 / n * m) * size + (t2 % m * n) /*外格*/, t + 1);
                        res++;
                    } while (s);
                    arr[(xyb / m) + (t2 / n * m)][(xyb % n) + (t2 % m * n)] = t + 1;
                }
            }
            return arr;
        }

        private int[][] createGame() //挖空格
        {

            int[][] arr = new int[n * m][];
            for (int t = 0; t != n * m; t++)
                arr[t] = new int[m * n];

            do
            {
                arr = copyArr(ans);
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
            while (chechUniq(arr, 0, 0) > 2);
            return arr;
        }

        private int[][] copyArr(int[][] arr) //複製陣列
        {
            int[][] newArr = new int[size][];
            for (int t = 0; t != size; t++)
            {
                newArr[t] = new int[size];
                for (int t2 = 0; t2 != size; t2++)
                    newArr[t][t2] = arr[t][t2];
            }
            return newArr;
        }

        private int chechUniq(int[][] arr, int xy, int asn) //檢查唯一解
        {
            if (xy >= 81)
                return asn + 1;
            if (arr[xy / 9][xy % 9] != 0)
                asn = chechUniq(arr, xy + 1, asn);
            else
                for (int t = 1; t != 10; t++)
                {
                    if (isTrue(arr, xy, t))
                    {
                        arr[xy / 9][xy % 9] = t;
                        asn = chechUniq(arr, xy + 1, asn);
                        arr[xy / 9][xy % 9] = 0;
                        if (asn > 1)
                            return asn;
                    }
                }
            return asn;
        }
    }
}
