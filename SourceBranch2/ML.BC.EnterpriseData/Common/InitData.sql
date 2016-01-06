USE [BCEnterpriseDB]
GO
--禁用外键约束
exec sp_msforeachtable 'alter table ? nocheck constraint all'
--清空数据
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
truncate table   dbo.UserLoginLog
truncate table   dbo.UserLoginState
truncate table   dbo.UserRoles
--启用外键约束
exec sp_msforeachtable 'alter table ? check constraint all'

/*初始化权限项*/
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root', N'Root', N'', N'', N'这是功能表的根，不能编辑', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission', N'APP权限', N'AppPermission', N'Root', N'App中的权限', 1, CAST(0x0000A4C2009F71B7 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneData', N'现场数据', N'SceneData', N'Root.AppPermission', N'现场数据的相关权限', 1, CAST(0x0000A4C800C8AD6E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneData.ArchiveSceneData', N'归档现场数据', N'ArchiveSceneData', N'Root.AppPermission.SceneData', N'归档现场数据', 1, CAST(0x0000A4C800C8D85B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneData.VerifySceneData', N'审核现场数据', N'VerifySceneData', N'Root.AppPermission.SceneData', N'审核现场数据', 1, CAST(0x0000A4C800C8C29E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage', N'现场管理', N'SceneManage', N'Root.AppPermission', N'现场管理', 1, CAST(0x0000A4C800C6D819 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.AchieveScene', N'完工现场', N'AchieveScene', N'Root.AppPermission.SceneManage', N'设置现场为完工', 1, CAST(0x0000A4C800C86B25 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.AddScene', N'新增现场', N'AddScene', N'Root.AppPermission.SceneManage', N'在App端新增现场的权限', 1, CAST(0x0000A4C800C75959 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.EditScene', N'编辑现场', N'EditScene', N'Root.AppPermission.SceneManage', N'在App中编辑现场的权限', 1, CAST(0x0000A4C800C74D9F AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.InspectScene', N'临检现场', N'InspectScene', N'Root.AppPermission.SceneManage', N'临检现场，审核现场数据的权限', 1, CAST(0x0000A4C800C83DA1 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.SceneBuilding', N'现场施工', N'SceneBuilding', N'Root.AppPermission.SceneManage', N'现场施工，发布现场施工数据', 1, CAST(0x0000A4C800C7AA33 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.AppPermission.SceneManage.ViewAchievedScene', N'查看已完工现场', N'ViewAchievedScene', N'Root.AppPermission.SceneManage', N'查看已完工现场', 1, CAST(0x0000A4C800D396C3 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement', N'知识管理', N'DataManagement', N'Root', N'知识管理', 1, CAST(0x0000A4BB011D7355 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement', N'知识清单', N'DataListManagement', N'Root.DataManagement', N'知识清单', 1, CAST(0x0000A4BB011C3BA4 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.Add', N'新增', N'Add', N'Root.DataManagement.DataListManagement', N'新增知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.Delete', N'删除', N'Delete', N'Root.DataManagement.DataListManagement', N'删除知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.Edit', N'编辑', N'Edit', N'Root.DataManagement.DataListManagement', N'编辑知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.Info', N'详情', N'Info', N'Root.DataManagement.DataListManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.List', N'知识列表', N'List', N'Root.DataManagement.DataListManagement', N'知识列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataListManagement.Download', N'下载文件', N'Download', N'Root.DataManagement.DataListManagement', N'下载文件', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement', N'知识类型', N'DataTypeManagement', N'Root.DataManagement', N'知识类型', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement.Add', N'新增', N'Add', N'Root.DataManagement.DataTypeManagement', N'新增知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement.Delete', N'删除', N'Delete', N'Root.DataManagement.DataTypeManagement', N'删除知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement.Edit', N'编辑', N'Edit', N'Root.DataManagement.DataTypeManagement', N'编辑知识', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement.Info', N'详情', N'Info', N'Root.DataManagement.DataTypeManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.DataManagement.DataTypeManagement.List', N'知识类型列表', N'List', N'Root.DataManagement.DataTypeManagement', N'知识类型列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement', N'项目管理', N'ProjectManagement', N'Root', N'项目管理', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.GISMapManagement', N'GIS地图', N'GISMapManagement', N'Root.ProjectManagement', N'GIS地图', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.GISMapManagement.SeeMap', N'现场地图', N'Info', N'Root.ProjectManagement.GISMapManagement', N'查看现场分布地图', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement', N'项目清单', N'ProjectListManagement', N'Root.ProjectManagement', N'项目清单', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.Add', N'新增', N'Add', N'Root.ProjectManagement.ProjectListManagement', N'新增项目', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.Delete', N'删除', N'Delete', N'Root.ProjectManagement.ProjectListManagement', N'删除项目', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.Edit', N'编辑', N'Edit', N'Root.ProjectManagement.ProjectListManagement', N'编辑项目', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.Info', N'详情', N'Info', N'Root.ProjectManagement.ProjectListManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.List', N'项目清单', N'List', N'Root.ProjectManagement.ProjectListManagement', N'项目清单', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.ProjectListManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.ProjectManagement.ProjectListManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement', N'现场清单', N'SceneListManagement', N'Root.ProjectManagement', N'现场清单', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.Add', N'新增', N'Add', N'Root.ProjectManagement.SceneListManagement', N'新增现场', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.AddSceneData', N'新增现场数据', N'AddSceneData', N'Root.ProjectManagement.SceneListManagement', N'新增现场数据', 1, CAST(0x0000A4C200A22DF9 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.ArchiveSceneData', N'归档现场数据', N'ArchiveSceneData', N'Root.ProjectManagement.SceneListManagement', N'归档现场数据', 1, CAST(0x0000A4C600BA46CC AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.CommentSceneData', N'评论现场数据', N'CommentSceneData', N'Root.ProjectManagement.SceneListManagement', N'评论现场数据', 1, CAST(0x0000A4C600BAA770 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.Delete', N'删除', N'Delete', N'Root.ProjectManagement.SceneListManagement', N'删除现场', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.DeleteSceneData', N'删除现场数据', N'DeleteSceneData', N'Root.ProjectManagement.SceneListManagement', N'删除现场数据', 1, CAST(0x0000A4C600B79E60 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.Edit', N'编辑', N'Edit', N'Root.ProjectManagement.SceneListManagement', N'编辑现场', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.Info', N'详情', N'Info', N'Root.ProjectManagement.SceneListManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.List', N'现场清单', N'List', N'Root.ProjectManagement.SceneListManagement', N'现场清单', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.ProjectManagement.SceneListManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.VerifySceneData', N'审核现场数据', N'VerifySceneData', N'Root.ProjectManagement.SceneListManagement', N'审核现场数据', 1, CAST(0x0000A4C600B75656 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.PicturePack', N'打包下载', N'PicturePack', N'Root.ProjectManagement.SceneListManagement', N'打包下载', 1, CAST(0x0000A4C600B75656 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ProjectManagement.SceneListManagement.ViewSceneData', N'查看现场数据', N'ViewSceneData', N'Root.ProjectManagement.SceneListManagement', N'查看现场数据', 1, CAST(0x0000A4C200A1EB37 AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement', N'报表管理', N'ReportManagement', N'Root', N'报表管理', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.DepartmentDataReport', N'部门数据报表', N'DepartmentDataReport', N'Root.ReportManagement', N'部门数据报表', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.DepartmentDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.DepartmentDataReport', N'查看所有部门数据的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.PersonStatisticsDataReport', N'工作人员统计', N'PersonStatisticsDataReport', N'Root.ReportManagement', N'工作人员统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.PersonStatisticsDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.PersonStatisticsDataReport', N'查看所有工作人员统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectCompleteCountDataReport', N'项目完工统计', N'ProjectCompleteCountDataReport', N'Root.ReportManagement', N'项目完工统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectCompleteCountDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.ProjectCompleteCountDataReport', N'查看所有项目完工统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectSceneExamineDataReport', N'项目现场审核统计', N'ProjectSceneExamineDataReport', N'Root.ReportManagement', N'项目现场审核统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectSceneExamineDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.ProjectSceneExamineDataReport', N'查看所有项目现场审核统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectSceneRectDataReport', N'项目现场整改统计', N'ProjectSceneRectCountDataReport', N'Root.ReportManagement', N'项目现场整改统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectSceneRectDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.ProjectSceneRectDataReport', N'查看所有项目现场整改统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectStatisticsDataReport', N'项目统计', N'ProjectStatisticsDataReport', N'Root.ReportManagement', N'项目统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ProjectStatisticsDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.ProjectStatisticsDataReport', N'查看所有项目统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ScanCountDataReport', N'项目现场浏览统计', N'ScanCountDataReport', N'Root.ReportManagement', N'项目现场浏览统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.ScanCountDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.ScanCountDataReport', N'查看所有项目现场浏览统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.AllCountDataReport', N'运行统计', N'AllCountDataReport', N'Root.ReportManagement', N'运行统计', 1, CAST(0x0000A4C90125099B AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.ReportManagement.AllCountDataReport.ShowAll', N'查看全部', N'ShowAll', N'Root.ReportManagement.AllCountDataReport', N'查看所有运行统计的权限', 1, CAST(0x0000A4C90127324E AS DateTime))
GO

INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting', N'系统设置', N'SystemSetting', N'Root', N'系统设置', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OperationLogManagement', N'操作日志', N'OperationLogManagement', N'Root.SystemSetting', N'操作日志', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OperationLogManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.OperationLogManagement', N'删除日志', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OperationLogManagement.Info', N'详情', N'Info', N'Root.SystemSetting.OperationLogManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OperationLogManagement.List', N'日志列表', N'List', N'Root.SystemSetting.OperationLogManagement', N'日志列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OperationLogManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.SystemSetting.OperationLogManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement', N'组织结构', N'OrganizationManagement', N'Root.SystemSetting', N'组织结构', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.Add', N'新增', N'Add', N'Root.SystemSetting.OrganizationManagement', N'新增', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.OrganizationManagement', N'删除', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.Edit', N'编辑', N'Edit', N'Root.SystemSetting.OrganizationManagement', N'编辑', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.Info', N'详情', N'Info', N'Root.SystemSetting.OrganizationManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.List', N'角色列表', N'List', N'Root.SystemSetting.OrganizationManagement', N'角色列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.OrganizationManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.SystemSetting.OrganizationManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement', N'角色管理', N'RoleManagement', N'Root.SystemSetting', N'用角色管理', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.Add', N'新增', N'Add', N'Root.SystemSetting.RoleManagement', N'新增角色', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.RoleManagement', N'删除角色', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.Edit', N'编辑', N'Edit', N'Root.SystemSetting.RoleManagement', N'编辑角色', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.Info', N'详情', N'Info', N'Root.SystemSetting.RoleManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.List', N'角色列表', N'List', N'Root.SystemSetting.RoleManagement', N'角色列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.RoleManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.SystemSetting.RoleManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement', N'现场类型', N'ScenetypeManagement', N'Root.SystemSetting', N'现场类型', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement.Add', N'新增', N'Add', N'Root.SystemSetting.ScenetypeManagement', N'新增现场类型', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.ScenetypeManagement', N'删除现场类型', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement.Edit', N'编辑', N'Edit', N'Root.SystemSetting.ScenetypeManagement', N'编辑现场类型', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement.Info', N'详情', N'Info', N'Root.SystemSetting.ScenetypeManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.ScenetypeManagement.List', N'现场类型列表', N'List', N'Root.SystemSetting.ScenetypeManagement', N'现场类型列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserLoginStateManagement', N'用户状态', N'UserLoginStateManagement', N'Root.SystemSetting', N'用户状态', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserLoginStateManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.UserLoginStateManagement', N'删除用户状态', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserLoginStateManagement.Info', N'详情', N'Info', N'Root.SystemSetting.UserLoginStateManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserLoginStateManagement.List', N'用户状态列表', N'List', N'Root.SystemSetting.UserLoginStateManagement', N'用户状态列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserLoginStateManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.SystemSetting.UserLoginStateManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement', N'用户管理', N'UserManagement', N'Root.SystemSetting', N'用户管理', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.Add', N'新增', N'Add', N'Root.SystemSetting.UserManagement', N'新增用户', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.Delete', N'删除', N'Delete', N'Root.SystemSetting.UserManagement', N'删除用户', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.Edit', N'编辑', N'Edit', N'Root.SystemSetting.UserManagement', N'编辑用户', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.Info', N'详情', N'Info', N'Root.SystemSetting.UserManagement', N'查看详情', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.List', N'用户列表', N'List', N'Root.SystemSetting.UserManagement', N'用户列表', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO
INSERT [dbo].[RFAFunctions] ([FunctionID], [Name], [MyID], [ParentID], [Desription], [Available], [UpdateTime]) VALUES (N'Root.SystemSetting.UserManagement.ShowAll', N'查看全部', N'ShowAll', N'Root.SystemSetting.UserManagement', N'查看全部', 1, CAST(0x0000A4B900AE557A AS DateTime))
GO



/*初始化通用角色*/
SET IDENTITY_INSERT [dbo].[RFARoles] ON 
GO
INSERT [dbo].[RFARoles] ([RoleID], [Name], [OwnerID], [Description], [Available], [UpdateTime]) VALUES (1, N'企业超级管理员', null, N'拥有企业的所有权限', 1, CAST(0x0000A4B900AE7BEE AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[RFARoles] OFF
GO

/*初始化行业类型与企业性质*/
