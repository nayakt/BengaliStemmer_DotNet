using System;
using System.Collections.Generic;
using System.Text;

namespace Stemmers
{
    public class BengaliStemmer
    {
        #region bengali unicodes

        private static char bn_chandrabindu = (char)0x0981;
        private static char bn_anusvara = (char)0x0982;
        private static char bn_visarga = (char)0x0983;
        private static char bn_a = (char)0x0985;
        private static char bn_aa = (char)0x0986;
        private static char bn_i = (char)0x0987;
        private static char bn_ii = (char)0x0988;
        private static char bn_u = (char)0x0989;
        private static char bn_uu = (char)0x098a;
        private static char bn_ri = (char)0x098b;
        private static char bn_e = (char)0x098f;
        private static char bn_ai = (char)0x0990;
        private static char bn_o = (char)0x0993;
        private static char bn_au = (char)0x0994;
        private static char bn_k = (char)0x0995;
        private static char bn_kh = (char)0x0996;
        private static char bn_g = (char)0x0997;
        private static char bn_gh = (char)0x0998;
        private static char bn_ng = (char)0x0999;
        private static char bn_ch = (char)0x099a;
        private static char bn_chh = (char)0x099b;
        private static char bn_j = (char)0x099c;
        private static char bn_jh = (char)0x099d;
        private static char bn_n = (char)0x099e;
        private static char bn_tt = (char)0x099f;
        private static char bn_tth = (char)0x09a0;
        private static char bn_dd = (char)0x09a1;
        private static char bn_ddh = (char)0x09a2;
        private static char bn_mn = (char)0x09a3;
        private static char bn_t = (char)0x09a4;
        private static char bn_th = (char)0x09a5;
        private static char bn_d = (char)0x09a6;
        private static char bn_dh = (char)0x09a7;
        private static char bn_dn = (char)0x09a8;
        private static char bn_p = (char)0x09aa;
        private static char bn_ph = (char)0x09ab;
        private static char bn_b = (char)0x09ac;
        private static char bn_bh = (char)0x09ad;
        private static char bn_m = (char)0x09ae;
        private static char bn_y = (char)0x09df;
        private static char bn_J = (char)0x09af;
        private static char bn_r = (char)0x09b0;
        private static char bn_l = (char)0x09b2;
        private static char bn_sh = (char)0x09b6;
        private static char bn_ss = (char)0x09b7;
        private static char bn_s = (char)0x09b8;
        private static char bn_h = (char)0x09b9;

        private static char bn_AA = (char)0x09be;
        private static char bn_I = (char)0x09bf;
        private static char bn_II = (char)0x09c0;
        private static char bn_U = (char)0x09c1;
        private static char bn_UU = (char)0x09c2;
        private static char bn_RI = (char)0x09c3;
        private static char bn_E = (char)0x09c7;
        private static char bn_AI = (char)0x09c8;
        private static char bn_O = (char)0x09cb;
        private static char bn_AU = (char)0x09cc;

        private static char bn_hosh = (char)0x09cd;
        private static char bn_nukta = (char)0x09bc;
        private static char bn_virama = (char)0x0964;
        private static char bn_khandata = (char)0x09ce;
        private static char bn_rr = (char)0x09dc;
        private static char bn_rrh = (char)0x09dd;

        private static char bn_zero = (char)0x09e6;
        private static char bn_one = (char)0x09e7;
        private static char bn_two = (char)0x09e8;
        private static char bn_three = (char)0x09e9;
        private static char bn_four = (char)0x09ea;
        private static char bn_five = (char)0x09eb;
        private static char bn_six = (char)0x09ec;
        private static char bn_seven = (char)0x09ed;
        private static char bn_eight = (char)0x09ee;
        private static char bn_nine = (char)0x09ef;

        #endregion

        private char[] swaraBarnas = { bn_AA, bn_E, bn_I, bn_II, bn_U, bn_UU };

        private bool IsBengaliSwaraBarna(char a)
        {
            for (int i = 0; i < swaraBarnas.Length; i++)
            {
                if (a == swaraBarnas[i])
                {
                    return true;
                }
            }
            return false;
        }

        /* A common suffix candidate is a sequence of vowels and bn_y. */
        private bool IsBnCommonSuffix(char a)
        {
            if (a >= bn_AA && a <= bn_AU)
                return true;
            if (a >= bn_aa && a <= bn_au)
                return true;
            return a == bn_y ? true : false;
        }

        private bool IsBengaliByanjanBarna(char a)
        {
            return a >= bn_k && a <= bn_y;
        }

        // Strip off suffixes "gulo", "guli", "gulote" "gulite"
        private void StripPluralSuffixes(ref string word)
        {
            int len = word.Length;
            if (word.Length <= 6)
            {
                return;
            }

            if (word[len - 1] == bn_E && word[len - 2] == bn_t)
            {
                word = word.Substring(0, len - 2);
                len = word.Length;
            }

            if (len <= 6)
            {
                return;
            }

            if (word[len - 4] == bn_g && word[len - 3] == bn_U && word[len - 2] == bn_l &&
                    (word[len - 1] == bn_O || word[len - 1] == bn_I))
            {
                word = word.Substring(0, len - 4);
            }
        }

        // Strip off suffixes like "rai", "tuku", "shil" "ta" etc.
        private bool StripCommonSuffixes(ref string word, bool i_removed)
        {
            int newlen = word.Length;
            int len = word.Length;
            do
            {
                if (len <= 4)
                {
                    break;
                }

                // Remove 'tta' or 'ta' (only if it is not preceeded with a m or g)
                if (word[len - 1] == bn_AA &&
                    (word[len - 2] == bn_tt || (word[len - 2] == bn_t &&
                     word[len - 3] != bn_m && word[len - 3] != bn_g)))
                {
                    word = word.Substring(0, len - 2);
                    len = word.Length;
                }

                if (len <= 4)
                {
                    break;
                }

                // Remove 'ti' or 'tti'
                if (word[len - 1] == bn_I &&
                    word[len - 2] == bn_tt)
                {
                    word = word.Substring(0, len - 2);
                    len = word.Length;
                }

                if (len <= 4)
                {
                    break;
                }

                // Remove "ra"  ("rai" has alreday been stemmed to "ra").
                if (word[len - 1] == bn_r)
                {
                    word = word.Substring(0, len - 1);
                    len = word.Length;

                    if (len <= 4)
                    {
                        break;
                    }

                    // Remove "-er"
                    if (word[len - 1] == bn_E)
                    {
                        int pos = word[len - 2] == bn_d ? 2 : 1; // Remove '-der'
                        word = word.Substring(0, len - pos);
                        len = word.Length;
                    }
                }

                if (len <= 5)
                {
                    break;
                }

                // Remove ttai tai ttar or tar
                if ((word[len - 1] == bn_y || word[len - 1] == bn_r) &&
                    word[len - 2] == bn_AA &&
                    (word[len - 3] == bn_tt || word[len - 3] == bn_t))
                {
                    word = word.Substring(0, len - 3);
                    len = word.Length;
                }
                else if ((word[len - 1] == bn_r) && word[len - 2] == bn_I && word[len - 3] == bn_tt)
                {
                    word = word.Substring(0, len - 3);
                    len = word.Length;
                }

                if (len <= 5)
                {
                    break;
                }

                if (word[len - 1] == bn_E && word[len - 2] == bn_k)
                {
                    word = word.Substring(0, len - 2);
                    len = word.Length;
                }

                if (len <= 5)
                    break;

                // Remove 'shil'
                if (word[len - 1] == bn_l && word[len - 2] == bn_II && word[len - 3] == bn_sh)
                {
                    word = word.Substring(0, len - 3);
                    len = word.Length;
                }

                if (len <= 6)
                {
                    break;
                }

                // Remove 'tuku'
                if (word[len - 1] == bn_U && word[len - 2] == bn_k && word[len - 3] == bn_U && word[len - 4] == bn_tt)
                {
                    word = word.Substring(0, len - 4);
                    len = word.Length;
                }

                // Remove 'debi'
                if (len <= 6)
                {
                    break;
                }

                if (word[len - 1] == bn_II && word[len - 2] == bn_b && word[len - 3] == bn_E && word[len - 4] == bn_d)
                {
                    word = word.Substring(0, len - 4);
                    len = word.Length;
                }

                // Remove 'babu'
                if (len <= 6)
                {
                    break;
                }

                if (word[len - 1] == bn_U && word[len - 2] == bn_b && word[len - 3] == bn_AA && word[len - 4] == bn_b)
                {
                    word = word.Substring(0, len - 4);
                    len = word.Length;
                }

                // Remove 'bhai'
                if (len <= 6 || !i_removed)
                {
                    break;
                }

                if (word[len - 1] == bn_AA && word[len - 2] == bn_bh)
                {
                    word = word.Substring(0, len - 2);
                    len = word.Length;
                }

                // Remove 'bhabe'
                if (len <= 6)
                {
                    break;
                }

                if (word[len - 1] == bn_b && word[len - 2] == bn_E && word[len - 3] == bn_AA && word[len - 4] == bn_bh)
                {
                    word = word.Substring(0, len - 4);
                    len = word.Length;
                }


            }
            while (false);

            return newlen != len;
        }

        private bool SuffixEndingByanjonBarna(char ch)
        {
            return (ch == bn_d || ch == bn_k || ch == bn_tt || ch == bn_t || ch == bn_m);
        }

        private string StemWord(string word, bool isAggressive)
        {
            int len = word.Length;
            int wordlen = len;
            bool i_removed = false;
            int p;

            string buff = string.Empty;
            for (int i = 0; i < len; i++)
            {
                buff += word[i];
            }

            if (!isAggressive)
            {
                if (len <= 3)
                {
                    return buff;
                }
            }

            // Remove trailing okhyor "i" and "o"
            if (buff[len - 1] == bn_i || buff[len - 1] == bn_o)
            {
                buff = buff.Substring(0, len - 1);
                len = len - 1;
                i_removed = true;
            }

            while (StripCommonSuffixes(ref buff, i_removed))
            {
                i_removed = false;
            }

            StripPluralSuffixes(ref buff);

            if (isAggressive)
            {
                p = buff.Length - 1;
                while (IsBnCommonSuffix(buff[p]))
                {
                    p--;
                }
                if (p - wordlen + 1 >= 2)
                {
                    buff = buff.Substring(0, p + 1);
                }
            }

            if (buff[buff.Length - 1] == bn_E)
            {
                buff = buff.Substring(0, buff.Length - 1);
            }

            return buff;
        }

        #region IStemmer Members

        public string Stem(string word)
        {
            return StemWord(word, isAggressive);
        }

        #endregion

        private bool isAggressive = false;

        public bool IsAggressive
        {
            get
            {
                return isAggressive;
            }
            set
            {
                isAggressive = value;
            }
        }
    }
}
