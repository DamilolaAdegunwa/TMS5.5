<template>
  <div class="page">
    <nav-bar title="检票统计" back="/account" />

    <van-cell class="header">
      <p class="header-text">
        <font-awesome-icon
          icon="ticket-alt"
          color="#009DDC"
          size="lg"
        />检票数量
      </p>
      <p class="header-number">{{ totalNum }}</p>
    </van-cell>

    <div class="chart">
      <ve-ring
        :data="chartData"
        :settings="chartSettings"
        :data-empty="chartData.rows.length === 0"
        :tooltip-visible="false"
        :legend-visible="true"
        height="300px"
      ></ve-ring>
    </div>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import VeRing from "v-charts/lib/ring.common.js";
import "v-charts/lib/style.css";
import moment from "moment";
import ticketService from "./../../services/ticketService.js";
import { isNullOrUndefined } from "util";
import { App } from "./../../handset-sdk.js";

export default {
  name: "AccountCheckStat",
  components: { NavBar, VeRing },
  data() {
    return {
      statInput: {
        startCTime: moment().format("YYYY-MM-DD"),
        endCTime: moment().format("YYYY-MM-DD 23:59:59"),
        gateId: this.getGateId(),
        checkerId: this.getStaffId(),
        statType: 0
      },
      chartSettings: {
        radius: [40, 70],
        offsetY: 180,
        hoverAnimation: false,
        label: {
          formatter: "{c}"
        }
      },
      chartData: {
        columns: ["ticketTypeName", "checkNum"],
        rows: []
      }
    };
  },
  computed: {
    totalNum() {
      if (this.chartData.rows.length == 0) return;
      return this.chartData.rows.map(d => d.checkNum).reduce((i, j) => i + j);
    }
  },
  async created() {
    const result = await ticketService.statTicketCheckInByTicketTypeAsync(
      this.statInput
    );
    this.chartData.rows = result.data;
  },
  methods: {
    getStaffId() {
      let staffId = sessionStorage.getItem("staffId");
      return Number(staffId);
    },
    getGateId() {
      if (App) {
        let result = App.getValue("gateId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("gateId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.header {
  margin-bottom: 10px;

  .header-text {
    .fa-ticket-alt {
      transform: translateY(-5px) rotate(-45deg);
      margin-right: 8px;
    }

    text-align: center;
    color: #999999;
    margin-top: 16px;
  }

  .header-number {
    text-align: center;
    font-size: 28px;
    letter-spacing: -0.5px;
    font-family: "DINOT-Medium", sans-serif;
    color: #fe5906;
  }
}

.chart {
  background-color: white;
  padding-top: 20px;
}
</style>
