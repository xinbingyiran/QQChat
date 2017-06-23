using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
{
    public class HEXMD5
    {
        private const int chrsz = 8;
        //private const int mode = 32;

        public static string Md5(string A)
        {
            return hex_md5(A);
        }
        public static string hex_md5(string A)
        {
            return binl2hex(core_md5(str2binl(A), A.Length * chrsz));
        }
        public static string str_md5(string A)
        {
            return binl2str(core_md5(str2binl(A), A.Length * chrsz));
        }
        public static Int32[] core_md5(EIArray<int> x, int len)
        {
		    x[len >> 5] |= 128 << ((len) % 32);
		    x[(((len + 64) >> 9) << 4) + 14] = len;
            var J = 1732584193;
            var I = -271733879;
            var H = -1732584194;
            var G = 271733878;
            for (var C = 0; C < x.Length; C += 16)
            {
                var E = J;
                var D = I;
                var B = H;
                var A = G;
                J = md5_ff(J, I, H, G, x[C + 0], 7, -680876936);
                G = md5_ff(G, J, I, H, x[C + 1], 12, -389564586);
                H = md5_ff(H, G, J, I, x[C + 2], 17, 606105819);
                I = md5_ff(I, H, G, J, x[C + 3], 22, -1044525330);
                J = md5_ff(J, I, H, G, x[C + 4], 7, -176418897);
                G = md5_ff(G, J, I, H, x[C + 5], 12, 1200080426);
                H = md5_ff(H, G, J, I, x[C + 6], 17, -1473231341);
                I = md5_ff(I, H, G, J, x[C + 7], 22, -45705983);
                J = md5_ff(J, I, H, G, x[C + 8], 7, 1770035416);
                G = md5_ff(G, J, I, H, x[C + 9], 12, -1958414417);
                H = md5_ff(H, G, J, I, x[C + 10], 17, -42063);
                I = md5_ff(I, H, G, J, x[C + 11], 22, -1990404162);
                J = md5_ff(J, I, H, G, x[C + 12], 7, 1804603682);
                G = md5_ff(G, J, I, H, x[C + 13], 12, -40341101);
                H = md5_ff(H, G, J, I, x[C + 14], 17, -1502002290);
                I = md5_ff(I, H, G, J, x[C + 15], 22, 1236535329);
                J = md5_gg(J, I, H, G, x[C + 1], 5, -165796510);
                G = md5_gg(G, J, I, H, x[C + 6], 9, -1069501632);
                H = md5_gg(H, G, J, I, x[C + 11], 14, 643717713);
                I = md5_gg(I, H, G, J, x[C + 0], 20, -373897302);
                J = md5_gg(J, I, H, G, x[C + 5], 5, -701558691);
                G = md5_gg(G, J, I, H, x[C + 10], 9, 38016083);
                H = md5_gg(H, G, J, I, x[C + 15], 14, -660478335);
                I = md5_gg(I, H, G, J, x[C + 4], 20, -405537848);
                J = md5_gg(J, I, H, G, x[C + 9], 5, 568446438);
                G = md5_gg(G, J, I, H, x[C + 14], 9, -1019803690);
                H = md5_gg(H, G, J, I, x[C + 3], 14, -187363961);
                I = md5_gg(I, H, G, J, x[C + 8], 20, 1163531501);
                J = md5_gg(J, I, H, G, x[C + 13], 5, -1444681467);
                G = md5_gg(G, J, I, H, x[C + 2], 9, -51403784);
                H = md5_gg(H, G, J, I, x[C + 7], 14, 1735328473);
                I = md5_gg(I, H, G, J, x[C + 12], 20, -1926607734);
                J = md5_hh(J, I, H, G, x[C + 5], 4, -378558);
                G = md5_hh(G, J, I, H, x[C + 8], 11, -2022574463);
                H = md5_hh(H, G, J, I, x[C + 11], 16, 1839030562);
                I = md5_hh(I, H, G, J, x[C + 14], 23, -35309556);
                J = md5_hh(J, I, H, G, x[C + 1], 4, -1530992060);
                G = md5_hh(G, J, I, H, x[C + 4], 11, 1272893353);
                H = md5_hh(H, G, J, I, x[C + 7], 16, -155497632);
                I = md5_hh(I, H, G, J, x[C + 10], 23, -1094730640);
                J = md5_hh(J, I, H, G, x[C + 13], 4, 681279174);
                G = md5_hh(G, J, I, H, x[C + 0], 11, -358537222);
                H = md5_hh(H, G, J, I, x[C + 3], 16, -722521979);
                I = md5_hh(I, H, G, J, x[C + 6], 23, 76029189);
                J = md5_hh(J, I, H, G, x[C + 9], 4, -640364487);
                G = md5_hh(G, J, I, H, x[C + 12], 11, -421815835);
                H = md5_hh(H, G, J, I, x[C + 15], 16, 530742520);
                I = md5_hh(I, H, G, J, x[C + 2], 23, -995338651);
                J = md5_ii(J, I, H, G, x[C + 0], 6, -198630844);
                G = md5_ii(G, J, I, H, x[C + 7], 10, 1126891415);
                H = md5_ii(H, G, J, I, x[C + 14], 15, -1416354905);
                I = md5_ii(I, H, G, J, x[C + 5], 21, -57434055);
                J = md5_ii(J, I, H, G, x[C + 12], 6, 1700485571);
                G = md5_ii(G, J, I, H, x[C + 3], 10, -1894986606);
                H = md5_ii(H, G, J, I, x[C + 10], 15, -1051523);
                I = md5_ii(I, H, G, J, x[C + 1], 21, -2054922799);
                J = md5_ii(J, I, H, G, x[C + 8], 6, 1873313359);
                G = md5_ii(G, J, I, H, x[C + 15], 10, -30611744);
                H = md5_ii(H, G, J, I, x[C + 6], 15, -1560198380);
                I = md5_ii(I, H, G, J, x[C + 13], 21, 1309151649);
                J = md5_ii(J, I, H, G, x[C + 4], 6, -145523070);
                G = md5_ii(G, J, I, H, x[C + 11], 10, -1120210379);
                H = md5_ii(H, G, J, I, x[C + 2], 15, 718787259);
                I = md5_ii(I, H, G, J, x[C + 9], 21, -343485551);
                J = safe_add(J, E);
                I = safe_add(I, D);
                H = safe_add(H, B);
                G = safe_add(G, A);
            }
            //if (mode == 16)
            //{
            //    return new Int32[]{ I, H};
            //}
            //else
            //{
                return new Int32[] { J, I, H, G };
            //}
        }
        public static int md5_cmn(int F, int C, int B, int A, int E, int D)
        {
            return safe_add(bit_rol(safe_add(safe_add(C, F), safe_add(A, D)), E), B);
        }
        public static int md5_ff(int C, int B, int G, int F, int A, int E, int D)
        {
            return md5_cmn((B & G) | ((~B) & F), C, B, A, E, D);
        }
        public static int md5_gg(int C, int B, int G, int F, int A, int E, int D)
        {
            return md5_cmn((B & F) | (G & (~F)), C, B, A, E, D);
        }
        public static int md5_hh(int C, int B, int G, int F, int A, int E, int D)
        {
            return md5_cmn(B ^ G ^ F, C, B, A, E, D);
        }
        public static int md5_ii(int C, int B, int G, int F, int A, int E, int D)
        {
            return md5_cmn(G ^ (B | (~F)), C, B, A, E, D);
        }
        public static int safe_add(int A, int D)
        {
            var C = (A & 65535) + (D & 65535);
            var B = (A >> 16) + (D >> 16) + (C >> 16);
            return (B << 16) | (C & 65535);
        }
        public static int bit_rol(int A, int B)
        {
            return (Int32)((uint)A << B | (uint)A >> (32 - B));
        }
        public static EIArray<int> str2binl(string D)
        {
            var A = (1 << chrsz) - 1;
            var bytes = Chars2Bytes(D.ToCharArray());
            var C = new EIArray<int>();
            for (var B = 0; B < bytes.Length * chrsz; B += chrsz)
            {
                C[B >> 5] |= (bytes[B / chrsz] & A) << (B % 32);
            }
            return C;
        }
        public static string binl2str(Int32[] C)
        {
            var D = "";
            var A = (1 << chrsz) - 1;
            for (var B = 0; B < C.Length * 32; B += chrsz)
            {
                D += String.Format("{0}", (((uint)(C[B / 32]) >> (B % 32)) & A));
            }
            return D;
        }
        public static string binl2hex(Int32[] C)
        {
            var D = "";
            for (var A = 0; A < C.Length; A++)
            {
                for (int i = 0; i < 32; i += 8)
                {
                    D += String.Format("{0:X02}", (C[A] >> i) & 0xff);
                }
            }
            return D;
        }

        public static string hexchar2bin(string str)
        {
            var arr = new List<byte>();
            for (var i = 0; i < str.Length; i = i + 2)
            {
                arr.Add(Convert.ToByte(str.Substring(i, 2), 16));
            }
            return new String(Bytes2Chars(arr.ToArray()));
        }

        public static char[] Bytes2Chars(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return chars;
        }

        public static byte[] Chars2Bytes(char[] chars)
        {
            byte[] bytes = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = (byte)chars[i];
            }
            return bytes;
        }
        public static int[] Bytes2Ints(byte[] bytes)
        {
            int[] ints = new int[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                ints[i] = bytes[i];
            }
            return ints;
        }
    }
}
