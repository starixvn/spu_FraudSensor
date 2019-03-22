/*
Creator: 20190321@Long
Task:    Exclude Internal Accounts from Office IP [Redmine ID: 108765]
Server:  bodb_FraudSensor

Reviewer: Billy
*/ 

CREATE TABLE [dbo].[OfficeIPInternalAccount](
	[CustId]		[int] NOT NULL,
	[RoleId]		[tinyint] NOT NULL,
	[CreatedBy]		[int] NULL,
	[CreatedDate]	[datetime] NULL,
	[IsActive]		[bit] NULL,
 CONSTRAINT [PK_OfficeIPInternalAccount] PRIMARY KEY CLUSTERED 
(
	[CustId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DATA]
) ON [DATA]

GO
