<template>
    <n-list bordered>
        <template #header>Loaded Assets</template>
        <n-list-item key="stickers">
            Sticker Categories
            <n-list bordered>
                <n-list-item v-for="cat in stickers" v-bind:key="cat">
            {{cat}}
            <template #suffix>
                <n-icon-wrapper :size="20" :border-radius="10" style="display: flex;">
                    <n-icon :size="14" :component="Checkmark" />
                </n-icon-wrapper>
            </template>
        </n-list-item>
            </n-list>
        </n-list-item>
        <template #footer>
            <!-- <n-statistic label="Workers" :value="workerCount" /> -->
        </template>
    </n-list>
</template>
<script setup lang="ts">
import { NIcon, useNotification, NList, NListItem, NIconWrapper, NStatistic } from "naive-ui";
import { Checkmark, Close } from "@vicons/ionicons5";
import { ref, onBeforeMount, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

// const props = withDefaults(defineProps<{}>(), {});

const notif = useNotification();
const stickers: Ref<string[]> = ref([])

onBeforeMount(() => {
    const headers = useRazorRequest();
    fetch('/?handler=assets', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            stickers.value = json.stickerCategories
        });
    });
});
</script>