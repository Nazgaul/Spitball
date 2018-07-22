alter table sb.[user]
add Created datetime2(7);
alter table sb.[user]
add Fictive bit;
alter table sb.QuestionSubject
add OrderColumn int;
update sb.QuestionSubject
set OrderColumn = 1
where id = 16