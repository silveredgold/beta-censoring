<template>
    <n-list bordered>
        <template #header>Loaded Stickers</template>
        <n-list-item v-for="cat in categories" v-if="allStickers">
            <n-statistic :label="cat" :value="allStickers[cat]" />
            <!-- {{cat}}
            <template #suffix>
                <n-icon-wrapper :size="20" :border-radius="10" style="display: flex;">
                    <n-icon :size="14" :component="Checkmark" />
                </n-icon-wrapper>
            </template> -->
        </n-list-item>
        <template #footer>
            {{categories.length}} categories loaded.
            <!-- <n-statistic label="Workers" :value="workerCount" /> -->
        </template>
    </n-list>
</template>
<script setup lang="ts">
import { NIcon, useNotification, NList, NListItem, NIconWrapper, NStatistic } from "naive-ui";
import { ref, onBeforeMount, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

// const props = withDefaults(defineProps<{}>(), {});

const notif = useNotification();
const categories: Ref<string[]> = ref([])
const allStickers: Ref<{[key: string]: number}|undefined> = ref(undefined);

onBeforeMount(() => {
    const headers = useRazorRequest();
    fetch('/config?handler=availableStickerSummary', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            console.log(json);
            categories.value = Object.keys(json);
            allStickers.value = json;
        });
    });
});
</script>