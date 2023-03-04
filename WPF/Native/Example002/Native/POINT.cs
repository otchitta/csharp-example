using System.Runtime.InteropServices;
using LONG = System.Int32;

namespace Otchitta.Example002.Native;

/// <summary>
/// 座標情報構造体です。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINT {
	/// <summary>
	/// X座標
	/// </summary>
	public LONG x;
	/// <summary>
	/// Y座標
	/// </summary>
	public LONG y;
}
