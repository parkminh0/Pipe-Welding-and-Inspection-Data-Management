
-- 28개 
-- 1. Material별 융착데이터 (Total Welding Points)
DROP view ViewChart1TotalWelding
GO
create view ViewChart1TotalWelding AS
select x.Material, sum(x.ItemCount) ItemCount
  from (
		select case when rn > 9 then 'Etc' ELSE x.Material end Material, x.ItemCount
		  from (
				select case when x.Material = '' then 'Empty' else x.Material end Material, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				  from (
					select Material, count(*) ItemCount
					  from ViewWeldMasterForSummary wm
					 group by Material
				 ) x
			) x
	) x
 group by x.Material
GO

-- 2. Material별 검수데이터 (Total Inspection Points)
drop view ViewChart2TotalInspection 
GO
create view ViewChart2TotalInspection AS
select Material, count(*) ItemCount
  from ViewBeadMasterForSummary
 group by Material
GO

-- 28개 
-- 3. MachineLine별 융착데이터 (machine Series)
select MachineLine, count(*) ItemCount
  from ViewWeldMasterForSummary 
 group by MachineLine
 order by ItemCount desc 

drop view ViewChart3MachineSeries
go
create view ViewChart3MachineSeries AS
select x.MachineLine, sum(x.ItemCount) ItemCount
  from (
		select case when rn > 9 then 'Etc' ELSE x.MachineLine end MachineLine, x.ItemCount
		  from (
				select case when x.MachineLine = '' then 'Empty' else x.MachineLine end MachineLine, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				  from (
					select MachineLine, count(*) ItemCount
					  from ViewWeldMasterForSummary wm
					 group by MachineLine
				 ) x
			) x
	) x
 group by x.MachineLine
GO


-- 87개
-- 4-1. Welding Status (Status)
drop view ViewChart4WeldingStatus
GO
create view ViewChart4WeldingStatus AS
select x.Status, sum(x.ItemCount) ItemCount
  from (
		select case when rn > 9 then 'Etc' ELSE x.Status end Status, x.ItemCount
		  from (
				select case when x.Status = '' then 'Empty' else x.Status end Status, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				  from (
					select Status, count(*) ItemCount
					  from ViewWeldMasterForSummary wm
					 where (wm.Status not LIKE '%ERROR%' AND wm.Status not LIKE '0%')
					 group by Status
				 ) x
			) x
	) x
 group by x.Status
GO

select Status, ItemCount from ViewChart4WeldingStatus order by ItemCount desc 


-- 4-2. Bead Status (DVS)
drop view ViewChart5InspectionStatus
GO
create view ViewChart5InspectionStatus AS
select case when DVS = 'TRUE' THEN 'Weld OK' else 'Weld NOK' end DVS, count(*) ItemCount
from ViewBeadMasterForSummary
group by DVS
go

select DVS, ItemCount from ViewChart5InspectionStatus order by ItemCount DESC



-- 7. Last Update (최근30일)
drop view ViewTotalPoints
GO
create view ViewTotalPoints AS
select x.WeldingPoints, y.BeadPoints, x.LatestDays
  from (
		select count(*) WeldingPoints, isnull(max(vs.LatestDays), 30) LatestDays
		  from ViewWeldMasterForSummary w
		  cross join ViewSettings vs
		 where w.WeldingDate BETWEEN CONVERT(NVARCHAR(10), DATEADD(DAY, isnull(vs.LatestDays, 30) * -1, GETDATE()), 120) AND CONVERT(NVARCHAR(10), GETDATE(), 120)
	) x
cross join (
		select count(*) BeadPoints, isnull(max(vs.LatestDays), 30) LatestDays
		  from ViewBeadMasterForSummary b
		  cross join ViewSettings vs
		 where InspectionDate BETWEEN CONVERT(NVARCHAR(10), DATEADD(DAY, isnull(vs.LatestDays, 30) * -1, GETDATE()), 120) AND CONVERT(NVARCHAR(10), GETDATE(), 120)
	) y
GO
select top 1 WeldingPoints, BeadPoints, LatestDays from ViewTotalPoints

-----------------------------------------------------------------------------------------------------------

-- 5. Welding Points Per Outdiameter
drop view ViewWeldPointsPerOD
go
create view ViewWeldPointsPerOD AS
select Diameter, x.ItemCount, rn, 'OD'+ dbo.lpad(CAST(Diameter as VARCHAR),4,'0') AS OD
  from (
	select x.Diameter, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
	  from (
			select FLOOR(Diameter) Diameter, count(*) ItemCount
			  from (select isnull(Diameter, 0) Diameter from ViewWeldMasterForSummary) x
			 group by FLOOR(Diameter)
		 ) x
	) x
go

select Diameter, ItemCount, rn  FROM ViewWeldPointsPerOD order by Diameter


-- 6-1. Welding Points Per Year
select w.Year, count(*) ItemCount
  from ViewWeldMasterForSummary w
 WHERE w.Year BETWEEN '2011' and cast(DATEPART(YEAR, GETDATE()) as NVARCHAR(4))
 group by w.Year
 order by Year

-- 6-2. Welding Points Per Month
select Months, sum(ItemCount) ItemCount
  from (
		select w.Months, count(*) ItemCount
		  from ViewWeldMasterForSummary w
		 WHERE w.Year = '2020'
		 group by w.Year, w.Months
		union all 
		select DISTINCT Months, 0 from PeriodDay where Year = '2020'
	) x
 group by Months
 order by Months

 -----------------------------------------------------------------------------------------------------------

-- 8. Error Ranking (Top10)
-- Status 코드에 대한 정의 필요
select Status, count(*) ItemCount
  from ViewWeldMasterForSummary w
 where w.Year = '2019'
 group by Status
 order by ItemCount DESC

-- 년도 목록
select DISTINCT Year, Year+'년도' YearNm 
from PeriodDay
WHERE Year BETWEEN '2011' and cast(DATEPART(YEAR, GETDATE()) as NVARCHAR(4))
order by Year desc 

-- Error Ranking --> 이 요건은 년도조건때문에 usp_GetChartData 프로시져로 생성함
select x.Status, x.ItemCount
  from (
	select case when x.Status = '' then 'Empty' else x.Status end Status, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
		from (
			select Status, count(*) ItemCount
			from ViewWeldMasterForSummary wm
			where (wm.Status LIKE '%ERROR%' OR wm.Status LIKE '0%')
				and wm.Year = '2020' --
			group by Status
		) x
	) x
 where rn <= 10
 ORDER BY x.ItemCount
GO


-- 9. Welder Ranking (Top 10)
select x.WelderName, x.ItemCount
  from (
		select WelderName, ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
		  from (
				select isnull(WelderName,'') WelderName, count(*) ItemCount
				  from (select isnull(WelderName, WelderCode) WelderName 
						  from ViewWeldMasterForSummary w
						 where w.Year = '2019' --
						   ) x
				 group by WelderName
			) x
		 where x.WelderName <> ''
	) x
 where rn <= 10
 ORDER BY x.ItemCount
go

-- 10.Project Ranking (Top 20)
select x.WeldProjectDescr, x.ItemCount
  from (
		select WeldProjectDescr, ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
		  from (
				select WeldProjectDescr, count(*) ItemCount
				  from ViewWeldMasterForSummary w
				 where w.Year = '2019' --
				 group by WeldProjectDescr
			) x
		 where x.WeldProjectDescr <> ''
	) x
 where rn <= 20
 order by x.ItemCount

