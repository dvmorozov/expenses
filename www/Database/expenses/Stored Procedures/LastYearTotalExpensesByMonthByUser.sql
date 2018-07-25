﻿
-- ==========================================================================================
-- Author:		D.V.Morozov
-- Last modified: 22/07/2018
-- Description:	For compatibility returns the first part of dataset corresponding to 
--				the first non-NULL currency. 
-- ==========================================================================================

CREATE PROCEDURE [expenses].LastYearTotalExpensesByMonthByUser @LastMonthNumber INT, @DataOwner UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @T TABLE (
		--	Mustn't allow NULLs. https://github.com/dvmorozov/expenses/issues/17
		Y INT NOT NULL, 
		M INT NOT NULL,
		Total FLOAT NOT NULL, 
		Month NVARCHAR(10) NULL,
		Currency NCHAR(5)
		)

	-- All parts must have the same size equal to @LastMonthNumber, 
	-- the dataset must be ordered by currency at first.
	INSERT INTO @T EXEC [expenses].LastYearTotalExpensesByMonthByUser2 @LastMonthNumber, @DataOwner

	SELECT TOP (@LastMonthNumber) Y, M, Total
	FROM @T
	WHERE Currency IS NOT NULL
END
