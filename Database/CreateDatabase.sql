-- =============================================
-- Web Tabanlı Test Oluşturma Sistemi
-- Veritabanı Oluşturma Script'i
-- =============================================

-- Veritabanı Oluştur
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TestCreationDb')
BEGIN
    CREATE DATABASE TestCreationDb;
END
GO

USE TestCreationDb;
GO

-- =============================================
-- ASP.NET Identity Tabloları
-- =============================================

-- Kullanıcılar
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256),
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256),
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    ConcurrencyStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    Name NVARCHAR(100),
    Surname NVARCHAR(100)
);

-- Roller
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256),
    NormalizedName NVARCHAR(256),
    ConcurrencyStamp NVARCHAR(MAX)
);

-- Kullanıcı-Rol İlişkisi
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- Kullanıcı Claims
CREATE TABLE AspNetUserClaims (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Kullanıcı Logins
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(450) NOT NULL,
    ProviderKey NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX),
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Kullanıcı Tokens
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX),
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Rol Claims
CREATE TABLE AspNetRoleClaims (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- =============================================
-- Uygulama Tabloları
-- =============================================

-- Testler Tablosu
CREATE TABLE Tests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    DurationInMinutes INT DEFAULT 30,
    CreatedByTeacherId NVARCHAR(450),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CreatedByTeacherId) REFERENCES AspNetUsers(Id)
);

-- Sorular Tablosu
CREATE TABLE Questions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TestId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    Points INT DEFAULT 10,
    FOREIGN KEY (TestId) REFERENCES Tests(Id) ON DELETE CASCADE
);

-- Seçenekler Tablosu
CREATE TABLE Options (
    Id INT PRIMARY KEY IDENTITY(1,1),
    QuestionId INT NOT NULL,
    OptionText NVARCHAR(500) NOT NULL,
    IsCorrect BIT DEFAULT 0,
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE
);

-- Sınav Sonuçları Tablosu
CREATE TABLE TestResults (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId NVARCHAR(450) NOT NULL,
    TestId INT NOT NULL,
    Score INT,
    TakenAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (StudentId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (TestId) REFERENCES Tests(Id)
);

-- =============================================
-- İndeksler (Performans için)
-- =============================================

CREATE INDEX IX_AspNetUsers_Email ON AspNetUsers(NormalizedEmail);
CREATE INDEX IX_AspNetUsers_UserName ON AspNetUsers(NormalizedUserName);
CREATE INDEX IX_Tests_CreatedByTeacherId ON Tests(CreatedByTeacherId);
CREATE INDEX IX_Questions_TestId ON Questions(TestId);
CREATE INDEX IX_Options_QuestionId ON Options(QuestionId);
CREATE INDEX IX_TestResults_StudentId ON TestResults(StudentId);
CREATE INDEX IX_TestResults_TestId ON TestResults(TestId);

GO

PRINT 'Veritabanı başarıyla oluşturuldu!';
