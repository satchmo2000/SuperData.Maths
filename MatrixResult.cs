using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SuperData.Maths
{
    /// <summary>
    /// 多元一次方程解题算法
    /// </summary>
    public class MatrixResult
    {
        /// <summary>
        /// 保存结果组的数据存在状态
        /// </summary>
        protected bool[] bHasResult = null;
        /// <summary>
        /// 验证分母值
        /// 矩阵尺寸为奇数时，数值相等，为偶数时，数值+-交替
        /// </summary>
        private double[] dDenominatorData = null;
        /// <summary>
        /// 结果数据存放，用于实时运算
        /// </summary>
        private double[,] dResultData = null;
        
        /// <summary>
        /// 存储原始参数矩阵
        /// </summary>
        private double[,] dMatrixData = null;

        /// <summary>
        /// 存储逆向计算的参数矩阵
        /// </summary>
        private double[,] dReverseMatrixData = null;

        /// <summary>
        /// 存储中间临时数据
        /// </summary>
        private Services.KeyStorage storage = null;

        /// <summary>
        /// 多元一次方程构造
        /// <param name="nMatrixSize">中间存储的数据分组数</param>
        /// </summary>
        public MatrixResult(int nMatrixSize)
        {
            storage = new SuperData.Services.KeyStorage(nMatrixSize);
        }

        /// <summary>
        /// 多元一次方程构造
        /// <param name="nMatrixSize">中间存储的数据分组数</param>
        /// <param name="strSQLConnection">采用数据库中间存储时的SQL连接串</param>
        /// </summary>
        public MatrixResult(int nMatrixSize , string strSQLConnection)
        {
            storage = new SuperData.Services.KeyStorage(nMatrixSize , strSQLConnection);
        }

        /// <summary>
        /// 关闭中间存储器
        /// </summary>
        public void CloseStorage()
        {
            storage.CloseConnect();
        }

        #region 矩阵逆变换服务
        /// <summary>
        /// 初始化变量矩阵，并生成逆向运算矩阵
        /// 待优化，将已经计算过的矩阵进行HASH存储，以降低重复运算
        /// </summary>
        /// <param name="data"></param>
        public void InitBaseMatrix(double[,] data)
        {
            InitBaseMatrix(data, 1);
        }

        /// <summary>
        /// 初始化变量矩阵，并生成逆向运算矩阵
        /// 待优化，将已经计算过的矩阵进行HASH存储，以降低重复运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nResultGroupCount">结果分组数</param>
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
                        throw new Exception(string.Format ("矩阵中存在零值分母，无法继续计算（存在可化解表达式），列号={0}！" , col + 1));
                    }
                }
            }
            else
            {
                throw new Exception("矩阵行数必须小于列数，实际只取n*n的矩阵内容！");
            }
        }

        /// <summary>
        /// 获取指定列的变量值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <returns></returns>
        public double GetValue(int nCalCol)
        {
            return GetValue(nCalCol, 0);
        }

        /// <summary>
        /// 获取指定列的变量值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <param name="nRGIndex">结果组</param>
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
                //下面两句结果一致
                double dResult = (dMolecule / dDenominatorData[nCalCol]);
                return dResult;
            }
            else
            {
                throw new Exception("矩阵中无初始数据或超出边界！");
            }
        }

        /// <summary>
        /// 获取指定列的结果值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <returns></returns>
        public double GetResult(int nCalCol)
        {
            return GetResult(nCalCol, 0);
        }

        /// <summary>
        /// 获取指定列的结果值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <param name="nRGIndex">结果组</param>
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
                throw new Exception("矩阵中无初始数据或超出边界！");
            }
        }

        /// <summary>
        /// 动态置入结果值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <param name="dResult">值</param>
        public void SetResult(int nCalCol, double dResult)
        {
            SetResult(nCalCol, 0, dResult);
        }

        /// <summary>
        /// 动态置入结果值
        /// </summary>
        /// <param name="nCalCol">列</param>
        /// <param name="nRGIndex">结果组</param>
        /// <param name="dResult">值</param>
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
                throw new Exception("矩阵中无初始数据或超出边界！");
            }
        }

        /// <summary>
        /// 动态置入所有结果值
        /// </summary>
        /// <param name="dResult"></param>
        public void SetResult(double[] dResult)
        {
            SetResult(0, dResult);
        }

        /// <summary>
        /// 动态置入所有结果值
        /// </summary>
        /// <param name="nRGIndex">结果组</param>
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
                throw new Exception("矩阵中无初始数据或超出边界！");
            }
        }

        /// <summary>
        /// 输入结果矩阵求出变量矩阵
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
                    //下面两句结果一致
                    //dVars[col] = dMolecule / dDenominator;
                    dVars[col] = dMolecule / dDenominatorData[col];
                }
                return dVars;
            }
            else
            {
                throw new Exception("结果（参数）矩阵与初始化的不一致，无法进行计算！");
            }
        }

        /// <summary>
        /// 输入参数及结果矩阵直接求出变量矩阵
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
                throw new Exception("矩阵行数必须小于列数，实际只取n*(n+1)的矩阵内容！");
            }

        }

        /// <summary>
        /// 输入参数及结果矩阵直接求出变量矩阵（高斯消元法）
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
                //下三角
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
                //上三角
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
                throw new Exception("矩阵行数必须小于列数，实际只取n*(n+1)的矩阵内容！");
            }

        }
        #endregion

        #region 计算维数=nSize的矩阵第nCalCol列的变量值相应的表达式
        /// <summary>
        /// 计算维数=nSize的矩阵第nCalCol列的变量值相应的表达式
        /// 变量由系统提供模拟
        /// </summary>
        /// <param name="nSize">维度</param>
        /// <param name="nCalCol">计算列0～维度值</param>
        /// <returns></returns>
        public string MakeCommon(int nSize, int nCalCol)
        {
            //初始化变量
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
        /// 对于矩阵数据strVars，计算第nCalCol列的变量值相应的数值表达式
        /// </summary>
        /// <param name="strVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nCalCol">计算列0～nSize</param>
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
                throw new Exception("矩阵必须n行n+1列，最后一列为结果存储列！");
            }
        }

        /// <summary>
        /// 求nCalCol列的变量值相应表达式
        /// nRightValue = true  分子表达式
        /// nRightValue = false 分母表达式
        /// </summary>
        /// <param name="strVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nSize">维数，实际上可计算</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="bRightValue">是否为最后一列，即是否为结果列</param>
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
                    //偶数时+-交替，奇数时++连续
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

        #region 按Double类型计算结果值
        /// <summary>
        /// 按Double类型计算结果值
        /// </summary>
        /// <param name="dVars">浮点矩阵变量</param>
        /// <param name="nCalCol">计算列</param>
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
                throw new Exception("矩阵必须n行n+1列，最后一列为结果存储列！");
            }
        }

        /// <summary>
        /// 求nCalCol列的变量值相应表达式
        /// nRightValue = true  分子表达式
        /// nRightValue = false 分母表达式
        /// </summary>
        /// <param name="dVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nSize">维数，实际上可计算</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="bRightValue">是否为最后一列，即是否为结果列</param>
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
                        //可以忽略nSize == 2的判断，与下一组一致
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

                    //偶数时+-交替，奇数时++连续
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
        /// 求nCalCol列的变量值相应表达式
        /// nRightValue = true  分子表达式
        /// nRightValue = false 分母表达式
        /// </summary>
        /// <param name="dVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nSize">维数，实际上可计算</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="bRightValue">是否为最后一列，即是否为结果列</param>
        /// <param name="strTable">行列描述</param>
        /// <returns></returns>
        private double MakeDouble(double[,] dVars, int nSize, int nCalCol, bool bRightValue , string [,] strTable)
        {
            if (nSize > 1)
            {
                //根据运算单元的第一个单元格进行分组存储（仅用于SortList模式）
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

                    //偶数时+-交替，奇数时++连续
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
        /// 将表达描述按行列顺序输出
        /// </summary>
        /// <param name="strTable"></param>
        /// <param name="nGroupId">矩阵所属ID（起始坐标为依据）</param>
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

        #region 按Fraction类型计算结果值
        /// <summary>
        /// 按Fraction类型计算结果值
        /// </summary>
        /// <param name="dVars">浮点矩阵变量</param>
        /// <param name="nCalCol">计算列</param>
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
                throw new Exception("矩阵必须n行n+1列，最后一列为结果存储列！");
            }
        }

        /// <summary>
        /// 求nCalCol列的变量值相应表达式
        /// nRightValue = true  分子表达式
        /// nRightValue = false 分母表达式
        /// </summary>
        /// <param name="dVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nSize">维数，实际上可计算</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="bRightValue">是否为最后一列，即是否为结果列</param>
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
                        //可以忽略nSize == 2的判断，与下一组一致
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

                    //偶数时+-交替，奇数时++连续
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
        /// 求nCalCol列的变量值相应表达式
        /// nRightValue = true  分子表达式
        /// nRightValue = false 分母表达式
        /// </summary>
        /// <param name="dVars">变量矩阵，最后一列为结果值</param>
        /// <param name="nSize">维数，实际上可计算</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="bRightValue">是否为最后一列，即是否为结果列</param>
        /// <param name="strTable">行列描述</param>
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

                    //偶数时+-交替，奇数时++连续
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
        /// 从矩阵中读取指定单元对应的子矩阵
        /// </summary>
        /// <typeparam name="T">单元格类型</typeparam>
        /// <param name="dVars">n*(n+1)矩阵</param>
        /// <param name="nCalCol">计算列</param>
        /// <param name="nCal">指定列</param>
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
            throw new Exception("矩阵必须n行n+1列，最后一列为结果存储列！");
        }

    }
}
