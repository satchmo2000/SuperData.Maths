using System;
using SuperData.Maths ;

namespace SuperData.Maths
{
	/// <summary>
	/// ��������,��Ԫ����n*n��ʽ�洢,���ڼ��������ʽ,
	/// </summary>
	public class Matrix
	{
		/// <summary>
		/// �洢��Ԫ,���ڴ洢Object���͵ļ���
		/// </summary>
		private System.Object [,] data;
		
		/// <summary>
		/// �����С,�ɳ�ʼ��ʱ��ֵ
		/// </summary>
		private int nCount = 0;

		#region public int Count
		/// <summary>
		/// ��þ����С
		/// </summary>
		public int Count
		{
			get
			{
				return nCount ;
			}
		}
		#endregion

		#region public System.Object this[int nRow , int nCol]
		/// <summary>
		/// ��Ԫ�����ݷ���
		/// </summary>
		public System.Object this[int nRow , int nCol]
		{
			get
			{
				if(nRow >= nCount || nCol >= nCount)
				{
					throw new System.Exception ("Row or Col is out of range.");
				}
				else
				{
					return data[nRow , nCol];
				}
			}
			set
			{
				if(nRow >= nCount || nCol >= nCount)
				{
					throw new System.Exception ("Row or Col is out of range.");
				}
				else
				{
					data[nRow , nCol] = value;
				}
			}
		}
		#endregion

		#region public System.Object this[int nIndex]
		/// <summary>
		/// ��Ԫ�����ݷ���
		/// </summary>
		public System.Object this[int nIndex]
		{
			get
			{
				if(nIndex >= nCount * nCount)
				{
					throw new System.Exception ("Index is out of range.");
				}
				else
				{
					return data[nIndex / nCount , nIndex % nCount];
				}
			}
			set
			{
				if(nIndex >= nCount * nCount)
				{
					throw new System.Exception ("Index is out of range.");
				}
				else
				{
					data[nIndex / nCount , nIndex % nCount] = value;
				}
			}
		}
		#endregion

		/// <summary>
		/// ��ʼ����������
		/// </summary>
		/// <param name="uSize">�����С</param>
		/// <param name="obj">��ʼ��һ��Ԫ��ֵ����</param>
		public Matrix(int uSize , System.Object obj)
		{
			// 
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			if(uSize == 0)
				uSize = 1;
			data = new System .Object [uSize,uSize];
			for(int i = 0;i < uSize;i ++)
			{
				for(int j = 0;j < uSize;j ++)
				{
					data[i,j] = obj;
				}
			}
			nCount = uSize;
		}

		#region public void RowAdd(int nRow1 , int nRow2)
		/// <summary>
		/// �������,����������һ��
		/// </summary>
		/// <param name="nRow1">��һ���к�</param>
		/// <param name="nRow2">�ڶ����к�</param>
		public void RowAdd(int nRow1 , int nRow2)
		{
			if(nRow1 >= nCount || nRow2 >= nCount)
			{
				throw new System.Exception ("Row is out of range.");
			}
			else
			{
				if(data[0 , 0].GetType ().FullName .Equals ("System.Double"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Double )data[nRow1 , i] + (System.Double )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int64"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int64 )data[nRow1 , i] + (System.Int64 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int32"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int32 )data[nRow1 , i] + (System.Int32 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int16"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int16 )data[nRow1 , i] + (System.Int16 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("SuperData.LongInt"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (LongInt )data[nRow1 , i] + (LongInt )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("SuperData.Fraction"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (Fraction )data[nRow1 , i] + (Fraction )data[nRow2 , i];
					}
				}
				else
				{
					throw new System.Exception ("not valid data type.");
				}
			}
		}
		#endregion

		#region public void RowSub(int nRow1 , int nRow2)
		/// <summary>
		/// �������,����������һ��
		/// </summary>
		/// <param name="nRow1">��һ���к�</param>
		/// <param name="nRow2">�ڶ����к�</param>
		public void RowSub(int nRow1 , int nRow2)
		{
			if(nRow1 >= nCount || nRow2 >= nCount)
			{
				throw new System.Exception ("Row is out of range.");
			}
			else
			{
				if(data[0 , 0].GetType ().FullName .Equals ("System.Double"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Double )data[nRow1 , i] - (System.Double )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int64"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int64 )data[nRow1 , i] - (System.Int64 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int32"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int32 )data[nRow1 , i] - (System.Int32 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("System.Int16"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (System.Int16 )data[nRow1 , i] - (System.Int16 )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("SuperData.LongInt"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (LongInt )data[nRow1 , i] - (LongInt )data[nRow2 , i];
					}
				}
				else if(data[0 , 0].GetType ().FullName .Equals ("SuperData.Fraction"))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow1 , i] = (Fraction )data[nRow1 , i] - (Fraction )data[nRow2 , i];
					}
				}
				else
				{
					throw new System.Exception ("not valid data type.");
				}
			}
		}
		#endregion

		#region public void RowMul(int nRow , System.Object obj)
		/// <summary>
		/// �г˷�����,����������һ��
		/// </summary>
		/// <param name="nRow">��һ���к�</param>
		/// <param name="obj">�ڶ����к�</param>
		public void RowMul(int nRow , System.Object obj)
		{
			if(nRow >= nCount)
				throw new System.Exception ("Index is out of range.");
			else
			{
				if(obj.GetType ().FullName .Equals ("System.Double" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Double )data[nRow , i] * (System.Double )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int64" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int64 )data[nRow , i] * (System.Int64 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int32" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int32 )data[nRow , i] * (System.Int32 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int16" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int16 )data[nRow , i] * (System.Int16 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("SuperData.LongInt" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (LongInt)data[nRow , i] * (LongInt)obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("SuperData.Fraction" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (Fraction )data[nRow , i] * (Fraction)obj;
					}
				}
				else
				{
					throw new System.Exception ("not valid data type.");
				}
			}
		}
		#endregion

		#region public void RowDiv(int nRow , System.Object obj)
		/// <summary>
		/// �г�������,����������һ��
		/// </summary>
		/// <param name="nRow">��һ���к�</param>
		/// <param name="obj">�ڶ����к�</param>
		public void RowDiv(int nRow , System.Object obj)
		{
			if(nRow >= nCount)
				throw new System.Exception ("Index is out of range.");
			else
			{
				if(obj.GetType ().FullName .Equals ("System.Double" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Double )data[nRow , i] / (System.Double )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int64" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int64 )data[nRow , i] / (System.Int64 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int32" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int32 )data[nRow , i] / (System.Int32 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("System.Int16" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (System.Int16 )data[nRow , i] / (System.Int16 )obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("SuperData.LongInt" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (LongInt)data[nRow , i] / (LongInt)obj;
					}
				}
				else if(obj.GetType ().FullName .Equals ("SuperData.Fraction" ))
				{
					for(int i = 0;i < nCount;i ++)
					{
						data[nRow , i] = (Fraction )data[nRow , i] / (Fraction)obj;
					}
				}
				else
				{
					throw new System.Exception ("not valid data type.");
				}
			}
		}
		#endregion

		#region public void CrossReplace()
		/// <summary>
		/// �Խ��û�,�����Ϸ������������·������ݽ����û�
		/// </summary>
		public void CrossReplace()
		{
			for(int i = 0;i < nCount;i ++)
			{
				for(int j = i + 1;j < nCount ;j ++)
				{
					System.Object obj = data[i , j];
					data[i , j] = data[j , i];
					data[j , i] = obj;
				}
			}
		}
		#endregion

	}
}
