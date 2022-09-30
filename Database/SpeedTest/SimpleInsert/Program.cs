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
	/// 取込データを削除します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="record">取込データ</param>
	/// <returns>削除件数</returns>
	private static int DeleteRecordData(SqlCommand command, DsvRecord record) {
		command.CommandText = "DELETE FROM import_data WHERE field_001 = @Field001";
		command.Parameters.Clear();
		var parameter = command.Parameters.Add("Field001", SqlDbType.VarChar);
		if (record.TryGetValue("Field001", out var field001)) {
			parameter.Value = field001 ?? (object)DBNull.Value;
		}
		return ExecuteNonQuery(command);
	}
	/// <summary>
	/// 取込リストを削除します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="records">取込リスト</param>
	/// <returns>削除件数</returns>
	private static int DeleteRecordList(SqlCommand command, IEnumerable<DsvRecord> records) {
		var result = 0;
		foreach (var record in records) {
			result += DeleteRecordData(command, record);
		}
		return result;
	}

	/// <summary>
	/// 取込データを登録します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="record">取込データ</param>
	/// <returns>登録件数</returns>
	private static int InsertRecordData(SqlCommand command, DsvRecord record) {
		// SQL文作成
		var builder = new StringBuilder();
		builder.Append("INSERT INTO import_data(");
		for (var index = 1; index <= record.Count; index ++) {
			if (index != 1) builder.Append(", ");
			builder.Append($"field_{index:000}");
		}
		builder.Append(") VALUES (");
		for (var index = 1; index <= record.Count; index ++) {
			if (index != 1) builder.Append(", ");
			builder.Append($"@Field{index:000}");
		}
		builder.Append(')');
		// SQL文実行
		command.CommandText = builder.ToString();
		command.Parameters.Clear();
		foreach (var (name, data) in record) {
			var parameter = command.Parameters.Add(name, SqlDbType.VarChar);
			parameter.Value = data ?? (object)DBNull.Value;
		}
		return ExecuteNonQuery(command);
	}
	/// <summary>
	/// 取込リストを登録します。
	/// </summary>
	/// <param name="command">DBコマンド</param>
	/// <param name="records">取込リスト</param>
	/// <returns>登録件数</returns>
	private static int InsertRecordList(SqlCommand command, IEnumerable<DsvRecord> records) {
		var result = 0;
		foreach (var record in records) {
			result += InsertRecordData(command, record);
		}
		return result;
	}

	/// <summary>
	/// CSVファイルの取込処理を実行します。
	/// </summary>
	/// <param name="parameter">DB接続文字列</param>
	/// <param name="sourceFile">CSVファイル</param>
	private static void MainRoutine(string parameter, string sourceFile) {
		// CSVファイルの取込
		Logger.Debug("[開始]ファイル取込");
		var records = CreateRecordList(sourceFile);
		Logger.Debug("[終了]ファイル取込:{0,6:#,0}件", records.Count);
		// データベース処理
		using (var connection = new SqlConnection(parameter)) {
			connection.Open();
			using (var transaction = connection.BeginTransaction()) {
				try {
					using (var command = connection.CreateCommand()) {
						command.Transaction = transaction;
						command.CommandTimeout = 600;
						// 重複データ削除
						Logger.Debug("[開始]重複情報削除");
						Logger.Debug("[終了]重複情報削除:{0,6:#,0}件", DeleteRecordList(command, records));
						// 取込データ登録
						Logger.Debug("[開始]取込情報登録");
						Logger.Debug("[終了]取込情報登録:{0,6:#,0}件", InsertRecordList(command, records));
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
