using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelDna.Integration;

namespace UyExcelFunctions
{
    public  class Uyghur
    {
        //private  static Syntax syn;
        public  Uyghur()
        {
            //syn = new Syntax();
        }
        private static char[] Sozuq = {'ا','ە','ې','ى','و','ۇ','ۆ','ۈ'};
        private static char[] TilArqa = { 'ا','ۇ','ې','و',};
        private static char[] TilAldi = { 'ە','ۈ','ۆ','ى',};

        [ExcelFunction(Description = "Convert Latin to Uyghur", Category = "UyghurDev.net functions")]
        public static string Latin2Uyghur(string str)
        {
            Syntax syn = new Syntax();
            return syn.getUyStrFromUKY(str,true);
        }

        [ExcelFunction(Description = "Convert Uyghur t Latin", Category = "UyghurDev.net functions")]
        public static string Uyghur2Latin(string str)
        {
            Syntax syn = new Syntax();
            return syn.getUKYFromUy(str);
        }

        [ExcelFunction(Description = "About Uyghur Functions", Category = "UyghurDev.net functions")]
        public static string About()
        {
            Syntax syn = new Syntax();
            return "Uyghur Functions\n Author:Sarwan(Eli Erkin)\n UyghurDev.net \n " + syn.getUKYFromUy("ئۇيغۇر يۇمشاق دېتال ئىجادىيەت تورى");
        }

        [ExcelFunction(Description = "Shortner. ex:Uyghur Software Developer Network to USDN", Category = "UyghurDev.net functions")]
        public static string toShort(string str)
        {
            string[] stra = str.Trim().Split(new char[] { ' ',',', ')',  '(', '?','.' });
            string strShort = "";
            foreach(string s in stra)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    strShort = strShort + s.Substring(0, 1);
                }
            }
            return strShort;
        }

        public static string fixPeil(string src)
        {
            if (!src.EndsWith("-"))
            { return src; }
            char chrTemp=getLastSozuq(src);
            if (chrTemp == '0')
            { return src; }

            if(isTilAldi(chrTemp))
            {
                return src.Remove(src.Length - 1, 1) + "مەك";
            }
            else if(isTilArqa(chrTemp))
            {
                return src.Remove(src.Length - 1, 1) + "ماق";
            }
            else
            {
            return src;
            }

        }

        private static char getLastSozuq(string src)
        {
            foreach(char chr in src.Reverse())
            {
                if (Sozuq.Contains(chr))
                {
                    return chr;
                }
            }
            return '0';
        }


        private static bool isTilAldi(char chr)
        {
            if(TilAldi.Contains(chr))
            {return true;}
            return false;
        }

        private static bool isTilArqa(char chr)
        {
            if (TilArqa.Contains(chr))
            { return true; }
            return false;
        }
    }
}
