using System;

namespace Otchitta.Example.ViewModels;

/// <summary>
/// <see cref="Delegate" />利用画面操作クラスです。
/// </summary>
public sealed class DelegateMenuModel : BaseMenuModel {
	/// <summary>
	/// 可否判定
	/// </summary>
	private Predicate<object?> accept;
	/// <summary>
	/// 実行処理
	/// </summary>
	private Action<object?> action;

	/// <summary>
	/// <see cref="Delegate" />利用画面操作を生成します。
	/// </summary>
	/// <param name="action">実行処理</param>
	/// <param name="accept">可否判定</param>
	public DelegateMenuModel(Action<object?> action, Predicate<object?>? accept = null) {
		this.action = action;
		this.accept = accept ?? (parameter => true);
	}
	/// <summary>
	/// <see cref="Delegate" />利用画面操作を生成します。
	/// </summary>
	/// <param name="action">実行処理</param>
	/// <param name="accept">可否判定</param>
	public DelegateMenuModel(Action action, Func<bool>? accept = null) {
		this.action = parameter => action();
		this.accept = parameter => accept?.Invoke() ?? true;
	}

	/// <summary>
	/// 実行可否を判定します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	/// <returns>実行可能である場合、<c>True</c>を返却</returns>
	protected override bool Accept(object? parameter) => this.accept(parameter);
	/// <summary>
	/// 操作処理を実行します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	protected override void Invoke(object? parameter) => this.action(parameter);
}
