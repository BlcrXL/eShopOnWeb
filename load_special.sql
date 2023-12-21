declare @catalog_brand int = 4 -- 1 for regular, 4 for special
declare @max_id int = isnull((select max(id) from catalog), 0)
insert [Catalog] (Id, [Name], Price, PictureUri, CatalogTypeId, CatalogBrandId)
select id + @max_id, SUBSTRING([file_name], 1, LEN([file_name]) - 4), 2, 'catalogbaseurltobereplaced/' + [file_name], 1, @catalog_brand from preview_file2
order by [file_name]
go
update [Catalog] set [Description] = [Name]
go