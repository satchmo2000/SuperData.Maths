using System;

namespace SuperData.Maths
{

	/// <summary>
	/// FractionΪ�����Ĵ洢��ʽ���洢��ԭʼ�ṹΪLongInt�������ɷ��ӣ���ĸ��ɣ���ΪLongInt����
	/// �������һ���������ͣ�ԭʼ���ݿ�����Դ�ںܶ����ͣ��糤���Σ�LongInt�ͣ��ַ�����
	/// �磺
	///		Fraction fValue = 1000L;
	///		Fraction fValue = "-100,101/56";
	/// </summary>
	public class Fraction
	{
		/// <summary>
		/// ����
		/// </summary>
		private LongInt liMolecule = new LongInt (0);

		/// <summary>
		/// ��ĸ
		/// </summary>
		private LongInt liDenominator = new LongInt (1);

		/// <summary>
		/// ���ݷ���,true��ʾΪ��,false��ʾΪ��
		/// </summary>
		private bool bPositive = true;
		
		/// <summary>
		/// ������ӷ����仯���¼�
		/// </summary>
		public event System.EventHandler GeneChanged;

		#region public LongInt Molecule
		/// <summary>
		/// ����
		/// </summary>
		public LongInt Molecule
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

		#region public LongInt Denominator
		/// <summary>
		/// ��ĸ
		/// </summary>
		public LongInt Denominator
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
		/// ����
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

		#region public int Length
		/// <summary>
		/// ��÷����ĳ���(ռ�ÿռ�)
		/// </summary>
		public int Length
		{
			get
			{
				return liMolecule.Length + liDenominator.Length ;
			}
		}
		#endregion

		#region public Fraction()
		/// <summary>
		/// ���캯��
		/// </summary>
		public Fraction()
		{
			this.liMolecule = new LongInt (0);
			this.liDenominator = new LongInt (1);
			this.bPositive = true;
		}
		#endregion

		#region public Fraction(long nValue)
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="nValue"></param>
		public Fraction(long nValue)
		{
			Molecule.Equal (nValue);
			Denominator.Equal (1);
			Positive = Molecule.Positive ;
			Molecule.Positive = true;
		}
		#endregion

		#region public Fraction(long nMolecule , long nDenominator)
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="nMolecule">����</param>
		/// <param name="nDenominator">��ĸ</param>
		public Fraction(long nMolecule , long nDenominator)
		{
			if(nDenominator == 0)
				throw new System.InvalidOperationException ("error by divide zero");
			Molecule.Equal (System.Math .Abs (nMolecule));
			Denominator.Equal (System.Math .Abs (nDenominator));
			Positive = ((nMolecule >= 0 && nDenominator > 0) || (nMolecule < 0 && nDenominator < 0));
			LongInt liCommGene = LongInt.GetCommonGene (Molecule , Denominator);
			if(liCommGene > 1)
			{
				Molecule /= liCommGene;
				Denominator /= liCommGene;
				if(this.GeneChanged != null)
					this.GeneChanged (this , System.EventArgs.Empty );
			}
		}
		#endregion

		#region public Fraction(LongInt liValue)
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="liValue"></param>
		public Fraction(LongInt liValue)
		{
			Molecule.Equal (liValue);
			Denominator.Equal (1);
			Positive = Molecule.Positive ;
			Molecule.Positive = true;
		}
		#endregion

		#region public Fraction(LongInt liMolecule , LongInt liDenominator)
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="liMolecule">����</param>
		/// <param name="liDenominator">��ĸ</param>
		public Fraction(LongInt liMolecule , LongInt liDenominator)
		{
			if(liDenominator == 0)
				throw new System.InvalidOperationException ("error by divide zero");
			Molecule.Equal (liMolecule);
			Denominator.Equal (liDenominator);
			Positive = ((liMolecule >= 0 && liDenominator > 0) || (liMolecule < 0 && liDenominator < 0));
			Molecule.Positive = true;
			Denominator.Positive = true;
			LongInt liCommGene = LongInt.GetCommonGene (Molecule , Denominator);
			if(liCommGene > 1)
			{
				Molecule /= liCommGene;
				Denominator /= liCommGene;
				if(this.GeneChanged != null )
					this.GeneChanged (this , System.EventArgs .Empty );
			}
		}
		#endregion

		#region public Fraction(Fraction fValue)
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="fValue"></param>
		public Fraction(Fraction fValue)
		{
			Molecule.Equal (fValue.Molecule );
			Denominator.Equal (fValue.Denominator );
			Positive = fValue.Positive ;
		}
		#endregion

		#region public static implicit operator Fraction(long nValue)
		/// <summary>
		/// ����ת��
		/// </summary>
		/// <param name="nValue"></param>
		/// <returns></returns>
		public static implicit operator Fraction(long nValue)
		{
			Fraction fResult = new Fraction ();
			fResult.Molecule.Equal (nValue);
			fResult.Denominator.Equal (1);
			fResult.Positive = fResult.Molecule .Positive ;
			fResult.Molecule .Positive = true;
			return fResult;
		}
		#endregion

		#region public static implicit operator Fraction(LongInt liValue)
		/// <summary>
		/// ����ת��
		/// </summary>
		/// <param name="liValue"></param>
		/// <returns></returns>
		public static implicit operator Fraction(LongInt liValue)
		{
			return new Fraction (liValue);
		}
		#endregion

		#region public static implicit operator Fraction(string strValue)
		/// <summary>
		/// ����ת��
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		public static implicit operator Fraction(string strValue)
		{
			string[] strSubString = strValue.Split ('/');
			int nCount = strSubString.Length ;
			if(nCount == 0)
				return 0L;
			else if(nCount == 1)
				return new Fraction (strSubString[0] , 1L);
			else
			{
				return new Fraction ((LongInt)strSubString[0] , (LongInt)strSubString[1]);
			}
		}
		#endregion

		#region public void Equal(long nValue)
		/// <summary>
		/// ��ֵ����
		/// </summary>
		/// <param name="nValue"></param>
		public void Equal(long nValue)
		{
			this.Positive = (nValue > 0);
			this.Molecule.Equal (System.Math .Abs (nValue));
			this.Denominator.Equal (1);
		}
		#endregion

		#region public void Equal(LongInt liValue)
		/// <summary>
		/// ��ֵ����
		/// </summary>
		/// <param name="liValue"></param>
		public void Equal(LongInt liValue)
		{
			this.Positive = liValue.Positive ;
			this.Molecule.Equal (liValue);
			this.Molecule .Positive = true;
			this.Denominator .Equal (1);
		}
		#endregion

		#region public void Equal(Fraction fValue)
		/// <summary>
		/// ��ֵ����
		/// </summary>
		/// <param name="fValue"></param>
		public void Equal(Fraction fValue)
		{
			this.Positive = fValue.Positive ;
			this.Molecule .Equal (fValue.Molecule );
			this.Denominator .Equal (fValue.Denominator );
		}
		#endregion

		#region public static Fraction operator -(Fraction a)
		/// <summary>
		/// ����һԪ��
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Fraction operator -(Fraction a)
		{
			Fraction fValue = new Fraction (a);
			fValue.Positive = !a.Positive ;
			return fValue;
		}
		#endregion

		#region public static Fraction operator +(Fraction a , Fraction b)
		/// <summary>
		/// ����+
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Fraction operator +(Fraction a , Fraction b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive )
			{
				Fraction fValue = new Fraction ((a.Molecule * b.Denominator) + (a.Denominator * b.Molecule) , a.Denominator * b.Denominator );
				fValue.Positive = a.Positive;
				return fValue;
			}
			else
				return a - (-b);
		}
		#endregion

		#region public static Fraction operator -(Fraction a , Fraction b)
		/// <summary>
		/// ����-
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Fraction operator -(Fraction a , Fraction b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive  )
			{
				Fraction fValue = new Fraction ((a.Molecule * b.Denominator) - (a.Denominator * b.Molecule) , a.Denominator * b.Denominator );
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

		#region public static Fraction operator *(Fraction a , Fraction b)
		/// <summary>
		/// ����*
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Fraction operator *(Fraction a , Fraction b)
		{
			Fraction fValue = new Fraction (a.Molecule * b.Molecule , a.Denominator * b.Denominator );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive  ;
			return fValue;
		}
		#endregion

		#region public static Fraction operator /(Fraction a , Fraction b)
		/// <summary>
		/// ����/
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Fraction operator /(Fraction a , Fraction b)
		{
			Fraction fValue = new Fraction (a.Molecule * b.Denominator , a.Denominator * b.Molecule );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive ;
			return fValue;
		}
		#endregion

		#region public static bool operator ==(Fraction a , Fraction b)
		/// <summary>
		/// ����==
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(Fraction a , Fraction b)
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

		#region public static bool operator !=(Fraction a , Fraction b)
		/// <summary>
		/// ����!=
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(Fraction a , Fraction b)
		{
			return !(a==b);
		}
		#endregion

		#region public static bool operator >(Fraction a , Fraction b)
		/// <summary>
		/// ����>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >(Fraction a , Fraction b)
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

		#region public static bool operator <(Fraction a , Fraction b)
		/// <summary>
		/// ����&lt;
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <(Fraction a , Fraction b)
		{
			return (b > a);
		}
		#endregion

		#region public static bool operator >=(Fraction a , Fraction b)
		/// <summary>
		/// ����>=
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >=(Fraction a , Fraction b)
		{
			return !(b > a);
		}
		#endregion

		#region public static bool operator <=(Fraction a , Fraction b)
		/// <summary>
		/// ����&lt;=
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <=(Fraction a , Fraction b)
		{
			return !(a > b);
		}
		#endregion

		#region public static Fraction Xn(Fraction x , uint n)
		/// <summary>
		/// ����X^n����
		/// </summary>
		/// <param name="x"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static Fraction Xn(Fraction x , uint n)
		{
			Fraction fValue = new Fraction (1);
			for(uint i = 1;i <= n;i ++)
				fValue *= x;
			return fValue;
		}
		#endregion

		#region public static Fraction Sqr(Fraction x)
		/// <summary>
		/// ����X^2
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Fraction Sqr(Fraction x)
		{
			return x * x;
		}
		#endregion

		#region public static Fraction Abs(Fraction a)
		/// <summary>
		/// ȡ����ֵ
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Fraction Abs(Fraction a)
		{
			Fraction fResult = new Fraction (a);
			fResult.Positive = true;
			return fResult;
		}
		#endregion

		#region public override bool Equals(object a)
		/// <summary>
		/// ����Equals
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals(object o) 
		{
			try 
			{
				return (bool) (this == (Fraction)o);
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
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.Positive .GetHashCode ();
		}
		#endregion

        #region public String SnapShot

        public override string ToString()
        {
            return Export(true);
        }

        /// <summary>
        /// �������
        /// </summary>
        public String SnapShot
        {
            get
            {
                String strResult = "";
                if (!Positive && Molecule != 0)
                    strResult += "-";
                strResult += Molecule.SnapShot;
                if (Denominator != 1)
                {
                    strResult += "/";
                    strResult += Denominator.SnapShot;
                }
                return strResult;
            }
        }

		/// <summary>
		/// ����Ϊ�ַ���
		/// </summary>
		/// <param name="bHaveComma"></param>
		/// <returns></returns>
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
		/// <param name="bHaveComma"></param>
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
        #endregion
    }
}