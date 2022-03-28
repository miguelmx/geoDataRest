/****** Object:  Table [dbo].[geotable]    Script Date: 28/03/2022 02:01:36 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[geotable](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[region] [varchar](150) NOT NULL,
	[origin_coord] [geography] NOT NULL,
	[destination_coord] [geography] NOT NULL,
	[datetime] [datetime] NOT NULL,
	[datasource] [varchar](150) NOT NULL,
	[ledger_start_transaction_id] [bigint] GENERATED ALWAYS AS transaction_id START HIDDEN NOT NULL,
	[ledger_end_transaction_id] [bigint] GENERATED ALWAYS AS transaction_id END HIDDEN NULL,
	[ledger_start_sequence_number] [bigint] GENERATED ALWAYS AS sequence_number START HIDDEN NOT NULL,
	[ledger_end_sequence_number] [bigint] GENERATED ALWAYS AS sequence_number END HIDDEN NULL,
 CONSTRAINT [PK_geotable] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[datetime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [dbo].[MSSQL_LedgerHistoryFor_1605580758] ), 
LEDGER = ON (LEDGER_VIEW = geotable_Ledger)
)
GO

ALTER TABLE [dbo].[geotable] ADD  CONSTRAINT [DF_geotable_datetime]  DEFAULT (getdate()) FOR [datetime]
GO



/****** Object:  StoredProcedure [dbo].[sp_list]    Script Date: 28/03/2022 01:15:07 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_list] 
as 
begin
select * from geotable
end

/****** Object:  StoredProcedure [dbo].[sp_get_box_average]    Script Date: 28/03/2022 01:05:42 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_get_box_average](
	@lat1  varchar(150), 
	@long1 varchar(150), 
	@lat2  varchar(150), 
	@long2 varchar(150) )
as
begin
	-- We take the four squares of the box area we want to query and create a polygon
	-- Remember to close the square (with the 5th coordinate) and keep in mind that long1 should be lower than long2

	DECLARE @boxv varchar(150)
	set @boxv = 'polygon((' + 
		@lat1 + ' ' + @long1 + ', ' + 
		@lat1 + ' ' + @long2 + ', ' + 
		@lat2 + ' ' + @long2 + ', ' + 
		@lat2 + ' ' + @long1 + ', ' + 
		@lat1 + ' ' + @long1 + '))' 
	DECLARE @g geography;  
	
	-- We create the binary geography polygon from the text
	SET @g = geography::STGeomFromText(@boxv, 4326);
	
	select avg(trips) average from 
	(
	select datepart(year, datetime) year , datepart(week, datetime ) week, count(1) trips
	from geotable
	where origin_coord.STWithin(@g) = 1
	group by  datepart(year, datetime) , datepart(week, datetime)
	) weekly_trips

	-- 1) We check origin_coord is within the @g polygon we created 
	-- 2) Group by year and week (because we can have multiple years in the dataset) and count the trips for each week
	-- 3) and finally take the average from the final list and return it

end

/****** Object:  StoredProcedure [dbo].[sp_get_region_average]    Script Date: 28/03/2022 01:11:25 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_get_region_average](@region varchar(150))
as
begin
	select avg(trips) average from 
	(
	select datepart(year, datetime) year , datepart(week, datetime ) week, count(1) trips
	from geotable
	where region = @region
	group by  datepart(year, datetime) , datepart(week, datetime)
	) weekly_trips

	-- 1) We check region match the provided one
	-- 2) Group by year and week (because we can have multiple years in the dataset) and count the trips for each week
	-- 3) and finally take the average from the final list and return it
end

/****** Object:  StoredProcedure [dbo].[sp_register_data]    Script Date: 28/03/2022 01:15:15 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [dbo].[sp_register_data]
(
@region varchar(150),
@origin_coord varchar(150),
@destination_coord varchar(150),
@datetime datetime, 
@datasource varchar(150)
)
as
begin

	-- a simple stored procedure to ingest data, 
	-- We take the coordinates in the point format (as the CSV) and convert it using the standard SRID

	INSERT INTO [dbo].[geotable]
           ([region]
           ,[origin_coord]
           ,[destination_coord]
           ,[datetime]
           ,[datasource])
     VALUES
           (@region, 
		   geography::STGeomFromText( @origin_coord,4326),
		   geography::STGeomFromText( @destination_coord,4326),
		   @datetime, @datasource) 
end



