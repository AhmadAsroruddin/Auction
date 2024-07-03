import { getTokenWorkaround } from "@/app/actions/authActions";

const baseUrl = "http://localhost:6001/";

async function get(url: string) {
    const requestOptions = {
        method: 'GET',
        headers: await getHeaders()
    };

    const fullUrl = `${baseUrl}${url}`;

    try {
        const response = await fetch(fullUrl, requestOptions);
        return handleResponse(response); // Kirim objek respons langsung ke handleResponse
    } catch (error) {
        console.error('Error fetching data:', error);
        throw error;
    }
}

async function post(url: string, body: {}) {
    const requestOptions = {
        method: 'POST',
        headers: await getHeaders(),
        body: JSON.stringify(body)
    };

    const fullUrl = `${baseUrl}${url}`;
    console.log(`Posting to URL: ${fullUrl}`); // Debugging: log URL

    try {
        const response = await fetch(fullUrl, requestOptions);
        console.log('Raw response:', response); // Debugging: log raw response
        return handleResponse(response);
    } catch (error) {
        console.error('Error posting data:', error);
        throw error;
    }
}

async function put(url: string, body: {}) {
    const requestOptions = {
        method: 'PUT',
        headers: await getHeaders(),
        body: JSON.stringify(body)
    };

    const fullUrl = `${baseUrl}${url}`;
    console.log(`Putting to URL: ${fullUrl}`); // Debugging: log URL

    try {
        const response = await fetch(fullUrl, requestOptions);
        console.log('Raw response:', response); // Debugging: log raw response
        return handleResponse(response);
    } catch (error) {
        console.error('Error putting data:', error);
        throw error;
    }
}

async function del(url: string) {
    const requestOptions = {
        method: 'DELETE',
        headers: await getHeaders()
    };

    const fullUrl = `${baseUrl}${url}`;
    console.log(`Deleting URL: ${fullUrl}`); // Debugging: log URL

    try {
        const response = await fetch(fullUrl, requestOptions);
        console.log('Raw response:', response); // Debugging: log raw response
        return handleResponse(response);
    } catch (error) {
        console.error('Error deleting data:', error);
        throw error;
    }
}

async function getHeaders() {
    const token = await getTokenWorkaround();
    const headers = { 'Content-type': 'application/json' } as any;
    if (token) {
        headers.Authorization = 'Bearer ' + token.access_token;
    }
    return headers;
}

async function handleResponse(response: Response) {
    const text = await response.text();

    let data;
    try {
        data = JSON.parse(text);
    } catch (error) {
        data = text;
    }

    if (response.ok) {
        return data || response.statusText;
    } else {
        const error = {
            status: response.status,
            message: typeof data === 'string' ? data : response.statusText
        };
        throw error; // Ubah dari return ke throw untuk melempar error
    }
}

export const fetchWrapper = {
    get,
    post,
    put,
    del
};
