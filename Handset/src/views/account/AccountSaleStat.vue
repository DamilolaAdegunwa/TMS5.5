<template>
  <div class="page">
    <nav-bar title="售票统计" back="/account" />

    <van-cell class="title">
      <font-awesome-icon icon="chart-line" color="#009DDC" />
      <span>售票统计</span>
    </van-cell>

    <van-cell-group>
      <van-cell class="table-header">
        <van-row>
          <van-col span="6">票类名称</van-col>
          <van-col span="6">单价</van-col>
          <van-col span="6">数量</van-col>
          <van-col span="6">金额</van-col>
        </van-row>
      </van-cell>
      <div class="table-body">
        <van-cell
          class="table-row"
          v-for="item in chartData.items"
          :key="item.key"
        >
          <van-row>
            <van-col span="6">{{ item.ticketTypeName }}</van-col>
            <van-col span="6">{{ item.reaPrice }}</van-col>
            <van-col span="6">{{ item.personNum }}</van-col>
            <van-col span="6">{{ item.reaMoney }}</van-col>
          </van-row>
        </van-cell>
      </div>
    </van-cell-group>
    <van-tabbar>
      <van-tabbar-item>
        总数量：
        <span class="number">{{ chartData.totalPersonNum }}</span>
      </van-tabbar-item>
      <van-tabbar-item>
        总金额：
        <span class="number">￥ {{ chartData.totalReaMoney }}</span>
      </van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import { App } from "../../handset-sdk.js";
import moment from "moment";
import math from "mathjs";
import { isNullOrUndefined } from "util";
import ticketService from "../../services/ticketService.js";

export default {
  name: "AccountSaleStat",
  components: { NavBar },
  data() {
    return {
      statInput: {
        startCTime: moment().format("YYYY-MM-DD"),
        endCTime: moment().format("YYYY-MM-DD 23:59:59"),
        cashierId: this.getStaffId(),
        salePointId: this.getSalePointId()
      },

      chartData: {
        totalPersonNum: 0,
        totalTicketNum: 0,
        totalReaMoney: 0.0,
        items: []
      }
    };
  },
  async created() {
    const result = await ticketService.statTicketSaleByTradeSourceAsync(
      this.statInput
    );
    let totalPersonNum = 0;
    let totalTicketNum = 0;
    let totalReaMoney = 0.0;
    result.forEach(item => {
      totalPersonNum = math.eval(totalPersonNum + "+" + item.personNum);
      totalTicketNum = math.add(totalTicketNum, item.ticketNum);
      totalReaMoney = math
        .chain(totalReaMoney)
        .add(item.reaMoney)
        .done();

      this.chartData.items.push(item);
    });
    this.chartData.totalPersonNum = totalPersonNum.toFixed(0);
    this.chartData.totalTicketNum = totalTicketNum.toFixed(0);
    this.chartData.totalReaMoney = totalReaMoney.toFixed(2);
  },
  methods: {
    getSalePointId() {
      if (App) {
        let result = App.getValue("salePointId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("salePointId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getStaffId() {
      let staffId = sessionStorage.getItem("staffId");
      return Number(staffId);
    }
  }
};
</script>

<style lang="scss" scoped>
.title {
  font-size: 18px;

  span {
    margin-left: 12px;
  }
}

.table-header {
  background: linear-gradient(to bottom, #ffffff, #f0f0f0, #d8d8d8);

  .van-col {
    text-align: center;
  }
}

.table-body {
  // 总高度减去其它部分
  max-height: calc(100vh - 200px);
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
}

.table-row .van-col {
  color: #999999;
  text-align: center;
}

.van-tabbar {
  height: 58px;

  .van-tabbar-item {
    height: 58px;
    font-size: 16px;
    color: #323233;

    .number {
      font-size: 20px;
      letter-spacing: -0.5px;
      font-family: "DINOT-Medium", sans-serif;
      color: #fe5906;
    }
  }
}
</style>
