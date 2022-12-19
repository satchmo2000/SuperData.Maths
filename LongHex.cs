using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperData.Maths
{
    /// <summary>
    /// LongHex �������洢�������ݽṹ����������
    /// ���Դ洢��10000!��100^1000�ȵķǳ��泬������
    /// �����һ���������ݡ��ַ�����ֱ�ӽ�����������ת�����ԭʼ����
    /// �磺
    ///		LongHex liValue = 1000L;
    ///		LongHex liValue = "-100,101";
    ///		LongHex liValue = LongHex.Cmn(100 , 10)���C100,10�Ľ��
    /// </summary>
    public class LongHex
    {
        /// <summary>
        /// ����Ϊ0x1000,�ɵ�λ���λ�洢
        /// �磬�洢��Ϊ10,11����ֵΪ11*0x1000+10=11,010
        /// </summary>
        public const int nBaseGene = 0x1000;

        /// <summary>
        /// �������룬��λ�������������
        /// </summary>
        public const int nBaseMask = 0xfff;

        /// <summary>
        /// λ������ȳ˷����������죬ͨ�����������������������
        /// </summary>
        public const int nBaseBitCount = 12;

        /// <summary>
        /// ֵΪ0�ĳ���
        /// </summary>
        public const int Zero = 0;

        /// <summary>
        /// ���ݷ���
        /// trueΪ��
        /// falseΪ��
        /// </summary>
        private bool bPositive = true;

        /// <summary>
        /// ���ݴ洢����,�ɵ�λ����λ�Ĵ洢��ʽ
        /// �磬�洢��Ϊ10,11����ֵΪ11*1000+10=11010
        /// </summary>
        private List<long> list = new List<long>(0);

        #region public bool Positive
        /// <summary>
        /// �Ƿ�Ϊ������TRUEΪ������FALSEΪ����
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
        /// �������εı�ʾ�б�,��С��,nBaseGene=0x1000����,����������ȫ,��ʵ�ַ���
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
        /// ��ó����εĳ���(ռ�ÿռ�)
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
        /// ��ó��������ֵĳ���
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
                    return nLength + (nCount - 1) * 3;	//ÿһ��ռ3���ֽ�,��󲻻ᳬ��0x1000
                }
                else
                    return 0;
            }
        }
        #endregion

        #region public LongHex()
        /// <summary>
        /// ���캯��,Ĭ��Ϊ0
        /// </summary>
        public LongHex()
        {
            bPositive = true;
            list.Add(LongHex.Zero);
        }
        #endregion

        #region public LongHex(long nValue)
        /// <summary>
        /// ���캯��LongHex(long nValue)
        /// </summary>
        /// <param name="nValue">��������</param>
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
        /// ���캯�� LongHex(LongHex liValue)
        /// </summary>
        /// <param name="liValue">������������</param>
        public LongHex(LongHex liValue)
        {
            bPositive = liValue.bPositive;
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public LongHex(LongInt liValue)
        /// <summary>
        /// ���캯�� LongHex(LongInt liValue)
        /// </summary>
        /// <param name="liValue">����ʮ��������</param>
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
        /// ����ʮ�����������ַ���
        /// </summary>
        /// <param name="strValue">�����ַ���</param>
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
        /// ����ת��
        /// </summary>
        /// <param name="nValue">����������</param>
        /// <returns>ʮ�����Ƴ���������</returns>
        public static implicit operator LongHex(long nValue)
        {
            LongHex liValue = new LongHex(nValue);
            return liValue;
        }
        #endregion

        #region public static implicit operator LongHex(string strValue)
        /// <summary>
        /// ����ת��,���ַ���ֱ��ת���ɳ�������,�Ӹ�λ���λֱ��һλһλ�ض���ת��,Ч�ʲ�̫��
        /// </summary>
        /// <param name="strValue">�����ַ���</param>
        /// <returns>ʮ�����Ƴ���������</returns>
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
                //�ȼ����λ���ݣ�(����8λ����)
                int nEndPos = strValue.Length % 8;
                long lValue = 0;
                for (int i = 0; i < nEndPos; i++)
                {
                    lValue *= 10L;
                    lValue += strValue[i] & 0x0f;
                }
                LongHex liValue = lValue;
                //������λ���ݣ�(λ��Ϊ8��������)
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
        /// ����ת������,�ɳ�������ת���ɳ�����,���̫�����׳��쳣.
        /// </summary>
        /// <param name="liValue">��������</param>
        /// <returns>��������</returns>
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
        /// ��������ת��
        /// </summary>
        /// <param name="liValue">ʮ�����Ƴ���������</param>
        /// <returns>�ַ���</returns>
        public static explicit operator String(LongHex liValue)
        {
            return liValue.Export(true);
        }
        #endregion

        #region public void Equal(long nValue)
        /// <summary>
        /// ��ֵ����Equal,�൱��"="����,����ֱ����this=nValue����
        /// </summary>
        /// <param name="nValue">��������</param>
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
        /// ��ֵEqual,�൱��"="����,����ֱ����this=liValue����
        /// </summary>
        /// <param name="liValue">��������</param>
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
        /// �Գ�����������,�ȼ���=a/0x1000^nBit
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="nBit">0x1000���Ƶ�λ</param>
        /// <returns>��������</returns>
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
        /// �Գ�����������,�ȼ���=a*0x1000^nBit
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="nBit">0x1000���Ƶ�λ</param>
        /// <returns>��������</returns>
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
        /// ���ص��ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>��ȷ���true</returns>
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
        /// ���ص��ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>���ȷ���true</returns>
        public static bool operator !=(LongHex a, LongHex b)
        {
            return !(a == b);
        }
        #endregion

        #region public static bool operator >(LongHex a , LongHex b)
        /// <summary>
        /// ���ش��ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������a�󷵻�true</returns>
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
        /// ����С�ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������b�󷵻�true</returns>
        public static bool operator <(LongHex a, LongHex b)
        {
            return (b > a);
        }
        #endregion

        #region public static bool operator >=(LongHex a , LongHex b)
        /// <summary>
        /// ���ش��ڵ��ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������a�ϴ󷵻�true</returns>
        public static bool operator >=(LongHex a, LongHex b)
        {
            return !(b > a);
        }
        #endregion

        #region public static bool operator <=(LongHex a , LongHex b)
        /// <summary>
        /// ���ز����ڷ���
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������b�ϴ󷵻�true</returns>
        public static bool operator <=(LongHex a, LongHex b)
        {
            return !(a > b);
        }
        #endregion

        #region public static LongHex operator -(LongHex a)
        /// <summary>
        /// һԪ��,ȡ��
        /// </summary>
        /// <param name="a">������a</param>
        /// <returns>������ȡ��</returns>
        public static LongHex operator -(LongHex a)
        {
            LongHex liResult = new LongHex(a);
            liResult.Positive = !a.Positive;
            return liResult;
        }
        #endregion

        #region public static LongHex operator +(LongHex a , LongHex b)
        /// <summary>
        /// ����+
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>�������ֵ</returns>
        public static LongHex operator +(LongHex a, LongHex b)
        {
            int nLengtha = a.Length;
            int nLengthb = b.Length;
            if (a.Positive && b.Positive || !a.Positive && !b.Positive)
            {
                //ִ�мӷ�
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
                //ִ�м���
                return a - (-b);
            }

        }
        #endregion

        #region public static LongHex operator -(LongHex a , LongHex b)
        /// <summary>
        /// ����-
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>�����������ֵ</returns>
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
                    //������
                    LongHex result = new LongHex();
                    result.Positive = a.Positive;

                    //ִ�м���
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
                        //ȡ�����
                        nInc = System.Math.Abs(nInc) - 1;
                        if (nInc > 0)
                            list.Add(nInc);
                        //���������´���
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

                    //�жϸ�λ�Ƿ�Ϊ0
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
                //ִ�мӷ�
                return a + (-b);
            }
        }
        #endregion

        #region public static LongHex operator *(LongHex a , long b)
        /// <summary>
        /// ����*
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>����������˽��</returns>
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
        /// ����*
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>����������˵Ľ��</returns>
        public static LongHex operator *(LongHex a, LongHex b)
        {
            LongHex liResult = new LongHex(0);
            List<long> listb = b.List;
            //�Ӹ�λ����λ��
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
        /// ִ�г��������س���������
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lMod">��</param>
        /// <returns>�������FALSE�����򷵻�TRUE</returns>
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
                        //���һ�β���Ҫ��λ���������
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
        public static bool bDebug = true;   //����ѭ��Ч�ʵĿ��أ������Div������
        public static int nLoopSum = 0;     //ѭ���ܴ���
        public static int nLoopCount = 0;   //�漰ѭ���ļ������
        public static int nLoopNum = 0;     //��ǰ�����ѭ������
        public static int nLoopNumMax = 0;  //����ѭ������
        public static bool bLoopFlag = false;

        #region private static bool Div_N(LongHex a ,LongHex b , out LongHex lhDiv , out LongHex lhMod)
        /// <summary>
        /// ִ�г��������س���������������ѭ����
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lhMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_N(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                    //����С����ĸ�󣬽��С�������϶�Ϊ��
                    Div(liTempa2, (long)liTempb2 + 1, out lhTmpDiv, out nTmpMod);
                    //����
                    if (lhTmpDiv == 0 && a >= b)
                    {
                        //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
        /// ִ�г��������س��������������صݹ飩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lhMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_R(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                //����С����ĸ�󣬽��С�������϶�Ϊ��
                Div(liTempa2, (long)liTempb2 + 1, out lhDiv, out nTmpMod);
                //����
                if (lhDiv == 0 && a >= b)
                {
                    //����λ����ʱ��Ӧֱ�Ӹ�����=1
                    lhDiv = 1;
                }
                lhMod = a - b * lhDiv;
                if (lhMod > 0)
                {
                    if (lhMod > b)
                    {
                        //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
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
        /// ִ�г��������س�������������ֵ֮��Ȩ�⣩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lhMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_G(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                    
                    //����С����ĸС�������ȷ�����������ܻ���
                    Div(liTempa2, (long)liTempb2, out lhTmpDiv, out nTmpMod);
                    //����
                    if (lhTmpDiv == 0)
                    {
                        //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
                        //����С����ĸ�󣬽��С�������϶�Ϊ��
                        Div(liTempa2, (long)liTempb2 + 1, out lhTmpDiv, out nTmpMod);
                        //����
                        if (lhTmpDiv == 0 && a >= b)
                        {
                            //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
        /// ִ�г��������س�������������ֵ֮��Ȩ�⣩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lhMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_2R(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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

                    //����С����ĸС�������ȷ�����������ܻ���
                    Div(liTempa2, (long)liTempb2, out lhDiv, out nTmpMod);
                    lhMod = a - b * lhDiv;
                    if (lhMod >= 0)
                    {
                        if (lhMod >= b)
                        {
                            lhDiv = lhDiv + 1;
                            lhMod -= b;
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
                            LongHex lhDivFree;
                            nLoopNum++;
                            Div_2R(lhMod, b, out lhDivFree, out lhMod);
                            lhDiv += lhDivFree;
                        }
                    }
                    else
                    {
                        //����С����ĸ�󣬽��С�������϶�Ϊ��
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
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
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
        /// ִ�г��������س��������������ַ���������
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="lhDiv">��</param>
        /// <param name="lhMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_2(LongHex a, LongHex b, out LongHex lhDiv, out LongHex lhMod)
        {
            int nFastLength = 4;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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

                    //����С����ĸС�������ȷ�����������ܻ���
                    Div(liTempa2, (long)liTempb2, out lhDiv, out nTmpMod);
                    lhMod = a - b * lhDiv;
                    if (lhMod >= 0)
                    {
                        if (lhMod >= b)
                        {
                            lhDiv = lhDiv + 1;
                            lhMod -= b;
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
                            LongHex lhDivFree;
                            nLoopNum++;
                            Div_2(lhMod, b, out lhDivFree, out lhMod);
                            lhDiv += lhDivFree;
                        }
                    }
                    else
                    {
                        // ���ַ���������Ч����ʱ������
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
                                //��ʵֵ����lhTmpDiv��С����lhDiv����֮�䣬���ö��ַ�����
                                LongHex lhMidDiv = (lhTmpDiv + lhDiv) / 2;
                                LongHex lhMidMod = a - b * lhMidDiv;
                                if (lhMidMod < 0)
                                {
                                    //��λ���µ�����Ҫѭ��
                                    lhDiv = lhMidDiv;
                                    nLoopNum++;
                                }
                                else if (lhMidMod < b)
                                {
                                    //�˳�ѭ��
                                    lhDiv = lhMidDiv;
                                    lhMod = lhMidMod;
                                }
                                else if (lhMidMod == b)
                                {
                                    //�˳�ѭ��
                                    lhDiv = lhMidDiv + 1;
                                    lhMod = 0L;
                                }
                                else
                                {
                                    //��λ���ϵ�����Ҫѭ��
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
        /// ����/
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������������Ľ��</returns>
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
        /// ����%(�㷨������д���Ż�)
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>��������</returns>
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
        /// ����/
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������������Ľ��</returns>
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
        /// ����%(�㷨������д���Ż�)
        /// </summary>
        /// <param name="a">������a</param>
        /// <param name="b">������b</param>
        /// <returns>������</returns>
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
        /// ����������
        /// </summary>
        /// <param name="a">ʮ�����Ƴ�������a</param>
        /// <param name="b">ʮ�����Ƴ�������b</param>
        /// <returns>�����������</returns>
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
        /// �����С��Լ��
        /// </summary>
        /// <param name="a">��������a</param>
        /// <param name="b">��������b</param>
        /// <returns>��С��Լ��</returns>
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
        /// ȡ����ֵ
        /// </summary>
        /// <param name="a">������</param>
        /// <returns>���ط���Ϊ����������ֵ</returns>
        public static LongHex Abs(LongHex a)
        {
            LongHex liResult = new LongHex(a);
            liResult.Positive = true;
            return liResult;
        }
        #endregion

        #region public static LongHex Xn(long x , int n)
        /// <summary>
        /// ����X^n��
        /// </summary>
        /// <param name="x">����</param>
        /// <param name="n">ָ��</param>
        /// <returns>����������ֵ</returns>
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
        /// ����X^n��
        /// </summary>
        /// <param name="x">����</param>
        /// <param name="n">ָ��</param>
        /// <returns>������ֵ</returns>
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
        /// ����n�Ľ׳�
        /// </summary>
        /// <param name="n">�׳�ϵ��</param>
        /// <returns>����������</returns>
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
        /// ����Cmn=m*(m-1)...*(m-n)/n!
        /// </summary>
        /// <param name="m">����m</param>
        /// <param name="n">ָ��n</param>
        /// <returns>������ֵ</returns>
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
        /// ����Equals
        /// </summary>
        /// <param name="o">ͬ�����</param>
        /// <returns>������ֵ�Ƿ����</returns>
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
        /// ����GetHashCode
        /// </summary>
        /// <returns>��������ŵ�HASHֵ</returns>
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
        /// �������,�������̫��,����ʡ�Ժű�ʾ
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
        /// �������,�������̫��,����ʡ�Ժű�ʾ(��ʮ�������)
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
        /// ����������ʽ,������8192λʱ,��������(̫����,û������)
        /// </summary>
        /// <param name="bHaveComma">�Ƿ�ʹ�ÿ�ѧ�������,</param>
        /// <returns>ֵ�ַ���</returns>
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
        /// ����ʮ���ƽ�����ʽ��������8192λʱ����������(̫���ˣ�û������)
        /// </summary>
        /// <param name="bHaveComma">�Ƿ�ʹ�ÿ�ѧ�������,</param>
        /// <returns>ֵ�ַ���</returns>
        public System.String Export(bool bHaveComma)
        {
            LongInt a = new LongInt(this);
            return a.Export(bHaveComma);
        }

        /// <summary>
        /// ���������̨
        /// </summary>
        /// <param name="bHaveComma">�Ƿ�ʹ�ÿ�ѧ�������,</param>
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
        /// ��ʮ�������������̨
        /// </summary>
        /// <param name="bHaveComma">�Ƿ�ʹ�ÿ�ѧ�������,</param>
        public void ToConsole(bool bHaveComma)
        {
            LongInt a = new LongInt(this);
            a.ToConsole(bHaveComma);
        }
        #endregion
    }
}