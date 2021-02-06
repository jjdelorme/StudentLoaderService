# Sample Windows Service

The purpose of this repo is to demonstrate a legacy Windows Service written in .NET Framework deployed to GKE/Windows Containers.

## Event Logs
The service writes to the Windows Event Log.  Using the [Microsoft Log Monitor](https://github.com/microsoft/windows-container-tools/blob/master/LogMonitor/README.md) those events are redirected to STDOUT in order to be collected by Kubernetes.

## Service Monitor
As a traditional Windows Service, the application (unchanged) can be deployed to Kubernetes, however in order to start and monitor the service, the [Service Monitor](https://github.com/microsoft/IIS.ServiceMonitor) is leveraged.

## SMB File Share
The service connects to a network Windows SMB share to monitor for new files created that it should process.  This demonstrates using SMB on a GKE with Windows Nodes using the [FlexVolume](https://github.com/kubernetes/community/blob/master/contributors/devel/sig-storage/flexvolume.md).

...

--Work in Progress--

## Database
TODO: Connects to Cloud SQL for SQL Server

## gMSA
TODO: Instead of secret, pod uses gMSA to authenticate to SMB share.