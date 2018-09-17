
@set _date=%date%
@set _time=%time:~0,8%
@set _year=%_date:~10,4%

@set /p _build=< build
@set /a _build=%_build%+1
@echo %_build%>build
@echo %_build%>Views/Shared/Build.cshtml
@echo %_year%>Views/Shared/Year.cshtml
@echo %date% %_time%>Views/Shared/Date.cshtml
@set %_build%=
@set %_date%=

@set _date=
@set _time=
@set _year=
