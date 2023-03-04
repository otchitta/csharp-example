using System.Runtime.InteropServices;

namespace Otchitta.Example002.Native;

/// <summary>
/// 入力結合構造体です。
/// </summary>
/// <seealso cref="https://qiita.com/kob58im/items/5a9d909377272d74eefd" />
[StructLayout(LayoutKind.Explicit)]
internal struct INPUTUNION {
	/// <summary>
	/// マウス情報
	/// </summary>
	[FieldOffset(0)]
	public MOUSEINPUT    mi;
	/// <summary>
	/// キーボード情報
	/// </summary>
	[FieldOffset(0)]
	public KEYBDINPUT    ki;
	/// <summary>
	/// ハードウェア情報
	/// </summary>
	[FieldOffset(0)]
	public HARDWAREINPUT hi;
}
