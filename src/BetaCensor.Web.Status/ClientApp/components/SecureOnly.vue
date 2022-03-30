<script setup lang="ts">
/// <reference types="wicg-file-system-access"/>
import { NAlert, NThing, NText, NButton, NSpace } from "naive-ui";
import { computed } from "vue";

const trusted = computed(() => !!window.showDirectoryPicker);
const httpsUrl = computed(() => `https://${window.location.host}${window.location.pathname}`);

const openLocal = () => {
window.open('http://localhost:2382' + window.location.pathname, '_blank');
}

const openHttps = () => {
    window.open(httpsUrl.value, '_blank');
}

</script>
<template>
    <n-alert title="Not Available" type="error" v-if="!trusted">
        <n-thing title="API blocked by current context">
            <n-text
                tag="p"
            >Due to security restrictions in the browser, this feature is only available over a secure connection.</n-text>
            <n-text tag="p">
                If your server is running on the same PC as you're accessing it from, you can try using the
                <n-button
                    text
                    @click="openLocal"
                >
                    <code>localhost</code>
                </n-button> address. If you are running your server somewhere else, make sure it's accessible over
                <n-button text @click="openHttps">
                    <code>HTTPS</code>
                </n-button>.
            </n-text>
            <template #action>
                <n-space>
                    <n-button size="small" @click="openLocal">
                        Try <code style="margin-left: 0.5em;">localhost</code>
                    </n-button>
                    <n-button size="small" @click="openHttps">
                        Try <code style="margin-left: 0.5em;">HTTPS</code>
                    </n-button>
                </n-space>
            </template>
        </n-thing>
    </n-alert>
    <template v-if="trusted">
        <slot></slot>
    </template>
</template>