$whereami=$pwd
Write-Output  $whereami

SC.exe STOP Scheduler
SC.exe DELETE Scheduler
$path = "$whereami\Logs"
    
If (!(test-path $path))
{
    md $path
}

Add-Content -Path "=$whereami\Scheduler.config" -value "\n$whereami\Logs"

$cmd = "$whereami\Scheduler.exe -config_file=$whereami\Scheduler.config"
SC.exe Create Scheduler binpath= "$cmd"
SC.exe START Scheduler