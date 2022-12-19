using System;

namespace SuperData.Maths
{

	/// <summary>
	/// FractionHexΪ�����Ĵ洢��ʽ���洢��ԭʼ�ṹΪLongHex�������ɷ��ӣ���ĸ��ɣ���ΪLongHex����
	/// �������һ���������ͣ�ԭʼ���ݿ�����Դ�ںܶ����ͣ��糤���Σ�LongHex�ͣ��ַ�����
	/// �磺
	///		FractionHex fValue = 1000L;
	///		FractionHex fValue = "100,110";
	/// </summary>
	public class FractionHex
	{
		/// <summary>
		/// ����
		/// </summary>
		private LongHex liMolecule = new LongHex (0);

		/// <summary>
		/// ��ĸ
		/// </summary>
		private LongHex liDenominator = new LongHex (1);

		/// <summary>
		/// ���ݷ���,true��ʾΪ��,false��ʾΪ��
		/// </summary>
		private bool bPositive = true;
		
		/// <summary>
		/// ������ӷ����仯���¼�
		/// </summary>
		public event System.EventHandler GeneChanged;

		#region public LongHex Molecule
		/// <summary>
		/// ��������
		/// </summary>
		public LongHex Molecule
		{
			get
			{
				return liMolecule;
			}
			set
			{
				liMolecule = value;
			}
		}
		#endregion

		#region public LongHex Denominator
		/// <summary>
		/// ��ĸ����
		/// </summary>
		public LongHex Denominator
		{
			get
			{
				return liDenominator;
			}
			set
			{
				liDenominator = value;
			}
		}
		#endregion

		#region public bool Positive
		/// <summary>
		/// ��������
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

		#region public String SnapShot
		/// <summary>
		/// �������
		/// </summary>
		public String SnapShot
		{
			get
			{
				String strResult = "";
				if(!Positive && Molecule != 0)
					strResult += "-";
				strResult += Molecule.SnapShot ;
				if(Denominator != 1)
				{
					strResult += "/";
					strResult += Denominator.SnapShot ;
				}
				return strResult;
			}
		}
		#endregion

		#region public int Length
		/// <summary>
		/// ��÷����ĳ���(ռ�ÿռ�)
		/// </summary>
		public int Length
		{
			get
			{
				return Molecule.Length + Denominator.Length ;
			}
		}
		#endregion

		#region public FractionHex()
		/// <summary>
		/// �չ��캯����0
		/// </summary>
		public FractionHex()
		{
			this.liMolecule = new LongHex (0);
			this.liDenominator = new LongHex (1);
			this.bPositive = true;
		}
		#endregion

		#region public FractionHex(long nValue)
		/// <summary>
		/// ���캯��=��������
		/// </summary>
		/// <param name="nValue">����������</param>
		public FractionHex(long nValue)
		{
			Molecule.Equal (nValue);
			Denominator.Equal (1);
			Positive = Molecule.Positive ;
			Molecule.Positive = true;
		}
		#endregion

		#region public FractionHex(long nMolecule , long nDenominator)
		/// <summary>
		/// ���캯��=����/��ĸ
		/// </summary>
		/// <param name="nMolecule">�����η���</param>
		/// <param name="nDenominator">�����η�ĸ</param>
		public FractionHex(long nMolecule , long nDenominator)
		{
			if(nDenominator == 0)
				throw new System.InvalidOperationException ("error by divide zero");
			Molecule.Equal (System.Math .Abs (nMolecule));
			Denominator.Equal (System.Math .Abs (nDenominator));
			Positive = ((nMolecule >= 0 && nDenominator > 0) || (nMolecule < 0 && nDenominator < 0));
			LongHex liCommGene = LongHex.GetCommonGene (Molecule , Denominator);
			if(liCommGene > 1)
			{
				Molecule /= liCommGene;
				Denominator /= liCommGene;
				if(this.GeneChanged != null)
					this.GeneChanged (this , System.EventArgs.Empty );
			}
		}
		#endregion

		#region public FractionHex(LongHex liValue)
		/// <summary>
		/// ���캯��=��������
		/// </summary>
		/// <param name="liValue">ʮ�����Ƴ�������</param>
		public FractionHex(LongHex liValue)
		{
			Molecule.Equal (liValue);
			Denominator.Equal (1);
			Positive = Molecule.Positive ;
			Molecule.Positive = true;
		}
		#endregion

		#region public FractionHex(LongHex liMolecule , LongHex liDenominator)
		/// <summary>
		/// ���캯��=�����η���/�����η�ĸ
		/// </summary>
		/// <param name="liMolecule">ʮ�����Ƴ����η���</param>
		/// <param name="liDenominator">ʮ�����Ƴ����η�ĸ</param>
		public FractionHex(LongHex liMolecule , LongHex liDenominator)
		{
			if(liDenominator == 0)
				throw new System.InvalidOperationException ("error by divide zero");
			Molecule.Equal (liMolecule);
			Denominator.Equal (liDenominator);
			Positive = ((liMolecule >= 0 && liDenominator > 0) || (liMolecule < 0 && liDenominator < 0));
			Molecule.Positive = true;
			Denominator.Positive = true;
			LongHex liCommGene = LongHex.GetCommonGene (Molecule , Denominator);
			if(liCommGene > 1)
			{
				Molecule /= liCommGene;
				Denominator /= liCommGene;
				if(this.GeneChanged != null )
					this.GeneChanged (this , System.EventArgs .Empty );
			}
		}
		#endregion

		#region public FractionHex(FractionHex fValue)
		/// <summary>
		/// ���캯��=����
		/// </summary>
		/// <param name="fValue">����</param>
		public FractionHex(FractionHex fValue)
		{
			Molecule.Equal (fValue.Molecule );
			Denominator.Equal (fValue.Denominator );
			Positive = fValue.Positive ;
		}
		#endregion

		#region public FractionHex(String strValue[ , bool bHex=true])
		/// <summary>
		/// ���캯����������Դ��ʮ�����������ַ���
		/// </summary>
		/// <param name="bHex" >�Ƿ���ʮ������</param>
		/// <param name="strValue">ʮ�������ַ���</param>
		public FractionHex(String strValue , bool bHex)
		{
			string[] strSubString = strValue.Split ('/');
			if(bHex)
			{
				int nCount = strSubString.Length ;
				if(nCount == 1)
					liMolecule = new LongHex(strSubString[0]);
				else
				{
					liMolecule = new LongHex(strSubString[0]);
					liDenominator = new LongHex(strSubString[1]);
				}
			}
			else
			{
				int nCount = strSubString.Length ;
				if(nCount == 1)
					liMolecule = (LongHex)strSubString[0];
				else
				{
					liMolecule = (LongHex)strSubString[0];
					liDenominator = (LongHex)strSubString[1];
				}
			}
			bPositive = liMolecule.Positive && liDenominator.Positive || !liMolecule.Positive && !liDenominator.Positive;
			liMolecule.Positive = true;
			liDenominator.Positive = true;
		}

		/// <summary>
		/// ���캯����������Դ��ʮ�����������ַ���
		/// </summary>
		/// <param name="strValue">ʮ�������ַ���</param>
		public FractionHex(String strValue)
		{
			string[] strSubString = strValue.Split ('/');
			int nCount = strSubString.Length ;
			if(nCount == 1)
				liMolecule = new LongHex(strSubString[0]);
			else
			{
				liMolecule = new LongHex(strSubString[0]);
				liDenominator = new LongHex(strSubString[1]);
			}
			bPositive = liMolecule.Positive && liDenominator.Positive || !liMolecule.Positive && !liDenominator.Positive;
			liMolecule.Positive = true;
			liDenominator.Positive = true;
		}
		#endregion

		#region public static implicit operator FractionHex(long nValue)
		/// <summary>
		/// ��������ת��Ϊ����
		/// </summary>
		/// <param name="nValue">��������</param>
		/// <returns>����</returns>
		public static implicit operator FractionHex(long nValue)
		{
			FractionHex fResult = new FractionHex ();
			fResult.Molecule.Equal (nValue);
			fResult.Denominator.Equal (1);
			fResult.Positive = fResult.Molecule .Positive ;
			fResult.Molecule .Positive = true;
			return fResult;
		}
		#endregion

		#region public static implicit operator FractionHex(LongHex liValue)
		/// <summary>
		/// ʮ��������ת��Ϊ����
		/// </summary>
		/// <param name="liValue">ʮ�����Ƴ�������</param>
		/// <returns>����</returns>
		public static implicit operator FractionHex(LongHex liValue)
		{
			return new FractionHex (liValue);
		}
		#endregion

		#region public static implicit operator FractionHex(string strValue)
		/// <summary>
		/// �����ַ���ת��Ϊ����
		/// </summary>
		/// <param name="strValue">�����ַ���</param>
		/// <returns>����</returns>
		public static implicit operator FractionHex(string strValue)
		{
			string[] strSubString = strValue.Split ('/');
			int nCount = strSubString.Length ;
			if(nCount == 0)
				return 0L;
			else if(nCount == 1)
				return new FractionHex (strSubString[0] , 1L);
			else
			{
				return new FractionHex ((LongHex)strSubString[0] , (LongHex)strSubString[1]);
			}
		}
		#endregion

		#region public void Equal(long nValue)
		/// <summary>
		/// ��ֵ����=����������
		/// </summary>
		/// <param name="nValue">����������</param>
		public void Equal(long nValue)
		{
			this.Positive = (nValue > 0);
			this.Molecule.Equal (System.Math .Abs (nValue));
			this.Denominator.Equal (1);
		}
		#endregion

		#region public void Equal(LongHex liValue)
		/// <summary>
		/// ��ֵ����=ʮ�����Ƴ���������
		/// </summary>
		/// <param name="liValue">ʮ�����Ƴ���������</param>
		public void Equal(LongHex liValue)
		{
			this.Positive = liValue.Positive ;
			this.Molecule.Equal (liValue);
			this.Molecule .Positive = true;
			this.Denominator .Equal (1);
		}
		#endregion

		#region public void Equal(FractionHex fValue)
		/// <summary>
		/// ��ֵ����=����
		/// </summary>
		/// <param name="fValue">����</param>
		public void Equal(FractionHex fValue)
		{
			this.Positive = fValue.Positive ;
			this.Molecule .Equal (fValue.Molecule );
			this.Denominator .Equal (fValue.Denominator );
		}
		#endregion

		#region public static FractionHex operator -(FractionHex a)
		/// <summary>
		/// ����һԪ��
		/// </summary>
		/// <param name="a">����</param>
		/// <returns>ȡ������</returns>
		public static FractionHex operator -(FractionHex a)
		{
			FractionHex fValue = new FractionHex (a);
			fValue.Positive = !a.Positive ;
			return fValue;
		}
		#endregion

		#region public static FractionHex operator +(FractionHex a , FractionHex b)
		/// <summary>
		/// ����+
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>�Ӻ����</returns>
		public static FractionHex operator +(FractionHex a , FractionHex b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive )
			{
				FractionHex fValue = new FractionHex ((a.Molecule * b.Denominator) + (a.Denominator * b.Molecule) , a.Denominator * b.Denominator );
				fValue.Positive = a.Positive;
				return fValue;
			}
			else
				return a - (-b);
		}
		#endregion

		#region public static FractionHex operator -(FractionHex a , FractionHex b)
		/// <summary>
		/// ����-
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>�������</returns>
		public static FractionHex operator -(FractionHex a , FractionHex b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive  )
			{
				FractionHex fValue = new FractionHex ((a.Molecule * b.Denominator) - (a.Denominator * b.Molecule) , a.Denominator * b.Denominator );
				if(!a.Positive )
					fValue.Positive = !fValue.Positive ;
				return fValue;
			}
			else
			{
				return a + (-b);
			}
		}
		#endregion

		#region public static FractionHex operator *(FractionHex a , FractionHex b)
		/// <summary>
		/// ����*
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>�˺����</returns>
		public static FractionHex operator *(FractionHex a , FractionHex b)
		{
			FractionHex fValue = new FractionHex (a.Molecule * b.Molecule , a.Denominator * b.Denominator );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive  ;
			return fValue;
		}
		#endregion

		#region public static FractionHex operator /(FractionHex a , FractionHex b)
		/// <summary>
		/// ����/
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>�������</returns>
		public static FractionHex operator /(FractionHex a , FractionHex b)
		{
			FractionHex fValue = new FractionHex (a.Molecule * b.Denominator , a.Denominator * b.Molecule );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive ;
			return fValue;
		}
		#endregion

		#region public static bool operator ==(FractionHex a , FractionHex b)
		/// <summary>
		/// ����==
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>������ȷ���true</returns>
		public static bool operator ==(FractionHex a , FractionHex b)
		{
			if(a.Molecule == b.Molecule && a.Denominator == b.Denominator )
			{
				if(a.Molecule == 0)
					return true;
				else
					return (a.Positive == b.Positive );
			}
			else
				return false;
		}
		#endregion

		#region public static bool operator !=(FractionHex a , FractionHex b)
		/// <summary>
		/// ����!=
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>��������ȷ���true</returns>
		public static bool operator !=(FractionHex a , FractionHex b)
		{
			return !(a==b);
		}
		#endregion

		#region public static bool operator >(FractionHex a , FractionHex b)
		/// <summary>
		/// ����>
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>ǰ�ߴ󷵻�true</returns>
		public static bool operator >(FractionHex a , FractionHex b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive )
			{
				//ͬ����
				if(a.Molecule * b.Denominator > b.Molecule * a.Denominator )
					return a.Positive ;
				else
					return !a.Positive ;
			}
			else
			{
				//���Ų�ͬ
				if(a.Molecule == 0)
				{
					if(b.Molecule == 0)
						return false;
					else
						return !b.Positive ;
				}
				else
					return a.Positive ;
			}
		}
		#endregion

		#region public static bool operator <(FractionHex a , FractionHex b)
		/// <summary>
		/// ����&lt;
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>���ߴ󷵻�true</returns>
		public static bool operator <(FractionHex a , FractionHex b)
		{
			return (b > a);
		}
		#endregion

		#region public static bool operator >=(FractionHex a , FractionHex b)
		/// <summary>
		/// ����>=
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>ǰ�߽ϴ󷵻�true</returns>
		public static bool operator >=(FractionHex a , FractionHex b)
		{
			return !(b > a);
		}
		#endregion

		#region public static bool operator <=(FractionHex a , FractionHex b)
		/// <summary>
		/// ����&lt;=
		/// </summary>
		/// <param name="a">����a</param>
		/// <param name="b">����b</param>
		/// <returns>ǰ�߽�С����true</returns>
		public static bool operator <=(FractionHex a , FractionHex b)
		{
			return !(a > b);
		}
		#endregion

		#region public static FractionHex Xn(FractionHex x , int n)
		/// <summary>
		/// ����X^n����
		/// </summary>
		/// <param name="x">����</param>
		/// <param name="n">ָ��</param>
		/// <returns>����</returns>
		public static FractionHex Xn(FractionHex x , int n)
		{
			FractionHex fValue = new FractionHex (1);
			for(int i = 1;i <= n;i ++)
				fValue *= x;
			return fValue;
		}
		#endregion

		#region public static FractionHex Sqr(FractionHex x)
		/// <summary>
		/// ����X^2
		/// </summary>
		/// <param name="x">����</param>
		/// <returns>��ƽ��</returns>
		public static FractionHex Sqr(FractionHex x)
		{
			return x * x;
		}
		#endregion

		#region public static FractionHex Abs(FractionHex a)
		/// <summary>
		/// ȡ����ֵ
		/// </summary>
		/// <param name="a">����a</param>
		/// <returns>�����ֵ</returns>
		public static FractionHex Abs(FractionHex a)
		{
			FractionHex fResult = new FractionHex (a);
			fResult.Positive = true;
			return fResult;
		}
		#endregion

		#region public override bool Equals(object a)
		/// <summary>
		/// ����Equals
		/// </summary>
		/// <param name="o">���Ƚ�ֵ</param>
		/// <returns>��ȷ���true</returns>
		public override bool Equals(object o) 
		{
			try 
			{
				return (bool) (this == (FractionHex)o);
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
		/// <returns>���ط��ŵ�Hashֵ</returns>
		public override int GetHashCode()
		{
			return this.Positive .GetHashCode ();
		}
		#endregion

		/// <summary>
		/// ����Ϊ�ַ���
		/// </summary>
		/// <param name="bHaveComma">�Ƿ���ÿ�ѧ������</param>
		/// <returns>���ط����ַ�����ʽֵ</returns>
		public String Export(bool bHaveComma)
		{
			if(Molecule.Length + Denominator.Length > 8192)
			{
				throw new System.InvalidOperationException ("data is too long , use ToConsole to do.");
			}

			String strResult = "";
			if(!Positive)
				strResult += "-";
			strResult += Molecule.Export (bHaveComma);
			if(Denominator != 1)
			{
				strResult += "/";
				strResult += Denominator.Export (bHaveComma);
			}
			return strResult;
		}

		/// <summary>
		/// ��ʾ������̨��
		/// </summary>
		/// <param name="bHaveComma">�Ƿ���ÿ�ѧ������</param>
		public void ToConsole(bool bHaveComma)
		{
			if(!Positive)
				System.Console .Write ("-");
			Molecule.ToConsole (bHaveComma);
			if(Denominator != 1)
			{
				System.Console .Write ("/");
				Denominator.ToConsole (bHaveComma);
			}
		}
	}
}