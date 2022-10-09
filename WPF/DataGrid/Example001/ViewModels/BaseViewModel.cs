using System.ComponentModel;

namespace Otchitta.Example001.ViewModels;

/// <summary>
/// 基底画面モデルクラスです。
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged {
	/// <summary>
	/// プロパティー変更後イベント処理
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// プロパティー変更後イベントを発行します。
	/// </summary>
	/// <param name="member">要素名称</param>
	protected void OnPropertyChanged(string member) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
	}
	/// <summary>
	/// プロパティーを設定します。
	/// </summary>
	/// <param name="sourceData">要素情報</param>
	/// <param name="updateData">更新情報</param>
	/// <param name="memberName">要素名称</param>
	/// <typeparam name="TValue">要素種別</typeparam>
	protected void SetProperty<TValue>(ref TValue sourceData, TValue updateData, string memberName) {
		if (Equals(sourceData, updateData) == false) {
			sourceData = updateData;
			OnPropertyChanged(memberName);
		}
	}
}
