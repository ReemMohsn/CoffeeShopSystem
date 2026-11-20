<h1 align="center">☕ Happiness Cafe — Online Coffee Shop</h1>

<p align="center">
  An online coffee and sweets ordering web application with an administrative dashboard built with <b>ASP.NET Core MVC</b> — University Team Project.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Framework-ASP.NET%20Core%20MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core MVC" />
  <img src="https://img.shields.io/badge/Language-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Live%20Demo-Visit%20Site-C5A030?style=for-the-badge" alt="Live Demo" />
</p>

<p align="center">
  <a href="#about">About</a> ·
  <a href="#features">Features</a> ·
  <a href="#screenshots">Screenshots</a> ·
  <a href="#live-demo">Live Demo</a> ·
  <a href="#tech-stack">Tech Stack</a> ·
  <a href="#getting-started">Getting Started</a>
</p>

---

<a id="about"></a>
## ☕ About the Project

**Happiness Cafe (Online Coffee Shop)** is a web application built with **ASP.NET Core MVC** as a university team project. It allows customers to explore hot drinks, cold drinks, coffee blends, and sweets, read reviews, and place orders online from home, while providing staff with a dedicated administrative dashboard to manage products and customer orders.

<a id="features"></a>
## ✨ Key Features

- 🛍️ **Categorized Storefront & Menu** — Browse **Hot Drinks**, **Cold Drinks**, **Sweets**, and packaged **Coffee Products** with pricing, sizes, and ratings.
- 🛒 **Shopping Cart, Favorites & Online Ordering** — Enables customers to favorite items, add products to the cart, and submit orders online from home.
- 🔐 **User Authentication** — Sign In, Sign Up, and Forgot Password flows for customer and admin accounts.
- ⭐ **Customer Reviews & Contact Us** — Displays customer ratings and feedback alongside an interactive location map and inquiry form.
- 🛠️ **Admin Dashboard** — Dedicated administrative panel to add, update, and manage menu products and incoming customer orders.

<a id="screenshots"></a>
## 🖥️ Screenshots

A look at the Happiness Cafe web application interfaces:

<table>
  <tr>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185423.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185423.jpg" alt="Home Page" width="420" /></a>
      <br /><b>Home Page</b>
    </td>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185429.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185429.jpg" alt="Our Strength and About Us" width="420" /></a>
      <br /><b>Our Strength & About Us</b>
    </td>
  </tr>
  <tr>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185432.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185432.jpg" alt="Menu Categories" width="420" /></a>
      <br /><b>Menu Categories</b>
    </td>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185439.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185439.jpg" alt="Coffee Products" width="420" /></a>
      <br /><b>Coffee Products</b>
    </td>
  </tr>
  <tr>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185515.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185515.jpg" alt="Sweets Menu" width="420" /></a>
      <br /><b>Sweets Menu</b>
    </td>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185448.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185448.jpg" alt="Customer Reviews" width="420" /></a>
      <br /><b>Customer Reviews</b>
    </td>
  </tr>
  <tr>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185459.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185459.jpg" alt="Contact Us and Location Map" width="420" /></a>
      <br /><b>Contact Us & Map</b>
    </td>
    <td align="center" width="50%">
      <a href="screenshots/لقطة%20شاشة%202026-11-20%20185522.jpg"><img src="screenshots/لقطة%20شاشة%202026-11-20%20185522.jpg" alt="Sign In Page" width="420" /></a>
      <br /><b>Sign In</b>
    </td>
  </tr>
</table>

<a id="live-demo"></a>
## 🌐 Live Demo

The deployed web application is accessible at:
- **Live Website:** [happinesscaffee.runasp.net](http://happinesscaffee.runasp.net/)

<a id="tech-stack"></a>
## 🛠️ Tech Stack

| Purpose | Technology |
| :--- | :--- |
| **Back-End & Architecture** | ASP.NET Core MVC, C# |
| **Front-End** | Razor Views, HTML, CSS, JavaScript |
| **Database** | Microsoft SQL Server |
| **Hosting** | MonsterASP.NET (`runasp.net`) |

<a id="getting-started"></a>
## 🚀 Getting Started

### Prerequisites

- **.NET SDK** (matching the project's target framework in the `.csproj` file)
- **Visual Studio 2022** (or VS Code with C# Dev Kit)
- **SQL Server** (LocalDB or SQL Server Express)

### Run Locally

```bash
git clone [https://github.com/ReemMohsn/CoffeeShopSystem.git](https://github.com/ReemMohsn/CoffeeShopSystem.git)
cd CoffeeShopSystem
dotnet restore
```

1. Update the database connection string in `appsettings.json` to match your local SQL Server instance.
2. Apply any Entity Framework migrations (if used) via `dotnet ef database update`.
3. Run the application using `dotnet run` or by pressing **F5** in Visual Studio.

## 👩‍💻 Author

- **[Reem Mohsen](https://github.com/ReemMohsn)** — Web Developer (University Team Project)
