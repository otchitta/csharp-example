using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Otchitta.Example.ViewModels;

namespace Otchitta.Example;

/// <summary>
/// 主体画面モデルクラスです。
/// </summary>
public class MainViewModel : BaseViewModel {
	#region メンバー変数定義
	/// <summary>
	/// 列一覧
	/// </summary>
	private ObservableCollection<ColumnViewModel> columnList;
	/// <summary>
	/// 行一覧
	/// </summary>
	private ObservableCollection<RecordViewModel> recordList;
	/// <summary>
	/// 列番号
	/// </summary>
	private int columnCode;
	/// <summary>
	/// 行番号
	/// </summary>
	private int recordCode;
	/// <summary>
	/// 列情報
	/// </summary>
	private ColumnViewModel columnData;
	/// <summary>
	/// 行情報
	/// </summary>
	private RecordViewModel recordData;
	/// <summary>
	/// 挿入操作
	/// </summary>
	private DelegateMenuModel? insertMenu;
	/// <summary>
	/// 追加操作
	/// </summary>
	private DelegateMenuModel? appendMenu;
	/// <summary>
	/// 削除操作
	/// </summary>
	private DelegateMenuModel? removeMenu;
	/// <summary>
	/// 結果種別(True:異常 False:正常)
	/// </summary>
	private bool resultFlag;
	/// <summary>
	/// 結果内容
	/// </summary>
	private string? resultText;
	#endregion メンバー変数定義

	#region プロパティー定義
	/// <summary>
	/// 列一覧を取得します。
	/// </summary>
	/// <value>列一覧</value>
	public ReadOnlyObservableCollection<ColumnViewModel> ColumnList {
		get;
	}
	/// <summary>
	/// 行一覧を取得します。
	/// </summary>
	/// <value>行一覧</value>
	public ReadOnlyObservableCollection<RecordViewModel> RecordList {
		get;
	}
	/// <summary>
	/// 列番号を取得または設定します。
	/// </summary>
	/// <value>列番号</value>
	public int ColumnCode {
		get => this.columnCode;
		set => SetProperty(ref this.columnCode, value, nameof(ColumnCode));
	}
	/// <summary>
	/// 行番号を取得または設定します。
	/// </summary>
	/// <value>行番号</value>
	public int RecordCode {
		get => this.recordCode;
		set => SetProperty(ref this.recordCode, value, nameof(RecordCode));
	}
	/// <summary>
	/// 列情報を取得します。
	/// </summary>
	/// <value>列情報</value>
	public ColumnViewModel ColumnData {
		get => this.columnData;
		private set => SetProperty(ref this.columnData, value, nameof(ColumnData));
	}
	/// <summary>
	/// 行情報を取得します。
	/// </summary>
	/// <value>行情報</value>
	public RecordViewModel RecordData {
		get => this.recordData;
		private set => SetProperty(ref this.recordData, value, nameof(RecordData));
	}
	/// <summary>
	/// 挿入操作を取得します。
	/// </summary>
	/// <returns>挿入操作</returns>
	public BaseMenuModel InsertMenu => this.insertMenu ??= new DelegateMenuModel(ActionInsertMenu);
	/// <summary>
	/// 追加操作を取得します。
	/// </summary>
	/// <returns>追加操作</returns>
	public BaseMenuModel AppendMenu => this.appendMenu ??= new DelegateMenuModel(ActionAppendMenu);
	/// <summary>
	/// 削除操作を取得します。
	/// </summary>
	/// <returns>削除操作</returns>
	public BaseMenuModel RemoveMenu => this.removeMenu ??= new DelegateMenuModel(ActionRemoveMenu);
	/// <summary>
	/// 結果種別を取得します。
	/// </summary>
	/// <value>結果種別</value>
	public bool ResultFlag {
		get => this.resultFlag;
		private set => SetProperty(ref this.resultFlag, value, nameof(ResultFlag));
	}
	/// <summary>
	/// 結果内容を取得します。
	/// </summary>
	/// <value>結果内容</value>
	public string? ResultText {
		get => this.resultText;
		private set => SetProperty(ref this.resultText, value, nameof(ResultText));
	}
	#endregion プロパティー定義

	#region 生成メソッド定義
	/// <summary>
	/// 主体画面モデルを生成します。
	/// </summary>
	public MainViewModel() {
		this.columnList = new ObservableCollection<ColumnViewModel>(CreateColumnList());
		this.recordList = new ObservableCollection<RecordViewModel>(CreateRecordList());
		this.columnCode = -1;
		this.recordCode = -1;
		this.columnData = CreateColumnData();
		this.recordData = CreateRecordData();
		this.insertMenu = null;
		this.appendMenu = null;
		this.removeMenu = null;
		this.resultFlag = false;
		this.resultText = null;
		ColumnList = new ReadOnlyObservableCollection<ColumnViewModel>(this.columnList);
		RecordList = new ReadOnlyObservableCollection<RecordViewModel>(this.recordList);
	}
	#endregion 生成メソッド定義

	#region 内部メソッド定義(生成処理:CreateColumnData/CreateColumnList/CreateRecordData/CreateRecordList)
	/// <summary>
	/// 列情報を生成します。
	/// </summary>
	/// <param name="code">連携名称</param>
	/// <param name="name">表題名称</param>
	/// <param name="show">表示状態</param>
	/// <returns>列情報</returns>
	private static ColumnViewModel CreateColumnData(string code, string name, bool show = true) =>
		new ColumnViewModel() { Code = code, Name = name, Show = show };
	/// <summary>
	/// 列情報を生成します。
	/// </summary>
	/// <param name="offset">列番号</param>
	/// <returns>列情報</returns>
	private static ColumnViewModel CreateColumnData(int offset) =>
		CreateColumnData($"Item{offset:00}", $"Item-{offset:00}");
	/// <summary>
	/// 列一覧を生成します。
	/// </summary>
	/// <returns>列一覧</returns>
	private static IEnumerable<ColumnViewModel> CreateColumnList() {
		for (var offset = 1; offset <= 3; offset ++) {
			yield return CreateColumnData(offset);
		}
	}
	/// <summary>
	/// 列情報を生成します。
	/// <para>既定値を設定した列情報を返却します。
	/// これは、動作確認する為に毎回手入力をする必要を省く目的となります。
	/// よって、当該規定値も列数の値を元にした値とします(単純ルールであり削除された等の考慮はしない)。</para>
	/// </summary>
	/// <returns>列情報</returns>
	private ColumnViewModel CreateColumnData() =>
		CreateColumnData(this.columnList.Count + 1);
	/// <summary>
	/// 行情報を生成します。
	/// </summary>
	/// <param name="prefix">先頭内容</param>
	/// <returns>行情報</returns>
	private static RecordViewModel CreateRecordData(string prefix) =>
		new RecordViewModel() {
			Item01 = prefix + "A",
			Item02 = prefix + "B",
			Item03 = prefix + "C",
			Item04 = prefix + "D",
			Item05 = prefix + "E",
			Item06 = prefix + "F",
			Item07 = prefix + "G",
			Item08 = prefix + "H",
			Item09 = prefix + "I",
			Item10 = prefix + "J",
		};
	/// <summary>
	/// 行情報を生成します。
	/// </summary>
	/// <param name="offset">行番号</param>
	/// <returns>行情報</returns>
	private static RecordViewModel CreateRecordData(int offset) =>
		CreateRecordData($"Data-{offset:00}");
	/// <summary>
	/// 行一覧を生成します。
	/// </summary>
	/// <returns>行一覧</returns>
	private static IEnumerable<RecordViewModel> CreateRecordList() {
		for (var offset = 1; offset <= 10; offset ++) {
			yield return CreateRecordData(offset);
		}
	}
	/// <summary>
	/// 行情報を生成します。
	/// <para>既定値を設定した行情報を返却します。
	/// これは、動作確認する為に毎回手入力をする必要を省く目的となります。
	/// よって、当該規定値も行数の値を元にした値とします(単純ルールであり削除された等の考慮はしない)。</para>
	/// </summary>
	/// <returns>行情報</returns>
	private RecordViewModel CreateRecordData() =>
		CreateRecordData(this.recordList.Count + 1);
	#endregion 内部メソッド定義(生成処理:CreateColumnData/CreateColumnList/CreateRecordData/CreateRecordList)

	#region 内部メソッド定義(操作処理:ActionInsertMenu/ActionAppendMenu/ActionRemoveMenu)
	/// <summary>
	/// 挿入操作を実行します。
	/// </summary>
	private void ActionInsertMenu(object? parameter) {
		if ("column".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 列情報挿入
			if (this.columnCode < 0 || this.columnList.Count <= this.columnCode) {
				ResultFlag = true;
				ResultText = "列一覧が選択されていません。列一覧にて挿入する場所を指定してください。";
			} else {
				// 列情報を移し替え
				this.columnList.Insert(this.columnCode, this.columnData);
				ColumnData = CreateColumnData();
				ResultFlag = false;
				ResultText = "指定の列情報を挿入しました。";
			}
		} else if ("record".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 行情報挿入
			if (this.recordCode < 0 || this.recordList.Count <= this.recordCode) {
				ResultFlag = true;
				ResultText = "行一覧が選択されていません。行一覧にて挿入する場所を指定してください。";
			} else {
				// 行情報を移し替え
				this.recordList.Insert(this.recordCode, this.recordData);
				RecordData = CreateRecordData();
				ResultFlag = false;
				ResultText = "指定の行情報を挿入しました。";
			}
		} else {
			ResultFlag = true;
			ResultText = "不明な挿入操作が実行されました。";
		}
	}
	/// <summary>
	/// 追加操作を実行します。
	/// </summary>
	private void ActionAppendMenu(object? parameter) {
		if ("column".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 列情報を移し替え
			this.columnList.Add(this.columnData);
			ColumnData = CreateColumnData();
			ResultFlag = false;
			ResultText = "指定の列情報を追加しました。";
		} else if ("record".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 行情報を移し替え
			this.recordList.Add(this.recordData);
			RecordData = CreateRecordData();
			ResultFlag = false;
			ResultText = "指定の行情報を追加しました。";
		} else {
			ResultFlag = true;
			ResultText = "不明な追加操作が実行されました。";
		}
	}
	/// <summary>
	/// 削除操作を実行します。
	/// </summary>
	/// <param name="parameter">削除情報</param>
	private void ActionRemoveMenu(object? parameter) {
		if (parameter is ColumnViewModel cache1) {
			// 列情報削除
			this.columnList.Remove(cache1);
			ResultFlag = false;
			ResultText = "指定の列情報を削除しました。";
		} else if (parameter is RecordViewModel cache2) {
			// 行情報削除
			this.recordList.Remove(cache2);
			ResultFlag = false;
			ResultText = "指定の行情報を削除しました。";
		} else if ("column".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 列情報削除
			this.columnList.Clear();
			ColumnData = CreateColumnData(); // 入力値リセット
			ResultFlag = false;
			ResultText = "列情報を削除しました。";
		} else if ("record".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 行情報削除
			this.recordList.Clear();
			RecordData = CreateRecordData(); // 入力値リセット
			ResultFlag = false;
			ResultText = "行情報を削除しました。";
		} else if ("remove".Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase)) {
			// 全情報削除
			this.columnList.Clear();
			this.recordList.Clear();
			ColumnData = CreateColumnData(); // 入力値リセット
			RecordData = CreateRecordData(); // 入力値リセット
			ResultFlag = false;
			ResultText = "全情報を削除しました。";
		} else {
			ResultFlag = true;
			ResultText = "不明な削除操作が実行されました。";
		}
	}
	#endregion 内部メソッド定義(操作処理:ActionInsertMenu/ActionAppendMenu/ActionRemoveMenu)
}
