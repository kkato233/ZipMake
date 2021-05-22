## パスワード付き ZIP ファイルを簡単に作成する。

### 暗号化ZIP を 簡単に作成する。

![フォーム](./img/Form1.png)

パスワードを入力して

ファイルを ファイル一覧 ドラッグ＆ドロップして 「ZIPファイル作成」
ボタンをクリックすると

入力ファイルと同じ場所に ZIP ファイルが作成できます。

パスワードは そのパソコン上に 暗号化された状態で保存されます。

次回アプリを起動したときには 前回入力したパスワードが自動的に設定されます。

パスワードを入力しなければ パスワード無しの ZIP ファイルが作成されます。

### パスワードの暗号化の仕組み
 Windows に標準で入っている `Crypt32.dll` の 

[Windows Data Protection](http://msdn2.microsoft.com/en-us/library/ms995355.aspx)

の 機能を使って そのパソコンでのみ解読可能な形で暗号化して格納しています。

暗号化されたパスワードは 他のパソコンでは 暗号化を復元する事はできません。

利用している Win32 API 
[CryptProtectData](https://docs.microsoft.com/en-us/windows/win32/api/dpapi/nf-dpapi-cryptprotectdata)





