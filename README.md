# 🪙 Meta Exchange BTC Engine (C# .NET)

A smart .NET app that simulates a **meta-exchange** for Bitcoin (BTC) trading. It reads multiple order books from crypto exchanges and gives you the **best execution plan** (buying or selling), all while respecting EUR/BTC balances.

---

## 🚀 Features

- 🛒 Buy BTC at the lowest possible price
- 💰 Sell BTC at the highest possible price
- 📊 Uses real-like order book data from JSON
- ♻️ Multi-exchange aggregation
- ✅ Console & Web modes
- 🧪 Unit tested with NSubstitute
- 📄 Swagger UI enabled (Web Mode)

---

MetaExchange.Console/
│
├── Program.cs                  # Entry point
├── Engine/                     # Buy/Sell engines
├── Models/                     # OrderBook, Bid, Ask, etc.
├── Requests/Responses/         # DTOs for API and console
├── FileReader/                 # Reads orderbooks.json
├── CommandHandlers/           # Console/Web/Help handlers
├── Resources/Path/            # ✅ orderbooks.json lives here
└── Tests/                     # Unit tests with NSubstitute



## 🛠️ Setup Instructions

### Prerequisites:
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Git (optional)
- Visual Studio / VS Code / CLI

---

### 🚀 Running the App

## Web Mode
dotnet run -- -web

http://localhost:5000/swagger

{
  "amount": 5,
  "orderType": "Buy"
}


## Console Mode 
dotnet run -- -console Buy 5