#!/bin/bash
set -e

CREDENTIALS_FILE="azure-credentials.json"
SUBSCRIPTION_ID=$(az account show --query id --output tsv)

az login --use-device-code

read -p "Enter resource group name (allowed chars: a-z, 0-9, -, _): " RESOURCE_GROUP_NAME
while [[ ! $RESOURCE_GROUP_NAME =~ ^[a-zA-Z0-9_-]+$ ]]; do
    echo "Invalid name! Only letters, numbers, hyphens and underscores allowed."
    read -p "Enter valid resource group name: " RESOURCE_GROUP_NAME
done

az group create \
  --name "$RESOURCE_GROUP_NAME" \
  --location polandcentral

echo "Creating service principal..."
SP_INFO=$(az ad sp create-for-rbac \
  --name "${RESOURCE_GROUP_NAME}-sp" \
  --role Contributor \
  --scopes "subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP_NAME" \
  --query "{clientId: appId, clientSecret: password, tenantId: tenant, subscriptionId: '$SUBSCRIPTION_ID', resourceGroup: '$RESOURCE_GROUP_NAME'}" \
  --output json)

echo "$SP_INFO" > "$CREDENTIALS_FILE"
chmod 600 "$CREDENTIALS_FILE"

echo -e "\n\033[1;32mAzure resources created successfully!\033[0m"
echo "Resource Group: $RESOURCE_GROUP_NAME"
echo "Credentials saved to: $CREDENTIALS_FILE"
echo -e "\n\033[1;33mWARNING: Keep credentials file secure! Do not commit to version control.\033[0m"
