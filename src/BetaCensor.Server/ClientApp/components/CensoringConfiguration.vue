<template>
    <n-list bordered>
        <template #header>Censoring Configuration</template>
        <n-list-item v-for="type in types" v-bind:key="type">
            {{type}}
            <template #suffix>
                <n-icon-wrapper :size="26" :border-radius="12">
                    <n-icon :size="20" :component="CodeWorking" />
                </n-icon-wrapper>
            </template>
        </n-list-item>
        <template #footer>
            <n-thing title="Censoring Provider" :description="provider" style="margin-bottom: 0.5rem;" />
            <n-thing title="Image Handler" :description="imageHandler" />
        </template>
    </n-list>
</template>
<script setup lang="ts">
import { NIcon, useNotification, NList, NListItem, NIconWrapper, NStatistic, NThing } from "naive-ui";
import { CodeWorking } from "@vicons/ionicons5";
import { ref, onBeforeMount, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

// const props = withDefaults(defineProps<{}>(), {});

const notif = useNotification();
const provider: Ref<string> = ref('unknown');
const imageHandler: Ref<string> = ref('unknown');
const types: Ref<string[]> = ref([]);

onBeforeMount(() => {
    const headers = useRazorRequest();
    fetch('/censoring/info', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            provider.value = json.provider;
            imageHandler.value = json.imageHandler;
            types.value = json.types ?? [];
        });
    });
});
</script>