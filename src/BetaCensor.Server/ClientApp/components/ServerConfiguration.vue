<template>
    <n-list bordered>
        <template #header>Enabled Services</template>
        <n-list-item key="rest">
            REST API
            <template #suffix>
                <n-icon-wrapper :size="26" :border-radius="12">
                    <n-icon :size="20" :component="restEnabled ? Checkmark : Close" />
                </n-icon-wrapper>
            </template>
        </n-list-item>
        <n-list-item key="signalr">
            SignalR API
            <template #suffix>
                <n-icon-wrapper :size="26" :border-radius="12">
                    <n-icon :size="20" :component="signalREnabled ? Checkmark : Close" />
                </n-icon-wrapper>
            </template>
        </n-list-item>
        <template #footer><n-statistic label="Workers" :value="workerCount" /></template>
    </n-list>
</template>
<script setup lang="ts">
import { NIcon, useNotification, NList, NListItem, NIconWrapper, NStatistic } from "naive-ui";
import { Checkmark, Close } from "@vicons/ionicons5";
import { ref, onBeforeMount } from "vue";
import { useRazorRequest } from "../request-plugin";

// const props = withDefaults(defineProps<{}>(), {});

const notif = useNotification();
const restEnabled = ref(false);
const signalREnabled = ref(false);
const workerCount = ref(0);
const iconSrc = '/images/icon.png';

onBeforeMount(() => {
    var headers = useRazorRequest();
    fetch('/?handler=serverConfiguration', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            restEnabled.value = json.services.rest;
            signalREnabled.value = json.services.signalr;
            workerCount.value = json.workers;
        });
    });
});
</script>