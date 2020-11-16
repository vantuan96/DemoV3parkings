﻿IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblEventPayment')
BEGIN
CREATE TABLE [dbo].tblEventPayment(
	[EventId] [varchar](128) NOT NULL,
	[DateCreated] [datetime] NULL,
	[TimeIn] [datetime] NULL,
	[TimeOut] [datetime] NULL,
	[Plate] [nvarchar](500) NULL,
	[Money] [int] NULL,
	[OrderId] [varchar](128) NULL,
	[PaymentStatus] [int] NULL,
	[isSuccessQRCode] [bit] NOT NULL DEFAULT(0),
	[isSuccessPay] [bit] NOT NULL DEFAULT(0),
	[ResponseContentQRCode] [nvarchar](1000) NULL,
	[ResponseContentPay] [nvarchar](1000) NULL,
 CONSTRAINT [PK_tblEventPayment] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END