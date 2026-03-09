-- Restaurant Management System Database Setup Script
-- Run this script in phpMyAdmin to create the database and tables

-- Create database (if not exists)
CREATE DATABASE IF NOT EXISTS `restaurant_db`;
USE `restaurant_db`;

-- Categories Table
CREATE TABLE IF NOT EXISTS `Categories` (
    `CategoryID` INT AUTO_INCREMENT PRIMARY KEY,
    `CategoryName` VARCHAR(100) NOT NULL,
    UNIQUE KEY `idx_category_name` (`CategoryName`)
);

-- MenuItems Table  
CREATE TABLE IF NOT EXISTS `MenuItems` (
    `ItemID` INT AUTO_INCREMENT PRIMARY KEY,
    `ItemName` VARCHAR(200) NOT NULL,
    `CategoryID` INT NOT NULL,
    `Price` DECIMAL(10,2) NOT NULL,
    `Description` VARCHAR(500) NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Available',
    FOREIGN KEY (`CategoryID`) REFERENCES `Categories`(`CategoryID`)
);

-- Tables Table
CREATE TABLE IF NOT EXISTS `Tables` (
    `TableID` INT AUTO_INCREMENT PRIMARY KEY,
    `TableNumber` VARCHAR(10) NOT NULL,
    `Capacity` INT NOT NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Available',
    UNIQUE KEY `idx_table_number` (`TableNumber`)
);

-- Orders Table
CREATE TABLE IF NOT EXISTS `Orders` (
    `OrderID` INT AUTO_INCREMENT PRIMARY KEY,
    `TableID` INT NOT NULL,
    `OrderDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `OrderStatus` VARCHAR(20) NOT NULL DEFAULT 'Pending',
    `TotalAmount` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    FOREIGN KEY (`TableID`) REFERENCES `Tables`(`TableID`)
);

-- OrderItems Table
CREATE TABLE IF NOT EXISTS `OrderItems` (
    `OrderItemID` INT AUTO_INCREMENT PRIMARY KEY,
    `OrderID` INT NOT NULL,
    `ItemID` INT NOT NULL,
    `Quantity` INT NOT NULL DEFAULT 1,
    `Price` DECIMAL(10,2) NOT NULL,
    `SubTotal` DECIMAL(10,2) NOT NULL,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`OrderID`) REFERENCES `Orders`(`OrderID`) ON DELETE CASCADE,
    FOREIGN KEY (`ItemID`) REFERENCES `MenuItems`(`ItemID`)
);

-- Bills Table
CREATE TABLE IF NOT EXISTS `Bills` (
    `BillID` INT AUTO_INCREMENT PRIMARY KEY,
    `OrderID` INT NOT NULL UNIQUE,
    `BillDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `SubTotal` DECIMAL(10,2) NOT NULL,
    `TaxAmount` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `FinalAmount` DECIMAL(10,2) NOT NULL,
    `PaymentMethod` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Pending',
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`OrderID`) REFERENCES `Orders`(`OrderID`)
);

-- Reset auto-increment values
ALTER TABLE Categories AUTO_INCREMENT = 1;
ALTER TABLE Tables AUTO_INCREMENT = 1;
ALTER TABLE MenuItems AUTO_INCREMENT = 1;
ALTER TABLE Orders AUTO_INCREMENT = 1;
ALTER TABLE OrderItems AUTO_INCREMENT = 1;
ALTER TABLE Bills AUTO_INCREMENT = 1;

-- Insert Vegetarian Categories (3 categories)
INSERT INTO Categories (CategoryName, Description) VALUES
('Main Course', 'Hearty vegetarian main dishes like curries, biryani, and dal'),
('Appetizers', 'Light vegetarian starters and snacks like samosa, pakora, and spring rolls'),
('Beverages', 'Refreshing vegetarian drinks including lassi, chai, and fresh juices');

-- Insert Tables (10 tables with mixed status)
INSERT INTO Tables (TableNumber, Capacity, Status) VALUES
('T1', 4, 'Available'),
('T2', 4, 'Available'),
('T3', 2, 'Occupied'),
('T4', 6, 'Available'),
('T5', 4, 'Available'),
('T6', 2, 'Available'),
('T7', 8, 'Available'),
('T8', 4, 'Occupied'),
('T9', 4, 'Available'),
('T10', 6, 'Available');

-- Insert Main Course Vegetarian Items (4 items)
INSERT INTO MenuItems (CategoryID, ItemName, Description, Price, ImageUrl, Status) VALUES
(1, 'Paneer Butter Masala', 'Soft cottage cheese cubes cooked in rich buttery tomato gravy with cream and aromatic spices', 250.00, '/images/paneer_butter_masala.jpg', 'Available'),
(1, 'Dal Makhani', 'Creamy black lentils slow-cooked overnight with butter, ginger, and aromatic spices', 180.00, '/images/dal_makhani.jpg', 'Available'),
(1, 'Vegetable Biryani', 'Fragrant basmati rice cooked with mixed vegetables, herbs, and saffron', 200.00, '/images/veg_biryani.jpg', 'Available'),
(1, 'Malai Kofta', 'Soft cottage cheese and vegetable dumplings in rich creamy gravy', 220.00, '/images/malai_kofta.jpg', 'Available');

-- Insert Appetizers (3 items)
INSERT INTO MenuItems (CategoryID, ItemName, Description, Price, ImageUrl, Status) VALUES
(2, 'Vegetable Samosa', 'Crispy triangular pastries filled with spiced potatoes, peas, and herbs', 60.00, '/images/veg_samosa.jpg', 'Available'),
(2, 'Paneer Tikka', 'Marinated cottage cheese cubes grilled in traditional tandoor with yogurt and spices', 150.00, '/images/paneer_tikka.jpg', 'Available'),
(2, 'Aloo Tikki', 'Spiced potato patties shallow fried and served with mint chutney', 80.00, '/images/aloo_tikki.jpg', 'Available');

-- Insert Beverages (4 items)
INSERT INTO MenuItems (CategoryID, ItemName, Description, Price, ImageUrl, Status) VALUES
(3, 'Fresh Lime Soda', 'Carbonated lime soda with fresh mint leaves and a hint of salt', 40.00, '/images/lime_soda.jpg', 'Available'),
(3, 'Mango Lassi', 'Sweet and creamy yogurt drink made with ripe mango pulp and cardamom', 50.00, '/images/mango_lassi.jpg', 'Available'),
(3, 'Masala Chai', 'Traditional Indian spiced tea made with milk, ginger, and aromatic spices', 30.00, '/images/masala_chai.jpg', 'Available'),
(3, 'Fresh Orange Juice', 'Freshly squeezed orange juice with pulp, no added sugar', 45.00, '/images/orange_juice.jpg', 'Available');

-- Insert Sample Orders (2 orders for testing)
INSERT INTO Orders (TableID, OrderDate, OrderStatus, TotalAmount) VALUES
(3, '2024-03-09 12:30:00', 'Completed', 250.00),
(8, '2024-03-09 13:45:00', 'Pending', 180.00);

-- Insert Sample OrderItems (4 items for testing)
INSERT INTO OrderItems (OrderID, ItemID, Quantity, Price, SubTotal) VALUES
(1, 1, 1, 250.00, 250.00),
(2, 4, 1, 60.00, 60.00),
(2, 5, 2, 30.00, 60.00),
(2, 7, 1, 50.00, 50.00);

-- Insert Sample Bills (1 bill for testing)
INSERT INTO Bills (OrderID, BillDate, SubTotal, TaxAmount, FinalAmount, PaymentMethod, Status) VALUES
(1, '2024-03-09 14:00:00', 250.00, 12.50, 262.50, 'Cash', 'Paid');

-- Success message
SELECT 'Database setup completed successfully with vegetarian data!' AS Message;
