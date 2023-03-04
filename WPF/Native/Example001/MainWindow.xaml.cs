using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Otchitta.Example001;

/// <summary>
/// 主要画面処理クラスです。
/// </summary>
public partial class MainWindow : Window {
	#region ログ出力処理定義
	/// <summary>
	/// ログ出力処理
	/// </summary>
	private static NLog.ILogger? logger;
	/// <summary>
	/// ログ出力処理を取得します。
	/// </summary>
	/// <returns>ログ出力設定</returns>
	private static NLog.ILogger Logger => logger ??= NLog.LogManager.GetCurrentClassLogger();
	#endregion ログ出力処理定義

	#region メンバー変数定義
	/// <summary>
	/// 結果一覧
	/// </summary>
	private readonly List<string> resultList;
	/// <summary>
	/// 結果一覧
	/// </summary>
	private readonly List<string> windowList;
	#endregion メンバー変数定義

	#region 生成メソッド定義
	/// <summary>
	/// 主要画面処理を生成します。
	/// </summary>
	public MainWindow() {
		InitializeComponent();
		this.resultList = new List<string>(21);
		this.windowList = new List<string>(21);
		var helper = new WindowInteropHelper(this);
		var handle = helper.EnsureHandle();
		var source = HwndSource.FromHwnd(handle);
		Logger.Info($"Window SourceHandle:{helper.Handle:X016}");
		Logger.Info($"Window EnsureHandle:{helper.EnsureHandle():X016}");
		source.AddHook(InvokeSourceData);
	}
	#endregion 生成メソッド定義

	#region 内部メソッド定義(定義情報関連:ChooseSourceData)
	/// <summary>
	/// メッセージ情報を取得します。
	/// </summary>
	/// <param name="source">メッセージ種別</param>
	/// <returns>メッセージ情報</returns>
	/// <seealso cref="https://blog.goo.ne.jp/masaki_goo_2006/e/65fe9047e5f97bde1830566766b4829e" />
	private static (string, string) ChooseSourceData(int source) {
		switch (source) {
			default:     return ("UNKNOWN",                   "不明なメッセージです。");
			case 0x0000: return ("WM_NULL", "特に意味はありません。特定のウインドウにこのメッセージを投げてタイムアウトするかどうかで生存確認を行う事ができます。");
			case 0x0001: return ("WM_CREATE", "ウインドウが作成されていることを示します。");
			case 0x0002: return ("WM_DESTROY", "ウインドウが破棄されようとしていることを示します。");
			case 0x0003: return ("WM_MOVE", "ウインドウの位置が変更されたことを示します。");
			case 0x0005: return ("WM_SIZE", "ウインドウのサイズが変更されていることを示します。");
			case 0x0006: return ("WM_ACTIVATE", "アクティブ状態が変更されていることを示します。");
			case 0x0007: return ("WM_SETFOCUS", "ウインドウがキーボード・フォーカスを取得したことを示します。");
			case 0x0008: return ("WM_KILLFOCUS", "ウインドウがキーボード・フォーカスを失っていることを示します。");
			case 0x000A: return ("WM_ENABLE", "ウインドウの有効または無効の状態が変更されていることを示します。");
			case 0x000B: return ("WM_SETREDRAW", "ウインドウ内の再描画を許可または禁止します。");
			case 0x000C: return ("WM_SETTEXT", "ウインドウのテキストを設定します。");
			case 0x000D: return ("WM_GETTEXT", "ウインドウに対応するテキストを取得します。");
			case 0x000E: return ("WM_GETTEXTLENGTH", "ウインドウに関連付けられているテキストの長さを取得します。");
			case 0x000F: return ("WM_PAINT", "ウインドウのクライアント領域を描画する必要があることを示します。");
			case 0x0010: return ("WM_CLOSE", "コントロール・メニューの[クローズ]コマンドが選ばれました。");
			case 0x0011: return ("WM_QUERYENDSESSION", "Windowsセッションを終了するよう要求します。");
			case 0x0012: return ("WM_QUIT", "アプリケーションを強制終了するよう要求します。");
			case 0x0013: return ("WM_QUERYOPEN", "アイコン化ウインドウを復元するよう要求します。");
			case 0x0014: return ("WM_ERASEBKGND", "ウインドウの背景を消去する必要があることを示します。");
			case 0x0015: return ("WM_SYSCOLORCHANGE", "システム・カラーの値が変更されたことを示します。");
			case 0x0016: return ("WM_ENDSESSION", "Windowsセッションが終了することを示します。");
			case 0x0017: return ("WM_SYSTEMERROR", "(Win32ではもはや用いられません)");
			case 0x0018: return ("WM_SHOWWINDOW", "ウインドウの表示または非表示の状態が変更されようとしていることを示します。");
			case 0x0019: return ("WM_CTLCOLOR", "子コントロールが描画される直前であることを示します。");
			case 0x001A: return ("WM_WININICHANGE", "WIN.INIが変更されたことをアプリケーションに通知します。Windowsの設定が変更されたことをアプリケーションに通知します。");
			case 0x001B: return ("WM_DEVMODECHANGE", "デバイス モードの設定が変更されたことを示します。");
			case 0x001C: return ("WM_ACTIVATEAPP", "新しいタスクがアクティブになるタイミングをアプリケーションに通知します。");
			case 0x001D: return ("WM_FONTCHANGE", "フォント リソース プールが変更されていることを示します。");
			case 0x001E: return ("WM_TIMECHANGE", "システム時刻が設定されたことを示します。");
			case 0x001F: return ("WM_CANCELMODE", "内部モードをキャンセルするようウインドウに通知します。");
			case 0x0020: return ("WM_SETCURSOR", "マウス カーソルの形状を設定するようウインドウに促します。");
			case 0x0021: return ("WM_MOUSEACTIVATE", "非アクティブ ウインドウ内でマウスがクリックされたことを示します。");
			case 0x0022: return ("WM_CHILDACTIVATE", "子ウインドウにアクティブであることを通知します。");
			case 0x0023: return ("WM_QUEUESYNC", "CBTメッセージを区切ります。");
			case 0x0024: return ("WM_GETMINMAXINFO", "アイコン表示時および最大表示時のサイズ情報を取得します。");
			case 0x0026: return ("WM_PAINTICON", "アイコンが描画されようとしています。");
			case 0x0027: return ("WM_ICONERASEBKGND", "アイコンの背景を塗りつぶすようアイコン化ウインドウに通知します。");
			case 0x0028: return ("WM_NEXTDLGCTL", "フォーカスを別のダイアログ ボックス コントロールに設定します。");
			case 0x002A: return ("WM_SPOOLERSTATUS", "印刷ジョブが追加または削除されたことを示します。(XP 以降ではサポートされません)");
			case 0x002B: return ("WM_DRAWITEM", "オーナー描画コントロールまたはオーナー描画メニューを再描画する必要があることを示します。");
			case 0x002C: return ("WM_MEASUREITEM", "オーナー描画のコントロールまたは項目の寸法を要求します。");
			case 0x002D: return ("WM_DELETEITEM", "ほかのオーナー描画項目またはオーナー描画コントロールに代わったことを示します。");
			case 0x002E: return ("WM_VKEYTOITEM", "リスト ボックスのキーストロークをそのオーナー ウインドウに提供します。");
			case 0x002F: return ("WM_CHARTOITEM", "リスト ボックスのキーストロークをそのオーナー ウインドウに提供します。");
			case 0x0030: return ("WM_SETFONT", "コントロールで使われるフォントを設定します。");
			case 0x0031: return ("WM_GETFONT", "コントロールで使われているフォントを取得します。");
			case 0x0032: return ("WM_SETHOTKEY", "ウインドウにホット キーを関連付けます。");
			case 0x0033: return ("WM_GETHOTKEY", "ウインドウのホット キーの仮想キー コードを取得します。");
			case 0x0037: return ("WM_QUERYDRAGICON", "アイコン化ウインドウに対してマウス カーソルのハンドルを要求します。");
			case 0x0039: return ("WM_COMPAREITEM", "コンボ ボックスまたはリスト ボックスの項目位置を判断します。");
			case 0x003D: return ("WM_GETOBJECT", "");
			case 0x0041: return ("WM_COMPACTING", "メモリ不足状態であることを示します。");
			case 0x0044: return ("WM_COMMNOTIFY", "(Win32 ではもはや用いられません)");
			case 0x0046: return ("WM_WINDOWPOSCHANGING", "ウインドウに新しいサイズまたは位置を通知します。");
			case 0x0047: return ("WM_WINDOWPOSCHANGED", "ウインドウにサイズまたは位置の変更を通知します。");
			case 0x0048: return ("WM_POWER", "システムが中断モードに入っていることを示します。");
			case 0x004A: return ("WM_COPYDATA", "ほかのアプリケーションにデータを渡します。");
			case 0x004B: return ("WM_CANCELJOURNAL", "ユーザーがジャーナル モードをキャンセルしました。");
			case 0x004E: return ("WM_NOTIFY", "");
			case 0x0050: return ("WM_INPUTLANGCHANGEREQUEST", "");
			case 0x0051: return ("WM_INPUTLANGCHANGE", "");
			case 0x0052: return ("WM_TCARD", "");
			case 0x0053: return ("WM_HELP", "");
			case 0x0054: return ("WM_USERCHANGED", "ユーザがログオン/ログオフしたことを示します。");
			case 0x0055: return ("WM_NOTIFYFORMAT", "");
			case 0x007B: return ("WM_CONTEXTMENU", "");
			case 0x007C: return ("WM_STYLECHANGING", "SetWindowLong() によってウインドウのスタイルが変更されようとしています。");
			case 0x007D: return ("WM_STYLECHANGED", "SetWindowLong() によってウインドウのスタイルが変更されました。");
			case 0x007E: return ("WM_DISPLAYCHANGE", "ディスプレイの解像度が変更されたことを示します。");
			case 0x007F: return ("WM_GETICON", "");
			case 0x0080: return ("WM_SETICON", "");
			case 0x0081: return ("WM_NCCREATE", "ウインドウの非クライアント領域が作成されていることを示します。");
			case 0x0082: return ("WM_NCDESTROY", "ウインドウの非クライアント領域が破棄されていることを示します。");
			case 0x0083: return ("WM_NCCALCSIZE", "ウインドウのクライアント領域のサイズを計算します。");
			case 0x0084: return ("WM_NCHITTEST", "マウス カーソルが移動したことを示します。");
			case 0x0085: return ("WM_NCPAINT", "ウインドウの枠を描画する必要があることを示します。");
			case 0x0086: return ("WM_NCACTIVATE", "非クライアント領域のアクティブ状態を変更します。");
			case 0x0087: return ("WM_GETDLGCODE", "ダイアログ プロシージャがコントロール入力を処理できるようにします。");
			case 0x00A0: return ("WM_NCMOUSEMOVE", "非クライアント領域でマウス カーソルが移動したことを示します。");
			case 0x00A1: return ("WM_NCLBUTTONDOWN", "非クライアント領域でマウスの左ボタンが押されたことを示します。");
			case 0x00A2: return ("WM_NCLBUTTONUP", "非クライアント領域でマウスの左ボタンが離されたことを示します。");
			case 0x00A3: return ("WM_NCLBUTTONDBLCLK", "非クライアント領域でマウスの左ボタンをダブルクリックしたことを示します。");
			case 0x00A4: return ("WM_NCRBUTTONDOWN", "非クライアント領域でマウスの右ボタンが押されたことを示します。");
			case 0x00A5: return ("WM_NCRBUTTONUP", "非クライアント領域でマウスの右ボタンが離されたことを示します。");
			case 0x00A6: return ("WM_NCRBUTTONDBLCLK", "非クライアント領域でマウスの右ボタンをダブルクリックしたことを示します。");
			case 0x00A7: return ("WM_NCMBUTTONDOWN", "非クライアント領域でマウスの中央ボタンが押されたことを示します。");
			case 0x00A8: return ("WM_NCMBUTTONUP", "非クライアント領域でマウスの中央ボタンが離されたことを示します。");
			case 0x00A9: return ("WM_NCMBUTTONDBLCLK", "非クライアント領域でマウスの中央ボタンをダブルクリックしたことを示します。");
			case 0x00AB: return ("WM_NCXBUTTONDOWN", "非クライアント領域でマウスの 4 つ目以降のボタンが押されたことを示します。");
			case 0x00AC: return ("WM_NCXBUTTONUP", "非クライアント領域でマウスの 4 つ目以降のボタンが離されたことを示します。");
			case 0x00AD: return ("WM_NCXBUTTONDBLCLK", "非クライアント領域でマウスの 4 つ目以降のボタンをダブルクリックしたことを示します。");
			case 0x00FE: return ("WM_INPUT_DEVICE_CHANGE", "");
			case 0x00FF: return ("WM_INPUT", "RAW Input Device (キーボード/マウス/リモコン等) からの入力があったことを示します。");
			//case 0x0100: return ("WM_KEYFIRST", "");
			case 0x0100: return ("WM_KEYDOWN", "非システム キーが押されたことを示します。");
			case 0x0101: return ("WM_KEYUP", "非システム キーが離されたことを示します。");
			case 0x0102: return ("WM_CHAR", "ユーザーが文字キーを押したことを示します。");
			case 0x0103: return ("WM_DEADCHAR", "ユーザーがデッド キーを押したことを示します。");
			case 0x0104: return ("WM_SYSKEYDOWN", "Alt+任意のキーが押されたことを示します。");
			case 0x0105: return ("WM_SYSKEYUP", "Alt+任意のキーが離されたことを示します。");
			case 0x0106: return ("WM_SYSCHAR", "コントロール メニュー キーが押されたことを示します。");
			case 0x0107: return ("WM_SYSDEADCHAR", "システム デッド キーが押されたを示します。");
			case 0x0109: return ("WM_UNICHAR", "");
			//case 0x0109: return ("WM_KEYLAST", "");
			case 0x010D: return ("WM_IME_STARTCOMPOSITION", "");
			case 0x010E: return ("WM_IME_ENDCOMPOSITION", "");
			case 0x010F: return ("WM_IME_COMPOSITION", "");
			//case 0x010F: return ("WM_IME_KEYLAST", "");
			case 0x0110: return ("WM_INITDIALOG", "ダイアログ ボックスを初期化します。");
			case 0x0111: return ("WM_COMMAND", "コマンド メッセージを指定します。");
			case 0x0112: return ("WM_SYSCOMMAND", "システム コマンドが要求されたことを示します。");
			case 0x0113: return ("WM_TIMER", "タイマのタイムアウト時間が経過したことを示します。");
			case 0x0114: return ("WM_HSCROLL", "水平スクロール バーがクリックされたことを示します。");
			case 0x0115: return ("WM_VSCROLL", "垂直スクロール バーがクリックされたことを示します。");
			case 0x0116: return ("WM_INITMENU", "メニューがアクティブ化されようとしていることを示します。");
			case 0x0117: return ("WM_INITMENUPOPUP", "ポップアップ メニューが作成されていることを示します。");
			case 0x0119: return ("WM_GESTURE", "ジェスチャに関する情報を渡します。");
			case 0x011A: return ("WM_GESTURENOTIFY", "ジェスチャ構成を設定できます。");
			case 0x011F: return ("WM_MENUSELECT", "ユーザーがメニュー項目を選択したことを示します。");
			case 0x0120: return ("WM_MENUCHAR", "未知のメニュー ニーモニックが押されたを示します。");
			case 0x0121: return ("WM_ENTERIDLE", "モーダル ダイアログ ボックスまたはメニューがアイドルであることを示します。");
			case 0x0122: return ("WM_MENURBUTTONUP", "メニュー項目にカーソルがある状態でマウスの右ボタンが離されたことを示します。");
			case 0x0123: return ("WM_MENUDRAG", "");
			case 0x0124: return ("WM_MENUGETOBJECT", "");
			case 0x0125: return ("WM_UNINITMENUPOPUP", "");
			case 0x0126: return ("WM_MENUCOMMAND", "");
			case 0x0127: return ("WM_CHANGEUISTATE", "");
			case 0x0128: return ("WM_UPDATEUISTATE", "");
			case 0x0129: return ("WM_QUERYUISTATE", "");
			case 0x0132: return ("WM_CTLCOLORMSGBOX", "メッセージ ボックスが描画されようとしています。");
			case 0x0133: return ("WM_CTLCOLOREDIT", "エディット コントロールが描画されようとしています。");
			case 0x0134: return ("WM_CTLCOLORLISTBOX", "リスト ボックスが描画されようとしています。");
			case 0x0135: return ("WM_CTLCOLORBTN", "ボタンが描画されようとしています。");
			case 0x0136: return ("WM_CTLCOLORDLG", "ダイアログ ボックスが描画されようとしています。");
			case 0x0137: return ("WM_CTLCOLORSCROLLBAR", "スクロール バーが描画されようとしていることを示します。");
			case 0x0138: return ("WM_CTLCOLORSTATIC", "スタティック コントロールが描画されようとしています。");
			//case 0x0200: return ("WM_MOUSEFIRST", "");
			case 0x0200: return ("WM_MOUSEMOVE", "マウス カーソルが移動したことを示します。");
			case 0x0201: return ("WM_LBUTTONDOWN", "左のマウス ボタンがいつ押されたかを示します。");
			case 0x0202: return ("WM_LBUTTONUP", "左のマウス ボタンがいつ離されたかを示します。");
			case 0x0203: return ("WM_LBUTTONDBLCLK", "マウスの左ボタンをダブルクリックしたことを示します。");
			case 0x0204: return ("WM_RBUTTONDOWN", "マウスの右ボタンがいつ押されたかを示します。");
			case 0x0205: return ("WM_RBUTTONUP", "マウスの右ボタンがいつ離されたかを示します。");
			case 0x0206: return ("WM_RBUTTONDBLCLK", "マウスの右ボタンをダブルクリックしたことを示します。");
			case 0x0207: return ("WM_MBUTTONDOWN", "中央のマウス ボタンがいつ押されたかを示します。");
			case 0x0208: return ("WM_MBUTTONUP", "中央のマウス ボタンがいつ離されたかを示します。");
			case 0x0209: return ("WM_MBUTTONDBLCLK", "マウスの中央ボタンをダブルクリックしたことを示します。");
			case 0x020A: return ("WM_MOUSEWHEEL", "マウス ホイールが回転した事を示します。");
			case 0x020B: return ("WM_XBUTTONDOWN", "マウスの 4 つ目以降のボタンがいつ押されたかを示します。");
			case 0x020C: return ("WM_XBUTTONUP", "マウスの 4 つ目以降のボタンがいつ離されたかを示します。");
			case 0x020D: return ("WM_XBUTTONDBLCLK", "マウスの 4 つ目以降のボタンをダブルクリックしたことを示します。");
			case 0x020E: return ("WM_MOUSEHWHEEL", "マウス ホイールが回転した事を示します。");
			//case 0x020E: return ("WM_MOUSELAST", "");
			case 0x0210: return ("WM_PARENTNOTIFY", "親ウインドウに子ウインドウのアクティブ状態を通知します。");
			case 0x0211: return ("WM_ENTERMENULOOP", "メニューのモーダル ループを開始します。");
			case 0x0212: return ("WM_EXITMENULOOP", "メニューのモーダル ループを終了します。");
			case 0x0213: return ("WM_NEXTMENU", "");
			case 0x0214: return ("WM_SIZING", "");
			case 0x0215: return ("WM_CAPTURECHANGED", "");
			case 0x0216: return ("WM_MOVING", "");
			case 0x0218: return ("WM_POWERBROADCAST", "");
			case 0x0219: return ("WM_DEVICECHANGE", "");
			case 0x0220: return ("WM_MDICREATE", "子ウインドウを作成するようMDIクライアントに促します。");
			case 0x0221: return ("WM_MDIDESTROY", "MDI子ウインドウをクローズします。");
			case 0x0222: return ("WM_MDIACTIVATE", "MDI子ウインドウをアクティブ化します。");
			case 0x0223: return ("WM_MDIRESTORE", "子ウインドウを復元するようMDIクライアントに促します。");
			case 0x0224: return ("WM_MDINEXT", "次のMDI子ウインドウをアクティブ化します。");
			case 0x0225: return ("WM_MDIMAXIMIZE", "MDI子ウインドウを最大化します。");
			case 0x0226: return ("WM_MDITILE", "MDI子ウインドウを並べて整列させます。");
			case 0x0227: return ("WM_MDICASCADE", "MDI子ウインドウを重ねて整列させます。");
			case 0x0228: return ("WM_MDIICONARRANGE", "アイコン化されたMDI子ウインドウを整列します。");
			case 0x0229: return ("WM_MDIGETACTIVE", "アクティブなMDI子ウインドウに関するデータを取得します。");
			case 0x0230: return ("WM_MDISETMENU", "MDIフレーム ウインドウのメニューを置き換えます。");
			case 0x0231: return ("WM_ENTERSIZEMOVE", "ウインドウのサイズ変更/移動が行われる前に通知されます。");
			case 0x0232: return ("WM_EXITSIZEMOVE", "ウインドウのサイズ変更/移動が行われた後に通知されます。");
			case 0x0233: return ("WM_DROPFILES", "ファイルがドロップされたことを示します。");
			case 0x0234: return ("WM_MDIREFRESHMENU", "MDIフレーム ウインドウのメニューを最新表示します。");
			case 0x0240: return ("WM_TOUCH", "1 つ以上の接触点 (指やペンなど) がタッチセンサー式デジタイザーの表面に触れたときにウインドウに通知します。");
			case 0x0281: return ("WM_IME_SETCONTEXT", "");
			case 0x0282: return ("WM_IME_NOTIFY", "");
			case 0x0283: return ("WM_IME_CONTROL", "");
			case 0x0284: return ("WM_IME_COMPOSITIONFULL", "");
			case 0x0285: return ("WM_IME_SELECT", "");
			case 0x0286: return ("WM_IME_CHAR", "");
			case 0x0288: return ("WM_IME_REQUEST", "");
			case 0x0290: return ("WM_IME_KEYDOWN", "");
			case 0x0291: return ("WM_IME_KEYUP", "");
			case 0x02A0: return ("WM_NCMOUSEHOVER", "TrackMouseEvent の前回の呼び出しで指定されている時間のあいだカーソルがウインドウの非クライアント領域に置かれていました。");
			case 0x02A1: return ("WM_MOUSEHOVER", "マウスがウインドウのクライアントエリア上でホバリングしてから、TrackMouseEvent 関数への呼び出しであらかじめ指定された時間が経過しました。");
			case 0x02A2: return ("WM_NCMOUSELEAVE", "マウスが、TrackMouseEvent の前回の呼び出しで指定されている時間のあいだカーソルがウインドウの非クライアント領域から出ていました。");
			case 0x02A3: return ("WM_MOUSELEAVE", "マウスが、TrackMouseEvent 関数への呼び出しであらかじめ指定されたウインドウのクライアントエリアを離れました。");
			case 0x02B1: return ("WM_WTSSESSION_CHANGE", "ユーザーの簡易切り替えが行われました。");
			case 0x02C0: return ("WM_TABLET_DEFBASE", "");
			case 0x02C8: return ("WM_TABLET_ADDED", "");
			case 0x02C9: return ("WM_TABLET_DELETED", "");
			case 0x02CB: return ("WM_TABLET_FLICK", "フリック入力があったことを示します。");
			case 0x02CC: return ("WM_TABLET_QUERYSYSTEMGESTURESTATUS", "");
			case 0x0300: return ("WM_CUT", "選択項目を削除し、 クリップボードにコピーします。");
			case 0x0301: return ("WM_COPY", "クリップボードに選択項目をコピーします。");
			case 0x0302: return ("WM_PASTE", "クリップボード データをエディット コントロールに挿入します。");
			case 0x0303: return ("WM_CLEAR", "エディット コントロールをクリアします。");
			case 0x0304: return ("WM_UNDO", "エディット コントロール内での直前の操作を取り消します。");
			case 0x0305: return ("WM_RENDERFORMAT", "クリップボード データをレンダするようオーナーに通知します。");
			case 0x0306: return ("WM_RENDERALLFORMATS", "すべてのクリップボード形式をレンダするようオーナーに通知します。");
			case 0x0307: return ("WM_DESTROYCLIPBOARD", "クリップボードが空になったことをオーナーに通知します。");
			case 0x0308: return ("WM_DRAWCLIPBOARD", "クリップボードの内容が変更されたことを示します。");
			case 0x0309: return ("WM_PAINTCLIPBOARD", "クリップボードの内容を表示するようオーナーに促します。");
			case 0x030A: return ("WM_VSCROLLCLIPBOARD", "クリップボードの内容をスクロールするようオーナーに促します。");
			case 0x030B: return ("WM_SIZECLIPBOARD", "クリップボードのサイズが変更されていることを示します。");
			case 0x030C: return ("WM_ASKCBFORMATNAME", "新しいタスクがアクティブになるタイミングをアプリケーションに通知します。");
			case 0x030D: return ("WM_CHANGECBCHAIN", "クリップボード ビューアのチェインからの除去を通知します。");
			case 0x030E: return ("WM_HSCROLLCLIPBOARD", "クリップボードの内容をスクロールするようオーナーに促します。");
			case 0x030F: return ("WM_QUERYNEWPALETTE", "ウインドウがその論理パレットを実現できるようにします。");
			case 0x0310: return ("WM_PALETTEISCHANGING", "パレットが変更されていることを各ウインドウに通知します。");
			case 0x0311: return ("WM_PALETTECHANGED", "フォーカス ウインドウがそのパレットを実現したことを示します。");
			case 0x0312: return ("WM_HOTKEY", "ホット キーが検出されています。");
			case 0x0317: return ("WM_PRINT", "");
			case 0x0318: return ("WM_PRINTCLIENT", "");
			case 0x0319: return ("WM_APPCOMMAND", "アプリケーション コマンドが要求されたことを示します。");
			case 0x031A: return ("WM_THEMECHANGED", "Windows のテーマが変更された事を示します。");
			case 0x031D: return ("WM_CLIPBOARDUPDATE", "");
			case 0x031E: return ("WM_DWMCOMPOSITIONCHANGED", "DWM 合成の設定が変更された事を示します。");
			case 0x031F: return ("WM_DWMNCRENDERINGCHANGED", "DWM レンダリングがクライアント領域外で変更された事を示します。");
			case 0x0320: return ("WM_DWMCOLORIZATIONCOLORCHANGED", "DWM 合成の基準となる配色が変更された事を示します。");
			case 0x0321: return ("WM_DWMWINDOWMAXIMIZEDCHANGE", "DWM 合成ウインドウが最大化または最大化解除された事を示します。");
			case 0x0323: return ("WM_DWMSENDICONICTHUMBNAIL", "");
			case 0x0326: return ("WM_DWMSENDICONICLIVEPREVIEWBITMAP", "");
			case 0x033F: return ("WM_GETTITLEBARINFOEX", "");
			case 0x0358: return ("WM_HANDHELDFIRST", "");
			case 0x0359: return ("WM_HANDHELDLAST", "");
			case 0x0380: return ("WM_PENWINFIRST", "");
			case 0x038F: return ("WM_PENWINLAST", "");
			case 0x0390: return ("WM_COALESCE_FIRST", "");
			case 0x039F: return ("WM_COALESCE_LAST", "");
			//case 0x03E0: return ("WM_DDE_FIRST", "");
			case 0x03E0: return ("WM_DDE_INITIATE", "DDE対話を開始します。");
			case 0x03E1: return ("WM_DDE_TERMINATE", "DDE対話を終了します。");
			case 0x03E2: return ("WM_DDE_ADVISE", "DDEデータ変更の更新を要求します。");
			case 0x03E3: return ("WM_DDE_UNADVISE", "DDEデータの更新要求を停止させます。");
			case 0x03E4: return ("WM_DDE_ACK", "DDEメッセージに対して受領通知をします。");
			case 0x03E5: return ("WM_DDE_DATA", "データををDDEクライアントに送ります。");
			case 0x03E6: return ("WM_DDE_REQUEST", "DDEサーバーからデータを要求します。");
			case 0x03E7: return ("WM_DDE_POKE", "未要求のデータをサーバーに送ります。");
			case 0x03E8: return ("WM_DDE_EXECUTE", "文字列をDDEサーバーに送ります。");
			case 0x0400: return ("WM_USER", "メッセージ値の範囲を示します。 0x0000..WM_USER-1 はシステム予約です。WM_USER..WM_APP-1 は Windows で利用されます。");
			case 0x0401: return ("WM_CHOOSEFONT_GETLOGFONT", "[フォントの指定]ダイアログ ボックスのLOGFONT構造体を取得します。");
			case 0x8000: return ("WM_APP", "WM_APP..WM_APP + 0x3FFF はアプリケーションで自由に定義できます。");
			case 0xCCCD: return ("WM_RASDIALEVENT", "RAS接続状態が変更されたことを通知します。");
		}
	}
	#endregion 内部メソッド定義(定義情報関連:ChooseSourceData)

	#region 内部メソッド定義(結果内容関連:UpdateResultText/UpdateWindowText)
	/// <summary>
	/// 結果内容を更新します。
	/// </summary>
	/// <param name="source">結果情報</param>
	private void UpdateResultText(string source) {
		this.resultList.Add(source);
		while (20 < this.resultList.Count) {
			this.resultList.RemoveAt(0);
		}
		ResultText.Text = String.Join("\r\n", this.resultList);
	}
	/// <summary>
	/// 結果内容を更新します。
	/// </summary>
	/// <param name="source">結果情報</param>
	private void UpdateWindowText(string source) {
		Logger.Info(source);
		this.windowList.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}:{source}");
		while (20 < this.windowList.Count) {
			this.windowList.RemoveAt(0);
		}
		WindowText.Text = String.Join("\r\n", this.windowList);
	}
	#endregion 内部メソッド定義(結果内容関連:UpdateResultText/UpdateWindowText)

	#region 内部メソッド定義(イベント関連:InvokeSourceData/ActionFinishMenu/ActionMouseDown/ActionMouseUp)
	/// <summary>
	/// 通知情報を実行します。
	/// </summary>
	/// <param name="handle">ウィンドウハンドル</param>
	/// <param name="source">メッセージ種別</param>
	/// <param name="wParam">メッセージ引数</param>
	/// <param name="lParam">メッセージ引数</param>
	/// <param name="finish">実行完了フラグ</param>
	/// <returns>実行結果</returns>
	private IntPtr InvokeSourceData(IntPtr handle, int source, IntPtr wParam, IntPtr lParam, ref bool finish) {
		if (source == 0x00000084) {
			// 処理なし
		} else {
			var choose = ChooseSourceData(source);
			var result = $"{DateTime.Now.ToString("HH:mm:ss.fff")}:{source:X08}[{wParam:X016}-{lParam:X016}]{choose.Item1}";
			UpdateResultText(result);
			Logger.Info($"{source:X08}[{wParam:X016}-{lParam:X016}] {choose.Item1} - {choose.Item2}");
		}
		return IntPtr.Zero;
	}
	/// <summary>
	/// 終了操作を実行します。
	/// </summary>
	/// <param name="sender">発行情報</param>
	/// <param name="values">発行引数</param>
	private void ActionFinishMenu(object? sender, RoutedEventArgs values) =>
		Close();
	/// <summary>
	/// マウスボタン押下イベントを処理します。
	/// </summary>
	/// <param name="sender">発行情報</param>
	/// <param name="values">発行引数</param>
	private void ActionMouseDown(object? sender, MouseButtonEventArgs values) =>
		UpdateWindowText($"押下:左[{values.LeftButton,-8}] 中[{values.MiddleButton,-8}] 右[{values.RightButton,-8}] 座標[{values.GetPosition(this)}]");
	/// <summary>
	/// マウスボタン開放イベントを処理します。
	/// </summary>
	/// <param name="sender">発行情報</param>
	/// <param name="values">発行引数</param>
	private void ActionMouseUp(object? sender, MouseButtonEventArgs values) =>
		UpdateWindowText($"開放:左[{values.LeftButton,-8}] 中[{values.MiddleButton,-8}] 右[{values.RightButton,-8}] 座標[{values.GetPosition(this)}]");
	#endregion 内部メソッド定義(イベント関連:InvokeSourceData/ActionFinishMenu/ActionMouseDown/ActionMouseUp)
}
