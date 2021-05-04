$AzCred = Get-Credential -UserName amit.naik8103@gmail.com
az login -u $azcred.username -p $azcred.getnetworkcredential().password