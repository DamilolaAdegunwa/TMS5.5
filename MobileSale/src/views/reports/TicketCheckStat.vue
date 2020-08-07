<template>
  <section v-if="pageLoaded">
    <search-time-range v-model="input.startCTime" class="margin-bottom-10"></search-time-range>
    <van-panel title="入园人数" class="margin-bottom-10">
      <ve-line
        :data="chartData"
        :extend="chartOptions"
        :data-empty="chartData.rows.length === 0"
        :tooltip-visible="false"
        height="250px"
      ></ve-line>
    </van-panel>
    <van-panel title="检票明细">
      <table class="table">
        <tr class="th">
          <td>园区</td>
          <td>检票点</td>
          <td>次数</td>
        </tr>
        <tr v-for="(row, index) in rows" :key="index">
          <td>{{ row.groundName }}</td>
          <td>{{ row.gateGroupName }}</td>
          <td>{{ row.checkNum }}</td>
        </tr>
      </table>
    </van-panel>
  </section>
</template>

<script>
import dayjs from "dayjs";
import VeLine from "v-charts/lib/line.common.js";
import "v-charts/lib/style.css";
import SearchTimeRange from "@/components/SearchTimeRange.vue";
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import { rowConvertToColumn } from "./../../utils/data.js";
import ticketService from "./../../services/ticketService.js";

export default {
  mixins: [mobileMixin],
  components: { VeLine, SearchTimeRange },
  data() {
    return {
      input: {
        startCTime: dayjs().toDateString(),
        endCTime: dayjs().toEndDateTimeString()
      },
      chartOptions: {
        grid: {
          top: 20
        },
        legend: {
          bottom: 10
        },
        series: {
          type: "line",
          label: {
            show: true
          }
        }
      },
      chartData: {
        columns: [],
        rows: []
      },
      rows: []
    };
  },
  watch: {
    "input.startCTime": async function() {
      await this.loadData();
    }
  },
  async created() {
    await this.loadData();
    this.pageLoaded = true;
  },
  methods: {
    async loadData() {
      try {
        this.loading();

        const statByGroundAndTimeTask = this.statByGroundAndTime();
        const statByGroundAndGateGroupTask = this.statByGroundAndGateGroup();

        await Promise.all([statByGroundAndTimeTask, statByGroundAndGateGroupTask]);
      } finally {
        this.loaded();
      }
    },
    async statByGroundAndTime() {
      const result = await ticketService.statTicketCheckByGroundAndTimeAsync(this.input);

      if (result.data.length <= 0) {
        this.chartData.columns = [];
        this.chartData.rows = [];
        return;
      }

      const rows = rowConvertToColumn(
        result.data.filter(d => d.出入类型 == "入口"),
        result.columns,
        result.columns[0],
        "xAxisName",
        ["出入类型", "21点后"]
      );

      this.chartData.columns = Object.keys(rows[0]);
      this.chartData.rows = rows;
    },
    async statByGroundAndGateGroup() {
      const result = await ticketService.statTicketCheckInByGroundAndGateGroupAsync(this.input);
      this.rows = result.data;
    }
  }
};
</script>

<style lang="scss" scoped>
.table {
  padding: 10px 15px;
  width: 100%;
  text-align: left;

  .th {
    font-weight: bold;
  }
}
</style>
