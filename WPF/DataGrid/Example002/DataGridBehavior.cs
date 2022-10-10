using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Otchitta.Example.ViewModels;

namespace Otchitta.Example;

/// <summary>
/// <see cref="DataGrid" />拡張処理クラスです。
/// </summary>
public static class DataGridBehavior {
	#region プロパティー定義(ColumnsProperty/ColumnsHandlerProperty)
	/// <summary>
	/// 列一覧プロパティー定義です。
	/// </summary>
	public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
		"Columns", typeof(IEnumerable), typeof(DataGridBehavior), new FrameworkPropertyMetadata(null, UpdateColumns));
	/// <summary>
	/// 列一覧を取得します。
	/// </summary>
	/// <param name="source">要素情報</param>
	/// <returns>取得情報</returns>
	public static IEnumerable? GetColumns(DataGrid source) =>
		(IEnumerable?)source.GetValue(ColumnsProperty);
	/// <summary>
	/// 列一覧を設定します。
	/// </summary>
	/// <param name="source">要素情報</param>
	/// <param name="values">設定情報</param>
	public static void SetColumns(DataGrid source, IEnumerable? values) =>
		source.SetValue(ColumnsProperty, values);

	/// <summary>
	/// 列処理プロパティー定義です。
	/// </summary>
	/// <remarks>内部利用の為、アクセスレベルは「private」としている</remarks>
	private static readonly DependencyProperty ColumnsHandlerProperty = DependencyProperty.RegisterAttached(
		"ColumnsHandler", typeof(ColumnsHandler), typeof(DataGridBehavior));
	/// <summary>
	/// 列処理を取得します。
	/// </summary>
	/// <param name="source">要素情報</param>
	/// <returns>取得情報</returns>
	private static ColumnsHandler? GetColumnsHandler(DataGrid source) =>
		(ColumnsHandler?)source.GetValue(ColumnsHandlerProperty);
	/// <summary>
	/// 列処理を設定します。
	/// </summary>
	/// <param name="source">要素情報</param>
	/// <param name="values">設定情報</param>
	private static void SetColumnsHandler(DataGrid source, ColumnsHandler? values) =>
		source.SetValue(ColumnsHandlerProperty, values);
	#endregion プロパティー定義(ColumnsProperty/ColumnsHandlerProperty)

	#region 内部メソッド定義(変更処理:UpdateColumns)
	/// <summary>
	/// 列一覧更新時の処理を行います。
	/// </summary>
	/// <param name="screen">画面要素</param>
	/// <param name="values">引数情報</param>
	private static void UpdateColumns(DataGrid screen, DependencyPropertyChangedEventArgs values) {
		// 前回処理の破棄
		GetColumnsHandler(screen)?.Dispose();
		// 今回処理の生成
		if (values.NewValue is IEnumerable choose) {
			// 列一覧がある場合：処理生成
			SetColumnsHandler(screen, ColumnsHandler.CreateHandle(screen, choose));
		} else {
			// 列一覧がない場合：NULL設定(プロパティーにNULLが設定された場合等)
			SetColumnsHandler(screen, null);
		}
	}
	/// <summary>
	/// 列一覧更新時の処理を行います。
	/// </summary>
	/// <param name="source">画面要素</param>
	/// <param name="values">引数情報</param>
	private static void UpdateColumns(DependencyObject source, DependencyPropertyChangedEventArgs values) {
		if (source is DataGrid screen) UpdateColumns(screen, values);
	}
	#endregion 内部メソッド定義(変更処理:UpdateColumns)

	#region 非公開クラス定義(ColumnsHandler)
	/// <summary>
	/// 列処理クラスです。
	/// </summary>
	private sealed class ColumnsHandler {
		#region メンバー変数定義
		/// <summary>
		/// 列識別プロパティー定義です。
		/// </summary>
		/// <remarks>内部利用の為、アクセスレベルは「private」としている</remarks>
		private static readonly DependencyProperty ColumnsMarkingProperty = DependencyProperty.RegisterAttached(
			"ColumnsMarking", typeof(ColumnsHandler), typeof(ColumnsHandler));
		/// <summary>
		/// 変換処理
		/// </summary>
		private static BooleanToVisibilityConverter? change;
		/// <summary>
		/// 画面要素
		/// </summary>
		private DataGrid? screen;
		/// <summary>
		/// 破棄処理
		/// </summary>
		private Action? action;
		#endregion メンバー変数定義

		#region プロパティー定義
		/// <summary>
		/// 変換処理を取得します。
		/// </summary>
		/// <para>シングルトンとしているのは追加削除等でメモリ増大を抑制をする目的である</para>
		private static BooleanToVisibilityConverter Change => change ??= new BooleanToVisibilityConverter();
		#endregion プロパティー定義

		#region 生成メソッド定義
		/// <summary>
		/// 列処理を生成します。
		/// </summary>
		/// <param name="screen">画面要素</param>
		private ColumnsHandler(DataGrid screen) {
			this.screen = screen;
			this.action = default;
		}
		/// <summary>
		/// 列処理を生成します。
		/// </summary>
		/// <param name="screen">画面要素</param>
		/// <param name="values">要素集合</param>
		/// <returns>列処理(監視不可である場合、<c>Null</c>を返却)</returns>
		public static ColumnsHandler? CreateHandle(DataGrid screen, IEnumerable values) {
			if (values is INotifyCollectionChanged source) {
				// 変更通知処理を実装している場合：列処理を生成
				var result = new ColumnsHandler(screen);
				// イベント設定
				source.CollectionChanged += result.ActionValues;
				// 破棄処理設定
				result.action = () => source.CollectionChanged -= result.ActionValues;
				// 要素一覧適用
				result.ReloadValues(screen, values);
				return result;
			} else {
				// 変更通知処理を実装していない場合：監視不可としてNULL返却
				return null;
			}
		}
		#endregion 生成メソッド定義

		#region 破棄メソッド定義
		/// <summary>
		/// 保持情報を破棄します。
		/// </summary>
		public void Dispose() {
			this.action?.Invoke();
			this.screen = default;
			this.action = default;
		}
		#endregion 破棄メソッド定義

		#region 内部メソッド定義(CreateColumn/RemoveColumn)
		/// <summary>
		/// 要素情報を生成します。
		/// <para>要素情報は<see cref="ColumnViewModel" />である事が前提の処理となります。</para>
		/// </summary>
		/// <param name="source">要素情報</param>
		/// <param name="offset">要素番号</param>
		/// <returns>要素情報</returns>
		private static DataGridColumn CreateColumn(ColumnViewModel source, int offset) {
			var result = new DataGridTextColumn();
			// ヘッダバインド設定(名称と表示に連携)
			BindingOperations.SetBinding(result, DataGridColumn.HeaderProperty,
				new Binding(nameof(ColumnViewModel.Name)) { Source = source });
			BindingOperations.SetBinding(result, DataGridColumn.VisibilityProperty,
				new Binding(nameof(ColumnViewModel.Show)) { Source = source, Converter = Change });
			// データバインド設定
			if (source.Code != null) {
				result.Binding = new Binding(source.Code);
			}
			// 特殊処理(連携経路はバインドできないのでロジックにて対応)
			source.PropertyChanged += (s, e) => {
				if (e.PropertyName == nameof(ColumnViewModel.Code)) {
					// 連携経路が変更された場合：バインドを変更
					result.Binding = new Binding(source.Code);
				}
			};
			return result;
		}
		/// <summary>
		/// 要素情報を生成します。
		/// <para>要素情報は<see cref="ColumnViewModel" />である事が前提の処理となります。</para>
		/// </summary>
		/// <param name="source">要素情報</param>
		/// <param name="offset">要素番号</param>
		/// <returns>要素情報</returns>
		private DataGridColumn CreateColumn(object? source, int offset) {
			DataGridColumn result;
			if (source is ColumnViewModel choose) {
				result = CreateColumn(choose, offset);
			} else {
				// ColumnViewModelではない場合、未設定のDataGridTextColumnを返却
				result = new DataGridTextColumn();
			}
			// !!重要!!XAMLで作成された要素と区別する為、添付ビヘイビアに自身を設定!!
			result.SetValue(ColumnsMarkingProperty, this);
			return result;
		}
		/// <summary>
		/// 要素情報を削除します。
		/// </summary>
		/// <param name="source">要素一覧</param>
		/// <param name="offset">削除番号</param>
		/// <returns>要素の削除を行った場合、<c>True</c>を返却(当該クラスにて作成された情報ではない等は<c>False</c>を返却)</returns>
		private bool RemoveColumn(ObservableCollection<DataGridColumn> source, int offset) {
			var choose = source[offset]; // 削除情報を一旦取得
			if (ReferenceEquals(this, choose.GetValue(ColumnsMarkingProperty)) == false) { // XAMLで作成された要素か判定
				// 当該情報によって作成された要素ではない為、削除は行わない：False返却
				return false;
			} else {
				// 当該情報によって作成された要素の為、削除
				source.RemoveAt(offset);
				choose.SetValue(ColumnsMarkingProperty, null); // ガーベージコレクション対象となるように自身の参照を削除
				return true;
			}
		}
		/// <summary>
		/// 要素情報を削除します。
		/// </summary>
		/// <param name="source">要素一覧</param>
		private void RemoveColumn(ObservableCollection<DataGridColumn> source) {
			for (var index = source.Count - 1; 0 <= index; index --) {
				RemoveColumn(source, index); // 結果は見ない(後方から処理している為、結果による分岐が必要ない)
			}
		}
		#endregion 内部メソッド定義(CreateColumn/RemoveColumn)

		#region 内部メソッド定義(ReloadValues/InsertValues/RemoveValues)
		/// <summary>
		/// 要素一覧を置換します。
		/// <para><see cref="DataGrid" />に存在する列を<paramref name="values" />の情報に全て置き換えます。</para>
		/// </summary>
		/// <param name="screen">画面要素</param>
		/// <param name="values">要素集合</param>
		private void ReloadValues(DataGrid screen, IEnumerable? values) {
			if (values != null) {
				RemoveColumn(screen.Columns);
				var offset = 0;
				foreach (var choose in values) {
					screen.Columns.Add(CreateColumn(choose, offset));
					offset ++;
				}
			}
		}
		/// <summary>
		/// 要素一覧を挿入します。
		/// </summary>
		/// <param name="screen">画面要素</param>
		/// <param name="offset">挿入位置</param>
		/// <param name="values">挿入一覧</param>
		private void InsertValues(DataGrid screen, int offset, IList? values) {
			if (values != null) {
				for (var index = 0; index < values.Count; index ++) {
					screen.Columns.Insert(offset + index, CreateColumn(values[index], offset));
				}
			}
		}
		/// <summary>
		/// 要素一覧を削除します。
		/// </summary>
		/// <param name="screen">画面要素</param>
		/// <param name="offset">削除位置</param>
		/// <param name="values">削除一覧</param>
		private void RemoveValues(DataGrid screen, int offset, IList? values) {
			// 削除一覧がない場合、実装不正として処理しない
			if (values != null) {
				for (var index = values.Count - 1; 0 <= index; index --) {
					RemoveColumn(screen.Columns, offset + index); // 結果は参照しない(処理が複雑になる為)
				}
			}
		}
		#endregion 内部メソッド定義(ReloadValues/InsertValues/RemoveValues)

		#region 内部メソッド定義(ActionValues)
		/// <summary>
		/// 要素変更を実行します。
		/// </summary>
		/// <param name="source">イベント情報</param>
		/// <param name="values">イベント引数</param>
		/// <remarks><paramref name="source" />が<see cref="IEnumerable" />を実装していない場合、実装不正として処理しない</remarks>
		private void ActionValues(object? source, NotifyCollectionChangedEventArgs values) {
			if (this.screen == null) {
				Dispose(); // 破棄後に呼び出された場合：想定外処理ではあるが例外を出さない様にする(念の為、再度破棄実行)
			} else if (source is IEnumerable choose) {
				switch (values.Action) {
				case NotifyCollectionChangedAction.Add: // 要素追加
					InsertValues(this.screen, values.NewStartingIndex, values.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove: // 要素削除
					RemoveValues(this.screen, values.OldStartingIndex, values.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace: // 要素置換
				case NotifyCollectionChangedAction.Move: // 要素移動
					if (values.NewStartingIndex <= values.OldStartingIndex) {
						// 削除要素が挿入要素より後方にある場合：削除後に挿入
						RemoveValues(this.screen, values.OldStartingIndex, values.OldItems);
						InsertValues(this.screen, values.NewStartingIndex, values.NewItems);
					} else {
						// 挿入要素が削除要素より後方にある場合：挿入後に削除
						InsertValues(this.screen, values.NewStartingIndex, values.NewItems);
						RemoveValues(this.screen, values.OldStartingIndex, values.OldItems);
					}
					break;
				case NotifyCollectionChangedAction.Reset: // リセット
					ReloadValues(this.screen, choose);
					break;
				}
			}
		}
		#endregion 内部メソッド定義(ActionValues)
	}
	#endregion 非公開クラス定義(ColumnsHandler)
}
