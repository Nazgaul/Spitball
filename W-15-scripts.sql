﻿alter table sb.[user]
add Created datetime2(7);
alter table sb.[user]
add Fictive bit;
alter table sb.QuestionSubject
add OrderColumn int;
update sb.QuestionSubject
set OrderColumn = 1
where id = 16;
alter table sb.[question]
add Updated datetime2(7);
update sb.Question
set Updated = Created
WHERE updated IS NULL
update sb.[user]
set Fictive = 1
where email like '%demi.com'

alter table sb.question
add color nvarchar(255);
alter table sb.answer
add color nvarchar(255)


ALTER TABLE sb.[User]
ADD FraudScore INT;
--Done untill here