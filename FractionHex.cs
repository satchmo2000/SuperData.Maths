using System;

namespace SuperData.Maths
{

	/// <summary>
	/// FractionHex为分数的存储格式，存储的原始结构为LongHex，分数由分子，分母组成，都为LongHex类型
	/// 符号另存一个布尔类型，原始数据可能来源于很多类型，如长整形，LongHex型，字符串等
	/// 如：
	///		FractionHex fValue = 1000L;
	///		FractionHex fValue = "100,110";
	/// </summary>
	public class FractionHex
	{
		/// <summary>
		/// 分子
		/// </summary>
		private LongHex liMolecule = new LongHex (0);

		/// <summary>
		/// 分母
		/// </summary>
		private LongHex liDenominator = new LongHex (1);

		/// <summary>
		/// 数据符号,true表示为正,false表示为否
		/// </summary>
		private bool bPositive = true;
		
		/// <summary>
		/// 最大公因子发生变化的事件
		/// </summary>
		public event System.EventHandler GeneChanged;

		#region public LongHex Molecule
		/// <summary>
		/// 分子属性
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
		/// 分母属性
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
		/// 分数符号
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
		/// 结果快照
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
		/// 获得分数的长度(占用空间)
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
		/// 空构造函数＝0
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
		/// 构造函数=长整形数
		/// </summary>
		/// <param name="nValue">长整形数字</param>
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
		/// 构造函数=分子/分母
		/// </summary>
		/// <param name="nMolecule">长整形分子</param>
		/// <param name="nDenominator">长整形分母</param>
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
		/// 构造函数=长整形数
		/// </summary>
		/// <param name="liValue">十六进制长整形数</param>
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
		/// 构造函数=长整形分子/长整形分母
		/// </summary>
		/// <param name="liMolecule">十六进制长整形分子</param>
		/// <param name="liDenominator">十六进制长整形分母</param>
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
		/// 构造函数=分数
		/// </summary>
		/// <param name="fValue">分数</param>
		public FractionHex(FractionHex fValue)
		{
			Molecule.Equal (fValue.Molecule );
			Denominator.Equal (fValue.Denominator );
			Positive = fValue.Positive ;
		}
		#endregion

		#region public FractionHex(String strValue[ , bool bHex=true])
		/// <summary>
		/// 构造函数，数据来源于十六进制数字字符串
		/// </summary>
		/// <param name="bHex" >是否是十六进制</param>
		/// <param name="strValue">十六进制字符串</param>
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
		/// 构造函数，数据来源于十六进制数字字符串
		/// </summary>
		/// <param name="strValue">十六进制字符串</param>
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
		/// 长整形数转化为分数
		/// </summary>
		/// <param name="nValue">长整形数</param>
		/// <returns>分数</returns>
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
		/// 十六进制数转化为分数
		/// </summary>
		/// <param name="liValue">十六进制长整形数</param>
		/// <returns>分数</returns>
		public static implicit operator FractionHex(LongHex liValue)
		{
			return new FractionHex (liValue);
		}
		#endregion

		#region public static implicit operator FractionHex(string strValue)
		/// <summary>
		/// 数字字符串转化为分数
		/// </summary>
		/// <param name="strValue">数字字符串</param>
		/// <returns>分数</returns>
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
		/// 赋值操作=长整形数字
		/// </summary>
		/// <param name="nValue">长整形数字</param>
		public void Equal(long nValue)
		{
			this.Positive = (nValue > 0);
			this.Molecule.Equal (System.Math .Abs (nValue));
			this.Denominator.Equal (1);
		}
		#endregion

		#region public void Equal(LongHex liValue)
		/// <summary>
		/// 赋值操作=十六进制长整形数字
		/// </summary>
		/// <param name="liValue">十六进制长整形数字</param>
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
		/// 赋值操作=分数
		/// </summary>
		/// <param name="fValue">分数</param>
		public void Equal(FractionHex fValue)
		{
			this.Positive = fValue.Positive ;
			this.Molecule .Equal (fValue.Molecule );
			this.Denominator .Equal (fValue.Denominator );
		}
		#endregion

		#region public static FractionHex operator -(FractionHex a)
		/// <summary>
		/// 重载一元减
		/// </summary>
		/// <param name="a">分数</param>
		/// <returns>取反分数</returns>
		public static FractionHex operator -(FractionHex a)
		{
			FractionHex fValue = new FractionHex (a);
			fValue.Positive = !a.Positive ;
			return fValue;
		}
		#endregion

		#region public static FractionHex operator +(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载+
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>加后分数</returns>
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
		/// 重载-
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>减后分数</returns>
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
		/// 重载*
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>乘后分数</returns>
		public static FractionHex operator *(FractionHex a , FractionHex b)
		{
			FractionHex fValue = new FractionHex (a.Molecule * b.Molecule , a.Denominator * b.Denominator );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive  ;
			return fValue;
		}
		#endregion

		#region public static FractionHex operator /(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载/
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>除后分数</returns>
		public static FractionHex operator /(FractionHex a , FractionHex b)
		{
			FractionHex fValue = new FractionHex (a.Molecule * b.Denominator , a.Denominator * b.Molecule );
			fValue.Positive = a.Positive && b.Positive || !a.Positive && !b.Positive ;
			return fValue;
		}
		#endregion

		#region public static bool operator ==(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载==
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>两数相等返回true</returns>
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
		/// 重载!=
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>两数不相等返回true</returns>
		public static bool operator !=(FractionHex a , FractionHex b)
		{
			return !(a==b);
		}
		#endregion

		#region public static bool operator >(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载>
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>前者大返回true</returns>
		public static bool operator >(FractionHex a , FractionHex b)
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

		#region public static bool operator <(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载&lt;
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>后者大返回true</returns>
		public static bool operator <(FractionHex a , FractionHex b)
		{
			return (b > a);
		}
		#endregion

		#region public static bool operator >=(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载>=
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>前者较大返回true</returns>
		public static bool operator >=(FractionHex a , FractionHex b)
		{
			return !(b > a);
		}
		#endregion

		#region public static bool operator <=(FractionHex a , FractionHex b)
		/// <summary>
		/// 重载&lt;=
		/// </summary>
		/// <param name="a">分数a</param>
		/// <param name="b">分数b</param>
		/// <returns>前者较小返回true</returns>
		public static bool operator <=(FractionHex a , FractionHex b)
		{
			return !(a > b);
		}
		#endregion

		#region public static FractionHex Xn(FractionHex x , int n)
		/// <summary>
		/// 计算X^n的幂
		/// </summary>
		/// <param name="x">底数</param>
		/// <param name="n">指数</param>
		/// <returns>求幂</returns>
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
		/// 计算X^2
		/// </summary>
		/// <param name="x">底数</param>
		/// <returns>求平方</returns>
		public static FractionHex Sqr(FractionHex x)
		{
			return x * x;
		}
		#endregion

		#region public static FractionHex Abs(FractionHex a)
		/// <summary>
		/// 取绝对值
		/// </summary>
		/// <param name="a">分数a</param>
		/// <returns>求绝对值</returns>
		public static FractionHex Abs(FractionHex a)
		{
			FractionHex fResult = new FractionHex (a);
			fResult.Positive = true;
			return fResult;
		}
		#endregion

		#region public override bool Equals(object a)
		/// <summary>
		/// 重载Equals
		/// </summary>
		/// <param name="o">被比较值</param>
		/// <returns>相等返回true</returns>
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
		/// 重载GetHashCode
		/// </summary>
		/// <returns>返回符号的Hash值</returns>
		public override int GetHashCode()
		{
			return this.Positive .GetHashCode ();
		}
		#endregion

		/// <summary>
		/// 导出为字符串
		/// </summary>
		/// <param name="bHaveComma">是否采用科学计数法</param>
		/// <returns>返回分数字符串形式值</returns>
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
		/// <param name="bHaveComma">是否采用科学计数法</param>
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