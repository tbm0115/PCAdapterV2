USE PCAdapter
GO
IF TYPE_ID(N'[mtc].[IDList]') IS NULL CREATE TYPE [mtc].[IDList] AS TABLE ( ID INT );
GO
IF EXISTS(SELECT * FROM sysobjects WHERE id=object_id(N'[mtc].[GetDuringDataItems]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) BEGIN DROP PROCEDURE [mtc].[GetDuringDataItems] END
GO
CREATE PROCEDURE [mtc].[GetDuringDataItems]
    -- Add the parameters for the stored procedure here
    @DataItemIds [mtc].[IDList] READONLY
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT * FROM [mtc].[Durations]
INNER JOIN
(SELECT 
	[Id] AS DataItemId, [Started] AS DataItemStarted, [Ended] AS DataItemEnded 
	FROM [mtc].[Durations] 
	WHERE [Id] IN (SELECT ID FROM @DataItemIds)) AS dataItem ON 
	(
		[Id]<>[dataItem].[DataItemId]
		AND (
				([Started]>=[dataItem].[DataItemStarted] AND [Started]<=[dataItem].[DataItemEnded])
				OR ([Ended]>=[dataItem].[DataItemStarted] AND [Ended]<=[dataItem].[DataItemEnded])
			)
	)
	JOIN [mtc].[DataItems] AS Item ON [Item_Id]=[Item].[Id]
END