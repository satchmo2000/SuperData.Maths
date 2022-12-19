using System;

namespace SuperData.Maths
{

	/// <summary>
	/// Fraction为分数的存储格式，存储的原始结构为LongInt，分数由分子，分母组成，都为LongInt类型
	/// 符号另存一个布尔类型，原始数据可能来源于很多类型，如长整形，LongInt型，字符串等
	/// 如：
	///		Fraction fValue = 1000L;
	///		Fraction fValue = "-100,101/56";
	/// </summary>
	public class Fraction
	{
		/// <summary>
		/// 分子
		/// </summary>
		private LongInt liMolecule = new LongInt (0);

		/// <summary>
		/// 分母
		/// </summary>
		private LongInt liDenominator = new LongInt (1);

		/// <summary>
		/// 数据符号,true表示为正,false表示为否
		/// </summary>
		private bool bPositive = true;
		
		/// <summary>
		/// 最大公因子发生变化的事件
		/// </summary>
		public event System.EventHandler GeneChanged;

		#region public LongInt Molecule
		/// <summary>
		/// 分子
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
		/// 分母
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
		/// 符号
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
		/// 获得分数的长度(占用空间)
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
		/// 构造函数
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
		/// 构造函数
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
		/// 构造函数
		/// </summary>
		/// <param name="nMolecule">分子</param>
		/// <param name="nDenominator">分母</param>
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
		/// 构造函数
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
		/// 构造函数
		/// </summary>
		/// <param name="liMolecule">分子</param>
		/// <param name="liDenominator">分母</param>
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
		/// 构造函数
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
		/// 类型转换
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
		/// 类型转换
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
		/// 类型转换
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
		/// 赋值操作
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
		/// 赋值操作
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
		/// 赋值操作
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
		/// 重载一元减
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
		/// 重载+
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
		/// 重载-
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
		/// 重载*
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
		/// 重载/
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
		/// 重载==
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
		/// 重载!=
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
		/// 重载>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >(Fraction a , Fraction b)
		{
			if(a.Positive && b.Positive || !a.Positive && !b.Positive )
			{
				//同符号
				if(a.Molecule * b.Denominator > b.Molecule * a.Denominator )
					return a.Positive ;
				else
					return !a.Positive ;
			}
			else
			{
				//符号不同
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
		/// 重载&lt;
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
		/// 重载>=
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
		/// 重载&lt;=
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
		/// 计算X^n的幂
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
		/// 计算X^2
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
		/// 取绝对值
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
		/// 重载Equals
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
		/// 重载GetHashCode
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
        /// 结果快照
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
		/// 导出为字符串
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
		/// 显示到控制台上
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