CREATE DATABASE Sanitizer;
GO

USE [Sanitizer]
GO
/****** Object:  Table [dbo].[SensitiveWords]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensitiveWords](
	[Word] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Word] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SensitiveWords]  WITH CHECK ADD  CONSTRAINT [Check_NonEmptyWord] CHECK  ((len([Word])>(0)))
GO
ALTER TABLE [dbo].[SensitiveWords] CHECK CONSTRAINT [Check_NonEmptyWord]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSensitiveWord]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteSensitiveWord]
    @Word NVARCHAR(100)
AS
BEGIN
    DELETE FROM SensitiveWords
    WHERE Word = @Word;
END;
GO
/****** Object:  StoredProcedure [dbo].[FindSensitiveWords]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[FindSensitiveWords]
    @DirtyString NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT Word
    FROM SensitiveWords
    WHERE @DirtyString LIKE '%' + Word + '%'

	END
GO
/****** Object:  StoredProcedure [dbo].[GetSensitiveWord]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSensitiveWord]
    @Word NVARCHAR(100)
AS
BEGIN
    SELECT *
    FROM SensitiveWords
    WHERE Word = @Word;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetSensitiveWordsPaged]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSensitiveWordsPaged]
    @Page INT,
    @PageSize INT,
    @SortOrder INT = 1,
    @Search NVARCHAR(100) = NULL,
    @TotalCount INT OUT
AS
BEGIN
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    
    ;WITH CTE AS (
        SELECT COUNT(*) OVER () AS TotalCount,
               ROW_NUMBER() OVER (ORDER BY 
                                     CASE WHEN @SortOrder = -1 THEN Word END DESC,
                                     CASE WHEN @SortOrder = 1 THEN Word END ASC) AS RowNum,
               *
        FROM SensitiveWords
        WHERE (@Search IS NULL OR Word LIKE '%' + @Search + '%')
    )
    SELECT @TotalCount = TotalCount
    FROM CTE
    WHERE RowNum = 1;

    SELECT *
    FROM (
        SELECT ROW_NUMBER() OVER (ORDER BY 
                                    CASE WHEN @SortOrder = -1 THEN Word END DESC,
                                    CASE WHEN @SortOrder = 1 THEN Word END ASC) AS RowNum,
               *
        FROM SensitiveWords
        WHERE (@Search IS NULL OR Word LIKE '%' + @Search + '%')
    ) AS PaginatedData
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertSensitiveWord]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertSensitiveWord]
    @Word NVARCHAR(100)
AS
BEGIN
    IF (LEN(@Word) > 0)
    BEGIN
        INSERT INTO SensitiveWords (Word)
        VALUES (@Word);
    END
    ELSE
    BEGIN
        RAISERROR('Cannot insert empty string.', 16, 1);
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdateSensitiveWord]    Script Date: 2024/05/11 13:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateSensitiveWord]
    @OldWord NVARCHAR(100),
    @NewWord NVARCHAR(100)
AS
BEGIN
    IF (LEN(@NewWord) > 0)
    BEGIN
        UPDATE SensitiveWords
        SET Word = @NewWord
        WHERE Word = @OldWord;
	IF (@@ROWCOUNT = 0)
		BEGIN
            RAISERROR('Word wasn''t updated. The word does not exist.', 16, 1 );
        END
    END
    ELSE
    BEGIN
        RAISERROR('Cannot update to an empty string.', 16, 1);
    END
END;
GO

INSERT INTO SensitiveWords (Word)
VALUES
('ACTION'),
('ADD'),
('ALL'),
('ALLOCATE'),
('ALTER'),
('ANY'),
('APPLICATION'),
('ARE'),
('AREA'),
('ASC'),
('ASSERTION'),
('ATOMIC'),
('AUTHORIZATION'),
('AVG'),
('BEGIN'),
('BY'),
('CALL'),
('CASCADE'),
('CASCADED'),
('CATALOG'),
('CHECK'),
('CLOSE'),
('COLUMN'),
('COMMIT'),
('COMPRESS'),
('CONNECT'),
('CONNECTION'),
('CONSTRAINT'),
('CONSTRAINTS'),
('CONTINUE'),
('CONVERT'),
('CORRESPONDING'),
('CREATE'),
('CROSS'),
('CURRENT'),
('CURRENT_PATH'),
('CURRENT_SCHEMA'),
('CURRENT_SCHEMAID'),
('CURRENT_USER'),
('CURRENT_USERID'),
('CURSOR'),
('DATA'),
('DEALLOCATE'),
('DECLARE'),
('DEFAULT'),
('DEFERRABLE'),
('DEFERRED'),
('DELETE'),
('DESC'),
('DESCRIBE'),
('DESCRIPTOR'),
('DETERMINISTIC'),
('DIAGNOSTICS'),
('DIRECTORY'),
('DISCONNECT'),
('DISTINCT'),
('DO'),
('DOMAIN'),
('DOUBLEATTRIBUTE'),
('DROP'),
('EACH'),
('EXCEPT'),
('EXCEPTION'),
('EXEC'),
('EXECUTE'),
('EXTERNAL'),
('FETCH'),
('FLOAT'),
('FOREIGN'),
('FOUND'),
('FULL'),
('FUNCTION'),
('GET'),
('GLOBAL'),
('GO'),
('GOTO'),
('GRANT'),
('GROUP'),
('HANDLER'),
('HAVING'),
('IDENTITY'),
('IMMEDIATE'),
('INDEX'),
('INDEXED'),
('INDICATOR'),
('INITIALLY'),
('INNER'),
('INOUT'),
('INPUT'),
('INSENSITIVE'),
('INSERT'),
('INTERSECT'),
('INTO'),
('ISOLATION'),
('JOIN'),
('KEY'),
('LANGUAGE'),
('LAST'),
('LEAVE'),
('LEVEL'),
('LOCAL'),
('LONGATTRIBUTE'),
('LOOP'),
('MODIFIES'),
('MODULE'),
('NAMES'),
('NATIONAL'),
('NATURAL'),
('NEXT'),
('NULLIF'),
('ON'),
('ONLY'),
('OPEN'),
('OPTION'),
('ORDER'),
('OUT'),
('OUTER'),
('OUTPUT'),
('OVERLAPS'),
('OWNER'),
('PARTIAL'),
('PATH'),
('PRECISION'),
('PREPARE'),
('PRESERVE'),
('PRIMARY'),
('PRIOR'),
('PRIVILEGES'),
('PROCEDURE'),
('PUBLIC'),
('READ'),
('READS'),
('REFERENCES'),
('RELATIVE'),
('REPEAT'),
('RESIGNAL'),
('RESTRICT'),
('RETURN'),
('RETURNS'),
('REVOKE'),
('ROLLBACK'),
('ROUTINE'),
('ROW'),
('ROWS'),
('SCHEMA'),
('SCROLL'),
('SECTION'),
('SELECT'),
('SEQ'),
('SEQUENCE'),
('SESSION'),
('SESSION_USER'),
('SESSION_USERID'),
('SET'),
('SIGNAL'),
('SOME'),
('SPACE'),
('SPECIFIC'),
('SQL'),
('SQLCODE'),
('SQLERROR'),
('SQLEXCEPTION'),
('SQLSTATE'),
('SQLWARNING'),
('STATEMENT'),
('STRINGATTRIBUTE'),
('SUM'),
('SYSACC'),
('SYSHGH'),
('SYSLNK'),
('SYSNIX'),
('SYSTBLDEF'),
('SYSTBLDSC'),
('SYSTBT'),
('SYSTBTATT'),
('SYSTBTDEF'),
('SYSUSR'),
('SYSTEM_USER'),
('SYSVIW'),
('SYSVIWCOL'),
('TABLE'),
('TABLETYPE'),
('TEMPORARY'),
('TRANSACTION'),
('TRANSLATE'),
('TRANSLATION'),
('TRIGGER'),
('UNDO'),
('UNION'),
('UNIQUE'),
('UNTIL'),
('UPDATE'),
('USAGE'),
('USER'),
('USING'),
('VALUE'),
('VALUES'),
('VIEW'),
('WHERE'),
('WHILE'),
('WITH'),
('WORK'),
('WRITE'),
('ALLSCHEMAS'),
('ALLTABLES'),
('ALLVIEWS'),
('ALLVIEWTEXTS'),
('ALLCOLUMNS'),
('ALLINDEXES'),
('ALLINDEXCOLS'),
('ALLUSERS'),
('ALLTBTS'),
('TABLEPRIVILEGES'),
('TBTPRIVILEGES'),
('MYSCHEMAS'),
('MYTABLES'),
('MYTBTS'),
('MYVIEWS'),
('SCHEMAVIEWS'),
('DUAL'),
('SCHEMAPRIVILEGES'),
('SCHEMATABLES'),
('STATISTICS'),
('USRTBL'),
('STRINGTABLE'),
('LONGTABLE'),
('DOUBLETABLE'),
('SELECT * FROM');