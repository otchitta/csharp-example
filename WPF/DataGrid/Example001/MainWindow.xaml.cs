using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Otchitta.Example.ViewModels;

namespace Otchitta.Example;

/// <summary>
/// メインウィンドウクラスです。
/// </summary>
public partial class MainWindow : Window {
	/// <summary>
	/// 画面モデル
	/// </summary>
	private MainViewModel viewModel;

	/// <summary>
	/// バインディングを設定します。
	/// </summary>
	/// <param name="views">画面側列一覧</param>
	/// <param name="datas">情報側列一覧</param>
	private static void SetBinding(IReadOnlyList<DataGridColumn> views, IReadOnlyList<ColumnViewModel> datas) {
		var count = Math.Min(views.Count, datas.Count);
		for (var index = 0; index < count; index ++) {
			var view = views[index];
			var data = datas[index];
			BindingOperations.SetBinding(view, DataGridColumn.HeaderProperty,
				new Binding($"[{index}].{nameof(ColumnViewModel.Name)}") { Source = datas });
			BindingOperations.SetBinding(view, DataGridColumn.VisibilityProperty,
				new Binding($"[{index}].{nameof(ColumnViewModel.Show)}") { Source = datas, Converter = new BooleanToVisibilityConverter() });
		}
	}

	/// <summary>
	/// 名称切替ボタン押下時の処理を行います。
	/// </summary>
	/// <param name="s">イベント情報</param>
	/// <param name="e">イベント引数</param>
	private void Button_OnClick1(object s, RoutedEventArgs e) => this.viewModel.ToggleName();
	/// <summary>
	/// 表示切替ボタン押下時の処理を行います。
	/// </summary>
	/// <param name="s">イベント情報</param>
	/// <param name="e">イベント引数</param>
	private void Button_OnClick2(object s, RoutedEventArgs e) => this.viewModel.ToggleShow();

	/// <summary>
	/// メインウィンドウを生成します。
	/// </summary>
	public MainWindow() {
		Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
		InitializeComponent();
		this.viewModel = new MainViewModel();
		DataContext = this.viewModel;
		SetBinding(Records.Columns, this.viewModel.Columns);
	}

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
