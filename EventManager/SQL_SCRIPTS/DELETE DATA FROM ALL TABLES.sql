USE [SeminarRegistrationDB]
GO

/****** Object: Table [dbo].[Events] Script Date: 27.07.2018 1:16:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- disable all constraints
EXEC sp_MSForEachTable "ALTER TABLE ? NOCHECK CONSTRAINT all"

-- delete data in all tables
EXEC sp_MSForEachTable "DELETE FROM ?"

-- enable all constraints
exec sp_MSForEachTable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"

--if some of the tables have identity columns we may want to reseed them
EXEC sp_MSForEachTable "DBCC CHECKIDENT ( '?', RESEED, 0)"
