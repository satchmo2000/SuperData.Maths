using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperData.Maths
{
    /// <summary>
    /// LongInt 是用来存储超长数据结构的数据类型
    /// 可以存储如10000!、100^1000等的非常规超大数据
    /// 允许从一般类型数据、字符串等直接进行数据类型转换获得原始数据
    /// 如：
    ///		LongInt liValue = 1000L;
    ///		LongInt liValue = "-100,101";
    ///		LongInt liValue = LongInt.Cmn(100 , 10)获得C100,10的结果
    /// </summary>
    public class LongInt
    {
        /// <summary>
        /// 进数为1000,由低位向高位存储
        /// 如，存储的为10,11，则值为11*1000+10=11,010
        /// </summary>
        public const int nBaseGene = 1000;

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
        /// 超长整形的表示列表,从小大到,nBaseGene=1000进制,这样做不安全,但实现方便
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

        #region public int DecLength
        /// <summary>
        /// 获得长整形数字的长度
        /// </summary>
        public int DecLength
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
                        nNum /= 10;
                    }

                    return nLength + (nCount - 1) * 3;	//每一节占3个字节,最大不会超过1000
                }
                else
                    return 0;
            }
        }
        #endregion

        #region public LongInt()
        /// <summary>
        /// 构造函数,默认为0
        /// </summary>
        public LongInt()
        {
            bPositive = true;
            list.Add(LongInt.Zero);
        }
        #endregion

        #region public LongInt(long nValue)
        /// <summary>
        /// 构造函数LongInt(long nValue)
        /// </summary>
        /// <param name="nValue">长整形数</param>
        public LongInt(long nValue)
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
                list.Add(nValue % nBaseGene);
                nValue /= nBaseGene;
            }
            while (nValue > nBaseGene);
            if (nValue > 0)
                list.Add(nValue);
        }
        #endregion

        #region public LongInt(LongInt liValue)
        /// <summary>
        /// 构造函数 LongInt(LongInt liValue)
        /// </summary>
        /// <param name="liValue">超长整形数据</param>
        public LongInt(LongInt liValue)
        {
            bPositive = liValue.bPositive;
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public LongInt(LongHex lhValue)
        /// <summary>
        /// 构造函数 LongInt(LongHex lhValue)
        /// </summary>
        /// <param name="lhValue">超长十六进制数据</param>
        public LongInt(LongHex lhValue)
        {
            LongInt a = new LongInt();
            bPositive = lhValue.Positive;
            if (lhValue.Length > 0)
            {
                a = lhValue.List[lhValue.Length - 1];
            }
            for (int i = lhValue.Length - 2; i >= 0; i--)
            {
                a *= LongHex.nBaseGene;
                a += lhValue.List[i];
            }
            list = a.List;
        }
        #endregion

        #region public static implicit operator LongInt(long nValue)
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="nValue">长整形数a</param>
        /// <returns>超长整形数</returns>
        public static implicit operator LongInt(long nValue)
        {
            LongInt liValue = new LongInt(nValue);
            return liValue;
        }
        #endregion

        #region public static implicit operator LongInt(string strValue)
        /// <summary>
        /// 类型转换,将字符串直接转换成超长整形,从高位向低位直接一位一位地读并转换,效率不太高
        /// </summary>
        /// <param name="strValue">数字字符串</param>
        /// <returns>超长整形数</returns>
        public static implicit operator LongInt(string strValue)
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
                int nEndPos = strValue.Length % 3;
                long lValue = 0;
                for (int i = 0; i < nEndPos; i++)
                {
                    lValue *= 10L;
                    lValue += strValue[i] & 0x0f;
                }
                LongInt liValue = lValue;
                //后计算低位数据，(位数为8的整数倍)
                for (int i = nEndPos; i < strValue.Length; i += 3)
                {
                    lValue = (strValue[i] & 0x0f) * 100 + (strValue[i + 1] & 0x0f) * 10 + (strValue[i + 2] & 0x0f);
                    liValue <<= 1;
                    liValue += lValue;
                }
                liValue.Positive = bPositive;
                return liValue;
            }
            else
            {
                LongInt liValue = new LongInt(0L);
                return liValue;
            }
        }
        #endregion

        #region public static explicit operator long(LongInt liValue)
        /// <summary>
        /// 类型转换函数,由超长整形转换成长整形,如果太长则抛出异常.
        /// </summary>
        /// <param name="liValue">超长整形</param>
        /// <returns>长整形数</returns>
        public static explicit operator long(LongInt liValue)
        {
            List<long> array = liValue.List;
            int nCount = array.Count;
            if (nCount > 5 || nCount == 0)
                throw new InvalidOperationException("data is too larger.");
            long nResult = 0;
            for (int i = nCount - 1; i >= 0; i--)
            {
                nResult += array[i];
                if (i > 0)
                    nResult *= nBaseGene;
            }
            if (liValue.Positive)
                return nResult;
            else
                return -nResult;
        }
        #endregion

        #region public static explicit operator String(LongInt liValue)
        /// <summary>
        /// 函数类型转换
        /// </summary>
        /// <param name="liValue">超长整形数a</param>
        /// <returns>字符串</returns>
        public static explicit operator String(LongInt liValue)
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
                list.Add(nValue % nBaseGene);
                nValue /= nBaseGene;
            }
            while (nValue > nBaseGene);
            if (nValue > 0)
                list.Add(nValue);
        }
        #endregion

        #region public void Equal(LongInt liValue)
        /// <summary>
        /// 赋值Equal,相当于"="运算,可以直接用this=liValue代替
        /// </summary>
        /// <param name="liValue">超长整形</param>
        public void Equal(LongInt liValue)
        {
            bPositive = liValue.bPositive;
            list.Clear();
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public static LongInt operator >>(LongInt a , int nBit)
        /// <summary>
        /// 对超长整形右移,等价于=a/1000^nBit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="nBit">1000进制的位</param>
        /// <returns>超长整形</returns>
        public static LongInt operator >>(LongInt a, int nBit)
        {
            int nLength = a.Length;
            LongInt liResult = new LongInt(0);
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

        #region public static LongInt operator <<(LongInt a , int nBit)
        /// <summary>
        /// 对超长整形左移,等价于=a*1000^nBit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="nBit">1000进制的位</param>
        /// <returns>超长整形</returns>
        public static LongInt operator <<(LongInt a, int nBit)
        {
            if (a == 0)
                return a;
            LongInt liResult = new LongInt(a);
            List<long> list = new List<long>();
            for (int i = 0; i < nBit; i++)
                list.Add(0L);
            for (int i = 0; i < a.List.Count; i++)
                list.Add(a.List[i]);
            liResult.List = list;
            return liResult;
        }
        #endregion

        #region public static bool operator ==(LongInt a , LongInt b)
        /// <summary>
        /// 重载等于符号
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator ==(LongInt a, LongInt b)
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

        #region public static bool operator !=(LongInt a , LongIt b)
        /// <summary>
        /// 重载不等于符号
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator !=(LongInt a, LongInt b)
        {
            return !(a == b);
        }
        #endregion

        #region public static bool operator >(LongInt a , LongInt b)
        /// <summary>
        /// 重载大于符号
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator >(LongInt a, LongInt b)
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

        #region public static bool operator <(LongInt a , LongInt b)
        /// <summary>
        /// 重载小于符号
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator <(LongInt a, LongInt b)
        {
            return (b > a);
        }
        #endregion

        #region public static bool operator >=(LongInt a , LongInt b)
        /// <summary>
        /// 重载大于等于
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator >=(LongInt a, LongInt b)
        {
            return !(b > a);
        }
        #endregion

        #region public static bool operator <=(LongInt a , LongInt b)
        /// <summary>
        /// 重载不大于符号
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>判断结果</returns>
        public static bool operator <=(LongInt a, LongInt b)
        {
            return !(a > b);
        }
        #endregion

        #region public static LongInt operator -(LongInt a)
        /// <summary>
        /// 一元减,取反
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <returns>取反后的数</returns>
        public static LongInt operator -(LongInt a)
        {
            LongInt liResult = new LongInt(a);
            liResult.Positive = !a.Positive;
            return liResult;
        }
        #endregion

        #region public static LongInt operator +(LongInt a , LongInt b)
        /// <summary>
        /// 重载+
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>加法结果</returns>
        public static LongInt operator +(LongInt a, LongInt b)
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
                    nInc = nNum / LongInt.nBaseGene;
                    nNum %= LongInt.nBaseGene;
                    list.Add(nNum);

                }
                for (j = i; j < nLengthb; j++)
                {
                    long nNum = b.List[j];
                    nNum += nInc;
                    nInc = nNum / LongInt.nBaseGene;
                    nNum %= LongInt.nBaseGene;
                    list.Add(nNum);
                }
                if (nInc > 0)
                    list.Add(nInc);
                LongInt result = new LongInt();
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

        #region public static LongInt operator -(LongInt a , LongInt b)
        /// <summary>
        /// 重载-
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>减的结果</returns>
        public static LongInt operator -(LongInt a, LongInt b)
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
                    LongInt result = new LongInt();
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
                            nNum += LongInt.nBaseGene;
                            nInc--;
                        }
                        nInc += nNum / LongInt.nBaseGene;
                        nNum %= LongInt.nBaseGene;
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
                            nNum += LongInt.nBaseGene;
                            nInc--;
                        }
                        nInc += nNum / LongInt.nBaseGene;
                        nNum %= LongInt.nBaseGene;
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
                            long nNum = LongInt.nBaseGene;
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

        #region public static LongInt operator *(LongInt a , long b)
        /// <summary>
        /// 重载*
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">长整形数b</param>
        /// <returns>乘的结果</returns>
        public static LongInt operator *(LongInt a, long b)
        {
            LongInt liResult = new LongInt();
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
                nResidual = nNum / LongInt.nBaseGene;
                list.Add(nNum % LongInt.nBaseGene);
            }
            while (nResidual > 0)
            {
                list.Add(nResidual % LongInt.nBaseGene);
                nResidual /= LongInt.nBaseGene;
            }
            liResult.List = list;
            return liResult;
        }
        #endregion

        #region public static LongInt operator *(LongInt a , LongInt b)
        /// <summary>
        /// 重载*
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>乘的结果</returns>
        public static LongInt operator *(LongInt a, LongInt b)
        {
            LongInt liResult = new LongInt(0);
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

        #region private static bool Div(LongInt a , long b , out LongInt liDiv , out long lMod)
        /// <summary>
        /// 执行除法，返回除数与余数
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="lMod">余</param>
        /// <returns>除零错返回FALSE，否则返回TRUE</returns>
        private static bool Div(LongInt a, long b, out LongInt liDiv, out long lMod)
        {
            liDiv = new LongInt();
            lMod = 0;
            if (b == 0)
            {
                return false;
            }
            else if (b < 0)
            {
                liDiv.Positive = !a.Positive;
                b = -b;
            }
            else
            {
                liDiv.Positive = a.Positive;
            }
            List<long> listResult = new List<long>();
            bool bFirst = true;

            int nIndex = a.List.Count - 1;
            for (int i = 0; i < a.List.Count; i++)
            {
                if (i > 0)
                    lMod *= nBaseGene;
                lMod += a.List[nIndex--];

                if (lMod >= b || !bFirst)
                {
                    long nDivide = lMod / b;
                    listResult.Add(nDivide);
                    bFirst = false;
                    if (nDivide > 0) lMod -= nDivide * b;
                }
            }

            if (bFirst)
                listResult.Add(0L);
            else
                listResult.Reverse();
            liDiv.List = listResult;
            if (!a.Positive)
            {
                lMod = -lMod;
            }
            return true;
        }
        #endregion

        private static bool Div(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            return Div_N(a, b, out liDiv, out liMod);
        }

        private static int nFastLength = 5;
        public static bool bDebug = true;   //测试循环效率的开关（仅针对Div函数）
        public static int nLoopSum = 0;     //循环总次数
        public static int nLoopCount = 0;   //涉及循环的计算次数
        public static int nLoopNum = 0;     //当前计算的循环次数
        public static int nLoopNumMax = 0;  //最多的循环次数
        public static bool bLoopFlag = false;

        #region private static bool Div_N(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// 执行除法，返回除数与余数（保守循环）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="liMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_N(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                liDiv = 0L;
                liMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out liDiv, out nMod))
                {
                    liMod = nMod;
                    return true;
                }
                else
                {
                    liMod = 0L;
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
                LongInt liTempb2 = b >> (nLengthb - nFastLength);

                liDiv = 0L;
                nLoopNum = 0;
                while (a >= b)
                {
                    LongInt liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;
                    LongInt liTmpDiv;
                    //分子小，分母大，结果小，余数肯定为正
                    Div(liTempa2, (long)liTempb2 + 1, out liTmpDiv, out nTmpMod);
                    //较正
                    if (liTmpDiv == 0 && a >= b)
                    {
                        //当高位相似时，应直接赋于商=1
                        liTmpDiv = 1L;
                    }
                    a -= b * liTmpDiv;
                    if (a == b)
                    {
                        liTmpDiv += 1L;
                        a = 0L;
                    }
                    liDiv += liTmpDiv;
                    nLoopNum++;
                }
                liMod = a;
                if (bDebug && nLoopNum > 0)
                {
                    nLoopSum += nLoopNum;
                    nLoopCount++;
                    if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                }

                liDiv.Positive = bDivPositive;
                liMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_R(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// 执行除法，返回除数与余数（保守递归）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="liMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_R(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                liDiv = 0L;
                liMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out liDiv, out nMod))
                {
                    liMod = nMod;
                    return true;
                }
                else
                {
                    liMod = 0L;
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
                LongInt liTempb2 = b >> (nLengthb - nFastLength);

                LongInt liTempa2 = a >> (nLengthb - nFastLength);
                long nTmpMod;
                //分子小，分母大，结果小，余数肯定为正
                Div(liTempa2, (long)liTempb2 + 1, out liDiv, out nTmpMod);
                //较正
                if (liDiv == 0 && a >= b)
                {
                    //当高位相似时，应直接赋于商=1
                    liDiv = 1;
                }
                liMod = a - b * liDiv;
                if (liMod > 0)
                {
                    if (liMod > b)
                    {
                        //递归计算剩余部份的商(比余数大的才递归)
                        LongInt liDivFree;
                        Div_R(liMod, b, out liDivFree, out liMod);
                        liDiv += liDivFree;
                    }
                    else if (liMod == b)
                    {
                        liDiv += 1L;
                        liMod = 0L;
                    }
                }

                liDiv.Positive = bDivPositive;
                liMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_G(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// 执行除法，返回除数与余数（两值之间权衡）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="liMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_G(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                liDiv = 0L;
                liMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out liDiv, out nMod))
                {
                    liMod = nMod;
                    return true;
                }
                else
                {
                    liMod = 0L;
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
                LongInt liTempb2 = b >> (nLengthb - nFastLength);

                liDiv = 0L;
                nLoopNum = 0;
                while (a >= b)
                {
                    LongInt liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;
                    LongInt liTmpDiv;
                    //分子小，分母小，结果不确定，余数可能会变否
                    Div(liTempa2, (long)liTempb2, out liTmpDiv, out nTmpMod);
                    //较正
                    if (liTmpDiv == 0)
                    {
                        //当高位相似时，应直接赋于商=1
                        liTmpDiv = 1L;
                    }
                    LongInt liTmpMod = a - b * liTmpDiv;
                    if (liTmpMod >= 0)
                    {
                        if (liTmpMod == b)
                        {
                            liTmpDiv += 1L;
                            a = 0L;
                        }
                        else if (liTmpMod < b)
                        {
                            a = liTmpMod;
                        }
                        liDiv += liTmpDiv;
                    }
                    else
                    {
                        //分子小，分母大，结果小，余数肯定为正
                        Div(liTempa2, (long)liTempb2 + 1, out liTmpDiv, out nTmpMod);
                        //较正
                        if (liTmpDiv == 0)
                        {
                            //当高位相似时，应直接赋于商=1
                            liTmpDiv = 1L;
                        }
                        a -= b * liTmpDiv;
                        if (a == b)
                        {
                            liTmpDiv += 1L;
                            a = 0L;
                        }
                        liDiv += liTmpDiv;
                    }
                    nLoopNum++;
                }
                liMod = a;
                if (bDebug && nLoopNum > 0)
                {
                    nLoopSum += nLoopNum;
                    nLoopCount++;
                    if (nLoopNum > nLoopNumMax) nLoopNumMax = nLoopNum;
                }

                liDiv.Positive = bDivPositive;
                liMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_2R(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// 执行除法，返回除数与余数（大数小数之间来回递归）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="liMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_2R(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                liDiv = 0L;
                liMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out liDiv, out nMod))
                {
                    liMod = nMod;
                    return true;
                }
                else
                {
                    liMod = 0L;
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
                LongInt liTempb2 = b >> (nLengthb - nFastLength);

                bool bLoopFlagCur = false;
                if (!bLoopFlag)
                {
                    bLoopFlag = true;
                    bLoopFlagCur = true;
                    nLoopNum = 0;
                }

                //方案2：内循环，相对递归，效率提升1%
                liDiv = 0L;
                if (a < b)
                {
                    liMod = a;
                }
                else if (a == b)
                {
                    liDiv = 1;
                    liMod = 0;
                }
                else
                {
                    LongInt liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;

                    //分子小，分母小，结果可大可小，余数可正可负
                    Div(liTempa2, (long)liTempb2, out liDiv, out nTmpMod);
                    liMod = a - b * liDiv;
                    if (liMod >= 0)
                    {
                        if (liMod >= b)
                        {
                            liDiv = liDiv + 1;
                            liMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongInt liDivFree;
                            nLoopNum++;
                            Div_2R(liMod, b, out liDivFree, out liMod);
                            liDiv += liDivFree;
                        }
                    }
                    else
                    {
                        Div(liTempa2, (long)liTempb2 + 1, out liDiv, out nTmpMod);
                        if (liDiv == 0)
                        {
                            liDiv = 1L;
                        }
                        liMod = a - b * liDiv;
                        if (liMod >= b)
                        {
                            liDiv = liDiv + 1;
                            liMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongInt liDivFree;
                            nLoopNum++;
                            Div_2R(liMod, b, out liDivFree, out liMod);
                            liDiv += liDivFree;
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

                liDiv.Positive = bDivPositive;
                liMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region private static bool Div_2(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// 执行除法，返回除数与余数（二分法）
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <param name="liDiv">商</param>
        /// <param name="liMod">余数</param>
        /// <returns>是否正常计算</returns>
        private static bool Div_2(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //采取的处理为计算近似值,再补充
            if (a.Length < b.Length)
            {
                liDiv = 0L;
                liMod = a;
                return true;
            }
            else if (b.Length <= nFastLength)
            {
                long nNum = (long)b;
                long nMod;
                if (Div(a, nNum, out liDiv, out nMod))
                {
                    liMod = nMod;
                    return true;
                }
                else
                {
                    liMod = 0L;
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
                LongInt liTempb2 = b >> (nLengthb - nFastLength);

                bool bLoopFlagCur = false;
                if (!bLoopFlag)
                {
                    bLoopFlag = true;
                    bLoopFlagCur = true;
                    nLoopNum = 0;
                }

                //方案2：内循环，相对递归，效率提升1%
                liDiv = 0L;
                if (a < b)
                {
                    liMod = a;
                }
                else if (a == b)
                {
                    liDiv = 1;
                    liMod = 0;
                }
                else
                {
                    LongInt liTempa2 = a >> (nLengthb - nFastLength);
                    long nTmpMod;
                    //分子小，分母小，结果可大可小，余数可正可负
                    Div(liTempa2, (long)liTempb2, out liDiv, out nTmpMod);
                    liMod = a - b * liDiv;
                    if (liMod >= 0)
                    {
                        if (liMod >= b)
                        {
                            liDiv = liDiv + 1;
                            liMod -= b;
                            //递归计算剩余部份的商(比余数大的才递归)
                            LongInt liDivFree;
                            nLoopNum++;
                            Div_2(liMod, b, out liDivFree, out liMod);
                            liDiv += liDivFree;
                        }
                    }
                    else
                    {
                        // 二分法，但测试效果暂时不理想
                        LongInt liTmpDiv;
                        Div(liTempa2, (long)liTempb2 + 1, out liTmpDiv, out nTmpMod);
                        if (liTmpDiv == 0)
                        {
                            liTmpDiv = 1L;
                        }
                        liMod = a - b * liTmpDiv;
                        if (liMod == b)
                        {
                            liDiv = liTmpDiv + 1;
                            liMod -= b;
                        }
                        else
                        {
                            while (liMod > b)
                            {
                                //真实值介于liTmpDiv（小）～liDiv（大）之间，采用二分法解题
                                LongInt liMidDiv = (liTmpDiv + liDiv) / 2;
                                LongInt liMidMod = a - b * liMidDiv;
                                if (liMidMod < 0)
                                {
                                    //高位向下调，需要循环
                                    liDiv = liMidDiv;
                                    nLoopNum++;
                                }
                                else if (liMidMod < b)
                                {
                                    //退出循环
                                    liDiv = liMidDiv;
                                    liMod = liMidMod;
                                }
                                else if (liMidMod == b)
                                {
                                    //退出循环
                                    liDiv = liMidDiv + 1;
                                    liMod = 0L;
                                }
                                else
                                {
                                    //低位向上调，需要循环
                                    liTmpDiv = liMidDiv;
                                    liMod = liMidMod;
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

                liDiv.Positive = bDivPositive;
                liMod.Positive = bModPositive;
                return true;
            }
        }
        #endregion

        #region public static LongInt operator /(LongInt a , long b)
        /// <summary>
        /// 重载/
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>除数</returns>
        public static LongInt operator /(LongInt a, long b)
        {
            LongInt liDiv;
            long lMod;
            if (Div(a, b, out liDiv, out lMod))
                return liDiv;
            else
                throw new InvalidOperationException("error by divide zero");
        }
        #endregion

        #region public static LongInt operator %(LongInt a , long b)
        /// <summary>
        /// 重载%
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">长整形数b</param>
        /// <returns>余数</returns>
        public static LongInt operator %(LongInt a, long b)
        {
            LongInt liDiv;
            long lMod;
            if (Div(a, b, out liDiv, out lMod))
                return lMod;
            else
                throw new InvalidOperationException("error by divide zero");
            //			return (a - ((a / b) * b));
        }
        #endregion

        #region public static LongInt operator /(LongInt a , LongInt b)
        /// <summary>
        /// 重载/
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>除数</returns>
        public static LongInt operator /(LongInt a, LongInt b)
        {
            LongInt liDiv;
            LongInt liMod;
            if (Div(a, b, out liDiv, out liMod))
                return liDiv;
            else
                throw new InvalidOperationException("error by divide zero");
        }
        #endregion

        #region public static LongInt operator %(LongInt a , LongInt b)
        /// <summary>
        /// 重载%
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>余数</returns>
        public static LongInt operator %(LongInt a, LongInt b)
        {
            LongInt liDiv;
            LongInt liMod;
            if (Div(a, b, out liDiv, out liMod))
                return liMod;
            else
                throw new InvalidOperationException("error by divide zero");
            //			return (a - ((a / b) * b));
        }
        #endregion

        #region public static LongInt GetCommonGene(LongInt a , LongInt b)
        /// <summary>
        /// 获得最大公因子
        /// </summary>
        /// <param name="a">超长整形数a</param>
        /// <param name="b">超长整形数b</param>
        /// <returns>最大公因子</returns>
        public static LongInt GetCommonGene(LongInt a, LongInt b)
        {
            if (a == 0)
            {
                if (b == 0)
                    return 1;
                else
                {
                    LongInt liResult = new LongInt(b);
                    liResult.Positive = true;
                    return liResult;
                }
            }
            else
            {
                if (b == 0)
                {
                    LongInt liResult = new LongInt(a);
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

        #region public static LongInt GetCommonDrivisor(LongInt a , LongInt b)
        /// <summary>
        /// 获得最小公约数
        /// </summary>
        /// <param name="a">长整形数a</param>
        /// <param name="b">长整形数b</param>
        /// <returns>最小公约数</returns>
        public static LongInt GetCommonDivisor(LongInt a, LongInt b)
        {
            if (a == 0 || b == 0)
                return 0;
            else
                return a * b / LongInt.GetCommonGene(a, b);
        }
        #endregion

        #region public static LongInt Abs(LongInt a)
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="a">运算数</param>
        /// <returns>返回符号为正的运算结果值</returns>
        public static LongInt Abs(LongInt a)
        {
            LongInt liResult = new LongInt(a);
            liResult.Positive = true;
            return liResult;
        }
        #endregion

        #region public static LongInt Xn(long x , int n)
        /// <summary>
        /// 计算X^n幂
        /// </summary>
        /// <param name="x">底数</param>
        /// <param name="n">指数</param>
        /// <returns>返回运算结果值</returns>
        public static LongInt Xn(long x, int n)
        {
            LongInt liResult = new LongInt(1);
            for (uint i = 1; i <= n; i++)
                liResult *= x;
            return liResult;
        }
        #endregion

        #region public static LongInt Xn(LongInt x , int n)
        /// <summary>
        /// 计算X^n幂
        /// </summary>
        /// <param name="x">底数</param>
        /// <param name="n">指数</param>
        /// <returns>计算结果值</returns>
        public static LongInt Xn(LongInt x, int n)
        {
            LongInt liResult = new LongInt(1);
            for (uint i = 1; i <= n; i++)
                liResult *= x;
            return liResult;
        }
        #endregion

        #region public static LongInt Pn(int n)
        /// <summary>
        /// 计算n的阶乘
        /// </summary>
        /// <param name="n">阶乘系数</param>
        /// <returns>长整形数</returns>
        public static LongInt Pn(int n)
        {
            LongInt liResult = new LongInt(1);
            for (int i = 2; i <= n; i++)
                liResult *= i;
            return liResult;
        }
        #endregion

        #region public static LongInt Cmn(uint m , int n)
        /// <summary>
        /// 计算Cmn=m*(m-1)...*(m-n)/n!
        /// </summary>
        /// <param name="m">底数m</param>
        /// <param name="n">指数n</param>
        /// <returns>计算结果值</returns>
        public static LongInt Cmn(int m, int n)
        {
            if (n > m || m < 1)
                throw new System.InvalidOperationException("error param in Cmn");
            if (n > m / 2)
                n = m - n;
            if (n == 0)
                return new LongInt(1);
            else
            {
                LongInt liResult = new LongInt(m);
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
                return (bool)(this == (LongInt)o);
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
                        strResult += List[nIndex].ToString();
                    else
                        strResult += string.Format("{0:D3}", List[nIndex]);
                }
            }
            else
                strResult += "0";

            return strResult;
        }

        /// <summary>
        /// 结果快照,但结果若太长,则用省略号表示
        /// </summary>
        public String SnapShot
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
                        if (i == 0)
                            strResult += List[nIndex].ToString();
                        else
                        {
                            if (i > 10)
                            {
                                strResult += "...";
                                break;
                            }
                            else
                            {
                                strResult += string.Format("{0:D3}", List[nIndex]);
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
        /// 导出结果表达式,若超过8192位时,不允许导出(太长了,没有意义)
        /// </summary>
        /// <param name="bHaveComma">是否科学计数符号,</param>
        /// <returns>值字符串</returns>
        public System.String Export(bool bHaveComma)
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
                        strResult += List[nIndex].ToString();
                    else
                    {
                        if (bHaveComma)
                            strResult += ",";
                        strResult += string.Format("{0:D3}", List[nIndex]);
                    }
                }
            }
            else
                strResult += "0";

            return strResult;
        }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        /// <param name="bHaveComma">是否科学计数符号,</param>
        public void ToConsole(bool bHaveComma)
        {
            if (!Positive)
                System.Console.Write("-");
            if (Length > 0)
            {
                for (int i = 0; i < Length; i++)
                {
                    int nIndex = Length - 1 - i;
                    if (i == 0)
                        Console.Write(List[nIndex].ToString());
                    else
                    {
                        if (bHaveComma)
                            System.Console.Write(",");
                        Console.Write(string.Format("{0:D3}", List[nIndex]));
                    }
                }
            }
            else
                System.Console.Write("0");
        }
        #endregion
    }
}