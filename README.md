# ToDoAPI
Second API used
https://github.com/bleach206/TaskAPI
Things to do.
1) Add API Flow Chart
2) Add CORS
3) Add JWT

```sql
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ToDoUser](
	[UserId] [int] IDENTITY(5244,1) NOT NULL,
	[FullName] [nvarchar](255) NULL,
	[Email] [varchar](320) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ToDo](
	[ToDoId] [int] IDENTITY(4244,1) NOT NULL,
	[UserId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[IsCompleted] [bit] NULL,
	[RowVersion] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ToDoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ToDo]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[ToDoUser] ([UserId])
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Task](
	[TaskId] [int] IDENTITY(3244,1) NOT NULL,
	[ToDoId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[IsCompleted] [bit] NULL,
	[RowVersion] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Task]  WITH CHECK ADD FOREIGN KEY([ToDoId])
REFERENCES [dbo].[ToDo] ([ToDoId])
GO

CREATE TYPE [dbo].[TaskTableType] AS TABLE(
	[Name] [nvarchar](255) NOT NULL
)
GO

CREATE TYPE [dbo].[ToDoTableType] AS TABLE(
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL
)
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[usp_DeleteToDo]
@Id INT,
@UserId INT
AS
BEGIN
	UPDATE [dbo].[ToDo]
    SET [IsCompleted] = 1
	WHERE ToDoId = @Id AND UserId = @UserId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROC [dbo].[usp_GetTaskById]
@Id INT,
@UserId INT
AS
BEGIN
	SELECT * FROM Task T
	INNER JOIN ToDo TD 
	ON T.ToDoId = TD.ToDoId
	WHERE TD.UserId = @UserId AND T.TaskId = @Id AND T.IsCompleted = 0		
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[usp_GetTaskByPageNumberAndPageSize]
@ToDoId INT,
@UserId INT,
@PageNumber INT,
@PageSize INT
AS
BEGIN

	SELECT T.TaskId Id, T.[Name], T.IsCompleted, T.[RowVersion] FROM Task T
	INNER JOIN ToDo TD
	ON T.ToDoId = TD.ToDoId
	WHERE TD.UserId = @UserId AND TD.ToDoId = @ToDoId AND TD.IsCompleted = 0	
	ORDER BY T.[RowVersion] DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY	
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[usp_GetToDoById]
@Id INT,
@ToDoId INT
AS
BEGIN
	SELECT TD.ToDoId Id, TD.Name, TD.Description, TD.RowVersion FROM ToDo TD	
	WHERE TD.UserId = @Id AND TD.ToDoId = @ToDoId AND IsCompleted = 0	
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[usp_GetToDoByPageNumberAndPageSize]
@UserId INT,
@PageNumber INT,
@PageSize INT
AS
BEGIN
	SELECT TD.ToDoId Id, TD.[Name], TD.[Description], TD.RowVersion FROM ToDo TD
	WHERE TD.UserId = @UserId AND IsCompleted = 0	
	ORDER BY TD.[RowVersion] DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[usp_GetToDoBySearchTermAndPageNumberAndPageSize]
@UserId INT,
@SearchTerm NVARCHAR(255),
@PageNumber INT,
@PageSize INT
AS
BEGIN	
	IF(ISNULL(@SearchTerm, '') <> '' AND EXISTS (SELECT ToDoId FROM ToDo WHERE UserId = @UserId AND [Name] LIKE '%' + @SearchTerm + '%'))	
	BEGIN	
		SELECT TD.ToDoId Id, TD.Name, TD.Description, TD.RowVersion FROM ToDo TD	
		WHERE TD.IsCompleted = 0 AND TD.UserId = @UserId AND TD.[Name] LIKE '%' + @SearchTerm + '%'
		ORDER BY TD.RowVersion DESC
		OFFSET (@PageNumber - 1) * @PageSize ROWS
		FETCH NEXT @PageSize ROWS ONLY
	END
	ELSE
		EXEC	[dbo].[usp_GetToDoByPageNumberAndPageSize] @UserId,  @PageNumber,	@PageSize
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_InsertTask]
@UserId INT,
@ToDoId INT,
@Name NVARCHAR(255)
AS
BEGIN
	DECLARE @ID AS INT

	IF NOT EXISTS (
	SELECT TaskId FROM Task T
	INNER JOIN ToDo TD
	ON T.ToDoId = TD.ToDoId 
	WHERE T.NAME = @Name AND TD.UserId = @UserId
	)
	BEGIN
		INSERT INTO [dbo].[Task]([ToDoId],[Name], [IsCompleted]) VALUES (@ToDoId, @Name, 0);
		SET @ID = SCOPE_IDENTITY();	
	END
	ELSE
	BEGIN
		SET @ID = 0;
	END
	
	SELECT @ID;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[usp_InsertTaskList]
@UserId INT,
@ToDoId INT,
@TDVP TaskTableType READONLY
AS
BEGIN
	DECLARE @ID AS INT

	IF NOT EXISTS 
	(
		SELECT T.TaskId FROM Task T
		INNER JOIN @TDVP VP
		ON T.Name = VP.Name
		INNER JOIN ToDo TD
		ON T.ToDoId = TD.ToDoId
		WHERE TD.UserId = @UserId 
	)
	BEGIN
		INSERT INTO [dbo].[Task]([ToDoId], [Name], [IsCompleted])		
		SELECT @ToDoId ToDoId, T.Name, 0 FROM @TDVP AS T
		SET @ID = @ToDoId;
	END
	ELSE
	BEGIN
		SET @ID = 0;
	END
	
	SELECT @ID;
END



GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_InsertToDo]
@UserId INT,
@Name NVARCHAR(255),
@Description NVARCHAR(255)
AS
BEGIN
	DECLARE @ID AS INT

	IF NOT EXISTS (SELECT ToDoId FROM ToDo WHERE NAME = @Name AND UserId = @UserId)
	BEGIN
		INSERT INTO [dbo].[ToDo]([UserId],[Name],[Description], [IsCompleted]) VALUES (@UserId, @Name, @Description, 0);
		SET @ID = SCOPE_IDENTITY();	
	END
	ELSE
	BEGIN
		SET @ID = 0;
	END
	
	SELECT @ID;
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_InsertToDoList]
@UserId INT,
@TDVP ToDoTableType READONLY
AS
BEGIN
	DECLARE @ID AS INT

	IF NOT EXISTS 
	(SELECT TD.ToDoId FROM ToDo TD
	INNER JOIN @TDVP VP
	ON TD.Name = VP.Name
	WHERE UserId = @UserId)
	BEGIN
		INSERT INTO [dbo].[ToDo]([UserId], [Name], [Description], [IsCompleted])		
		SELECT @UserId UserId, T.Name, T.Description, 0 FROM @TDVP AS T
		SET @ID = @UserId;
	END
	ELSE
	BEGIN
		SET @ID = 0;
	END
	
	SELECT @ID;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_UpdateTaskIsCompleted]
@Id INT,
@UserId INT,
@IsCompleted BIT
AS
BEGIN
	UPDATE T	
    SET [IsCompleted] = @IsCompleted
	FROM dbo.Task T
	INNER JOIN ToDo TD
	ON T.ToDoId = TD.ToDoId
	WHERE T.TaskId = @Id AND TD.UserId = @UserId
END



GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_UpdateTaskName]
@Id INT,
@UserId INT,
@Name NVARCHAR(255)
AS
BEGIN
	UPDATE T	
    SET [Name] = @Name
	FROM dbo.Task T
	INNER JOIN ToDo TD
	ON T.ToDoId = TD.ToDoId
	WHERE T.TaskId = @Id AND TD.UserId = @UserId
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_UpdateToDo]
@UserId INT,
@Id INT,
@Name NVARCHAR(255),
@Description NVARCHAR(255)
AS
BEGIN
	UPDATE [dbo].[ToDo]
    SET [Name] = @Name,[Description] = @Description
	WHERE ToDoId = @Id AND UserId = @UserId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_UpdateToDoDescription]
@Id INT,
@UserId INT,
@Description NVARCHAR(255)
AS
BEGIN
	UPDATE [dbo].[ToDo]
    SET [Description] = @Description
	WHERE ToDoId = @Id AND UserId = @UserId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[usp_UpdateToDoName]
@Id INT,
@UserId INT,
@Name NVARCHAR(255)
AS
BEGIN
	UPDATE [dbo].[ToDo]
    SET [Name] = @Name
	WHERE ToDoId = @Id AND UserId = @UserId
END

GO

```
