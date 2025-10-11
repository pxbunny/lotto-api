# Lotto API

[![License: GPL-3.0](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

A REST API providing draw results for Polish Lotto and Lotto Plus games. The API supports JSON and CSV formats and includes automated updates for new draws.

## Features

- **Historical Data Endpoint**: Fetch draw results via `/api/draw-results`.
- **Multiple Formats**: Set `Accept: application/json` (default) or `Accept: application/octet-stream` (to download a CSV file).
- **Auto-Update**: New results are added automatically 45 minutes after each draw (Tue/Thu/Fri at 22:45 CET/CEST).
- **Initialization Tools**: Python scripts to populate the database with historical data.

## Getting Started

### Prerequisites
- .NET 9 SDK
- Azure account with active subscription
- Azure CLI
- Python (for initialization scripts, version 3.12+ recommended)
- Lotto API key from [Lotto Developers Portal](https://developers.lotto.pl/)

## Deployment via GitHub Actions

1. **Initialize Azure resources**:
   ```bash
   ./azure_init.sh
   ```

   Follow prompts to create resources. It generates an `azure-credentials.json` file in the root of the repository.

2. **Configure secrets** in your GitHub repository:
   - `LOTTO_API_KEY`: Your Lotto.pl API key
   - `AZURE_CREDENTIALS`: Contents of `azure-credentials.json`
   - `AZURE_RESOURCE_GROUP`: Your Azure resource group name

3. **Trigger deployment workflow**:

   Push to main branch or manually run "Build and Deploy Azure Resources" action

## Post-Deployment Setup

1. **Prepare python environment**:
   ```bash
   cd tools
   python -m venv .venv
   source .venv/bin/activate  # Linux/MacOS
   pip install -r requirements.txt
   ```

2. **Configure environment**:
   Copy `.env.template` to `.env` and update values:
   ```env
   LOTTO_API_KEY="<your-api-key>"
   USER_AGENT="<your_name/contact_info>" # no spaces
   STORAGE_CONNECTION_STRING="<from Azure Portal>"
   ```

3. **Initialize data**:
   ```bash
   # Fetch data from Lotto.pl API (default start date: 2000-01-01)
   python fetch_to_csv.py -d 2000-01-01 -f data.csv

   # Upload to Azure Storage
   python csv_to_storage.py -f data.csv
   ```

## Future Improvements

APIM with some of the Lotto API endpoints integrated.
