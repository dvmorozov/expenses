
CREATE VIEW [expenses].[ControlStrings]
AS
SELECT Strings.Text, Languages.Language, Controls.Name AS ControlName, Languages.ID AS LanguageId
FROM expenses.Strings INNER JOIN
                  expenses.Languages ON Strings.LangId = Languages.ID INNER JOIN
                  expenses.Controls ON Strings.ControlId = Controls.ID

