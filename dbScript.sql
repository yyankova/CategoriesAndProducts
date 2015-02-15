USE [master]
GO
CREATE DATABASE [ProductsDB]
GO
ALTER DATABASE [ProductsDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ProductsDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ProductsDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ProductsDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ProductsDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ProductsDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ProductsDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ProductsDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ProductsDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ProductsDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ProductsDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ProductsDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ProductsDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ProductsDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ProductsDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ProductsDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ProductsDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ProductsDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ProductsDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ProductsDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ProductsDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ProductsDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ProductsDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ProductsDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ProductsDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ProductsDB] SET  MULTI_USER 
GO
ALTER DATABASE [ProductsDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ProductsDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ProductsDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ProductsDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [ProductsDB]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2/15/2015 11:02:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 2/15/2015 11:02:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[CategoryId] [int] NOT NULL,
	[Image] [image] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (1, N'Clothes', N'Clothes some description.')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (2, N'Accessories', N'Hats, jewels, scarfs description')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (3, N'Chocolate', N'Chocolate description')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (4, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (6, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (7, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (8, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (9, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (10, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (11, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (12, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (13, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (14, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (15, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (16, N'New category', N'fdfdfdfdfdfdfdf')
GO
INSERT [dbo].[Categories] ([CategoryId], [Name], [Description]) VALUES (17, N'New category', N'fdfdfdfdfdfdfdf')
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

GO
INSERT [dbo].[Products] ([ProductId], [Name], [Description], [CategoryId], [Image]) VALUES (1, N'Summer hat', N'Some description', 2, NULL)
GO
INSERT [dbo].[Products] ([ProductId], [Name], [Description], [CategoryId], [Image]) VALUES (2, N'Silk scarf', N'Some description for scarf', 2, NULL)
GO
INSERT [dbo].[Products] ([ProductId], [Name], [Description], [CategoryId], [Image]) VALUES (3, N'Torn jeans', N'Some description jeans', 1, NULL)
GO
INSERT [dbo].[Products] ([ProductId], [Name], [Description], [CategoryId], [Image]) VALUES (4, N'Chocolate 70%', N'Some chocolate description', 3, NULL)
GO
INSERT [dbo].[Products] ([ProductId], [Name], [Description], [CategoryId], [Image]) VALUES (5, N'Chocolate 80%', N'Some chocolate 2', 3, NULL)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories]
GO
USE [master]
GO
ALTER DATABASE [ProductsDB] SET  READ_WRITE 
GO
