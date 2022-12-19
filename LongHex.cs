using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperData.Maths
{
    /// <summary>
    /// LongHex 是用来存储超长数据结构的数据类型
    /// 可以存储如10000!、100^1000等的非常规超大数据
    /// 允许从一般类型数据、字符串等直接进行数据类型转换获得原始数据
    /// 如：
    ///		LongHex liValue = 1000L;
    ///		LongHex liValue = "-100,101";
    ///		LongHex liValue = LongHex.Cmn(100 , 10)获得C100,10的结果
    /// </summary>
    public class LongHex
    {
        /// <summary>
        /// 进数为0x1000,由低位向高位存储
        /// 如，存储的为10,11，则值为11*0x1000+10=11,010
        /// </summary>
        public const int nBaseGene = 0x1000;

        /// <summary>
        /// 计算掩码，用位运算来提高性能
        /// </summary>
        public const int nBaseMask = 0xfff;

        /// <summary>
        /// 位移运算比乘法、除法更快，通过与掩码的配合提高运算性能
        /// </summary>
        public const int nBaseBitCount = 12;

        /// <summary>
        /// 值为0的常量
        /// </summary>
        public const int Zero = 0;

        /// <summary>
        /// 数据符号
        /// true为正
        /// false为否
        /// </summary>
        private bool bPositive = true;

        /// <summary>
        /// 数据存储集合,由低位到高位的存储格式
        /// 如，存储的为10,11，则值为11*1000+10=11010
        /// </summary>
        private List<long> list = new List<long>(0);

        #region public bool Positive
        /// <summary>
        /// 是否为正数，TRUE为正数，FALSE为负数
        /// </summary>
        public bool Positive
        {
            get
            {
                return bPositive;
            }
            set
            {
                bPositive = value;
            }
        }
        #endregion

        #region public List<long> List
        /// <summary>
        /// 超长整形的表示列表,从小大到,nBaseGene=0x1000进制,这样做不安全,但实现方便
        /// </summary>
        public List<long> List
        {
            get
            {
                return list;
            }
            set
            {
                if (value == null)
                    throw new System.InvalidOperationException("data is null.");
                else
                    list = value;
            }
        }
        #endregion

        #region public int Length
        /// <summary>
        /// 获得长整形的长度(占用空间)
        /// </summary>
        public int Length
        {
            get
            {
                return list.Count;
            }
        }
        #endregion

        #region public int HexLength
        /// <summary>
        /// 获得长整形数字的长度
        /// </summary>
        public int HexLength
        {
            get
            {
                int nCount = list.Count;
                if (nCount > 0)
                {
                    long nNum = list[nCount - 1];
                    int nLength = 0;
                    while (nNum > 0)
                    {
                        nLength++;
                        nNum /= 0x10;
                    }
                    return nLength + (nCount - 1) * 3;	//每一节占3个字节,最大不会超过0x1000
                }
                else
                    return 0;
            }
        }
        #endregion

        #region public LongHex()
        /// <summary>
        /// 构造函数,默认为0
        /// </summary>
        public LongHex()
        {
            bPositive = true;
            list.Add(LongHex.Zero);
        }
        #endregion

        #region public LongHex(long nValue)
        /// <summary>
        /// 构造函数LongHex(long nValue)
        /// </summary>
        /// <param name="nValue">长整形数</param>
        public LongHex(long nValue)
        {
            if (nValue < 0)
            {
                bPositive = false;
                nValue = -nValue;
            }
            else
                bPositive = true;
            do
            {
                list.Add(nValue & nBaseMask);
                nValue >>= nBaseBitCount;
            }
            while (nValue > nBaseGene);
            if (nValue > 0)
                list.Add(nValue);
        }
        #endregion

        #region public LongHex(LongHex liValue)
        /// <summary>
        /// 构造函数 LongHex(LongHex liValue)
        /// </summary>
        /// <param name="liValue">超长整形数据</param>
        public LongHex(LongHex liValue)
        {
            bPositive = liValue.bPositive;
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public LongHex(LongInt liValue)
        /// <summary>
        /// 构造函数 LongHex(LongInt liValue)
        /// </summary>
        /// <param name="liValue">超长十进制数据</param>
        public LongHex(LongInt liValue)
        {
            LongHex a = new LongHex();
            bPositive = liValue.Positive;
            if (liValue.Length > 0)
            {
                a = liValue.List[liValue.Length - 1];
            }
            for (int i = liValue.Length - 2; i >= 0; i--)
            {
                a *= LongInt.nBaseGene;
                a += liValue.List[i];
            }
            list = a.List;
        }
        #endregion

        #region public LongHex(String strValue)
        /// <summary>
        /// 导入十六进制数字字符串
        /// </summary>
        /// <param name="strValue">数字字符串</param>
        public LongHex(String strValue)
        {
            strValue = strValue.Replace(",", "");
            if (strValue.Length > 0)
            {
                bPositive = true;
                if (strValue.Substring(0, 1).Equals("-"))
                {
                    bPositive = false;
                    strValue = strValue.Replace("-", "");
                }
                int nLength = strValue.Length;
                if (nLength % 3 > 0)
                {
                    list.Add(LongBase.Hex2Long(strValue.Substring(0, nLength % 3)));
                }
                for (int i = nLength % 3; i < nLength; i += 3)
                {
                    list.Insert(0, LongBase.Hex2Long(strValue.Substring(i, 3)));
                }
            }
            else
            {
                bPositive = true;
            }
        }
        #endregion

        #region public static implicit operator LongHex(long nValue)
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="nValue">长整形数字</param>
        /// <returns>十六进制长整形数字</returns>
        public static implicit operator LongHex(long nValue)
        {
            LongHex liValue = new LongHex(nValue);
            return liValue;
        }
        #endregion

        #region public static implicit operator LongHex(string strValue)
        /// <summary>
        /// 类型转换,将字符串直接转换成超长整形,从高位向低位直接一位一位地读并转换,效率不太高
        /// </summary>
        /// <param name="strValue">数字字符串</param>
        /// <returns>十六进制长整形数字</returns>
        public static implicit operator LongHex(string strValue)
        {
            strValue = strValue.Replace(",", "");
            if (strValue.Length > 0)
            {
                bool bPositive = true;
                if (strValue.Substring(0, 1).Equals("-"))
                {
                    bPositive = false;
                    strValue = strValue.Replace("-", "");
                }
                //先计算高位数据，(不足8位部份)
                int nEndPos = strValue.Length % 8;
                long lValue = 0;
                for (int i = 0; i < nEndPos; i++)
                {
                    lValue *= 10L;
                    lValue += strValue[i] & 0x0f;
                }
                LongHex liValue = lValue;
                //后计算低位数据，(位数为8的整数倍)
                for (int i = nEndPos; i < strValue.Length; i += 8)
                {
                    lValue = 0;
                    for (int j = i; j < i + 8; j += 2)
                    {
                        lValue *= 100L;
                        lValue += (strValue[j] & 0x0f) * 10 + (strValue[j + 1] & 0x0f);
                    }
                    liValue *= 100000000L;
                    liValue += lValue;
                }
                liValue.Positive = bPositive;
                return liValue;
            }
            else
            {
                LongHex liValue = new LongHex(0L);
                return liValue;
            }
        }
        #endregion

        #region public static explicit operator long(LongHex liValue)
        /// <summary>
        /// 类型转换函数,由超长整形转换成长整形,如果太长则抛出异常.
        /// </summary>
        /// <param name="liValue">超长整形</param>
        /// <returns>长整形数</returns>
        public static explicit operator long(LongHex liValue)
        {
            List<long> array = liValue.List;
            int nCount = array.Count;
            if (nCount > 4 || nCount == 0)
                throw new InvalidOperationException("data is too larger.");
            long nResult = 0;
            for (int i = nCount - 1; i >= 0; i--)
            {
                nResult += array[i];
                if (i > 0)
                    nResult <<= nBaseBitCount;
            }
            if (liValue.Positive)
                return nResult;
            else
                return -nResult;
        }
        #endregion

        #region public static explicit operator String(LongHex liValue)
        /// <summary>
        /// 函数类型转换
        /// </summary>
        /// <param name="liValue">十六进制长整形数字</param>
        /// <returns>字符串</returns>
        public static explicit operator String(LongHex liValue)
        {
            return liValue.Export(true);
        }
        #endregion

        #region public void Equal(long nValue)
        /// <summary>
        /// 赋值函数Equal,相当于"="运算,可以直接用this=nValue代替
        /// </summary>
        /// <param name="nValue">长整形数</param>
        public void Equal(long nValue)
        {
            if (nValue < 0)
            {
                bPositive = false;
                nValue = -nValue;
            }
            else
                bPositive = true;
            list.Clear();
            do
            {
                list.Add(nValue & nBaseMask);
                nValue >>= nBaseBitCount;
            }
            while (nValue > nBaseGene);
            if (nValue > 0)
                list.Add(nValue);
        }
        #endregion

        #region public void Equal(LongHex liValue)
        /// <summary>
        /// 赋值Equal,相当于"="运算,可以直接用this=liValue代替
        /// </summary>
        /// <param name="liValue">超长整形</param>
        public void Equal(LongHex liValue)
        {
            bPositive = liValue.bPositive;
            list.Clear();
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public static LongHex operator >>(LongHex a , int nBit)
        /// <summary>
        /// 对超长整形右移,等价于=a/0x1000^nBit
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="nBit">0x1000进制的位</param>
        /// <returns>超长整形</returns>
        public static LongHex operator >>(LongHex a, int nBit)
        {
            int nLength = a.Length;
            LongHex liResult = new LongHex(0);
            liResult.Positive = a.Positive;
            List<long> list = new List<long>();
            if (nLength > nBit)
            {
                for (int i = (int)nBit; i < nLength; i++)
                    list.Add(a.List[i]);
                liResult.List = list;
            }

            return liResult;
        }
        #endregion

        #region public static LongHex operator <<(LongHex a , int nBit)
        /// <summary>
        /// 对超长整形左移,等价于=a*0x1000^nBit
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="nBit">0x1000进制的位</param>
        /// <returns>超长整形</returns>
        public static LongHex operator <<(LongHex a, int nBit)
        {
            if (a == 0)
                return a;
            LongHex liResult = new LongHex(a);
            List<long> list = new List<long>();
            for (int i = 0; i < nBit; i++)
                list.Add(0L);
            for (int i = 0; i < a.List.Count; i++)
                list.Add(a.List[i]);
            liResult.List = list;
            return liResult;
        }
        #endregion

        #region public static bool operator ==(LongHex a , LongHex b)
        /// <summary>
        /// 重载等于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>相等返回true</returns>
        public static bool operator ==(LongHex a, LongHex b)
        {
            int nLengtha = a.Length;
            int nLengthb = b.Length;
            if (nLengtha == nLengthb)
            {
                bool bZero = true;
                for (int i = 0; i < nLengtha; i++)
                {
                    if (a.List[i] != b.List[i])
                        return false;
                    else if (a.List[i] != 0)
                        bZero = false;
                }
                if (bZero)
                    return true;
                else
                    return (a.Positive == b.Positive);
            }
            else
                return false;
        }
        #endregion

        #region public static bool operator !=(LongHex a , LongIt b)
        /// <summary>
        /// 重载等于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>不等返回true</returns>
        public static bool operator !=(LongHex a, LongHex b)
        {
            return !(a == b);
        }
        #endregion

        #region public static bool operator >(LongHex a , LongHex b)
        /// <summary>
        /// 重载大于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>运算数a大返回true</returns>
        public static bool operator >(LongHex a, LongHex b)
        {
            if (a == 0 && b == 0)
                return false;
            if (a == 0)
                return !b.Positive;
            if (b == 0)
                return a.Positive;
            if (a.Positive && b.Positive || !a.Positive && !b.Positive)
            {
                int nLengtha = a.Length;
                int nLengthb = b.Length;
                if (nLengtha > nLengthb)
                    return a.Positive;
                else if (nLengtha < nLengthb)
                    return !a.Positive;
                else
                {
                    for (int i = nLengtha - 1; i >= 0; i--)
                    {
                        if (a.List[i] > b.List[i])
                            return a.Positive;
                        else if (a.List[i] < b.List[i])
                            return !a.Positive;
                    }
                    return false;
                }
            }
            else
                return a.Positive;
        }
        #endregion

        #region public static bool operator <(LongHex a , LongHex b)
        /// <summary>
        /// 重载小于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>运算数b大返回true</returns>
        public static bool operator <(LongHex a, LongHex b)
        {
            return (b > a);
        }
        #endregion

        #region public static bool operator >=(LongHex a , LongHex b)
        /// <summary>
        /// 重载大于等于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>运算数a较大返回true</returns>
        public static bool operator >=(LongHex a, LongHex b)
        {
            return !(b > a);
        }
        #endregion

        #region public static bool operator <=(LongHex a , LongHex b)
        /// <summary>
        /// 重载不大于符号
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>运算数b较大返回true</returns>
        public static bool operator <=(LongHex a, LongHex b)
        {
            return !(a > b);
        }
        #endregion

        #region public static LongHex operator -(LongHex a)
        /// <summary>
        /// 一元减,取反
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <returns>运算数取反</returns>
        public static LongHex operator -(LongHex a)
        {
            LongHex liResult = new LongHex(a);
            liResult.Positive = !a.Positive;
            return liResult;
        }
        #endregion

        #region public static LongHex operator +(LongHex a , LongHex b)
        /// <summary>
        /// 重载+
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回相加值</returns>
        public static LongHex operator +(LongHex a, LongHex b)
        {
            int nLengtha = a.Length;
            int nLengthb = b.Length;
            if (a.Positive && b.Positive || !a.Positive && !b.Positive)
            {
                //执行加法
                List<long> list = new List<long>(0);
                long nInc = 0;
                int i, j;
                for (i = 0; i < nLengtha; i++)
                {
                    long nNum = a.List[i];
                    nNum += nInc;
                    if (i < nLengthb)
                    {
                        nNum += b.List[i];
                    }
                    nInc = nNum >> nBaseBitCount;
                    nNum &= LongHex.nBaseMask;
                    list.Add(nNum);

                }
                for (j = i; j < nLengthb; j++)
                {
                    long nNum = b.List[j];
                    nNum += nInc;
                    nInc = nNum >> nBaseBitCount;
                    nNum &= LongHex.nBaseMask;
                    list.Add(nNum);
                }
                if (nInc > 0)
                    list.Add(nInc);
                LongHex result = new LongHex();
                result.Positive = a.Positive;
                result.List = list;
                return result;
            }
            else
            {
                //执行减法
                return a - (-b);
            }

        }
        #endregion

        #region public static LongHex operator -(LongHex a , LongHex b)
        /// <summary>
        /// 重载-
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回两数相减值</returns>
        public static LongHex operator -(LongHex a, LongHex b)
        {
            int nLengtha = a.Length;
            int nLengthb = b.Length;
            if (a.Positive && b.Positive || !a.Positive && !b.Positive)
            {
                if (a.Positive && a < b)
                    return -(b - a);
                else
                {
                    //定义结果
                    LongHex result = new LongHex();
                    result.Positive = a.Positive;

                    //执行减法
                    List<long> list = new List<long>(0);
                    long nInc = 0;
                    for (int i = 0; i < nLengtha; i++)
                    {
                        long nNum = a.List[i];
                        nNum += nInc;
                        if (i < nLengthb)
                        {
                            nNum -= b.List[i];
                        }
                        nInc = 0;
                        while (nNum < 0)
                        {
                            nNum += LongHex.nBaseGene;
                            nInc--;
                        }
                        nInc += nNum >> nBaseBitCount;
                        nNum &= LongHex.nBaseMask;
                        list.Add(nNum);
                    }
                    /*
                    for (int j = i; j < nLengthb; j++)
                    {
                        long nNum = nInc;
                        nNum -= b.List[j];
                        nInc = 0;
                        while (nNum < 0)
                        {
                            nNum += LongHex.nBaseGene;
                            nInc--;
                        }
                        nInc += nNum >> nBaseBitCount;
                        nNum &= LongHex.nBaseMask;
                        list.Add(nNum);
                    }

                    if (nInc > 0)
                        list.Add(nInc);
                    else if (nInc < 0)
                    {
                        //取否操作
                        nInc = System.Math.Abs(nInc) - 1;
                        if (nInc > 0)
                            list.Add(nInc);
                        //将该数向下传递
                        for (i = list.Count - 1; i >= 0; i--)
                        {
                            long nNum = LongHex.nBaseGene;
                            nNum -= list[i];
                            if (i > 0)
                                nNum--;
                            list[i] = nNum;
                        }
                        result.Positive = !result.Positive;
                    }
                    */

                    //判断高位是否为0
                    int nCount = list.Count;
                    int nPos = nCount - 1;
                    while (nPos > 0)
                    {
                        if (list[nPos] == 0)
                        {
                            list.RemoveAt(nPos);
                            nPos--;
                        }
                        else
                            break;
                    }
                    result.List = list;
                    return result;
                }
            }
            else
            {
                //执行加法
                return a + (-b);
            }
        }
        #endregion

        #region public static LongHex operator *(LongHex a , long b)
        /// <summary>
        /// 重载*
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回两数相乘结果</returns>
        public static LongHex operator *(LongHex a, long b)
        {
            LongHex liResult = new LongHex();
            if (b == 0)
            {
                return 0;
            }
            else if (b < 0)
            {
                liResult.Positive = !a.Positive;
                b = System.Math.Abs(b);
            }
            else
                liResult.Positive = a.Positive;

            long nResidual = 0;
            List<long> list = new List<long>();
            for (int i = 0; i < a.Length; i++)
            {
                long nNum = a.List[i] * b;
                nNum += nResidual;
                nResidual = nNum >> nBaseBitCount;
                list.Add(nNum & LongHex.nBaseMask);
            }
            while (nResidual > 0)
            {
                list.Add(nResidual & LongHex.nBaseMask);
                nResidual >>= nBaseBitCount;
            }
            liResult.List = list;
            return liResult;
        }
        #endregion

        #region public static LongHex operator *(LongHex a , LongHex b)
        /// <summary>
        /// 重载*
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回两数相乘的结果</returns>
        public static LongHex operator *(LongHex a, LongHex b)
        {
            LongHex liResult = new LongHex(0);
            List<long> listb = b.List;
            //从高位往低位算
            //listb.Reverse ();
            for (int i = listb.Count - 1; i >= 0; i--)
            {
                liResult += a * listb[i];
                if (i > 0)
                    liResult <<= 1;
            }
            //listb.Reverse ();
            liResult.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive;
            return liResult;
        }
        #endregion

        #region private static bool Div(LongHex a , long b , out LongHex lhDiv , out long lMod)
        /// <summary>
        /// 执行除法，返回除数与余数
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lMod">余</param>
        /// <returns>除零错返回FALSE，否则返回TRUE</returns>
        private static bool Div(LongHex a, long b, out LongHex lhDiv, out long lMod)
        {
            lhDiv = new LongHex();
            lMod = 0;
            if (b == 0)
            {
                return false;
            }
            else if (b < 0)
            {
                lhDiv.Positive = !a.Positive;
                b = -b;
            }
            else
            {
                lhDiv.Positive = a.Positive;
            }
            List<long> listResult = new List<long>();
            bool bFirst = true;
            bool bOrdered = true;
            if (bOrdered)
            {
                int nIndex = a.List.Count - 1;
                for (int i = 0; i < a.List.Count; i++)
                {

                    if (i > 0)
                        lMod <<= nBaseBitCount;
                    lMod += a.List[nIndex--];

                    if (lMod >= b || !bFirst)
                    {
                        long nDivide = lMod / b;
                        listResult.Add(nDivide);
                        bFirst = false;
                        if (nDivide > 0) lMod -= nDivide * b;
                    }
                }
            }
            else
            {
                for (int i = a.List.Count - 1; i >= 0; i--)
                {
                    lMod += a.List[i];
                    long nDivide = lMod / b;
                    if (nDivide > 0 || !bFirst)
                    {
                        listResult.Add(nDivide);
                        bFirst = false;
                        if (nDivide > 0) lMod %= b;
                    }
                    if (i > 0)
                    {
                        //最后一次不需要移位，计算结束
                        lMod <<= nBaseBitCount;
                    }
                }
            }
            if (bFirst)
                listResult.Add(0L);
            else
                listResult.Reverse();
            lhDiv.List = listResult;
            if (!a.Positive)
            {
                lMod = -lMod;
            }
            return true;
        }
        #endregion

        private static bool Div(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            return Div_N(a, b, out lhDiv, out lhMod);
        }

        private const int nFastLength = 4;
        public static bool bDebug = true;   //测试循环效率的开关（仅针对Div函数）
        public static int nLoopSum = 0;     //循环总次数
        public static int nLoopCount = 0;   //涉及循环的计算次数
        public static int nLoopNum = 0;     //当前计算的循环次数
        public static int nLoopNumMax = 0;  //最多的循环次数
        public static bool bLoopFlag = false;

        #region private static bool Div_N(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// 执行除法，返回除数与余数（保守循环）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lhMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_N(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                lhDiv = 0L;
                lhMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out lhDiv, out nMod))
                {
                    lhMod = nMod;
                    return true;
                }
                else
                {
                    lhMod = 0L;
                    return false;
                }
            }
            else
            {
                bool bDivPositive = a.Positive && b.Positive || !a.Positive && !b.Positive;
                bool bModPositive = a.Positive;
                a.Positive = true;
                b.Positive = true;
                int nLengthb = b.Length;
                LongHex liTempb2 = b >> (nLengthb - nFastLength);

                lhDiv = 0L;
                nLoopNum = 0;
                while (a >= b)
                {
                    LongHex liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;
                    LongHex lhTmpDiv;
                    //分子小，分母大，结果小，余数肯定为正
                    Div(liTempa2, (long)liTempb2 + 1, out lhTmpDiv, out nTmpMod);
                    //较正
                    if (lhTmpDiv == 0 && a >= b)
                    {
                        //当高位相似时，应直接赋于商=1
                        lhTmpDiv = 1L;
                    }
                    a -= b * lhTmpDiv;
                    if (a == b)
                    {
                        lhTmpDiv += 1L;
                        a = 0L;
                    }
                    lhDiv += lhTmpDiv;
                    nLoopNum++;
                }
                lhMod = a;
                if (bDebug && nLoopNum > 0)
                {
                    nLoopSum += nLoopNum;
                    nLoopCount++;
                    if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                }

                lhDiv.Positive = bDivPositive;
                lhMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_R(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// 执行除法，返回除数与余数（保守递归）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lhMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_R(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                lhDiv = 0L;
                lhMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out lhDiv, out nMod))
                {
                    lhMod = nMod;
                    return true;
                }
                else
                {
                    lhMod = 0L;
                    return false;
                }
            }
            else
            {
                bool bDivPositive = a.Positive && b.Positive || !a.Positive && !b.Positive;
                bool bModPositive = a.Positive;
                a.Positive = true;
                b.Positive = true;
                int nLengthb = b.Length;
                LongHex liTempb2 = b >> (nLengthb - nFastLength);
                LongHex liTempa2 = a >> (nLengthb - nFastLength);
                long nTmpMod;
                //分子小，分母大，结果小，余数肯定为正
                Div(liTempa2, (long)liTempb2 + 1, out lhDiv, out nTmpMod);
                //较正
                if (lhDiv == 0 && a >= b)
                {
                    //当高位相似时，应直接赋于商=1
                    lhDiv = 1;
                }
                lhMod = a - b * lhDiv;
                if (lhMod > 0)
                {
                    if (lhMod > b)
                    {
                        //递归计算剩余部份的商(比余数大的才递归)
                        LongHex lhDivFree;
                        Div_R(lhMod, b, out lhDivFree, out lhMod);
                        lhDiv += lhDivFree;
                    }
                    else if (lhMod == b)
                    {
                        lhDiv += 1L;
                        lhMod = 0L;
                    }
                }

                lhDiv.Positive = bDivPositive;
                lhMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_G(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// 执行除法，返回除数与余数（两值之间权衡）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lhMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_G(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                lhDiv = 0L;
                lhMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out lhDiv, out nMod))
                {
                    lhMod = nMod;
                    return true;
                }
                else
                {
                    lhMod = 0L;
                    return false;
                }
            }
            else
            {
                bool bDivPositive = a.Positive && b.Positive || !a.Positive && !b.Positive;
                bool bModPositive = a.Positive;
                a.Positive = true;
                b.Positive = true;
                int nLengthb = b.Length;
                LongHex liTempb2 = b >> (nLengthb - nFastLength);

                lhDiv = 0L;
                nLoopNum = 0;
                while (a >= b)
                {
                    LongHex liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;
                    LongHex lhTmpDiv;
                    
                    //分子小，分母小，结果不确定，余数可能会变否
                    Div(liTempa2, (long)liTempb2, out lhTmpDiv, out nTmpMod);
                    //较正
                    if (lhTmpDiv == 0)
                    {
                        //当高位相似时，应直接赋于商=1
                        lhTmpDiv = 1L;
                    }
                    LongHex lhTmpMod = a - b * lhTmpDiv;
                    if (lhTmpMod >= 0)
                    {
                        if (lhTmpMod == b)
                        {
                            lhTmpDiv += 1L;
                            a = 0L;
                        }
                        else if (lhTmpMod < b)
                        {
                            a = lhTmpMod;
                        }
                        lhDiv += lhTmpDiv;
                    }
                    else
                    {
                        //分子小，分母大，结果小，余数肯定为正
                        Div(liTempa2, (long)liTempb2 + 1, out lhTmpDiv, out nTmpMod);
                        //较正
                        if (lhTmpDiv == 0 && a >= b)
                        {
                            //当高位相似时，应直接赋于商=1
                            lhTmpDiv = 1L;
                        }
                        a -= b * lhTmpDiv;
                        if (a == b)
                        {
                            lhTmpDiv += 1L;
                            a = 0L;
                        }
                        lhDiv += lhTmpDiv;
                    }
                    nLoopNum++;
                }
                lhMod = a;
                if (bDebug && nLoopNum > 0)
                {
                    nLoopSum += nLoopNum;
                    nLoopCount++;
                    if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                }

                lhDiv.Positive = bDivPositive;
                lhMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_2R(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// 执行除法，返回除数与余数（两值之间权衡）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lhMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_2R(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                lhDiv = 0L;
                lhMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out lhDiv, out nMod))
                {
                    lhMod = nMod;
                    return true;
                }
                else
                {
                    lhMod = 0L;
                    return false;
                }
            }
            else
            {
                bool bDivPositive = a.Positive && b.Positive || !a.Positive && !b.Positive;
                bool bModPositive = a.Positive;
                a.Positive = true;
                b.Positive = true;
                int nLengthb = b.Length;
                LongHex liTempb2 = b >> (nLengthb - nFastLength);

                bool bLoopFlagCur = false;
                if (!bLoopFlag)
                {
                    bLoopFlag = true;
                    bLoopFlagCur = true;
                    nLoopNum = 0;
                }

                lhDiv = 0L;
                if (a < b)
                {
                    lhMod = a;
                }
                else if (a == b)
                {
                    lhDiv = 1;
                    lhMod = 0;
                }
                else
                {

                    LongHex liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;

                    //分子小，分母小，结果不确定，余数可能会变否
                    Div(liTempa2, (long)liTempb2, out lhDiv, out nTmpMod);
                    lhMod = a - b * lhDiv;
                    if (lhMod >= 0)
                    {
                        if (lhMod >= b)
                        {
                            lhDiv = lhDiv + 1;
                            lhMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongHex lhDivFree;
                            nLoopNum++;
                            Div_2R(lhMod, b, out lhDivFree, out lhMod);
                            lhDiv += lhDivFree;
                        }
                    }
                    else
                    {
                        //分子小，分母大，结果小，余数肯定为正
                        Div(liTempa2, (long)liTempb2 + 1, out lhDiv, out nTmpMod);
                        if (lhDiv == 0)
                        {
                            lhDiv = 1L;
                        }
                        lhMod = a - b * lhDiv;
                        if (lhMod >= b)
                        {
                            lhDiv = lhDiv + 1;
                            lhMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongHex lhDivFree;
                            nLoopNum++;
                            Div_2R(lhMod, b, out lhDivFree, out lhMod);
                            lhDiv += lhDivFree;
                        }
                    }
                }
                if (bLoopFlagCur)
                {
                    bLoopFlag = false;
                    if (bDebug && nLoopNum > 0)
                    {
                        nLoopSum += nLoopNum;
                        nLoopCount++;
                        if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                    }
                }

                lhDiv.Positive = bDivPositive;
                lhMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_2(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// 执行除法，返回除数与余数（二分法，最慢）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="lhDiv">商</param>
        /// <param name="lhMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_2(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                lhDiv = 0L;
                lhMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out lhDiv, out nMod))
                {
                    lhMod = nMod;
                    return true;
                }
                else
                {
                    lhMod = 0L;
                    return false;
                }
            }
            else
            {
                bool bDivPositive = a.Positive && b.Positive || !a.Positive && !b.Positive;
                bool bModPositive = a.Positive;
                a.Positive = true;
                b.Positive = true;
                int nLengthb = b.Length;
                LongHex liTempb2 = b >> (nLengthb - nFastLength);

                bool bLoopFlagCur = false;
                if (!bLoopFlag)
                {
                    bLoopFlag = true;
                    bLoopFlagCur = true;
                    nLoopNum = 0;
                }

                lhDiv = 0L;
                if (a < b)
                {
                    lhMod = a;
                }
                else if (a == b)
                {
                    lhDiv = 1;
                    lhMod = 0;
                }
                else
                {

                    LongHex liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;

                    //分子小，分母小，结果不确定，余数可能会变否
                    Div(liTempa2, (long)liTempb2, out lhDiv, out nTmpMod);
                    lhMod = a - b * lhDiv;
                    if (lhMod >= 0)
                    {
                        if (lhMod >= b)
                        {
                            lhDiv = lhDiv + 1;
                            lhMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongHex lhDivFree;
                            nLoopNum++;
                            Div_2(lhMod, b, out lhDivFree, out lhMod);
                            lhDiv += lhDivFree;
                        }
                    }
                    else
                    {
                        // 二分法，但测试效果暂时不理想
                        LongHex lhTmpDiv;
                        Div(liTempa2, (long)liTempb2 + 1, out lhTmpDiv, out nTmpMod);
                        if (lhTmpDiv == 0)
                        {
                            lhTmpDiv = 1L;
                        }
                        lhMod = a - b * lhTmpDiv;
                        if (lhMod == b)
                        {
                            lhDiv = lhTmpDiv + 1;
                            lhMod -= b;
                        }
                        else
                        {
                            while (lhMod > b)
                            {
                                //真实值介于lhTmpDiv（小）～lhDiv（大）之间，采用二分法解题
                                LongHex lhMidDiv = (lhTmpDiv + lhDiv) / 2;
                                LongHex lhMidMod = a - b * lhMidDiv;
                                if (lhMidMod < 0)
                                {
                                    //高位向下调，需要循环
                                    lhDiv = lhMidDiv;
                                    nLoopNum++;
                                }
                                else if (lhMidMod < b)
                                {
                                    //退出循环
                                    lhDiv = lhMidDiv;
                                    lhMod = lhMidMod;
                                }
                                else if (lhMidMod == b)
                                {
                                    //退出循环
                                    lhDiv = lhMidDiv + 1;
                                    lhMod = 0L;
                                }
                                else
                                {
                                    //低位向上调，需要循环
                                    lhTmpDiv = lhMidDiv;
                                    lhMod = lhMidMod;
                                    nLoopNum++;
                                }
                            }
                            if (bDebug && nLoopNum > 0)
                            {
                                nLoopSum += nLoopNum;
                                nLoopCount++;
                                if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                            }
                        }
                    }
                }
                if (bLoopFlagCur)
                {
                    bLoopFlag = false;
                    if (bDebug && nLoopNum > 0)
                    {
                        nLoopSum += nLoopNum;
                        nLoopCount++;
                        if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                    }
                }

                lhDiv.Positive = bDivPositive;
                lhMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region public static LongHex operator /(LongHex a , long b)
        /// <summary>
        /// 重载/
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回相数相除的结果</returns>
        public static LongHex operator /(LongHex a, long b)
        {
            LongHex lhDiv;
            long lMod;
            if (Div(a, b, out lhDiv, out lMod))
                return lhDiv;
            else
                throw new InvalidOperationException("error by divide zero");
        }
        #endregion

        #region public static LongHex operator %(LongHex a , long b)
        /// <summary>
        /// 重载%(算法仍需重写并优化)
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回余数</returns>
        public static LongHex operator %(LongHex a, long b)
        {
            LongHex lhDiv;
            long lMod;
            if (Div(a, b, out lhDiv, out lMod))
                return lMod;
            else
                throw new InvalidOperationException("error by divide zero");
            //			return (a - ((a / b) * b));
        }
        #endregion

        #region public static LongHex operator /(LongHex a , LongHex b)
        /// <summary>
        /// 重载/
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>返回两数相除的结果</returns>
        public static LongHex operator /(LongHex a, LongHex b)
        {
            LongHex lhDiv;
            LongHex lhMod;
            if (Div(a, b, out lhDiv, out lhMod))
                return lhDiv;
            else
                throw new InvalidOperationException("error by divide zero");
        }
        #endregion

        #region public static LongHex operator %(LongHex a , LongHex b)
        /// <summary>
        /// 重载%(算法仍需重写并优化)
        /// </summary>
        /// <param name="a">运算数a</param>
        /// <param name="b">运算数b</param>
        /// <returns>除余数</returns>
        public static LongHex operator %(LongHex a, LongHex b)
        {
            LongHex lhDiv;
            LongHex lhMod;
            if (Div(a, b, out lhDiv, out lhMod))
                return lhMod;
            else
                throw new InvalidOperationException("error by divide zero");
            //			return (a - ((a / b) * b));
        }
        #endregion

        #region public static LongHex GetCommonGene(LongHex a , LongHex b)
        /// <summary>
        /// 获得最大公因子
        /// </summary>
        /// <param name="a">十六进制长整形数a</param>
        /// <param name="b">十六进制长整形数b</param>
        /// <returns>返回最大公因子</returns>
        public static LongHex GetCommonGene(LongHex a, LongHex b)
        {
            if (a == 0)
            {
                if (b == 0)
                    return 1;
                else
                {
                    LongHex liResult = new LongHex(b);
                    liResult.Positive = true;
                    return liResult;
                }
            }
            else
            {
                if (b == 0)
                {
                    LongHex liResult = new LongHex(a);
                    liResult.Positive = true;
                    return liResult;
                }
                else
                {
                    while (true)
                    {
                        a %= b;
                        if (a == 0)
                        {
                            if (b.Positive)
                                return b;
                            else
                                return -b;
                        }
                        b %= a;
                        if (b == 0)
                        {
                            if (a.Positive)
                                return a;
                            else
                                return -a;
                        }
                    }
                }
            }
        }
        #endregion

        #region public static LongHex GetCommonDrivisor(LongHex a , LongHex b)
        /// <summary>
        /// 获得最小公约数
        /// </summary>
        /// <param name="a">超长整形a</param>
        /// <param name="b">超长整形b</param>
        /// <returns>最小公约数</returns>
        public static LongHex GetCommonDivisor(LongHex a, LongHex b)
        {
            if (a == 0 || b == 0)
                return 0;
            else
                return a * b / LongHex.GetCommonGene(a, b);
        }
        #endregion

        #region public static LongHex Abs(LongHex a)
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="a">运算数</param>
        /// <returns>返回符号为正的运算结果值</returns>
        public static LongHex Abs(LongHex a)
        {
            LongHex liResult = new LongHex(a);
            liResult.Positive = true;
            return liResult;
        }
        #endregion

        #region public static LongHex Xn(long x , int n)
        /// <summary>
        /// 计算X^n幂
        /// </summary>
        /// <param name="x">底数</param>
        /// <param name="n">指数</param>
        /// <returns>返回运算结果值</returns>
        public static LongHex Xn(long x, int n)
        {
            LongHex liResult = new LongHex(1);
            for (int i = 1; i <= n; i++)
                liResult *= x;
            return liResult;
        }
        #endregion

        #region public static LongHex Xn(LongHex x , int n)
        /// <summary>
        /// 计算X^n幂
        /// </summary>
        /// <param name="x">底数</param>
        /// <param name="n">指数</param>
        /// <returns>计算结果值</returns>
        public static LongHex Xn(LongHex x, int n)
        {
            LongHex liResult = new LongHex(1);
            for (int i = 1; i <= n; i++)
                liResult *= x;
            return liResult;
        }
        #endregion

        #region public static LongHex Pn(int n)
        /// <summary>
        /// 计算n的阶乘
        /// </summary>
        /// <param name="n">阶乘系数</param>
        /// <returns>超长整形数</returns>
        public static LongHex Pn(int n)
        {
            LongHex liResult = new LongHex(1);
            for (int i = 2; i <= n; i++)
                liResult *= i;
            return liResult;
        }
        #endregion

        #region public static LongHex Cmn(int m , int n)
        /// <summary>
        /// 计算Cmn=m*(m-1)...*(m-n)/n!
        /// </summary>
        /// <param name="m">底数m</param>
        /// <param name="n">指数n</param>
        /// <returns>计算结果值</returns>
        public static LongHex Cmn(int m, int n)
        {
            if (n > m || m < 1)
                throw new System.InvalidOperationException("error param in Cmn");
            if (n > m / 2)
                n = m - n;
            if (n == 0)
                return new LongHex(1);
            else
            {
                LongHex liResult = new LongHex(m);
                for (int i = m - n + 1; i < m; i++)
                {
                    liResult *= i;
                }
                liResult /= Pn(n);
                return liResult;
            }
        }
        #endregion

        #region public override bool Equals(object a)
        /// <summary>
        /// 重载Equals
        /// </summary>
        /// <param name="o">同类对象</param>
        /// <returns>返回两值是否相等</returns>
        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (LongHex)o);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region public override int GetHashCode()
        /// <summary>
        /// 重载GetHashCode
        /// </summary>
        /// <returns>返回其符号的HASH值</returns>
        public override int GetHashCode()
        {
            return this.Positive.GetHashCode();
        }
        #endregion

        #region public override string ToString()

        public override string ToString()
        {
            String strResult = "";
            if (!Positive)
                strResult = "-";
            if (Length > 0)
            {
                for (int i = 0; i < Length; i++)
                {
                    int nIndex = Length - 1 - i;
                    if (i == 0)
                        strResult += string.Format("{0:X}", List[nIndex]);
                    else
                        strResult += string.Format("{0:X3}", List[nIndex]);
                }
            }
            else
                strResult += "0";

            return strResult;
        }

        /// <summary>
        /// 结果快照,但结果若太长,则用省略号表示
        /// </summary>
        public String SnapShotHex
        {
            get
            {
                String strResult = "";
                if (!Positive)
                    strResult = "-";
                if (Length > 0)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        int nIndex = Length - 1 - i;
                        if (nIndex == 0)
                            strResult += string.Format("{0:X}", List[nIndex]);
                        else
                        {
                            if (i > 10)
                            {
                                strResult += "...";
                                break;
                            }
                            else
                            {
                                strResult += string.Format("{0:X3}", List[nIndex]);
                            }
                        }
                    }
                }
                else
                    strResult += "0";

                return strResult;
            }
        }

        /// <summary>
        /// 结果快照,但结果若太长,则用省略号表示(按十进制输出)
        /// </summary>
        public String SnapShot
        {
            get
            {
                LongInt a = new LongInt(this);
                return a.SnapShot;
            }
        }

        /// <summary>
        /// 导出结果表达式,若超过8192位时,不允许导出(太长了,没有意义)
        /// </summary>
        /// <param name="bHaveComma">是否使用科学计算符号,</param>
        /// <returns>值字符串</returns>
        public System.String ExportHex(bool bHaveComma)
        {
            if (Length > 8192)
            {
                throw new System.InvalidOperationException("data is too long , use ToConsole to do.");
            }

            String strResult = "";
            if (!Positive)
                strResult = "-";
            if (Length > 0)
            {
                for (int i = 0; i < Length; i++)
                {
                    int nIndex = Length - 1 - i;
                    if (i == 0)
                        strResult += string.Format("{0:X}", List[nIndex]);
                    else
                    {
                        if (bHaveComma)
                            strResult += ",";
                        strResult += string.Format("{0:X3}", List[nIndex]);
                    }
                }
            }
            else
                strResult += "0";

            return strResult;
        }

        /// <summary>
        /// 导出十进制结果表达式，若超过8192位时，不允许导出(太长了，没有意义)
        /// </summary>
        /// <param name="bHaveComma">是否使用科学计算符号,</param>
        /// <returns>值字符串</returns>
        public System.String Export(bool bHaveComma)
        {
            LongInt a = new LongInt(this);
            return a.Export(bHaveComma);
        }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        /// <param name="bHaveComma">是否使用科学计算符号,</param>
        public void ToConsoleHex(bool bHaveComma)
        {
            if (!Positive)
                System.Console.Write("-");
            if (Length > 0)
            {
                for (int i = 0; i < Length; i++)
                {
                    int nIndex = Length - 1 - i;
                    if (i == 0)
                        Console.Write(string.Format("{0:X}", List[nIndex]));
                    else
                    {
                        if (bHaveComma)
                            System.Console.Write(",");
                        Console.Write(string.Format("{0:X3}", List[nIndex]));
                    }
                }
            }
            else
                System.Console.Write("0");
        }

        /// <summary>
        /// 按十进制输出到控制台
        /// </summary>
        /// <param name="bHaveComma">是否使用科学计算符号,</param>
        public void ToConsole(bool bHaveComma)
        {
            LongInt a = new LongInt(this);
            a.ToConsole(bHaveComma);
        }
        #endregion
    }
}