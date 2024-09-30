-- =============================================
-- Author:		<우리비엔씨 이완종>
-- Create date: <2020.09.22>
-- Description:	Chart용 데이터 추출
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetChartData]
	@iDataNo INT, 
	@iYear NVARCHAR(4),
	@iCondInt INT,
	@oResult int OUT
AS
BEGIN
	SET NOCOUNT ON;
	SET @oResult = 0;

	if @iDataNo = 1 BEGIN
		select x.Status, x.ItemCount
		  from (
			select case when x.Status = '' then 'Empty' else x.Status end Status, x.ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				from (
					select Status, count(*) ItemCount
					from ViewWeldMasterForSummary wm
					where (wm.Status LIKE '%ERROR%' OR wm.Status LIKE '0%')
						and wm.Year = case when @iYear = '' then wm.Year ELSE @iYear END
					group by Status
				) x
			) x
		 where rn <= 10
		 ORDER BY x.ItemCount
	END
	ELSE if @iDataNo = 2 BEGIN
		select x.WelderName, x.ItemCount
		  from (
				select WelderName, ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				  from (
						select isnull(WelderName,'') WelderName, count(*) ItemCount
						  from (select isnull(WelderName, WelderCode) WelderName 
								  from ViewWeldMasterForSummary w
								 where w.Year = case when @iYear = '' then w.Year ELSE @iYear END
							) x
						 group by WelderName
					) x
				 where x.WelderName <> ''
			) x
		 where rn <= 10
		 ORDER BY x.ItemCount
	END
	ELSE if @iDataNo = 3 BEGIN
		select x.WeldProjectDescr, x.ItemCount
		  from (
				select WeldProjectDescr, ItemCount, ROW_NUMBER() over(order by ItemCount desc) rn
				  from (
						select WeldProjectDescr, count(*) ItemCount
						  from ViewWeldMasterForSummary w
						 where w.Year = case when @iYear = '' then w.Year ELSE @iYear END
						 group by WeldProjectDescr
					) x
				 where x.WeldProjectDescr <> ''
			) x
		 where rn <= 20
		 order by x.ItemCount
	END
END
