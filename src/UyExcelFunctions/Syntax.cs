using System ;
using System.Text ;
using System.Collections;
using System.Text.RegularExpressions;

namespace UyExcelFunctions
{
    public class Syntax
    {
        public enum Begtype
        {
            WDBEG = 0,
            INBEG,
            NOBEG
        }

        public readonly bool changeQuote = false; //there different requirements for handling quotes, use it as flag

        public static char BPAD = '\u0600'; // Basic region
        public static char BMAX = '\u06FF';
        public static char EPAD = '\uFB00'; // presentation form region (extented region)
        public static char EMAX = '\uFEFF';
        public static char CPAD = '\u0400'; // Cyrillic region
        public static char CMAX = '\u04FF';

        private static char CHEE = '\u0686';
        private static char GHEE = '\u063A';
        private static char NGEE = '\u06AD';
        private static char SHEE = '\u0634';
        private static char SZEE = '\u0698';
        private static char LA = '\uFEFB';
        private static char _LA = '\uFEFC';
        private static char HAMZA = '\u0626';
        public static char BQUOTE = '\u00AB';  // beginning quote in Uyghur ( >> )
        public static char EQUOTE = '\u00BB';  // ending quote in Uyghur ( << )
        public static char RCQUOTE = '\u2019'; // 0x2019 is right closed curly quote
        public static char RCODQUOTE = '\u201C'; // 0x2019 is right closed opening double curly quote
        public static char RCCDQUOTE = '\u201D'; // 0x2019 is right closed closing double curly quote
        public static char GLOBEDIT_EHE = '\u0647';

        public static string AllahsNameLong = "\u0627\u0644\u0644\u0647";
        public static string AllahsNameShort = "\u0627\uFDF2";

        // character map for latin based Uyghur writings and its inverse table
        char[] cmap = new char[256];
        char[] cmapinv = new char[256];

        // character map for Cyrillic based Uyghur scripts and its inverse table (mapped to ULY)
        char[] cyrmap = new char[256];
        char[] cyrmapinv = new char[256];

        public char begdelim = '`';  // beginning delimiter
        public char enddelim = '`';  // ending delimiter
        public char deepdelim = '\u2026';

        // using a hashtable Presentation form to Basic region mapping
        // private Hashtable pf2basic;
        char[,] pf2basic = new char[EMAX - EPAD, 2];

        // key_map holds the mapping between key codes and 
        // Uyghur characters in basic Arabic range
        char[] key_map = new char[BMAX - BPAD + 1];
        // rev_key_map holds the mapping between Uyghur characters and key codes
        char[] rev_key_map = new char[BMAX - BPAD + 1];

        // for pasting from Al-Katip text
        char[] uyghur_keys = new char[] { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', '/', 'D', 'F', 'G', 'H', 'J', 'K', '?' };

        int[] ak_keymap = new int[] { 45, 36, 38, 49, 42, 74, 48, 43, 72, 53, 71, 51, 47, 39, 41, 57, 66, 67, 68, 50, 52, 58, 73, 40, 70, 69, 56, 34, 65, 54, 46, 44, 35, 31 };
        char[] akmap = new char[256];

        int[] dd_keymap = new int[] { 134, 36, 53, 49, 42, 74, 48, 43, 72, 126, 71, 51, 47, 39, 41, 38, 66, 67, 68, 50, 52, 58, 37, 40, 70, 69, 56, 152, 65, 175, 46, 44, 35, 31 };
        char[] ddmap = new char[256];

        // character mapping for IlikYurt. It uses ASCII range to display Uyghur characters
        char[,] ilik_map = new char[128, 2];

        // character mapping for Uyghur Notebook. It uses ASCII range to display Uyghur characters
        char[,] unote_map = new char[128, 2];

        Ligatures[] pform = new Ligatures[256];

        public Syntax()
        {
            initialize();
        }

        // loads beginning, medial, and ending forms
        private void initialize()
        {
            int i;
            char ch;

            for (i = 0; i < cmap.Length; i++)
            {
                cmap[i] = '\0';
            }

            for (i = 0; i < cmapinv.Length; i++)
            {
                cmapinv[i] = '\0';
            }

            cmap['A'] = '\u0627';
            cmap['a'] = '\u0627';
            cmap['B'] = '\u0628';
            cmap['b'] = '\u0628';
            cmap['C'] = '\u0643';
            cmap['c'] = '\u0643';
            cmap['D'] = '\u062F';
            cmap['d'] = '\u062F';
            cmap['E'] = '\u06D5';
            cmap['e'] = '\u06D5';
            cmap['F'] = '\u0641';
            cmap['f'] = '\u0641';
            cmap['G'] = '\u06AF';
            cmap['g'] = '\u06AF';
            cmap['H'] = '\u06BE';
            cmap['h'] = '\u06BE';
            cmap['I'] = '\u0649';
            cmap['i'] = '\u0649';
            cmap['J'] = '\u062C';
            cmap['j'] = '\u062C';
            cmap['K'] = '\u0643';
            cmap['k'] = '\u0643';
            cmap['L'] = '\u0644';
            cmap['l'] = '\u0644';
            cmap['M'] = '\u0645';
            cmap['m'] = '\u0645';
            cmap['N'] = '\u0646';
            cmap['n'] = '\u0646';
            cmap['O'] = '\u0648';
            cmap['o'] = '\u0648';
            cmap['P'] = '\u067E';
            cmap['p'] = '\u067E';
            cmap['Q'] = '\u0642';
            cmap['q'] = '\u0642';
            cmap['R'] = '\u0631';
            cmap['r'] = '\u0631';
            cmap['S'] = '\u0633';
            cmap['s'] = '\u0633';
            cmap['T'] = '\u062A';
            cmap['t'] = '\u062A';
            cmap['U'] = '\u06C7';
            cmap['u'] = '\u06C7';
            cmap['V'] = '\u06CB';
            cmap['v'] = '\u06CB';
            cmap['W'] = '\u06CB';
            cmap['w'] = '\u06CB';
            cmap['X'] = '\u062E';
            cmap['x'] = '\u062E';
            cmap['Y'] = '\u064A';
            cmap['y'] = '\u064A';
            cmap['Z'] = '\u0632';
            cmap['z'] = '\u0632';

            cmap['É'] = '\u06D0';
            cmap['é'] = '\u06D0';
            cmap['Ö'] = '\u06C6';
            cmap['ö'] = '\u06C6';
            cmap['Ü'] = '\u06C8';
            cmap['ü'] = '\u06C8';

            // Uyghur punctuation marks
            cmap[';'] = '\u061B';
            cmap['?'] = '\u061F';
            cmap[','] = '\u060C';

            // the inverse of cmap table, to speed up lookups (without wasting much space)
            // we use BPAD for index operations, we would be wasting BPAD many bytes.
            // We could have used a hash table instead, but didn't think it is worthwhile.
            for (i = 0; i < cmapinv.Length; i++)
            {
                ch = cmap[i];
                if (ch != 0)
                {
                    cmapinv[ch - BPAD] = (char)i;
                }
            }

            // For Cyrillic. This maps between ULY and Cyrillic.
            for (i = 0; i < cyrmap.Length; i++)
            {
                cyrmap[i] = '\0';
            }

            for (i = 0; i < cyrmapinv.Length; i++)
            {
                cyrmapinv[i] = '\0';
            }

            cyrmap['А' - CPAD] = cmap['a'];
            cyrmap['а' - CPAD] = cmap['a'];
            cyrmap['Б' - CPAD] = cmap['b'];
            cyrmap['б' - CPAD] = cmap['b'];
            cyrmap['Д' - CPAD] = cmap['d'];
            cyrmap['д' - CPAD] = cmap['d'];
            cyrmap['Ә' - CPAD] = cmap['e'];
            cyrmap['ә' - CPAD] = cmap['e'];
            cyrmap['Ф' - CPAD] = cmap['f'];
            cyrmap['ф' - CPAD] = cmap['f'];
            cyrmap['Г' - CPAD] = cmap['g'];
            cyrmap['г' - CPAD] = cmap['g'];
            cyrmap['Һ' - CPAD] = cmap['h'];
            cyrmap['һ' - CPAD] = cmap['h'];
            cyrmap['И' - CPAD] = cmap['i'];
            cyrmap['и' - CPAD] = cmap['i'];
            cyrmap['Җ' - CPAD] = cmap['j'];
            cyrmap['җ' - CPAD] = cmap['j'];
            cyrmap['К' - CPAD] = cmap['k'];
            cyrmap['к' - CPAD] = cmap['k'];
            cyrmap['Л' - CPAD] = cmap['l'];
            cyrmap['л' - CPAD] = cmap['l'];
            cyrmap['М' - CPAD] = cmap['m'];
            cyrmap['м' - CPAD] = cmap['m'];
            cyrmap['Н' - CPAD] = cmap['n'];
            cyrmap['н' - CPAD] = cmap['n'];
            cyrmap['О' - CPAD] = cmap['o'];
            cyrmap['о' - CPAD] = cmap['o'];
            cyrmap['П' - CPAD] = cmap['p'];
            cyrmap['п' - CPAD] = cmap['p'];
            cyrmap['Қ' - CPAD] = cmap['q'];
            cyrmap['қ' - CPAD] = cmap['q'];
            cyrmap['Р' - CPAD] = cmap['r'];
            cyrmap['р' - CPAD] = cmap['r'];
            cyrmap['С' - CPAD] = cmap['s'];
            cyrmap['с' - CPAD] = cmap['s'];
            cyrmap['Т' - CPAD] = cmap['t'];
            cyrmap['т' - CPAD] = cmap['t'];
            cyrmap['У' - CPAD] = cmap['u'];
            cyrmap['у' - CPAD] = cmap['u'];
            cyrmap['В' - CPAD] = cmap['v'];
            cyrmap['в' - CPAD] = cmap['v'];
            cyrmap['Х' - CPAD] = cmap['x'];
            cyrmap['х' - CPAD] = cmap['x'];
            cyrmap['Й' - CPAD] = cmap['y'];
            cyrmap['й' - CPAD] = cmap['y'];
            cyrmap['З' - CPAD] = cmap['z'];
            cyrmap['з' - CPAD] = cmap['z'];
            cyrmap['е' - CPAD] = cmap['é'];
            cyrmap['Е' - CPAD] = cmap['é'];
            cyrmap['Ө' - CPAD] = cmap['ö'];
            cyrmap['ө' - CPAD] = cmap['ö'];
            cyrmap['Ү' - CPAD] = cmap['ü'];
            cyrmap['ү' - CPAD] = cmap['ü'];
            cyrmap['Ж' - CPAD] = SZEE;
            cyrmap['ж' - CPAD] = SZEE;
            cyrmap['Ғ' - CPAD] = GHEE;
            cyrmap['ғ' - CPAD] = GHEE;
            cyrmap['Ң' - CPAD] = NGEE;
            cyrmap['ң' - CPAD] = NGEE;
            cyrmap['Ч' - CPAD] = CHEE;
            cyrmap['ч' - CPAD] = CHEE;
            cyrmap['Ш' - CPAD] = SHEE;
            cyrmap['ш' - CPAD] = SHEE;

            // the inverse of cyrmap table, to speed up lookups (without wasting much space)
            // we use CPAD for index operations, we would be wasting CPAD many bytes.
            // We could have used a hash table instead, but didn't think it is worthwhile.
            for (i = 0; i < cyrmapinv.Length; i++)
            {
                ch = cyrmap[i];
                if (ch != 0)
                {
                    cyrmapinv[ch - BPAD] = (char)i;
                }
            }

            for (i = 0; i < pform.Length; i++)
            {
                pform[i] = null;
            }

            pform[cmap['a'] - BPAD] = new Ligatures('\uFE8D', '\uFE8D', '\uFE8D', '\uFE8E', Begtype.WDBEG);
            pform[cmap['e'] - BPAD] = new Ligatures('\uFEE9', '\uFEE9', '\uFEE9', '\uFEEA', Begtype.WDBEG);
            pform[cmap['b'] - BPAD] = new Ligatures('\uFE8F', '\uFE91', '\uFE92', '\uFE90', Begtype.NOBEG);
            pform[cmap['p'] - BPAD] = new Ligatures('\uFB56', '\uFB58', '\uFB59', '\uFB57', Begtype.NOBEG);
            pform[cmap['t'] - BPAD] = new Ligatures('\uFE95', '\uFE97', '\uFE98', '\uFE96', Begtype.NOBEG);
            pform[cmap['j'] - BPAD] = new Ligatures('\uFE9D', '\uFE9F', '\uFEA0', '\uFE9E', Begtype.NOBEG);
            pform[CHEE - BPAD] = new Ligatures('\uFB7A', '\uFB7C', '\uFB7D', '\uFB7B', Begtype.NOBEG);
            pform[cmap['x'] - BPAD] = new Ligatures('\uFEA5', '\uFEA7', '\uFEA8', '\uFEA6', Begtype.NOBEG);
            pform[cmap['d'] - BPAD] = new Ligatures('\uFEA9', '\uFEA9', '\uFEAA', '\uFEAA', Begtype.INBEG);
            pform[cmap['r'] - BPAD] = new Ligatures('\uFEAD', '\uFEAD', '\uFEAE', '\uFEAE', Begtype.INBEG);
            pform[cmap['z'] - BPAD] = new Ligatures('\uFEAF', '\uFEAF', '\uFEB0', '\uFEB0', Begtype.INBEG);
            pform[SZEE - BPAD] = new Ligatures('\uFB8A', '\uFB8A', '\uFB8B', '\uFB8B', Begtype.INBEG);
            pform[cmap['s'] - BPAD] = new Ligatures('\uFEB1', '\uFEB3', '\uFEB4', '\uFEB2', Begtype.NOBEG);
            pform[SHEE - BPAD] = new Ligatures('\uFEB5', '\uFEB7', '\uFEB8', '\uFEB6', Begtype.NOBEG);
            pform[GHEE - BPAD] = new Ligatures('\uFECD', '\uFECF', '\uFED0', '\uFECE', Begtype.NOBEG);
            pform[cmap['f'] - BPAD] = new Ligatures('\uFED1', '\uFED3', '\uFED4', '\uFED2', Begtype.NOBEG);
            pform[cmap['q'] - BPAD] = new Ligatures('\uFED5', '\uFED7', '\uFED8', '\uFED6', Begtype.NOBEG);
            pform[cmap['k'] - BPAD] = new Ligatures('\uFED9', '\uFEDB', '\uFEDC', '\uFEDA', Begtype.NOBEG);
            pform[cmap['g'] - BPAD] = new Ligatures('\uFB92', '\uFB94', '\uFB95', '\uFB93', Begtype.NOBEG);
            pform[NGEE - BPAD] = new Ligatures('\uFBD3', '\uFBD5', '\uFBD6', '\uFBD4', Begtype.NOBEG);
            pform[cmap['l'] - BPAD] = new Ligatures('\uFEDD', '\uFEDF', '\uFEE0', '\uFEDE', Begtype.NOBEG);
            pform[cmap['m'] - BPAD] = new Ligatures('\uFEE1', '\uFEE3', '\uFEE4', '\uFEE2', Begtype.NOBEG);
            pform[cmap['n'] - BPAD] = new Ligatures('\uFEE5', '\uFEE7', '\uFEE8', '\uFEE6', Begtype.NOBEG);
            pform[cmap['h'] - BPAD] = new Ligatures('\uFEEB', '\uFEEB', '\uFEEC', '\uFEEC', Begtype.NOBEG);
            //pform[ cmap['h'] - BPAD ]    = new Ligatures ( '\uFBAA', '\uFBAA, '\uFBAD, '\uFBAD, Begtype.NOBEG ) ;
            pform[cmap['o'] - BPAD] = new Ligatures('\uFEED', '\uFEED', '\uFEEE', '\uFEEE', Begtype.INBEG);
            pform[cmap['u'] - BPAD] = new Ligatures('\uFBD7', '\uFBD7', '\uFBD8', '\uFBD8', Begtype.INBEG);
            pform[cmap['ö'] - BPAD] = new Ligatures('\uFBD9', '\uFBD9', '\uFBDA', '\uFBDA', Begtype.INBEG);
            pform[cmap['ü'] - BPAD] = new Ligatures('\uFBDB', '\uFBDB', '\uFBDC', '\uFBDC', Begtype.INBEG);
            pform[cmap['w'] - BPAD] = new Ligatures('\uFBDE', '\uFBDE', '\uFBDF', '\uFBDF', Begtype.INBEG);
            pform[cmap['é'] - BPAD] = new Ligatures('\uFBE4', '\uFBE6', '\uFBE7', '\uFBE5', Begtype.NOBEG);
            pform[cmap['i'] - BPAD] = new Ligatures('\uFEEF', '\uFBE8', '\uFBE9', '\uFEF0', Begtype.NOBEG);
            pform[cmap['y'] - BPAD] = new Ligatures('\uFEF1', '\uFEF3', '\uFEF4', '\uFEF2', Begtype.NOBEG);
            pform[HAMZA - BPAD] = new Ligatures('\uFE8B', '\uFE8B', '\uFE8C', '\uFB8C', Begtype.NOBEG);

            for (i = 0; i < EMAX - EPAD; i++)
            {
                pf2basic[i, 0] = '\0';
                pf2basic[i, 1] = '\0';
            }

            // initialize presentation form to basic region mapping
            for (i = 0; i < pform.Length; i++)
            {
                if (pform[i] != null)
                {
                    Ligatures lig = pform[i];
                    pf2basic[(int)lig.iform - EPAD, 0] = (char)(i + BPAD);
                    pf2basic[(int)lig.bform - EPAD, 0] = (char)(i + BPAD);
                    pf2basic[(int)lig.mform - EPAD, 0] = (char)(i + BPAD);
                    pf2basic[(int)lig.eform - EPAD, 0] = (char)(i + BPAD);
                }
            }

            // the letter 'h' has some other mappings
            pf2basic['\uFBAA' - EPAD, 0] = cmap['h'];
            pf2basic['\uFBAB' - EPAD, 0] = cmap['h'];
            pf2basic['\uFBAC' - EPAD, 0] = cmap['h'];
            pf2basic['\uFBAD' - EPAD, 0] = cmap['h'];

            // joint letter LA and _LA
            pf2basic['\uFEFB' - EPAD, 0] = cmap['l'];
            pf2basic['\uFEFB' - EPAD, 1] = cmap['a'];
            pf2basic['\uFEFC' - EPAD, 0] = cmap['l'];
            pf2basic['\uFEFC' - EPAD, 1] = cmap['a'];

            // joint letter AA, AE, EE, II, OO, OE, UU, UE
            // AA, _AA
            pf2basic['\uFBEA' - EPAD, 0] = HAMZA;
            pf2basic['\uFBEA' - EPAD, 1] = cmap['a'];
            pf2basic['\uFBEB' - EPAD, 0] = HAMZA;
            pf2basic['\uFBEB' - EPAD, 1] = cmap['a'];

            // AE, _AE
            pf2basic['\uFBEC' - EPAD, 0] = HAMZA;
            pf2basic['\uFBEC' - EPAD, 1] = cmap['e'];
            pf2basic['\uFBED' - EPAD, 0] = HAMZA;
            pf2basic['\uFBED' - EPAD, 1] = cmap['e'];

            // EE, _EE, _EE_
            pf2basic['\uFBF6' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF6' - EPAD, 1] = cmap['é'];
            pf2basic['\uFBF7' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF7' - EPAD, 1] = cmap['é'];
            pf2basic['\uFBF8' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF8' - EPAD, 1] = cmap['é'];
            pf2basic['\uFBD1' - EPAD, 0] = HAMZA;
            pf2basic['\uFBD1' - EPAD, 1] = cmap['é'];

            // II, _II, _II_
            pf2basic['\uFBF9' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF9' - EPAD, 1] = cmap['i'];
            pf2basic['\uFBFA' - EPAD, 0] = HAMZA;
            pf2basic['\uFBFA' - EPAD, 1] = cmap['i'];
            pf2basic['\uFBFB' - EPAD, 0] = HAMZA;
            pf2basic['\uFBFB' - EPAD, 1] = cmap['i'];

            // OO, _OO
            pf2basic['\uFBEE' - EPAD, 0] = HAMZA;
            pf2basic['\uFBEE' - EPAD, 1] = cmap['o'];
            pf2basic['\uFBEF' - EPAD, 0] = HAMZA;
            pf2basic['\uFBEF' - EPAD, 1] = cmap['o'];

            // OE, _OE
            pf2basic['\uFBF2' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF2' - EPAD, 1] = cmap['ö'];
            pf2basic['\uFBF3' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF3' - EPAD, 1] = cmap['ö'];

            // UU, _UU
            pf2basic['\uFBF0' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF0' - EPAD, 1] = cmap['u'];
            pf2basic['\uFBF1' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF1' - EPAD, 1] = cmap['u'];

            // UE, _UE
            pf2basic['\uFBF4' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF4' - EPAD, 1] = cmap['ü'];
            pf2basic['\uFBF5' - EPAD, 0] = HAMZA;
            pf2basic['\uFBF5' - EPAD, 1] = cmap['ü'];


            for (i = 0; i < key_map.Length; i++)
            {
                key_map[i] = '\0';
            }

            key_map['A'] = '\u06BE';
            //key_map['A'] = '\u0640'; // space filler character
            key_map['a'] = '\u06BE';
            key_map['B'] = '\u0628';
            key_map['b'] = '\u0628';
            key_map['C'] = '\u063a';
            key_map['c'] = '\u063a';
            key_map['D'] = '\u0698';
            key_map['d'] = '\u062f';
            key_map['E'] = '\u06d0';
            key_map['e'] = '\u06d0';
            key_map['f'] = '\u0627';
            key_map['F'] = '\u0641';
            key_map['g'] = '\u06d5';
            key_map['G'] = '\u06af';
            key_map['h'] = '\u0649';
            key_map['H'] = '\u062e';
            key_map['I'] = '\u06ad';
            key_map['i'] = '\u06ad';
            key_map['j'] = '\u0642';
            key_map['J'] = '\u062c';
            key_map['K'] = '\u06c6';
            key_map['k'] = '\u0643';
            key_map['L'] = '\u0644';
            key_map['l'] = '\u0644';
            key_map['M'] = '\u0645';
            key_map['m'] = '\u0645';
            key_map['N'] = '\u0646';
            key_map['n'] = '\u0646';
            key_map['O'] = '\u0648';
            key_map['o'] = '\u0648';
            key_map['P'] = '\u067e';
            key_map['p'] = '\u067e';
            key_map['Q'] = '\u0686';
            key_map['q'] = '\u0686';
            key_map['R'] = '\u0631';
            key_map['r'] = '\u0631';
            key_map['S'] = '\u0633';
            key_map['s'] = '\u0633';
            //key_map ['T'] = '\u062a' ;
            key_map['T'] = '\u0640'; // space filler character
            key_map['t'] = '\u062a';
            key_map['U'] = '\u06c7';
            key_map['u'] = '\u06c7';
            key_map['V'] = '\u06c8';
            key_map['v'] = '\u06c8';
            key_map['W'] = '\u06cb';
            key_map['w'] = '\u06cb';
            key_map['X'] = '\u0634';
            key_map['x'] = '\u0634';
            key_map['Y'] = '\u064a';
            key_map['y'] = '\u064a';
            key_map['Z'] = '\u0632';
            key_map['z'] = '\u0632';
            key_map[','] = '\u060c';
            key_map['.'] = '\u002e';
            key_map['/'] = '\u0626';
            key_map['?'] = '\u061F';
            key_map[';'] = '\u061B';

            for (i = 0; i < rev_key_map.Length; i++)
            {
                rev_key_map[i] = '\0';
            }

            for (i = 0; i < key_map.Length; i++)
            {
                if (key_map[i] != '\0' && key_map[i] >= BPAD && key_map[i] < BMAX)
                {
                    rev_key_map[key_map[i] - BPAD] = (char)i;
                }
            }

            // zero-out all entries first for Al-Katip
            for (i = 0; i < akmap.Length; i++)
            {
                akmap[i] = '\0';
            }

            for (i = 0; i < uyghur_keys.Length; i++)
            {
                ch = uyghur_keys[i];
                int code = ak_keymap[i];

                akmap[code] = key_map[ch];
            }

            // zero-out all entries first for DulDul
            for (i = 0; i < ddmap.Length; i++)
            {
                ddmap[i] = '\0';
            }

            for (i = 0; i < uyghur_keys.Length; i++)
            {
                ch = uyghur_keys[i];
                int code = dd_keymap[i];

                ddmap[code] = key_map[ch];
            }


            // initialize IlikYurt conversion table
            for (i = 0; i < 128; i++)
            {
                ilik_map[i, 0] = '\0';
                ilik_map[i, 1] = '\0';
            }

            ilik_map[(int)'"', 0] = HAMZA;
            ilik_map[(int)'#', 0] = HAMZA;
            ilik_map[(int)'$', 0] = cmap['a'];
            ilik_map[(int)'$', 1] = cmap['l'];
            ilik_map[(int)'%', 0] = cmap['a'];
            ilik_map[(int)'%', 1] = cmap['l'];
            ilik_map[(int)'&', 0] = cmap['a'];
            ilik_map[(int)'\'', 0] = cmap['a'];
            ilik_map[(int)'(', 0] = cmap['e'];
            ilik_map[(int)')', 0] = cmap['e'];
            ilik_map[(int)'*', 0] = cmap['b'];
            ilik_map[(int)'+', 0] = cmap['b'];
            ilik_map[(int)',', 0] = cmap[','];
            ilik_map[(int)'-', 0] = cmap['b'];
            ilik_map[(int)'.', 0] = '\u0640';
            ilik_map[(int)'/', 0] = cmap['p'];
            ilik_map[(int)'0', 0] = cmap['p'];
            ilik_map[(int)'1', 0] = cmap['p'];
            ilik_map[(int)'2', 0] = cmap['t'];
            ilik_map[(int)'3', 0] = cmap['t'];
            ilik_map[(int)'4', 0] = cmap['t'];
            ilik_map[(int)'5', 0] = cmap['j'];
            ilik_map[(int)'6', 0] = cmap['j'];
            ilik_map[(int)'7', 0] = cmap['j'];
            ilik_map[(int)'8', 0] = cmap['j'];
            ilik_map[(int)'9', 0] = CHEE;
            ilik_map[(int)':', 0] = CHEE;
            ilik_map[(int)';', 0] = cmap[';'];
            ilik_map[(int)'<', 0] = CHEE;
            ilik_map[(int)'=', 0] = CHEE;
            ilik_map[(int)'>', 0] = cmap['x'];
            ilik_map[(int)'?', 0] = cmap['?'];
            ilik_map[(int)'@', 0] = cmap['x'];
            ilik_map[(int)'A', 0] = cmap['x'];
            ilik_map[(int)'B', 0] = cmap['x'];
            ilik_map[(int)'C', 0] = cmap['d'];
            ilik_map[(int)'D', 0] = cmap['r'];
            ilik_map[(int)'E', 0] = cmap['z'];
            ilik_map[(int)'F', 0] = SZEE;
            ilik_map[(int)'G', 0] = cmap['s'];
            ilik_map[(int)'H', 0] = cmap['s'];
            ilik_map[(int)'I', 0] = cmap['s'];
            ilik_map[(int)'J', 0] = cmap['s'];
            ilik_map[(int)'K', 0] = SHEE;
            ilik_map[(int)'L', 0] = SHEE;
            ilik_map[(int)'M', 0] = SHEE;
            ilik_map[(int)'N', 0] = SHEE;
            ilik_map[(int)'O', 0] = GHEE;
            ilik_map[(int)'P', 0] = GHEE;
            ilik_map[(int)'Q', 0] = GHEE;
            ilik_map[(int)'R', 0] = GHEE;
            ilik_map[(int)'S', 0] = cmap['f'];
            ilik_map[(int)'T', 0] = cmap['f'];
            ilik_map[(int)'U', 0] = cmap['f'];
            ilik_map[(int)'V', 0] = cmap['q'];
            ilik_map[(int)'W', 0] = cmap['q'];
            ilik_map[(int)'X', 0] = cmap['q'];
            ilik_map[(int)'Y', 0] = cmap['g'];
            ilik_map[(int)'Z', 0] = cmap['g'];
            ilik_map[(int)'[', 0] = cmap['b'];
            ilik_map[(int)'\\', 0] = cmap['k'];
            ilik_map[(int)']', 0] = cmap['k'];
            ilik_map[(int)'^', 0] = cmap['p'];
            ilik_map[(int)'_', 0] = NGEE;
            ilik_map[(int)'`', 0] = NGEE;
            ilik_map[(int)'a', 0] = cmap['t'];
            ilik_map[(int)'b', 0] = cmap['l'];
            ilik_map[(int)'c', 0] = cmap['l'];
            ilik_map[(int)'d', 0] = cmap['l'];
            ilik_map[(int)'e', 0] = cmap['m'];
            ilik_map[(int)'f', 0] = cmap['m'];
            ilik_map[(int)'g', 0] = cmap['m'];
            ilik_map[(int)'h', 0] = cmap['m'];
            ilik_map[(int)'i', 0] = cmap['n'];
            ilik_map[(int)'j', 0] = cmap['n'];
            ilik_map[(int)'k', 0] = cmap['n'];
            ilik_map[(int)'l', 0] = cmap['h'];
            ilik_map[(int)'m', 0] = cmap['h'];
            ilik_map[(int)'n', 0] = cmap['o'];
            ilik_map[(int)'o', 0] = cmap['u'];
            ilik_map[(int)'p', 0] = cmap['ö'];
            ilik_map[(int)'q', 0] = cmap['ü'];
            ilik_map[(int)'r', 0] = cmap['w'];
            ilik_map[(int)'s', 0] = cmap['é'];
            ilik_map[(int)'t', 0] = cmap['é'];
            ilik_map[(int)'u', 0] = cmap['é'];
            ilik_map[(int)'v', 0] = cmap['é'];
            ilik_map[(int)'w', 0] = cmap['i'];
            ilik_map[(int)'x', 0] = cmap['i'];
            ilik_map[(int)'y', 0] = cmap['i'];
            ilik_map[(int)'z', 0] = cmap['i'];
            ilik_map[(int)'{', 0] = cmap['y'];
            ilik_map[(int)'|', 0] = cmap['y'];
            ilik_map[(int)'}', 0] = cmap['y'];
            ilik_map[(int)'~', 0] = cmap['y'];

            // initialize Uyghur Notebook conversion table
            for (i = 0; i < 128; i++)
            {
                unote_map[i, 0] = '\0';
                unote_map[i, 1] = '\0';
            }

            unote_map[(int)'#', 0] = cmap['e'];
            unote_map[(int)'$', 0] = cmap['y'];
            unote_map[(int)'&', 0] = SHEE;
            unote_map[(int)',', 0] = cmap[','];
            unote_map[(int)'/', 0] = HAMZA;
            unote_map[(int)';', 0] = cmap[';'];
            unote_map[(int)'<', 0] = cmap['e'];
            unote_map[(int)'=', 0] = cmap['é'];
            unote_map[(int)'>', 0] = cmap['y'];
            unote_map[(int)'?', 0] = cmap['?'];
            unote_map[(int)'@', 0] = cmap['l'];
            unote_map[(int)'A', 0] = cmap['a'];
            unote_map[(int)'B', 0] = cmap['b'];
            unote_map[(int)'C', 0] = GHEE;
            unote_map[(int)'D', 0] = SZEE;
            unote_map[(int)'E', 0] = cmap['é'];
            unote_map[(int)'F', 0] = cmap['f'];
            unote_map[(int)'G', 0] = cmap['g'];
            unote_map[(int)'H', 0] = cmap['x'];
            unote_map[(int)'I', 0] = NGEE;
            unote_map[(int)'J', 0] = cmap['j'];
            unote_map[(int)'K', 0] = cmap['ö'];
            unote_map[(int)'L', 0] = cmap['a'];
            unote_map[(int)'L', 1] = cmap['l'];
            unote_map[(int)'M', 0] = cmap['m'];
            unote_map[(int)'N', 0] = cmap['n'];
            unote_map[(int)'O', 0] = cmap['q'];
            unote_map[(int)'P', 0] = cmap['p'];
            unote_map[(int)'Q', 0] = cmap['k'];
            unote_map[(int)'R', 0] = cmap['f'];
            unote_map[(int)'S', 0] = cmap['f'];
            unote_map[(int)'T', 0] = cmap['t'];
            unote_map[(int)'U', 0] = cmap['q'];
            unote_map[(int)'V', 0] = cmap['h'];
            unote_map[(int)'W', 0] = HAMZA;
            unote_map[(int)'X', 0] = GHEE;
            unote_map[(int)'Y', 0] = cmap['y'];
            unote_map[(int)'Z', 0] = cmap['j'];
            unote_map[(int)'[', 0] = cmap['j'];
            unote_map[(int)'\\', 0] = CHEE;
            unote_map[(int)']', 0] = CHEE;
            unote_map[(int)'^', 0] = cmap['x'];
            unote_map[(int)'_', 0] = cmap['x'];
            unote_map[(int)'`', 0] = GHEE;
            unote_map[(int)'a', 0] = cmap['h'];
            unote_map[(int)'b', 0] = cmap['b'];
            unote_map[(int)'c', 0] = GHEE;
            unote_map[(int)'d', 0] = cmap['d'];
            unote_map[(int)'e', 0] = cmap['é'];
            unote_map[(int)'f', 0] = cmap['a'];
            unote_map[(int)'g', 0] = cmap['e'];
            unote_map[(int)'h', 0] = cmap['i'];
            unote_map[(int)'i', 0] = NGEE;
            unote_map[(int)'j', 0] = cmap['q'];
            unote_map[(int)'k', 0] = cmap['k'];
            unote_map[(int)'l', 0] = cmap['l'];
            unote_map[(int)'m', 0] = cmap['m'];
            unote_map[(int)'n', 0] = cmap['n'];
            unote_map[(int)'o', 0] = cmap['o'];
            unote_map[(int)'p', 0] = cmap['p'];
            unote_map[(int)'q', 0] = CHEE;
            unote_map[(int)'r', 0] = cmap['r'];
            unote_map[(int)'s', 0] = cmap['s'];
            unote_map[(int)'t', 0] = cmap['t'];
            unote_map[(int)'u', 0] = cmap['u'];
            unote_map[(int)'v', 0] = cmap['ü'];
            unote_map[(int)'w', 0] = cmap['w'];
            unote_map[(int)'x', 0] = SHEE;
            unote_map[(int)'y', 0] = cmap['y'];
            unote_map[(int)'z', 0] = cmap['z'];
            unote_map[(int)'{', 0] = cmap['s'];
            unote_map[(int)'|', 0] = cmap['i'];
            unote_map[(int)'}', 0] = cmap['i'];
            unote_map[(int)'~', 0] = SHEE;
        }

        public string getReverseUyString(string asciiString, bool useBasicRange)
        {
            string reverseUyString = "";
            string uyString = getUyString(asciiString, useBasicRange);

            for (int i = 0; i < uyString.Length; i++)
            {
                reverseUyString = uyString[i] + reverseUyString;
            }

            return reverseUyString;
        }

        public string getUyStrFromPF(string pfstr)
        {
            char ch;
            int j;

            if (pfstr == null || pfstr.Length == 0)
            {
                return "";
            }

            char[] t = new char[pfstr.Length * 2];

            j = 0;
            for (int i = 0; i < pfstr.Length; i++)
            {
                ch = pfstr[i];

                if (ch >= EPAD && ch < EMAX && pf2basic[(int)ch - EPAD, 0] != '\0')
                {
                    t[j++] = pf2basic[(int)ch - EPAD, 0];

                    if (pf2basic[(int)ch - EPAD, 1] != '\0')
                    {
                        t[j++] = pf2basic[(int)ch - EPAD, 1];
                    }
                }
                else
                {
                    t[j++] = pfstr[i];
                }
            }

            return new string(t, 0, j);
        }

        public  string getUyStrFromUKY(string ukyString, bool useBasicRange)
        {
            string p = ukyString;

            if (p == null || p.Length == 0)
            {
                return "";
            }

            if (useBasicRange)
            {
                // make URL addresses that begin with http(s), ftp,... verbatim
                string pat = @"(\w+[p|s]:\/\/\S*)";
                p = Regex.Replace(p, pat, deepdelim + "$1" + deepdelim);

                // make URL addresses that do not start with http(s) verbatim
                pat = @"([\s|(]+\w+\.\w+\.\w+\S*)";
                p = Regex.Replace(p, pat, deepdelim + "$1" + deepdelim);

                // make two-part URL addresses (e.g. ukij.org) verbatim only if closed by space or parens 
                pat = @"([^\.|\/|\w])(\w+\.com)";
                p = Regex.Replace(p, pat, "$1" + deepdelim + "$2" + deepdelim);
                pat = @"([^\.|\/|\w])(\w+\.net)";
                p = Regex.Replace(p, pat, "$1" + deepdelim + "$2" + deepdelim);
                pat = @"([^\.|\/|\w])(\w+\.org)";
                p = Regex.Replace(p, pat, "$1" + deepdelim + "$2" + deepdelim);
                pat = @"([^\.|\/|\w])(\w+\.cn)";
                p = Regex.Replace(p, pat, "$1" + deepdelim + "$2" + deepdelim);

                // make email addresses verbatim
                pat = @"(\w+@\w+\.\w[\w|\.]*\w)";
                p = Regex.Replace(p, pat, deepdelim + "$1" + deepdelim);
            }

            string uyString = getUyString(p, useBasicRange);
            return uyString;
        }

        public string getUyStrFromAK(string str)
        {
            string akstr = str.Replace(AllahsNameLong, AllahsNameShort);

            char[] tstr = new char[akstr.Length];

            for (int i = 0; i < akstr.Length; i++)
            {
                int code = akstr[i];

                if (code < BPAD || code >= BPAD + akmap.Length)
                {
                    if (code == '{')
                    {
                        tstr[i] = BQUOTE; // >> sign
                    }
                    else if (code == '}')
                    {
                        tstr[i] = EQUOTE; // << sign
                    }
                    else
                    {
                        tstr[i] = akstr[i];
                    }
                    continue;
                }

                code = code - BPAD;

                if (code < akmap.Length && akmap[code] != '\0')
                {
                    tstr[i] = akmap[code];
                }
                else
                {
                    tstr[i] = akstr[i];
                }
            }

            return new string(tstr);
        }

        public string getUyStrFromDulDul(string str)
        {
            string ddstr = str.Replace(AllahsNameLong, AllahsNameShort);

            char[] tstr = new char[ddstr.Length];

            for (int i = 0; i < ddstr.Length; i++)
            {
                int code = ddstr[i];

                if (code < BPAD || code >= BPAD + ddmap.Length)
                {
                    if (code == '{')
                    {
                        tstr[i] = BQUOTE; // >> sign
                    }
                    else if (code == '}')
                    {
                        tstr[i] = EQUOTE; // << sign
                    }
                    else
                    {
                        tstr[i] = ddstr[i];
                    }
                    continue;
                }

                code = code - BPAD;

                if (code < ddmap.Length && ddmap[code] != '\0')
                {
                    tstr[i] = ddmap[code];
                }
                else
                {
                    tstr[i] = ddstr[i];
                }
            }

            return new string(tstr);
        }

        // convert IlikYurt 3.0 text to Unicode
        public string getUyStrFromIlik(string ilikstr)
        {
            int j;
            char[] tstr = new char[ilikstr.Length * 2];

            j = 0;
            for (int i = 0; i < ilikstr.Length; i++)
            {
                int code = ilikstr[i];

                if (code >= 0 && code < 128 && ilik_map[code, 0] != '\0')
                {
                    tstr[j++] = ilik_map[code, 0];

                    if (ilik_map[code, 1] != 0)
                    {
                        tstr[j++] = ilik_map[code, 1];
                    }
                }
                else
                {
                    tstr[j++] = (char)code;
                }
            }

            return new string(tstr, 0, j);
        }

        public string getUyStrFromGlobalEdit(string gestr)
        {
            int j;
            char[] tstr = new char[gestr.Length];

            j = 0;
            for (int i = 0; i < gestr.Length; i++)
            {
                int code = gestr[i];

                if (code == '\u0647') // only one letter differs in GlobalEdit
                {
                    tstr[j++] = '\u06BE';
                }
                else if (code == Syntax.RCODQUOTE || code == Syntax.RCCDQUOTE)
                {
                    tstr[j++] = '"';
                }
                else if (code == '(')
                {
                    tstr[j++] = ')';
                }
                else if (code == ')')
                {
                    tstr[j++] = '(';
                }
                else
                {
                    tstr[j++] = (char)code;
                }
            }

            return new string(tstr, 0, j);
        }

        // convert Uyghur Notepad text to Unicode
        public string getUyStrFromUnote(string unotestr)
        {
            int j;
            char[] tstr = new char[unotestr.Length * 2];

            j = 0;
            for (int i = 0; i < unotestr.Length; i++)
            {
                int code = unotestr[i];

                if (code >= 0 && code < 128 && unote_map[code, 0] != '\0')
                {
                    tstr[j++] = unote_map[code, 0];

                    if (unote_map[code, 1] != 0)
                    {
                        tstr[j++] = unote_map[code, 1];
                    }
                }
                else
                {
                    tstr[j++] = (char)code;
                }
            }

            return new string(tstr, 0, j);
        }

        // convert IlikYurt 3.0 text to IlikYurt Unicode and get reverse
        public string getRevUyStrFromIlik(string ilikstr)
        {
            char[] delims = new char[] { '\n' };
            string str = "";
            string[] lines = ilikstr.Split(delims);

            for (int j = 0; j < lines.Length; j++)
            {
                string line = getUyStrFromIlik(lines[j]);

                char[] tstr = new char[line.Length];

                for (int i = 0; i < line.Length; i++)
                {
                    tstr[line.Length - 1 - i] = line[i];
                }

                if (j == lines.Length - 1)
                {
                    str += new string(tstr);
                }
                else
                {
                    str += new string(tstr) + "\n";
                }
            }

            return str;
        }

        // convert Uyghur Notepad text to IlikYurt Unicode and get reverse
        public string getRevUyStrFromUnote(string unotestr)
        {
            char[] delims = new char[] { '\n' };
            string str = "";
            string[] lines = unotestr.Split(delims);

            for (int j = 0; j < lines.Length; j++)
            {
                string line = getUyStrFromUnote(lines[j]);

                char[] tstr = new char[line.Length];

                for (int i = 0; i < line.Length; i++)
                {
                    tstr[line.Length - 1 - i] = line[i];
                }

                if (j == lines.Length - 1)
                {
                    str += new string(tstr);
                }
                else
                {
                    str += new string(tstr) + "\n";
                }
            }

            return str;
        }

        // convert Uyghur Notepad text to Unicode
        public string getUyStrFromCyr(string cyrstr)
        {
            bool putHamza = true;
            bool openBrack = true;
            int j;
            char[] tstr = new char[cyrstr.Length * 2];
            char uch;

            j = 0;
            for (int i = 0; i < cyrstr.Length; i++)
            {
                int code = cyrstr[i];

                if (code >= CPAD && code < CMAX &&
                    (cyrmap[code - CPAD] != '\0' || code == 'Я' || code == 'я' || code == 'Ю' || code == 'ю'))
                {
                    if (code == 'Я' || code == 'я') // YA in Cyrillic
                    {
                        tstr[j++] = cmap['y'];
                        tstr[j++] = cmap['a'];
                        putHamza = true;
                    }
                    else if (code == 'Ю' || code == 'ю') // YU in Cyrillic
                    {
                        tstr[j++] = cmap['y'];
                        tstr[j++] = cmap['u'];
                    }
                    else
                    {
                        uch = cyrmap[code - CPAD];

                        if (isUyVowel(uch)) // decide if we should put hamza
                        {
                            if (putHamza)
                            {
                                tstr[j++] = HAMZA;
                            }
                            else
                            {
                                putHamza = true;
                            }
                        }
                        else
                        {
                            putHamza = false;
                        }

                        tstr[j++] = uch;
                    }
                }
                else // non-cyrillic letters
                {
                    if (code == ',')
                    {
                        tstr[j++] = cmap[','];
                    }
                    else if (code == '?')
                    {
                        tstr[j++] = cmap['?'];
                    }
                    else if (code == ';')
                    {
                        tstr[j++] = cmap[';'];
                    }
                    else if (code == '"' && changeQuote)
                    {
                        if (openBrack)
                        {
                            tstr[j++] = Syntax.BQUOTE;
                            openBrack = false;
                        }
                        else
                        {
                            tstr[j++] = Syntax.EQUOTE;
                            openBrack = true;
                        }
                    }
                    else if (code == Syntax.RCODQUOTE) // opening double curly quote
                    {
                        if (changeQuote)
                        {
                            tstr[j++] = Syntax.BQUOTE;
                        }
                        else
                        {
                            tstr[j++] = '"';
                        }
                    }
                    else if (code == Syntax.RCCDQUOTE) // closing double curly quote
                    {
                        if (changeQuote)
                        {
                            tstr[j++] = Syntax.EQUOTE;
                        }
                        else
                        {
                            tstr[j++] = '"';
                        }
                    }
                    else
                    {
                        tstr[j++] = (char)code;
                    }

                    // check to to see if we should put hamza before next letter
                    if (isInUyghurRange((char)code) == false || isvowel(code) == true)
                    {
                        putHamza = true;
                    }
                }
            }

            return new string(tstr, 0, j);
        }

        // Convert Uyghur Unicode text to text in Uyghur Latin alphabet
        public string getUKYFromUy(string uystr)
        {
            int j;
            char ch;
            char prev = '\0';
            char[] t = new char[uystr.Length * 2];

            j = 0;
            for (int i = 0; i < uystr.Length; i++)
            {
                ch = uystr[i];

                char next = '\0';

                if (i < uystr.Length - 1)
                {
                    next = uystr[i + 1];
                }

                if (ch == HAMZA)
                {
                    // no hamza necessary in UKY (ULY)
                    if (isInUyghurRange(prev) && !isUyVowel(prev) && isInUyghurRange(next))
                    {
                        t[j++] = '\'';
                    }

                    continue;
                }

                if (ch == GHEE)
                {
                    if (prev == cmap['n'])
                    {
                        t[j++] = '\'';
                    }

                    t[j++] = 'g';
                    t[j++] = 'h';
                }
                else if (ch == SZEE)
                {
                    t[j++] = 'j';
                }
                else if (ch == CHEE)
                {
                    t[j++] = 'c';
                    t[j++] = 'h';
                }
                else if (ch == SHEE)
                {
                    t[j++] = 's';
                    t[j++] = 'h';
                }
                else if (ch == NGEE)
                {
                    t[j++] = 'n';
                    t[j++] = 'g';
                }
                else if (BPAD <= ch && ch < (BPAD + cmapinv.Length) && cmapinv[(int)ch - BPAD] != '\0')
                {
                    // put a seperator between characters than can form a joint letter
                    if ((prev == cmap['n'] && ch == cmap['g']) ||
                        (prev == cmap['s'] && ch == cmap['h']))
                    {
                        t[j++] = '\'';
                    }

                    // the following two statements are not necessary. The initialization
                    // step for cmapinv points inverse index to lower case letters.
                    // string s = cmapinv[(int)ch-BPAD] + "";
                    // t[j++] = s.ToLower()[0];

                    t[j++] = cmapinv[(int)ch - BPAD];
                }
                else
                {
                    if (ch == cmap['?'])
                    {
                        t[j++] = '?';
                    }
                    else if (ch == cmap[','])
                    {
                        t[j++] = ',';
                    }
                    else if (ch == cmap[';'])
                    {
                        t[j++] = ';';
                    }
                    else if (ch == Syntax.BQUOTE || ch == Syntax.EQUOTE)
                    {
                        t[j++] = '"';
                    }
                    else
                    {
                        t[j++] = ch;
                    }
                }

                prev = ch;
            }

            string str = new string(t, 0, j);
            return str;
        }

        // Convert Uyghur Unicode text to text in Uyghur Latin alphabet
        public string getCyrFromUy(string uystr)
        {
            int j;
            char ch;
            char[] t = new char[uystr.Length];
            string hstr; // Uyghur string half replaced with Cyrillic YA and YU

            string ya = "" + cmap['y'] + cmap['a'];
            string yu = "" + cmap['y'] + cmap['u'];

            hstr = uystr.Replace(ya, "я");
            hstr = hstr.Replace(yu, "ю");

            j = 0;
            for (int i = 0; i < hstr.Length; i++)
            {
                ch = hstr[i];

                if (ch == HAMZA)
                {
                    // no hamza necessary in UKY (ULY)
                    continue;
                }

                if (BPAD <= ch && ch < (BPAD + cyrmapinv.Length) && cyrmapinv[(int)ch - BPAD] != '\0')
                {
                    // put a seperator between characters than can form a joint letter
                    t[j++] = (char)(CPAD + cyrmapinv[(int)ch - BPAD]);
                }
                else
                {
                    if (ch == cmap['?'])
                    {
                        t[j++] = '?';
                    }
                    else if (ch == cmap[','])
                    {
                        t[j++] = ',';
                    }
                    else if (ch == cmap[';'])
                    {
                        t[j++] = ';';
                    }
                    else if (ch == Syntax.BQUOTE || ch == Syntax.EQUOTE)
                    {
                        t[j++] = '"';
                    }
                    else
                    {
                        t[j++] = ch;
                    }
                }
            }

            string str = new string(t, 0, j);
            return str.ToLower();
        }

        public string getRevUyStrFromUKY(string ukyString, bool useBasicRange)
        {
            string uyString = getUyString(ukyString, useBasicRange);
            return getReverseUyString(uyString, useBasicRange);
        }

        public bool isInUyghurRange(char input)
        {
            if (input >= 0x0600 && input <= 0x06FF)
            {
                return true;
            }

            return false;
        }

        public bool isUyghurLetter(char input)
        {
            if (input >= 0x0600 && input <= 0x06FF)
            {
                char key = rev_key_map[input - BPAD];

                if (isalpha((int)key) || key == '/')
                {
                    return true;
                }
            }

            return false;
        }

        public int codeToKey(int uniChar)
        {
            int i = uniChar;

            if (uniChar >= BPAD && uniChar <= BMAX)
            {
                i = rev_key_map[uniChar - BPAD];
            }

            return i;
        }

        public int keyToCode(int keycode)
        {
            int i = keycode;

            if (keycode < key_map.Length && key_map[keycode] != 0)
            {
                i = key_map[keycode];
            }

            return i;
        }

        // convert ULY to Uyghur
        public string getUyString(string str, bool useBasicRange)
        {
            string p = str;

            if (p == null || p.Length == 0)
            {
                return "";
            }

            bool verbatim = false;
            bool dverbatim = false; // for text delimited by deepdelim (inserted by regex ops)

            bool wdbeg = true;
            bool openBrack = true;

            string uyString = "";
            char prev, cur, next, nnext, wch;
            int i, j;
            char[] wp = new char[p.Length * 2 + 1];

            j = 0;
            prev = '\0';
            for (i = 0; i < p.Length; i++)
            {
                cur = p[i];

                if (dverbatim == true)
                {
                    if (cur == deepdelim)
                    {
                        // ending verbatim mode
                        dverbatim = false;
                    }
                    else
                    {
                        wp[j++] = cur;
                    }
                    continue;
                }

                if (cur == deepdelim)
                {
                    dverbatim = true;
                    continue;
                }

                if (verbatim == true)
                {
                    if (cur == enddelim)
                    {
                        // ending verbatim mode
                        verbatim = false;
                    }
                    else
                    {
                        wp[j++] = cur;
                    }
                    continue;
                }

                if (cur == begdelim)
                {
                    verbatim = true;
                    continue;
                }

                if (i < p.Length - 1)
                {
                    next = p[i + 1];
                }
                else
                {
                    next = '\0';
                }

                if (i < p.Length - 2)
                {
                    nnext = p[i + 2];
                }
                else
                {
                    nnext = '\0';
                }

                wch = '\0';

                /* In some words that come from foreign languages, such as zhungxua, jiayuguan, etc.,
                 * we use medial forms of AA or AE. Compare this to Uyghur word sual, for example.
                 * By default, we use beginning forms of AA and AE in such cases, as in normal Uyghur.
                 * To force medial forms, put a '|' between vowels, e.g., "shinxu|a".
                 */
                if (cur == '|' && (prev == 'u' || prev == 'U') &&
                    (next == 'a' || next == 'A' || next == 'e' || next == 'E'))
                {
                    wdbeg = false;
                    continue;
                }

                // add hamza in front of vowels in word-beginning positions
                if (wdbeg == true)
                {
                    if (isvowel(cur))
                    {
                        wp[j++] = (char)HAMZA;
                    }
                    else if (cur == '\'' || cur == RCQUOTE)
                    {
                        // sometimes people add extra "'" in front of vowels, e.g. "sana'et".
                        // we just drop the tick or single quotes for such cases
                        if (isvowel(next))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (cur == '\'' || cur == RCQUOTE)
                    { // wdbeg == false means prev is a non-vowel letter
                        /* we try to force a hamza in certain occasions, e.g., compare 
                         * suret (picture) and sur'et (velocity). To minimize the effects
                         * of this substitution, we only do this if "'" is fllowed by a
                         * vowel and it is not in the word-beginning position. 
                         */
                        if (isvowel(next))
                        {
                            wdbeg = false; // don't add another hamza in next round
                            wp[j++] = HAMZA;
                            continue;
                        }
                        else if (isalpha(next))
                        {
                            /* Besides, we also want to separate two letters that form
                             * joint letter using "'". For example, to avoid the "ng" 
                             * from being treated as a joint letter NGEE in words
                             * yemenge, yigenge,...,  a "'" can be placed between them.
                             * For example, yemen'ge, yigen'ge,... .
                             */
                            continue;
                        }
                    }
                }

                // AA, AE, and non-alpha-numeric letters makes word beginning
                if (isvowel(cur) || !isalpha(cur))
                {
                    wdbeg = true;
                }
                else
                {
                    wdbeg = false;
                }

                switch (cur)
                {
                    case 'c':
                    case 'C':
                        if (next == 'h' || next == 'H')
                        {
                            wch = CHEE;
                        }
                        break;
                    case 'g':
                    case 'G':
                        if (next == 'h' || next == 'H')
                        {
                            wch = GHEE;
                        }
                        break;
                    case 'n':
                    case 'N':
                        if (next == 'g' || next == 'G')
                        {
                            /* for cases where we have a sequence of ngh, it could be
                                * translated as either NGEE + EHE or NEE + GHEE. However, the
                                * latter is much more common than the former in Uyghur language
                                * and we opt to translate it as NEE + GHEE. If there is a
                                * need to have NGEE + EHE, a single quote ("'") can be used.
                                */
                            if (nnext != 'h' && nnext != 'H')
                            {
                                wch = NGEE;
                            }
                        }
                        break;
                    case 's':
                    case 'S':
                        if (next == 'h' || next == 'H')
                        {
                            wch = SHEE;
                        }
                        else if (next == 'z' || next == 'Z')
                        {
                            // UKY does not provide a unique SZEE, we take joint 
                            // letters "sz" for SZEE, as in purszin [spring (coil)]
                            wch = SZEE;
                        }
                        break;
                    default:
                        break;
                }

                if (wch != 0)
                {
                    i++; // there is a joint letter, advance index
                    wp[j] = wch;
                }
                else if (cur < cmap.Length && cmap[cur] != '\0')
                {
                    wp[j] = cmap[cur]; // no joint letter, but valid UKY
                }
                else
                {
                    if (cur == '"' && changeQuote)
                    {
                        if (openBrack)
                        {
                            wp[j] = Syntax.BQUOTE;
                            openBrack = false;
                        }
                        else
                        {
                            wp[j] = Syntax.EQUOTE;
                            openBrack = true;
                        }
                    }
                    else if (cur == Syntax.RCODQUOTE) // opening double curly quote
                    {
                        if (changeQuote)
                        {
                            wp[j] = Syntax.BQUOTE;
                        }
                        else
                        {
                            wp[j] = '"';
                        }
                    }
                    else if (cur == Syntax.RCCDQUOTE) // closing double curly quote
                    {
                        if (changeQuote)
                        {
                            wp[j] = Syntax.EQUOTE;
                        }
                        else
                        {
                            wp[j] = '"';
                        }
                    }
                    else
                    {
                        wp[j] = cur; // non-UKY, return whatever is entered
                    }
                }

                prev = cur;
                j++;
            }

            uyString = new String(wp, 0, j);

            if (useBasicRange == true)
            {
                return uyString;
            }
            else
            {
                return getUyPFStr(uyString);
            }
        }

        public string getRevUyPFStr(string unistr)
        {
            string str = getUyPFStr(unistr);
            return getRevStr(str);
        }

        // returns the reverse of a string
        public string getRevStr(string str)
        {
            int len = str.Length;

            char[] arr = new char[len];

            for (int i = len - 1; i >= 0; i--)
            {
                arr[i] = str[(len - 1) - i];
            }

            return new string(arr);
        }

        // returns Uyghur string in presentation form range, input is expected to be in basic range
        public string getUyPFStr(string str)
        {
            Ligatures lsyn = pform[cmap['l'] - BPAD];
            Ligatures syn, tsyn;
            Begtype bt = Begtype.WDBEG;

            string wp = str;
            string pfstr = "";
            int n = str.Length;
            int i, j = 0;
            char wc;   // current char
            char pfwc = '\0'; // presentation form char
            char prevwc = '\0'; // previous char
            char ppfwc = '\0';  // previous presenation form char

            char[] pfwp = new char[n];

            for (i = 0; i < n; i++)
            {
                wc = wp[i];
                if (BPAD <= wc && wc < BMAX)
                {
                    syn = pform[wc - BPAD];

                    if (syn != null)
                    {
                        switch (bt)
                        {
                            case Begtype.WDBEG:
                                pfwc = syn.iform;
                                break;
                            case Begtype.INBEG:
                                pfwc = syn.iform;
                                break;
                            case Begtype.NOBEG:
                                pfwc = syn.eform;
                                break;
                            default:
                                break;
                        }

                        /* previous letter does not ask for word-beginning form,
                         * and we have to change it to either medial or beginning form,
                         * depending on the previous letter's current form.
                         */
                        //this means the previous letter was a joinable Uyghur letter
                        if (bt != Begtype.WDBEG)
                        {
                            tsyn = pform[prevwc - BPAD];

                            // special cases for LA and _LA
                            if (ppfwc == lsyn.iform && wc == cmap['a'])
                            {
                                pfwp[j - 1] = LA;
                                bt = Begtype.WDBEG;
                                continue;
                            }
                            else if (ppfwc == lsyn.eform && wc == cmap['a'])
                            {
                                pfwp[j - 1] = _LA;
                                bt = Begtype.WDBEG;
                                continue;
                            }

                            // update previous character
                            if (ppfwc == tsyn.iform)
                            {
                                pfwp[j - 1] = tsyn.bform;
                            }
                            else if (ppfwc == tsyn.eform)
                            {
                                pfwp[j - 1] = tsyn.mform;
                            }
                        }
                        bt = syn.btype; // we will need this in next round
                    }
                    else
                    { // a non-Uyghur char in basic range
                        pfwc = wc;
                        bt = Begtype.WDBEG;
                    }
                }
                else
                { // not in basic Arabic range ( 0x0600-0x06FF )
                    pfwc = wc;
                    bt = Begtype.WDBEG;
                }

                pfwp[j] = pfwc;
                ppfwc = pfwc; // previous presentation form wide character
                prevwc = wc;
                j++;
            }

            pfstr = new String(pfwp, 0, j);

            return pfstr;
        }

        public bool isalpha(int ch)
        {
            if (('A' <= ch && ch <= 'Z') || ('a' <= ch && ch <= 'z'))
            {
                return true;
            }

            return false;
        }

        public bool isvowel(int ch)
        {
            if (ch == 'a' || ch == 'A' || ch == 'e' || ch == 'E' ||
                ch == 'é' || ch == 'É' || ch == 'i' || ch == 'I' ||
                ch == 'o' || ch == 'O' || ch == 'Ö' || ch == 'ö' ||
                ch == 'u' || ch == 'U' || ch == 'ü' || ch == 'Ü')
            {
                return true;
            }

            return false;
        }

        public bool isUyVowel(int ch)
        {
            if (ch == cmap['a'] || ch == cmap['e'] || ch == cmap['i'] ||
                ch == cmap['é'] || ch == cmap['o'] || ch == cmap['ö'] ||
                ch == cmap['u'] || ch == cmap['ü'])
            {
                return true;
            }

            return false;
        }

        // isvowel_ks -- return true if key ch represents a vowel in Uyghur
        public bool isvowel_ks(int ch)
        {
            if (ch == 'f' || ch == 'g' || ch == 'h' || ch == 'e' ||
                ch == 'o' || ch == 'O' || ch == 'u' || ch == 'U' ||
                ch == 'K' || ch == 'v' || ch == 'V')
            {
                return true;
            }

            return false;
        }

        public static string getInfo()
        {
            return "Yulghun.com";
        }

        private class Ligatures
        {
            public char iform, bform, mform, eform;
            public Begtype btype;

            public Ligatures(char i, char b, char m, char e, Begtype bt)
            {
                this.iform = i;
                this.bform = b;
                this.mform = m;
                this.eform = e;
                this.btype = bt;
            }
        }
    }
}