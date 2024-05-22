-- Create the database
CREATE DATABASE TicketingSystem;

-- Use the database
USE TicketingSystem;

CREATE TABLE Tickets (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Destination NVARCHAR(100),
    PaymentMethod NVARCHAR(50),
    SeatQuantity INT,
    TotalPrice DECIMAL(10, 2)
);



