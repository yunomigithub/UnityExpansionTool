# UnityExpansionTool
Unityをさらに便利にするための拡張ツールです。<br>
勉強の一環として作成したものであるため処理が下手な部分等あるかもしれませんが、機能は一通りできておりますので開発のお供として多くの人に使ってもらえると幸いです。随時更新していく予定です。

## 各機能の説明
### アタッチしたオブジェクトを起点として処理を行う
* 起点オブジェクトからみて、第一階層目のオブジェクトにアンカーが付いているかを確認する
* 子要素のオブジェクト名に付く(1)などの不要文字を一括で削除する
* 子要素のImage/TextコンポーネントのRaycastTargetを一括でOFFにする
* オブジェクト名リストを作成する
* オブジェクトのアタッチリストを作成する
* オブジェクトのリセットリストを作成する

### 対象となるオブジェクトを探し出し、リスト表示する
* スクリプトのアタッチがMissingとなっているものを探しだす

### シーンに対して一括で何らかの処理を行う
* 特定のシーン内にあるすべてのオブジェクトから(1)などの不要文字を一括で削除する<br>
  ※対象シーンはAssets/Scenes以下のみです。

### UI配置の下敷きを簡単に作成する
* 750*1334のUI下敷きのベースを作成する

### シーンビューに対しての拡張
* QuarterViewボタン<br>
  押すとシーンビューの画面が斜め見下ろしに変わります。
* CameraSyncボタン<br>
  押すと「MainCamera」タグの付いたカメラのPositionとRotationがシーンと同期します。<br>
  ※適用後はUndoができないので、事前にシーン保存やGit保存をオススメします。
* SceneMoveボタン<br>
  テキスト欄に入力したシーンへ移動することができます。<br>
  ※対象シーンはAssets/Scenes以下のみです。

### オブジェクト作成のショートカットを追加
* Toggle<br>
  画像のON/OFFを行うトグルを一発で作成することができます。 
* Scroll<br>
  縦スクロールを一発で作成することができます。
* Image<br>
  RaycastTargetがOFFになっているImageオブジェクトを一発で作成することができます。

## 推奨動作環境
Unity5.5.1以降のバージョン
