 param (
    [string]$version = "v2.1"
 )

$projectID = gcloud config get-value project;
gcloud --quiet auth configure-docker;
docker build -t gcr.io/$projectID/studentloader:$version -f c:\workspace\Dockerfile c:\workspace;
if ($?) {
    docker push gcr.io/$projectID/studentloader:$version;
}