<template>
    <n-list bordered>
        <template #header>Loaded Stickers</template>
        <n-list-item v-for="cat in categories" v-if="allStickers">
            <n-statistic :label="cat" :value="allStickers[cat]" />
        </n-list-item>
        <template #footer>
            {{categories.length}} categories loaded.
        </template>
    </n-list>
</template>
<script setup lang="ts">
import { NIcon, NList, NListItem, NIconWrapper, NStatistic } from "naive-ui";
import { ref, onBeforeMount, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

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