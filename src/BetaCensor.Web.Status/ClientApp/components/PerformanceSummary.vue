<script setup lang="ts">
import { onBeforeMount, Ref, ref } from 'vue'
import { NButton, NGrid, NGridItem, NText } from "naive-ui";
import CensoringPerformance from './CensoringPerformance.vue';
import InferencePerformance from './InferencePerformance.vue';
import type { CensoringRecord } from '../util';

const performanceData: Ref<CensoringRecord[] | undefined> = ref(undefined);

const refresh = () => {
    fetch('_server/performance').then(resp => {
        console.log(resp);
        resp.json().then(json => {
            console.log(json);
            performanceData.value = json;
        });
    });
}

onBeforeMount(() => {
    refresh()
});

</script>

<template>
    <n-grid :cols="6">
        <n-grid-item :span="6">
            <n-button @click="refresh">Refresh</n-button>
            <n-text depth="3" style="margin-left: 1.5rem;">This will only include data from the current session (i.e. since the server started).</n-text>
        </n-grid-item>
        <n-grid-item :span="2">
            <!-- <n-alert closable title="Be Patient!">
                            Showing these images and previews requires loading, processing and parsing a lot of data from your server, so please be patient while it loads!
            </n-alert>-->
            <CensoringPerformance :records="performanceData" />
        </n-grid-item>
        <n-grid-item :span="4">
            <InferencePerformance :records="performanceData" />
        </n-grid-item>
    </n-grid>
</template>

<style>
html {
    max-width: 80%;
    padding: 1.5rem;
    margin-left: auto;
    margin-right: auto;
}
</style>
