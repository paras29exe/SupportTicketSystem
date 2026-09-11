const customerForm = document.getElementById("customerForm");
const message = document.getElementById("message");

customerForm.addEventListener("submit", async event => {
    event.preventDefault();
    message.textContent = "Creating customer...";
    message.className = "message";

    const formData = new FormData(customerForm);
    const customer = {
        name: formData.get("name"),
        email: formData.get("email"),
        phone: formData.get("phone")
    };

    try {
        await requestApi("/customers", {
            method: "POST",
            body: JSON.stringify(customer)
        });
        message.textContent = "Customer created successfully.";
        message.className = "message success";
        customerForm.reset();
    } catch (error) {
        message.textContent = error.message;
        message.className = "message error";
    }
});