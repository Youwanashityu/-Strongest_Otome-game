# タイトル画面UIセット — パーツ納品一式

元PSD `ui.psd`（3840×2160）のレイヤーを、1レイヤー=1ファイルのPNGに切り分けたものです。
ピクセルが完全一致する重複レイヤーは1ファイルに統合してあります（統合の内訳は下表と `layout.json` を参照）。

## まず見るもの
- `guide/` — デモ画像に番号を振った説明画像。**「画面のどの部分がどのファイルか」はここが正**です
- `guide/parts_contact_sheet.png` — 全パーツのサムネイル一覧
- `demos/` — 元のデモ画像5枚
- `layout.json` — 全パーツの配置座標（3840×2160基準の画面座標 `bbox_px` と、0〜1の `bbox_norm`）

## フォルダ構成
- `parts/` — 実装に使うパーツ 21点（P01〜P21）
- `reference/` — サンプル/ダミー/テキスト参考 6点（R01〜R06）。**本番素材への差し替え前提**で、パーツではありません
- `verification/` — パーツを layout 通りに再合成した画像。デモとほぼ一致することを確認済み（差は主にデモ側スクリーンショットのカラープロファイルによる彩度差）

## パーツ一覧（parts/）

| ID | ファイル | 元レイヤー名 | サイズ | 使う画面 | 説明 |
|---|---|---|---|---|---|
| P01 | P01_titleBG.png | titleBG | 3840×2160 | 01 タイトル | タイトル画面の背景 |
| P02 | P02_START.png | START | 2132×555 | 01 タイトル | STARTボタン（白パネル） |
| P03 | P03_load.png | road | 1033×271 | 01 タイトル | LOADボタン（グラデ帯、STARTに重なる） |
| P04 | P04_gallery_button.png | gallery | 1360×366 | 01 タイトル | GALLERYボタン（タイトル画面用） |
| P05 | P05_credit_button.png | credit | 1360×366 | 01 タイトル | CREDITボタン（タイトル画面用） |
| P06 | P06_BGM_SE_panel.png | BGM SE | 1702×378 | 01 タイトル | BGM/SE 音量パネル土台（白パネル＋BGM/SE文字） |
| P07 | P07_slider_base.png | BGMbase / SEbase | 1025×118 | 01 タイトル | 音量スライダー：枠（8マスの輪郭） |
| P08 | P08_slider_BG.png | BGMBG / SEBG | 1005×98 | 01 タイトル | 音量スライダー：下地（暗色8マス） |
| P09 | P09_slider_bar.png | BGNbar / SEbar | 1005×98 | 01 タイトル | 音量スライダー：バー（グラデ8マス） |
| P10 | P10_creditBG.png | creditBG | 3624×1979 | 02 CREDIT | CREDIT画面のウィンドウ枠 |
| P11 | P11_credittitle.png | credittitle | 1617×382 | 02 CREDIT | CREDIT見出し帯（グラデ） |
| P12 | P12_galleryBG.png | galleryBG | 3840×2127 | 03 GALLERY | GALLERY画面の枠＋上下の菱形模様帯 |
| P13 | P13_gallerytitle.png | gallerytitle | 1422×304 | 03 GALLERY | GALLERY見出し帯（グラデ） |
| P14 | P14_LOADtitle.png | LOADtitle | 1234×391 | ロード画面（デモなし） | LOAD見出し帯（グラデ、?なし） |
| P15 | P15_SAVEtitle.png | SAVEtitle | 1234×391 | セーブ画面（デモなし） | SAVE見出し帯（グラデ、?なし） |
| P16 | P16_pop.png | pop | 2279×1280 | 04 LOAD確認、05 SAVE確認 | 確認ダイアログの土台パネル |
| P17 | P17_Savecheck.png | Savecheck | 1617×382 | 05 SAVE確認 | SAVE?見出し帯（確認ダイアログ用） |
| P18 | P18_Loadcheck.png | Loadcheck | 1617×382 | 04 LOAD確認 | LOAD?見出し帯（確認ダイアログ用） |
| P19 | P19_YES.png | YES | 860×297 | 04 LOAD確認、05 SAVE確認 | はいボタン（ピンク） |
| P20 | P20_NO.png | NO | 860×297 | 04 LOAD確認、05 SAVE確認 | いいえボタン（青） |
| P21 | P21_close.png | close | 280×281 | 02 CREDIT、03 GALLERY、04 LOAD確認、05 SAVE確認 | 閉じるボタン（×） |

## 参考一覧（reference/、差し替え前提）

| ID | ファイル | 元レイヤー名 | サイズ | 使う画面 | 説明 |
|---|---|---|---|---|---|
| R01 | R01_rogosample.png | rogosample | 1503×1503 | 01 タイトル | ロゴのサンプル（ゲームタイトル サンプル） |
| R02 | R02_titlesample.png | titlesample | 982×148 | 01 タイトル | キャラクター別タイトル のサンプル文字 |
| R03 | R03_creditcharademo.png | creditcharademo | 3318×1465 | 02 CREDIT | CREDIT画面のキャラ紹介ラフ（手書き） |
| R04 | R04_gallerydemo.png | gallerydemo | 3131×1487 | 03 GALLERY | GALLERY画面の画像配置ダミー（黒/白の矩形） |
| R05 | R05_savedemo.png | savedemo | 904×126 | 05 SAVE確認 | セーブしますか？ の文字 |
| R06 | R06_loaddemo.png | loaddemo | 898×122 | 04 LOAD確認 | ロードしますか？ の文字 |

## 重ね順（下→上）
- タイトル: 背景(P01) → ロゴ参考(R01) → サンプル文字(R02) → START(P02) → 音量パネル(P06) → スライダー下地(P08) → バー(P09) → 枠(P07) → GALLERYボタン(P04) → CREDITボタン(P05) → LOADボタン(P03)
- CREDIT: ウィンドウ枠(P10) → キャラ資料(R03) → 見出し帯(P11) → ×(P21)
- GALLERY: 枠(P12) → ダミー配置(R04) → 見出し帯(P13) → ×(P21)（背景にはタイトル画面が見える）
- LOAD確認: （タイトル画面の上）土台(P16) → LOAD?帯(P18) → テキスト(R06) → はい(P19) → いいえ(P20) → ×(P21)
- SAVE確認: （タイトル画面の上）土台(P16) → SAVE?帯(P17) → テキスト(R05) → はい(P19) → いいえ(P20) → ×(P21)
- ロード画面 / セーブ画面: 見出し帯として P14 / P15 を使用（この2画面の他の要素とデモ画像はなし）

## 実装メモ
- **座標**: 各パーツ画像はレイヤーの外接矩形で切り出してあります。配置は `layout.json` の `bbox_px` の左上座標にそのまま置けば再現できます。1920×1080基準なら座標・サイズとも0.5倍です
- **音量スライダー**: P08（下地・暗色）→ P09（バー・グラデ）→ P07（枠）の順に重ねます。P09は音量に応じて横幅をマスク/クリップして使う想定です。BGM用とSE用は同一画像のため、各1点しかありません（配置は2か所）
- **×ボタン P21**: CREDIT / GALLERY / 確認ダイアログで同一画像を共用します。PSD内に3レイヤーありましたが1点に統合しました（配置は計4か所）
- **右端の見切れ**: START(P02)、GALLERYボタン(P04)、CREDITボタン(P05)、音量パネル(P06) はキャンバス右端（x=3840）で切れるのがデザインです
- **LOADボタン P03**: PSD上の表記は `ROAD` でしたが LOAD の誤字でした（制作者確認済み）。このパッケージの `P03_load.png` は画像加工で L に修正した差し替え版です。デモ画像は修正前（ROAD表記）のままなので、このボタンの文字だけデモと異なります。PSD側の原画も直してください
- **P14 LOADtitle / P15 SAVEtitle**: それぞれロード画面／セーブ画面の見出し帯（?なし）として使います。確認ダイアログ用の Loadcheck / Savecheck（?付き）とは別物です。この2画面のデモ画像はありません（PSD内ではGALLERYフォルダーに同梱されていました）
- **BGNbar**: P09 の元レイヤー名は PSD 上 `BGNbar`（BGM側）でした。`BGMbar` の表記ゆれと思われます

元レイヤー総数: 32（重複統合後: パーツ21点 + 参考6点 = 27ファイル）
