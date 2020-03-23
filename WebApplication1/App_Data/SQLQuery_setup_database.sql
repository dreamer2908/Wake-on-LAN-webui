CREATE TABLE [dbo].[Computers] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [username] VARCHAR (50)  NULL,
    [name]     VARCHAR (50)  NULL,
    [ip]       VARCHAR (50)  NULL,
    [subnet]   VARCHAR (50)  NULL,
    [mac]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_Computers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_Computers_username_name_ip_subnet_mac]
    ON [dbo].[Computers]([username] ASC, [name] ASC, [ip] ASC, [subnet] ASC, [mac] ASC) WITH (IGNORE_DUP_KEY = ON);

