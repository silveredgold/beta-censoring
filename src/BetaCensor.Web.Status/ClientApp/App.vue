<script setup lang="ts">
import { computed } from 'vue'
import { darkTheme, NConfigProvider, NGlobalStyle, useOsTheme, NGrid, NGridItem, NTabs, NTabPane } from "naive-ui";
import { themeOverrides } from './util'
import ServerHeader from './components/ServerHeader.vue';
import ServerConfiguration from './components/ServerConfiguration.vue';
import AssetStoreState from './components/AssetStoreState.vue';
import CensoringConfiguration from './components/CensoringConfiguration.vue';
import PerformanceSummary from './components/PerformanceSummary.vue';

const osTheme = useOsTheme();
const theme = computed(() => (osTheme.value === 'dark' ? darkTheme : null))

</script>

<template>
    <n-config-provider :theme-overrides="themeOverrides" :theme="theme">
        <ServerHeader />
        <n-tabs size="large" justify-content="space-evenly">
            <n-tab-pane name="status" tab="Status">
                <n-grid :cols="3" :x-gap="20" :y-gap="20">
                    <n-grid-item><ServerConfiguration /></n-grid-item>
                    <n-grid-item><CensoringConfiguration /></n-grid-item>
                    <n-grid-item><AssetStoreState /></n-grid-item>
                </n-grid>
            </n-tab-pane>
            <n-tab-pane name="performance" tab="Performance" display-directive="show:lazy">
                <PerformanceSummary />
            </n-tab-pane>
        </n-tabs>
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
