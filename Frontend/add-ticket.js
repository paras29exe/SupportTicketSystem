const ticketForm = document.getElementById("ticketForm");
const message = document.getElementById("message");

ticketForm.addEventListener("submit", async event => {
    event.preventDefault();
    message.textContent = "Creating ticket...";
    message.className = "message";

    const formData = new FormData(ticketForm);
    const ticket = {
        title: formData.get("title"),
        description: formData.get("description") || null,
        priority: formData.get("priority"),
        customerId: Number(formData.get("customerId"))
    };

    try {
        await requestApi("/tickets", {
            method: "POST",
            body: JSON.stringify(ticket)
        });
        message.textContent = "Ticket created successfully.";
        message.className = "message success";
        ticketForm.reset();
    } catch (error) {
        message.textContent = error.message;
        message.className = "message error";
    }
});