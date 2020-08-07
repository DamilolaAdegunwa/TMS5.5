<template>
  <div>
    <search-time-range v-model="statInput.startCTime" class="margin-bottom-10"></search-time-range>
    <van-panel title="按年龄段" class="margin-bottom-10">
      <ve-histogram
        :data="statByAgeRangeData"
        :settings="statByAgeRangeChartSettings"
        :extend="statByAgeRangeChartOptions"
        :data-empty="statByAgeRangeData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        height="300px"
      ></ve-histogram>
    </van-panel>
    <van-panel title="按性别" class="margin-bottom-10">
      <ve-pie
        :data="statBySexData"
        :settings="statBySexChartSettings"
        :data-empty="statBySexData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        height="230px"
      ></ve-pie>
    </van-panel>
    <van-panel title="按地区">
      <ve-bar
        :data="statByAreaData"
        :settings="statByAreaChartSettings"
        :extend="statByAreaChartOptions"
        :data-empty="statByAreaData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        :height="barHeight"
      ></ve-bar>
    </van-panel>
  </div>
</template>

<script>
import VeHistogram from "v-charts/lib/histogram.common.js";
import VeBar from "v-charts/lib/bar.common.js";
import VePie from "v-charts/lib/pie.common.js";
import "v-charts/lib/style.css";
import SearchTimeRange from "@/components/SearchTimeRange.vue";
import dayjs from "dayjs";
import ticketService from "@/services/ticketService.js";

export default {
  components: {
    VeHistogram,
    VeBar,
    VePie,
    SearchTimeRange
  },
  data() {
    return {
      statInput: {
        startCTime: dayjs().toDateString(),
        endCTime: dayjs().toEndDateTimeString()
      },
      statByAgeRangeChartSettings: {
        itemStyle: {
          color: "#5AB1EF"
        },
        label: {
          show: true,
          position: "top"
        }
      },
      statByAgeRangeChartOptions: {
        grid: {
          top: 30,
          bottom: 30
        },
        series: {
          barMaxWidth: 25
        }
      },
      statByAgeRangeData: {
        columns: ["年龄段", "数量"],
        rows: []
      },
      statBySexChartSettings: {
        radius: 80,
        offsetY: 120,
        hoverAnimation: false,
        label: {
          formatter: "{b}: {d}%"
        }
      },
      statBySexData: {
        columns: [],
        rows: []
      },
      statByAreaChartSettings: {
        itemStyle: {
          color: "#5AB1EF"
        },
        dataOrder: {
          label: "合计",
          order: "desc"
        },
        label: {
          show: true,
          position: "insideLeft"
        }
      },
      statByAreaChartOptions: {
        grid: {
          top: 30,
          bottom: 0,
          right: 50
        },
        xAxis: {
          show: false
        },
        series: {
          barMaxWidth: 20
        }
      },
      statByAreaData: {
        columns: ["地区", "合计"],
        rows: []
      }
    };
  },
  computed: {
    barHeight() {
      let height =
        (this.statByAreaData.rows.length + 1) * 25 +
        this.statByAreaChartOptions.grid.top +
        this.statByAreaChartOptions.grid.bottom;
      if (this.statByAreaData.rows.length === 0) {
        height = 100;
      }
      return `${height}px`;
    }
  },
  watch: {
    "statInput.startCTime": function() {
      this.loadData();
    }
  },
  methods: {
    loadData() {
      ticketService.statTouristByAgeRangeAsync(this.statInput).then(response => {
        this.statByAgeRangeData.rows = [];
        if (response.data.length <= 0) {
          return;
        }
        let keys = Object.keys(response.data[0]);
        keys.forEach(key => {
          if (key != "年份" && key != "合计") {
            this.statByAgeRangeData.rows.push({
              年龄段: key,
              数量: response.data.map(item => item[key]).reduce((s, j) => s + j)
            });
          }
        });
      });
      ticketService.statTouristBySexAsync(this.statInput).then(response => {
        this.statBySexData.columns = response.columns;
        this.statBySexData.rows = response.data;
      });
      ticketService.statTouristByAreaAsync(this.statInput).then(response => {
        this.statByAreaData.rows = response.data;
      });
    }
  },
  created() {
    this.loadData();
  }
};
</script>
