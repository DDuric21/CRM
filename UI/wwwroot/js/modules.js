export async function logExceptionToServer(baseUrl, message, stackTrace, url, level = "Error") {
    try {
        await fetch(`${baseUrl}/Logging/Error`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                message: message,
                stackTrace: stackTrace,
                url: url || window.location.href,
                logLevel: level,
                timestamp: new Date().toISOString()
            })
        });
    } catch (e) {
        console.error("Logging failed", e);
    }
}
