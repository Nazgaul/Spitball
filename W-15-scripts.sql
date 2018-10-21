ALTER TABLE sb.[User]
ADD Culture nvarchar(256);
ALTER TABLE sb.[User]
ADD Country nvarchar(2);
ALTER TABLE zbox.University
ADD Pending bit;
insert into sb.HiLoGenerator values ('university',180000)


-- FraudScore reset
update sb.[User] 
set FraudScore = 0

--Hadar SQL 
create table sb.PhoneNumberIso (ISO nvarchar(2), CountryCode nvarchar(20))

insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AF','93')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AL','355')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DZ','213')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AS','1-684')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AD','376')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AO','244')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AI','1-264')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AQ','672')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AG','1-268')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AR','54')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AM','374')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AW','297')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AU','61')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AT','43')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AZ','994')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BS','1-242')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BH','973')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BD','880')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BB','1-246')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BY','375')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BE','32')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BZ','501')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BJ','229')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BM','1-441')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BT','975')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BO','591')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BA','387')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BW','267')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BR','55')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IO','246')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VG','1-284')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BN','673')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BG','359')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BF','226')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BI','257')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KH','855')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CM','237')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CV','238')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KY','1-345')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CF','236')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TD','235')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CL','56')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CN','86')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CX','61')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CC','61')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CO','57')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KM','269')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CK','682')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CR','506')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('HR','385')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CU','53')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CW','599')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CY','357')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CZ','420')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CD','243')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DK','45')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DJ','253')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DM','1-767')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DO','1-809, 1-829, 1-849')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TL','670')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('EC','593')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('EG','20')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SV','503')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GQ','240')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ER','291')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('EE','372')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ET','251')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FK','500')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FO','298')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FJ','679')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FI','358')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FR','33')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PF','689')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GA','241')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GM','220')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GE','995')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('DE','49')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GH','233')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GI','350')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GR','30')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GL','299')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GD','1-473')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GU','1-671')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GT','502')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GG','44-1481')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GN','224')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GW','245')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GY','592')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('HT','509')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('HN','504')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('HK','852')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('HU','36')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IS','354')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IN','91')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ID','62')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IR','98')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IQ','964')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IE','353')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IM','44-1624')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IL','972')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('IT','39')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CI','225')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('JM','1-876')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('JP','81')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('JE','44-1534')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('JO','962')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KZ','7')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KE','254')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KI','686')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('XK','383')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KW','965')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KG','996')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LA','856')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LV','371')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LB','961')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LS','266')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LR','231')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LY','218')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LI','423')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LT','370')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LU','352')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MO','853')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MK','389')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MG','261')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MW','265')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MY','60')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MV','960')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ML','223')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MT','356')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MH','692')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MR','222')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MU','230')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('YT','262')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MX','52')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('FM','691')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MD','373')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MC','377')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MN','976')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ME','382')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MS','1-664')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MA','212')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MZ','258')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MM','95')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NA','264')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NR','674')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NP','977')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NL','31')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AN','599')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NC','687')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NZ','64')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NI','505')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NE','227')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NG','234')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NU','683')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KP','850')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MP','1-670')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('NO','47')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('OM','968')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PK','92')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PW','680')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PS','970')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PA','507')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PG','675')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PY','595')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PE','51')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PH','63')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PN','64')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PL','48')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PT','351')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PR','1-787, 1-939')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('QA','974')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CG','242')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('RE','262')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('RO','40')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('RU','7')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('RW','250')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('BL','590')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SH','290')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KN','1-869')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LC','1-758')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('MF','590')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('PM','508')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VC','1-784')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('WS','685')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SM','378')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ST','239')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SA','966')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SN','221')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('RS','381')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SC','248')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SL','232')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SG','65')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SX','1-721')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SK','421')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SI','386')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SB','677')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SO','252')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ZA','27')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('KR','82')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SS','211')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ES','34')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('LK','94')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SD','249')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SR','597')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SJ','47')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SZ','268')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SE','46')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('CH','41')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('SY','963')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TW','886')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TJ','992')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TZ','255')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TH','66')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TG','228')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TK','690')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TO','676')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TT','1-868')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TN','216')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TR','90')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TM','993')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TC','1-649')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('TV','688')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VI','1-340')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('UG','256')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('UA','380')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('AE','971')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('GB','44')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('US','1')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('UY','598')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('UZ','998')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VU','678')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VA','379')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VE','58')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('VN','84')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('WF','681')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('EH','212')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('YE','967')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ZM','260')
insert into sb.PhoneNumberIso (ISO, CountryCode) VALUES  ('ZW','263')


update sb.[User]
  set sb.[User].Country = ISO
  FROM [sb].[PhoneNumberIso] I
	inner join sb.[User] U 
	on REPLACE(U.PhoneNumberHash, '+' , '')like I.[CountryCode] + '%';
drop table sb.PhoneNumberIso


/****** Object:  Table [sb].[UserCourseRelationship]    Script Date: 08/10/2018 10:49:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [sb].[UserCourseRelationship](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[CourseId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[CourseId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [sb].[UserCourseRelationship]  WITH CHECK ADD  CONSTRAINT [UserCourseRelationship_Course] FOREIGN KEY([CourseId])
REFERENCES [Zbox].[Box] ([BoxId])
GO

ALTER TABLE [sb].[UserCourseRelationship] CHECK CONSTRAINT [UserCourseRelationship_Course]
GO

ALTER TABLE [sb].[UserCourseRelationship]  WITH CHECK ADD  CONSTRAINT [UserCourseRelationship_User] FOREIGN KEY([UserId])
REFERENCES [sb].[User] ([Id])
GO

ALTER TABLE [sb].[UserCourseRelationship] CHECK CONSTRAINT [UserCourseRelationship_User]
GO


alter table sb.[questionSubject]
Add Subject_Hebrew nvarchar(255)
--TODO need someone to popluate it

alter table [sb].[Transaction]
add InvitedUserId [bigint];

ALTER TABLE [sb].[Transaction]  WITH CHECK ADD  CONSTRAINT [Transaction_InvitedUser] FOREIGN KEY([InvitedUserId])
REFERENCES [sb].[User] ([Id]);


ALTER TABLE sb.Question
DROP CONSTRAINT Question_AskQuestionSubject;  


CREATE TABLE [sb].[UserLocation](
	[Id] [uniqueidentifier] NOT NULL,
	[Ip] [nvarchar](255) NULL,
	[Country] [nvarchar](10) NULL,
	[UserId] [bigint] NULL,
	[CreationTime] [datetime2](7) NULL,
	[UpdateTime] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [sb].[UserLocation]  WITH CHECK ADD  CONSTRAINT [UserLocation_User] FOREIGN KEY([UserId])
REFERENCES [sb].[User] ([Id])
GO

ALTER TABLE [sb].[UserLocation] CHECK CONSTRAINT [UserLocation_User]
GO

alter table sb.question
add State nvarchar(255);


---V7
-- need to create signalr service