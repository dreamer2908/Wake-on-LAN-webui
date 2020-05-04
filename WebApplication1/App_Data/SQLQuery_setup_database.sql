CREATE TABLE [dbo].[Computers] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [username] VARCHAR (50)  NULL,
    [name]     VARCHAR (50)  NULL,
    [ip]       VARCHAR (50)  NULL,
    [subnet]   VARCHAR (50)  NULL,
    [mac]      VARCHAR (50) NULL,
    CONSTRAINT [PK_Computers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_Computers_username_name_ip_subnet_mac]
    ON [dbo].[Computers]([username] ASC, [name] ASC, [ip] ASC, [subnet] ASC, [mac] ASC) WITH (IGNORE_DUP_KEY = ON);


CREATE TABLE [dbo].[Log]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [timestamp] DATETIME NULL, 
    [ip] VARCHAR(50) NULL, 
    [username] VARCHAR(50) NULL, 
    [action] NVARCHAR(50) NULL, 
    [detail] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Log] PRIMARY KEY ([Id]),
)

CREATE UNIQUE NONCLUSTERED INDEX [UIX_Log]
    ON [dbo].[Log]([timestamp] ASC, ip ASC, [username] ASC, [action] ASC) WITH (IGNORE_DUP_KEY = ON);


CREATE TABLE [dbo].[Settings]
(
    [key] NVARCHAR(50) NOT NULL, 
    [value] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Table] PRIMARY KEY ([key]) 
)


CREATE TABLE [dbo].[Users]
(
	[username] VARCHAR(50) NOT NULL PRIMARY KEY, 
    [password] VARCHAR(50) NOT NULL, 
    [admin] INT NOT NULL
)

INSERT INTO [dbo].[Users] ([username], [password], [admin]) VALUES ('administrator', 'da39a3ee5e6b4b0d3255bfef95601890afd80709', 1)
