SET IDENTITY_INSERT [sb].[QuestionSubject] ON
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (1, N'Mathematics')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (2, N'History')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (3, N'English')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (4, N'Biology')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (5, N'Chemistry')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (6, N'Physics')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (7, N'Economics')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (8, N'Social Studies')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (9, N'Geography')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (10, N'Health')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (11, N'Arts')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (12, N'Business')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (13, N'Computers and Technology')
INSERT INTO [sb].[QuestionSubject] ([Id], [Subject]) VALUES (14, N'Languages')
SET IDENTITY_INSERT [sb].[QuestionSubject] OFF

INSERT INTO sb.hilogenerator VALUES('Question',1)
INSERT INTO sb.hilogenerator VALUES('User',1)

ALTER TABLE sb.Question  
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)  

 CREATE OR ALTER VIEW sb.question_indexer_view
	AS 
	SELECT ct.Id,qs.Subject,q.Price,q.Text, 
	Operation = 
	CASE 
		WHEN q.CorrectAnswer_id != NULL THEN 0
		WHEN ct.SYS_CHANGE_OPERATION = 'D' THEN 0
		ELSE 1
	End, 
	ct.SYS_CHANGE_VERSION AS RowNum
	FROM sb.Question q
	JOIN sb.QuestionSubject qs ON q.Subject_id = qs.Id
	right JOIN  
     CHANGETABLE(CHANGES Sb.Question, 0) AS CT 
	 ON q.Id = ct.Id
