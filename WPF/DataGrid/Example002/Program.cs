using System;
using System.Windows;

namespace Otchitta.Example;

/// <summary>
/// 実行処理クラスです。
/// </summary>
public sealed class Program : Application {
	[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
	public static extern bool AttachConsole(int processId);

	/// <summary>
	/// 実行処理クラスを生成します。
	/// </summary>
	private Program() {
		// 処理なし
	}

	/// <summary>
	/// サンプルプログラムを実行します。
	/// </summary>
	[STAThread]
	public static void Main() {
		// 初期定義
		AttachConsole(-1);

		// 設定処理
		var source = new Program();
		source.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

		// 実行処理
		source.Run();
	}
}
