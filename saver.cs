using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace sudoku_win
{
    public struct SudokuData //結構
    {
        public int n, m, level;
        public int[][][] game;

        public SudokuData(Sudoku data)
        {
            int[] buff = data.readData();
            n = buff[0];
            m = buff[1];
            level = buff[2];
            game = data.getFullGame();
        }
    }

    public class FileCtr
    {
        private static String filePath = "Save.xml"; //存檔位置

        public static void save( Sudoku su ) //存檔
        {
            if (su.start == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SudokuData));
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fs, new SudokuData(su));
                }
            }
            else //如果結束就刪掉
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception e) { }
            }
        }

        public static Sudoku load() //讀檔
        {
            SudokuData buff ;

            XmlSerializer serializer = new XmlSerializer(typeof(SudokuData));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                buff = (SudokuData)serializer.Deserialize(fs);
            }
            return new Sudoku(buff.n, buff.m, buff.level, buff.game[0], buff.game[1], buff.game[2]);
        }

    }
}