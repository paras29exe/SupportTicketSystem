-- 1. All open tickets with customer name and agent name
SELECT
    t.id,
    t.title,
    t.description,
    t.priority,
    t.status,
    c.name AS customerName,
    a.name AS agentName,
    t.createdAt
FROM tickets AS t
INNER JOIN customers AS c
    ON t.customerId = c.id
LEFT JOIN agents AS a
    ON t.agentId = a.id
WHERE t.status COLLATE Latin1_General_100_CI_AS = 'open';


-- 2. All agents with ticket count, including agents with no tickets
SELECT
    a.id,
    a.name,
    a.email,
    COUNT(t.id) AS ticketCount
FROM agents AS a
LEFT JOIN tickets AS t
    ON a.id = t.agentId
GROUP BY
    a.id,
    a.name,
    a.email;


-- 3. Tickets whose title contains a given word

SELECT
    id,
    title,
    description,
    priority,
    status,
    customerId,
    agentId,
    createdAt
FROM tickets
WHERE title LIKE '%' + 'password' + '%';


-- 4. Top 3 agents by number of resolved tickets
SELECT TOP 3
    a.id,
    a.name,
    COUNT(t.id) AS resolvedTicketCount
FROM agents AS a
LEFT JOIN tickets AS t
    ON a.id = t.agentId
    AND t.status COLLATE Latin1_General_100_CI_AS = 'resolved'
GROUP BY
    a.id,
    a.name
ORDER BY
    resolvedTicketCount DESC


-- 5. Customers who have more than 2 open tickets
SELECT
    c.id,
    c.name,
    c.email,
    COUNT(t.id) AS openTicketCount
FROM customers AS c
INNER JOIN tickets AS t
    ON c.id = t.customerId
WHERE t.status Collate Latin1_General_100_CI_AS = 'open'
GROUP BY
    c.id,
    c.name,
    c.email
HAVING COUNT(t.id) > 2;


-- 6. Assign a ticket to an agent.
-- Roll back if the agent is inactive or does not exist or invalid ticketId.

DECLARE @ticketId INT = 1;
DECLARE @agentId INT = 1;

BEGIN TRANSACTION
    IF NOT EXISTS (SELECT 1 from agents where id = @agentId)
        BEGIN
            PRINT 'agent is inactive or doesnt exist';
            ROLLBACK TRANSACTION;
        END;

    UPDATE tickets 
        SET agentId = @agentId
    WHERE id = @ticketId;

    IF @@ROWCOUNT = 0
        BEGIN
            PRINT 'Invalid ticketId provided to update';
            ROLLBACK TRANSACTION;
        END;

    COMMIT TRANSACTION;
