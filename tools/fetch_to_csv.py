import csv
import os
import time
from datetime import datetime, timedelta

import requests
from dotenv import load_dotenv


ENDPOINT = 'https://developers.lotto.pl/api/open/v1/lotteries/draw-results/by-date-per-game'
OUTPUT_FILE = 'data.csv'
DELAY_SEC = 0.5


def fetch_draw_results(date, api_key):
    try:
        headers = {
            'User-Agent': 'rczajka.me',
            'secret': api_key
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

        if response.status_code == 404:
            print(f"No data for {date}, skipping...")
            return None

        response.raise_for_status()
        return response.json()
    except requests.RequestException as e:
        print(e)
        raise e


def save_to_csv(data, filename):
    with open(filename, mode='w', newline='', encoding='utf-8') as file:
        writer = csv.writer(file)
        writer.writerow(["Draw date", "Lotto numbers", "Plus numbers"])
        
        for draw in data:
            writer.writerow(draw)


if __name__ == '__main__':
    results = []
    date = datetime(2000, 1, 1)
    end_date = datetime.today()

    load_dotenv()
    api_key = os.getenv('LOTTO_API_KEY')

    while date <= end_date:
        date_str = date.strftime('%Y-%m-%d')

        try:
            data = fetch_draw_results(date_str, api_key)
        except:
            break

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

        date += timedelta(days=1)
        time.sleep(DELAY_SEC) # To prevent the 429 error code

    save_to_csv(results, OUTPUT_FILE)
    print(f"Data saved to '{OUTPUT_FILE}'")
