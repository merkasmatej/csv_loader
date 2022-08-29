-- ================================================
-- Template generated from Template Explorer using:

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE AddPerson
	 @ime NVARCHAR(255)
	,@prezime NVARCHAR(255)
	,@postanski_broj NVARCHAR(128)
	,@grad NVARCHAR(255)
	,@telefon NVARCHAR(255)
	,@returnMsg NVARCHAR(max) OUTPUT -- out parameter / returns success or error message

AS

IF (
		(SELECT COUNT(*) 
		FROM  [matej_test].[dbo].[Podaci]
		WHERE Telefon = @telefon)
		 < 1 )
BEGIN

	BEGIN TRY 
		INSERT INTO [matej_test].[dbo].[Podaci] 
		(
			[Ime],
			[Prezime],
			[PostanskiBroj],
			[Grad],
			[Telefon]
		)
		VALUES (
			@ime,
			@prezime,
			@postanski_broj,
			@grad,
			@telefon
		)
		END TRY

	BEGIN CATCH
		SET @returnMsg = ERROR_MESSAGE()
		RAISERROR(@returnMsg, 16,1);

		RETURN ERROR_NUMBER()
	END CATCH
END

SET @returnMsg = 'OK'

RETURN 0;