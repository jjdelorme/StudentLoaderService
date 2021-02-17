 param (
    [string]$version = "v2.0"
 )

$projectID = gcloud config get-value project;
gcloud --quiet auth configure-docker;
docker build -t gcr.io/$projectID/studentloader:$version -f Dockerfile .;
if ($?) {
    docker push gcr.io/$projectID/studentloader:$version;
}