--禁用外键约束
exec sp_msforeachtable 'alter table ? nocheck constraint all'
--清空数据
truncate table dbo.BackUsers
truncate table  dbo.RFAAuthorizations
truncate table  dbo.RFAFunctions
truncate table  dbo.RFARoles 
truncate table   dbo.UserRoles
--启用外键约束
exec sp_msforeachtable 'alter table ? check constraint all'
