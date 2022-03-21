<template>
    <n-list bordered>
        <template #header>Enabled Services</template>
        <n-list-item v-for="service of services" :key="service[0]">
            {{service[0]}} API 
            <template #suffix>
                <n-icon-wrapper :size="26" :border-radius="12" :color="service[1] ? themeVars.successColor : themeVars.warningColor">
                    <n-icon :size="20" :component="service[1] ? Checkmark : Close" />
                </n-icon-wrapper>
            </template>
        </n-list-item>
        <template #footer>
            <n-statistic v-for="(component, name) in components" :key="component" :label="name.toString()" :value="component" />
        </template>
    </n-list>
</template>
<script setup lang="ts">
import { useThemeVars, NIcon, NList, NListItem, NIconWrapper, NStatistic } from "naive-ui";
import { Checkmark, Close } from "@vicons/ionicons5";
import { ref, Ref, onMounted } from "vue";
import { useRazorRequest } from "../request-plugin";

const themeVars = useThemeVars();

const services: Ref<Map<string, boolean>> = ref(new Map());
const components: Ref<{[name: string]: string}|undefined> = ref(undefined);

onMounted(() => {
    var headers = useRazorRequest();
    fetch('/?handler=serverConfiguration', { headers }).then(resp => {
        console.log(resp);
        resp.json().then(json => {
            console.log(json);
            for (const serviceName of Object.keys(json.services)) {
                services.value.set(serviceName, json.services[serviceName]);
            }
            components.value = json.components;
        });
    });
});
</script>