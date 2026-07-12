# ⚡ HelpWaiter

**HelpWaiter** is a lightweight, high-performance CQRS and Mediator library for .NET. Its core feature is the **strict separation of concerns between Read and Write operations**, ensuring queries and commands never get mixed up in the same pipeline.

## 👨‍💻 Author & Contact

Feel free to reach out, connect, or check out my other projects!

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/GabrielHBF) 
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/gabriel-hbf-dev)
---

## 📦 Installation

Add the package to your project via terminal:

```bash
dotnet add package HelpWaiter


using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Registers IWaiter and automatically scans for handlers
builder.Services.AddWaiter(Assembly.GetExecutingAssembly());

var app = builder.Build();
