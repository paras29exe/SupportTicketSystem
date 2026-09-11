let agents = [];

const agentsTableBody = document.getElementById("agentsTableBody");
const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");

let paginationValues = getPaginationValues();

async function loadAgents() {
    paginationValues = getPaginationValues();
    loadingMessage.textContent = "Loading agents...";
    errorMessage.textContent = "";

    try {
        const response = await requestApi(`/agents?page=${paginationValues.page}&pageSize=${paginationValues.pageSize}`);
        agents = getListData(response);
        renderAgents();
        renderPagination("agentPagination", response.data.pagination, loadAgents);
    } catch (error) {
        errorMessage.textContent = error.message;
    } finally {
        loadingMessage.textContent = "";
    }
}

function renderAgents() {
    agentsTableBody.innerHTML = agents.map(agent => `
        <tr>
            <td>${agent.id}</td>
            <td>${escapeHtml(agent.name)}</td>
            <td>${escapeHtml(agent.email)}</td>
            <td>${escapeHtml(agent.department || "")}</td>
            <td>${agent.isActive ? "Yes" : "No"}</td>
        </tr>
    `).join("");
}



document.getElementById("loadAgentsButton").addEventListener("click", loadAgents);
loadAgents();
