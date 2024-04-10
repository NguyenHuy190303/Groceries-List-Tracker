from flask import Flask, jsonify, request
import pymysql.cursors
from datetime import datetime

app = Flask(__name__)

# Database connection
connection = pymysql.connect(
    host='mariadb.vamk.fi',
    user='e2101098',
    password='cqgYeaFEN6A',
    db='e2101098_Windows',
    charset='utf8mb4',
    cursorclass=pymysql.cursors.DictCursor
)

@app.route('/user/<int:UserID>/purchases', methods=['GET'])
def get_user_purchases(UserID):
    start_date = request.args.get('start_date')
    end_date = request.args.get('end_date')
    start_date = datetime.strptime(start_date, '%Y-%m-%d')
    end_date = datetime.strptime(end_date, '%Y-%m-%d')

    try:
        with connection.cursor() as cursor:
            sql = "SELECT * FROM Purchases JOIN UserPurchases ON Purchases.PurchaseID = UserPurchases.PurchaseID WHERE UserPurchases.UserID = %s AND Purchases.PurchaseDate BETWEEN %s AND %s"
            cursor.execute(sql, (UserID, start_date, end_date))
            purchases = cursor.fetchall()
            return jsonify(purchases)
    except Exception as e:
        return jsonify({'error': str(e)})

@app.route('/item/<int:ItemID>/prices', methods=['GET'])
def get_item_prices(ItemID):
    try:
        with connection.cursor() as cursor:
            sql = "SELECT * FROM Purchases WHERE ItemID = %s ORDER BY PurchaseDate"
            cursor.execute(sql, (ItemID,))
            purchases = cursor.fetchall()
            return jsonify(purchases)
    except Exception as e:
        return jsonify({'error': str(e)})

@app.route('/user/<int:UserID>/budget', methods=['POST'])
def set_budget(UserID):
    data = request.get_json()
    budget_amount = data['budget_amount']
    time_period = data['time_period']

    try:
        with connection.cursor() as cursor:
            sql = "INSERT INTO Budgets (UserID, Amount, TimePeriod) VALUES (%s, %s, %s)"
            cursor.execute(sql, (UserID, budget_amount, time_period))
            connection.commit()
            return jsonify({'message': 'Budget set!'})
    except Exception as e:
        return jsonify({'error': str(e)})

@app.route('/user/<int:UserID>/spending', methods=['GET'])
def get_spending(UserID):
    now = datetime.now()
    start_date = now.replace(day=1)

    try:
        with connection.cursor() as cursor:
            sql = "SELECT SUM(Price) AS total_spending FROM Purchases JOIN UserPurchases ON Purchases.PurchaseID = UserPurchases.PurchaseID WHERE UserPurchases.UserID = %s AND Purchases.PurchaseDate BETWEEN %s AND %s"
            cursor.execute(sql, (UserID, start_date, now))
            result = cursor.fetchone()
            spending = result['total_spending'] if result['total_spending'] else 0
            return jsonify({'spending': spending})
    except Exception as e:
        return jsonify({'error': str(e)})

if __name__ == '__main__':
    app.run(debug=True)
