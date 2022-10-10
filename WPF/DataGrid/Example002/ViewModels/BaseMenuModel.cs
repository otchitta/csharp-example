using System;
using System.Windows.Input;

namespace Otchitta.Example.ViewModels;

/// <summary>
/// 基底画面操作クラスです。
/// </summary>
public abstract class BaseMenuModel : ICommand {
	/// <summary>
	/// 実行可否変更後イベント処理
	/// </summary>
	public event EventHandler? CanExecuteChanged;

	/// <summary>
	/// 可否変更を通知します。
	/// </summary>
	protected void Notify() =>
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);

	/// <summary>
	/// 実行可否を判定します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	/// <returns>実行可能である場合、<c>True</c>を返却</returns>
	protected abstract bool Accept(object? parameter);
	/// <summary>
	/// 操作処理を実行します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	protected abstract void Invoke(object? parameter);

	/// <summary>
	/// 実行可否を判定します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	/// <returns>実行可能である場合、<c>True</c>を返却</returns>
	bool ICommand.CanExecute(object? parameter) => Accept(parameter);
	/// <summary>
	/// 操作処理を実行します。
	/// </summary>
	/// <param name="parameter">引数情報</param>
	void ICommand.Execute(object? parameter) => Invoke(parameter);
}
