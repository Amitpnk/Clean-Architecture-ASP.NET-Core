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

### Create SQL Server (cleanarchdb/sqladmin@123)

```
az sql server create --resource-group "rg-cleanarch-dev-shared-001" \
                     --name "sqldb-cleanarch-dev" \
                     --location "southindia" \
                     --admin-user "cleanarchuser" \
                     --admin-password 'sqladmin@123'
```

### Configure Firewall

```
az sql server firewall-rule create --resource-group "ClassifiedAds_DEV" \
                                    --server "sqldb-cleanarch-dev" \
                                    --name "development" \
                                    --start-ip-address "0.0.0.0" \
                                    --end-ip-address "255.255.255.255"                                   
```

### Create SQL Database

```
az sql db create --resource-group "rg-cleanarch-dev-shared-001" \
                 --name "classifiedadsdevdb" \
                 --server  "classifiedadsdev" \
                 --service-objective Basic \
                 --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

### Create Key Vault

```
az keyvault create --resource-group "rg-cleanarch-dev-shared-001" \
                   --name "classifiedadsdev" \
                   --location "southeastasia" \
                   --sku F1 \
                   --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

### Create App Service Plan

```
az appservice plan create --resource-group "rg-cleanarch-dev-shared-001" \
                          --name CleanArch-Hosts \
                          --location "southindia" \
                          --sku F1 \
                          --tags "Environment=Development" "Project=CleanArch" "Department=SD"
                          --number-of-workers 2
```

### Create App Services:

```
az webapp create --resource-group "rg-cleanarch-dev-shared-001" --plan CleanArch-Hosts --name webapi-Cleanarch --runtime "DOTNET:5.0" --tags "Environment=Development" "Project=CleanArch" "Department=SD"
```

### Create Lock:
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
