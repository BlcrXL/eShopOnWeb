create table #preview_file (id int identity (1,1), [file_name] varchar(128), dummy int, is_file int)
declare @subfolder varchar(256) = 'anigif_special'
declare @path varchar(256) = 'D:\Alex\lnrxxx18\' + @subfolder
insert #preview_file exec xp_dirtree @path, 2, 1

-- select * from #preview_file order by file_name -- 1879_O61_2_6.gif / 20130830_200459.3gp.jpg in MS SQL differs to FAR/Explorer sorting

select line from (
    select -1 sorter, 'insert preview_file2 values' line union all
    select id, '(' + cast(id as nvarchar(128)) + ', ''' + [file_name] + '''' + '),' from #preview_file lines
) lines2 order by sorter
OFFSET 1000 ROWS FETCH NEXT 1000 ROWS ONLY -- 0/1000 4 offset

--create table preview_file2 (id int, [file_name] varchar(128))

declare @catalog_brand int = 4 -- 1 for regular, 4 for special
declare @max_id int = isnull((select max(id) from catalog), 0)
insert [Catalog] (Id, [Name], Price, PictureUri, CatalogTypeId, CatalogBrandId)
select id + @max_id, SUBSTRING([file_name], 1, LEN([file_name]) - 4), 2, 'catalogbaseurltobereplaced/' + [file_name], 1, @catalog_brand from preview_file2
order by [file_name]
go
update [Catalog] set [Description] = [Name]
go

drop table #preview_file
--drop table preview_file2