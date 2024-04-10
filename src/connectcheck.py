import pymysql.cursors

def check_mariadb_connection():
    try:
        connection = pymysql.connect(
            host='mariadb.vamk.fi',
            user='e2101098',
            password='cqgYeaFEN6A',
            db='e2101098_Windows',
            charset='utf8mb4',
            cursorclass=pymysql.cursors.DictCursor
        )
        print("MariaDB connection successful!")
    except pymysql.Error as e:
        print("Error connecting to MariaDB:", e)

if __name__ == "__main__":
    check_mariadb_connection()
