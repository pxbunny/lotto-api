import csv
import os
from argparse import ArgumentParser
from datetime import datetime

from dotenv import load_dotenv
from azure.data.tables import TableServiceClient, TableTransactionError, UpdateMode


load_dotenv()

STORAGE_CONNECTION_STRING = os.getenv('STORAGE_CONNECTION_STRING')

TABLE_NAME = 'LottoDrawResults'
DEFAULT_CSV_FILE_NAME = 'data.csv'


def parse_args():
    parser = ArgumentParser()
    parser.add_argument('-f', '--file', default=DEFAULT_CSV_FILE_NAME,
                        help='the csv file name to get the data from (default: %(default)s)')
    return parser.parse_args()


def upload_batch(table_client, batch):
    try:
        table_client.submit_transaction(batch)
        print(f"Uploaded batch of {len(batch)} records.")
    except TableTransactionError as e:
        print(f"Batch upload failed: {e}")


def upload_data_to_storage(csv_file, batch_size=100):
    table_service = TableServiceClient.from_connection_string(STORAGE_CONNECTION_STRING)
    table_client = table_service.get_table_client(TABLE_NAME)
    batch = []

    with open(csv_file, mode='r', encoding='utf-8') as file:
        MAX_DATE = datetime(9999, 12, 31)
        reader = csv.DictReader(file)

        for row in reader:
            draw_date_str = row['DrawDate']
            draw_date = datetime.strptime(draw_date_str, "%Y-%m-%d")
            reversed_draw_date = datetime(1, 1, 1) + (MAX_DATE - draw_date)
            entity = {
                'PartitionKey': 'LottoData',
                'RowKey': reversed_draw_date.strftime("%Y-%m-%d").replace('-', ''),
                'DrawDate': draw_date_str,
                'LottoNumbers': row['LottoNumbers'],
                'PlusNumbers': row['PlusNumbers']
            }
            batch.append(('upsert', entity, {'mode': UpdateMode.REPLACE}))

            if len(batch) >= batch_size:
                upload_batch(table_client, batch)
                batch = []

    if batch:
        upload_batch(table_client, batch)


if __name__ == '__main__':
    args = parse_args()
    upload_data_to_storage(args.file)
    print("Upload completed successfully!")
