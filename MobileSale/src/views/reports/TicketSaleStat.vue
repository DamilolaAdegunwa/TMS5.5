<template>
  <section v-if="pageLoaded">
    <search-time-range v-model="input.startCTime" class="margin-bottom-10"></search-time-range>
    <van-panel title="销售渠道" class="margin-bottom-10">
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
    <van-panel title="票类统计">
      <table class="table">
        <tr class="th">
          <td>票类</td>
          <td>人数</td>
          <td>金额</td>
        </tr>
        <tr v-for="(row, index) in rows" :key="index">
          <td>{{ row.ticketTypeName }}</td>
          <td>{{ row.realPersonNum }}</td>
          <td>{{ row.realMoney.toFixed(2) }}</td>
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
import ticketService from "./../../services/ticketService.js";

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
          label: "realMoney",
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
        columns: ["tradeSourceName", "realMoney"],
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

        const statByTicketTypeTask = this.statByTicketType();
        const statByTradeSourceTask = this.statByTradeSource();

        await Promise.all([statByTicketTypeTask, statByTradeSourceTask]);
      } finally {
        this.loaded();
      }
    },
    async statByTicketType() {
      const input = { statType: 6, ...this.input };
      const items = await ticketService.statTicketSaleAsync(input);
      this.rows = items;
    },
    async statByTradeSource() {
      const input = { statType: 7, ...this.input };
      const items = await ticketService.statTicketSaleAsync(input);
      const totalIndex = items.findIndex(r => r.statType == "合计");
      items.splice(totalIndex, 1);
      this.chartData.rows = items.map(r => {
        return {
          tradeSourceName: r.statType,
          realMoney: r.realMoney.toFixed(2)
        };
      });
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
