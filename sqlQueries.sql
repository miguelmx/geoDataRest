-- I used a query to find the "two most commonly appearing regions" 
-- .. and a sub query looking for the latest datasource of each one
select region, (select top 1 datasource from geotable gt where gt.region = common.region order by datetime desc) latestdatasource from 
(select top 2 region, count(1)  c from geotable group by region order by c desc) common

-- selecting one occurence for those recordds who have had 'cheap_mobile' as their datasource
select distinct region from geotable where datasource = 'cheap_mobile'
