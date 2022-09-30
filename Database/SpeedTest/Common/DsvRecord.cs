using System.Diagnostics.CodeAnalysis;

namespace Otchitta.Example.Database.SpeedTest;

/// <summary>
/// DSV用要素情報クラスです。
/// </summary>
public sealed class DsvRecord : IReadOnlyCollection<KeyValuePair<string, string>> {
	#region メンバー変数定義
	/// <summary>
	/// 要素一覧
	/// </summary>
	private readonly Dictionary<string, string> values;
	#endregion メンバー変数定義

	#region プロパティー定義
	/// <summary>
	/// 要素個数を取得します。
	/// </summary>
	/// <returns>要素個数</returns>
	public int Count => this.values.Count;
	/// <summary>
	/// 要素情報を取得します。
	/// </summary>
	/// <param name="name">要素名称</param>
	/// <value>要素情報</value>
	public string this[string name] => GetData(name);
	#endregion プロパティー定義

	#region 生成メソッド定義
	/// <summary>
	/// DSV用要素情報を生成します。
	/// </summary>
	/// <param name="values">要素集合</param>
	internal DsvRecord(IEnumerable<KeyValuePair<string, string>> values) {
		this.values = new Dictionary<string, string>(values);
	}
	#endregion 生成メソッド定義

	#region 内部メソッド定義
	/// <summary>
	/// 要素情報を取得します。
	/// </summary>
	/// <param name="name">要素名称</param>
	/// <returns>要素情報</returns>
	/// <exception cref="KeyNotFoundException">要素名称が存在しない場合</exception>
	private string GetData(string name) {
		if (this.values.TryGetValue(name, out var result)) {
			return result;
		} else {
			throw new KeyNotFoundException($"name is not found.(name={name})");
		}
	}
	#endregion 内部メソッド定義

	#region 実装メソッド定義
	/// <summary>
	/// 要素情報を取得します。
	/// </summary>
	/// <param name="name">要素名称</param>
	/// <param name="data">要素情報</param>
	/// <returns>要素名称が存在した場合、<c>True</c>を返却</returns>
	public bool TryGetValue(string name, [MaybeNullWhen(false)]out string data) => this.values.TryGetValue(name, out data);

	/// <summary>
	/// 反復処理を取得します。
	/// </summary>
	/// <returns>反復処理</returns>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		foreach (var choose in this.values) {
			yield return choose;
		}
	}
	/// <summary>
	/// 反復処理を取得します。
	/// </summary>
	/// <returns>反復処理</returns>
	IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() {
		foreach (var choose in this.values) {
			yield return choose;
		}
	}
	#endregion 実装メソッド定義
}
