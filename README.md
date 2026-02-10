# Lotto API

[![License: GPL-3.0](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

> A **REST API** providing draw results for Polish **Lotto** and **Lotto Plus** games.
> The API supports JSON and CSV formats and includes automated updates for new draws.

## Features

- **Historical Data Endpoints**: Fetch draw results via `/api/draw-results`, `/api/draw-results/{date}`, or `/api/draw-results/latest`.
- **Sync Status**: Check if storage is up to date via `/api/sync`.
- **Multiple Formats**: Use `Accept: application/json` for JSON and `Accept: application/octet-stream` for CSV in `/api/draw-results`.
- **Auto-Update**: New results are added automatically 45 minutes after each draw (Tue/Thu/Sat at 22:45 CET/CEST).
- **Initialization Tools**: Python scripts to populate the database with historical data.

## API Endpoints

All routes use the default Azure Functions prefix: `/api`.

### `GET /api/draw-results`
Returns historical draw results from storage.

Optional query parameters:
- `dateFrom` (`yyyy-MM-dd`)
- `dateTo` (`yyyy-MM-dd`)
- `top` (positive integer)

Supported `Accept` headers:
- `application/json`, `application/*`, `*/*` -> JSON response
- `application/octet-stream` -> CSV file download

Possible status codes: `200`, `400`, `404`, `406`.

### `GET /api/draw-results/{date}`
Returns draw results for a single date from storage. `date` must use `yyyy-MM-dd` format.

Possible status codes: `200`, `400`, `404`.

### `GET /api/draw-results/latest`
Returns the latest draw results available in storage.

### `GET /api/sync`
Returns synchronization status between storage and the external Lotto API.

## Getting Started

### Prerequisites
- .NET 10.0.100 or later
- Azure account with active subscription
- Azure CLI
- Python (for initialization scripts, version 3.12+ recommended)
- Lotto API key from [Lotto Developers Portal](https://developers.lotto.pl/)
- Bash shell (Linux/macOS, or WSL/Git Bash on Windows for `azure_init.sh`)

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

   Push to main branch or manually run **Build and Deploy** action

## Post-Deployment Setup

1. **Prepare Python environment**:

   Linux / MacOS
   ```bash
   cd tools
   python -m venv .venv
   source .venv/bin/activate
   pip install -r requirements.txt
   ```
   Windows (PowerShell):
   ```powershell
   cd tools
   python -m venv .venv
   .\.venv\Scripts\Activate.ps1
   pip install -r requirements.txt
   ```
   Windows (Command Prompt):
   ```cmd
   cd tools
   python -m venv .venv
   .venv\Scripts\activate.bat
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
