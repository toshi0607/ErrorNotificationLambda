# ErrorNotificationLambda

![通知奴](https://user-images.githubusercontent.com/7035446/34076287-38bc8e72-e324-11e7-9811-ce00cdc2cc03.PNG "通知奴")

## 概要
CloudWatch Logsのイベントを拾ってログの内容をSlackで通知するためのLambdaです。

DLQで飛ばせるのはイベントの内容そのもので、その情報（request idなど）でCloudWatch Logsを検索し、
エラー情報を探すのは少し面倒です。

そのため、特定の文字列（Error!など）でCloudWatch Logsでメトリックフィルターを作成し、
Lambdaにストリーミングすることにしました。

特定の文字列を含むログにスタックトレースを含めていればSlackに直接スタックトレースが飛びます。

## セットアップ
### 本番環境
1. このLambdaを通知対象のCloudWatch Logsと同一リージョンにデプロイします。

次の3つの環境変数をセットしてください。

* SLACK_WEBHOOK_URL
* CLOUDWATCH_LOG_GROUP_URL
* CLOUDWATCH_METRICS_URL

Slackに通知を行うときに使用する[Incoming Webhooks](https://api.slack.com/incoming-webhooks)と内容の一部として使用するリンク用です。

2. CloudWatch Logsから監視したいロググループを選択し、アクション -> Lambdaサービスへのストリーミングの開始を選択してください。

手順に従ってフィルターをセットし、１でデプロイしたLambdaを接続先に指定します。

### ローカル開発環境
ｘUnitのテストをドライバとして実行します。

**FunctionTest.csの**Propertiesフォルダにある`launciSettings.json`の`[YOUR URL]`部分を**本番環境**でセットしたものに書き換えてください。

fixtureでローカル実行時に環境変数をセットするのに使用しています。

## テストデータ
CloudWatch Logsからは次のようなイベントがjsonで送られてきます。

```json
{
  "awslogs": {
    "data": "H4sIAAAAAAAAAHWPwQqCQBCGX0Xm7EFtK+smZBEUgXoLCdMhFtKV3akI8d0bLYmibvPPN3wz00CJxmQnTO41whwWQRIctmEcB6sQbFC3CjW3XW8kxpOpP+OC22d1Wml1qZkQGtoMsScxaczKN3plG8zlaHIta5KqWsozoTYw3/djzwhpLwivWFGHGpAFe7DL68JlBUk+l7KSN7tCOEJ4M3/qOI49vMHj+zCKdlFqLaU2ZHV2a4Ct/an0/ivdX8oYc1UVX860fQDQiMdxRQEAAA=="
   }
 }
```

これは元のログがgzipされ、Base64エンコードされたもので、元のログ（`data`内部）は次のような構造になっています。

```json
{
  "messageType":"DATA_MESSAGE",
  "owner":"123456789123",
  "logGroup":"testLogGroup",
  "logStream":"testLogStream",
  "subscriptionFilters":["testFilter"],
  "logEvents":
    [
      {
        "id":"eventId1",
        "timestamp":1440442987000,
        "message":"[ERROR] First test message"
      },
      {
        "id":"eventId2",
        "timestamp":1440442987001,
        "message":"[ERROR] Second test message"
      }
    ]
}
```

