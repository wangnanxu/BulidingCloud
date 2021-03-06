USE [BCEnterpriseDB]
GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'UserRoles', N'COLUMN',N'Deleted'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserRoles', @level2type=N'COLUMN',@level2name=N'Deleted'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Scenes', N'COLUMN',N'Woker'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Scenes', @level2type=N'COLUMN',@level2name=N'Woker'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFARoles', N'COLUMN',N'OwnerID'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFARoles', @level2type=N'COLUMN',@level2name=N'OwnerID'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAFunctions', N'COLUMN',N'FunctionID'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAFunctions', @level2type=N'COLUMN',@level2name=N'FunctionID'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAAuthorizations', N'COLUMN',N'Deleted'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAAuthorizations', @level2type=N'COLUMN',@level2name=N'Deleted'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Roles'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Roles'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Managers'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Managers'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Departments'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Departments'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Enterprises', N'COLUMN',N'EnterpriseID'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Enterprises', @level2type=N'COLUMN',@level2name=N'EnterpriseID'

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserRoles_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [DF_UserRoles_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserRoles_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [DF_UserRoles_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserLoginState_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserLoginState] DROP CONSTRAINT [DF_UserLoginState_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SceneTypes_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SceneTypes] DROP CONSTRAINT [DF_SceneTypes_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_HasData]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_HasData]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_UpdateTime_1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_UpdateTime_1]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_Status]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_RegistDate]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_ParentSceneID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] DROP CONSTRAINT [DF_Scenes_ParentSceneID]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] DROP CONSTRAINT [DF_RFARoles_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] DROP CONSTRAINT [DF_RFARoles_Available]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_OwnerID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] DROP CONSTRAINT [DF_RFARoles_OwnerID]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] DROP CONSTRAINT [DF_RFAFunctions_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] DROP CONSTRAINT [DF_RFAFunctions_Available]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_ParentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] DROP CONSTRAINT [DF_RFAFunctions_ParentID]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_MyID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] DROP CONSTRAINT [DF_RFAFunctions_MyID]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAAuthorizations_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAAuthorizations] DROP CONSTRAINT [DF_RFAAuthorizations_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAAuthorizations_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAAuthorizations] DROP CONSTRAINT [DF_RFAAuthorizations_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] DROP CONSTRAINT [DF_Projects_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] DROP CONSTRAINT [DF_Projects_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] DROP CONSTRAINT [DF_Projects_Status]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] DROP CONSTRAINT [DF_Projects_RegistDate]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_MaterialType_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MaterialType] DROP CONSTRAINT [DF_MaterialType_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_Closed]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_Closed]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_LoginByMobile]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_LoginByMobile]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_LoginByDesktop]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_LoginByDesktop]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_RegistDate]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_DepartmentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] DROP CONSTRAINT [DF_FrontUsers_DepartmentID]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] DROP CONSTRAINT [DF_Enterprises_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] DROP CONSTRAINT [DF_Enterprises_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] DROP CONSTRAINT [DF_Enterprises_RegistDate]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] DROP CONSTRAINT [DF_Enterprises_Status]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterprisePropertys_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterprisePropertys] DROP CONSTRAINT [DF_EnterprisePropertys_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterprisePropertys_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterprisePropertys] DROP CONSTRAINT [DF_EnterprisePropertys_Available]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterpriseProfessions_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterpriseProfessions] DROP CONSTRAINT [DF_EnterpriseProfessions_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterpriseProfessions_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterpriseProfessions] DROP CONSTRAINT [DF_EnterpriseProfessions_Available]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] DROP CONSTRAINT [DF_Departments_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] DROP CONSTRAINT [DF_Departments_Deleted]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] DROP CONSTRAINT [DF_Departments_Available]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_ParentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] DROP CONSTRAINT [DF_Departments_ParentID]
END

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
DROP TABLE [dbo].[UserRoles]
GO
/****** Object:  Table [dbo].[UserLoginState]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserLoginState]') AND type in (N'U'))
DROP TABLE [dbo].[UserLoginState]
GO
/****** Object:  Table [dbo].[UserLoginLog]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserLoginLog]') AND type in (N'U'))
DROP TABLE [dbo].[UserLoginLog]
GO
/****** Object:  Table [dbo].[SyncState]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncState]') AND type in (N'U'))
DROP TABLE [dbo].[SyncState]
GO
/****** Object:  Table [dbo].[SceneTypes]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SceneTypes]') AND type in (N'U'))
DROP TABLE [dbo].[SceneTypes]
GO
/****** Object:  Table [dbo].[Scenes]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Scenes]') AND type in (N'U'))
DROP TABLE [dbo].[Scenes]
GO
/****** Object:  Table [dbo].[RFARoles]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFARoles]') AND type in (N'U'))
DROP TABLE [dbo].[RFARoles]
GO
/****** Object:  Table [dbo].[RFAFunctions]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAFunctions]') AND type in (N'U'))
DROP TABLE [dbo].[RFAFunctions]
GO
/****** Object:  Table [dbo].[RFAAuthorizations]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAAuthorizations]') AND type in (N'U'))
DROP TABLE [dbo].[RFAAuthorizations]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))
DROP TABLE [dbo].[Projects]
GO
/****** Object:  Table [dbo].[MaterialType]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MaterialType]') AND type in (N'U'))
DROP TABLE [dbo].[MaterialType]
GO
/****** Object:  Table [dbo].[FrontUsers]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FrontUsers]') AND type in (N'U'))
DROP TABLE [dbo].[FrontUsers]
GO
/****** Object:  Table [dbo].[Enterprises]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Enterprises]') AND type in (N'U'))
DROP TABLE [dbo].[Enterprises]
GO
/****** Object:  Table [dbo].[EnterprisePropertys]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnterprisePropertys]') AND type in (N'U'))
DROP TABLE [dbo].[EnterprisePropertys]
GO
/****** Object:  Table [dbo].[EnterpriseProfessions]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnterpriseProfessions]') AND type in (N'U'))
DROP TABLE [dbo].[EnterpriseProfessions]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 2015/6/30 14:56:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND type in (N'U'))
DROP TABLE [dbo].[Departments]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Departments](
	[DepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NULL,
	[EnterpriseID] [varchar](32) NOT NULL,
	[ParentID] [int] NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Available] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EnterpriseProfessions]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnterpriseProfessions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EnterpriseProfessions](
	[EnterpriseProfessionID] [char](1) NOT NULL,
	[Name] [nvarchar](32) NULL,
	[Available] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EnterpriseProfessions] PRIMARY KEY CLUSTERED 
(
	[EnterpriseProfessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EnterprisePropertys]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnterprisePropertys]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EnterprisePropertys](
	[EnterprisePropertyID] [char](1) NOT NULL,
	[Name] [nvarchar](32) NULL,
	[Available] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EnterprisePropertys] PRIMARY KEY CLUSTERED 
(
	[EnterprisePropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Enterprises]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Enterprises]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Enterprises](
	[EnterpriseID] [varchar](32) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[ProfessionID] [char](1) NOT NULL,
	[PropertyID] [char](1) NOT NULL,
	[Province] [varchar](32) NOT NULL,
	[City] [varchar](32) NOT NULL,
	[Address] [nvarchar](64) NULL,
	[Telephone] [varchar](32) NULL,
	[Fax] [varchar](32) NULL,
	[Status] [tinyint] NOT NULL,
	[RegistDate] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Enterprises] PRIMARY KEY CLUSTERED 
(
	[EnterpriseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FrontUsers]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FrontUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FrontUsers](
	[UserID] [varchar](32) NOT NULL,
	[Name] [nvarchar](32) NULL,
	[Picture] [nvarchar](50) NULL,
	[Password] [varchar](32) NULL,
	[EnterpiseID] [varchar](32) NOT NULL,
	[DepartmentID] [int] NULL,
	[Mobile] [varchar](32) NULL,
	[RegistDate] [datetime] NOT NULL,
	[LastDate] [datetime] NULL,
	[LastIP] [varchar](32) NULL,
	[LoginByDesktop] [bit] NOT NULL,
	[LoginByMobile] [bit] NOT NULL,
	[Closed] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_FrontUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MaterialType]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MaterialType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MaterialType](
	[MaterialTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Available] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_MaterialType] PRIMARY KEY CLUSTERED 
(
	[MaterialTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Projects](
	[ProjectID] [varchar](32) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[EnterpriseID] [varchar](32) NOT NULL,
	[Departments] [varchar](256) NULL,
	[Managers] [varchar](256) NULL,
	[Roles] [varchar](256) NULL,
	[RegistDate] [datetime] NOT NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RFAAuthorizations]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAAuthorizations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RFAAuthorizations](
	[RoleID] [int] NOT NULL,
	[FunctionID] [varchar](256) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_RFAAuthorizations] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[FunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RFAFunctions]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAFunctions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RFAFunctions](
	[FunctionID] [varchar](256) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[MyID] [varchar](32) NOT NULL,
	[ParentID] [varchar](256) NOT NULL,
	[Desription] [varchar](256) NULL,
	[Available] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RFAFunctions] PRIMARY KEY CLUSTERED 
(
	[FunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RFARoles]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFARoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RFARoles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[OwnerID] [varchar](256) NULL,
	[Description] [nvarchar](256) NULL,
	[Available] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RFARoles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Scenes]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Scenes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Scenes](
	[SceneID] [varchar](32) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[EnterpriseID] [varchar](32) NOT NULL,
	[ProjectID] [varchar](32) NOT NULL,
	[ParentSceneID] [varchar](32) NOT NULL,
	[Woker] [varchar](2000) NULL,
	[Address] [varchar](256) NULL,
	[LatitudeAndLongitude] [varchar](50) NULL,
	[RegistDate] [datetime] NOT NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Type] [varchar](256) NULL,
	[Status] [tinyint] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[HasData] [bit] NOT NULL,
 CONSTRAINT [PK_Scenes] PRIMARY KEY CLUSTERED 
(
	[SceneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SceneTypes]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SceneTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SceneTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ParentID] [int] NULL,
	[Available] [bit] NOT NULL,
	[EnterpriseID] [varchar](32) NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK__SceneTyp__3214EC273E1D39E1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SyncState]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncState]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SyncState](
	[SyncStateID] [int] IDENTITY(1,1) NOT NULL,
	[ActionType] [tinyint] NOT NULL,
	[SyncTime] [datetime] NOT NULL,
	[UserID] [varchar](32) NOT NULL,
	[DeviceID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SyncState] PRIMARY KEY CLUSTERED 
(
	[SyncStateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLoginLog]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserLoginLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserLoginLog](
	[UserLoginLogID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](32) NOT NULL,
	[UserName] [nvarchar](32) NOT NULL,
	[IP] [varchar](50) NOT NULL,
	[Time] [datetime] NOT NULL,
	[Device] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[Description] [nvarchar](100) NULL,
 CONSTRAINT [PK_UserLoginLog] PRIMARY KEY CLUSTERED 
(
	[UserLoginLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLoginState]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserLoginState]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserLoginState](
	[UserLoginStateID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](32) NOT NULL,
	[UserName] [varchar](32) NULL,
	[LoginIP] [nvarchar](32) NULL,
	[Device] [nvarchar](50) NULL,
	[LoginToken] [varchar](1000) NULL,
	[LoginTime] [datetime] NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK__UserLogi__86A0D8F625518C17] PRIMARY KEY CLUSTERED 
(
	[UserLoginStateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2015/6/30 14:56:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserRoles](
	[UserID] [varchar](32) NOT NULL,
	[RoleID] [int] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_ParentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] ADD  CONSTRAINT [DF_Departments_ParentID]  DEFAULT ((0)) FOR [ParentID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] ADD  CONSTRAINT [DF_Departments_Available]  DEFAULT ((0)) FOR [Available]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] ADD  CONSTRAINT [DF_Departments_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Departments_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Departments] ADD  CONSTRAINT [DF_Departments_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterpriseProfessions_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterpriseProfessions] ADD  CONSTRAINT [DF_EnterpriseProfessions_Available]  DEFAULT ((1)) FOR [Available]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterpriseProfessions_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterpriseProfessions] ADD  CONSTRAINT [DF_EnterpriseProfessions_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterprisePropertys_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterprisePropertys] ADD  CONSTRAINT [DF_EnterprisePropertys_Available]  DEFAULT ((1)) FOR [Available]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_EnterprisePropertys_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[EnterprisePropertys] ADD  CONSTRAINT [DF_EnterprisePropertys_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] ADD  CONSTRAINT [DF_Enterprises_Status]  DEFAULT ((0)) FOR [Status]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] ADD  CONSTRAINT [DF_Enterprises_RegistDate]  DEFAULT (getdate()) FOR [RegistDate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] ADD  CONSTRAINT [DF_Enterprises_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Enterprises_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Enterprises] ADD  CONSTRAINT [DF_Enterprises_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_DepartmentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_DepartmentID]  DEFAULT ((0)) FOR [DepartmentID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_RegistDate]  DEFAULT (getdate()) FOR [RegistDate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_LoginByDesktop]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_LoginByDesktop]  DEFAULT ((1)) FOR [LoginByDesktop]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_LoginByMobile]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_LoginByMobile]  DEFAULT ((1)) FOR [LoginByMobile]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_Closed]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_Closed]  DEFAULT ((0)) FOR [Closed]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FrontUsers_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FrontUsers] ADD  CONSTRAINT [DF_FrontUsers_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_MaterialType_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MaterialType] ADD  CONSTRAINT [DF_MaterialType_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_RegistDate]  DEFAULT (getdate()) FOR [RegistDate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_Status]  DEFAULT ((0)) FOR [Status]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Projects_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAAuthorizations_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAAuthorizations] ADD  CONSTRAINT [DF_RFAAuthorizations_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAAuthorizations_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAAuthorizations] ADD  CONSTRAINT [DF_RFAAuthorizations_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_MyID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] ADD  CONSTRAINT [DF_RFAFunctions_MyID]  DEFAULT ('') FOR [MyID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_ParentID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] ADD  CONSTRAINT [DF_RFAFunctions_ParentID]  DEFAULT ('ROOT') FOR [ParentID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] ADD  CONSTRAINT [DF_RFAFunctions_Available]  DEFAULT ((1)) FOR [Available]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFAFunctions_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFAFunctions] ADD  CONSTRAINT [DF_RFAFunctions_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_OwnerID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] ADD  CONSTRAINT [DF_RFARoles_OwnerID]  DEFAULT ('') FOR [OwnerID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] ADD  CONSTRAINT [DF_RFARoles_Available]  DEFAULT ((1)) FOR [Available]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_RFARoles_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[RFARoles] ADD  CONSTRAINT [DF_RFARoles_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_ParentSceneID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_ParentSceneID]  DEFAULT ((0)) FOR [ParentSceneID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_RegistDate]  DEFAULT (getdate()) FOR [RegistDate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_Status]  DEFAULT ((0)) FOR [Status]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_UpdateTime_1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_UpdateTime_1]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Scenes_HasData]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Scenes] ADD  CONSTRAINT [DF_Scenes_HasData]  DEFAULT ((0)) FOR [HasData]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SceneTypes_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SceneTypes] ADD  CONSTRAINT [DF_SceneTypes_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserLoginState_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserLoginState] ADD  CONSTRAINT [DF_UserLoginState_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserRoles_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [DF_UserRoles_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_UserRoles_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [DF_UserRoles_Deleted]  DEFAULT ((0)) FOR [Deleted]
END

GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Enterprises', N'COLUMN',N'EnterpriseID'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ProfessionID+PropertyID+5个数字编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Enterprises', @level2type=N'COLUMN',@level2name=N'EnterpriseID'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Departments'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DepartmentID|DepartmentID|...' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Departments'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Managers'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'UserID|UserID|UserID|...' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Managers'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Projects', N'COLUMN',N'Roles'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'RoleID|RoleID|RoleID|...' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Projects', @level2type=N'COLUMN',@level2name=N'Roles'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAAuthorizations', N'COLUMN',N'Deleted'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标记为是否删除，一旦删除就不能再使用，然后由后台程序定期删除，这样做的目的是便于缓存区同步被删除的数据。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAAuthorizations', @level2type=N'COLUMN',@level2name=N'Deleted'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAFunctions', N'COLUMN',N'FunctionID'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ParentID.MyID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAFunctions', @level2type=N'COLUMN',@level2name=N'FunctionID'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFARoles', N'COLUMN',N'OwnerID'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外部拥有者，可以不设置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFARoles', @level2type=N'COLUMN',@level2name=N'OwnerID'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Scenes', N'COLUMN',N'Woker'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'UserID|UserID|...' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Scenes', @level2type=N'COLUMN',@level2name=N'Woker'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'UserRoles', N'COLUMN',N'Deleted'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标记为是否删除，一旦删除就不能再使用，然后由后台程序定期删除，这样做的目的是便于缓存区同步被删除的数据。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserRoles', @level2type=N'COLUMN',@level2name=N'Deleted'
GO
