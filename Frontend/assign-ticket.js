const assignTicketForm = document.getElementById("assignTicketForm");
const message = document.getElementById("message");
const ticketIdInput = document.getElementById("ticketId");

const ticketIdFromUrl = new URLSearchParams(window.location.search).get("ticketId");
if (ticketIdFromUrl) {
    ticketIdInput.value = ticketIdFromUrl;
}

assignTicketForm.addEventListener("submit", async event => {
    event.preventDefault();
    message.textContent = "Assigning agent...";
    message.className = "message";

    const ticketId = Number(ticketIdInput.value);
    const agentId = Number(document.getElementById("agentId").value);

    try {
        await requestApi(`/tickets/assign/${ticketId}/agent/${agentId}`, {
            method: "POST"
        });
        message.textContent = "Agent assigned successfully.";
        message.className = "message success";
    } catch (error) {
        message.textContent = error.message;
        message.className = "message error";
    }
});
