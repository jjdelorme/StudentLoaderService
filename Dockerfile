# escape=`

FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Install LogMonitor.exe and ServiceMonitor.exe
RUN powershell -Command `
    New-Item -ItemType Directory C:\Cymbal; `
    New-Item -ItemType Directory C:\LogMonitor; `
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


# Copy Executable
COPY ./src/bin/Release/*.* C:/Cymbal/

# Copy log configuration file
COPY ./deploy/LogMonitorConfig.json C:/LogMonitor

# Create the Windows Service
RUN sc create StudentLoader start=demand binpath=C:\\Cymbal\\StudentLoaderService.exe

# Start the service
ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\Cymbal\\ServiceMonitor.exe", "StudentLoader"]