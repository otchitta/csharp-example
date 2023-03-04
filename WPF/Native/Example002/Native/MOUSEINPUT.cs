using System.Runtime.InteropServices;
using DWORD = System.UInt32;
using LONG = System.Int32;
using ULONG_PTR = System.IntPtr;

namespace Otchitta.Example002.Native;

/// <summary>
/// シミュレートされたマウスイベントに関する情報が含まれています。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MOUSEINPUT {
	/// <summary>
	/// dwFlagsメンバーの値に応じて、マウスの絶対位置、または最後のマウスイベントが生成されてからのモーションの量。
	/// 絶対データはマウスのx座標として指定されます。相対データは、移動されたピクセル数として指定されます。
	/// </summary>
	[MarshalAs(UnmanagedType.I4)]
	public LONG dx;
	/// <summary>
	/// dwFlagsメンバーの値に応じて、マウスの絶対位置、または最後のマウス イベントが生成されてからのモーションの量。
	/// 絶対データはマウスのy座標として指定されます。相対データは、移動されたピクセル数として指定されます。
	/// </summary>
	[MarshalAs(UnmanagedType.I4)]
	public LONG dy;
	/// <summary>
	/// dwFlagsにMOUSEEVENTF_WHEELが含まれている場合、mouseDataはホイールの移動量を指定します。
	/// 正の値は、ホイールがユーザーから離れて前方に回転したことを示します。
	/// 負の値は、ホイールがユーザーに向かって後方に回転したことを示します。
	/// 1つのホイールクリックは、WHEEL_DELTA(120)として定義されます。
	/// <para>dwFlagsにMOUSEEVENTF_WHEEL、MOUSEEVENTF_XDOWN、またはMOUSEEVENTF_XUPが含まれていない場合は、mouseDataを0にする必要があります。</para>
	/// <para>dwFlagsにMOUSEEVENTF_XDOWNまたはMOUSEEVENTF_XUPが含まれている場合、mouseDataは、どのXボタンを押すか離したかを指定します。
	/// この値は、次のフラグを任意に組み合わせて使用できます。
	/// <para>XBUTTON1 = 0x0001 : 最初のXボタンを押すか離す場合に設定します。</para>
	/// <para>XBUTTON2 = 0x0002 : 2つ目のXボタンを押すか離す場合に設定します。</para></para>
	/// </summary>
	[MarshalAs(UnmanagedType.U4)]
	public DWORD mouseData;
	/// <summary>
	/// マウスの動きとボタンのクリックのさまざまな側面を指定するビットフラグのセット。
	/// このメンバーのビットは、次の値の任意の妥当な組み合わせにすることができます。
	/// <para>マウスボタンの状態を指定するビットフラグは、進行中の状態ではなく、状態の変化を示すように設定されます。
	/// たとえば、マウスの左ボタンを押したまま押すと、左ボタンが最初に押されたときにMOUSEEVENTF_LEFTDOWNが設定されますが、
	/// 後続のモーションには設定されません。同様に、MOUSEEVENTF_LEFTUPは、ボタンが最初に離されたときにのみ設定されます。</para>
	/// <para>両方ともmouseDataフィールドを使用する必要があるため、MOUSEEVENTF_WHEELフラグとMOUSEEVENTF_XDOWNフラグまたはMOUSEEVENTF_XUPフラグの両方をdwFlagsパラメーターで同時に指定することはできません。</para>
	/// <para>MOUSEEVENTF_MOVE            = 0x0001 : 移動が発生しました。</para>
	/// <para>MOUSEEVENTF_LEFTDOWN        = 0x0002 : 移動が発生しました。</para>
	/// <para>MOUSEEVENTF_LEFTUP          = 0x0004 : 左側のボタンが解放されました。</para>
	/// <para>MOUSEEVENTF_RIGHTDOWN       = 0x0008 : 右ボタンが押されました。</para>
	/// <para>MOUSEEVENTF_RIGHTUP         = 0x0010 : 右側のボタンが解放されました。</para>
	/// <para>MOUSEEVENTF_MIDDLEDOWN      = 0x0020 : 中央のボタンが押されました。</para>
	/// <para>MOUSEEVENTF_MIDDLEUP        = 0x0040 : 中央のボタンが解放されました。</para>
	/// <para>MOUSEEVENTF_XDOWN           = 0x0080 : Xボタンが押されました。</para>
	/// <para>MOUSEEVENTF_XUP             = 0x0100 : Xボタンが開放されました。</para>
	/// <para>MOUSEEVENTF_WHEEL           = 0x0800 : マウスにホイールがある場合は、ホイールを移動しました。移動の量はmouseDataで指定されます。</para>
	/// <para>MOUSEEVENTF_HWHEEL          = 0x1000 : マウスにホイールがある場合、ホイールは水平方向に移動しました。移動の量はmouseDataで指定されます。</para>
	/// <para>MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000 : WM_MOUSEMOVEメッセージは結合されません。既定の動作では、WM_MOUSEMOVEメッセージを結合します。</para>
	/// <para>MOUSEEVENTF_VIRTUALDESK     = 0x4000 : 座標をデスクトップ全体にマップします。MOUSEEVENTF_ABSOLUTEで使用する必要があります。</para>
	/// <para>MOUSEEVENTF_ABSOLUTE        = 0x8000 : dxメンバーとdyメンバーには、正規化された絶対座標が含まれています。
	/// フラグが設定されていない場合、dxとdyには相対データ(最後に報告された位置以降の位置の変化)が含まれます。
	/// このフラグは、システムに接続されているマウスやその他のポインティングデバイスの種類に関係なく、設定することも設定することもできません。
	/// 相対マウスモーションの詳細については、次の「解説」セクションを参照してください。</para>
	/// </summary>
	public DWORD dwFlag;
	/// <summary>
	/// イベントのタイムスタンプ(ミリ秒単位)。このパラメーターが0の場合、システムは独自のタイムスタンプを提供します。
	/// </summary>
	public DWORD time;
	/// <summary>
	/// マウスイベントに関連付けられている追加の値。アプリケーションはGetMessageExtraInfoを呼び出して、この追加情報を取得します。
	/// </summary>
	public ULONG_PTR dwExtraInfo;
}
