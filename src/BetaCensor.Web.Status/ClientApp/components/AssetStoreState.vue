<template>
    <n-list bordered>
        <template #header>Loaded Assets</template>
        <n-list-item key="stickers">
            Sticker Categories
            <n-empty description="No stickers available" v-if="!stickers || stickers.length == 0" />
            <n-list bordered v-if="stickers && stickers.length > 0">
                <n-list-item v-for="cat in stickers" v-bind:key="cat">
                    {{ cat }}
                    <template #suffix>
                        <n-icon-wrapper :size="20" :border-radius="10" style="display: flex;" :color="themeVars.successColor">
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
import { useThemeVars, NIcon, NList, NListItem, NIconWrapper, NEmpty } from "naive-ui";
import { Checkmark, Close } from "@vicons/ionicons5";
import { ref, onMounted, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

// const props = withDefaults(defineProps<{}>(), {});

const themeVars = useThemeVars();
const stickers: Ref<string[]> = ref([])

onMounted(() => {
    fetch('/assets/categories?type=stickers').then(resp => {
        console.log(resp);
        resp.json().then(json => {
            stickers.value = json
        });
    });
});
</script>