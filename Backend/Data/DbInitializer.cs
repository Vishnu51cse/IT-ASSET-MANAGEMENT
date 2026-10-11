using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Users.AnyAsync())
            {
                return; // DB already seeded
            }

            // 1. Categories
            var categories = new List<AssetCategory>
            {
                new AssetCategory { Name = "Laptop", Description = "Company laptops and mobile workstations", IsActive = true },
                new AssetCategory { Name = "Desktop", Description = "Desktop PCs and workstation towers", IsActive = true },
                new AssetCategory { Name = "Monitor", Description = "External computer displays and monitors", IsActive = true },
                new AssetCategory { Name = "Mobile Phone", Description = "Corporate mobile phones and smartphones", IsActive = true },
                new AssetCategory { Name = "Keyboard", Description = "Wired and wireless keyboards", IsActive = true },
                new AssetCategory { Name = "Mouse", Description = "Ergonomic and standard computer mice", IsActive = true },
                new AssetCategory { Name = "Printer", Description = "Office laser and inkjet printers", IsActive = true },
                new AssetCategory { Name = "Software License", Description = "SaaS and desktop software licenses", IsActive = true }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // 2. Employees
            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeCode = "EMP-001",
                    Name = "John Doe",
                    Email = "employee@company.com",
                    Department = "Engineering",
                    Designation = "Senior Software Engineer",
                    Phone = "+1-555-0101",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddMonths(-6)
                },
                new Employee
                {
                    EmployeeCode = "EMP-002",
                    Name = "Jane Smith",
                    Email = "janesmith@company.com",
                    Department = "Human Resources",
                    Designation = "HR Manager",
                    Phone = "+1-555-0102",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddMonths(-5)
                },
                new Employee
                {
                    EmployeeCode = "EMP-003",
                    Name = "Robert Johnson",
                    Email = "robert.j@company.com",
                    Department = "Finance",
                    Designation = "Financial Analyst",
                    Phone = "+1-555-0103",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddMonths(-4)
                },
                new Employee
                {
                    EmployeeCode = "EMP-004",
                    Name = "Emily Davis",
                    Email = "emily.davis@company.com",
                    Department = "Product",
                    Designation = "Product Manager",
                    Phone = "+1-555-0104",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddMonths(-3)
                }
            };
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // 3. Users
            string passwordHash = PasswordHasher.HashPassword("Password123!");

            var users = new List<User>
            {
                new User
                {
                    Email = "admin@company.com",
                    PasswordHash = passwordHash,
                    Role = "Admin",
                    EmployeeId = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new User
                {
                    Email = "employee@company.com",
                    PasswordHash = passwordHash,
                    Role = "Employee",
                    EmployeeId = employees[0].Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new User
                {
                    Email = "janesmith@company.com",
                    PasswordHash = passwordHash,
                    Role = "Employee",
                    EmployeeId = employees[1].Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // 4. Assets
            var laptopCat = categories.First(c => c.Name == "Laptop").Id;
            var monitorCat = categories.First(c => c.Name == "Monitor").Id;
            var mobileCat = categories.First(c => c.Name == "Mobile Phone").Id;
            var mouseCat = categories.First(c => c.Name == "Mouse").Id;
            var printerCat = categories.First(c => c.Name == "Printer").Id;
            var licenseCat = categories.First(c => c.Name == "Software License").Id;

            var assets = new List<Asset>
            {
                new Asset
                {
                    AssetTag = "AST-1001",
                    AssetName = "Dell XPS 15 9530",
                    CategoryId = laptopCat,
                    Brand = "Dell",
                    Model = "XPS 15",
                    SerialNumber = "SN-DLL-998811",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-12),
                    PurchaseCost = 1899.99m,
                    WarrantyExpiry = DateTime.UtcNow.AddYears(2),
                    Status = "Assigned",
                    Location = "Building A - Floor 3",
                    Description = "Intel Core i7, 32GB RAM, 1TB SSD"
                },
                new Asset
                {
                    AssetTag = "AST-1002",
                    AssetName = "ThinkPad X1 Carbon Gen 11",
                    CategoryId = laptopCat,
                    Brand = "Lenovo",
                    Model = "X1 Carbon Gen 11",
                    SerialNumber = "SN-LNV-445522",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-4),
                    PurchaseCost = 1650.00m,
                    WarrantyExpiry = DateTime.UtcNow.AddYears(2),
                    Status = "Available",
                    Location = "IT Storage Cabinet B",
                    Description = "Intel Core i7, 16GB RAM, 512GB SSD"
                },
                new Asset
                {
                    AssetTag = "AST-1003",
                    AssetName = "LG UltraFine 27-Inch 4K Monitor",
                    CategoryId = monitorCat,
                    Brand = "LG",
                    Model = "27UK850-W",
                    SerialNumber = "SN-LG-887733",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-10),
                    PurchaseCost = 449.50m,
                    WarrantyExpiry = DateTime.UtcNow.AddYears(1),
                    Status = "Assigned",
                    Location = "Building A - Floor 3",
                    Description = "27 inch 4K UHD IPS Display with USB-C"
                },
                new Asset
                {
                    AssetTag = "AST-1004",
                    AssetName = "iPhone 14 Pro 256GB",
                    CategoryId = mobileCat,
                    Brand = "Apple",
                    Model = "iPhone 14 Pro",
                    SerialNumber = "SN-APL-112233",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-8),
                    PurchaseCost = 1099.00m,
                    WarrantyExpiry = DateTime.UtcNow.AddMonths(4),
                    Status = "UnderMaintenance",
                    Location = "IT Repair Lab",
                    Description = "Space Black executive testing device"
                },
                new Asset
                {
                    AssetTag = "AST-1005",
                    AssetName = "Logitech MX Master 3S",
                    CategoryId = mouseCat,
                    Brand = "Logitech",
                    Model = "MX Master 3S",
                    SerialNumber = "SN-LOG-665544",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-2),
                    PurchaseCost = 99.99m,
                    WarrantyExpiry = DateTime.UtcNow.AddYears(1),
                    Status = "Available",
                    Location = "IT Desk Supply",
                    Description = "Performance Wireless Mouse"
                },
                new Asset
                {
                    AssetTag = "AST-1006",
                    AssetName = "HP LaserJet Pro MFP M428fdw",
                    CategoryId = printerCat,
                    Brand = "HP",
                    Model = "M428fdw",
                    SerialNumber = "SN-HP-334455",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-18),
                    PurchaseCost = 429.00m,
                    WarrantyExpiry = DateTime.UtcNow.AddMonths(-6),
                    Status = "Available",
                    Location = "Floor 2 Copy Room",
                    Description = "Multifunction Wireless Laser Printer"
                },
                new Asset
                {
                    AssetTag = "AST-1007",
                    AssetName = "Microsoft 365 E5 Enterprise License",
                    CategoryId = licenseCat,
                    Brand = "Microsoft",
                    Model = "Enterprise E5",
                    SerialNumber = "LIC-MS365-9900",
                    PurchaseDate = DateTime.UtcNow.AddMonths(-6),
                    PurchaseCost = 450.00m,
                    WarrantyExpiry = DateTime.UtcNow.AddMonths(6),
                    Status = "Assigned",
                    Location = "Cloud Tenant",
                    Description = "Annual Microsoft 365 E5 seat license"
                }
            };
            await context.Assets.AddRangeAsync(assets);
            await context.SaveChangesAsync();

            // 5. Assignments
            var assignments = new List<AssetAssignment>
            {
                new AssetAssignment
                {
                    AssetId = assets[0].AssetId, // Dell XPS 15
                    EmployeeId = employees[0].Id, // John Doe
                    AssignedDate = DateTime.UtcNow.AddMonths(-5),
                    ExpectedReturnDate = DateTime.UtcNow.AddMonths(7),
                    AssignedBy = "System Admin",
                    Notes = "Primary development laptop assigned during onboarding",
                    Status = "Active"
                },
                new AssetAssignment
                {
                    AssetId = assets[2].AssetId, // LG Monitor
                    EmployeeId = employees[0].Id, // John Doe
                    AssignedDate = DateTime.UtcNow.AddMonths(-4),
                    ExpectedReturnDate = DateTime.UtcNow.AddMonths(8),
                    AssignedBy = "System Admin",
                    Notes = "Dual screen workstation setup",
                    Status = "Active"
                },
                new AssetAssignment
                {
                    AssetId = assets[6].AssetId, // MS 365 License
                    EmployeeId = employees[1].Id, // Jane Smith
                    AssignedDate = DateTime.UtcNow.AddMonths(-3),
                    ExpectedReturnDate = DateTime.UtcNow.AddMonths(9),
                    AssignedBy = "System Admin",
                    Notes = "HR Office productivity suite",
                    Status = "Active"
                }
            };
            await context.AssetAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();

            // 6. Asset Requests
            var requests = new List<AssetRequest>
            {
                new AssetRequest
                {
                    EmployeeId = employees[2].Id, // Robert Johnson
                    CategoryId = laptopCat,
                    Reason = "Requires laptop for remote financial auditing travel next week",
                    RequestedDate = DateTime.UtcNow.AddDays(-3),
                    Status = "Pending",
                    AdminComment = ""
                },
                new AssetRequest
                {
                    EmployeeId = employees[3].Id, // Emily Davis
                    CategoryId = monitorCat,
                    Reason = "Product roadmap planning requires additional screen real estate",
                    RequestedDate = DateTime.UtcNow.AddDays(-10),
                    Status = "Approved",
                    AdminComment = "Approved. Pending physical pickup from IT department."
                }
            };
            await context.AssetRequests.AddRangeAsync(requests);
            await context.SaveChangesAsync();

            // 7. Maintenance Records
            var maintenance = new List<MaintenanceRecord>
            {
                new MaintenanceRecord
                {
                    AssetId = assets[3].AssetId, // iPhone 14 Pro
                    Issue = "Battery drain fast and screen touch responsiveness glitch",
                    ReportedDate = DateTime.UtcNow.AddDays(-5),
                    MaintenanceDate = DateTime.UtcNow.AddDays(-2),
                    Vendor = "Apple Authorized Service Provider",
                    Cost = 120.00m,
                    Resolution = "Replacing battery module under warranty diagnosis",
                    Status = "InProgress",
                    CompletedDate = null
                }
            };
            await context.MaintenanceRecords.AddRangeAsync(maintenance);
            await context.SaveChangesAsync();

            // 8. Asset Histories
            var histories = new List<AssetHistory>
            {
                new AssetHistory
                {
                    AssetId = assets[0].AssetId,
                    Action = "Created",
                    PerformedBy = "System Admin",
                    Date = DateTime.UtcNow.AddMonths(-12),
                    Description = "Asset AST-1001 (Dell XPS 15 9530) registered into inventory."
                },
                new AssetHistory
                {
                    AssetId = assets[0].AssetId,
                    Action = "Assigned",
                    PerformedBy = "System Admin",
                    Date = DateTime.UtcNow.AddMonths(-5),
                    Description = "Assigned to John Doe (EMP-001)."
                },
                new AssetHistory
                {
                    AssetId = assets[3].AssetId,
                    Action = "MaintenanceStart",
                    PerformedBy = "System Admin",
                    Date = DateTime.UtcNow.AddDays(-5),
                    Description = "Asset sent for maintenance: Battery drain fast and screen glitch."
                }
            };
            await context.AssetHistories.AddRangeAsync(histories);
            await context.SaveChangesAsync();
        }
    }
}
