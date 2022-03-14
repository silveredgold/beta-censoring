import { Plugin, InjectionKey, inject } from "vue";

export const requestHeaders: InjectionKey<() => HeadersInit> = Symbol();

export const requestPlugin: Plugin = {
    install: (app, options) => {
        app.provide(requestHeaders, () => {
            return {
                "RequestVerificationToken": (document.querySelector('input[type="hidden"][name="__RequestVerificationToken"]') as HTMLInputElement).value
            }
        });
    }
}

export function useRazorRequest(headers?: HeadersInit): HeadersInit {
    const events = inject(requestHeaders, null);
    if (events === null) {
        throw new Error(`[request-plugin]: Failed to inject request headers.`)
      }
    const baseRequests = events();
    if (headers !== undefined) {
        return {
            ...baseRequests,
            ...headers
        }
    }
    return baseRequests;
}