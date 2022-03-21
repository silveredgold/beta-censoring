<template>
    <n-tabs
        size="large"
        justify-content="space-evenly"
        @before-leave="beforeTabChange"
        @update:value="onTabChange"
    >
        <template #prefix>Categories</template>
        <template #suffix>
            <n-popover trigger="hover" placement="left">
                <template #trigger>
                    <n-button ghost strong circle type="primary" size="small">
                        <template #icon>
                            <n-icon :component="Help" />
                        </template>
                    </n-button>
                </template>
                <n-thing style="max-width: 40rem;" title="Previewing your stickers">
                    Click on a category name to preview all of the stickers available for that category. Click any sticker to open the image in more detail.
                    <template #footer>
                        <n-text depth="3">Be patient! Previews can take a bit to load.</n-text>
                    </template>
                </n-thing>
            </n-popover>
        </template>
        <n-tab-pane
            v-for="cat in categories"
            display-directive="show:lazy"
            v-if="allStickers"
            :name="cat"
        >
            <template #tab>
                <n-space>
                    {{ cat }}
                    <n-badge :value="allStickers[cat].length" :max="99" type="info" />
                </n-space>
            </template>
            <n-space>
                <n-image-group>
                    <n-space>
                        <n-image
                            v-for="img in allStickers[cat]"
                            width="100"
                            :src="toDataUrl(img.value, img.key)"
                        />
                    </n-space>
                </n-image-group>
            </n-space>
        </n-tab-pane>
    </n-tabs>
</template>
<script setup lang="ts">
import { NIcon, NPopover, NButton, NSpace, NImage, NImageGroup, NTabs, NTabPane, useLoadingBar, NBadge, NThing, NText } from "naive-ui";
import { Help } from "@vicons/ionicons5";
import { ref, onBeforeMount, Ref } from "vue";
import { useRazorRequest } from "../request-plugin";

const loadingBar = useLoadingBar();
const categories: Ref<string[]> = ref([])
const allStickers: Ref<{ [key: string]: { key: string, value: string }[] } | undefined> = ref(undefined);

onBeforeMount(() => {
    const headers = useRazorRequest();
    loadingBar.start();
    fetch('/config?handler=availableStickers', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            categories.value = Object.keys(json);
            allStickers.value = json;
            loadingBar.finish();
        });
    });
});

const beforeTabChange = (tabName: string) => {
    return true;
}

const onTabChange = (value: string) => {
    console.log('after');
}

const toDataUrl = (encoded: string, type: string) => {
    return `data:${type};base64,${encoded}`;
}
</script>