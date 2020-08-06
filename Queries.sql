-- Add a new column '[NewColumnName]' to table '[TableName]' in schema '[dbo]'
ALTER TABLE [sb].[course2]
    ADD [Version] /*new_column_name*/ int /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

update sb.Course2
set [version] = 0
where [version] is null