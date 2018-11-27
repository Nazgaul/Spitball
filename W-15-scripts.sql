DROP INDEX iDocumentOldId   
    ON sb.Document;  


	alter table sb.document
add state nvarchar(255) null
	--REMOVE ab testing

	--need to run FixFilesAsync in console app on production