## DataGridのサンプルプログラム（其の２）
### サンプルプログラムについて
画面の操作（追加・削除）にて動的に列のバインドを行い画面へ反映するサンプルプログラムとなります。  
※Example001を少し改修するのみの予定でしたが、かなりの変更となりました（ほぼ全て変わっている）。

### 実装ポイント
1. `DataGridBehavior`にて列の名称と表示をビューモデルにバインドしている（`DataGridBehavior`内の`CreateColumn`メソッド内を参照）
1. `DataGridBehavior`を利用して`MainWindow.xaml.cs`内のコードビハインドを除去している（ソースコードの流用性が目的であるが`DataGridBehavior`内にて`ColumnViewModel`を直接扱ってる為、あまり意味がない）
1. `DataGrid.Columns.Clear()`を実行するとxamlで定義した列[^xaml_column]まで削除されてしまう為、`DataGridBehavior+ColumnsHandler`内にて削除を行うかどうかの判別を行っている（`DataGridBehavior`内の`CreateColumn`と`RemoveColumn`を参照）

### 未実装項目について
以下の内容は実装していない
* 列一覧側でのドラッグ＆ドロップにて順序入れ替え（`DataGridBehavior`にてMoveとReplaceの動作が未確認の為）
* 列一覧または行一覧の幅を極端に狭くすると表示領域からはみ出てしまいデザイン的によろしくない（`ScrollViewer`をラップするのが良い）
* `RecordViewModel`の要素（プロパティー）が固定である為、あまり汎用性が効かない（`System.Data.DataTable`を利用するサンプルもあれば良い）  
  注記：`System.Data.DataTable`を直接`DataGrid#ItemsSource`にバインドすれば表示は行えるがバインド後に列の削除を行っても画面に反映されなかった記憶がある（記憶であり現時点での確認は行っていない）
* ヘルプメニューのバージョン情報は実装しない（デザイン上の体裁の為、定義しているのみ）

### 備忘録
* ウィンドウを閉じるタイミングにて以下の例外が不定期に発生している。[^close_error]
```{#lst:id console caption="エラー内容" number="no"}
Fatal error. System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
Repeat 2 times:
--------------------------------
   at MS.Win32.UnsafeNativeMethods+ITfThreadMgr.Deactivate()
--------------------------------
   at System.Windows.Documents.TextServicesHost.DeactivateThreadManager()
   at System.Windows.Documents.TextServicesHost.OnUnregisterTextStore(System.Object)
   at System.Windows.Documents.TextServicesHost.UnregisterTextStore(System.Windows.Documents.TextStore, Boolean)
   at System.Windows.Documents.TextStore.OnDetach(Boolean)
   at System.Windows.Documents.TextEditor.DetachTextStore(Boolean)
   at System.Windows.Documents.TextEditor+TextEditorShutDownListener.OnShutDown(System.Object, System.Object, System.EventArgs)
   at MS.Internal.ShutDownListener.HandleShutDown(System.Object, System.EventArgs)
   at System.Windows.Threading.Dispatcher.ShutdownImplInSecurityContext(System.Object)
   at MS.Internal.CulturePreservingExecutionContext.CallbackWrapper(System.Object)
   at System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object)
   at MS.Internal.CulturePreservingExecutionContext.Run(MS.Internal.CulturePreservingExecutionContext, System.Threading.ContextCallback, System.Object)
   at System.Windows.Threading.Dispatcher.ShutdownImpl()
   at System.Windows.Threading.Dispatcher.PushFrameImpl(System.Windows.Threading.DispatcherFrame)
   at System.Windows.Threading.Dispatcher.PushFrame(System.Windows.Threading.DispatcherFrame)
   at System.Windows.Threading.Dispatcher.Run()
   at System.Windows.Application.RunDispatcher(System.Object)
   at System.Windows.Application.RunInternal(System.Windows.Window)
   at System.Windows.Application.Run()
   at Otchitta.Example.Program.Main()
```

### その他
* 当該ソースコードの変数命名規約はかなり特殊になっているのはご了承ください（代入記号（=）を縦並びに一致させたい為で深い意味はない：矩形選択等の操作をやりやすくしている）
* サンプルプログラムでは発生しないが行一覧の全削除を行うとヘッダーの高さが狭くなりデザイン上の見栄えが良くなかった為、「MainWindow.xaml」にて`ColumnHeaderHeight`を設定している  
  （現行のサンプルプログラムでは「削除」列が存在する為、列が空となることはない）
* 上記未実装項目より優先してExample003を作成する（データベース処理が優先）

---
[^xaml_column]: xamlで定義した列も初期表示にて削除されると思われたが、実際には定義された列も表示されたので実行順序が添付ビヘイビアの方が優先されるのかもしれない（xamlの定義が残る動作の方が望ましいので調査は行わない）
[^close_error]: エラーの内容自体はメモリ書き込みに失敗しているとの事であるが`DataGridBehavior`の可能性が高いと思われる  
  今までは`MainWindow.xaml.cs`にて`DispatcherUnhandledException`をハンドリングしていなかったので気づかなかっただけの可能性もある  
  発生タイミングは上記の記載どおりウィンドウを閉じるタイミングのみとなる
