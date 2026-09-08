/* 1. View showing ticket details with customer and agent names */
CREATE OR ALTER VIEW ticketDetails
AS
SELECT
    t.id AS ticketId,
    t.title,
    t.priority,
    t.status,
    c.name AS customerName,
    a.name AS agentName,
    t.createdAt AS createdDate
FROM tickets AS t
INNER JOIN customers AS c
    ON t.customerId = c.id
LEFT JOIN agents AS a
    ON t.agentId = a.id;
GO


/* 2. Stored procedure returning tickets for a customer */
CREATE OR ALTER PROCEDURE getTicketsByCustomer
    @customerId INT
AS
BEGIN;
    SELECT
        id,
        title,
        description,
        priority,
        status,
        customerId,
        agentId,
        createdAt,
        closedAt
    FROM tickets
    WHERE customerId = @customerId;
END;
GO


/* 3. Stored procedure updating status unless the ticket is already Closed */
CREATE OR ALTER PROCEDURE updateTicketStatus
    @ticketId INT,
    @newStatus VARCHAR(20)
AS
BEGIN;
    IF EXISTS
    (
        SELECT 1
        FROM tickets
        WHERE id = @ticketId
          AND status COLLATE Latin1_General_100_CI_AS = 'Closed'
    )
    BEGIN;
        THROW 50001, 'Ticket status cannot be changed because the ticket is already Closed.', 1;
    END;

    IF @newStatus COLLATE Latin1_General_100_CI_AS = 'Closed'
        BEGIN;
            UPDATE tickets
            SET status = @newStatus,
            closedAt = SYSUTCDATETIME()
            WHERE id = @ticketId;
        END;
    ELSE
        UPDATE tickets
        SET status = @newStatus
        WHERE id = @ticketId;

    IF @@ROWCOUNT = 0
    BEGIN;
        THROW 50002, 'Ticket does not exist.', 1;
    END;
END;
GO

-- Executing views and procedures
SELECT * FROM ticketDetails;

EXEC getTicketsByCustomer @customerId = 1;

EXEC updateTicketStatus
    @ticketId = 1,
    @newStatus = 'closed';

select * from tickets;
update tickets set status = 'Open', closedAt = NULL where id = 1;