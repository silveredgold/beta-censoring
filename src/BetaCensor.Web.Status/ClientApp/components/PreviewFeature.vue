<script setup lang="ts">
import { NAlert, NThing, NText, NButton, NSpace } from "naive-ui";
import { computed, ref } from "vue";

const accepted = ref(false);

const accept = () => {
    accepted.value = true;
}

</script>
<template>
    <n-alert title="Experimental Preview Feature" type="warning" v-if="!accepted">
        <n-thing title="Read the following before proceeding!">
            <n-text tag="p">This is an experimental preview feature. That means it might not be fully ready, could still have bugs or might not be fully tested yet. If you're okay with that, click the button below to accept the risk and continue.</n-text>
            <n-text tag="p">Just keep in mind that this feature might not be as polished as you'd expect. If you're not ready for that, be patient and the feature will be stable soon enough!</n-text>
            <template #action>
                <n-space>
                    <n-button size="large" @click="accept">
                        I understand and wish to proceed
                    </n-button>
                </n-space>
            </template>
        </n-thing>
    </n-alert>
    <template v-if="accepted">
        <slot></slot>
    </template>
</template>