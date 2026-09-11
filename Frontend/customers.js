let customers = [];

const customersTableBody = document.getElementById("customersTableBody");
const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");

let paginationValues = getPaginationValues();

async function loadCustomers() {
    paginationValues = getPaginationValues();
    loadingMessage.textContent = "Loading customers...";
    errorMessage.textContent = "";

    try {
        const response = await requestApi(`/customers?page=${paginationValues.page}&pageSize=${paginationValues.pageSize}`);
        customers = getListData(response);
        renderCustomers();
        renderPagination("customerPagination", response.data.pagination, loadCustomers);
    } catch (error) {
        errorMessage.textContent = error.message;
    } finally {
        loadingMessage.textContent = "";
    }
}

function renderCustomers() {
    customersTableBody.innerHTML = customers.map(customer => `
        <tr>
            <td>${customer.id}</td>
            <td>${escapeHtml(customer.name)}</td>
            <td>${escapeHtml(customer.email)}</td>
            <td>${escapeHtml(customer.phone)}</td>
            <td><button type="button" data-customer-id="${customer.id}">Delete</button></td>
        </tr>
    `).join("");

    customersTableBody.querySelectorAll("button[data-customer-id]").forEach(button => {
        button.addEventListener("click", () => deleteCustomer(Number(button.dataset.customerId)));
    });
}

async function deleteCustomer(id) {
    if (!confirm("Delete this customer? Customers with open tickets cannot be deleted.")) {
        return;
    }

    try {
        await requestApi(`/customers/${id}`, { method: "DELETE" });
        await loadCustomers();
    } catch (error) {
        errorMessage.textContent = error.message;
    }
}

document.getElementById("loadCustomersButton").addEventListener("click", loadCustomers);
loadCustomers();