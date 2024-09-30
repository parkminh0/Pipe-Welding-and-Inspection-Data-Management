-- =============================================
-- Author:		<�츮�񿣾� �̿���>
-- Create date: <2020.07.07>
-- Description:	<���� Key ä������>
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetNextKey]
	@iCategory nvarchar(30), 
	@iInterval int = 1,
	@oNextKey int out
AS
BEGIN
	SET NOCOUNT ON;
	set @oNextKey = -1;
	declare @vCount int;

	BEGIN TRY
        BEGIN TRANSACTION

		select @vCount = Count(*) from KeyNumbering where Category = @iCategory;
		if @vCount = 0 begin
			insert into KeyNumbering values (@iCategory, 0, '');
		end

		if @iInterval < 1 BEGIN
			set @iInterval = 1;
		END

		UPDATE KeyNumbering set LastKeyNo = LastKeyNo + @iInterval where Category = @iCategory;
		select @oNextKey = LastKeyNo from KeyNumbering where Category = @iCategory;

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
