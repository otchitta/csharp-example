using System;
using System.Runtime.InteropServices;
using BOOL = System.Boolean;
using HWND = System.IntPtr;
using LPARAM = System.IntPtr;
using LPINPUT = Otchitta.Example002.Native.INPUT;
using UINT = System.UInt32;

namespace Otchitta.Example002.Native;

/// <summary>
/// WindowsAPI共通関数クラスです。
/// </summary>
internal static class NativeMethod {
	#region 外部メソッド定義
	/// <summary>
	/// 指定したポイントのクライアント領域座標を画面座標に変換します。
	/// </summary>
	/// <param name="hWnd">変換にクライアント領域が使用されるウィンドウへのハンドル。</param>
	/// <param name="lpPoint">変換するクライアント座標を含むPOINT構造体へのポインター。関数が成功すると、新しい画面座標がこの構造体にコピーされます。</param>
	/// <returns>関数が成功すると、戻り値は 0 以外になります。
	/// 関数が失敗した場合は、0 を返します。</returns>
	[DllImport("user32.dll", EntryPoint="ClientToScreen", SetLastError=true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern BOOL ClientToScreen(HWND hWnd, ref POINT lpPoint);
	/// <summary>
	/// キーストローク、マウスの動き、ボタンのクリックを合成します。
	/// </summary>
	/// <param name="cInputs">pInputs配列内の構造体の数。</param>
	/// <param name="pInputs">INPUT構造体の配列。各構造体は、キーボードまたはマウス入力ストリームに挿入されるイベントを表します。</param>
	/// <param name="cbSize">INPUT構造体のサイズ(バイト単位)。cbSizeがINPUT構造体のサイズでない場合、関数は失敗します。</param>
	/// <returns>キーボードまたはマウス入力ストリームに正常に挿入されたイベントの数を返します。
	/// 関数から0が返された場合、入力は別のスレッドによって既にブロックされています。
	/// 詳細なエラー情報を得るには、GetLastErrorを呼び出します。
	/// UIPIによってブロックされると失敗します。GetLastErrorも戻り値も、エラーがUIPIのブロックによって引き起こされたことを示しません。</returns>
	[DllImport("user32.dll", EntryPoint="SendInput", SetLastError=true)]
	private static extern UINT SendInput(UINT cInputs, LPINPUT[] pInputs, int cbSize);
	/// <summary>
	/// 現在のスレッドの追加のメッセージ情報を取得します。
	/// 追加のメッセージ情報は、現在のスレッドのメッセージキューに関連付けられているアプリケーションまたはドライバー定義の値です。
	/// </summary>
	/// <returns>戻り値は追加情報を指定します。 追加情報の意味は、デバイス固有です。</returns>
	[DllImport("user32.dll", EntryPoint="GetMessageExtraInfo", SetLastError=true)]
	private static extern LPARAM GetMessageExtraInfo();
	/// <summary>
	/// 指定したシステムメトリックまたはシステム構成設定を取得します。
	/// <para>GetSystemMetricsによって取得されるすべてのディメンションはピクセル単位であることに注意してください。</para>
	/// </summary>
	/// <param name="nIndex">取得するシステム メトリックまたは構成設定。</param>
	/// <returns></returns>
	[DllImport("user32.dll", EntryPoint="GetSystemMetrics", SetLastError=true)]
	private static extern int GetSystemMetrics(int nIndex);
	#endregion 外部メソッド定義

	#region メンバー定数定義
	private const uint MOUSEEVENTF_MOVE            = 0x0001;
	private const uint MOUSEEVENTF_LEFTDOWN        = 0x0002;
	private const uint MOUSEEVENTF_LEFTUP          = 0x0004;
	private const uint MOUSEEVENTF_ABSOLUTE        = 0x8000;

	private const int SM_CXSCREEN = 0;
	private const int SM_CYSCREEN = 1;
	#endregion メンバー定数定義

	#region 内部メソッド定義(WindowsAPI関連:SendInput)
	/// <summary>
	/// 入力情報を送信します。
	/// </summary>
	/// <param name="pInputs">入力配列</param>
	/// <returns>実行個数</returns>
	private static UINT SendInput(params LPINPUT[] pInputs) =>
		SendInput((UINT)pInputs.Length, pInputs, Marshal.SizeOf(typeof(INPUT)));
	#endregion 内部メソッド定義(WindowsAPI関連:SendInput)

	#region 内部メソッド関連(入力情報関連:CreateMouseData)
	/// <summary>
	/// マウスの移動情報を生成します。
	/// </summary>
	/// <param name="mouseX">X座標(絶対値)</param>
	/// <param name="mouseY">Y座標(絶対値)</param>
	/// <returns>マウスの移動情報</returns>
	private static INPUT CreateMouseData(int mouseX, int mouseY) =>
		INPUT.Create(new MOUSEINPUT() {
			dx = mouseX * 65536 / GetSystemMetrics(SM_CXSCREEN),
			dy = mouseY * 65536 / GetSystemMetrics(SM_CYSCREEN),
			mouseData = 0,
			dwFlag = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
			time = 0,
			//dwExtraInfo = GetMessageExtraInfo()
			dwExtraInfo = IntPtr.Zero
		});
	/// <summary>
	/// マウスの操作情報を生成します。
	/// </summary>
	/// <param name="action">ボタン状態</param>
	/// <returns>マウスの操作情報</returns>
	private static INPUT CreateMouseData(uint action) =>
		INPUT.Create(new MOUSEINPUT() {
			dx = 0,
			dy = 0,
			mouseData = 0,
			dwFlag = action,
			time = 0,
			//dwExtraInfo = GetMessageExtraInfo()
			dwExtraInfo = IntPtr.Zero
		});
	#endregion 内部メソッド関連(入力情報関連:CreateMouseData)

	#region 公開メソッド定義
	/// <summary>
	/// マウスのカーソルを移動します。
	/// </summary>
	/// <param name="mouseX">X座標(絶対値)</param>
	/// <param name="mouseY">Y座標(絶対値)</param>
	/// <returns>移動に成功した場合、<c>True</c>を返却</returns>
	public static bool ActionMouseMove(int mouseX, int mouseY) =>
		SendInput(CreateMouseData(mouseX, mouseY)) == 1;
	/// <summary>
	/// マウスの左ボタンを押下します。
	/// </summary>
	/// <returns>押下に成功した場合、<c>True</c>を返却</returns>
	public static bool ActionMouseLButtonDown() =>
		SendInput(CreateMouseData(MOUSEEVENTF_LEFTDOWN)) == 1;
	/// <summary>
	/// マウスの左ボタンを開放します。
	/// </summary>
	/// <returns>開放に成功した場合、<c>True</c>を返却</returns>
	public static bool ActionMouseLButtonUp() =>
		SendInput(CreateMouseData(MOUSEEVENTF_LEFTUP)) == 1;
	/// <summary>
	/// 指定ウィンドウに対してマウスクリックを実行します。
	/// </summary>
	/// <param name="window">ウィンドウハンドル</param>
	/// <param name="valueX">ウィンドウの左上からの相対X座標</param>
	/// <param name="valueY">ウィンドウの左上からの相対Y座標</param>
	/// <returns>クリックに成功した場合、<c>True</c>を返却</returns>
	public static bool ActionMouseLButtonClick(IntPtr window, int valueX, int valueY) {
		var source = new POINT() { x = valueX, y = valueY };
		if (ClientToScreen(window, ref source) == false) {
			return false;
		} else {
			return SendInput(CreateMouseData(source.x, source.y), CreateMouseData(MOUSEEVENTF_LEFTDOWN), CreateMouseData(MOUSEEVENTF_LEFTUP)) == 3;
		}
	}
	#endregion 公開メソッド定義
}
