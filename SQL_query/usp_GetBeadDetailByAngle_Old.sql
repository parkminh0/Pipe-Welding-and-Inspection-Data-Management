-- =============================================
-- Author:		<우리비엔씨 이완종>
-- Create date: <2020.09.22>
-- Description:	<BeadDetail 데이터를 Angle(각도) 단위로 SELECT>
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetBeadDetailByAngle]
	@iBeadKey INT, 
	@iAngleUnit INT,
	@iChoiceAngle INT = 1,  -- 0:전체 1:해당 각도만
	@iIsActiveRow INT = 1,   -- 0:중복각도 포함 1:최종 Row만
	@oResult int OUT
AS
BEGIN
	SET NOCOUNT ON;
	declare @vAngleCount int;
	SET @vAngleCount = 360 / @iAngleUnit;
	SET @oResult = 0;

	select rn AngleNo, ((rn - 1) * @iAngleUnit) PrevAngle, (rn * @iAngleUnit) Angle
	  into #Tmp_Angle
	  from (select ROW_NUMBER() over(ORDER by pd.BaseYmd) rn from PeriodDay pd) CopyT
	 where CopyT.rn <= @vAngleCount ;

	select   bd.AngleNo
			,bd.Angle
			,bd.BeadDetailKey
			,bd.BeadKey
			,bd.[LineNo]
			,bd.SerialNo
			,bd.InspectionNo
			,bd.InspectionDate
			,bd.InspectionTime
			,bd.Position
			,bd.isPass
			,bd.KValue
			,bd.isKPass
			,bd.BeadWidthTotal
			,bd.isBWidthPass
			,bd.B1Width
			,bd.B2Width
			,bd.BRatio
			,bd.isBRatioPass
			,bd.MissAlignValue
			,bd.isMissAlignPass
			,bd.NotchValue
			,bd.isNotchPass
			,bd.ContactAngle
	  into #tmp_AngleData
	  from (
			select *
			  from (
					select * ,ROW_NUMBER() over(PARTITION BY t.Angle ORDER BY bd.InspectionTime DESC, bd.BeadDetailKey DESC) isActiveRow
					  from BeadDetail bd
					  cross join #Tmp_Angle t
					where bd.BeadKey = @iBeadKey
					  and bd.Position >= t.PrevAngle and bd.Position < t.Angle
				) x
		) bd
	 where bd.isActiveRow = CASE WHEN @iIsActiveRow = 1 then 1 ELSE bd.isActiveRow END
	   and bd.Angle = CASE WHEN @iChoiceAngle > 0 THEN @iChoiceAngle ELSE bd.Angle END
    ORDER BY bd.Position --DESC;

	-- @iChoiceAngle가 0이면 전체 각도의 집합과 Status만 가져옴
	if @iChoiceAngle = 0 BEGIN
		select t.Angle, d.BeadDetailKey, 
			   CASE WHEN d.isPass is NULL then 0 
					WHEN UPPER(d.isPass) = 'TRUE' THEN 1 
					ELSE 2
			   END Status
		  from #Tmp_Angle t
		  left join #tmp_AngleData d 
			on t.Angle = d.Angle
		  ORDER BY t.Angle
	END
	ELSE BEGIN
		SELECT * FROM #tmp_AngleData;
	END

END
