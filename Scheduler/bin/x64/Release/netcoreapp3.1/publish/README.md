# Scheduler Setup

Step by step guide to setup Scheduler.

> *This application can be only run on windows 10 *

### Prerequisite
1. Install Microsoft SQL serve
2. Run the DB script as per the sequence

### Steps

1. Create a new folder and copy below files to created folder 
<pre>
C:\Scheduler
              setup.bat
              setup.ps1
              Scheduler.exe
			  Scheduler.config </pre>
2. Update DB connection string in the **Scheduler.config** file with relevant detials     <pre>DB_Conn=Data Source=localhost,1433;Initial Catalog=L3;User ID=structo;Password=structo123;TrustServerCertificate=True;Encrypt=False;</pre>
3. Open Command line (CMD) as administrator  run the below command.
        cd C:\Scheduler
        	.\setup.bat 
> This will create a  Window Service named **Scheduler** and **Log** folder.

#### Verification
1. Open windows services and check for **Scheduler** service created, and the status should be  **Running**
2. Check for the** Logs** folder inside the setup folder (eg: C:\Scheduler)
3. Check if there any connection related errors inside the latest log file insidein the  Logs folder;
