<template>
    <n-grid :cols="6">
        <n-grid-item :span="4">
            <ApexCharts v-if="series.length > 0" type="bar" :options="options" :series="series"></ApexCharts>
        </n-grid-item>
        <n-grid-item :span="2">
            <n-list bordered>
                <n-list-item>
                    <n-statistic label="Average Inference Time" :value="averageModelRunTime">
                        <template #suffix>s</template>
                    </n-statistic>
                </n-list-item>
                <n-list-item>
                    <n-statistic label="Average Loading Time" :value="averageLoadTime">
                        <template #suffix>s</template>
                    </n-statistic>
                </n-list-item>
            </n-list>
        </n-grid-item>
    </n-grid>
</template>
<script setup lang="ts">
import { NList, NListItem, NStatistic, NGrid, NGridItem } from "naive-ui";
import { Duration } from "luxon";
import { ref, watch, Ref, computed, toRefs } from "vue";
import ApexCharts from "vue3-apexcharts";
import { ApexOptions } from "apexcharts";
import type { CensoringRecord } from "../util";

const props = withDefaults(defineProps<{
    records: CensoringRecord[] | undefined
}>(), {});

const { records } = toRefs(props)

const performanceSessions = computed(() => {
    if (records.value !== undefined) {
        return records.value.filter(r => !!r?.inference?.modelRunTime);
    }
    return [];
});

const modelRunTimes = computed(() => performanceSessions.value.map(o => o.inference.modelRunTime).map(s => Duration.fromISOTime(s)));

const tensorLoadTimes = computed(() => performanceSessions.value.flatMap(o => o.inference.tensorLoadTime).map(s => Duration.fromISOTime(s)));

const imageLoadTimes = computed(() => performanceSessions.value.flatMap(o => o.inference.imageLoadTime).map(s => Duration.fromISOTime(s)));

const averageModelRunTime = computed(() => modelRunTimes.value.reduce(function (p, c, i) { return p + (c.as('seconds') - p) / (i + 1) }, 0).toFixed(3));
const averageLoadTime = computed(() => tensorLoadTimes.value.map((e, i) => [e, imageLoadTimes.value[i]]).map(c => c[0].as('seconds')+c[1].as('seconds')).reduce(function (p, c, i) { return p + (c - p) / (i + 1) }, 0).toFixed(3));

watch(records, () => {
    series.value = [{
        name: 'Image Loading',
        data: imageLoadTimes.value.slice(0, 10).map(m => m.as('seconds').toFixed(2))
    }, {
        name: 'Tensor Loading',
        data: tensorLoadTimes.value.slice(0, 10).map(m => m.as('seconds').toFixed(2))
    }, {
        name: 'Model Run',
        data: modelRunTimes.value.slice(0, 10).map(m => m.as('seconds').toFixed(2))
    }];
})

const series: Ref<any[]> = ref([]);

const options: ApexOptions = {
    chart: {
        type: 'bar',
        height: 350,
        stacked: true,
    },
    plotOptions: {
        bar: {
            horizontal: true,
        },
    },
    stroke: {
        width: 1,
        colors: ['#fff']
    },
    title: {
        text: 'Inference Performance (last 10 requests)'
    },
    xaxis: {
        categories: [...Array(performanceSessions.value.slice(0, 10).length).keys()].map(k => k + 1),
        labels: {
            formatter: function (val: string) {
                return val + "s"
            }
        }
    },
    yaxis: {
        title: {
            text: undefined
        },
    },
    tooltip: {
        y: {
            formatter: function (val: number) {
                return val + "s"
            }
        }
    },
    fill: {
        opacity: 1
    },
    legend: {
        position: 'top',
        horizontalAlign: 'left',
        offsetX: 40
    }
};



</script>