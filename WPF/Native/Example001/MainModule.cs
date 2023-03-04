using System;
using System.Windows;

[assembly:ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

namespace Otchitta.Example001;

/// <summary>
/// 主要画面管理クラスです。
/// </summary>
internal sealed class MainModule : Application {
	#region 実行メソッド定義
	/// <summary>
	/// 主要処理を開始します。
	/// </summary>
	/// <param name="commands">コマンドライン引数</param>
	[STAThread]
	public static void Main(string[] commands) {
		// 初期定義

		// 生成処理
		var source = new MainModule();

		// 設定処理
		source.StartupUri = new Uri("/MainWindow.xaml", UriKind.Relative);

		// 実行処理
		source.Run();
	}
	#endregion 実行メソッド定義
}
