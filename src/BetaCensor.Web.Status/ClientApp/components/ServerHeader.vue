<template>
    <n-grid :cols="3">
        <n-gi :span="2">
            <n-page-header
                :subtitle="extraText"
                style="padding-bottom: 2rem;"
            >
                <template #header>{{title}}</template>
                <template #title>Beta Censoring</template>
                <template #avatar>
                    <n-avatar :src="iconSrc" />
                </template>
                <template #extra>
                    <n-space>
                        <n-popover trigger="hover" placement="bottom-end">
                            <template #trigger>
                                <n-button @click="openConfig">
                                    <n-icon size="30" :component="Open" />
                                </n-button>
                            </template>
                            Stickers and Configuration
                        </n-popover>
                    </n-space>
                </template>
                <n-grid :cols="3">
                    <n-gi>
                        <n-statistic label="Server Version" :value="serverVersion" />
                    </n-gi>
                    <n-gi>
                        <n-statistic v-if="coreLoaded" label="CensorCore Version" :value="coreVersion" />
                        <n-thing v-if="!coreLoaded" title="CensorCore Version">
                            <n-popover :scrollable="true" width="trigger">
                                    <template #trigger>
                                        Could not load AI runtime!
                                    </template>
                                    <n-text>The server could not load the AI runtime. This usually indicates a corrupt installation or that your PC does not have the <a href="https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist" target="_blank">VC++ runtime</a> installed.</n-text>
                            </n-popover>
                        </n-thing>
                    </n-gi>
                    <n-gi>
                        <n-statistic label="Pending Requests" :value="requestCount">
                            <template #suffix>
                                <n-popover trigger="hover" placement="bottom" v-if="requestCount >= 0">
                                    <template #trigger>
                                        <n-button ghost strong circle @click="refresh" size="small" style="margin-left: 0.25rem;">
                                            <n-icon size="18" :component="Refresh" />
                                        </n-button>
                                    </template>
                                    Refresh
                                </n-popover>
                                <n-popover trigger="hover" placement="right" v-if="requestCount < 0">
                                    <template #trigger>
                                        <n-button ghost strong circle type="primary" size="small">
                                            <template #icon>
                                                <n-icon :component="Alert" />
                                            </template>
                                        </n-button>
                                    </template>
                                    <n-thing
                                        style="max-width: 40rem;"
                                        title="Wait, how is there negative requests?"
                                    >If the count says -1, that just means the server is unable to accurately check how many requests are pending.</n-thing>
                                </n-popover>
                            </template>
                        </n-statistic>
                    </n-gi>
                </n-grid>
            </n-page-header>
        </n-gi>
        <n-gi>
            <n-space :vertical="false" item-style="display: flex;" justify="end" :wrap="false">
                <ConnectionStatus :compact="true" :host-config="getHost" />
                <n-popover trigger="hover" placement="left">
                    <template #trigger>
                        <n-button ghost strong circle type="primary" size="small">
                            <template #icon>
                                <n-icon :component="Help" />
                            </template>
                        </n-button>
                    </template>
                    <n-thing
                        style="max-width: 40rem;"
                        title="What does this mean?"
                    >This status is not just whether the server is running, this status connects to the server the same way a client would. If this is showing Connected, the odds are high clients will be able to connect as well.</n-thing>
                </n-popover>
            </n-space>
        </n-gi>
    </n-grid>
</template>
<script setup lang="ts">
import { NPageHeader, NGrid, NGi, NAvatar, NStatistic, NButton, NSpace, NIcon, NPopover, NThing, NText } from "naive-ui";
import { Open, Refresh, Alert, Help } from "@vicons/ionicons5";
import { computed, toRefs, ref, Ref, onMounted } from "vue";
import { ConnectionStatus } from "@silveredgold/beta-shared-components";
import type { HostConfigurator } from '@silveredgold/beta-shared-components'
import iconSrc from '../assets/siteIcon.png'
import { useRazorRequest } from "../request-plugin";

const getHost: HostConfigurator = {
    getBackendHost: async (): Promise<string> => {
        return window.location.origin;
    }
}

const props = withDefaults(defineProps<{title?: string}>(), {title: "Server Status Panel"});

const coreVersion = ref("unknown");
const coreError = ref(0);
const coreLoaded = computed(() => coreError.value === 0 || coreError.value === 200);
const serverVersion = ref("v0.0.0");
const requestCount = ref(0);
const hostname = ref("");
const requestHeaders: Ref<HeadersInit|undefined> = ref(undefined);

const {title} = toRefs(props);

const extraText = computed(() => `Server is now running${hostname.value ? " on " + hostname.value : ""}, and listening for censoring requests.`);

onMounted(() => {
    const headers = useRazorRequest();
    requestHeaders.value = headers;
    fetch('/censoring/info', { headers }).then(resp => {
        console.log(resp);
        coreError.value = resp.status || 0;
        resp.json().then(json => {
            coreVersion.value = json.version;
        })
    });
    fetch('/?handler=serverVersion', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            serverVersion.value = json.version;
            hostname.value = json.hostname ?? '';
        })
    });
    refresh();
});

const refresh = async () => {
    const resp = await fetch('/?handler=requestCount', { headers: requestHeaders.value });
    console.log(resp);
    var json = await resp.json();
    console.log(JSON.stringify(json));
    requestCount.value = json.requests;
}

const openConfig = async () => {
    window.open('/config', '_blank');
}
</script>