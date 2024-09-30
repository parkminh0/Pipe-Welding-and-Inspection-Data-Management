DROP TABLE [Company] 
GO
CREATE TABLE [Company] 
(
 	 [CompKey]	 INT NOT NULL ,
 	 [CompName]	 NVARCHAR(60)  NULL ,
	 [Identify] NVARCHAR(255)  NULL ,
 	 [Descr]	 NVARCHAR(500)  NULL ,
	 [isDeleted] bit null,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [Company]
 ADD CONSTRAINT Company_PK PRIMARY KEY NONCLUSTERED ( [CompKey] )
GO
-- 0번코드 아그루코리아 하나는 고정으로 생성하되 거래처 설치시에는 포함되서는 안됨
insert into Company (CompKey, CompName, Identify, Descr, isDeleted, CreateID, CreateDtm)
	VALUES (0, N'아그루코리아', 'sxyQJBrBJkbWVrfQ+rhfRw==', '', 0, N'admin', GETDATE()); --AGRUKOREA132$
GO
-- 1번 이후의 다수의 거래처는 아그루본사에서는 생성가능
-- 거래처에서는 1번으로 하나만 거래처 생성해놓는 방식으로 가자
insert into Company (CompKey, CompName, Identify, Descr, isDeleted, CreateID, CreateDtm)
	VALUES (1, N'업체명 변경', 'P/260Ekktntyo2plNyVwFg==', '', 0, N'admin', GETDATE()); --LocalClient
GO

DROP TABLE [Project] 
GO
CREATE TABLE [Project] 
(
 	 [ProjectKey]	 INT NOT NULL ,
 	 [ProjectName]	 NVARCHAR(60)  NULL ,
 	 [CompKey]	 INT NOT NULL ,
	 isDeleted bit null,
 	 [Descr]	 NVARCHAR(500)  NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [Project]
 ADD CONSTRAINT Project_PK PRIMARY KEY NONCLUSTERED ( [ProjectKey] )
GO
insert into Project (ProjectKey, ProjectName, CompKey, isDeleted, Descr, CreateID, CreateDtm)
	VALUES (0, N'SWS 프로젝트', 0, 0, '', N'admin', GETDATE());
GO

DROP TABLE [TempWeldMaster] 
GO
CREATE TABLE [TempWeldMaster] 
(
 	 [MachineSerialNo]	 NVARCHAR(20)  NULL ,
 	 [WeldNo]	 NVARCHAR(30)  NULL ,
 	 [WeldingDate]	 DATETIME NULL ,
 	 [Status]	 INT NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
create INDEX ix01_TempWeldMaster on [TempWeldMaster] (CreateID)
GO


DROP TABLE [WeldMaster] 
GO
CREATE TABLE [WeldMaster]
(
 	 [WeldKey]	 INT IDENTITY(1,1) NOT NULL ,
	 -- 2023.11.08 추가
	 [ProjectKey] INT NULL,
 	 [MachineSerialNo]	 NVARCHAR(20)  NULL ,
 	 [MachineLine]	 NVARCHAR(20)  NULL ,
 	 [MachineType]	 NVARCHAR(20)  NULL ,
 	 [SoftwareVersion]	 NVARCHAR(30)  NULL ,
 	 [LastMaintDate]	 DATETIME NULL ,
 	 [NextMaintDate]	 DATETIME NULL ,
 	 [WeldProjectDescr]	 NVARCHAR(50)  NULL ,
 	 [AdditionalData]	 NVARCHAR(300)  NULL ,
 	 [InstallingCompany]	 NVARCHAR(60)  NULL ,
 	 [WelderCode]	 NVARCHAR(60)  NULL ,
 	 [WelderName]	 NVARCHAR(30)  NULL ,
 	 [WeldersCompany]	 NVARCHAR(60)  NULL ,
 	 [WelderCertExpireDate]	 DATETIME NULL ,
 	 [WeldNo]	 NVARCHAR(30)  NULL ,
 	 [DataSeqNo]	 NVARCHAR(10)  NULL ,
 	 [WeldingDate]	 DATETIME NULL ,
 	 [WeldingDateTime]	 DATETIME NULL ,
 	 [AmbientTemperature]	 FLOAT NULL ,
 	 [Status]	 NVARCHAR(100)  NULL ,
 	 [Weather]	 NVARCHAR(30)  NULL ,
 	 [Material]	 NVARCHAR(20)  NULL ,
 	 [Diameter]	 FLOAT NULL ,
 	 [Diameter2]	 FLOAT NULL ,
 	 [SDR]	 FLOAT NULL ,
 	 [WallThickness]	 FLOAT NULL ,
 	 [Angle]	 FLOAT NULL ,
 	 [FittingManufacturer]	 NVARCHAR(60)  NULL ,
 	 [Design]	 NVARCHAR(30)  NULL ,
 	 [OperatingMode]	 NVARCHAR(10)  NULL ,
 	 [Standard]	 NVARCHAR(10)  NULL ,
 	 [Mode]	 NVARCHAR(10)  NULL ,
 	 [FittingCode]	 NVARCHAR(30)  NULL ,
 	 [WeldingBeadLeft]	 FLOAT NULL ,
 	 [WeldingBeadRight]	 FLOAT NULL ,
 	 [WeldingBeadTotal]	 FLOAT NULL ,
 	 [WeldingBeadLeft2]	 FLOAT NULL ,
 	 [WeldingBeadRight2]	 FLOAT NULL ,
 	 [WeldingBeadTotal2]	 FLOAT NULL ,
 	 [WeldingBeadLeft3]	 FLOAT NULL ,
 	 [WeldingBeadRight3]	 FLOAT NULL ,
 	 [WeldingBeadTotal3]	 FLOAT NULL ,
 	 [Latitude]	 NVARCHAR(10)  NULL ,
 	 [Longitude]	 NVARCHAR(10)  NULL ,
 	 [DragForceNominalValue]	 FLOAT NULL ,
 	 [DragForceNominalUnit]	 NVARCHAR(10)  NULL ,
 	 [DragForceActualValue]	 FLOAT NULL ,
 	 [DragForceActualUnit]	 NVARCHAR(10)  NULL ,
 	 [MirrorTempNominalValue]	 FLOAT NULL ,
 	 [MirrorTempActualValue]	 FLOAT NULL ,
 	 [PreheatingTimeNominalValue]	 FLOAT NULL ,
 	 [PreheatingTimeActualValue]	 FLOAT NULL ,
 	 [BeadBuidupTimeNominalValue]	 FLOAT NULL ,
 	 [BeadBuidupTimeActualValue]	 FLOAT NULL ,
 	 [BeadBuildupForceNominalValue]	 FLOAT NULL ,
 	 [BeadBuildupForceNominalUnit]	 NVARCHAR(10)  NULL ,
 	 [BeadBuildupForceActualValue]	 FLOAT NULL ,
 	 [BeadBuildupForceActualUnit]	 NVARCHAR(10)  NULL ,
 	 [HeatingTimeNominaValue]	 FLOAT NULL ,
 	 [HeatingTimeActualValue]	 FLOAT NULL ,
 	 [HeatingForceNominalValue]	 FLOAT NULL ,
 	 [HeatingForceNominalUnit]	 NVARCHAR(10) NULL ,
 	 [HeatingForceActualValue]	 FLOAT NULL ,
 	 [HeatingForceActualUnit]	 NVARCHAR(10) NULL ,
 	 [ChangeOverTimeNominalValue]	 FLOAT NULL ,
 	 [ChangeOverTimeActualValue]	 FLOAT NULL ,
 	 [JoiningPressRampNominalValue]	 FLOAT NULL ,
 	 [JoiningPressRampActualValue]	 FLOAT NULL ,
 	 [JoiningForceNominalValue]	 FLOAT NULL ,
 	 [JoiningForceNominalUnit]	 NVARCHAR(10)  NULL ,
 	 [JoiningForceActualValue]	 FLOAT NULL ,
 	 [JoiningForceActualUnit]	 NVARCHAR(10)  NULL ,
 	 [CoolingTimeNominalValue]	 FLOAT NULL ,
 	 [CoolingTimeActualValue]	 FLOAT NULL ,
 	 [TwoLevelCoolingTimeNominalValue]	 FLOAT NULL ,
 	 [TwoLevelCoolingTimeActualValue]	 FLOAT NULL ,
 	 [TwoLevelCoolingForceNominalValue]	 FLOAT NULL ,
 	 [TwoLevelCoolingForceNominalUnit]	 NVARCHAR(10)  NULL ,
 	 [TwoLevelCoolingForceActualValue]	 FLOAT NULL ,
 	 [TwoLevelCoolingForceActualUnit]	 NVARCHAR(10)  NULL ,
 	 [WeldingDistance]	 FLOAT NULL ,
 	 [WeldingVoltageNominalValue]	 FLOAT NULL ,
 	 [WeldingVoltageActualValue]	 FLOAT NULL ,
 	 [ResistanceNominalValue]	 FLOAT NULL ,
 	 [ResistanceActualValue]	 FLOAT NULL ,
 	 [WorkNominalValue]	 FLOAT NULL ,
 	 [WorkActualValue]	 FLOAT NULL ,
 	 [TotalTimeNominalValue]	 FLOAT NULL ,
 	 [TotalTimeActualValue]	 FLOAT NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [WeldMaster]
 ADD CONSTRAINT WeldMaster_PK PRIMARY KEY NONCLUSTERED ( [WeldKey] )
GO
create INDEX ix01_WeldMaster on WeldMaster([ProjectKey])
go
create INDEX ix02_WeldMaster on WeldMaster([MachineSerialNo], [WeldNo], [WeldingDate])
go
create INDEX ix03_WeldMaster on WeldMaster ([WeldingDate], [CreateID])
GO
create INDEX ix04_WeldMaster on WeldMaster ([CreateDtm])
GO

--update STATISTICS WeldMaster
--UPDATE STATISTICS BeadMaster

DROP TABLE [TempBeadMaster] 
GO
CREATE TABLE [TempBeadMaster] 
(
 	 [BeadKey]	 INT NOT NULL ,
 	 [ProjectKey]	 INT NOT NULL ,
 	 [LineNo]	 INT NULL ,
 	 [SerialNo]	 NVARCHAR(20)  NULL ,
 	 [InspectionNo]	 INT  NULL ,
 	 [InspectionDate]	 DATETIME NULL ,
 	 [Material]	 NVARCHAR(30)  NULL ,
 	 [OD]	 FLOAT NULL ,
 	 [WallThickness]	 FLOAT NULL ,
 	 [SDR]	 NVARCHAR(10)  NULL ,
 	 [Rate]	 INT NULL ,
 	 [Status]	 INT NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
create INDEX ix01_TempBeadMaster on [TempBeadMaster] (CreateID)
GO

DROP TABLE [BeadMaster] 
GO
CREATE TABLE [BeadMaster] 
(
 	 [BeadKey]	 INT IDENTITY(1,1) NOT NULL ,
 	 [ProjectKey]	 INT NOT NULL ,
 	 [LineNo]	 INT NULL ,
 	 [SerialNo]	 NVARCHAR(20)  NULL ,
 	 [InspectionNo]	 INT NULL ,
 	 [InspectionDate]	 DATETIME NULL ,
 	 [InspectionTime]	 NVARCHAR(8)  NULL , 
 	 [InspectorID]	 NVARCHAR(20)  NULL ,
 	 [SoftwareVersion]	 NVARCHAR(30) NULL ,
 	 [Material]	 NVARCHAR(30)  NULL ,
 	 [OD]	 FLOAT NULL ,
 	 [WallThickness]	 FLOAT NULL ,
 	 [SDR]	 NVARCHAR(10)  NULL ,
 	 [ProdCode]	 NVARCHAR(20)  NULL ,
 	 [BatchNo]	 NVARCHAR(10)  NULL ,
 	 [IDNo]	 NVARCHAR(10)  NULL ,
 	 [ProdCode2]	 NVARCHAR(20)  NULL ,
 	 [BatchNo2]	 NVARCHAR(10)  NULL ,
 	 [IDNo2]	 NVARCHAR(10)  NULL ,
 	 [Manufacturer]	 NVARCHAR(60)  NULL ,
 	 [MachineSerialNo]	 NVARCHAR(20)  NULL ,
 	 [WeldNo]	 NVARCHAR(30)  NULL ,
 	 [WeldingDate]	 DATETIME NULL ,
 	 [WeldedTime]	 NVARCHAR(8)  NULL , 
 	 [Welder]	 NVARCHAR(60)  NULL ,
 	 [Project]	 NVARCHAR(60)  NULL ,
 	 [Floor]	 NVARCHAR(20)  NULL ,
 	 [Column]	 NVARCHAR(20)  NULL ,
 	 [Location]	 NVARCHAR(60)  NULL ,
 	 [LineSeries]	 NVARCHAR(20)  NULL ,
 	 [Medium]	 NVARCHAR(20)  NULL ,
 	 [OtherInfo1]	 NVARCHAR(1000)  NULL ,
 	 [AmbTemp]	 FLOAT NULL ,
 	 [PipeTemp]	 FLOAT NULL ,
 	 [OperatingPressure]	 FLOAT NULL ,
 	 [Constructor]	 NVARCHAR(60)  NULL ,
 	 [OtherInfo2]	 NVARCHAR(1000)  NULL ,
 	 [TestPoints]	 INT NULL ,
 	 [CompletionRate]	 FLOAT NULL ,
 	 [PassRate]	 FLOAT NULL ,
 	 [K_C]	 FLOAT NULL ,
 	 [KMin]	 FLOAT NULL ,
 	 [KMax]	 FLOAT NULL ,
 	 [KAvg]	 FLOAT NULL ,
 	 [isKPass]	 NVARCHAR(5)  NULL ,
 	 [BMin_C]	 FLOAT NULL ,
 	 [BMax_C]	 FLOAT NULL ,
 	 [BMin]	 FLOAT NULL ,
 	 [BMax]	 FLOAT NULL ,
 	 [BAvg]	 FLOAT NULL ,
 	 [isBPass]	 NVARCHAR(5)  NULL ,
 	 [BRatio_C]	 FLOAT NULL ,
 	 [B1Min]	 FLOAT NULL ,
 	 [B2Min]	 FLOAT NULL ,
 	 [BRatioMin]	 FLOAT NULL ,
 	 [B1Max]	 FLOAT NULL ,
 	 [B2Max]	 FLOAT NULL ,
 	 [BRatioMax]	 FLOAT NULL ,
 	 [B1Avg]	 FLOAT NULL ,
 	 [B2Avg]	 FLOAT NULL ,
 	 [BRatioAvg]	 FLOAT NULL ,
 	 [isBRatioPass]	 NVARCHAR(5)  NULL ,
 	 [MissAlign_C]	 FLOAT NULL ,
 	 [MissAlignMin]	 FLOAT NULL ,
 	 [MissAlignMax]	 FLOAT NULL ,
 	 [MissAlignAvg]	 FLOAT NULL ,
 	 [isMissAlignPass]	 NVARCHAR(5)  NULL ,
 	 [Notch_C]	 FLOAT NULL ,
 	 [NotchMin]	 FLOAT NULL ,
 	 [NotchMax]	 FLOAT NULL ,
 	 [NotchAvg]	 FLOAT NULL ,
 	 [isNotchPass]	 NVARCHAR(5)  NULL ,
 	 [isNotchEyes]	 NVARCHAR(5)  NULL ,
 	 [NotchNote]	 NVARCHAR(500)  NULL ,
 	 [isAngularDevEyes]	 NVARCHAR(5)  NULL ,
 	 [AngularDevNote]	 NVARCHAR(500)  NULL ,
 	 [isCrackEyes]	 NVARCHAR(5)  NULL ,
 	 [CrackNote]	 NVARCHAR(500)  NULL ,
 	 [isVoidEyes]	 NVARCHAR(5)  NULL ,
 	 [VoidNote]	 NVARCHAR(500)  NULL ,
 	 [isSupportPadEyes]	 NVARCHAR(5)  NULL ,
 	 [SupportPadNote]	 NVARCHAR(500)  NULL ,
 	 [isInterruptionEyes]	 NVARCHAR(5)  NULL ,
 	 [InterruptionNote]	 NVARCHAR(500)  NULL ,
 	 [isOverheatingEyes]	 NVARCHAR(5)  NULL ,
 	 [OverheatingNote]	 NVARCHAR(500)  NULL ,
 	 [DVS]	 NVARCHAR(10)  NULL ,
 	 [VIScore]	 FLOAT NULL ,
 	 [VIGrade]	 NVARCHAR(5)  NULL ,
 	 [Code]	 NVARCHAR(30)  NULL ,
 	 [Result]	 NVARCHAR(20)  NULL ,
 	 [EvaluationScore]	 FLOAT NULL ,
 	 [Grade]	 NVARCHAR(5)  NULL ,
 	 [Descr]	 NVARCHAR(100)  NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [BeadMaster]
 ADD CONSTRAINT BeadMaster_PK PRIMARY KEY NONCLUSTERED ( [BeadKey] )
GO
create INDEX ix01_BeadMaster on BeadMaster (ProjectKey)
GO
create INDEX ix02_BeadMaster on BeadMaster (InspectionDate, CreateID)
GO
create INDEX ix03_BeadMaster on BeadMaster (SerialNo, InspectionDate, InspectionNo)
GO
create INDEX ix04_BeadMaster on BeadMaster (MachineSerialNo, WeldNo, WeldingDate)
GO
create INDEX ix05_BeadMaster on BeadMaster (CreateDtm)
GO


DROP TABLE [BeadDetail] 
GO
CREATE TABLE [BeadDetail] 
(
 	 [BeadDetailKey]	 INT NOT NULL ,
 	 [BeadKey]	 INT NOT NULL ,
 	 [LineNo]	 INT NULL ,
 	 [SerialNo]	 NVARCHAR(20)  NULL ,
 	 [InspectionNo]	 INT NULL ,
 	 [InspectionDate]	 DATETIME NULL ,
 	 [InspectionTime]	 NVARCHAR(8)  NULL , 
 	 [Position]	 INT NULL ,
 	 [isPass]	 NVARCHAR(10)  NULL ,
 	 [KValue]	 FLOAT NULL ,
 	 [isKPass]	 NVARCHAR(10)  NULL ,
 	 [BeadWidthTotal]	 FLOAT NULL ,
 	 [isBWidthPass]	 NVARCHAR(10)  NULL ,
 	 [B1Width]	 FLOAT NULL ,
 	 [B2Width]	 FLOAT NULL ,
 	 [BRatio]	 FLOAT NULL ,
 	 [isBRatioPass]	 NVARCHAR(10)  NULL ,
 	 [MissAlignValue]	 FLOAT NULL ,
 	 [isMissAlignPass]	 NVARCHAR(10)  NULL ,
 	 [NotchValue]	 FLOAT NULL ,
 	 [isNotchPass]	 NVARCHAR(10)  NULL ,
 	 [ContactAngle]	 FLOAT NULL ,
 	 [AttrFloat1]	 FLOAT NULL ,
 	 [AttrFloat2]	 FLOAT NULL ,
 	 [AttrFloat3]	 FLOAT NULL ,
 	 [AttrString1]	 NVARCHAR(10)  NULL ,
 	 [AttrString2]	 NVARCHAR(20)  NULL ,
 	 [AttrString3]	 NVARCHAR(30)  NULL ,
 	 [Descr]	 NVARCHAR(100)  NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [BeadDetail]
 ADD CONSTRAINT BeadDetail_PK PRIMARY KEY NONCLUSTERED ( [BeadDetailKey] )
GO
CREATE INDEX ix01_BeadDetail on BeadDetail ([BeadKey])
GO

DROP TABLE [BeadImageInfo] 
GO
CREATE TABLE [BeadImageInfo] 
(
 	 [BeadDetailKey]	 INT NOT NULL ,
 	 [RawImage]	 VARBINARY(max)  NULL ,
 	 [OedImage]	 VARBINARY(max)  NULL ,
 	 [LineInfo]	 VARBINARY(max)  NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
ALTER TABLE [BeadImageInfo]
 ADD CONSTRAINT BeadImageInfo_PK PRIMARY KEY NONCLUSTERED ( [BeadDetailKey] )
GO

DROP TABLE [UserInfo] 
GO
CREATE TABLE [UserInfo] 
(
 	 [CompKey]	 INT NOT NULL ,
 	 [UserID]	 NVARCHAR(20)  NOT NULL ,
 	 [UserName]	 NVARCHAR(60)  NULL ,
 	 [Passwd]	 NVARCHAR(256)  NULL ,
 	 [AuthLevel]	 INT NULL ,
	 [ProjectKey]	 INT NULL ,               -- 2023.11.07 박민호 추가
 	 -- [Department]	 NVARCHAR(30)  NULL ,    2023.11.07 박민호 삭제
	 [ParentAdminID]	 NVARCHAR(60)  NULL , -- 2023.11.07 박민호 추가
 	 [ParentManagerID]	 NVARCHAR(60)  NULL , -- 2023.11.07 박민호 추가
 	 [PhoneNo]	 NVARCHAR(30)  NULL ,
 	 [Descr]	 NVARCHAR(100)  NULL ,
 	 [isDeleted] BIT NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL ,
 	 [CreateDtm]	 DATETIME NULL ,
 	 [ExpireDtm]	 DATETIME NULL            -- 2023.11.07 박민호 추가
	 [isPWUpdated] BIT NULL                   -- 2023.11.07 박민호 추가
)
GO
ALTER TABLE [UserInfo]
 ADD CONSTRAINT UserInfo_PK PRIMARY KEY NONCLUSTERED ( [CompKey], [UserID] )
GO

INSERT INTO [dbo].[UserInfo]
           ([CompKey]
           ,[UserID]
           ,[UserName]
           ,[Passwd]
           ,[AuthLevel]
           ,[ProjectKey]
		   ,[ParentAdminID]
		   ,[ParentManagerID]
           ,[PhoneNo]
           ,[Descr]
		   ,isDeleted
           ,[CreateID]
           ,[CreateDtm]
		   ,[ExpireDtm]
		   ,[isPWUpdated])
     VALUES
           (0
           ,'admin'
           ,'관리자'
           ,'BgbhAogexqWu7+4uAZA/Fg==' -- 1234
           ,0
           ,''
           ,''
           ,''
		   ,''
		   ,''
		   ,0
           ,'sys'
           ,GETDATE()
		   ,'2099-12-31'
		   ,1)
GO

DROP TABLE [CodeInfomation]
GO
CREATE TABLE [CodeInfomation] 
(
 	 [Category]	 NVARCHAR(20)  NOT NULL,
 	 [DetailCode]	 NVARCHAR(30)  NOT NULL,
 	 [CodeDescr]	 NVARCHAR(100)  NULL,
 	 [CodeType]	 INT NULL , --기타정보 담기
 	 [OrderNo]	 INT NULL ,
 	 [CreateDtm]	 DATETIME NULL 
)
GO
-- 2023.11.03 권한 수정
ALTER TABLE [CodeInfomation]
 ADD CONSTRAINT CodeInfomation_PK PRIMARY KEY NONCLUSTERED ( [Category], [DetailCode] )
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('AuthLevel', '0', 'Super Admin', 0, 1, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('AuthLevel', '1', 'Admin', 0, 2, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('AuthLevel', '2', 'Manager', 0, 3, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('AuthLevel', '3', 'Operator', 0, 4, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '20', '', 0, 1, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '25', '', 0, 2, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '32', '', 0, 3, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '40', '', 0, 4, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '50', '', 0, 5, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '63', '', 0, 6, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '75', '', 0, 7, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '90', '', 0, 8, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '110', '', 0, 9, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '125', '', 0, 10, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '140', '', 0, 11, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '160', '', 0, 12, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '180', '', 0, 13, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '200', '', 0, 14, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '225', '', 0, 15, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '250', '', 0, 16, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '280', '', 0, 17, GETDATE())
GO
INSERT INTO [CodeInfomation] ([Category],[DetailCode],[CodeDescr],[CodeType],[OrderNo],[CreateDtm]) 
  VALUES('Diameter', '315', '', 0, 18, GETDATE())
GO


DROP TABLE [UserAuth]
GO

CREATE TABLE [UserAuth] 
(
 	 [CompKey]	 INT NOT NULL ,
 	 [UserID]	 NVARCHAR(20)  NOT NULL ,
 	 [UploadInspectData]	 BIT NULL ,
 	 [UploadWeldData]	 BIT NULL ,
 	 [ShowInspectUploadRecord]	 BIT NULL ,
 	 [ShowWeldUploadRecord]	 BIT NULL ,
 	 [FormBeadMasterManage]	 BIT NULL ,
 	 [FormWeldMasterManage]	 BIT NULL ,
 	 [FormBeadDetailList]	 BIT NULL ,
 	 [FormBeadAndWeldList]	 BIT NULL ,
 	 [FormGeneralChart1]	 BIT NULL ,
 	 [FormPivotResult]	 BIT NULL ,
 	 [FormDashboard]	 BIT NULL ,
 	 [FormDashboard2]	 BIT NULL ,
 	 [FormDashboard3]	 BIT NULL ,
 	 [SaveExcel]	 BIT NULL ,
 	 [DeleteInspectData]	 BIT NULL ,
 	 [DeleteWeldData]	 BIT NULL 
)
GO


ALTER TABLE [UserAuth]
 ADD CONSTRAINT UserAuth_PK PRIMARY KEY NONCLUSTERED ( [CompKey],[UserID] )
GO
INSERT INTO [UserAuth] 
  VALUES(0, 'admin', 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
GO


DROP TABLE [KeyNumbering]
GO
CREATE TABLE [KeyNumbering](
	[Category] [nvarchar](30) NOT NULL,
	[LastKeyNo] [int] NULL,
	[Descr] [nvarchar](100) NULL,
)
GO
ALTER TABLE [KeyNumbering]
 ADD CONSTRAINT KeyNumbering_PK PRIMARY KEY NONCLUSTERED ( [Category] )
GO

-- Period Day
--DROP TABLE [PeriodDay2]
CREATE TABLE [PeriodDay](
	[BaseYmd] DATETIME NOT NULL,
	[BaseYmdNm] [nvarchar](20) NULL,
	[Months] [nvarchar](2) NULL,
	[YearMonth] [nvarchar](7) NULL,
	[yearMonthNm] [nvarchar](20) NULL,
	[YearQuarter] [nvarchar](7) NULL,
	[YearQuarterNm] [nvarchar](20) NULL,
	[YearHalf] [nvarchar](7) NULL,
	[YearHalfNm] [nvarchar](20) NULL,
	[Year] [nvarchar](4) NULL,
	[YearNm] [nvarchar](20) NULL,
	[WeekDay] [int] NULL,
	[WeekDayNm] [nvarchar](10) NULL,
	[HolyType] [nvarchar](10) NULL,
	[HolyTypeNm] [nvarchar](20) NULL,
	[DayCount] [int] NULL
)
GO
ALTER TABLE [PeriodDay]
 ADD CONSTRAINT PeriodDay_PK PRIMARY KEY NONCLUSTERED ( [BaseYmd] )
GO
create INDEX ix01_PeriodDay on PeriodDay (Year)
GO

-- 2023.10.25 
DROP TABLE [ChartTable]
GO

CREATE TABLE [ChartTable] 
(
 	 [ChartKey]	 INT NULL ,
 	 [ChartName]	 NVARCHAR(100)  NULL ,
 	 [DataDate]	 DATETIME NULL ,
 	 [ProjectKey]	 INT NULL ,
 	 [ProjectName]	 NVARCHAR(100)  NULL ,
 	 [CompKey]	 INT NULL ,
 	 [CompName]	 NVARCHAR(100)  NULL ,
 	 [ItemName]	 NVARCHAR(100)  NULL ,
 	 [ItemValue]	 INT NULL ,
 	 [CreateID]	 NVARCHAR(20)  NULL
)
GO

--drop index ix01_ChartTable on ChartTable
--go
create clustered index ix01_ChartTable on ChartTable (ChartKey, DataDate)
go
--create index ix03_ChartTable on ChartTable (ProjectKey)
--go
--create index ix04_ChartTable on ChartTable (CompKey)
--go


DROP TABLE [ChartTime]
GO

CREATE TABLE [ChartTime] 
(
 	 [DataLoadTime]	 DATETIME NULL 
)
GO
---------------------------------
CREATE FUNCTION [dbo].[lpad]
 (
  @mstr AS varchar(8000),
  @nofchars AS int,
  @fillchar AS varchar(8000)=' '
 )
RETURNS varchar(200)
AS
BEGIN
 RETURN
  CASE
   WHEN LEN(@mstr) >= @nofchars THEN SUBSTRING(@mstr,1,@nofchars)
   ELSE
    SUBSTRING(REPLICATE(@fillchar,@nofchars), 1, @nofchars-LEN(@mstr)) + @mstr
  END
END
GO
---------------------------------

DROP VIEW ViewAllMasterList
GO
CREATE VIEW ViewAllMasterList as 
select bm.BeadKey
	  ,bm.ProjectKey
	  ,p.ProjectName
	  ,bm.[LineNo]
	  ,bm.SerialNo
	  ,bm.InspectionNo
	  ,convert(VARCHAR(10), bm.InspectionDate, 120) InspectionDate
	  ,bm.InspectionTime
	  ,bm.InspectorID
	  ,bm.SoftwareVersion
	  ,bm.Material
	  ,bm.OD
	  ,bm.WallThickness
	  ,bm.SDR
	  ,bm.ProdCode
	  ,bm.BatchNo
	  ,bm.IDNo
	  ,bm.ProdCode2
	  ,bm.BatchNo2
	  ,bm.IDNo2
	  ,bm.Manufacturer
	  ,bm.MachineSerialNo
	  ,bm.WeldNo
	  ,bm.WeldingDate
	  ,bm.WeldedTime
	  ,bm.Welder
	  ,bm.Project
	  ,bm.Floor
	  ,bm.[Column]
	  ,bm.Location
	  ,bm.LineSeries
	  ,bm.Medium
	  ,bm.OtherInfo1
	  ,bm.AmbTemp
	  ,bm.PipeTemp
	  ,bm.OperatingPressure
	  ,bm.Constructor
	  ,bm.OtherInfo2
	  ,bm.TestPoints
	  ,bm.CompletionRate
	  ,bm.PassRate
	  ,bm.K_C
	  ,bm.KMin
	  ,bm.KMax
	  ,bm.KAvg
	  ,bm.isKPass
	  ,bm.BMin_C
	  ,bm.BMax_C
	  ,bm.BMin
	  ,bm.BMax
	  ,bm.BAvg
	  ,bm.isBPass
	  ,bm.BRatio_C
	  ,bm.B1Min
	  ,bm.B2Min
	  ,bm.BRatioMin
	  ,bm.B1Max
	  ,bm.B2Max
	  ,bm.BRatioMax
	  ,bm.B1Avg
	  ,bm.B2Avg
	  ,bm.BRatioAvg
	  ,bm.isBRatioPass
	  ,bm.MissAlign_C
	  ,bm.MissAlignMin
	  ,bm.MissAlignMax
	  ,bm.MissAlignAvg
	  ,bm.isMissAlignPass
	  ,bm.Notch_C
	  ,bm.NotchMin
	  ,bm.NotchMax
	  ,bm.NotchAvg
	  ,bm.isNotchPass
	  ,bm.isNotchEyes
	  ,bm.NotchNote
	  ,bm.isAngularDevEyes
	  ,bm.AngularDevNote
	  ,bm.isCrackEyes
	  ,bm.CrackNote
	  ,bm.isVoidEyes
	  ,bm.VoidNote
	  ,bm.isSupportPadEyes
	  ,bm.SupportPadNote
	  ,bm.isInterruptionEyes
	  ,bm.InterruptionNote
	  ,bm.isOverheatingEyes
	  ,bm.OverheatingNote
	  ,bm.DVS
	  ,bm.VIScore
	  ,bm.VIGrade
	  ,bm.Code
	  ,bm.Result
	  ,bm.EvaluationScore
	  ,bm.Grade
	  ,bm.Descr
	  ,bm.CreateID InspectCreateID
	  ,wm.WeldKey
	  --,wm.MachineSerialNo
	  ,wm.ProjectKey wProjectKey
	  ,wm.MachineLine
	  ,wm.MachineType
	  ,wm.SoftwareVersion as WeldSoftwareVersion
	  ,wm.LastMaintDate
	  ,wm.NextMaintDate
	  ,wm.WeldProjectDescr
	  ,wm.AdditionalData
	  ,wm.InstallingCompany
	  ,wm.WelderCode
	  ,wm.WelderName
	  ,wm.WeldersCompany
	  ,wm.WelderCertExpireDate
	  --,wm.WeldNo
	  ,wm.DataSeqNo
	  --,wm.WeldingDate
	  ,wm.WeldingDateTime
	  ,wm.AmbientTemperature
	  ,wm.Status
	  ,wm.Weather
	  ,wm.Material as WeldMaterial
	  ,wm.Diameter
	  ,wm.Diameter2
	  ,wm.SDR as WeldSDR
	  ,wm.WallThickness as WeldWallThickness
	  ,wm.Angle
	  ,wm.FittingManufacturer
	  ,wm.Design
	  ,wm.OperatingMode
	  ,wm.Standard
	  ,wm.Mode
	  ,wm.FittingCode
	  ,wm.WeldingBeadLeft
	  ,wm.WeldingBeadRight
	  ,wm.WeldingBeadTotal
	  ,wm.WeldingBeadLeft2
	  ,wm.WeldingBeadRight2
	  ,wm.WeldingBeadTotal2
	  ,wm.WeldingBeadLeft3
	  ,wm.WeldingBeadRight3
	  ,wm.WeldingBeadTotal3
	  ,wm.Latitude
	  ,wm.Longitude
	  ,wm.DragForceNominalValue
	  ,wm.DragForceNominalUnit
	  ,wm.DragForceActualValue
	  ,wm.DragForceActualUnit
	  ,wm.MirrorTempNominalValue
	  ,wm.MirrorTempActualValue
	  ,wm.PreheatingTimeNominalValue
	  ,wm.PreheatingTimeActualValue
	  ,wm.BeadBuidupTimeNominalValue
	  ,wm.BeadBuidupTimeActualValue
	  ,wm.BeadBuildupForceNominalValue
	  ,wm.BeadBuildupForceNominalUnit
	  ,wm.BeadBuildupForceActualValue
	  ,wm.BeadBuildupForceActualUnit
	  ,wm.HeatingTimeNominaValue
	  ,wm.HeatingTimeActualValue
	  ,wm.HeatingForceNominalValue
	  ,wm.HeatingForceNominalUnit
	  ,wm.HeatingForceActualValue
	  ,wm.HeatingForceActualUnit
	  ,wm.ChangeOverTimeNominalValue
	  ,wm.ChangeOverTimeActualValue
	  ,wm.JoiningPressRampNominalValue
	  ,wm.JoiningPressRampActualValue
	  ,wm.JoiningForceNominalValue
	  ,wm.JoiningForceNominalUnit
	  ,wm.JoiningForceActualValue
	  ,wm.JoiningForceActualUnit
	  ,wm.CoolingTimeNominalValue
	  ,wm.CoolingTimeActualValue
	  ,wm.TwoLevelCoolingTimeNominalValue
	  ,wm.TwoLevelCoolingTimeActualValue
	  ,wm.TwoLevelCoolingForceNominalValue
	  ,wm.TwoLevelCoolingForceNominalUnit
	  ,wm.TwoLevelCoolingForceActualValue
	  ,wm.TwoLevelCoolingForceActualUnit
	  ,wm.WeldingDistance
	  ,wm.WeldingVoltageNominalValue
	  ,wm.WeldingVoltageActualValue
	  ,wm.ResistanceNominalValue
	  ,wm.ResistanceActualValue
	  ,wm.WorkNominalValue
	  ,wm.WorkActualValue
	  ,wm.TotalTimeNominalValue
	  ,wm.TotalTimeActualValue
	  ,wm.CreateID WeldCreateID
  FROM BeadMaster bm
  FULL OUTER join WeldMaster wm
		on bm.MachineSerialNo = wm.MachineSerialNo and bm.WeldNo = wm.WeldNo and bm.WeldingDate = wm.WeldingDate
GO

/*
DROP view ViewBeadMaster
GO
create view ViewBeadMaster AS
SELECT [BeadKey]
      ,c.CompKey
	  ,c.CompName
      ,bm.[ProjectKey]
	  ,p.ProjectName
      ,[LineNo]
      ,[SerialNo]
      ,[InspectionNo]
      ,[InspectionDate]
      ,[InspectionTime]
      ,[InspectorID]
      ,[SoftwareVersion]
      ,[Material]
      ,[OD]
      ,[WallThickness]
      ,[SDR]
      ,[ProdCode]
      ,[BatchNo]
      ,[IDNo]
      ,[ProdCode2]
      ,[BatchNo2]
      ,[IDNo2]
      ,[Manufacturer]
      ,[MachineSerialNo]
      ,[WeldNo]
      ,[WeldingDate]
      ,[WeldedTime]
      ,[Welder]
      ,[Project]
      ,[Floor]
      ,[Column]
      ,[Location]
      ,[LineSeries]
      ,[Medium]
      ,[OtherInfo1]
      ,[AmbTemp]
      ,[PipeTemp]
      ,[OperatingPressure]
      ,[Constructor]
      ,[OtherInfo2]
      ,[TestPoints]
      ,[CompletionRate]
      ,[PassRate]
      ,[K_C]
      ,[KMin]
      ,[KMax]
      ,[KAvg]
      ,[isKPass]
      ,[BMin_C]
      ,[BMax_C]
      ,[BMin]
      ,[BMax]
      ,[BAvg]
      ,[isBPass]
      ,[BRatio_C]
      ,[B1Min]
      ,[B2Min]
      ,[BRatioMin]
      ,[B1Max]
      ,[B2Max]
      ,[BRatioMax]
      ,[B1Avg]
      ,[B2Avg]
      ,[BRatioAvg]
      ,[isBRatioPass]
      ,[MissAlign_C]
      ,[MissAlignMin]
      ,[MissAlignMax]
      ,[MissAlignAvg]
      ,[isMissAlignPass]
      ,[Notch_C]
      ,[NotchMin]
      ,[NotchMax]
      ,[NotchAvg]
      ,[isNotchPass]
      ,[isNotchEyes]
      ,[NotchNote]
      ,[isAngularDevEyes]
      ,[AngularDevNote]
      ,[isCrackEyes]
      ,[CrackNote]
      ,[isVoidEyes]
      ,[VoidNote]
      ,[isSupportPadEyes]
      ,[SupportPadNote]
      ,[isInterruptionEyes]
      ,[InterruptionNote]
      ,[isOverheatingEyes]
      ,[OverheatingNote]
      ,[DVS]
      ,[VIScore]
      ,[VIGrade]
      ,[Code]
      ,[Result]
      ,[EvaluationScore]
      ,[Grade]
  FROM [dbo].[BeadMaster] bm
  left join Project p
    on bm.ProjectKey = p.ProjectKey
  left join Company c
    on p.CompKey = c.CompKey
 where 1 = 1
GO
*/

DROP VIEW ViewBeadMasterForSummary
GO
CREATE VIEW ViewBeadMasterForSummary AS
select p.CompKey, bm.ProjectKey, bm.BeadKey, pd.YearMonth, pd.Year, bm.SerialNo, bm.InspectionDate, --CONVERT(NVARCHAR(10), bm.InspectionDate, 120) InspectionDate, 
       bm.InspectionNo, bm.InspectorID, bm.CreateID InspectCreateID, wm.CreateID WeldCreateID, bm.Material,'OD'+ dbo.lpad(CAST(bm.OD as VARCHAR),3,'0') AS OD, bm.SDR, 
       UPPER([isKPass]) [K-Value], UPPER([isBPass]) [Bead Width], UPPER([isBRatioPass]) [Uneven Bead], 
	   UPPER([isMissAlignPass]) [Mis-Al], UPPER([isNotchPass]) [Notch], UPPER(bm.isNotchEyes) [NotchEye],
       UPPER([isAngularDevEyes]) [Ang-Dev], UPPER([isCrackEyes]) [Crack], UPPER([isVoidEyes]) [Void], 
	   UPPER([isSupportPadEyes]) [Support], UPPER([isInterruptionEyes]) [Interrupt], UPPER([isOverheatingEyes]) [Overheat], 
	   UPPER([DVS]) DVS, UPPER([VIGrade]) VIGrade, 1 beadCount,
	   CASE WHEN bm.OtherInfo2 IS NULL OR bm.OtherInfo2 = '' THEN CAST('TRUE' AS nvarchar(5)) ELSE CAST('FALSE' AS nvarchar(5)) END OtherInfo2
  from BeadMaster bm
  join PeriodDay pd
    on pd.BaseYmd = bm.InspectionDate
  LEFT JOIN Project p
    ON p.ProjectKey = bm.ProjectKey
  LEFT JOIN (select MachineSerialNo, WeldNo, WeldingDate, CreateID, min(WeldKey) WeldKey from WeldMaster group by MachineSerialNo, WeldNo, WeldingDate, CreateID) wm
    ON bm.MachineSerialNo = wm.MachineSerialNo and bm.WeldNo = wm.WeldNo and bm.WeldingDate = wm.WeldingDate
 where 1 = 1
GO


DROP VIEW ViewWeldMasterForSummary
GO
CREATE VIEW ViewWeldMasterForSummary AS
select w.Material, w.MachineSerialNo, w.MachineLine, w.Status, w.Diameter, w.SDR, w.WallThickness, w.WeldingDate, w.WelderName, w.WelderCode, w.WeldProjectDescr, p.Year, p.Months
  from WeldMaster w
  join PeriodDay p
    on p.BaseYmd = w.WeldingDate
GO

-- 기존값 정보만 별도로 담아두자
DROP view ViewSettings
GO
create view ViewSettings as
select 30 as LatestDays
GO

-- 1. Material별 융착데이터 (Total Welding Points)
--DROP view ViewChart1TotalWelding
--GO
--create view ViewChart1TotalWelding AS
--select x.Material, sum(x.ItemCount) ItemCount
--  from (
--		select case when rn > 9 then 'Etc' ELSE x.Material end Material, x.ItemCount
--		  from (
--				select case when x.Material = '' then 'Empty' else x.Material end Material, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
--				  from (
--					select Material, count(*) ItemCount
--					  from ViewWeldMasterForSummary wm
--					 group by Material
--				 ) x
--			) x
--	) x
-- group by x.Material
--GO

-- 2. Material별 검수데이터 (Total Inspection Points)
--drop view ViewChart2TotalInspection 
--GO
--create view ViewChart2TotalInspection AS
--select Material, count(*) ItemCount
--  from ViewBeadMasterForSummary
-- group by Material
--GO

--drop view ViewChart3MachineSeries
--go
--create view ViewChart3MachineSeries AS
--select x.MachineLine, sum(x.ItemCount) ItemCount
--  from (
--		select case when rn > 9 then 'Etc' ELSE x.MachineLine end MachineLine, x.ItemCount
--		  from (
--				select case when x.MachineLine = '' then 'Empty' else x.MachineLine end MachineLine, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
--				  from (
--					select MachineLine, count(*) ItemCount
--					  from ViewWeldMasterForSummary wm
--					 group by MachineLine
--				 ) x
--			) x
--	) x
-- group by x.MachineLine
--GO


-- 4-1. Welding Status (Status)
--drop view ViewChart4WeldingStatus
--GO
--create view ViewChart4WeldingStatus AS
--select x.Status, sum(x.ItemCount) ItemCount
--  from (
--		select case when rn > 9 then 'Etc' ELSE x.Status end Status, x.ItemCount
--		  from (
--				select case when x.Status = '' then 'Empty' else x.Status end Status, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
--				  from (
--					select Status, count(*) ItemCount
--					  from ViewWeldMasterForSummary wm
--					 where (wm.Status not LIKE '%ERROR%' AND wm.Status not LIKE '0%')
--					 group by Status
--				 ) x
--			) x
--	) x
-- group by x.Status
--GO


-- 4-2. Bead Status (DVS)
--drop view ViewChart5InspectionStatus
--GO
--create view ViewChart5InspectionStatus AS
--select case when DVS = 'TRUE' THEN 'Weld OK' else 'Weld NOK' end DVS, count(*) ItemCount
--from ViewBeadMasterForSummary
--group by DVS
--go


-- 7. Last Update (최근30일)
--drop view ViewTotalPoints
--GO
--create view ViewTotalPoints AS
--select x.WeldingPoints, y.BeadPoints, x.LatestDays
--  from (
--		select count(*) WeldingPoints, isnull(max(vs.LatestDays), 30) LatestDays
--		  from ViewWeldMasterForSummary w
--		  cross join ViewSettings vs
--		 where w.WeldingDate BETWEEN CONVERT(NVARCHAR(10), DATEADD(DAY, isnull(vs.LatestDays, 30) * -1, GETDATE()), 120) AND CONVERT(NVARCHAR(10), GETDATE(), 120)
--	) x
--cross join (
--		select count(*) BeadPoints, isnull(max(vs.LatestDays), 30) LatestDays
--		  from ViewBeadMasterForSummary b
--		  cross join ViewSettings vs
--		 where InspectionDate BETWEEN CONVERT(NVARCHAR(10), DATEADD(DAY, isnull(vs.LatestDays, 30) * -1, GETDATE()), 120) AND CONVERT(NVARCHAR(10), GETDATE(), 120)
--	) y
--GO

--drop view ViewWeldPointsPerOD
--go
--create view ViewWeldPointsPerOD AS
--select Diameter, x.ItemCount, rn, 'OD'+ dbo.lpad(CAST(Diameter as VARCHAR),4,'0') AS OD
--  from (
--	select x.Diameter, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
--	  from (
--			select FLOOR(Diameter) Diameter, count(*) ItemCount
--			  from (select isnull(Diameter, 0) Diameter from ViewWeldMasterForSummary) x
--			 group by FLOOR(Diameter)
--		 ) x
--	) x
--go



