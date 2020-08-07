<template>
  <section v-if="pageLoaded">
    <search-time-range v-model="input.startCTime" class="margin-bottom-10"></search-time-range>
    <van-panel title="销售类型" class="margin-bottom-10">
      <ve-bar
        :data="chartData"
        :settings="chartSettings"
        :extend="chartOptions"
        :data-empty="chartData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        :height="barHeight"
      ></ve-bar>
    </van-panel>
    <van-panel title="付款方式">
      <table class="table">
        <tr class="th">
          <td>付款方式</td>
          <td>金额</td>
        </tr>
        <tr v-for="(row, index) in rows" :key="index">
          <td>{{ row.payTypeName }}</td>
          <td>{{ row.payMoney.toFixed(2) }}</td>
        </tr>
      </table>
    </van-panel>
  </section>
</template>

<script>
import dayjs from "dayjs";
import VeBar from "v-charts/lib/bar.common.js";
import "v-charts/lib/style.css";
import SearchTimeRange from "@/components/SearchTimeRange.vue";
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import tradeService from "./../../services/tradeService.js";

export default {
  mixins: [mobileMixin],
  components: {
    VeBar,
    SearchTimeRange
  },
  data() {
    return {
      input: {
        startCTime: dayjs().toDateString(),
        endCTime: dayjs().toEndDateTimeString()
      },
      chartSettings: {
        itemStyle: {
          color: "#5AB1EF"
        },
        dataOrder: {
          label: "totalMoney",
          order: "desc"
        },
        label: {
          show: true,
          position: "insideLeft"
        }
      },
      chartOptions: {
        grid: {
          top: 20,
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
      chartData: {
        columns: ["tradeTypeName", "totalMoney"],
        rows: []
      },
      rows: []
    };
  },
  computed: {
    barHeight() {
      let height =
        (this.chartData.rows.length + 1) * 25 +
        this.chartOptions.grid.top +
        this.chartOptions.grid.bottom;

      return `${height}px`;
    }
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

        const statTradeTask = this.statTrade();
        const statPayDetailTask = this.statPayDetail();

        await Promise.all([statTradeTask, statPayDetailTask]);
      } finally {
        this.loaded();
      }
    },
    async statTrade() {
      const input = { statType: 6, ...this.input };
      const result = await tradeService.statTradeAsync(input);
      const totalIndex = result.data.findIndex(r => r.tradeTypeName == "合计");
      result.data.splice(totalIndex, 1);

      this.chartData.rows = result.data.map(r => {
        return {
          tradeTypeName: r.tradeTypeName,
          totalMoney: r.totalMoney.toFixed(2)
        };
      });
    },
    async statPayDetail() {
      const rows = await tradeService.statPayDetailJbAsync(this.input);
      if (rows.length > 0) {
        rows.push({
          payTypeName: "合计",
          payMoney: rows.map(r => r.payMoney).reduce((i, j) => i + j)
        });
      }

      this.rows = rows;
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
