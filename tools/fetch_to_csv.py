import requests
import csv
import time

URL = 'https://developers.lotto.pl/api/open/v1/lotteries/draw-results/by-date-per-game'
HEADERS = {
    'Content-Type': 'application/json',
    'secret': ''
}
OUTPUT_FILE = 'data.csv'


def fetch_draw_results(date):
    try:
        params = {
            'gameType': 'Lotto',
            'drawDate': date,
            'index': '1',
            'size': '2',
            'sort': 'drawBySystemId',
            'order': 'ASC'
        }
        response = requests.get(URL, params=params, headers=HEADERS)
        response.raise_for_status()
        return response.json()
    except requests.RequestException as e:
        print(f"Error for the date {date}: {e}")
        return None


def save_to_csv(data, filename):
    with open(filename, mode='w', newline='', encoding='utf-8') as file:
        writer = csv.writer(file)
        writer.writerow(["Draw date", "Lotto numbers"])
        
        for draw in data:
            writer.writerow(draw)


if __name__ == '__main__':
    results = []
    start_date = '2025-01-30'

    # for _ in range(?):
    data = fetch_draw_results(start_date)

    if data and data.get('items'):
        draw = data['items'][0]
        draw_date = draw['drawDate']
        numbers = draw['results'][0]['resultsJson']
        
        results.append([draw_date, ', '.join(map(str, numbers))])
        print(f'Numbers: {draw_date} -> {numbers}')

        start_date = draw_date[:10]
        time.sleep(1)
    else:
        print(f'No data for {start_date}.')
        # break

    save_to_csv(results, OUTPUT_FILE)
    print(f'Data saved to file: {OUTPUT_FILE}')
