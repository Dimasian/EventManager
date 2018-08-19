USE [EventsRegistrationDB]
GO

TRUNCATE TABLE  dbo.Users

INSERT INTO dbo.Users  
VALUES 
('Homer',NULL,'Simpson',NULL,'homer@simpson.com',NULL,'20180617'),
('Marge',NULL,'Simpson',NULL,'marge@simpson.com',NULL,'20180618'),
('Bart',NULL,'Simpson',NULL,'bart@simpson.com',NULL,'20180618'),
('Liza',NULL,'Simpson',NULL,'liza@simpson.com',NULL,'20180618'),
('Maggie',NULL,'Simpson',NULL,'maggie@simpson.com',NULL,'20180618'),
('Ned',NULL,'Simpson',NULL,'ned@flanders.com',NULL,'20180618'),
('Kent',NULL,'Simpson',NULL,'kent@brockman.com',NULL,'20180618'),
('Charles','Montgomery','Burns',NULL,'charles@burns.com',NULL,'20180618')
;


USE [EventsRegistrationDB]
GO
INSERT INTO dbo.EventTypes  
VALUES 
('Seminar'),
('Meeting'),
('Music event'),
('Sport event'),
('Other')
;


USE [EventsRegistrationDB]
GO
INSERT INTO dbo.ParticipantTypes  
VALUES 
('Manager'),
('Visitor'),
('Speaker'),
('Supplier'),
('Other')
;