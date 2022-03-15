<script setup lang="ts">
import { ref, computed, onBeforeMount } from 'vue'
import { darkTheme, NConfigProvider, NGlobalStyle, NNotificationProvider, useOsTheme, NGrid, NGridItem } from "naive-ui";
import { themeOverrides } from './util'
import ServerHeader from './components/ServerHeader.vue';
import ServerConfiguration from './components/ServerConfiguration.vue';
import AssetStoreState from './components/AssetStoreState.vue';
import CensoringConfiguration from './components/CensoringConfiguration.vue';

const osTheme = useOsTheme();
const theme = computed(() => (osTheme.value === 'dark' ? darkTheme : null))

const props = withDefaults(defineProps<{ msg?: string }>(), { msg: 'Testing' });

</script>

<template>
    <n-config-provider :theme-overrides="themeOverrides" :theme="theme">
        <n-notification-provider>
            <ServerHeader />
            <n-grid :cols="3" :x-gap="20" :y-gap="20">
            <n-grid-item><ServerConfiguration /></n-grid-item>
            <n-grid-item><CensoringConfiguration /></n-grid-item>
            <n-grid-item><AssetStoreState /></n-grid-item>
            </n-grid>
        </n-notification-provider>
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
