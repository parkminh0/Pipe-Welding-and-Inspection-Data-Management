USE [AGRUDB]
GO

/****** Object:  StoredProcedure [dbo].[usp_CreateChartTable]    Script Date: 2023-11-17 오전 11:27:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =====================================================================
-- Author:		<우리비엔씨 박민호>
-- Create date: <2023.10.25>
-- Description:	<요약통계1, 2 화면용 ChartTable 생성>
-- =====================================================================
ALTER PROCEDURE [dbo].[usp_CreateChartTable]
AS
BEGIN
	BEGIN TRY
        BEGIN TRANSACTION
			-------------------------------------------------------------------------
		    -- 비우기
			-------------------------------------------------------------------------
			TRUNCATE TABLE ChartTable
			TRUNCATE TABLE ChartTime
			;
			-------------------------------------------------------------------------
			-- 0. 데이터 로드 시간
			-------------------------------------------------------------------------
			INSERT INTO ChartTime
			VALUES (CURRENT_TIMESTAMP)
			;
			-------------------------------------------------------------------------
			-- 1. Total Welding Point 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO charttable
				SELECT 1 chartkey,
					'Total Welding Point' chartname,
					w.WeldingDate DataDate,
					w.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					NULL ItemName,
					COUNT(*) ItemValue,
					w.CreateID
				FROM WeldMaster w
			LEFT JOIN Project p
					ON w.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.Compkey = c.Compkey
				WHERE 1 = 1
				GROUP BY c.CompName,
						 c.CompKey,
						 w.ProjectKey,
						 p.ProjectName,
						 w.WeldingDate,
						 w.CreateID
			;
			-------------------------------------------------------------------------
			-- 2. Total Inspection Point 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO charttable
				SELECT 2 chartkey,
					'Total Inspection Point' chartname,
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					NULL itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM BeadMaster bm
			LEFT JOIN Project p
					ON bm.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.Compkey
				GROUP BY c.CompName,
						c.CompKey,
						p.ProjectKey,
						p.ProjectName,
						bm.InspectionDate,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 3. Weld OK, NOK 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO charttable
				SELECT 3 chartkey,
					'Weld OK, NOK' chartname,
					w.WeldingDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					REPLACE(w.Status, 'Error', 'ERROR') itemname,
					COUNT(*) itemvalue,
					w.CreateID
				FROM WeldMaster w
				JOIN PeriodDay pd
					ON pd.BaseYmd = w.WeldingDate
			LEFT JOIN BeadMaster bm
					ON bm.MachineSerialNo = w.MachineSerialNo
					AND bm.WeldNo = w.WeldNo
					AND bm.WeldingDate = w.WeldingDate
			LEFT JOIN Project p
					ON w.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.Compkey = c.Compkey
				WHERE 1 = 1
				GROUP BY c.CompName,
						c.CompKey,
						p.ProjectKey,
						p.ProjectName,
						w.WeldingDate,
						w.Status,
						w.CreateID
			;
			-------------------------------------------------------------------------
			-- 4. Inspect OK, NOK 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO charttable
				SELECT 4 chartkey,
					'Inspect OK, NOK' chartname,
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					bm.DVS itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM beadmaster bm
				JOIN PeriodDay pd
					ON pd.BaseYmd = bm.InspectionDate
			LEFT JOIN Project p
					ON bm.Projectkey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.CompKey
				WHERE 1 = 1
				GROUP BY bm.DVS,
						bm.InspectionDate,
						p.ProjectKey,
						p.ProjectName,
						c.CompName,
						c.CompKey,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 5. Welding Material 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 5 chartkey,
					'Welding Material' chartname,
					wm.WeldingDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					wm.Material itemname,
					COUNT(*) itemvalue,
					wm.CreateID
				FROM WeldMaster wm
				JOIN PeriodDay pd
					ON pd.BaseYmd = wm.WeldingDate
			LEFT JOIN BeadMaster bm
					ON bm.MachineSerialNo = wm.MachineSerialNo
					AND bm.WeldNo = wm.WeldNo
					AND bm.WeldingDate = wm.WeldingDate
			LEFT JOIN Project p
					ON wm.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.Compkey = c.Compkey
				WHERE 1 = 1
				GROUP BY c.CompName,
						p.ProjectKey,
						p.ProjectName,
						c.CompKey,
						wm.WeldingDate,
						wm.Material,
						wm.CreateID
			;
			-------------------------------------------------------------------------
			-- 6. Inspection Material 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 6 chartkey,
					'Inspection Material' chartname,
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					bm.Material itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM beadmaster bm
				JOIN PeriodDay pd
					ON pd.BaseYmd = bm.InspectionDate
			LEFT JOIN Project p
					ON bm.Projectkey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.CompKey
				WHERE 1 = 1
				GROUP BY bm.InspectionDate,
						p.ProjectName,
						p.ProjectKey,
						c.CompKey,
						c.CompName,
						bm.Material,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 7. Inspection NOK Reason 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 8 chartkey,
					'Inspection NOK Reason' chartname,
					x.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					x.ItemName itemname,
					SUM(ItemValue) itemvalue,
					x.CreateID
				FROM ( SELECT   InspectCreateID CreateID,
				                ProjectKey,
								InspectionDate,
								ItemName,
								COUNT(*) itemvalue
						FROM ViewBeadMasterForSummary v
								UNPIVOT ( tfCount
										FOR ItemName IN ([K-Value],
														[Bead Width],
														[Uneven Bead],
														[Mis-Al],
														[Notch],
														[NotchEye],
														[Ang-Dev],
														[Crack],
														[Void],
														[Support],
														[Interrupt],
														[Overheat],
														[OtherInfo2]) ) AS x
						WHERE 1 = 1
							AND UPPER(x.tfCount) = 'FALSE'
						GROUP BY InspectCreateID, 
						         ProjectKey,
								 InspectionDate,
								 ItemName ) x
			LEFT JOIN Project p
					ON x.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON c.CompKey = p.ProjectKey
				WHERE 1 = 1
				GROUP BY x.InspectionDate,
						p.ProjectName,
						p.ProjectKey,
						c.CompKey,
						c.CompName,
						x.ItemName,
						x.CreateID

			-------------------------------------------------------------------------
			-- 9. Welder 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 9 chartkey,
					'Welder' chartname,
					wm.WeldingDate DataDate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					wm.WelderName itemname,
					COUNT(*) itemvalue,
					wm.CreateID
				FROM WeldMaster wm
				JOIN PeriodDay pd
					ON pd.BaseYmd = wm.WeldingDate
			LEFT JOIN BeadMaster bm
					ON bm.MachineSerialNo = wm.MachineSerialNo
					AND bm.WeldNo = wm.WeldNo
					AND bm.WeldingDate = wm.WeldingDate
			LEFT JOIN Project p
					ON wm.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.Compkey = c.Compkey
				WHERE 1 = 1
				AND wm.WelderName IS NOT NULL
				AND wm.WelderName != ''
				AND wm.WelderName != 'WELDER'
				GROUP BY c.CompName,
						c.CompKey,
						p.ProjectKey,
						p.ProjectName,
						wm.WeldingDate,
						wm.WelderName,
						wm.CreateID
			;
			-------------------------------------------------------------------------
			-- 10. Inspector 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 10 chartkey,
					'Inspector' chartname,
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					bm.InspectorID itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM beadmaster bm
				JOIN PeriodDay pd
					ON pd.BaseYmd = bm.InspectionDate
			LEFT JOIN Project p
					ON bm.Projectkey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.CompKey
				WHERE 1 = 1
				AND bm.InspectorID IS NOT NULL
				AND bm.InspectorID != ''
				GROUP BY bm.InspectorID,
						bm.InspectionDate,
						p.ProjectKey,
						p.ProjectName,
						c.CompName,
						c.CompKey,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 11. Welding Machine 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 11 chartkey,
					'Welding Machine' chartname,
					wm.WeldingDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					CASE WHEN MachineLine = '' THEN 'Empty'
					ELSE MachineLine
					END itemname,
					COUNT(*) itemvalue,
					wm.CreateID
				FROM WeldMaster wm
				JOIN PeriodDay pd
					ON pd.BaseYmd = wm.WeldingDate
			LEFT JOIN BeadMaster bm
					ON bm.MachineSerialNo = wm.MachineSerialNo
					AND bm.WeldNo = wm.WeldNo
					AND bm.WeldingDate = wm.WeldingDate
			LEFT JOIN Project p
					ON wm.ProjectKey = p.ProjectKey
			LEFT JOIN Company c
					ON p.Compkey = c.Compkey
				WHERE 1 = 1
				GROUP BY c.CompName,
						p.ProjectKey,
						c.CompKey,
						p.ProjectName,
						wm.WeldingDate,
						wm.MachineLine,
						wm.CreateID
			-------------------------------------------------------------------------
			-- 12. Inspection Machine 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 12 chartkey,
					'Inspection Machine',
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					SerialNo itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM BeadMaster bm
				JOIN PeriodDay pd
					ON pd.BaseYmd = bm.InspectionDate
			LEFT JOIN Project p
					ON bm.Projectkey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.CompKey
				WHERE 1 = 1
				GROUP BY bm.InspectionDate,
						p.ProjectName,
						p.ProjectKey,
						c.CompName,
						c.CompKey,
						bm.SerialNo,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 13. Welding Outside Diameter 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO charttable
				 SELECT 13 chartkey,
						'Welding Outside Diameter' chartname,
						wm.WeldingDate datadate,
						p.ProjectKey,
						p.ProjectName,
						c.CompKey,
						c.CompName,
						wm.Diameter itemname,
						COUNT(*) itemvalue,
						wm.CreateID
				   FROM WeldMaster wm
				   JOIN PeriodDay pd
					 ON pd.BaseYmd = wm.WeldingDate
			  LEFT JOIN BeadMaster bm
					 ON bm.MachineSerialNo = wm.MachineSerialNo
						AND bm.WeldNo = wm.WeldNo
						AND bm.WeldingDate = wm.WeldingDate
			  LEFT JOIN Project p
					 ON wm.ProjectKey = p.ProjectKey
			  LEFT JOIN Company c
					 ON p.Compkey = c.Compkey
				  GROUP BY c.CompName,
						   c.CompKey,
						   p.ProjectKey,
						   p.ProjectName,
						   wm.WeldingDate,
						   wm.Diameter ,
						   wm.CreateID
			;
			-------------------------------------------------------------------------
			-- 14. Inspection Outside Diameter 챠트 데이터 생성
			-------------------------------------------------------------------------
			INSERT INTO ChartTable
				SELECT 14 chartkey,
					'Inspection Outside Diameter' chartname,
					bm.InspectionDate datadate,
					p.ProjectKey,
					p.ProjectName,
					c.CompKey,
					c.CompName,
					bm.OD itemname,
					COUNT(*) itemvalue,
					bm.CreateID
				FROM beadmaster bm
				JOIN PeriodDay pd
					ON pd.BaseYmd = bm.InspectionDate
			LEFT JOIN Project p
					ON bm.Projectkey = p.ProjectKey
			LEFT JOIN Company c
					ON p.CompKey = c.CompKey
				 JOIN CodeInfomation ci 
				    ON ci.Category = 'Diameter' and cast(ci.DetailCode as int) = bm.OD
				WHERE 1 = 1
				--AND bm.OD IN (20, 25, 32, 40,
				--				50, 63, 75, 90,
				--				110, 125, 140, 160,
				--				180, 200, 225, 250,
				--				280, 315)
				GROUP BY bm.OD,
						bm.InspectionDate,
						p.ProjectKey,
						p.ProjectName,
						c.CompName,
						c.CompKey,
						bm.CreateID
			;
			-------------------------------------------------------------------------
			-- 15. 요약통계2 화면용 Welding Outside Diameter 챠트 데이터 생성(PVDF, PP 구분)
			-------------------------------------------------------------------------
			INSERT INTO charttable
				 SELECT 15 chartkey,
						CASE WHEN wm.Material = 'PVDF' THEN 'Welding Outside Diameter-F'
							WHEN wm.Material = 'PP' THEN 'Welding Outside Diameter-P'
						END AS chartname,
						wm.WeldingDate datadate,
						p.ProjectKey,
						p.ProjectName,
						c.CompKey,
						c.CompName,
						wm.Diameter itemname,
						COUNT(*) itemvalue,
						wm.CreateID
				   FROM WeldMaster wm
				   JOIN PeriodDay pd
					 ON pd.BaseYmd = wm.WeldingDate
			  LEFT JOIN BeadMaster bm
					 ON bm.MachineSerialNo = wm.MachineSerialNo
						AND bm.WeldNo = wm.WeldNo
						AND bm.WeldingDate = wm.WeldingDate
			  LEFT JOIN Project p
					 ON wm.ProjectKey = p.ProjectKey
			  LEFT JOIN Company c
					 ON p.Compkey = c.Compkey
				  WHERE wm.Material = 'PVDF'
					 OR wm.material = 'PP'
				  GROUP BY c.CompName,
						   c.CompKey,
						   p.ProjectKey,
						   p.ProjectName,
						   wm.WeldingDate,
						   wm.Diameter,
						   wm.Material,
						   wm.CreateID
			;
			-------------------------------------------------------------------------
			-- 16. 요약통계2 화면용 Inspection Outside Diameter 챠트 데이터 생성(PVDF, PP 구분)
			-------------------------------------------------------------------------
			INSERT INTO charttable
				 SELECT 16 chartkey,
						CASE WHEN bm.Material = 'PVDF' THEN 'Inspection Outside Diameter-F'
							WHEN bm.Material = 'PP' THEN 'Inspection Outside Diameter-P'
						END AS chartname,
						bm.InspectionDate datadate,
						p.ProjectKey,
						p.ProjectName,
						c.CompKey,
						c.CompName,
						bm.OD itemname,
						COUNT(*) itemvalue,
						bm.CreateID
				   FROM BeadMaster bm
				   JOIN PeriodDay pd
					 ON pd.BaseYmd = bm.InspectionDate
			  LEFT JOIN Project p
					 ON bm.ProjectKey = p.ProjectKey
			  LEFT JOIN Company c
					 ON p.Compkey = c.Compkey
				  WHERE bm.Material = 'PVDF'
					 OR bm.material = 'PP'
				  GROUP BY c.CompName,
						   c.CompKey,
						   p.ProjectKey,
						   p.ProjectName,
						   bm.InspectionDate,
						   bm.OD,
						   bm.Material,
						   bm.CreateID


        if @@TRANCOUNT > 0
			COMMIT TRANSACTION;

		return 0;
    END TRY
    BEGIN CATCH
        if @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		return -1;
    END CATCH;

END
GO


