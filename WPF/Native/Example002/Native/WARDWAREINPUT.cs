using System.Runtime.InteropServices;
using DWORD = System.UInt32;
using WORD = System.UInt16;

namespace Otchitta.Example002.Native;

/// <summary>
/// キーボードまたはマウス以外の入力デバイスによって生成されたシミュレートされたメッセージに関する情報が含まれます。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct HARDWAREINPUT {
	/// <summary>
	/// 入力ハードウェアによって生成されたメッセージ。
	/// </summary>
	public DWORD uMsg;
	/// <summary>
	/// uMsgのlParamパラメーターの下位ワード。
	/// </summary>
	public WORD wParamL;
	/// <summary>
	/// uMsgのlParamパラメーターの上位ワード。
	/// </summary>
	public WORD wParamH;
}
