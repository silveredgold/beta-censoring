<template>
    <n-list bordered>
        <n-list-item>
            <n-statistic label="Average Censoring Time" :value="averageCensorTimes">
                <template #suffix>s</template>
            </n-statistic>
            <n-statistic label="Highest Censoring Time" :value="highestCensorTime">
                <template #suffix>s</template>
            </n-statistic>
        </n-list-item>
    </n-list>
</template>
<script setup lang="ts">
import { NList, NListItem, NStatistic } from "naive-ui";
import { Duration } from "luxon";
import { computed, toRefs } from "vue";
import type {CensoringRecord} from '../util'

const props = withDefaults(defineProps<{
    records: CensoringRecord[]|undefined
}>(), {});

const {records} = toRefs(props)

const censorTimes = computed(() => {
    if (records.value !== undefined) {
        return records.value.filter(m => !!m).filter(o => !!o.censoring?.censoringTime).map(o => o.censoring.censoringTime).map(s => Duration.fromISOTime(s));
    }
    return [];
});

const averageCensorTimes = computed(() => censorTimes.value.reduce(function (p, c, i) { return p + (c.as('seconds') - p) / (i + 1) }, 0).toFixed(3));
const highestCensorTime = computed(() => Math.max(...censorTimes.value.map(c => c.as('seconds'))));

</script>