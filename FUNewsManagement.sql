USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'FUNewsManagement')
BEGIN
    ALTER DATABASE FUNewsManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FUNewsManagement;
END
GO

CREATE DATABASE FUNewsManagement;
GO

USE FUNewsManagement;
GO

-- 1. Table: SystemAccount
CREATE TABLE SystemAccount (
    AccountID smallint IDENTITY(1,1) NOT NULL,
    AccountName nvarchar(100) NULL,
    AccountEmail nvarchar(70) NOT NULL UNIQUE,
    AccountRole int NULL, -- 1: Staff, 2: Lecturer
    AccountPassword nvarchar(70) NOT NULL,
    CONSTRAINT PK_SystemAccount PRIMARY KEY CLUSTERED (AccountID ASC)
);
GO

-- 2. Table: Category
CREATE TABLE Category (
    CategoryID smallint IDENTITY(1,1) NOT NULL,
    CategoryName nvarchar(100) NOT NULL,
    CategoryDesciption nvarchar(250) NOT NULL,
    ParentCategoryID smallint NULL,
    IsActive bit NOT NULL DEFAULT 1,
    CONSTRAINT PK_Category PRIMARY KEY CLUSTERED (CategoryID ASC),
    CONSTRAINT FK_Category_Category FOREIGN KEY (ParentCategoryID) REFERENCES Category(CategoryID)
);
GO

-- 3. Table: Tag
CREATE TABLE Tag (
    TagID int IDENTITY(1,1) NOT NULL,
    TagName nvarchar(50) NOT NULL,
    Note nvarchar(400) NULL,
    CONSTRAINT PK_Tag PRIMARY KEY CLUSTERED (TagID ASC)
);
GO

-- 4. Table: NewsArticle
CREATE TABLE NewsArticle (
    NewsArticleID nvarchar(20) NOT NULL,
    NewsTitle nvarchar(400) NOT NULL,
    Headline nvarchar(150) NOT NULL,
    CreatedDate datetime NULL DEFAULT GETDATE(),
    NewsContent nvarchar(max) NULL,
    NewsSource nvarchar(400) NULL,
    CategoryID smallint NULL,
    NewsStatus bit NOT NULL DEFAULT 1, -- 1: Active, 0: Inactive
    CreatedByID smallint NULL,
    UpdatedByID smallint NULL,
    ModifiedDate datetime NULL,
    CONSTRAINT PK_NewsArticle PRIMARY KEY CLUSTERED (NewsArticleID ASC),
    CONSTRAINT FK_NewsArticle_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    CONSTRAINT FK_NewsArticle_CreatedBy FOREIGN KEY (CreatedByID) REFERENCES SystemAccount(AccountID),
    CONSTRAINT FK_NewsArticle_UpdatedBy FOREIGN KEY (UpdatedByID) REFERENCES SystemAccount(AccountID)
);
GO

-- 5. Table: NewsTag
CREATE TABLE NewsTag (
    NewsArticleID nvarchar(20) NOT NULL,
    TagID int NOT NULL,
    CONSTRAINT PK_NewsTag PRIMARY KEY CLUSTERED (NewsArticleID ASC, TagID ASC),
    CONSTRAINT FK_NewsTag_NewsArticle FOREIGN KEY (NewsArticleID) REFERENCES NewsArticle(NewsArticleID) ON DELETE CASCADE,
    CONSTRAINT FK_NewsTag_Tag FOREIGN KEY (TagID) REFERENCES Tag(TagID) ON DELETE CASCADE
);
GO

-- SEED DATA
-- System Accounts (Staff: Role 1, Lecturer: Role 2)
SET IDENTITY_INSERT SystemAccount ON;
INSERT INTO SystemAccount (AccountID, AccountName, AccountEmail, AccountRole, AccountPassword) VALUES
(1, N'Staff Member One', N'staff1@funews.org', 1, N'123456'),
(2, N'Staff Member Two', N'staff2@funews.org', 1, N'123456'),
(3, N'Lecturer John Doe', N'lecturer1@funews.org', 2, N'123456'),
(4, N'Staff Member Three', N'staff3@funews.org', 1, N'123456');
SET IDENTITY_INSERT SystemAccount OFF;
GO

-- Categories
SET IDENTITY_INSERT Category ON;
INSERT INTO Category (CategoryID, CategoryName, CategoryDesciption, ParentCategoryID, IsActive) VALUES
(1, N'Academic News', N'All educational and academic announcements', NULL, 1),
(2, N'Examinations', N'Midterm and final exam schedules and guidelines', 1, 1),
(3, N'Campus Events', N'Cultural, technical, and sports events on campus', NULL, 1),
(4, N'Student Affairs', N'Clubs, scholarships and student services', NULL, 1),
(5, N'Archived Notices', N'Old notices no longer active', NULL, 0);
SET IDENTITY_INSERT Category OFF;
GO

-- Tags
SET IDENTITY_INSERT Tag ON;
INSERT INTO Tag (TagID, TagName, Note) VALUES
(1, N'#Education', N'General academic and educational content'),
(2, N'#FPTU', N'Campus life and news directly related to FPT University'),
(3, N'#Scholarship', N'Scholarship opportunities and student aids'),
(4, N'#Workshop', N'Technical and soft skill workshops'),
(5, N'#Notice', N'Official university announcements');
SET IDENTITY_INSERT Tag OFF;
GO

-- NewsArticles
INSERT INTO NewsArticle (NewsArticleID, NewsTitle, Headline, CreatedDate, NewsContent, NewsSource, CategoryID, NewsStatus, CreatedByID, UpdatedByID, ModifiedDate) VALUES
(N'NA0001', N'Fall Semester Examination Schedule Announced', N'Detailed timetable for Fall exam week is now published.', '2026-09-10 08:30:00', N'The Academic Department has published the complete final examination schedule for the Fall semester. Students are requested to check their individual portals for exact room assignments and timing.', N'Academic Office', 2, 1, 1, 1, '2026-09-11 10:00:00'),
(N'NA0002', N'FPT University TechFest 2026 Opening Ceremony', N'Join us this weekend for groundbreaking student tech showcases.', '2026-09-15 09:00:00', N'TechFest 2026 brings together hundreds of innovative student software and IoT projects, guest speakers from leading tech enterprises, and exciting coding hackathons.', N'PR Department', 3, 1, 1, NULL, NULL),
(N'NA0003', N'Merit Scholarships for Top Performers in Semester', N'Honoring outstanding students with attractive tuition scholarships.', '2026-09-20 14:00:00', N'Congratulations to the top 50 academic performers who have been awarded merit-based scholarships for the upcoming term. Detailed instructions for claiming the reward have been emailed to awardees.', N'Student Affairs', 4, 1, 2, NULL, NULL),
(N'NA0004', N'Upcoming Maintenance of Online Library Portal', N'Temporary maintenance window scheduled for midnight Saturday.', '2026-09-25 17:00:00', N'The university library database will undergo scheduled system upgrades on Saturday night from 00:00 to 04:00 AM. Please download necessary reference materials in advance.', N'IT Department', 1, 0, 2, NULL, NULL);
GO

-- NewsTag associations
INSERT INTO NewsTag (NewsArticleID, TagID) VALUES
(N'NA0001', 1),
(N'NA0001', 5),
(N'NA0002', 2),
(N'NA0002', 4),
(N'NA0003', 1),
(N'NA0003', 3),
(N'NA0004', 5);
GO
