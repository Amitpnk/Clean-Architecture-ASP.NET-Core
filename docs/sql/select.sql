;with result as(
select *, CAST(Chapter  AS varchar)+'.'+CAST(verse AS varchar) as bg from Cards 
)
select * from result 
where bg in (18.59)
