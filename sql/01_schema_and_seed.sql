--Create Database SupportTicketDb;
use SupportTicketDb; 

CREATE TABLE customers
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    email VARCHAR(320) NOT NULL UNIQUE,
    phone VARCHAR(30) NULL,
    createdAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE agents
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    email VARCHAR(320) NOT NULL UNIQUE,
    department VARCHAR(100) NULL,
    isActive BIT NOT NULL DEFAULT 1,
    createdAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE tickets
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(250) NOT NULL,
    description VARCHAR(MAX) NULL,
    priority VARCHAR(20) NOT NULL,
    status VARCHAR(20) NOT NULL,
    customerId INT NOT NULL,
    agentId INT NULL,
    createdAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    closedAt DATETIME2 NULL,

    
    FOREIGN KEY (customerId) REFERENCES customers (id),
    FOREIGN KEY (agentId) REFERENCES agents (id),
    CHECK (priority COLLATE Latin1_General_100_CI_AS
    IN ('low', 'medium', 'high')),
    CHECK (status COLLATE Latin1_General_100_CI_AS
    IN ('open', 'inProgress', 'resolved', 'closed'))
);
GO

CREATE TABLE ticketNotes
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    ticketId INT NOT NULL,
    noteText VARCHAR(MAX) NOT NULL,
    createdAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    FOREIGN KEY (ticketId) REFERENCES tickets (id)
);
GO

/* Seed customers */
INSERT INTO customers (name, email, phone)
VALUES
    ('Abhishek', 'abhishek@example.com', '+91-1231231230'),
    ('Aryan', 'aryan@example.com', '+91-9991239990'),
    ('Jashan', 'jashan@example.com', '+91-7988012990');
GO

/* Seed agents */
INSERT INTO agents (name, email, department)
VALUES
    ('Paras', 'paras@support.example.com', 'Technical Support'),
    ('Sukhman', 'sukhman@support.example.com', 'Billing'),
    ('Ashish', 'ashish@support.example.com', 'Customer Success');
GO

/* Seed tickets */
DECLARE @abhishekId INT =
    (SELECT id FROM customers WHERE email = 'abhishek@example.com');
DECLARE @aryanId INT =
    (SELECT id FROM customers WHERE email = 'aryan@example.com');
DECLARE @jashanId INT =
    (SELECT id FROM customers WHERE email = 'jashan@example.com');

DECLARE @parasId INT =
    (SELECT id FROM agents WHERE email = 'paras@support.example.com');
DECLARE @sukhmanId INT =
    (SELECT id FROM agents WHERE email = 'sukhman@support.example.com');
DECLARE @ashishId INT =
    (SELECT id FROM agents WHERE email = 'ashish@support.example.com');

INSERT INTO tickets
    (title, description, priority, status, customerId, agentId)
VALUES
    ('Unable to reset password', 'Password reset email is not arriving.', 'High', 'Open', @abhishekId, NULL),
    ('Incorrect invoice total', 'The latest invoice includes an unexpected charge.', 'Medium', 'InProgress', @abhishekId, @sukhmanId),
    ('Application page loads slowly', 'Dashboard requests take more than thirty seconds.', 'High', 'InProgress', @aryanId, @parasId),
    ('Export report fails', 'CSV export returns an error for large date ranges.', 'Medium', 'Open', @aryanId, NULL),
    ('Update billing address', 'Please update the billing address on the account.', 'Low', 'Resolved', @jashanId, @sukhmanId),
    ('Missing email notification', 'Order confirmation emails are not being delivered.', 'High', 'Open', @jashanId, @ashishId),
    ('Request for user access', 'Please add a new administrator to the workspace.', 'Medium', 'Closed', @abhishekId, @ashishId),
    ('Feature question about alerts', 'Customer needs help configuring alert rules.', 'Low', 'Resolved', @aryanId, @parasId);
GO

/* Seed ticket notes */
DECLARE @ticket1Id INT =
    (SELECT id FROM tickets WHERE title = 'Unable to reset password');
DECLARE @ticket2Id INT =
    (SELECT id FROM tickets WHERE title = 'Incorrect invoice total');
DECLARE @ticket3Id INT =
    (SELECT id FROM tickets WHERE title = 'Application page loads slowly');
DECLARE @ticket5Id INT =
    (SELECT id FROM tickets WHERE title = 'Update billing address');
DECLARE @ticket7Id INT =
    (SELECT id FROM tickets WHERE title = 'Request for user access');

INSERT INTO ticketNotes (ticketId, noteText)
VALUES
    (@ticket1Id, 'Customer confirmed that the email address on the account is correct.'),
    (@ticket2Id, 'Billing team is reviewing the charge with the finance ledger.'),
    (@ticket3Id, 'Agent requested browser version and approximate request time.'),
    (@ticket5Id, 'Billing address was updated and verified with the customer.'),
    (@ticket7Id, 'Access was granted and the customer confirmed the new administrator can sign in.');
GO

/* Verify the seeded row counts */
SELECT
    (SELECT COUNT(*) FROM customers) AS customerCount,
    (SELECT COUNT(*) FROM agents) AS agentCount,
    (SELECT COUNT(*) FROM tickets) AS ticketCount,
    (SELECT COUNT(*) FROM ticketNotes) AS ticketNoteCount;
GO