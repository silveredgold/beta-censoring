import { Plugin } from "vue";
import type { ICensorBackend } from "@silveredgold/beta-shared/transport";
// import { BetaSafetyBackendClient, defaultMessageEvents } from "@silveredgold/beta-shared/transport/beta-safety";
import { censorBackend } from "@silveredgold/beta-shared-components";
import { BetaCensorClient } from "@silveredgold/beta-censor-client";


export const backendProviderPlugin: Plugin = {
    install: (app, options) => {
        const getBackendAsync = () => new Promise<ICensorBackend>((resolve) => {
            getRequestClient().then(client => {
                resolve(client);
            });
        });
        app.provide(censorBackend, getBackendAsync);
    }
}

const getRequestClient = async (host?: string): Promise<ICensorBackend> => {
    const client = new BetaCensorClient(window.location.href.slice(0,-1));
    client.ephemeral = true;
    // const client = new BetaSafetyBackendClient(defaultMessageEvents);
    return client;
}