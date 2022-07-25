set Scheduler=%cd%
echo %mypath%

sc STOP Scheduler

sc DELETE Scheduler

mkdir %cd%\Logs
ECHO.>>"%cd%\Scheduler.config"
ECHO -log_save_location=%cd%\Logs>>"%cd%\Scheduler.config"
ECHO.>>"%cd%\Scheduler.config"

sc CREATE Scheduler "binpath=%cd%\Scheduler.exe -config_file=%cd%\Scheduler.config"

sc START Scheduler