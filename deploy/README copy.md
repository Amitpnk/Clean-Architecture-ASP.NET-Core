https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming

### Login Azure

-u <user-name> -p <password> -t <tenant-id(GUID)>

```
az login -u xxx@xxx.com -p xxxx --tenant 10xxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx

or

az login

```

### Group exist

```
az group exists -n rg-cleanarch-dev-shared-001
```

### Create Resource Group

```
az group create --name "rg-cleanarch-dev-shared-001" 
                --location "southindia" 
                --tags "Environment=Development" 
                "Project=CleanArch" "Department=SD" "ResourceType=Mixed"
```

### Create App Service Plan

```
az appservice plan create --resource-group "rg-cleanarch-dev-shared-001" \
                          --name cleanarch-hosts \
                          --location "southindia" \
                          --sku F1 \
                          --tags "Environment=Development" "Project=CleanArch" "Department=SD"
                          --number-of-workers 2
```

### Create App Services:

```
az webapp create --resource-group "rg-cleanarch-dev-shared-001" 
                 --plan cleanarch-hosts 
                 --name app-cleanarchapi-dev-001 
                 --runtime "DOTNET:5.0" 
                 --tags "Environment=Development" "Project=CleanArch" "Department=SD"
```

### Set username for deployment:

```
az webapp deployment user set --user-name cleanarchPs
```

### Create SQL Server (cleanarchuser/sqladmin123#@)

```ps1
az sql server create --resource-group "rg-cleanarch-dev-shared-001" \
                     --name "sql-cleanarch-dev" \
                     --location "centralindia" \
                     --admin-user "cleanarchuser" \
                     --admin-password 'sqladmin123#@'
```

### Create SQL Database

```ps1
az sql db create --resource-group "rg-cleanarch-dev-shared-001" \
                --name "sqldb-cleanarch-dev" \
                --server  "sql-cleanarch-dev" \
                --service-objective Free \
                --tags "Environment=Development" "Project=CleanArch" "Department=SD"
```

For reference:

```ps1
az sql db list-editions -a -o table -l centralindia
```


### Configure Firewall

```ps1
az sql server firewall-rule create --resource-group "rg-cleanarch-dev-shared-001" \
                                    --server "sqldb-cleanarch-dev" \
                                    --name "development" \
                                    --start-ip-address "0.0.0.0" \
                                    --end-ip-address "255.255.255.255"                                   
```

### Create Lock

```
az lock create --lock-type CanNotDelete \
               --name CanNotDelete \
               --resource-group rg-cleanarch-dev-shared-001
```

### Clean Up

```
az lock delete --name CanNotDelete --resource-group rg-cleanarch-dev-shared-001
az group delete --name "rg-cleanarch-dev-shared-001" --yes
```
