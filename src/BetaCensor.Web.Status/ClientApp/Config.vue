<script setup lang="ts">
import { computed } from 'vue'
import { darkTheme, NConfigProvider, NGlobalStyle, useOsTheme, NGrid, NGridItem, NTabs, NTabPane, NLoadingBarProvider, NAlert, NText } from "naive-ui";
import { themeOverrides } from './util'
import ServerHeader from './components/ServerHeader.vue';
import StickerSummary from './components/StickerSummary.vue';
import StickerPreviews from './components/StickerPreviews.vue';
import ConfigWizard from './components/ConfigWizard.vue';

const osTheme = useOsTheme();
const theme = computed(() => (osTheme.value === 'dark' ? darkTheme : null))

const props = withDefaults(defineProps<{ msg?: string }>(), { msg: 'Testing' });

</script>

<template>
    <n-config-provider :theme-overrides="themeOverrides" :theme="theme">
        <n-loading-bar-provider>
            <ServerHeader title="Server Configuration" />
            <n-tabs size="large" justify-content="space-evenly">
                <n-tab-pane name="local" tab="Stickers">
                    <n-alert closable title="Be Patient!">
                        Showing these images and previews requires loading, processing and parsing a lot of data from your server, so please be patient while it loads!
                    </n-alert>
                    <n-grid :cols="3" :x-gap="20" :y-gap="20">
                        <n-grid-item>
                            <StickerSummary />
                        </n-grid-item>
                        <n-grid-item :span="2">
                            <StickerPreviews />
                        </n-grid-item>
                    </n-grid>
                </n-tab-pane>
                <!-- <n-tab-pane name="server-config" tab="Server Config">
                    <ConfigWizard />
                </n-tab-pane> -->
            </n-tabs>
        </n-loading-bar-provider>
        <n-global-style />
    </n-config-provider>
</template>

<style>
html {
    max-width: 80%;
    padding: 1.5rem;
    margin-left: auto;
    margin-right: auto;
}
</style>
