# 🍽️ Restaurant Order & Billing Management System

A comprehensive ASP.NET Core MVC application for managing restaurant operations including orders, billing, menu management, and table occupancy tracking.

## 🌟 **Features**

### 🎯 **Core Functionality**
- 📋 **Order Management** - Complete order lifecycle with status tracking
- 💰 **Billing System** - Automated bill generation with tax calculations
- 🍽 **Menu Management** - Vegetarian menu items with categories
- 🪑 **Table Management** - Automatic status updates based on orders
- 📊 **Dashboard** - Real-time statistics and analytics
- 🔒 **Security** - CSRF protection and input validation
- � **Performance** - Response compression and optimized queries

### 🎨 **UI/UX Highlights**
- 📱 **Responsive Design** - Bootstrap 5 mobile-friendly interface
- 🇮🇳 **Currency Support** - Indian Rupee (₹) formatting throughout
- 🎨 **Professional Theme** - Modern teal and purple color scheme
- ⚡ **Interactive Forms** - Dynamic quantity updates and calculations
- 📈 **Status Indicators** - Color-coded badges and icons
- 🔔 **Notifications** - Success/error messages with TempData

### �️ **Technical Stack**
- **Backend:** ASP.NET Core 6.0 MVC
- **Database:** MySQL with Entity Framework Core (Pomelo provider)
- **Frontend:** Bootstrap 5, Font Awesome 6, jQuery
- **Architecture:** Repository Pattern with Dependency Injection
- **Error Handling:** Global Exception Filter with logging
- **Security:** Anti-forgery tokens and input validation

## 🚀 **Quick Start**

### 📋 **Prerequisites**
- **.NET 6.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/6.0)
- **MySQL Server** - [XAMPP/WAMP/MAMP](https://www.apachefriends.org/) or standalone
- **Visual Studio 2022** - Recommended IDE (optional)

### ⚡ **Installation & Setup**

#### **1. Clone Repository**
```bash
git clone https://github.com/yourusername/restaurant-order-billing-system.git
cd restaurant-order-billing-system
```

#### **2. Database Setup**
```bash
# Option A: XAMPP Users
1. Start XAMPP Control Panel
2. Start Apache and MySQL services
3. Open phpMyAdmin: http://localhost/phpmyadmin
4. Import `database_setup.sql` file

# Option B: Manual MySQL
1. Install MySQL Server
2. Create database: CREATE DATABASE restaurant_db;
3. Import `database_setup.sql` file
```

#### **3. Application Configuration**
```bash
# Update connection string in appsettings.json
2. Find pending order
3. Click "Generate Bill"
4. Select payment method
5. Generate bill
6. Order status: "Completed"
7. View/Print bill details
```

### 4. **Table Management Workflow**
```
1. Go to Table Management
2. Add restaurant tables with capacity
3. Tables show: Available/Occupied status
4. Status updates automatically with orders
```

### 5. **Dashboard Monitoring**
```
1. Admin Dashboard shows real-time stats
2. Monitor: Total Menu Items, Orders, Sales
3. Track Today's performance
4. View Recent Orders status
5. Monitor Table availability
```

## 🏗️ Project Structure

```
RestaurantManagementSystem/
├── Controllers/
│   ├── AdminController.cs          # Admin dashboard & stats
│   ├── OrderController.cs         # Order management
│   ├── BillingController.cs       # Billing operations
│   ├── MenuController.cs          # Menu & category management
│   └── TableController.cs         # Table management
├── Models/
│   ├── Order.cs                   # Order entity
│   ├── MenuItem.cs                # Menu item entity
│   ├── Category.cs                # Category entity
│   ├── Table.cs                   # Table entity
│   ├── Bill.cs                    # Bill entity
│   └── OrderItem.cs               # Order item entity
├── Views/
│   ├── Admin/                     # Admin dashboard views
│   ├── Order/                     # Order management views
│   ├── Billing/                   # Billing views
│   ├── Menu/                      # Menu management views
│   ├── Table/                     # Table management views
│   └── Shared/                    # Shared layouts & components
├── wwwroot/
│   ├── css/                       # Stylesheets
│   ├── js/                        # JavaScript files
│   └── images/                    # Static images
├── Data/
│   └── RestaurantDbContext.cs     # Database context
├── Program.cs                     # Application entry point
├── appsettings.json               # Configuration settings
└── RestaurantManagementSystem.csproj
```

## ⚙️ Configuration

### Database Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;password=;database=restaurant_db;"
  }
}
```

### Currency Settings
- Default currency: Indian Rupee (₹)
- Format: ₹1,234.56
- Tax rate: Configured in billing calculations

### Application Settings
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## 🐛 Troubleshooting

### Common Issues & Solutions

#### 1. **Build Error - File Locked**
**Problem:** "The process cannot access the file because it is being used by another process"
**Solution:** 
- Stop the running application: `taskkill /F /PID <process-id>`
- Or close the command prompt window
- Then rebuild: `dotnet build`

#### 2. **Database Connection Error**
**Problem:** "Unable to connect to MySQL server"
**Solution:**
- Ensure XAMPP MySQL service is running
- Check connection string in `appsettings.json`
- Verify database name exists in phpMyAdmin

#### 3. **Port 5000 Already in Use**
**Problem:** "Address already in use"
**Solution:**
- Find process: `netstat -ano | findstr :5000`
- Kill process: `taskkill /F /PID <process-id>`
- Or use different port: `dotnet run --urls="http://localhost:5001"`

#### 4. **Missing Menu Items**
**Problem:** "No menu items available" when creating order
**Solution:**
- Import database setup SQL file
- Add menu items through Menu Management
- Ensure items have "Available" status

#### 5. **Build Warnings**
**Problem:** "Target framework 'net6.0' is out of support"
**Solution:** This is just a warning, application will work fine
- Can ignore or upgrade to .NET 7/8 if desired

### Getting Help

1. **Check logs** in command window for error details
2. **Verify database** connection and data
3. **Ensure all prerequisites** are installed
4. **Restart services** (XAMPP, application)

## 📞 Support

For technical support or questions:
1. Check this README first
2. Review error logs in console
3. Verify database setup
4. Ensure all steps in workflow are followed

---

## 🎉 Quick Start Summary

```bash
# 1. Start XAMPP (Apache + MySQL)
# 2. Import database in phpMyAdmin
# 3. Run application
cd "d:\Restaurant Order & Billing Management System\RestaurantManagementSystem"
dotnet run
# 4. Open browser: http://localhost:5000
# 5. Start managing your restaurant!
