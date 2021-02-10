kubectl create secret generic smb-secret \
    --from-file=./username \
    --from-file=./password \
    --type=microsoft.com/smb.cmd