# Student Loader Service

The purpose of this repo is to demonstrate a legacy [Windows Service](https://docs.microsoft.com/en-us/dotnet/framework/windows-services/) written in .NET Framework deployed without any code change as a Windows Container to GKE Windows that persists to a SQL Server.  This sample highlights a few common considerations and solutions for working with Windows Containrs.

This service complements the [Cymbal University](https://github.com/jjdelorme/contoso-university/tree/cymbal)
 sample and is responsible for watching a configured directory for new files with student records.  When a file is detected the service parses the file and adds new students to the SQL Server database running in [Cloud SQL for SQL Server](https://cloud.google.com/sql/docs/sqlserver/quickstart).

## Building the container

Please see [this](https://cloud.google.com/kubernetes-engine/docs/how-to/creating-a-cluster-windows) for instructions on how to create a GKE Cluster with a Windows Server node pool run Windows Containers.

The dockerfile leverages the [sc tool](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/sc-create) already installed in the standard Windows Server image to add this service to the Service Control manager database on Windows.

```dockerfile
# Create the Windows Service
RUN sc create StudentLoader start=demand binpath=C:\\Cymbal\\StudentLoaderService.exe
```
### Service Monitor
A traditional Windows Service can be deployed to Kubernetes in a container **unchanged**, however in order to start and monitor the service, the [Service Monitor](https://github.com/microsoft/IIS.ServiceMonitor) is leveraged.

### Event Logs
As with many traditional Windows Services, it logs to the Windows Event Log.  In a Linux world most applications write to STDOUT, as such Kuberenetes was built to collect container logs from STDOUT and has no knowledge of the Winows Event Log.  Using the [Microsoft Log Monitor](https://github.com/microsoft/windows-container-tools/blob/master/LogMonitor/README.md) Windows events are redirected to STDOUT and can be collected by Kubernetes.

The service uses it's own Event Log Source which is created before the application is launched.  This powershell command is executed in the Dockerfile:

```powershell
New-EventLog -LogName "StudentLoader" -Source "StudentLoader";
```

To configure LogMonitor as a sink for the StudentLoader log we create a ```LogMonitorConfig.json``` file configured for that channel:

```json
        "channels": [
          {
            "name": "StudentLoader",
            "level": "Verbose"
          }
```

Both the ServiceMonitor.exe and LogMonitor.exe binaries are downloaded from their release repositories and copied to the container:

```dockerfile
# Create custom Event Log, Install LogMonitor.exe and ServiceMonitor.exe
RUN powershell -Command `

    $downloads = `
    @( `
        @{ `
            uri = 'https://dotnetbinaries.blob.core.windows.net/servicemonitor/2.0.1.10/ServiceMonitor.exe'; `
            outFile = 'C:\Cymbal\ServiceMonitor.exe' `
        }, `
        @{ `
            uri = 'https://github.com/microsoft/windows-container-tools/releases/download/v1.1/LogMonitor.exe'; `
            outFile = 'C:\LogMonitor\LogMonitor.exe' `
        } `
    ); `
    $downloads.ForEach({ Invoke-WebRequest -UseBasicParsing -Uri $psitem.uri -OutFile $psitem.outFile })

```

The following chaining of these tools is used as the docker entry point to capture the logs and monitor the service:

```dockerfile
# Start the service
ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\Cymbal\\ServiceMonitor.exe", "StudentLoader"]
```

## SMB File Share
The service connects to a network Windows SMB share to monitor for new files created that it should process.  This demonstrates using SMB on a GKE with Windows Nodes using the [FlexVolume](https://github.com/kubernetes/community/blob/master/contributors/devel/sig-storage/flexvolume.md).

The FlexVolume is deployed as a [DaemonSet](https://kubernetes.io/docs/concepts/workloads/controllers/daemonset/) in order to copy the driver to every node in the cluster.  This is setup in ```deploy\smb-flex-driver-setup.yaml```.  

The StudentLoader service deployment creates a volume with the following command:

```yaml
      volumes:
      - name: smb-volume
        flexVolume:
          driver: "microsoft.com/smb.cmd"
          secretRef:
            name: "smb-secret"
          options:
            source: "\\\\MY-WINDOWS-SERVER\\smbshare"
```

## Database

The [Cymbal University](https://github.com/jjdelorme/contoso-university/tree/cymbal) sample describes how to setup the SQL Server database which is running **unchanged** in Cloud SQL for SQL Server in GCP as a managed service.  The application leverages a Kubernetes secret for the database connection string which is mounted as a file ```C:\Cymbal\secret\connectionStrings.config```.  To mount the secret, the deployment mainfest contains these volume mount commands:

```yaml
...
        volumeMounts:
        - name: connection-strings
          mountPath: "/Cymbal/secret"
...
      volumes:
      - name: connection-strings
        secret:
          secretName: connection-strings            
```

To leverage this file from the application, we change the app config file to read from the file:

```xml
<configuration>
  ...
  <connectionStrings configSource="secret\connectionStrings.config"/>
  ...
</configuration>

...

--Work in Progress--

## gMSA
TODO: Instead of secret, pod uses gMSA to authenticate to SMB share.