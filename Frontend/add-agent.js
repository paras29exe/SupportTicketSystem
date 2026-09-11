const agentForm = document.getElementById("agentForm");
const message = document.getElementById("message");

agentForm.addEventListener("submit", async event => {
    event.preventDefault();
    message.textContent = "Creating agent...";
    message.className = "message";

    const formData = new FormData(agentForm);
    const agent = {
        name: formData.get("name"),
        email: formData.get("email"),
        department: formData.get("department") || null
    };

    try {
        await requestApi("/agents", {
            method: "POST",
            body: JSON.stringify(agent)
        });
        message.textContent = "Agent created successfully.";
        message.className = "message success";
        agentForm.reset();
    } catch (error) {
        message.textContent = error.message;
        message.className = "message error";
    }
});
