import csv
import os
import uuid

from dotenv import load_dotenv
from azure.data.tables import TableServiceClient, TableTransactionError, UpdateMode


load_dotenv()

STORAGE_CONNECTION_STRING = os.getenv('STORAGE_CONNECTION_STRING')

TABLE_NAME = 'LottoResults'
CSV_FILE = 'data_full_2025-01-31.csv'


def upload_batch(table_client, batch):
    try:
        table_client.submit_transaction(batch)
        print(f"Uploaded batch of {len(batch)} records.")
    except TableTransactionError as e:
        print(f"Batch upload failed: {e}")


def upload_data_to_azure(csv_file, batch_size=100):
    table_service = TableServiceClient.from_connection_string(STORAGE_CONNECTION_STRING)
    table_client = table_service.get_table_client(TABLE_NAME)
    batch = []

    with open(csv_file, mode='r', encoding='utf-8') as file:
        reader = csv.DictReader(file)

        for row in reader:
            entity = {
                'PartitionKey': 'LottoData',
                'RowKey': row['Draw date'],
                'LottoNumbers': row['Lotto numbers'],
                'PlusNumbers': row['Plus numbers']
            }
            batch.append(('upsert', entity, {'mode': UpdateMode.REPLACE}))

            if len(batch) >= batch_size:
                upload_batch(table_client, batch)
                batch = []
    
    if batch:
        upload_batch(table_client, batch)


if __name__ == '__main__':
    upload_data_to_azure(CSV_FILE)
    print("Upload completed successfully!")
