let tickets = [];

const ticketsTableBody = document.getElementById("ticketsTableBody");
const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");

let paginationValues = getPaginationValues();

document.addEventListener("DOMContentLoaded", loadTickets);

async function loadTickets() {
    paginationValues = getPaginationValues();
    loadingMessage.textContent = "Loading tickets...";
    errorMessage.textContent = "";
    successMessage.textContent = "";

    try {
        const response = await requestApi(`/tickets?page=${paginationValues.page}&pageSize=${paginationValues.pageSize}`);
        tickets = getListData(response);
        renderTickets();
        renderPagination("ticketPagination", response.data.pagination, loadTickets);
    } catch (error) {
        errorMessage.textContent = error.message;
    } finally {
        loadingMessage.textContent = "";
    }
}

function renderTickets() {
    ticketsTableBody.innerHTML = tickets.map(ticket => `
        <tr data-ticket-id="${ticket.id}">
            <td>${ticket.id}</td>
            <td>${escapeHtml(ticket.title)}</td>
            <td>${escapeHtml(ticket.description || "")}</td>
            <td>${ticket.priority}</td>
            <td>
                <select class="status-control" data-original-status="${ticket.status}">
                    <option value="open" ${ticket.status === "open" ? "selected" : ""}>Open</option>
                    <option value="inProgress" ${ticket.status === "inProgress" ? "selected" : ""}>In progress</option>
                    <option value="resolved" ${ticket.status === "resolved" ? "selected" : ""}>Resolved</option>
                    <option value="closed" ${ticket.status === "closed" ? "selected" : ""}>Closed</option>
                </select>
                <button class="status-button" type="button" disabled>Update</button>
            </td>
            <td>${formatDate(ticket.createdAt)}</td>
            <td>${ticket.closedAt ? formatDate(ticket.closedAt) : "-"}</td>
            <td>${escapeHtml(ticket.customerName || ticket.customerId)}</td>
            <td>${escapeHtml(ticket.agentName || ticket.agentId || "Unassigned")}</td>
            <td>
                <a class="assign-agent-btn" href="assign-ticket.html?ticketId=${ticket.id}">Assign agent</a>
            </td>
        </tr>
    `).join("");

    ticketsTableBody.querySelectorAll("tr[data-ticket-id]").forEach(row => {
        const button = row.querySelector(".status-button");
        const statusSelect = row.querySelector(".status-control");

        statusSelect.addEventListener("change", () => {
            button.disabled = statusSelect.value === statusSelect.dataset.originalStatus;
        });
        button.addEventListener("click", () => updateTicketStatus(row));
    });
}

function formatDate(value) {
    if (!value) {
        return "-";
    }

    const date = new Date(value);
    return date.toLocaleString();
}

async function updateTicketStatus(row) {
    const id = Number(row.dataset.ticketId);
    const button = row.querySelector(".status-button");
    const statusSelect = row.querySelector(".status-control");
    const status = statusSelect.value;
    button.disabled = true;

    try {
        await requestApi(`/tickets/${id}/status`, {
            method: "PATCH",
            body: JSON.stringify({ status })
        });
        await loadTickets();
        successMessage.textContent = "Ticket: " + id + " status updated successfully.";
    } catch (error) {
        errorMessage.textContent = error.message;
        button.disabled = false;
    }
}


document.getElementById("loadTicketsButton").addEventListener("click", loadTickets);
