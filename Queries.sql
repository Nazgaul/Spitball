

--v5

ALTER TABLE [sb].[Course2]
    ADD [StartTime] /*new_column_name*/ DATETIME2(7) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

DROP VIEW [vw_Answer];

alter table sb.[user]
alter COLUMN fictive bit null
