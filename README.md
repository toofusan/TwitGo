# Prototype for Twitter Client for Oculus Go

※ 試したら全然動かなかったので調整します

Unityがあればこういうのができます。自分用に作っていたので他の人からすると変な可能性大ですごめんなさい。
https://twitter.com/toofu__/status/1028836485196214273

確認環境は`Unity2017.4.2f`です

## 使い方
1. このリポジトリをいい具合にcloneにしてUnityプロジェクトとして扱う
2. Oculus IntegrationをAsset Storeからインポートする
3. [Twity](https://github.com/toofusan/Twity)をUnityプロジェクトにインポートする
4. プロジェクト内に`UserConfig.cs`を作成
```C#
public class UserConfig
{
  public static string twitterConsumerKey = "XXXXXXXXXXXXXXXXXXXXXXXXX";
  public static string twitterConsumerSecret = "XXXXXXXXXXXXXXXXXXXXXXXXX";
  public static string twitterAccessToken = "XXXXXXXXXXXXXXXXXXXXXXXXX";
  public static string twitterAccessTokenSecret = "XXXXXXXXXXXXXXXXXXXXXXXXX";
  public static string twitterScreenName = "ツイッターID";
}
```
5. Oculus Goにビルドする（[参考](https://framesynthesis.jp/tech/unity/oculusgo/)）

## 操作方法
- いわゆる人差し指のPrimaryIndexTriggerを押して操作します
  - 眼の前の緑色のパネルに照準を合わせてTriggerを引くとTLの最新数十件が表示されます
  - ボタンっぽい見た目のものや画像は照準合わせてTriggerを引くと何かが起きます
  - 各パネルのボタンっぽいめためや画像意外の場所に照準を合わせてTriggerを押すと、ツイートを動かすことができます
- 親指のPrimaryTouchpadの真ん中付近を押すとメニューパネルが開きます
  - 各メニューに標準を合わせてTriggerを引くと何かが起きます

## 注意事項
- Oculus Riftでも一応動きます。顔面からレーザーが出たりメニューが出せない以外は。どちらでも使えるように直すかもしれないですが。
- アップデートは今後もするつもりですが完全に不完全なので、いろいろおかしいところや動かないボタンがあったりします。ごめんなさい。
- ツイートについている「Open Web」ボタンを押すとWebviewが小さく開きますが、これをすると処理が異常に重くなります。そうなったらアプリを終了してください。
- 使用は各自の責任でお願いします。
