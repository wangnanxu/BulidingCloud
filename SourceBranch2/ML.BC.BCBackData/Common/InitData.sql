USE [BCBackDB]
GO
/*清除所有数据*/
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

/*生成Functions*/
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root', N'Root', N'', N'', N'这是功能表的根，不能编辑', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPBillManagement', N'计费管理', N'EPBill', N'Root', N'计费管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement', N'企业管理', N'EPManagement', N'Root', N'运营平台企业管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPList', N'企业列表', N'EPList', N'Root.EPManagement', N'企业列表管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPList.Add', N'新增', N'Add', N'Root.EPManagement.EPList', N'新增企业', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPList.Delete', N'删除', N'Delete', N'Root.EPManagement.EPList', N'删除企业', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPList.Edit', N'编辑', N'Edit', N'Root.EPManagement.EPList', N'编辑企业', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPLog', N'企业日志', N'EPLog', N'Root.EPManagement', N'企业日志管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPLog.ClearLogs', N'清空日志', N'ClearLogs', N'Root.EPManagement.EPLog', N'清空日志', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPLog.ViewDetail', N'查看详细', N'ViewDetail', N'Root.EPManagement.EPLog', N'查看详细', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProfession', N'行业类型', N'EPProfession', N'Root.EPManagement', N'行业类型管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProfession.Add', N'新增', N'Add', N'Root.EPManagement.EPProfession', N'新增行业类型', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProfession.Delete', N'删除', N'Delete', N'Root.EPManagement.EPProfession', N'删除行业类型', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProfession.Edit', N'编辑', N'Edit', N'Root.EPManagement.EPProfession', N'编辑行业类型', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProperty', N'企业性质', N'EPProperty', N'Root.EPManagement', N'企业性质管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProperty.Add', N'新增', N'Add', N'Root.EPManagement.EPProperty', N'新增企业性质', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProperty.Delete', N'删除', N'Delete', N'Root.EPManagement.EPProperty', N'删除企业性质', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPProperty.Edit', N'编辑', N'Edit', N'Root.EPManagement.EPProperty', N'编辑企业性质', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPRole', N'企业角色', N'EPRole', N'Root.EPManagement', N'企业角色管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPRole.Add', N'新增', N'Add', N'Root.EPManagement.EPRole', N'新增企业角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPRole.Delete', N'删除', N'Delete', N'Root.EPManagement.EPRole', N'删除企业角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPRole.Edit', N'编辑', N'Edit', N'Root.EPManagement.EPRole', N'编辑企业角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPUser', N'企业帐号', N'EPUser', N'Root.EPManagement', N'企业帐号管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPUser.Add', N'新增', N'Add', N'Root.EPManagement.EPUser', N'新增企业帐号', 0)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPUser.Delete', N'删除', N'Delete', N'Root.EPManagement.EPUser', N'删除企业帐号', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPUser.Edit', N'编辑', N'Edit', N'Root.EPManagement.EPUser', N'编辑企业帐号', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.EPUser.ViewDetail', N'企业账户详情', N'ViewDetail', N'Root.EPManagement.EPUser', N'查看企业账户详情', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.EPManagement.LoginLog', N'登录日志', N'LoginLog', N'Root.EPManagement', N'登录日志', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement', N'系统平台管理', N'SysManagement', N'Root', N'系统平台管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.EPFunMangement', N'企业功能项管理', N'EPFunMangement', N'Root.SysManagement', N'企业功能项管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.EPFunMangement.Add', N'新增', N'Add', N'Root.SysManagement.EPFunMangement', N'新增企业功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.EPFunMangement.Delete', N'删除', N'Delete', N'Root.SysManagement.EPFunMangement', N'删除功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.EPFunMangement.Edit', N'编辑', N'Edit', N'Root.SysManagement.EPFunMangement', N'编辑功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.OnlineUser', N'在线人员', N'OnlineUser', N'Root.SysManagement', N'在线人员查询', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.OnlineUser.ForceUserLogout', N'强制用户登出', N'ForceUserLogout', N'Root.SysManagement.OnlineUser', N'强制用户登出的权限', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysFunManagement', N'系统功能项管理', N'SysFunManagement', N'Root.SysManagement', N'系统功能项管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysFunManagement.AddFun', N'新增', N'AddFun', N'Root.SysManagement.SysFunManagement', N'新增功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysFunManagement.DeleteFun', N'删除', N'DeleteFun', N'Root.SysManagement.SysFunManagement', N'删除功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysFunManagement.EditFun', N'编辑', N'EditFun', N'Root.SysManagement.SysFunManagement', N'编辑功能项', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysLogManagement', N'系统日志管理', N'SysLogManagement', N'Root.SysManagement', N'系统日志管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysLogManagement.Delete', N'删除', N'Delete', N'Root.SysManagement.SysLogManagement', N'删除系统日志', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysLogManagement.Down', N'下载', N'Down', N'Root.SysManagement.SysLogManagement', N'下载日志', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysLogManagement.ViewDetail', N'查看详情', N'ViewDetail', N'Root.SysManagement.SysLogManagement', N'查看日志详情', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysRoleManagement', N'系统角色管理', N'SysRoleManagement', N'Root.SysManagement', N'系统角色管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysRoleManagement.Add', N'新增', N'Add', N'Root.SysManagement.SysRoleManagement', N'新增角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysRoleManagement.Delete', N'删除', N'Delete', N'Root.SysManagement.SysRoleManagement', N'删除角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysRoleManagement.Edit', N'编辑', N'Edit', N'Root.SysManagement.SysRoleManagement', N'编辑角色', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysUserManagement', N'系统用户管理', N'SysUserManagement', N'Root.SysManagement', N'系统用户管理', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysUserManagement.Add', N'新增', N'Add', N'Root.SysManagement.SysUserManagement', N'新增系统用户', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysUserManagement.Delete', N'删除', N'Delete', N'Root.SysManagement.SysUserManagement', N'删除系统用户', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysUserManagement.Edit', N'编辑', N'Edit', N'Root.SysManagement.SysUserManagement', N'编辑系统用户', 1)
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available]) VALUES (N'Root.SysManagement.SysUserManagement.ViewDetail', N'查看详细', N'ViewDetail', N'Root.SysManagement.SysUserManagement', N'查看系统用户详细信息', 1)
GO

/*生成默认用户,及其角色权限*/
INSERT [dbo].[BackUsers] ([UserID], [Name], [Password], [Mobile], [RegistDate], [LastDate], [LastIP], [Closed], [UpdateTime]) VALUES (N'A00001', N'admin', N'mEcSGCkPn+g=', N'18623412037', CAST(0x0000A4A400FD2F6E AS DateTime), CAST(0x0000A4CE00B153E9 AS DateTime), N'::1', 0, CAST(0x0000A4CE00B2C6DA AS DateTime))
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root', CAST(0x0000A4CE00B2B23F AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPBillManagement', CAST(0x0000A4CE00B2B250 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement', CAST(0x0000A4CE00B2B251 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPList', CAST(0x0000A4CE00B2B251 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPList.Add', CAST(0x0000A4CE00B2B252 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPList.Delete', CAST(0x0000A4CE00B2B254 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPList.Edit', CAST(0x0000A4CE00B2B255 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPLog', CAST(0x0000A4CE00B2B256 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPLog.ClearLogs', CAST(0x0000A4CE00B2B257 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPLog.ViewDetail', CAST(0x0000A4CE00B2B258 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProfession', CAST(0x0000A4CE00B2B25A AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProfession.Add', CAST(0x0000A4CE00B2B25B AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProfession.Delete', CAST(0x0000A4CE00B2B25C AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProfession.Edit', CAST(0x0000A4CE00B2B25D AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProperty', CAST(0x0000A4CE00B2B25E AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProperty.Add', CAST(0x0000A4CE00B2B25F AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProperty.Delete', CAST(0x0000A4CE00B2B260 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPProperty.Edit', CAST(0x0000A4CE00B2B261 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPRole', CAST(0x0000A4CE00B2B262 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPRole.Add', CAST(0x0000A4CE00B2B263 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPRole.Delete', CAST(0x0000A4CE00B2B263 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPRole.Edit', CAST(0x0000A4CE00B2B264 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPUser', CAST(0x0000A4CE00B2B265 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPUser.Add', CAST(0x0000A4CE00B2B266 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPUser.Delete', CAST(0x0000A4CE00B2B267 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPUser.Edit', CAST(0x0000A4CE00B2B269 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.EPUser.ViewDetail', CAST(0x0000A4CE00B2B26A AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.EPManagement.LoginLog', CAST(0x0000A4CE00B2B26B AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement', CAST(0x0000A4CE00B2B26C AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.EPFunMangement', CAST(0x0000A4CE00B2B26E AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.EPFunMangement.Add', CAST(0x0000A4CE00B2B26F AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.EPFunMangement.Delete', CAST(0x0000A4CE00B2B26F AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.EPFunMangement.Edit', CAST(0x0000A4CE00B2B270 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.OnlineUser', CAST(0x0000A4CE00B2B271 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.OnlineUser.ForceUserLogout', CAST(0x0000A4CE00B2B272 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysFunManagement', CAST(0x0000A4CE00B2B273 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysFunManagement.AddFun', CAST(0x0000A4CE00B2B274 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysFunManagement.DeleteFun', CAST(0x0000A4CE00B2B275 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysFunManagement.EditFun', CAST(0x0000A4CE00B2B276 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysLogManagement', CAST(0x0000A4CE00B2B277 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysLogManagement.Delete', CAST(0x0000A4CE00B2B278 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysLogManagement.Down', CAST(0x0000A4CE00B2B279 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysLogManagement.ViewDetail', CAST(0x0000A4CE00B2B27A AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysRoleManagement', CAST(0x0000A4CE00B2B27B AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysRoleManagement.Add', CAST(0x0000A4CE00B2B27C AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysRoleManagement.Delete', CAST(0x0000A4CE00B2B27D AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysRoleManagement.Edit', CAST(0x0000A4CE00B2B27E AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysUserManagement', CAST(0x0000A4CE00B2B27F AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysUserManagement.Add', CAST(0x0000A4CE00B2B280 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysUserManagement.Delete', CAST(0x0000A4CE00B2B281 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysUserManagement.Edit', CAST(0x0000A4CE00B2B282 AS DateTime), 0)
GO
INSERT [dbo].[RFAAuthorizations] ([RoleID], [FunctionID], [UpdateTime], [Deleted]) VALUES (1, N'Root.SysManagement.SysUserManagement.ViewDetail', CAST(0x0000A4CE00B2B283 AS DateTime), 0)
GO

SET IDENTITY_INSERT [dbo].[RFARoles] ON 
GO
INSERT [dbo].[RFARoles] ([RoleID], [Name], [OwnerID], [Description], [Available]) VALUES (1, N'系统超级管理员', N'', N'系统超级管理员，初始只权限项管理的权限', 1)
GO
SET IDENTITY_INSERT [dbo].[RFARoles] OFF
GO
INSERT [dbo].[UserRoles] ([UserID], [RoleID], [UpdateTime], [Deleted]) VALUES (N'A00001', 1, CAST(0x0000A4A400FD2F75 AS DateTime), 0)
GO
