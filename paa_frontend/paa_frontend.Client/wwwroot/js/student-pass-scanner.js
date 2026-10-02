window.studentPassScanner = (() => {
    let reader = null;
    let abortController = null;

    function isWebNfcSupported() {
        return typeof window !== "undefined" && "NDEFReader" in window;
    }

    async function startWebNfc(dotNetRef) {
        if (!isWebNfcSupported()) {
            throw new Error("Web NFC is unavailable. Use Chrome on Android over HTTPS.");
        }

        await stopWebNfc();

        reader = new NDEFReader();
        abortController = new AbortController();

        reader.addEventListener("reading", async event => {
            const code = extractCode(event);
            if (!code) {
                return;
            }

            await dotNetRef.invokeMethodAsync("ReceiveNfcCode", code);
        });

        reader.addEventListener("readingerror", () => {
            console.warn("The NFC tag was detected, but its payload could not be read.");
        });

        await reader.scan({ signal: abortController.signal });
    }

    async function stopWebNfc() {
        if (abortController) {
            abortController.abort();
            abortController = null;
        }

        reader = null;
    }

    function extractCode(event) {
        if (event.serialNumber && event.serialNumber.trim()) {
            return event.serialNumber.trim();
        }

        if (!event.message?.records?.length) {
            return "";
        }

        const decoder = new TextDecoder();

        for (const record of event.message.records) {
            try {
                if (record.recordType === "text" || record.mediaType === "text/plain") {
                    return decoder.decode(record.data).trim();
                }
            } catch (error) {
                console.warn("Failed to decode NFC record", error);
            }
        }

        return "";
    }

    return {
        isWebNfcSupported,
        startWebNfc,
        stopWebNfc
    };
})();
