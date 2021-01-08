const poolData = {
  UserPoolId: 'ap-northeast-1_3qtuiiygd',
  ClientId: '32lfvqq4mi5gsbsqf7reij3tp3'
};

const userPool = new AmazonCognitoIdentity.CognitoUserPool(poolData);

// Amazon Cognito 認証情報プロバイダーを初期化します
AWS.config.region = 'ap-northeast-1'; // リージョン
AWS.config.credentials = new AWS.CognitoIdentityCredentials({
  IdentityPoolId: 'ap-northeast-1:fb137cef-8331-4ea1-bb3a-7fc30273afa6',
});

