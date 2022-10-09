using System;
using System.Collections.ObjectModel;
using Otchitta.Example001.ViewModels;

namespace Otchitta.Example001;

/// <summary>
/// 主体画面モデルクラスです。
/// </summary>
public class MainViewModel {
	#region プロパティー定義
	/// <summary>
	/// 列一覧
	/// </summary>
	private ObservableCollection<ColumnViewModel> columns;
	/// <summary>
	/// 列一覧を取得します。
	/// </summary>
	/// <value>列一覧</value>
	public ReadOnlyObservableCollection<ColumnViewModel> Columns {
		get;
	}
	/// <summary>
	/// 行一覧
	/// </summary>
	private ObservableCollection<RecordViewModel> records;
	/// <summary>
	/// 行一覧を取得します。
	/// </summary>
	/// <value>行一覧</value>
	public ReadOnlyObservableCollection<RecordViewModel> Records {
		get;
	}
	#endregion プロパティー定義

	#region 生成メソッド定義
	/// <summary>
	/// 主体画面モデルを生成します。
	/// </summary>
	public MainViewModel() {
		this.columns = new ObservableCollection<ColumnViewModel>() {
			new ColumnViewModel() { Name = "Item-01", Show = true },
			new ColumnViewModel() { Name = "Item-02", Show = true },
			new ColumnViewModel() { Name = "Item-03", Show = true },
		};
		this.records = new ObservableCollection<RecordViewModel>() {
			new RecordViewModel() { Item01 = "Data-01", Item02 = "Data-02", Item03 = "Data-03" },
			new RecordViewModel() { Item01 = "Data-11", Item02 = "Data-12", Item03 = "Data-13" },
			new RecordViewModel() { Item01 = "Data-21", Item02 = "Data-22", Item03 = "Data-23" },
			new RecordViewModel() { Item01 = "Data-31", Item02 = "Data-32", Item03 = "Data-33" },
			new RecordViewModel() { Item01 = "Data-41", Item02 = "Data-42", Item03 = "Data-43" },
			new RecordViewModel() { Item01 = "Data-51", Item02 = "Data-52", Item03 = "Data-53" },
			new RecordViewModel() { Item01 = "Data-61", Item02 = "Data-62", Item03 = "Data-63" },
			new RecordViewModel() { Item01 = "Data-71", Item02 = "Data-72", Item03 = "Data-73" },
			new RecordViewModel() { Item01 = "Data-81", Item02 = "Data-82", Item03 = "Data-83" },
			new RecordViewModel() { Item01 = "Data-91", Item02 = "Data-92", Item03 = "Data-93" },
		};
		Columns = new ReadOnlyObservableCollection<ColumnViewModel>(this.columns);
		Records = new ReadOnlyObservableCollection<RecordViewModel>(this.records);
	}
	#endregion 生成メソッド定義

	#region 公開メソッド定義
	/// <summary>
	/// 列名称を変更します。
	/// </summary>
	public void ToggleName() {
		var column = this.columns[0];
		switch (column.Name) {
		case "Item-01":
			Console.WriteLine("名称変更:Item-01 -> ColumnA");
			column.Name = "ColumnA";
			break;
		case "ColumnA":
			Console.WriteLine("名称変更:ColumnA -> Item-01");
			column.Name = "Item-01";
			break;
		}
	}
	/// <summary>
	/// 列表示を変更します。
	/// </summary>
	public void ToggleShow() {
		var column = this.columns[1];
		if (column.Show) {
			Console.WriteLine("表示変更:隠蔽");
			column.Show = false;
		} else {
			Console.WriteLine("表示変更:表示");
			column.Show = true;
		}
	}
	#endregion 公開メソッド定義
}
