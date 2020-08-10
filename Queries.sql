-- Add a new column '[NewColumnName]' to table '[TableName]' in schema '[dbo]'
ALTER TABLE [sb].[document]
    ADD [Position] /*new_column_name*/ int /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

update x
set x.POSITION = New_CODE_DEST -1
from (
  SELECT *, ROW_NUMBER() OVER (partition by courseId ORDER BY [updatetime] desc) AS New_CODE_DEST
      FROM sb.Document
      where [state] = 'Ok') x