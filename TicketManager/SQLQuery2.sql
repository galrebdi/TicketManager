INSERT INTO Employees (
    Id,
    FullName,
    MobileNumber,
    Email,
    DateOfBirth,
    Type,
    HashedPassword,
    Address,
    IsActive
)
VALUES (
    NEWID(), 
    'Support Manager',
    '0501234567',
    'support.manager@example.com',
    '1980-01-01',
    1, 
    'hashed-password-goes-here',
    'N/A',
    1 
);