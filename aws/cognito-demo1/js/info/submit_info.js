// 1Amazon Cognito 認証情報プロバイダーを初期化します
// Amazon Cognito 認証情報プロバイダーを初期化します
AWS.config.region = 'ap-northeast-1'; // リージョン
AWS.config.credentials = new AWS.CognitoIdentityCredentials({
  IdentityPoolId: 'ap-northeast-1:fb137cef-8331-4ea1-bb3a-7fc30273afa6',
});

// 2Amazon Cognito Userpoolの指定＋クライアントアプリの指定
let poolData = {
  UserPoolId: 'ap-northeast-1_3qtuiiygd', //ユーザープールのID
  ClientId: '32lfvqq4mi5gsbsqf7reij3tp3' //クライアントアプリの設定上のID

};
//ユーザープール＋クライアントアプリの情報を格納
let userPool = new AmazonCognitoIdentity.CognitoUserPool(poolData);
let attributeList = []; //本来であればattributelistに電話番号や住所など入れることも可能（今回はしない）
let cognitoUser;