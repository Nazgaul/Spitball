-- Add a new column '[NewColumnName]' to table '[TableName]' in schema '[dbo]'
ALTER TABLE [sb].[Coupon]
    ADD [CourseId] /*new_column_name*/ bigint /*new_column_datatype*/ NULL /*new_column_nullability*/
GO


ALTER TABLE [sb].[Coupon]  WITH CHECK ADD  CONSTRAINT [FK_COUPON_COURSE] FOREIGN KEY([CourseId])
REFERENCES [sb].[course2] ([Id])
GO
ALTER TABLE [sb].[Coupon]
 CHECK CONSTRAINT [FK_COUPON_COURSE]


 ALTER TABLE [sb].[Coupon]
    ADD [value2] /*new_column_name*/ float /*new_column_datatype*/ NULL /*new_column_nullability*/
GO


ALTER table sb.coupon
ALTER COLUMN [value] DECIMAL(19,5) null


update sb.Coupon
set value2 = value
where value2 is null


 ALTER TABLE [sb].[UserCoupon]
    ADD [Amount] /*new_column_name*/ float /*new_column_datatype*/ NULL /*new_column_nullability*/
GO

 ALTER TABLE [sb].[UserCoupon]
    ADD [Currency] /*new_column_name*/ nvarchar(255) /*new_column_datatype*/ NULL /*new_column_nullability*/
GO