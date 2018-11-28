DROP INDEX iDocumentOldId   
    ON sb.Document;  


	alter table sb.document
add state nvarchar(255) null
	--REMOVE ab testing


	--need to run FixFilesAsync in console app on production

begin tran
delete from sb.[Transaction]
where [User_id] in (select Id from sb.[User] where EmailConfirmed = 0 and PhoneNumberConfirmed = 0)
commit