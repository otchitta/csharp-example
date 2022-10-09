namespace Otchitta.Example001.ViewModels;

/// <summary>
/// 行画面モデルクラスです。
/// </summary>
public sealed class RecordViewModel : BaseViewModel {
	/// <summary>
	/// 項目情報１
	/// </summary>
	private object? item01;
	/// <summary>
	/// 項目情報１を取得または設定します。
	/// </summary>
	/// <value>項目情報１</value>
	public object? Item01 {
		get => this.item01;
		set => SetProperty(ref this.item01, value, nameof(Item01));
	}

	/// <summary>
	/// 項目情報２
	/// </summary>
	private object? item02;
	/// <summary>
	/// 項目情報２を取得または設定します。
	/// </summary>
	/// <value>項目情報２</value>
	public object? Item02 {
		get => this.item02;
		set => SetProperty(ref this.item02, value, nameof(Item02));
	}

	/// <summary>
	/// 項目情報３
	/// </summary>
	private object? item03;
	/// <summary>
	/// 項目情報３を取得または設定します。
	/// </summary>
	/// <value>項目情報３</value>
	public object? Item03 {
		get => this.item03;
		set => SetProperty(ref this.item03, value, nameof(Item03));
	}
}
