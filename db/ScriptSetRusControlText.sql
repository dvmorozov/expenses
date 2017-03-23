
use ExpensesDev
go

--begin tran

delete from Languages
delete from Controls
delete from Strings

declare @LangIds table (ID int)
declare @ControlIds table (ID int, Name nchar(30))

insert into Controls (Name)
output inserted.id, inserted.Name into @ControlIds
values 
	(N'ButtonAddExpense'), (N'ButtonAddCategory'), (N'LabelAssociateWithCategories'), 
	(N'LabelNewCategory'), (N'LabelExpense'), (N'LabelAmount'), 
	(N'LabelNote'), (N'LabelToday'), (N'LabelForMonth'), 
	(N'LabelDate')

insert into Languages (Language)
output inserted.id into @LangIds
values (N'Русский')

declare @LangId int
set @LangId = (select top(1) ID from @LangIds) --(select ID from Languages offset 0 rows fetch first 1 row only)

insert into Strings (LangId, ControlId, Text)
select @LangId, c.ID, 
	case 
		when c.Name = N'ButtonAddExpense' then N'Добавить Трату'
		when c.Name = N'ButtonAddCategory' then N'Добавить Категорию'
		when c.Name = N'LabelAssociateWithCategories' then N'Включить в категории'
		when c.Name = N'LabelNewCategory' then N'Новая категория'
		when c.Name = N'LabelExpense' then N'Трата'
		when c.Name = N'LabelAmount' then N'Сумма'
		when c.Name = N'LabelDate' then N'Дата'
		when c.Name = N'LabelNote' then N'Замечание'
		when c.Name = N'LabelToday' then N'За день'
		when c.Name = N'LabelForMonth' then N'За месяц'
	else
		NULL
	end
from @ControlIds c

--rollback
