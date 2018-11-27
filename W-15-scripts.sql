DROP INDEX iDocumentOldId   
    ON sb.Document;  


	alter table sb.document
add state nvarchar(255) null
	--REMOVE ab testing


	--need to run FixFilesAsync in console app on production
--remove from sb.transaction the fat generated from migrate the old users
begin tran
delete from sb.[Transaction]
where [User_id] in (select Id from sb.[User] where OldUser = 1 and PhoneNumberConfirmed = 0)
commit