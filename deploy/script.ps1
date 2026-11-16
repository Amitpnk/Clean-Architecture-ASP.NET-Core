# Signs in to Azure. Interactive browser login is the default (supports MFA);
# pass -UserName to be prompted for that account's password instead.
param(
    [string]$UserName
)

if ([string]::IsNullOrWhiteSpace($UserName)) {
    az login
}
else {
    $AzCred = Get-Credential -UserName $UserName
    az login -u $AzCred.UserName -p $AzCred.GetNetworkCredential().Password
}
