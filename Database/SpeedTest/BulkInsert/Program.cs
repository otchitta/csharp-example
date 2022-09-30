using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Otchitta.Example.Database.SpeedTest;

internal static class Program {
	#region ログ出力処理定義
	/// <summary>
	/// ログ出力処理
	/// </summary>
	private static NLog.ILogger? logger;
	/// <summary>
	/// ログ出力処理を取得します。
	/// </summary>
	/// <returns>ログ出力処理</returns>
	private static NLog.ILogger Logger => logger ??= NLog.LogManager.GetCurrentClassLogger();
	#endregion ログ出力処理定義

	#region 取込処理定義(CreateRecordList/CreateDataTable)
	/// <summary>
	/// CSVファイルから取込リストを生成します。
	/// </summary>
	/// <param name="sourceFile">CSVファイル</param>
	/// <returns>取込リスト</returns>
	private static List<DsvRecord> CreateRecordList(string sourceFile) {
		using (var stream = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
		using (var reader = new StreamReader(stream, Encoding.GetEncoding("Shift-JIS"))) {
			return new List<DsvRecord>(DsvHelper.DecodeData(new DsvConfig(), reader));
		}
	}
	/// <summary>
	/// CSVファイルから<c>DataTable</c>を作成します。
	/// </summary>
	/// <param name="sourceFile">CSVファイル</param>
	/// <returns>取込情報</returns>
	private static DataTable CreateDataTable(string sourceFile) {
		// CSVファイル取込み
		var sourceList = CreateRecordList(sourceFile);
		// CSVファイルの最大列数を取得
		var columnSize = 0;
		foreach (var sourceData in sourceList) {
			columnSize = Math.Max(columnSize, sourceData.Count);
		}
		// 返却情報作成
		var resultData = new DataTable("#import_data");
		for (var index = 1; index <= columnSize; index ++) {
			resultData.Columns.Add($"field_{index:000}", typeof(string));
		}
		foreach (var choose in sourceList) {
			var record = resultData.NewRow();
			for (var index = 1; index <= columnSize; index ++) {
				var value1 = $"Field{index:000}";
				var value2 = $"field_{index:000}";
				if (choose.TryGetValue(value1, out var value3)) {
					record[value2] = value3;
				} else {
					record[value2] = DBNull.Value;
				}
			}
			resultData.Rows.Add(record);
		}
		return resultData;
	}
	#endregion 取込処理定義(CreateRecordList/CreateDataTable)

	#region データベース処理定義(ExecuteNonQuery)
	/// <summary>
	/// DBコマンドを実行します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <returns>処理件数</returns>
	private static int ExecuteNonQuery(SqlCommand command) {
		try {
			return command.ExecuteNonQuery();
		} catch {
			Logger.Error("実行構文:{0}", command.CommandText);
			var parameters = command.Parameters;
			for (var index = 0; index < parameters.Count; index ++) {
				var parameter = parameters[index];
				Logger.Error("引数{0:0000}:{1}={2}", index + 1, parameter.ParameterName, parameter.Value);
			}
			throw;
		}
	}
	#endregion データベース処理定義(ExecuteNonQuery)

	#region データベース処理定義(CreateTempTable/DeleteFromTempTable/InsertFromTempTable/InsertBulk)
	/// <summary>
	/// 一時テーブルを作成します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="columnSize">列数</param>
	private static void CreateTempTable(SqlCommand command, int columnSize) {
		// SQL文作成
		var builder = new StringBuilder();
		builder.Append("CREATE TABLE #import_data (");
		for (var index = 1; index <= columnSize; index ++) {
			if (index != 1) builder.Append(",");
			builder.Append($"field_{index:000} NVARCHAR(MAX) NULL");
		}
		builder.Append(")");
		// SQL文実行
		command.CommandText = builder.ToString();
		command.ExecuteNonQuery();
	}
	/// <summary>
	/// 一時テーブルを作成します。
	/// </summary>
	/// <param name="connection">DBコネクション</param>
	/// <param name="columnSize">列数</param>
	private static void CreateTempTable(SqlConnection connection, int columnSize) {
		using (var command = connection.CreateCommand()) {
			CreateTempTable(command, columnSize);
		}
	}
	/// <summary>
	/// 一時テーブルに存在するデータを確定テーブルより削除します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <returns>削除件数</returns>
	private static int DeleteFromTempTable(SqlCommand command) {
		// SQL文実行
		command.CommandText = "DELETE FROM import_data WHERE field_001 IN (SELECT field_001 FROM #import_data)";
		return ExecuteNonQuery(command);
	}
	/// <summary>
	/// 一時テーブルより格納テーブルへ挿入します。
	/// <para>当該メソッドを実行する前に<c>DeleteFromTempTable</c>を実行してください。</para>
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="columnSize">列数</param>
	/// <returns>登録件数</returns>
	private static int InsertFromTempTable(SqlCommand command, int columnSize) {
		// SQL文作成
		var builder = new StringBuilder();
		builder.Append("INSERT INTO import_data(");
		for (var index = 1; index <= columnSize; index ++) {
			if (index != 1) builder.Append(",");
			builder.Append($"field_{index:000}");
		}
		builder.Append(")SELECT ");
		for (var index = 1; index <= columnSize; index ++) {
			if (index != 1) builder.Append(",");
			builder.Append($"field_{index:000}");
		}
		builder.Append(" FROM #import_data");
		// SQL文実行
		command.CommandText = builder.ToString();
		return ExecuteNonQuery(command);
	}
	/// <summary>
	/// 一時テーブルへ一括登録を実行します。
	/// </summary>
	/// <param name="connection">DBコネクション</param>
	/// <param name="dataTable">登録情報</param>
	private static void InsertBulk(SqlConnection connection, DataTable dataTable) {
		using (var bulk = new SqlBulkCopy(connection)) {
			bulk.DestinationTableName = "#import_data";
			bulk.WriteToServer(dataTable);
		}
	}
	#endregion データベース処理定義(CreateTempTable/DeleteFromTempTable/InsertFromTempTable/InsertBulk)

	/// <summary>
	/// CSVファイルの取込処理を実行します。
	/// </summary>
	/// <param name="parameter">DB接続文字列</param>
	/// <param name="sourceFile">CSVファイル</param>
	private static void MainRoutine(string parameter, string sourceFile) {
		// CSVファイルの取込
		Logger.Debug("[開始]ファイル取込");
		var sourceData = CreateDataTable(sourceFile);
		var columnSize = sourceData.Columns.Count;
		Logger.Debug("[終了]ファイル取込:{0,6:#,0}件", sourceData.Rows.Count);
		// データベース処理
		using (var connection = new SqlConnection(parameter)) {
			connection.Open();
			// 一時テーブル作成(一時テーブルなのでトランザクション外)
			CreateTempTable(connection, columnSize);
			// 一括登録実行
			Logger.Debug("[開始]一括一時登録");
			InsertBulk(connection, sourceData);
			Logger.Debug("[終了]一括一時登録");
			// 確定テーブルへの転送処理(ここはトランザクション内)
			using (var transaction = connection.BeginTransaction()) {
				try {
					using (var command = connection.CreateCommand()) {
						command.Transaction = transaction;
						command.CommandTimeout = 600;
						// 重複データ削除
						Logger.Debug("[開始]重複情報削除");
						Logger.Debug("[終了]重複情報削除:{0,6:#,0}件", DeleteFromTempTable(command));
						// 取込データ登録
						Logger.Debug("[開始]取込情報登録");
						Logger.Debug("[終了]取込情報登録:{0,6:#,0}件", InsertFromTempTable(command, columnSize));
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}
			}
		}
	}

	/// <summary>
	/// CSVファイルの取込処理を実行します。
	/// </summary>
	/// <param name="parameter">DB接続文字列</param>
	/// <param name="sourceData">CSVファイルまたはCSVディレクトリ</param>
	private static void FirstRoutine(string parameter, string sourceData) {
		if (File.Exists(sourceData)) {
			MainRoutine(parameter, sourceData);
		} else if (Directory.Exists(sourceData)) {
			var sourceList = Directory.GetFiles(sourceData, "*.csv");
			for (var index = 0; index < sourceList.Length; index ++) {
				var sourceFile = sourceList[index];
				Logger.Info("[情報]({0,3}/{1,3}){2}", index + 1, sourceList.Length, sourceFile);
				MainRoutine(parameter, sourceFile);
			}
		} else {
			Logger.Info("[情報]第１引数がファイルでもディレクトリでもありません");
		}
	}

	/// <summary>
	/// 登録処理を起動します。
	/// </summary>
	/// <param name="args">コマンドライン引数</param>
	public static void Main(string[] args) {
		if (args.Length == 2) {
			Logger.Info("[開始]================================================================");
			Logger.Info("取込位置：{0}", args[0]);
			Logger.Info("接続情報：{0}", args[1]);
			try {
				var current = DateTime.Now;
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				FirstRoutine(args[1], args[0]);
				Logger.Info("[情報]合計実行時間:{0}", (DateTime.Now - current));
			} catch (Exception error) {
				Logger.Error(error, "[情報]想定外のエラーが発生しました。");
			} finally {
				Logger.Info("[終了]================================================================");
			}
		} else {
			Console.WriteLine("コマンドライン引数が異なります。");
			Console.WriteLine("以下の様に入力してください。");
			Console.WriteLine();
			Console.WriteLine("dotnet run CSVファイル DB接続文字列");
			Console.WriteLine();
			Console.WriteLine("例：");
			Console.WriteLine("> dotnet run \"C:\\temp\\test.csv\" \"Data Source=localhost; Initial Catalog=database; User ID=user; Password=pass;\"");
		}
	}
}
