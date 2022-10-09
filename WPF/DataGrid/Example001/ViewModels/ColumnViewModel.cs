namespace Otchitta.Example001.ViewModels;

/// <summary>
/// 列画面モデルクラスです。
/// </summary>
public sealed class ColumnViewModel : BaseViewModel {
	/// <summary>
	/// 表示状態(<c>True</c>:表示 <c>False</c>:非表示)
	/// </summary>
	private bool show;
	/// <summary>
	/// 表示状態を取得または設定します。
	/// </summary>
	/// <value>表示状態(<c>True</c>:表示 <c>False</c>:非表示)</value>
	public bool Show {
		get => this.show;
		set => SetProperty(ref this.show, value, nameof(Show));
	}

	/// <summary>
	/// 表題名称
	/// </summary>
	private string? name;
	/// <summary>
	/// 表題名称を取得または設定します。
	/// </summary>
	/// <value>表題名称</value>
	public string? Name {
		get => this.name;
		set => SetProperty(ref this.name, value, nameof(Name));
	}
}
