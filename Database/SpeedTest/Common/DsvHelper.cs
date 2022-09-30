using System;
using System.Collections.Generic;
using System.Text;

namespace Otchitta.Example.Database.SpeedTest;

/// <summary>
/// DSV用共通関数クラスです。
/// </summary>
public static class DsvHelper {
	/// <summary>
	/// 要素情報を生成します。
	/// </summary>
	/// <param name="config">設定情報</param>
	/// <param name="record">解析情報</param>
	/// <returns>要素情報</returns>
	private static DsvRecord CreateData(DsvConfig config, IReadOnlyList<string> record) {
		var result = new Dictionary<string, string>();
		for (var index = 0; index < record.Count; index ++) {
			result.Add(config.GetName(index + 1), record[index]);
		}
		return new DsvRecord(result);
	}
	/// <summary>
	/// 項目情報を変換します。
	/// </summary>
	/// <param name="source">項目情報</param>
	/// <param name="escape">特殊文字</param>
	/// <returns>変換情報</returns>
	public static string DecodeItem(ReadOnlySpan<char> source, char escape) {
		var result = new StringBuilder(source.Length);
		var before = '\u0000';
		foreach (var choose in source) {
			if (before != escape) {
				result.Append(choose);
				before = choose;
			} else if (choose == escape) {
				before = '\u0000';
			} else {
				throw new SystemException("not duplicated character.");
			}
		}
		if (before == '"') {
			throw new SystemException("not duplicated character.");
		} else {
			return result.ToString();
		}
	}

	/// <summary>
	/// 要素情報を変換します。
	/// </summary>
	/// <param name="config">設定情報</param>
	/// <param name="source">要素情報</param>
	/// <returns>変換情報</returns>
	public static DsvRecord DecodeLine(DsvConfig config, ReadOnlySpan<char> source) {
		var result = new List<string>();
		var escape = config.Escape;
		var divide = config.Divide;
		var offset = 0;
		var ignore = false;
		for (var index = 0; index < source.Length; index ++) {
			var choose = source[index];
			if (choose == escape) {
				ignore = !ignore;
			} else if (ignore) {
				// 処理なし
			} else if (choose != divide) {
				// 処理なし
			} else {
				var values = source.Slice(offset, index - offset);
				if (values.Length <= 1) {
					result.Add(DecodeItem(values, escape));
				} else if (values[0] == escape && values[values.Length - 1] == escape) {
					result.Add(DecodeItem(values.Slice(1, values.Length - 2), escape));
				} else {
					result.Add(DecodeItem(values, escape));
				}
				offset = index + 1;
			}
		}
		if (offset < source.Length) {
			var values = source.Slice(offset);
			if (values.Length <= 1) {
				result.Add(DecodeItem(values, escape));
			} else if (values[0] == escape && values[values.Length - 1] == escape) {
				result.Add(DecodeItem(values.Slice(1, values.Length - 2), escape));
			} else {
				result.Add(DecodeItem(values, escape));
			}
		}
		return CreateData(config, result);
	}

	/// <summary>
	/// 要素情報を変換します。
	/// </summary>
	/// <param name="config">設定情報</param>
	/// <param name="reader">読込処理</param>
	/// <returns>変換情報</returns>
	public static IEnumerable<DsvRecord> DecodeData(DsvConfig config, TextReader reader) {
		var escape = config.Escape;
		var buffer = new StringBuilder();
		var ignore = false;
		var before = '\u0000';
		var number = 1;
		while (true) {
			var choose = reader.Read();
			if (choose == -1) {
				break;
			} else if (ignore) {
				buffer.Append((char)choose);
				ignore = !ignore;
			} else if (ignore) {
				buffer.Append((char)choose);
			} else if (before == '\r' && choose == '\n') {
				buffer.Length --;
				yield return DecodeLine(config, buffer.ToString());
				buffer.Clear();
				number ++;
			} else {
				buffer.Append((char)choose);
			}
			before = (char)choose;
		}
		if (buffer.Length != 0) {
			yield return DecodeLine(config, buffer.ToString());
		}
	}
}
