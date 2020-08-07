<template>
  <div>
    <search-time-range v-model="statInput.startDate" class="margin-bottom-10"></search-time-range>
    <van-panel title="客流来源统计" class="margin-bottom-10">
      <ve-ring
        :data="statTicketCheckByTradeSourceData"
        :settings="statTicketCheckByTradeSourceChartSettings"
        :data-empty="statTicketCheckByTradeSourceData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        height="230px"
      ></ve-ring>
    </van-panel>
    <van-panel title="展厅客流统计">
      <ve-histogram
        :data="stadiumTicketCheckData"
        :settings="stadiumTicketCheckChartSettings"
        :extend="stadiumTicketCheckChartOptions"
        :data-empty="stadiumTicketCheckData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="false"
        height="300px"
      ></ve-histogram>
    </van-panel>
  </div>
</template>

<script>
import VeHistogram from "v-charts/lib/histogram.common.js";
import VeRing from "v-charts/lib/ring.common.js";
import "v-charts/lib/style.css";
import SearchTimeRange from "@/components/SearchTimeRange.vue";
import dayjs from "dayjs";
import ticketService from "@/services/ticketService.js";

export default {
  components: {
    VeHistogram,
    VeRing,
    SearchTimeRange
  },
  data() {
    return {
      statInput: {
        startDate: dayjs().toDateString(),
        endDate: dayjs()
          .addDays(1)
          .toDateString()
      },
      stadiumTicketCheckChartSettings: {
        itemStyle: {
          color: "#5AB1EF"
        },
        label: {
          show: true,
          position: "top"
        }
      },
      stadiumTicketCheckChartOptions: {
        grid: {
          top: 30,
          bottom: 30
        },
        series: {
          barMaxWidth: 25
        }
      },
      stadiumTicketCheckData: {
        columns: ["gateName", "checkIn"],
        rows: []
      },
      statTicketCheckByTradeSourceChartSettings: {
        radius: [40, 70],
        offsetY: 120,
        hoverAnimation: false,
        label: {
          formatter: "{b}: {d}%"
        }
      },
      statTicketCheckByTradeSourceData: {
        columns: ["tradeSourceName", "checkNum"],
        rows: []
      }
    };
  },
  watch: {
    "statInput.startDate": function() {
      this.loadData();
    }
  },
  methods: {
    loadData() {
      ticketService.getStadiumTicketCheckOverviewAsync(this.statInput).then(response => {
        this.stadiumTicketCheckData.rows = response;
      });
      ticketService
        .statTicketCheckByTradeSourceAsync({
          StartCTime: this.statInput.startDate,
          EndCTime: dayjs().toEndDateTimeString(),
          StatType: 3
        })
        .then(response => {
          if (this.statTicketCheckByTradeSourceData.rows.length > 0) {
            this.statTicketCheckByTradeSourceData.rows = [];
          }
          if (response.data.length <= 0) {
            return;
          }
          const keys = Object.keys(response.data[0]).filter(key => key != "年份" && key != "合计");
          keys.forEach(key => {
            const checkNum = response.data.map(item => item[key]).reduce((s, j) => s + j);
            if (checkNum > 0) {
              this.statTicketCheckByTradeSourceData.rows.push({
                tradeSourceName: key == "本地" ? "自助机" : key,
                checkNum: checkNum
              });
            }
          });
        });
    }
  },
  created() {
    this.loadData();
  }
};
</script>
