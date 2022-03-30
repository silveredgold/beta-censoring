<script setup lang="ts">
import SecureOnly from './SecureOnly.vue';
import PreviewFeature from './PreviewFeature.vue';
import { NText, NSpace, NSteps, NStep, NThing, NIcon, NButton, NCard } from "naive-ui";
import { HelpCircle, Settings } from "@vicons/ionicons5";
import { Ref, ref } from 'vue';

const currentStep = ref(1);
const currentStepStatus: Ref<"wait" | "error" | "finish" | "process"> = ref('process');

const setWorkerCount = ref(false);
const setOptimization = ref(false);
const setStartup = ref(false);

const openDocs = (anchor?: string) => {
    window.open('https://silveredgold.github.io/beta-censoring/content/configuration#' + (anchor ?? ''), '_blank');
}

</script>
<template>
    <SecureOnly>
        <PreviewFeature>
        <n-steps :current="currentStep" :status="currentStepStatus" vertical>
        <n-step title="About">
            <n-card v-if="currentStep ==1">
                <n-thing title="Server Configuration Wizard" v-if="currentStep == 1">
                    <template #avatar><n-icon :component="HelpCircle" /></template>
                    <n-text tag="p">This wizard will help you to generate a configuration file to tweak the behaviour of the server without needing to write your own configuration file.</n-text>
                    <n-text tag="p">We will ask you some questions then generate a configuration you can download or save to the server, ready for the next time it starts.</n-text>
                    <n-text tag="p">Note that there's plenty more options for configuration beyond what's presented here in the wizard. In particular, the wizard will not include sticker configuration. Check out the <n-button text dashed size="small" @click="openDocs()">user guide</n-button> for all the options.</n-text>
                    <template #action>
                        <n-button @click="currentStep++">Next</n-button>
                    </template>
                </n-thing>
            </n-card>
        </n-step>
        <n-step title="Server Options">
            <n-card v-if="currentStep == 2" title="Server Configuration Options">
                
                <n-thing title="Worker Count">
                    <template #avatar><n-icon :component="Settings" /></template>
                    <n-text tag="p">This is how many workers .</n-text>
                    <n-text tag="p">We will ask you some questions then generate a configuration you can download or save to the server, ready for the next time it starts.</n-text>
                    <n-text tag="p">Note that there's plenty more options for configuration beyond what's presented here in the wizard. In particular, the wizard will not include sticker configuration. Check out the <n-button text dashed size="small" @click="openDocs()">user guide</n-button> for all the options.</n-text>
                    <template #action>
                        <n-button @click="currentStep++">Next</n-button>
                    </template>
                </n-thing>
            </n-card>

        </n-step>
        </n-steps>
        </PreviewFeature>
    </SecureOnly>
</template>
