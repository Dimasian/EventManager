/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4206)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Express Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [EventsRegistrationDB]
GO

/****** Object:  Table [dbo].[Invitations]    Script Date: 27.07.2018 14:29:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Invitations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [int] NULL,
	[Date] [datetime] NOT NULL,
	[ParticipantTypeId] [int] NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Invitations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK_Invitations_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
GO

ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK_Invitations_Events]
GO

ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK_Invitations_ParticipantTypes] FOREIGN KEY([ParticipantTypeId])
REFERENCES [dbo].[ParticipantTypes] ([Id])
GO

ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK_Invitations_ParticipantTypes]
GO

ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK_Invitations_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK_Invitations_Users]
GO

