USE [BCBackDB]
GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'UserRoles', N'COLUMN',N'Deleted'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserRoles', @level2type=N'COLUMN',@level2name=N'Deleted'

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
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] DROP CONSTRAINT [DF_BackUsers_UpdateTime]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_Closed]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] DROP CONSTRAINT [DF_BackUsers_Closed]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] DROP CONSTRAINT [DF_BackUsers_RegistDate]
END

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2015/6/28 19:29:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
DROP TABLE [dbo].[UserRoles]
GO
/****** Object:  Table [dbo].[RFARoles]    Script Date: 2015/6/28 19:29:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFARoles]') AND type in (N'U'))
DROP TABLE [dbo].[RFARoles]
GO
/****** Object:  Table [dbo].[RFAFunctions]    Script Date: 2015/6/28 19:29:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAFunctions]') AND type in (N'U'))
DROP TABLE [dbo].[RFAFunctions]
GO
/****** Object:  Table [dbo].[RFAAuthorizations]    Script Date: 2015/6/28 19:29:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RFAAuthorizations]') AND type in (N'U'))
DROP TABLE [dbo].[RFAAuthorizations]
GO
/****** Object:  Table [dbo].[BackUsers]    Script Date: 2015/6/28 19:29:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BackUsers]') AND type in (N'U'))
DROP TABLE [dbo].[BackUsers]
GO
/****** Object:  Table [dbo].[BackUsers]    Script Date: 2015/6/28 19:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BackUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BackUsers](
	[UserID] [varchar](32) NOT NULL,
	[Name] [nvarchar](32) NULL,
	[Password] [varchar](32) NULL,
	[Mobile] [varchar](32) NULL,
	[RegistDate] [datetime] NOT NULL,
	[LastDate] [datetime] NULL,
	[LastIP] [varchar](32) NULL,
	[Closed] [bit] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_BackUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RFAAuthorizations]    Script Date: 2015/6/28 19:29:40 ******/
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
/****** Object:  Table [dbo].[RFAFunctions]    Script Date: 2015/6/28 19:29:40 ******/
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
 CONSTRAINT [PK_RFAFunctions] PRIMARY KEY CLUSTERED 
(
	[FunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RFARoles]    Script Date: 2015/6/28 19:29:40 ******/
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
	[OwnerID] [varchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Available] [bit] NOT NULL,
 CONSTRAINT [PK_RFARoles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2015/6/28 19:29:40 ******/
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
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_RegistDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] ADD  CONSTRAINT [DF_BackUsers_RegistDate]  DEFAULT (getdate()) FOR [RegistDate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_Closed]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] ADD  CONSTRAINT [DF_BackUsers_Closed]  DEFAULT ((0)) FOR [Closed]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BackUsers_UpdateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BackUsers] ADD  CONSTRAINT [DF_BackUsers_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
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
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAAuthorizations', N'COLUMN',N'Deleted'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标记为是否删除，一旦删除就不能再使用，然后由后台程序定期删除，这样做的目的是便于缓存区同步被删除的数据。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAAuthorizations', @level2type=N'COLUMN',@level2name=N'Deleted'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFAFunctions', N'COLUMN',N'FunctionID'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ParentID.MyID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFAFunctions', @level2type=N'COLUMN',@level2name=N'FunctionID'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'RFARoles', N'COLUMN',N'OwnerID'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外部拥有者，可以不设置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RFARoles', @level2type=N'COLUMN',@level2name=N'OwnerID'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'UserRoles', N'COLUMN',N'Deleted'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标记为是否删除，一旦删除就不能再使用，然后由后台程序定期删除，这样做的目的是便于缓存区同步被删除的数据。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserRoles', @level2type=N'COLUMN',@level2name=N'Deleted'
GO
