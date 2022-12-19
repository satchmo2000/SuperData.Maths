using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SuperData.Maths
{
    /// <summary>
    /// ��Ԫһ�η��̽����㷨
    /// </summary>
    public class MatrixResult
    {
        /// <summary>
        /// ������������ݴ���״̬
        /// </summary>
        protected bool[] bHasResult = null;
        /// <summary>
        /// ��֤��ĸֵ
        /// ����ߴ�Ϊ����ʱ����ֵ��ȣ�Ϊż��ʱ����ֵ+-����
        /// </summary>
        private double[] dDenominatorData = null;
        /// <summary>
        /// ������ݴ�ţ�����ʵʱ����
        /// </summary>
        private double[,] dResultData = null;
        
        /// <summary>
        /// �洢ԭʼ��������
        /// </summary>
        private double[,] dMatrixData = null;

        /// <summary>
        /// �洢�������Ĳ�������
        /// </summary>
        private double[,] dReverseMatrixData = null;

        /// <summary>
        /// �洢�м���ʱ����
        /// </summary>
        private Services.KeyStorage storage = null;

        /// <summary>
        /// ��Ԫһ�η��̹���
        /// <param name="nMatrixSize">�м�洢�����ݷ�����</param>
        /// </summary>
        public MatrixResult(int nMatrixSize)
        {
            storage = new SuperData.Services.KeyStorage(nMatrixSize);
        }

        /// <summary>
        /// ��Ԫһ�η��̹���
        /// <param name="nMatrixSize">�м�洢�����ݷ�����</param>
        /// <param name="strSQLConnection">�������ݿ��м�洢ʱ��SQL���Ӵ�</param>
        /// </summary>
        public MatrixResult(int nMatrixSize , string strSQLConnection)
        {
            storage = new SuperData.Services.KeyStorage(nMatrixSize , strSQLConnection);
        }

        /// <summary>
        /// �ر��м�洢��
        /// </summary>
        public void CloseStorage()
        {
            storage.CloseConnect();
        }

        #region ������任����
        /// <summary>
        /// ��ʼ���������󣬲����������������
        /// ���Ż������Ѿ�������ľ������HASH�洢���Խ����ظ�����
        /// </summary>
        /// <param name="data"></param>
        public void InitBaseMatrix(double[,] data)
        {
            InitBaseMatrix(data, 1);
        }

        /// <summary>
        /// ��ʼ���������󣬲����������������
        /// ���Ż������Ѿ�������ľ������HASH�洢���Խ����ظ�����
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nResultGroupCount">���������</param>
        public void InitBaseMatrix(double[,] data , int nResultGroupCount)
        {
            storage.ClearKey();
            
            int nRowCount = data.GetUpperBound(0) + 1;
            int nColCount = data.GetUpperBound(1) + 1;
            if (nRowCount > 0 && nColCount >= nRowCount)
            {
                int nSize = nRowCount;
                dMatrixData = new double[nSize, nSize];
                dReverseMatrixData = new double[nSize, nSize];
                string[,] strTableDesc = new string[nSize, nSize];

                for (int row = 0; row < nSize; row++)
                {
                    for (int col = 0; col < nSize; col++)
                    {
                        dMatrixData[row , col] = data[row , col];
                        strTableDesc[row, col] = string.Format("{0:D4}", row * nSize + col);
                    }
                }
                for (int row = 0; row < nSize; row++)
                {
                    for (int col = 0; col < nSize; col++)
                    {
                        Console.WriteLine("[{0}]row={1},col={2}", DateTime .Now .ToLongTimeString () , row, col);
                        
                        double[,] dSubVars = new double[nSize - 1, nSize - 1];
                        string[,] strSubTableDesc = new string[nSize - 1, nSize - 1];
                        for (int i = 0; i < nSize - 1; i++)
                        {
                            for (int j = 0; j < nSize - 1; j++)
                            {
                                dSubVars[i, j] = dMatrixData[(row + 1 + i) % nSize, (col + 1 + j) % nSize];
                                strSubTableDesc[i, j] = strTableDesc[(row + 1 + i) % nSize, (col + 1 + j) % nSize];
                            }
                        }
                        dReverseMatrixData[row, col] = MakeDouble(dSubVars, nSize - 1, 0, false , strSubTableDesc);
                    }
                }
                dDenominatorData = new double[nSize];
                dResultData = new double[nSize, nResultGroupCount];
                for (int col = 0; col < nSize; col++)
                {
                    for (int rgi = 0; rgi < nResultGroupCount; rgi++)
                    {
                        dResultData[col, rgi] = 0;
                    }
                    dDenominatorData[col] = 0;
                    for (int row = 0; row < nSize; row++)
                    {
                        if (nSize % 2 == 0 && row % 2 == 1)
                            dDenominatorData[col] -= dMatrixData[row, col] * dReverseMatrixData[row, col];
                        else
                            dDenominatorData[col] += dMatrixData[row, col] * dReverseMatrixData[row, col];
                    }
                    if (dDenominatorData[col] == 0)
                    {
                        throw new Exception(string.Format ("�����д�����ֵ��ĸ���޷��������㣨���ڿɻ�����ʽ�����к�={0}��" , col + 1));
                    }
                }
            }
            else
            {
                throw new Exception("������������С��������ʵ��ֻȡn*n�ľ������ݣ�");
            }
        }

        /// <summary>
        /// ��ȡָ���еı���ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <returns></returns>
        public double GetValue(int nCalCol)
        {
            return GetValue(nCalCol, 0);
        }

        /// <summary>
        /// ��ȡָ���еı���ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <param name="nRGIndex">�����</param>
        /// <returns></returns>
        public double GetValue(int nCalCol , int nRGIndex)
        {
            int nResultGroupCount = dResultData.GetUpperBound(1) + 1;
            if (dResultData != null && nCalCol <= dResultData.GetUpperBound(0) && nRGIndex < nResultGroupCount)
            {
                int nSize = dResultData.GetUpperBound(0) + 1;

                double dMolecule = 0;
                for (int row = 0; row < nSize; row++)
                {
                    if (nSize % 2 == 0 && row % 2 == 1)
                    {
                        dMolecule -= dReverseMatrixData[row, nCalCol] * dResultData[row, nRGIndex];
                    }
                    else
                    {
                        dMolecule += dReverseMatrixData[row, nCalCol] * dResultData[row, nRGIndex];
                    }
                }
                //����������һ��
                double dResult = (dMolecule / dDenominatorData[nCalCol]);
                return dResult;
            }
            else
            {
                throw new Exception("�������޳�ʼ���ݻ򳬳��߽磡");
            }
        }

        /// <summary>
        /// ��ȡָ���еĽ��ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <returns></returns>
        public double GetResult(int nCalCol)
        {
            return GetResult(nCalCol, 0);
        }

        /// <summary>
        /// ��ȡָ���еĽ��ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <param name="nRGIndex">�����</param>
        /// <returns></returns>
        public double GetResult(int nCalCol, int nRGIndex)
        {
            int nResultGroupCount = dResultData.GetUpperBound(1) + 1;
            if (dResultData != null && nCalCol <= dResultData.GetUpperBound(0) && nRGIndex < nResultGroupCount)
            {
                return dResultData[nCalCol, nRGIndex];
            }
            else
            {
                throw new Exception("�������޳�ʼ���ݻ򳬳��߽磡");
            }
        }

        /// <summary>
        /// ��̬������ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <param name="dResult">ֵ</param>
        public void SetResult(int nCalCol, double dResult)
        {
            SetResult(nCalCol, 0, dResult);
        }

        /// <summary>
        /// ��̬������ֵ
        /// </summary>
        /// <param name="nCalCol">��</param>
        /// <param name="nRGIndex">�����</param>
        /// <param name="dResult">ֵ</param>
        public void SetResult(int nCalCol, int nRGIndex, double dResult)
        {
            int nSize = dResultData.GetUpperBound(0) + 1;
            int nResultGroupCount = dResultData .GetUpperBound (1) + 1;
            if (dResultData != null && nCalCol < nSize && nRGIndex < nResultGroupCount)
            {
                dResultData[nCalCol , nRGIndex] = dResult;
                if (bHasResult != null && !bHasResult [nRGIndex ])
                {
                    bHasResult[nRGIndex] = true;
                    int nNextRGIndex = (nRGIndex + (nResultGroupCount / 2)) % nResultGroupCount;
                    if (bHasResult[nNextRGIndex])
                    {
                        bHasResult[nNextRGIndex] = false;
                        for (int i = 0; i < nSize; i++)
                        {
                            dResultData[i, nNextRGIndex] = 0;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("�������޳�ʼ���ݻ򳬳��߽磡");
            }
        }

        /// <summary>
        /// ��̬�������н��ֵ
        /// </summary>
        /// <param name="dResult"></param>
        public void SetResult(double[] dResult)
        {
            SetResult(0, dResult);
        }

        /// <summary>
        /// ��̬�������н��ֵ
        /// </summary>
        /// <param name="nRGIndex">�����</param>
        /// <param name="dResult"></param>
        public void SetResult(int nRGIndex, double[] dResult)
        {
            int nResultGroupCount = dResultData.GetUpperBound(1) + 1;
            if (dResultData != null && dResult.GetUpperBound(0) <= dResultData.GetUpperBound(0) && nRGIndex < nResultGroupCount)
            {
                int nSize = dResultData.GetUpperBound(0) + 1;
                for (int row = 0; row < nSize; row++)
                {
                    dResultData[row, nRGIndex] = dResult[row];
                }
            }
            else
            {
                throw new Exception("�������޳�ʼ���ݻ򳬳��߽磡");
            }
        }

        /// <summary>
        /// ���������������������
        /// </summary>
        /// <param name="dResult"></param>
        /// <returns></returns>
        public double[] MakeMatrix(double[] dResult)
        {
            int nLength = dResult.Length;
            int nSize = dMatrixData.GetUpperBound(0) + 1;
            if (nLength == nSize)
            {
                double[] dVars = new double[nSize];
                for (int col = 0; col < nSize; col++)
                {
                    double dMolecule = 0;
                    double dDenominator = 0;
                    for (int row = 0; row < nSize; row++)
                    {
                        if (nSize % 2 == 0 && row % 2 == 1)
                        {
                            dMolecule -= dReverseMatrixData[row, col] * dResult[row];
                            dDenominator -= dReverseMatrixData[row, col] * dMatrixData[row, col];
                        }
                        else
                        {
                            dMolecule += dReverseMatrixData[row, col] * dResult[row];
                            dDenominator += dReverseMatrixData[row, col] * dMatrixData[row, col];
                        }
                    }
                    //����������һ��
                    //dVars[col] = dMolecule / dDenominator;
                    dVars[col] = dMolecule / dDenominatorData[col];
                }
                return dVars;
            }
            else
            {
                throw new Exception("������������������ʼ���Ĳ�һ�£��޷����м��㣡");
            }
        }

        /// <summary>
        /// ����������������ֱ�������������
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] MakeMatrixD(double[,] data)
        {
            int nRowCount = data.GetUpperBound(0) + 1;
            int nColCount = data.GetUpperBound(1) + 1;
            if (nRowCount > 0 && nRowCount < nColCount)
            {
                int nSize = nRowCount ;
                InitBaseMatrix(data , 1);
                double[] dResult = new double[nSize];
                for (int i = 0; i < nSize; i++)
                {
                    dResult[i] = data[i, nSize];
                }
                return MakeMatrix(dResult);
            }
            else
            {
                throw new Exception("������������С��������ʵ��ֻȡn*(n+1)�ľ������ݣ�");
            }

        }

        /// <summary>
        /// ����������������ֱ������������󣨸�˹��Ԫ����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] MakeMatrix_GS(double[,] data)
        {
            int nRowCount = data.GetUpperBound(0) + 1;
            int nColCount = data.GetUpperBound(1) + 1;
            if (nRowCount > 0 && nRowCount < nColCount)
            {
                int nSize = nRowCount;
                //������
                for (int k = 0; k < nSize; k++)
                {
                    for (int i = k; i < nSize; i++)
                    {
                        double dRefData = data[i, k];
                        for (int j = k; j < nSize + 1; j++)
                        {
                            data[i, j] /= dRefData;
                            if (i != k)
                                data[i, j] -= data[k, j];
                        }
                    }
                }
                //������
                for (int k = 0; k < nSize ; k++)
                {
                    int nk = (nSize - 1) - k;
                    for (int i = k; i < nSize; i++)
                    {
                        int ni = (nSize - 1) - i;
                        double dRefData = data[ni, nk];
                        for (int j = k; j <= i + 1; j++)
                        {
                            int nj = (nSize - 1) - j;
                            if(nj < ni)
                                nj =nSize ;
                            data[ni, nj] /= dRefData;
                            if (ni != nk)
                                data[ni, nj] -= data[nk, nj];
                        }
                    }
                }
                double[] dResult = new double[nSize];
                for (int i = 0; i < nSize; i++)
                {
                    dResult[i] = data[i, nSize];
                }
                return dResult;
            }
            else
            {
                throw new Exception("������������С��������ʵ��ֻȡn*(n+1)�ľ������ݣ�");
            }

        }
        #endregion

        #region ����ά��=nSize�ľ����nCalCol�еı���ֵ��Ӧ�ı��ʽ
        /// <summary>
        /// ����ά��=nSize�ľ����nCalCol�еı���ֵ��Ӧ�ı��ʽ
        /// ������ϵͳ�ṩģ��
        /// </summary>
        /// <param name="nSize">ά��</param>
        /// <param name="nCalCol">������0��ά��ֵ</param>
        /// <returns></returns>
        public string MakeCommon(int nSize, int nCalCol)
        {
            //��ʼ������
            object[,] strVars = new object[nSize, nSize + 1];
            for (int i = 0; i < nSize; i++)
            {
                for (int j = 0; j < nSize ; j++)
                {
                    strVars[i, j] = string.Format("{0}{1}", i + 1, j + 1);
                }
                strVars[i, nSize] = string.Format("{0}R", i + 1);
            }
            return string.Format ("({0})/({1})" ,  
                MakeExpress(strVars, nSize , nCalCol , true) ,
                MakeExpress(strVars, nSize ,nCalCol , false));
        }

        /// <summary>
        /// ���ھ�������strVars�������nCalCol�еı���ֵ��Ӧ����ֵ���ʽ
        /// </summary>
        /// <param name="strVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nCalCol">������0��nSize</param>
        /// <returns></returns>
        public string MakeExpress(object[,] strVars, int nCalCol)
        {
            int nRowCount = strVars.GetUpperBound(0) + 1;
            int nColCount = strVars.GetUpperBound(1) + 1;
            if (nColCount - nRowCount == 1)
            {
                return string.Format("({0})/({1})",
                    MakeExpress(strVars, nRowCount, nCalCol, true),
                    MakeExpress(strVars, nRowCount, nCalCol, false));
            }
            else
            {
                throw new Exception("�������n��n+1�У����һ��Ϊ����洢�У�");
            }
        }

        /// <summary>
        /// ��nCalCol�еı���ֵ��Ӧ���ʽ
        /// nRightValue = true  ���ӱ��ʽ
        /// nRightValue = false ��ĸ���ʽ
        /// </summary>
        /// <param name="strVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nSize">ά����ʵ���Ͽɼ���</param>
        /// <param name="nCalCol">������</param>
        /// <param name="bRightValue">�Ƿ�Ϊ���һ�У����Ƿ�Ϊ�����</param>
        /// <returns></returns>
        private string MakeExpress(object[,] strVars, int nSize, int nCalCol, bool bRightValue)
        {
            if (nSize > 1)
            {
                string strRet = string.Empty;
                for (int n = 0; n < nSize; n++)
                {
                    object[,] strSubVars = new object[nSize - 1 , nSize];
                    for(int i = 0;i < nSize - 1;i ++)
                    {
                        for(int j = 0;j < nSize - 1;j ++)
                        {
                            strSubVars [i , j] = strVars [(i + n + 1 + nSize) % nSize , (j + nCalCol + 1) % nSize];
                        }
                        strSubVars[i, nSize - 1] = strVars[(i + n + 1 + nSize) % nSize, nCalCol];
                    }
                    //ż��ʱ+-���棬����ʱ++����
                    if (nSize % 2 == 0 && n % 2 == 1)
                        strRet += "-";
                    else if(n != 0)
                        strRet += "+";
                    if (nSize == 2)
                    {
                        if (bRightValue)
                            strRet += string.Format("{0}*{1}", strVars[n, nSize], MakeExpress(strSubVars, nSize - 1, 0 , false));
                        else
                            strRet += string.Format("{0}*{1}", strVars[n, nCalCol], MakeExpress(strSubVars, nSize - 1, 0, false));
                    }
                    else
                    {
                        if (bRightValue)
                            strRet += string.Format("{0}*({1})", strVars[n, nSize], MakeExpress(strSubVars, nSize - 1, 0, false));
                        else
                            strRet += string.Format("{0}*({1})", strVars[n, nCalCol], MakeExpress(strSubVars, nSize - 1, 0, false));
                    }
                }
                return strRet;
            }
            else
            {
                if (bRightValue)
                    return strVars[0, 1].ToString();
                else
                    return strVars[0, 0].ToString();
            }
        }
        #endregion

        #region ��Double���ͼ�����ֵ
        /// <summary>
        /// ��Double���ͼ�����ֵ
        /// </summary>
        /// <param name="dVars">����������</param>
        /// <param name="nCalCol">������</param>
        /// <returns></returns>
        public double MakeDouble(double[,] dVars, int nCalCol)
        {
            int nRowCount = dVars.GetUpperBound(0) + 1;
            int nColCount = dVars.GetUpperBound(1) + 1;
            if (nColCount - nRowCount == 1)
            {
                return MakeDouble(dVars, nRowCount, nCalCol, true) / MakeDouble(dVars, nRowCount, nCalCol, false);
            }
            else
            {
                throw new Exception("�������n��n+1�У����һ��Ϊ����洢�У�");
            }
        }

        /// <summary>
        /// ��nCalCol�еı���ֵ��Ӧ���ʽ
        /// nRightValue = true  ���ӱ��ʽ
        /// nRightValue = false ��ĸ���ʽ
        /// </summary>
        /// <param name="dVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nSize">ά����ʵ���Ͽɼ���</param>
        /// <param name="nCalCol">������</param>
        /// <param name="bRightValue">�Ƿ�Ϊ���һ�У����Ƿ�Ϊ�����</param>
        /// <returns></returns>
        private double MakeDouble(double[,] dVars, int nSize, int nCalCol, bool bRightValue)
        {
            if (nSize > 1)
            {
                double dRet = 0;
                for (int n = 0; n < nSize; n++)
                {
                    /*
                    double[,] dSubVars = new double[nSize - 1, nSize];
                    for (int i = 0; i < nSize - 1; i++)
                    {
                        for (int j = 0; j < nSize - 1; j++)
                        {
                            dSubVars[i, j] = dVars[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                        }
                        dSubVars[i, nSize - 1] = dVars[(i + n + 1 + nSize) % nSize, nCalCol];
                    }
                    */
                    double[,] dSubVars = MakeSubTable<double>(dVars, nCalCol, n);
                    double dOne = 0;
                    if (nSize == 2)
                    {
                        //���Ժ���nSize == 2���жϣ�����һ��һ��
                        if (bRightValue)
                            dOne = dVars[n, nSize] * MakeDouble(dSubVars, nSize - 1, 0, false);
                        else
                            dOne = dVars[n, nCalCol] * MakeDouble(dSubVars, nSize - 1, 0, false);
                    }
                    else
                    {
                        if (bRightValue)
                            dOne = dVars[n, nSize] * MakeDouble(dSubVars, nSize - 1, 0, false);
                        else
                            dOne = dVars[n, nCalCol] * MakeDouble(dSubVars, nSize - 1, 0, false);
                    }

                    //ż��ʱ+-���棬����ʱ++����
                    if (nSize % 2 == 0 && n % 2 == 1)
                        dRet -= dOne;
                    else
                        dRet += dOne;
                }
                return dRet;
            }
            else
            {
                if(bRightValue )
                    return dVars[0, 1];
                else
                    return dVars[0, 0];
            }
        }

        /// <summary>
        /// ��nCalCol�еı���ֵ��Ӧ���ʽ
        /// nRightValue = true  ���ӱ��ʽ
        /// nRightValue = false ��ĸ���ʽ
        /// </summary>
        /// <param name="dVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nSize">ά����ʵ���Ͽɼ���</param>
        /// <param name="nCalCol">������</param>
        /// <param name="bRightValue">�Ƿ�Ϊ���һ�У����Ƿ�Ϊ�����</param>
        /// <param name="strTable">��������</param>
        /// <returns></returns>
        private double MakeDouble(double[,] dVars, int nSize, int nCalCol, bool bRightValue , string [,] strTable)
        {
            if (nSize > 1)
            {
                //�������㵥Ԫ�ĵ�һ����Ԫ����з���洢��������SortListģʽ��
                int nGroupId;
                string strTextValue = Table2String(strTable, out nGroupId);
                string strKeyId = storage.CreateKey(strTextValue);

                //string strKeyId = Table2String(strTable, out nGroupId);
                //string strTextValue = storage.CreateKey(strKeyId);
                
                double dResult = 0;
                if (storage.GetValue(strKeyId, out dResult, nGroupId))
                    return dResult;

                double dRet = 0;
                for (int n = 0; n < nSize; n++)
                {
                    double[,] dSubVars = new double[nSize - 1, nSize];
                    string[,] strSubTableDesc = new string[nSize - 1, nSize - 1];
                    for (int i = 0; i < nSize - 1; i++)
                    {
                        for (int j = 0; j < nSize - 1; j++)
                        {
                            dSubVars[i, j] = dVars[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                            strSubTableDesc[i, j] = strTable[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                        }
                        dSubVars[i, nSize - 1] = dVars[(i + n + 1 + nSize) % nSize, nCalCol];
                    }
                    double dOne = 0;
                    if (bRightValue)
                        dOne = dVars[n, nSize] * MakeDouble(dSubVars, nSize - 1, 0, false, strSubTableDesc);
                    else
                        dOne = dVars[n, nCalCol] * MakeDouble(dSubVars, nSize - 1, 0, false, strSubTableDesc);

                    //ż��ʱ+-���棬����ʱ++����
                    if (nSize % 2 == 0 && n % 2 == 1)
                        dRet -= dOne;
                    else
                        dRet += dOne;
                }

                storage.InsertKey(strKeyId, dRet , strTextValue , nGroupId);

                return dRet;
            }
            else
            {
                if (bRightValue)
                    return dVars[0, 1];
                else
                    return dVars[0, 0];
            }
        }

        /// <summary>
        /// ���������������˳�����
        /// </summary>
        /// <param name="strTable"></param>
        /// <param name="nGroupId">��������ID����ʼ����Ϊ���ݣ�</param>
        /// <returns></returns>
        private string Table2String(string[,] strTable, out int nGroupId)
        {
            nGroupId = int.Parse(strTable[0, 0]);
            string strRet = string.Empty ;
            //for (int i = strTable.GetLowerBound(0); i <= strTable.GetUpperBound(0); i++)
            //{
            //    for (int j = strTable.GetLowerBound(1); j <= strTable.GetUpperBound(1); j++)
            //    {
            //        strRet += strTable[i, j];
            //    }
            //}
            for (int i = strTable.GetLowerBound(0); i <= strTable.GetUpperBound(0); i++)
            {
                strRet += strTable[i, i];
            }
            return strRet;
        }
        #endregion

        #region ��Fraction���ͼ�����ֵ
        /// <summary>
        /// ��Fraction���ͼ�����ֵ
        /// </summary>
        /// <param name="dVars">����������</param>
        /// <param name="nCalCol">������</param>
        /// <returns></returns>
        public Fraction MakeFraction(Fraction[,] dVars, int nCalCol)
        {
            int nRowCount = dVars.GetUpperBound(0) + 1;
            int nColCount = dVars.GetUpperBound(1) + 1;
            if (nColCount - nRowCount == 1)
            {
                return MakeFraction(dVars, nRowCount, nCalCol, true) / MakeFraction(dVars, nRowCount, nCalCol, false);
            }
            else
            {
                throw new Exception("�������n��n+1�У����һ��Ϊ����洢�У�");
            }
        }

        /// <summary>
        /// ��nCalCol�еı���ֵ��Ӧ���ʽ
        /// nRightValue = true  ���ӱ��ʽ
        /// nRightValue = false ��ĸ���ʽ
        /// </summary>
        /// <param name="dVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nSize">ά����ʵ���Ͽɼ���</param>
        /// <param name="nCalCol">������</param>
        /// <param name="bRightValue">�Ƿ�Ϊ���һ�У����Ƿ�Ϊ�����</param>
        /// <returns></returns>
        private Fraction MakeFraction(Fraction[,] dVars, int nSize, int nCalCol, bool bRightValue)
        {
            if (nSize > 1)
            {
                Fraction dRet = 0;
                for (int n = 0; n < nSize; n++)
                {
                    /*
                    Fraction[,] dSubVars = new Fraction[nSize - 1, nSize];
                    for (int i = 0; i < nSize - 1; i++)
                    {
                        for (int j = 0; j < nSize - 1; j++)
                        {
                            dSubVars[i, j] = dVars[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                        }
                        dSubVars[i, nSize - 1] = dVars[(i + n + 1 + nSize) % nSize, nCalCol];
                    }
                    */
                    Fraction[,] dSubVars = MakeSubTable<Fraction>(dVars, nCalCol, n);

                    Fraction dOne = 0;
                    if (nSize == 2)
                    {
                        //���Ժ���nSize == 2���жϣ�����һ��һ��
                        if (bRightValue)
                            dOne = dVars[n, nSize] * MakeFraction(dSubVars, nSize - 1, 0, false);
                        else
                            dOne = dVars[n, nCalCol] * MakeFraction(dSubVars, nSize - 1, 0, false);
                    }
                    else
                    {
                        if (bRightValue)
                            dOne = dVars[n, nSize] * MakeFraction(dSubVars, nSize - 1, 0, false);
                        else
                            dOne = dVars[n, nCalCol] * MakeFraction(dSubVars, nSize - 1, 0, false);
                    }

                    //ż��ʱ+-���棬����ʱ++����
                    if (nSize % 2 == 0 && n % 2 == 1)
                        dRet -= dOne;
                    else
                        dRet += dOne;
                }
                return dRet;
            }
            else
            {
                if (bRightValue)
                    return dVars[0, 1];
                else
                    return dVars[0, 0];
            }
        }

        /// <summary>
        /// ��nCalCol�еı���ֵ��Ӧ���ʽ
        /// nRightValue = true  ���ӱ��ʽ
        /// nRightValue = false ��ĸ���ʽ
        /// </summary>
        /// <param name="dVars">�����������һ��Ϊ���ֵ</param>
        /// <param name="nSize">ά����ʵ���Ͽɼ���</param>
        /// <param name="nCalCol">������</param>
        /// <param name="bRightValue">�Ƿ�Ϊ���һ�У����Ƿ�Ϊ�����</param>
        /// <param name="strTable">��������</param>
        /// <returns></returns>
        private Fraction MakeFraction(Fraction[,] dVars, int nSize, int nCalCol, bool bRightValue, string[,] strTable)
        {
            if (nSize > 1)
            {

                Fraction dRet = 0;
                for (int n = 0; n < nSize; n++)
                {
                    Fraction[,] dSubVars = new Fraction[nSize - 1, nSize];
                    string[,] strSubTableDesc = new string[nSize - 1, nSize - 1];
                    for (int i = 0; i < nSize - 1; i++)
                    {
                        for (int j = 0; j < nSize - 1; j++)
                        {
                            dSubVars[i, j] = dVars[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                            strSubTableDesc[i, j] = strTable[(i + n + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                        }
                        dSubVars[i, nSize - 1] = dVars[(i + n + 1 + nSize) % nSize, nCalCol];
                    }
                    Fraction dOne = 0;
                    if (bRightValue)
                        dOne = dVars[n, nSize] * MakeFraction(dSubVars, nSize - 1, 0, false, strSubTableDesc);
                    else
                        dOne = dVars[n, nCalCol] * MakeFraction(dSubVars, nSize - 1, 0, false, strSubTableDesc);

                    //ż��ʱ+-���棬����ʱ++����
                    if (nSize % 2 == 0 && n % 2 == 1)
                        dRet -= dOne;
                    else
                        dRet += dOne;
                }

                return dRet;
            }
            else
            {
                if (bRightValue)
                    return dVars[0, 1];
                else
                    return dVars[0, 0];
            }
        }

        #endregion

        /// <summary>
        /// �Ӿ����ж�ȡָ����Ԫ��Ӧ���Ӿ���
        /// </summary>
        /// <typeparam name="T">��Ԫ������</typeparam>
        /// <param name="dVars">n*(n+1)����</param>
        /// <param name="nCalCol">������</param>
        /// <param name="nCal">ָ����</param>
        /// <returns></returns>
        private T[,] MakeSubTable<T>(T[,] dVars, int nCalCol, int nCal) where T : new()
        {
            int nRowCount = dVars.GetUpperBound(0) + 1;
            if (nRowCount > 0)
            {
                int nColCount = dVars.GetUpperBound(1) + 1;
                if (nColCount - nRowCount == 1)
                {
                    int nSize = nRowCount;
                    T[,] dSubVars = new T[nSize - 1, nSize];
                    for (int i = 0; i < nSize - 1; i++)
                    {
                        for (int j = 0; j < nSize - 1; j++)
                        {
                            dSubVars[i, j] = dVars[(i + nCal + 1 + nSize) % nSize, (j + nCalCol + 1) % nSize];
                        }
                        dSubVars[i, nSize - 1] = dVars[(i + nCal + 1 + nSize) % nSize, nCalCol];
                    }
                    return dSubVars;
                }
            }
            throw new Exception("�������n��n+1�У����һ��Ϊ����洢�У�");
        }

    }
}
