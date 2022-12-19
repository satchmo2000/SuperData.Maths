using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperData.Maths
{
    /// <summary>
    /// LongInt �������洢�������ݽṹ����������
    /// ���Դ洢��10000!��100^1000�ȵķǳ��泬������
    /// �����һ���������ݡ��ַ�����ֱ�ӽ�����������ת�����ԭʼ����
    /// �磺
    ///		LongInt liValue = 1000L;
    ///		LongInt liValue = "-100,101";
    ///		LongInt liValue = LongInt.Cmn(100 , 10)���C100,10�Ľ��
    /// </summary>
    public class LongInt
    {
        /// <summary>
        /// ����Ϊ1000,�ɵ�λ���λ�洢
        /// �磬�洢��Ϊ10,11����ֵΪ11*1000+10=11,010
        /// </summary>
        public const int nBaseGene = 1000;

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
        /// �������εı�ʾ�б�,��С��,nBaseGene=1000����,����������ȫ,��ʵ�ַ���
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

        #region public int DecLength
        /// <summary>
        /// ��ó��������ֵĳ���
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

                    return nLength + (nCount - 1) * 3;	//ÿһ��ռ3���ֽ�,��󲻻ᳬ��1000
                }
                else
                    return 0;
            }
        }
        #endregion

        #region public LongInt()
        /// <summary>
        /// ���캯��,Ĭ��Ϊ0
        /// </summary>
        public LongInt()
        {
            bPositive = true;
            list.Add(LongInt.Zero);
        }
        #endregion

        #region public LongInt(long nValue)
        /// <summary>
        /// ���캯��LongInt(long nValue)
        /// </summary>
        /// <param name="nValue">��������</param>
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
        /// ���캯�� LongInt(LongInt liValue)
        /// </summary>
        /// <param name="liValue">������������</param>
        public LongInt(LongInt liValue)
        {
            bPositive = liValue.bPositive;
            for (int i = 0; i < liValue.Length; i++)
                list.Add(liValue.List[i]);
        }
        #endregion

        #region public LongInt(LongHex lhValue)
        /// <summary>
        /// ���캯�� LongInt(LongHex lhValue)
        /// </summary>
        /// <param name="lhValue">����ʮ����������</param>
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
        /// ����ת��
        /// </summary>
        /// <param name="nValue">��������a</param>
        /// <returns>����������</returns>
        public static implicit operator LongInt(long nValue)
        {
            LongInt liValue = new LongInt(nValue);
            return liValue;
        }
        #endregion

        #region public static implicit operator LongInt(string strValue)
        /// <summary>
        /// ����ת��,���ַ���ֱ��ת���ɳ�������,�Ӹ�λ���λֱ��һλһλ�ض���ת��,Ч�ʲ�̫��
        /// </summary>
        /// <param name="strValue">�����ַ���</param>
        /// <returns>����������</returns>
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
                //�ȼ����λ���ݣ�(����8λ����)
                int nEndPos = strValue.Length % 3;
                long lValue = 0;
                for (int i = 0; i < nEndPos; i++)
                {
                    lValue *= 10L;
                    lValue += strValue[i] & 0x0f;
                }
                LongInt liValue = lValue;
                //������λ���ݣ�(λ��Ϊ8��������)
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
        /// ����ת������,�ɳ�������ת���ɳ�����,���̫�����׳��쳣.
        /// </summary>
        /// <param name="liValue">��������</param>
        /// <returns>��������</returns>
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
        /// ��������ת��
        /// </summary>
        /// <param name="liValue">����������a</param>
        /// <returns>�ַ���</returns>
        public static explicit operator String(LongInt liValue)
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
        /// ��ֵEqual,�൱��"="����,����ֱ����this=liValue����
        /// </summary>
        /// <param name="liValue">��������</param>
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
        /// �Գ�����������,�ȼ���=a/1000^nBit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="nBit">1000���Ƶ�λ</param>
        /// <returns>��������</returns>
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
        /// �Գ�����������,�ȼ���=a*1000^nBit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="nBit">1000���Ƶ�λ</param>
        /// <returns>��������</returns>
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
        /// ���ص��ڷ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
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
        /// ���ز����ڷ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
        public static bool operator !=(LongInt a, LongInt b)
        {
            return !(a == b);
        }
        #endregion

        #region public static bool operator >(LongInt a , LongInt b)
        /// <summary>
        /// ���ش��ڷ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
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
        /// ����С�ڷ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
        public static bool operator <(LongInt a, LongInt b)
        {
            return (b > a);
        }
        #endregion

        #region public static bool operator >=(LongInt a , LongInt b)
        /// <summary>
        /// ���ش��ڵ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
        public static bool operator >=(LongInt a, LongInt b)
        {
            return !(b > a);
        }
        #endregion

        #region public static bool operator <=(LongInt a , LongInt b)
        /// <summary>
        /// ���ز����ڷ���
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�жϽ��</returns>
        public static bool operator <=(LongInt a, LongInt b)
        {
            return !(a > b);
        }
        #endregion

        #region public static LongInt operator -(LongInt a)
        /// <summary>
        /// һԪ��,ȡ��
        /// </summary>
        /// <param name="a">����������a</param>
        /// <returns>ȡ�������</returns>
        public static LongInt operator -(LongInt a)
        {
            LongInt liResult = new LongInt(a);
            liResult.Positive = !a.Positive;
            return liResult;
        }
        #endregion

        #region public static LongInt operator +(LongInt a , LongInt b)
        /// <summary>
        /// ����+
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�ӷ����</returns>
        public static LongInt operator +(LongInt a, LongInt b)
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
                //ִ�м���
                return a - (-b);
            }

        }
        #endregion

        #region public static LongInt operator -(LongInt a , LongInt b)
        /// <summary>
        /// ����-
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>���Ľ��</returns>
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
                    //������
                    LongInt result = new LongInt();
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
                        //ȡ�����
                        nInc = System.Math.Abs(nInc) - 1;
                        if (nInc > 0)
                            list.Add(nInc);
                        //���������´���
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

        #region public static LongInt operator *(LongInt a , long b)
        /// <summary>
        /// ����*
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">��������b</param>
        /// <returns>�˵Ľ��</returns>
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
        /// ����*
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�˵Ľ��</returns>
        public static LongInt operator *(LongInt a, LongInt b)
        {
            LongInt liResult = new LongInt(0);
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

        #region private static bool Div(LongInt a , long b , out LongInt liDiv , out long lMod)
        /// <summary>
        /// ִ�г��������س���������
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="lMod">��</param>
        /// <returns>�������FALSE�����򷵻�TRUE</returns>
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
        public static bool bDebug = true;   //����ѭ��Ч�ʵĿ��أ������Div������
        public static int nLoopSum = 0;     //ѭ���ܴ���
        public static int nLoopCount = 0;   //�漰ѭ���ļ������
        public static int nLoopNum = 0;     //��ǰ�����ѭ������
        public static int nLoopNumMax = 0;  //����ѭ������
        public static bool bLoopFlag = false;

        #region private static bool Div_N(LongInt a ,LongInt b , out LongInt liDiv , out LongInt liMod)
        /// <summary>
        /// ִ�г��������س���������������ѭ����
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="liMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_N(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                    //����С����ĸ�󣬽��С�������϶�Ϊ��
                    Div(liTempa2, (long)liTempb2 + 1, out liTmpDiv, out nTmpMod);
                    //����
                    if (liTmpDiv == 0 && a >= b)
                    {
                        //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
        /// ִ�г��������س��������������صݹ飩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="liMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_R(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                //����С����ĸ�󣬽��С�������϶�Ϊ��
                Div(liTempa2, (long)liTempb2 + 1, out liDiv, out nTmpMod);
                //����
                if (liDiv == 0 && a >= b)
                {
                    //����λ����ʱ��Ӧֱ�Ӹ�����=1
                    liDiv = 1;
                }
                liMod = a - b * liDiv;
                if (liMod > 0)
                {
                    if (liMod > b)
                    {
                        //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
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
        /// ִ�г��������س�������������ֵ֮��Ȩ�⣩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="liMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_G(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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
                    //����С����ĸС�������ȷ�����������ܻ���
                    Div(liTempa2, (long)liTempb2, out liTmpDiv, out nTmpMod);
                    //����
                    if (liTmpDiv == 0)
                    {
                        //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
                        //����С����ĸ�󣬽��С�������϶�Ϊ��
                        Div(liTempa2, (long)liTempb2 + 1, out liTmpDiv, out nTmpMod);
                        //����
                        if (liTmpDiv == 0)
                        {
                            //����λ����ʱ��Ӧֱ�Ӹ�����=1
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
        /// ִ�г��������س���������������С��֮�����صݹ飩
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="liMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_2R(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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

                //����2����ѭ������Եݹ飬Ч������1%
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

                    //����С����ĸС������ɴ��С�����������ɸ�
                    Div(liTempa2, (long)liTempb2, out liDiv, out nTmpMod);
                    liMod = a - b * liDiv;
                    if (liMod >= 0)
                    {
                        if (liMod >= b)
                        {
                            liDiv = liDiv + 1;
                            liMod -= b;
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
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
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
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
        /// ִ�г��������س��������������ַ���
        /// </summary>
        /// <param name="a">����</param>
        /// <param name="b">������</param>
        /// <param name="liDiv">��</param>
        /// <param name="liMod">����</param>
        /// <returns>�Ƿ���������</returns>
        private static bool Div_2(LongInt a, LongInt b, out LongInt liDiv, out LongInt liMod)
        {
            int nFastLength = 5;
            //��ȡ�Ĵ���Ϊ�������ֵ,�ٲ���
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

                //����2����ѭ������Եݹ飬Ч������1%
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
                    //����С����ĸС������ɴ��С�����������ɸ�
                    Div(liTempa2, (long)liTempb2, out liDiv, out nTmpMod);
                    liMod = a - b * liDiv;
                    if (liMod >= 0)
                    {
                        if (liMod >= b)
                        {
                            liDiv = liDiv + 1;
                            liMod -= b;
                            //�ݹ����ʣ�ಿ�ݵ���(��������Ĳŵݹ�)
                            LongInt liDivFree;
                            nLoopNum++;
                            Div_2(liMod, b, out liDivFree, out liMod);
                            liDiv += liDivFree;
                        }
                    }
                    else
                    {
                        // ���ַ���������Ч����ʱ������
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
                                //��ʵֵ����liTmpDiv��С����liDiv����֮�䣬���ö��ַ�����
                                LongInt liMidDiv = (liTmpDiv + liDiv) / 2;
                                LongInt liMidMod = a - b * liMidDiv;
                                if (liMidMod < 0)
                                {
                                    //��λ���µ�����Ҫѭ��
                                    liDiv = liMidDiv;
                                    nLoopNum++;
                                }
                                else if (liMidMod < b)
                                {
                                    //�˳�ѭ��
                                    liDiv = liMidDiv;
                                    liMod = liMidMod;
                                }
                                else if (liMidMod == b)
                                {
                                    //�˳�ѭ��
                                    liDiv = liMidDiv + 1;
                                    liMod = 0L;
                                }
                                else
                                {
                                    //��λ���ϵ�����Ҫѭ��
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
        /// ����/
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>����</returns>
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
        /// ����%
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">��������b</param>
        /// <returns>����</returns>
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
        /// ����/
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>����</returns>
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
        /// ����%
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>����</returns>
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
        /// ����������
        /// </summary>
        /// <param name="a">����������a</param>
        /// <param name="b">����������b</param>
        /// <returns>�������</returns>
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
        /// �����С��Լ��
        /// </summary>
        /// <param name="a">��������a</param>
        /// <param name="b">��������b</param>
        /// <returns>��С��Լ��</returns>
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
        /// ȡ����ֵ
        /// </summary>
        /// <param name="a">������</param>
        /// <returns>���ط���Ϊ����������ֵ</returns>
        public static LongInt Abs(LongInt a)
        {
            LongInt liResult = new LongInt(a);
            liResult.Positive = true;
            return liResult;
        }
        #endregion

        #region public static LongInt Xn(long x , int n)
        /// <summary>
        /// ����X^n��
        /// </summary>
        /// <param name="x">����</param>
        /// <param name="n">ָ��</param>
        /// <returns>����������ֵ</returns>
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
        /// ����X^n��
        /// </summary>
        /// <param name="x">����</param>
        /// <param name="n">ָ��</param>
        /// <returns>������ֵ</returns>
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
        /// ����n�Ľ׳�
        /// </summary>
        /// <param name="n">�׳�ϵ��</param>
        /// <returns>��������</returns>
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
        /// ����Cmn=m*(m-1)...*(m-n)/n!
        /// </summary>
        /// <param name="m">����m</param>
        /// <param name="n">ָ��n</param>
        /// <returns>������ֵ</returns>
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
        /// ����Equals
        /// </summary>
        /// <param name="o">ͬ�����</param>
        /// <returns>������ֵ�Ƿ����</returns>
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
        /// �������,�������̫��,����ʡ�Ժű�ʾ
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
        /// ����������ʽ,������8192λʱ,��������(̫����,û������)
        /// </summary>
        /// <param name="bHaveComma">�Ƿ��ѧ��������,</param>
        /// <returns>ֵ�ַ���</returns>
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
        /// ���������̨
        /// </summary>
        /// <param name="bHaveComma">�Ƿ��ѧ��������,</param>
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