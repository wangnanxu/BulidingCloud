--�������Լ��
exec sp_msforeachtable 'alter table ? nocheck constraint all'
--�������
truncate table dbo.BackUsers
truncate table  dbo.RFAAuthorizations
truncate table  dbo.RFAFunctions
truncate table  dbo.RFARoles 
truncate table   dbo.UserRoles
--�������Լ��
exec sp_msforeachtable 'alter table ? check constraint all'
