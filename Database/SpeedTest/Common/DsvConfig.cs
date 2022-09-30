using System.Text;

namespace Otchitta.Example.Database.SpeedTest;

/// <summary>
/// DSV用設定情報クラスです。
/// </summary>
public sealed class DsvConfig {
	#region プロパティー定義
	/// <summary>
	/// 区切文字を取得します。
	/// </summary>
	/// <value>区切文字</value>
	public char Divide {
		get;
	}
	/// <summary>
	/// 特殊文字を取得します。
	/// </summary>
	/// <value>特殊文字</value>
	public char Escape {
		get;
	}
	/// <summary>
	/// 番号名称を取得します。
	/// </summary>
	/// <value>番号名称</value>
	public string? Number {
		get;
	}
	/// <summary>
	/// 先頭情報を取得します。
	/// </summary>
	/// <value>先頭情報</value>
	public string Prefix {
		get;
	}
	/// <summary>
	/// 添字書式を取得します。
	/// </summary>
	/// <value>添字書式</value>
	public string Format {
		get;
	}
	#endregion プロパティー定義

	#region 生成メソッド定義
	/// <summary>
	/// 連続設定情報を生成します。
	/// </summary>
	/// <param name="divide">区切文字</param>
	/// <param name="escape">特殊文字</param>
	/// <param name="number">番号名称</param>
	/// <param name="prefix">先頭情報</param>
	/// <param name="length">添字桁数</param>
	public DsvConfig(char divide = ',', char escape = '"', string? number = null, string prefix = "Field", int length = 3) {
		Divide = divide;
		Escape = escape;
		Number = number;
		Prefix = prefix;
		Format = ToFormat(length);
	}
	#endregion 生成メソッド定義

	#region 内部メソッド定義
	/// <summary>
	/// 添字書式へ変換します。
	/// </summary>
	/// <param name="length">添字桁数</param>
	/// <returns>添字書式</returns>
	private static string ToFormat(int length) =>
		new String('0', length);
	#endregion 内部メソッド定義

	#region 実装メソッド定義
	/// <summary>
	/// 要素番号を取得します。
	/// </summary>
	/// <param name="name">要素名称</param>
	/// <returns>要素番号</returns>
	public int GetCode(string name) {
		if (String.IsNullOrEmpty(name)) {
			return -1;
		} else if (name.StartsWith(Prefix) == false) {
			return -1;
		} else if (Int32.TryParse(name.Substring(Prefix.Length), out var result) == false) {
			return -1;
		} else {
			return result;
		}
	}
	/// <summary>
	/// 要素名称を取得します。
	/// </summary>
	/// <param name="code">要素番号</param>
	/// <returns>要素名称</returns>
	public string GetName(int code) {
		var result = new StringBuilder();
		result.Append(Prefix);
		result.Append(code.ToString(Format));
		return result.ToString();
	}
	#endregion 実装メソッド定義
}
