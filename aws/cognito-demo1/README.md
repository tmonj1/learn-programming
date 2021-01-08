## AWS Cognito を使ったWebアプリケーションのサインアップとサインインのデモ

URL: http://tmj-cognito-login-demo1.s3-website-ap-northeast-1.amazonaws.com/

出典： [AWS Cognito使ってみた](https://hacknote.jp/archives/57871/)

単に Cognito を使ったサインアップとサインインをするだけで、他のAWSサービスを呼び出したりはしない (なので Identity Pool は構成しているものの実質的に使ってないはず)。

### 構成

|構成要素|名称|説明|
|:--|:--|:--|
|Cognito User Pool|testpool1|認証 (サインアップとサインイン)|
|Cognito Identity Pool|testidpool1|用途不明 (そもそも使ってる?)|
|S3|s3://tmj-cognito-login-demo1|Static Website Hosting でHTM/JSファイルを格納|
|UI (HTML/JS)|'-|サインアップとサインインのためのHTMLページ|
