using System.Runtime.InteropServices;
using DWORD = System.UInt32;
using ULONG_PTR = System.IntPtr;
using WORD = System.UInt16;

namespace Otchitta.Example002.Native;

/// <summary>
/// シミュレートされたキーボードイベントに関する情報が含まれています。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct KEYBDINPUT {
	/// <summary>
	/// 仮想キーコード。コードは、1から254の範囲の値である必要があります。dwFlagsメンバーがKEYEVENTF_UNICODEを指定する場合、wVkは0である必要があります。
	/// </summary>
	public WORD wVk;
	/// <summary>
	/// キーのハードウェアスキャンコード。dwFlagsがKEYEVENTF_UNICODEを指定する場合、wScanは、フォアグラウンドアプリケーションに送信されるUnicode文字を指定します。
	/// </summary>
	public WORD wScan;
	/// <summary>
	/// キーストロークのさまざまな側面を指定します。このメンバーには、次の値の特定の組み合わせを指定できます。
	/// <para>KEYEVENTF_EXTENDEDKEY = 0x0001 : 指定した場合、スキャンコードの前に、0xE0(224)の値を持つプレフィックスバイトが付いています。</para>
	/// <para>KEYEVENTF_KEYUP       = 0x0002 : 指定した場合、キーは解放されます。指定しない場合は、キーが押されています。</para>
	/// <para>KEYEVENTF_SCANCODE    = 0x0004 : 指定した場合、wScanはキーを識別し、wVkは無視されます。</para>
	/// <para>KEYEVENTF_UNICODE     = 0x0008 : 指定した場合、システムはVK_PACKETキーストロークを合成します。wVkパラメーターは0である必要があります。このフラグは、KEYEVENTF_KEYUPフラグとのみ組み合わせることができます。詳細については、「解説」を参照してください。</para>
	/// </summary>
	public DWORD dwFlags;
	/// <summary>
	/// イベントのタイムスタンプ(ミリ秒単位)。このパラメーターが0の場合、システムは独自のタイムスタンプを提供します。
	/// </summary>
	public DWORD time;
	/// <summary>
	/// キーストロークに関連付けられている追加の値。GetMessageExtraInfo関数を使用して、この情報を取得します。
	/// </summary>
	public ULONG_PTR dwExtraInfo;
}
