import csv
import os
import time
from argparse import ArgumentParser
from datetime import datetime, timedelta

import requests
from dotenv import load_dotenv


load_dotenv()

LOTTO_API_KEY = os.getenv('LOTTO_API_KEY')
USER_AGENT = os.getenv('USER_AGENT')

ENDPOINT = 'https://developers.lotto.pl/api/open/v1/lotteries/draw-results/by-date-per-game'
DEFAULT_CSV_FILE_NAME = 'data.csv'
DEFAULT_START_DATE = '2000-01-01'
DATE_FORMAT = '%Y-%m-%d'
DELAY_SEC = 0.5


def parse_args():
    parser = ArgumentParser()
    parser.add_argument('-f', '--file', default=DEFAULT_CSV_FILE_NAME,
                        help='the csv file name to save the data to (default: %(default)s)')
    parser.add_argument('-d', '--date', default=DEFAULT_START_DATE,
                        help='date from which data collection starts (default: %(default)s)')
    return parser.parse_args()


def fetch_draw_results(date):
    headers = {
        'User-Agent': USER_AGENT,
        'secret': LOTTO_API_KEY
    }
    params = {
        'gameType': 'Lotto',
        'drawDate': date,
        'index': 1,
        'size': 100,
        'sort': 'drawSystemId',
        'order': 'ASC'
    }
    response = requests.get(ENDPOINT, headers=headers, params=params)
    code = response.status_code

    if code == 404 or code == 500:
        return code, response.json()

    response.raise_for_status()
    return code, response.json()


def save_to_csv(data, filename):
    with open(filename, mode='w', newline='', encoding='utf-8') as file:
        writer = csv.writer(file)
        writer.writerow(["Draw date", "Lotto numbers", "Plus numbers"])

        for draw in data:
            writer.writerow(draw)


if __name__ == '__main__':
    args = parse_args()
    date = datetime.strptime(args.date, DATE_FORMAT)
    end_date = datetime.today()
    results = []

    while date <= end_date:
        date_str = date.strftime(DATE_FORMAT)

        try:
            code, data = fetch_draw_results(date_str)

            if data and data.get('items'):
                game_results = {item['gameType']: item['results'][0]['resultsJson'] for item in data['items']}
                numbers = game_results.get('Lotto', [])
                plus_numbers = game_results.get('LottoPlus', [])

                results.append([
                    date_str,
                    ','.join(map(str, numbers)),
                    ','.join(map(str, plus_numbers))
                ])
                print(f"Numbers: {date_str} -> {numbers}, (plus: {plus_numbers})")

            elif code == 500:
                delay = DELAY_SEC * 10
                print(f'Request failed (response code: {code}). Retrying after {delay} seconds...')
                time.sleep(delay)
                continue

            else:
                print(f"No data for {date_str}, skipping...")

        except Exception as e:
            print(e)
            break

        date += timedelta(days=1)
        time.sleep(DELAY_SEC) # To prevent the 429 error code

    save_to_csv(results, args.file)
    print(f"Data saved to '{args.file}'")
