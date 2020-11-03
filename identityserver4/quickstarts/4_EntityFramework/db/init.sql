CREATE LOGIN idpadmin
WITH
  PASSWORD = 'idpadmin0!',
  DEFAULT_DATABASE = master,
  CHECK_EXPIRATION = OFF, -- 有効期限チェックしない
  CHECK_POLICY = OFF -- パスワードの複雑性要件をチェックしない
GO

CREATE SCHEMA idpadmin
GO

CREATE USER idpadmin FOR LOGIN idpadmin WITH DEFAULT_SCHEMA=idpadmin
GO

EXEC sp_addrolemember 'db_owner', 'idpadmin'
GO