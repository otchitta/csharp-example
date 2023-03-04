using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Otchitta.Example002.Native;

namespace Otchitta.Example002;

/// <summary>
/// 主要画面処理クラスです。
/// </summary>
public partial class MainWindow : Window {
	#region 生成メソッド定義
	/// <summary>
	/// 主要画面処理を生成します。
	/// </summary>
	public MainWindow() {
		InitializeComponent();
	}
	#endregion 生成メソッド定義

	#region 内部メソッド定義
	/// <summary>
	/// 実行情報を抽出します。
	/// </summary>
	/// <param name="result">実行情報</param>
	/// <returns>抽出に成功した場合、<c>True</c>を返却</returns>
	private static bool ChooseInvokeData([MaybeNullWhen(false)]out Process result) {
		foreach (var choose in Process.GetProcesses()) {
			if (choose.ProcessName == "Otchitta.Example001") {
				result = choose;
				return true;
			}
		}
		result = default;
		return false;
	}
	/// <summary>
	/// 実行操作を実行します。
	/// </summary>
	/// <param name="sender">実行情報</param>
	/// <param name="values">実行引数</param>
	private void ActionInvokeMenu(object? sender, RoutedEventArgs values) {
		if (Int32.TryParse(PositionX.Text, out var positionX) == false) {
			ResultText.Text = "X座標を整数値で入力してください\r\n" + ResultText.Text;
		} else if (Int32.TryParse(PositionY.Text, out var positionY) == false) {
			ResultText.Text = "Y座標を整数値で入力してください\r\n" + ResultText.Text;
		} else if (positionX < 0 || 65535 < positionX) {
			ResultText.Text = "X座標を0～65535の間で入力してください\r\n" + ResultText.Text;
		} else if (positionY < 0 || 65535 < positionY) {
			ResultText.Text = "Y座標を0～65535の間で入力してください\r\n" + ResultText.Text;
		} else if (EventType.SelectedIndex == -1) {
			ResultText.Text = "実行種別を選択してください" + ResultText.Text;
		} else if (ChooseInvokeData(out var invokeData) == false) {
			ResultText.Text = "受信プログラムを起動してください\r\n" + ResultText.Text;
		} else if (NativeMethod.ActionMouseLButtonClick(invokeData.MainWindowHandle, positionX, positionY) == false) {
			ResultText.Text = "左ボタン開放に失敗しました\r\n" + ResultText.Text;
		} else {
			ResultText.Text = $"左クリック処理に成功しました\r\n" + ResultText.Text;
		}
	}
	#endregion 内部メソッド定義
}
