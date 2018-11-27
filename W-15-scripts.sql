DROP INDEX iDocumentOldId   
    ON sb.Document;  

	--REMOVE ab testing


--remove from sb.transaction the fat generated from migrate the old users
begin tran
delete from sb.[Transaction]
where [User_id] in (select Id from sb.[User] where OldUser = 1 and PhoneNumberConfirmed = 0)
commit