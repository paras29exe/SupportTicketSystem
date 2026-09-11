const apiBaseUrl = "https://localhost:7040/api";

async function requestApi(path, options = {}) {
    const response = await fetch(`${apiBaseUrl}${path}`, {
        headers: {
            "Content-Type": "application/json",
            ...(options.headers || {})
        },
        ...options
    });

    const body = await response.json().catch(() => null);

    if (!response.ok) {
        const errorMessage = body && body.message
            ? body.message
            : `Request failed with status ${response.status}`;
        throw new Error(errorMessage);
    }

    return body;
}

function getListData(response) {
    return response && response.data && response.data.data
        ? response.data.data
        : [];
}