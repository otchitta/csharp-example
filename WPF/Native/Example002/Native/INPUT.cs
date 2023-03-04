using System.Runtime.InteropServices;
using DWORD = System.UInt32;

namespace Otchitta.Example002.Native;

/// <summary>
/// SendInputによって、キーストローク、マウスの動き、マウスクリックなどの入力イベントを合成するための情報を格納するために使用されます。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct INPUT {
	/// <summary>
	/// 入力イベントの型。このメンバーには、次のいずれかの値を指定できます。
	/// <para>INPUT_MOUSE    = 0 : イベントはマウスイベントです。共用体のmi構造体を使用します。</para>
	/// <para>INPUT_KEYBOARD = 1 : イベントはキーボードイベントです。共用体のki構造を使用します。</para>
	/// <para>INPUT_HARDWARE = 2 : イベントはハードウェアイベントです。共用体のhi構造体を使用します。</para>
	/// </summary>
	public DWORD type;
	/// <summary>
	/// 共通参照
	/// </summary>
	public INPUTUNION union;

	/// <summary>
	/// マウス入力情報を生成します。
	/// </summary>
	/// <param name="source">マウス情報</param>
	/// <returns>マウス入力情報</returns>
	public static INPUT Create(MOUSEINPUT source) =>
		new INPUT() { type = 0, union = new INPUTUNION() { mi = source }};
	/// <summary>
	/// キーボード入力情報を生成します。
	/// </summary>
	/// <param name="source">キーボード情報</param>
	/// <returns>キーボード入力情報</returns>
	public static INPUT Create(KEYBDINPUT source) =>
		new INPUT() { type = 0, union = new INPUTUNION() { ki = source }};
	/// <summary>
	/// ハードウェア入力情報を生成します。
	/// </summary>
	/// <param name="source">ハードウェア情報</param>
	/// <returns>ハードウェア入力情報</returns>
	public static INPUT Create(HARDWAREINPUT source) =>
		new INPUT() { type = 0, union = new INPUTUNION() { hi = source }};
}
