import csv
import os
import uuid
from datetime import datetime

from dotenv import load_dotenv
from azure.data.tables import TableServiceClient, TableTransactionError, UpdateMode


load_dotenv()

STORAGE_CONNECTION_STRING = os.getenv('STORAGE_CONNECTION_STRING')

TABLE_NAME = 'LottoResults'
CSV_FILE = 'data.csv'


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
        MAX_DATE = datetime(9999, 12, 31)
        reader = csv.DictReader(file)

        for row in reader:
            draw_date_str = row['Draw date']
            draw_date = datetime.strptime(draw_date_str, "%Y-%m-%d")
            reversed_draw_date = datetime(1, 1, 1) + (MAX_DATE - draw_date)
            entity = {
                'PartitionKey': 'LottoData',
                'RowKey': reversed_draw_date.strftime("%Y-%m-%d").replace('-', ''),
                'DrawDate': draw_date_str,
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
