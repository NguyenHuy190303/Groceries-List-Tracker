# Groceries List Tracker

A comprehensive application for managing groceries, tracking prices, and analyzing budgets. 

## Core Functionalities

- **Item & Price Tracking**: Users can input items purchased and their prices. The system stores and organizes this data by date or time period (e.g., weekly, monthly).
  
- **Price Comparison**: The application visualizes price changes over time, highlighting increases and decreases. This could be through graphs, charts, or simple percentage differences.
  
- **Budget & Spending Analysis**: Allow users to set a grocery budget. The system tracks spending against the budget, providing alerts if it's exceeded.

## Backend

REST API provides a well-defined interface for the Windows application (possibly future web and mobile apps) to interact with the database.

### Language Considerations

- **Python (Flask or FastAPI)**: Popular choices for building REST APIs due to speed and ease of development.

- **Node.js (Express)**: Another strong option, particularly if JavaScript is preferred.

- **Java (Spring Boot)**: Robust framework well-suited for larger, enterprise-level applications.

### API Endpoints

Design endpoints for:

- Adding grocery items and prices
- Retrieving price history for an item
- Getting spending summaries (weekly/monthly)
- Setting/updating a budget

## Windows Application (C#)

### User Interface

- Design intuitive forms for adding grocery items and prices.
- Utilize data visualization libraries (like OxyPlot) to display price trends effectively.
- Provide clear spending summaries and budget progress visualizations.

### Communication with Backend

Utilize C#'s networking capabilities to make the necessary requests to the REST API.

## Database (MariaDB)

### Table Design

- **items**: 
  - `item_id INT AUTO_INCREMENT PRIMARY KEY`
  - `item_name VARCHAR(100) UNIQUE`
  - `category VARCHAR(50)`
  - `description TEXT`

- **purchases**: 
  - `purchase_id INT AUTO_INCREMENT PRIMARY KEY`
  - `item_id INT`
  - `price DECIMAL(10,2)`
  - `quantity INT`
  - `purchase_date DATE`
  - `FOREIGN KEY (item_id) REFERENCES items(item_id)`

- **users**: 
  - `user_id INT AUTO_INCREMENT PRIMARY KEY`
  - `username VARCHAR(50) NOT NULL`
  - `password VARCHAR(100) NOT NULL`
  - `email VARCHAR(100) UNIQUE`

- **user_purchases**: 
  - `user_id INT`
  - `purchase_id INT`
  - `PRIMARY KEY (user_id, purchase_id)`
  - `FOREIGN KEY (user_id) REFERENCES users(user_id)`
  - `FOREIGN KEY (purchase_id) REFERENCES purchases(purchase_id)`

## Additional Features (for Web/Mobile Expansion)

- **User Accounts**: Allow users to save their data and track trends over longer periods.
  
- **Recipe Integration**: Users could input recipes, and the system would generate a shopping list and estimated cost.
  
- **Store Price Comparisons**: Help users find the cheapest place to buy certain items (this might require data scraping or 3rd party API integration).
  
- **Alerts & Notifications**: Notify users of significant price increases or when they are approaching their budget limit.

### Considerations

- **Scalability**: If the project aims to serve a large user base, consider a cloud-hosted database (AWS Postgres, Azure PostgreSQL) for easier scaling.
  
- **Security**: Implement user authentication and data encryption, especially if handling sensitive financial information.
  
- **Usability**: Focus on an intuitive and user-friendly interface for the Windows application. This is crucial for user adoption.

## Author Information

- **Name**: Nguyen Huy
- **ID**: e2101098

---
Feel free to reach out for any inquiries or collaborations!
