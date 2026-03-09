# 🏗️ Restaurant Management System - Project Structure & Architecture

## 📋 Table of Contents

- [System Overview](#system-overview)
- [Project Structure](#project-structure)
- [Architecture Patterns](#architecture-patterns)
- [Core Components](#core-components)
- [Database Design](#database-design)
- [Order Management System](#order-management-system)
- [Table Occupancy Logic](#table-occupancy-logic)
- [Security Features](#security-features)
- [Performance Optimizations](#performance-optimizations)
- [How to Run the Project](#how-to-run-the-project)
- [How to Maintain or Extend](#how-to-maintain-or-extend)

## 🎯 System Overview

The Restaurant Management System is a comprehensive ASP.NET Core MVC application designed to streamline restaurant operations including order management, billing, menu management, and table tracking.

### **Key Features:**
- 🍽️ **Menu Management** - Categories and menu items with availability status
- 🪑 **Table Management** - Real-time table status tracking
- 📝 **Order Management** - Complete order lifecycle with status tracking
- 💰 **Billing System** - Automated bill generation with tax calculations
- 📊 **Admin Dashboard** - Real-time statistics and monitoring
- 🎨 **Modern UI** - Bootstrap 5 responsive design

### **Technology Stack:**
- **Backend:** ASP.NET Core 6.0 MVC
- **Database:** MySQL 8.0+ with Entity Framework Core
- **Frontend:** Bootstrap 5, HTML5, CSS3, JavaScript
- **Architecture:** MVC Pattern with Repository Pattern
- **Security:** CSRF Protection, Input Validation, SQL Injection Prevention

## 🏗️ Project Structure

```
RestaurantManagementSystem/
├── 📁 Controllers/                    # HTTP Request Handlers
│   ├── AdminController.cs           # Dashboard & admin operations
│   ├── BillingController.cs         # Bill generation & management
│   ├── CategoryController.cs        # Category CRUD operations
│   ├── HomeController.cs            # Default/home page
│   ├── MenuController.cs            # Menu item CRUD operations
│   ├── OrderController.cs           # Order lifecycle management
│   └── TableController.cs           # Table management
├── 📁 Models/                        # Data Models & Entities
│   ├── Bill.cs                      # Bill entity
│   ├── Category.cs                  # Category entity
│   ├── DashboardViewModel.cs        # Dashboard data model
│   ├── MenuItem.cs                  # Menu item entity
│   ├── Order.cs                     # Order entity
│   ├── OrderItem.cs                 # Order line items
│   ├── OrderViewModel.cs            # Order creation view model
│   └── Table.cs                     # Table entity
├── 📁 Views/                         # Razor Views (UI)
│   ├── Admin/                       # Admin dashboard views
│   │   ├── Index.cshtml             # Main dashboard
│   │   └── Dashboard.cshtml         # Statistics dashboard
│   ├── Billing/                     # Billing views
│   │   ├── Index.cshtml             # Bills list
│   │   ├── Details.cshtml           # Bill details
│   │   ├── GenerateBill.cshtml      # Bill generation
│   │   └── PrintBill.cshtml         # Printable bill
│   ├── Category/                    # Category management views
│   ├── Menu/                        # Menu item management views
│   ├── Order/                       # Order management views
│   ├── Shared/                      # Shared components
│   │   ├── _Layout.cshtml           # Main layout
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml             # Custom error page
│   └── Table/                       # Table management views
├── 📁 Data/                         # Database Context
│   └── ApplicationDbContext.cs      # EF Core database context
├── 📁 Filters/                       # Custom Filters
│   └── GlobalExceptionFilter.cs     # Global error handling
├── 📁 wwwroot/                      # Static Files
│   ├── css/                         # Stylesheets
│   ├── js/                          # JavaScript files
│   └── images/                      # Static images
├── 📄 Program.cs                    # Application entry point
├── 📄 appsettings.json              # Configuration settings
├── 📄 RestaurantManagementSystem.csproj  # Project file
└── 📄 README.md                     # Project documentation
```

## 🏛️ Architecture Patterns

### **MVC Pattern (Model-View-Controller)**
- **Models:** Represent data entities and business logic
- **Views:** Handle UI presentation and user interaction
- **Controllers:** Process HTTP requests and coordinate between models/views

### **Repository Pattern (Implicit)**
- `ApplicationDbContext` acts as a repository
- Centralized data access through Entity Framework Core
- Separation of data access from business logic

### **Dependency Injection**
- Services registered in `Program.cs`
- Controllers receive dependencies via constructor injection
- Promotes loose coupling and testability

## 🧩 Core Components

### **1. Controllers Layer**
**Purpose:** Handle HTTP requests, validate input, orchestrate business logic

**Key Controllers:**
- **OrderController:** Manages order lifecycle, table status updates
- **BillingController:** Handles bill generation and printing
- **AdminController:** Provides dashboard statistics and admin functions
- **TableController:** Manages restaurant tables and availability

### **2. Models Layer**
**Purpose:** Define data structures, business rules, and relationships

**Key Models:**
- **Order:** Represents customer orders with status tracking
- **Table:** Restaurant tables with capacity and status
- **MenuItem:** Menu items with pricing and availability
- **Bill:** Generated bills with payment information

### **3. Views Layer**
**Purpose:** Render HTML, handle user interaction, display data

**Key Views:**
- **Dashboard:** Real-time statistics and quick actions
- **Order/Create:** Interactive order creation with menu selection
- **Billing/Details:** Comprehensive bill information display

## 🗄️ Database Design

### **Entity Relationships**
```
Categories (1) → (Many) MenuItems
Tables (1) → (Many) Orders
Orders (1) → (Many) OrderItems
Orders (1) → (1) Bills
MenuItems (1) → (Many) OrderItems
```

### **Key Tables**

#### **Orders Table**
```sql
OrderID (PK) | TableID (FK) | OrderDate | OrderStatus | TotalAmount
------------|-------------|-----------|-------------|------------
1           | 3           | 2024-01-15| Pending     | 250.00
2           | 5           | 2024-01-15| Completed   | 180.00
```

#### **Tables Table**
```sql
TableID (PK) | TableNumber | Capacity | Status
------------|-------------|----------|--------
1           | T1          | 4        | Available
2           | T2          | 4        | Occupied
```

#### **MenuItems Table**
```sql
ItemID (PK) | CategoryID (FK) | ItemName | Price | Status
------------|----------------|----------|-------|--------
1           | 1              | Paneer   | 250   | Available
2           | 2              | Samosa   | 60    | Available
```

## 📋 Order Management System

### **Order Lifecycle**

#### **1. Order Creation**
```
User selects table → Validates availability → Adds menu items → Calculates total → Submits order
```

**Key Validations:**
- Table must be "Available"
- At least one menu item must be selected
- Quantities must be positive integers

#### **2. Order Status Flow**
```
Pending → In Progress → Completed
    ↓           ↓
  Cancelled  ←  ──────
```

**Status Meanings:**
- **Pending:** Order created, waiting for processing
- **In Progress:** Order being prepared
- **Completed:** Order finished, bill generated
- **Cancelled:** Order cancelled, table freed

#### **3. Order Operations**
- **Create:** New order with table and menu items
- **Edit:** Modify order details, status, or table
- **Cancel:** Cancel order and free table
- **Delete:** Remove order completely

### **Business Rules**
1. **Table Uniqueness:** Only one active order per table
2. **Item Availability:** Only "Available" menu items can be ordered
3. **Status Transitions:** Follow defined status flow
4. **Amount Calculation:** Automatic total calculation from items

## 🪑 Table Occupancy Logic

### **Table Status Management**

#### **Status Types:**
- **Available:** Table is free for new orders
- **Occupied:** Table has an active order
- **Reserved:** Table is reserved (future enhancement)

#### **Status Transitions**

**Order Creation:**
```csharp
// When order is created
table.Status = "Occupied";
```

**Order Completion/Cancellation:**
```csharp
// When order is completed or cancelled
table.Status = "Available";
```

### **Validation Logic**

#### **Table Availability Check**
```csharp
var table = await _context.Tables.FindAsync(order.TableID);
if (table == null || table.Status != "Available")
{
    ModelState.AddModelError("", "Selected table is not available.");
    return View(viewModel);
}
```

#### **Concurrent Order Prevention**
- Database constraints prevent duplicate orders
- Status validation before order creation
- Real-time status updates

### **Automatic Status Updates**

#### **Order Events → Table Status**
1. **Order Created:** Table → "Occupied"
2. **Order Completed:** Table → "Available"
3. **Order Cancelled:** Table → "Available"
4. **Order Deleted:** Table → "Available"

## 🔒 Security Features

### **Input Validation**
- Required field validation
- Range validation for quantities
- String length limits
- Data type validation

### **CSRF Protection**
```csharp
[ValidateAntiForgeryToken]
// Applied to all POST actions
```

### **SQL Injection Prevention**
- Entity Framework Core parameterized queries
- No raw SQL concatenation
- Input sanitization

### **Error Handling**
- Global exception filter
- User-friendly error messages
- No stack traces exposed
- Proper logging

### **Security Headers**
- HTTPS redirection
- Secure cookie settings
- SameSite cookie protection

## ⚡ Performance Optimizations

### **Database Optimizations**
```csharp
// Connection retry mechanism
mysqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), ...)

// Eager loading to prevent N+1 queries
.Include(o => o.Table)
.Include(o => o.OrderItems)
.ThenInclude(oi => oi.MenuItem)
```

### **Response Compression**
```csharp
// Gzip compression enabled
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});
```

### **Session Management**
```csharp
// Optimized session settings
options.IdleTimeout = TimeSpan.FromMinutes(30);
options.Cookie.HttpOnly = true;
options.Cookie.IsEssential = true;
```

### **Static File Caching**
- Versioned CSS/JS files
- Browser caching headers
- CDN for external resources

## 🚀 How to Run the Project

### **Prerequisites**
- .NET 6.0 SDK or later
- MySQL Server 8.0+ (XAMPP recommended)
- Visual Studio 2022 or VS Code

### **Step-by-Step Setup**

#### **1. Database Setup**
```bash
# Start XAMPP (Apache + MySQL)
# Open phpMyAdmin: http://localhost/phpmyadmin
# Create database: restaurant_db
# Import: database_setup.sql
```

#### **2. Application Setup**
```bash
# Navigate to project directory
cd "RestaurantManagementSystem"

# Restore dependencies
dotnet restore

# Build application
dotnet build

# Run application
dotnet run
```

#### **3. Access Application**
- **URL:** http://localhost:5000
- **Default Route:** Admin Dashboard
- **Database:** MySQL on localhost:3306

### **Configuration**

#### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;password=;database=restaurant_db;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## 🔧 How to Maintain or Extend

### **Adding New Features**

#### **1. New Entity Example**
```csharp
// Create Model
public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
}

// Add to DbContext
public DbSet<Customer> Customers { get; set; }

// Create Controller
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;
    public CustomerController(ApplicationDbContext context) => _context = context;
    
    public async Task<IActionResult> Index() => View(await _context.Customers.ToListAsync());
}
```

#### **2. Adding New Views**
```
Views/
├── Customer/
│   ├── Index.cshtml      # List view
│   ├── Create.cshtml     # Create form
│   ├── Edit.cshtml       # Edit form
│   └── Details.cshtml    # Details view
```

#### **3. Database Migration**
```bash
# Add migration
dotnet ef migrations add AddCustomerEntity

# Apply migration
dotnet ef database update
```

### **Code Quality Standards**

#### **1. Naming Conventions**
- **Controllers:** `[Name]Controller.cs`
- **Models:** PascalCase (e.g., `OrderItem`)
- **Views:** Same as action name (e.g., `Index.cshtml`)
- **Database:** PascalCase with plural table names

#### **2. Error Handling**
```csharp
try
{
    // Database operation
    await _context.SaveChangesAsync();
    TempData["Success"] = "Operation completed successfully!";
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    ModelState.AddModelError("", "Something went wrong. Please try again.");
}
```

#### **3. Validation Patterns**
```csharp
// Model validation
[Required(ErrorMessage = "Field is required")]
[StringLength(100, ErrorMessage = "Maximum 100 characters")]
public string Name { get; set; }

// Controller validation
if (!ModelState.IsValid)
{
    return View(model);
}
```

### **Testing Guidelines**

#### **1. Unit Testing**
```csharp
[Test]
public async Task CreateOrder_ValidData_ReturnsRedirect()
{
    // Arrange
    var order = new Order { TableID = 1, /* other properties */ };
    
    // Act
    var result = await _controller.Create(order);
    
    // Assert
    Assert.IsInstanceOf<RedirectToActionResult>(result);
}
```

#### **2. Integration Testing**
```csharp
[Test]
public async Task OrderFlow_CompleteWorkflow_Success()
{
    // Test complete order flow
    // Create order → Add items → Generate bill → Complete
}
```

### **Deployment Considerations**

#### **1. Production Configuration**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-server;Database=restaurant_db;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

#### **2. Environment Variables**
```bash
# Set environment variables
set ASPNETCORE_ENVIRONMENT=Production
set ConnectionStrings__DefaultConnection=server=prod;database=db;
```

### **Performance Monitoring**

#### **1. Logging**
```csharp
_logger.LogInformation("Order {OrderId} created for table {TableId}", order.OrderID, order.TableID);
```

#### **2. Metrics**
- Response times
- Database query performance
- Memory usage
- Error rates

### **Future Enhancements**

#### **Planned Features**
1. **User Authentication:** Login system with role-based access
2. **Inventory Management:** Track ingredient stock
3. **Customer Management:** Customer database and loyalty program
4. **Online Ordering:** Web-based ordering for customers
5. **Mobile App:** Native mobile application
6. **Payment Integration:** Online payment gateways
7. **Reporting System:** Advanced analytics and reports
8. **Multi-Restaurant:** Support for multiple restaurant locations

#### **Technical Improvements**
1. **Microservices Architecture:** Split into smaller services
2. **Caching Layer:** Redis for performance
3. **Message Queue:** RabbitMQ for async processing
4. **API Documentation:** Swagger/OpenAPI integration
5. **Automated Testing:** CI/CD pipeline setup

---

## 📞 Support & Maintenance

### **Common Issues**
1. **Database Connection:** Check MySQL service and connection string
2. **Port Conflicts:** Kill existing processes or change port
3. **Build Errors:** Clean and rebuild solution
4. **Runtime Errors:** Check logs and validate data

### **Debugging Tips**
1. Use browser developer tools (F12)
2. Check application logs
3. Verify database state in phpMyAdmin
4. Test with sample data

### **Backup Strategy**
1. **Database Backups:** Regular MySQL dumps
2. **Code Backups:** Git version control
3. **Configuration Backups:** Save appsettings.json

---

**🎉 This Restaurant Management System provides a solid foundation for restaurant operations with room for future enhancements and scalability!**
