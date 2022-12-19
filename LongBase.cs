using System;
using System.Collections.Generic;
using System.Text;

namespace SuperData.Maths
{
    class LongBase
    {

        #region 基础函数库

        /// <summary>
        /// 将数字转换成四位长度的十六进制字符串（用于表示长度）
        /// </summary>
        /// <param name="lValue">数字（表示长度）</param>
        /// <returns>四位十六进制字符串</returns>
        public static string Long2Hex4(long lValue)
        {
            return string.Format("{0:X4}", lValue);
            /*
            int[] nBlock = new int[4];
			for(int i = 0;i < 4;i ++)
			{
				nBlock[i] = (int)(lValue % 16);
				if(nBlock[i] > 9)
					nBlock[i] += 0x37;
				else
					nBlock[i] += 0x30;
				lValue /= 16;
			}
			return string.Format ("{3}{2}{1}{0}" , (char)nBlock[0] , (char)nBlock[1] , (char)nBlock[2] , (char)nBlock[3]);
            */
        }

        /// <summary>
        /// 将十六进制字符串转换成长整形数字
        /// </summary>
        /// <param name="strHex">十六进制字符串</param>
        /// <returns>长整形数字</returns>
        public static long Hex2Long(string strHex)
        {
            long lValue = 0;
            for (int i = 0; i < strHex.Length; i++)
            {
                lValue *= 0x10;
                int nBlock = (int)strHex[i] - 0x30;
                if (nBlock > 9)
                    lValue += nBlock - 7;
                else
                    lValue += nBlock;
            }
            return lValue;
        }
        #endregion

    }
}
