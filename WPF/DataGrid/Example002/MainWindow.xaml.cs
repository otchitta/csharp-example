using System;
using System.Windows;
using System.Windows.Threading;

namespace Otchitta.Example;

/// <summary>
/// メインウィンドウクラスです。
/// </summary>
public partial class MainWindow : Window {
	/// <summary>
	/// メインウィンドウを生成します。
	/// </summary>
	public MainWindow() {
		Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
		InitializeComponent();
	}

	/// <summary>
	/// 終了処理を実行します。
	/// </summary>
	/// <param name="s">イベント情報</param>
	/// <param name="e">イベント引数</param>
	private void ActionClose(object s, RoutedEventArgs e) => Close();

	/// <summary>
	/// エラー時のイベント処理を実行します。
	/// </summary>
	/// <param name="s">イベント情報</param>
	/// <param name="e">イベント引数</param>
	private static void OnDispatcherUnhandledException(object s, DispatcherUnhandledExceptionEventArgs e) {
		Console.WriteLine(e.Exception);
		Environment.Exit(-1);
	}
}
