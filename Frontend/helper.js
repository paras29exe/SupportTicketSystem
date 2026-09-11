function getPaginationValues() {
    const url = new URL(window.location.href);
    const page = getPositiveNumber(url.searchParams.get("page"), 1);
    const pageSize = getPositiveNumber(url.searchParams.get("pageSize"), 20);

    url.searchParams.set("page", page);
    url.searchParams.set("pageSize", Math.min(pageSize, 20));
    window.history.replaceState({}, "", url);

    return { page, pageSize };
}

function setPaginationPage(page, pageSize) {
    const url = new URL(window.location.href);
    url.searchParams.set("page", page);
    url.searchParams.set("pageSize", pageSize);
    window.history.pushState({}, "", url);
}

function renderPagination(parentComponentName, pagination, loadPage) {
    const parent = document.getElementById(parentComponentName);
    if (!parent) {
        return;
    }

    parent.innerHTML = "";

    const totalCount = document.createElement("p");
    totalCount.textContent = `Total records: ${pagination.totalCount}`;
    parent.appendChild(totalCount);

    for (let page = 1; page <= pagination.totalPages; page += 1) {
        const button = document.createElement("button");
        button.type = "button";
        button.textContent = page;
        button.disabled = page === pagination.page;
        button.addEventListener("click", () => {
            setPaginationPage(page, pagination.pageSize);
            loadPage();
        });
        parent.appendChild(button);
    }
}

function getPositiveNumber(value, defaultValue) {
    const number = Number(value);
    return Number.isInteger(number) && number > 0 ? number : defaultValue;
}

// took help from chat gpt here to avoid excaping html
function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}
