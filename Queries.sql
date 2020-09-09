update sb.[user]
set Fictive = 0
where Fictive is null

ALTER TABLE [sb].[Tutor]
    ADD [TutorType] /*new_column_name*/ nvarchar(255) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO


ALTER TABLE [sb].[studyRoom]
    ADD [Code] /*new_column_name*/ nvarchar(255) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO
--need to create tailored tutor