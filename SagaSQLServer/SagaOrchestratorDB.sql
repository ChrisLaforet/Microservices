USE [Microservices]
GO
/****** Object:  Table [dbo].[SagaOrchestrator]    Script Date: 1/14/2020 9:33:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SagaOrchestrator](
	[guid] [varchar](100) NOT NULL,
	[orchestratorName] [varchar](max) NOT NULL,
	[stageName] [varchar](max) NOT NULL,
	[creationTimestamp] [datetime2](7) NOT NULL,
	[completionTimestamp] [datetime2](7) NULL,
	[stageComplete] [bit] NOT NULL,
	[stageSuccess] [bit] NOT NULL,
	[stageRewinding] [bit] NOT NULL,
 CONSTRAINT [PK_SagaOrchestrator] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
