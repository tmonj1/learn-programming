// 1Amazon Cognito 認証情報プロバイダーを初期化します
// Amazon Cognito 認証情報プロバイダーを初期化します
AWS.config.region = 'ap-northeast-1'; // リージョン
AWS.config.credentials = new AWS.CognitoIdentityCredentials({
  IdentityPoolId: 'ap-northeast-1:fb137cef-8331-4ea1-bb3a-7fc30273afa6',
});
//よくわからないけど初期化します
AWSCognito.config.region = 'ap-northeast-1'; // リージョン(デフォルトだとこのまま)
AWSCognito.config.credentials = new AWS.CognitoIdentityCredentials({
  IdentityPoolId: 'ap-northeast-1:fb137cef-8331-4ea1-bb3a-7fc30273afa6', // ID プールのID
});