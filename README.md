# Project requirements

## Bitcoin Price Tracker Program

## Task Overview

Create a C# program using the .NET 4.5 or higher framework, or .NET Core, and an MSSQL database that contains two tabs:

### Live Data Tab
- Periodically fetches the current Bitcoin price from the Coindesk API:
  - [Coindesk API](https://api.coindesk.com/v1/bpi/currentprice.json)
- Calculates the Bitcoin price in CZK from the BTC/EUR price using the exchange rate obtained from the Czech National Bank (ČNB) API for currency exchange rates.
- Displays the fetched data in a grid.
- Contains a "Save Data" button, which saves the data to the database.

### Saved Data Tab
- Displays the saved data in a grid.
- The grid contains an editable "Note" column.
- The program includes a "Delete Selected Records" button.
- The program includes a "Save" button that saves the edited items.

### Solution Requirements:
- The solution should be uploaded to GitHub.
- The task includes a T-SQL script to create the necessary tables, stored procedures, etc.

## Bonus Task:
- Display the price trend of Bitcoin both in a table and a graph. You can use any charting library to display the graph.
