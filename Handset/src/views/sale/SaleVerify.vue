<template>
  <div class="page">
    <nav-bar title="售票管理" back="/home" />
    <sale-tabs :active="2" />

    <div class="ticket-id-inputs">
      <van-field placeholder="请输入票号" v-model="code" />
      <van-button @click="searchTicket">
        <font-awesome-icon icon="search" color="#ff3a3c" />
        <span>搜索</span>
      </van-button>
      <van-button @click="readBarcode">
        <font-awesome-icon icon="qrcode" color="#009DDC" />
        <span>扫码</span>
      </van-button>
      <van-button @click="readCard">
        <font-awesome-icon icon="credit-card" color="#FE5906" />
        <span>刷卡</span>
      </van-button>
    </div>

    <div v-show="showInfo">
      <van-cell-group class="ticket-info">
        <van-cell title="门票信息" />
        <van-cell>
          <van-row>
            <van-col span="12">销售时间：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.ctime
            }}</van-col>
          </van-row>
        </van-cell>

        <van-cell>
          <van-row>
            <van-col span="12">销售点：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.salePointName
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">付款方式：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.payTypeName
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">人数：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.quantity
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">单价：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.reaPrice
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">实售金额：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.reaPrice * ticketInfo.quantity
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">票号：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.ticketCode
            }}</van-col>
          </van-row>
        </van-cell>
      </van-cell-group>
      <van-cell-group class="ticket-info">
        <van-cell title="使用情况" />
        <van-cell>
          <van-row>
            <van-col span="12">区域</van-col>
            <van-col span="6">剩余次数</van-col>
            <van-col span="6">场次</van-col>
          </van-row>
        </van-cell>
      </van-cell-group>
      <van-cell-group class="ticket-info2">
        <van-cell v-for="item in ticketInfo.grounds" :key="item.groundName">
          <van-row>
            <van-col span="12">{{ item.groundName }}</van-col>
            <van-col span="6">{{ item.surplusNum }}</van-col>
            <van-col span="6">{{ item.changCiName }}</van-col>
          </van-row>
        </van-cell>
      </van-cell-group>
      <van-cell-group class="ticket-info" v-if="!(seats == '')">
        <van-cell title="座位" :value="seats" />
      </van-cell-group>
    </div>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import SaleTabs from "./SaleTabs";
import appSdk from "./../../appSdk.js";
import ticketService from "../../services/ticketService.js";

export default {
  name: "SaleVerify",
  components: { SaleTabs, NavBar },
  data() {
    return {
      showInfo: false,
      seats: "",
      code: "",
      queryInput: {
        ticketCode: "",
        certNo: ""
      },
      ticketInfo: {}
    };
  },
  methods: {
    async readBarcode() {
      this.code = await appSdk.scanBarcode();
      await this.searchTicket();
    },
    async readCard() {
      try {
        this.$dialog.alert({ message: "请刷卡...", showConfirmButton: false });
        const result = await appSdk.readIdCard();
        this.code = result.card.no;
        await this.searchTicket();
      } finally {
        this.$dialog.close();
      }
    },
    async searchTicket() {
      this.showInfo = false;

      if (!this.code) {
        this.$toast.fail("请输入票号");
        return;
      }

      if (this.code.length == 18) {
        this.queryInput.ticketCode = "";
        this.queryInput.certNo = this.code;
      } else {
        this.queryInput.ticketCode = this.code;
        this.queryInput.certNo = "";
      }

      let response = await ticketService.GetTicketFullInfoAsync(
        this.queryInput
      );
      this.ticketInfo = response.result;

      this.ticketInfo.seats.forEach(seat => {
        this.seats += seat.seatName + ",";
      });
      this.seats = this.seats.trimRight(",");

      this.showInfo = true;
    }
  }
};
</script>

<style lang="scss" scoped>
.ticket-id-inputs {
  margin: 0 12px;
  display: flex;

  .van-field {
    flex-grow: 1;
    border: 1px solid #ebedf0;
    height: 48px;
    margin-right: 2px;
  }

  .van-button {
    display: block;
    min-width: 48px;
    height: 48px;
    padding: 0;
    margin-right: 2px;

    &:last-of-type {
      margin-right: 0;
    }

    span {
      display: block;
      font-size: 14px;
      line-height: 18px;
    }
  }
}

.ticket-info {
  margin: 0 14px;
  margin-top: 5px;
}
.ticket-info2 {
  margin: 0 14px;
}
</style>
