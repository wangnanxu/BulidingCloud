--�������Լ��
exec sp_msforeachtable 'alter table ? nocheck constraint all'
--�������
truncate table dbo.Departments
truncate table  dbo.EnterpriseProfessions
truncate table  dbo.EnterprisePropertys 
truncate table   dbo.Enterprises
truncate table   dbo.FrontUsers
truncate table    dbo.MaterialType
truncate table  dbo.Projects
truncate table   dbo.RFAAuthorizations
truncate table    dbo.RFAFunctions
truncate table   dbo.RFARoles
truncate table   dbo.Scenes
truncate table   dbo.SceneTypes
truncate table   dbo.SyncState
truncate table   dbo.UserLoginState
truncate table   dbo.UserRoles
--�������Լ��
exec sp_msforeachtable 'alter table ? check constraint all'      
