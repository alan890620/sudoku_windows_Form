using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace sudoku_win
{
    public struct Su_data //結構
    {
        public int n, m, level;
        public int[][] game, ans, qs;

        public Su_data(Sudoku data)
        {
            int[] buff = data.ReadData();
            n = buff[0];
            m = buff[1];
            level = buff[2];
            game = data.ReadGame();
            ans = data.ReadAnswer();
            qs = data.ReadQuestion();
        }
    }

    public class FileCtr
    {
        private static String filePath = "Save.xml"; //存檔位置


        public static void save( Sudoku su ) //存檔
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Su_data));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, new Su_data(su));
            }
        }

        public static Sudoku load() //讀檔
        {
            Su_data buff ;

            XmlSerializer serializer = new XmlSerializer(typeof(Su_data));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                buff = (Su_data)serializer.Deserialize(fs);
            }
            return new Sudoku(buff.n, buff.m, buff.level, buff.game, buff.ans, buff.qs);
        }

        public static void delete() //刪除
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e) { }
        }//*/

    }
}