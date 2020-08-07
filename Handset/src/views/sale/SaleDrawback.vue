<template>
  <div class="page">
    <nav-bar title="售票管理" back="/home" />
    <sale-tabs :active="1" />

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
        <van-cell>
          <van-row>
            <van-col span="12">{{ ticketInfo.ticketTypeName }}</van-col>
            <van-col span="12" style="text-align: right;">{{
              Number(ticketInfo.surplusQuantity) == 0
                ? "已用完"
                : Number(ticketInfo.quantity) ==
                  Number(ticketInfo.surplusQuantity)
                ? "未使用"
                : "部分使用"
            }}</van-col>
          </van-row>
        </van-cell>

        <van-cell>
          <van-row>
            <van-col span="12">票 号：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.ticketCode
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
            <van-col span="12">人 数：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.quantity
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">可退人数：</van-col>
            <van-col span="12" style="text-align: right;">{{
              ticketInfo.surplusQuantity
            }}</van-col>
          </van-row>
        </van-cell>
        <van-cell>
          <van-row>
            <van-col span="12">单 价：</van-col>
            <van-col span="12" style="text-align: right;">
              <span class="number">¥ {{ ticketInfo.reaPrice }}</span>
            </van-col>
          </van-row>
        </van-cell>

        <van-cell>
          <van-row>
            <van-col span="12">可退金额：</van-col>
            <van-col span="12" style="text-align: right;">
              <span class="number"
                >¥ {{ ticketInfo.reaPrice * ticketInfo.surplusQuantity }}</span
              >
            </van-col>
          </van-row>
        </van-cell>
      </van-cell-group>
    </div>

    <div class="bottom-bar">
      <span class="title">退款总额</span>
      <span class="summary-money">¥ {{ totalMoney }}</span>
      <button
        :disabled="disabled"
        class="checkout-button"
        @click="gotoSaleDrawback"
      >
        退款
      </button>
    </div>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import SaleTabs from "./SaleTabs";
import { App } from "../../handset-sdk";
import math from "mathjs";
import ticketService from "../../services/ticketService.js";
import { isNullOrUndefined } from "util";
import appSdk from "./../../appSdk.js";

export default {
  name: "SaleDrawback",
  components: { SaleTabs, NavBar },
  data() {
    return {
      disabled: false,
      totalMoney: "0.00",
      showInfo: false,
      ticketInfo: {},
      code: "",
      queryInput: {
        ticketCode: "",
        certNo: ""
      },
      refundInput: {
        ticketCode: "",
        cashierId: this.getStaffId(),
        cashPcid: this.getPcId(),
        salePointId: this.getSalePointId(),
        parkId: this.getParkId()
      }
    };
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
    getParkId() {
      let parkId = sessionStorage.getItem("parkId");
      return Number(parkId);
    },
    getStaffId() {
      let staffId = sessionStorage.getItem("staffId");
      return Number(staffId);
    },

    getPcId() {
      if (App) {
        let result = App.getValue("pcId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("pcId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    async readBarcode() {
      this.code = await appSdk.scanBarcode();
      this.searchTicket();
    },
    async readCard() {
      try {
        this.$dialog.alert({ message: "请刷卡...", showConfirmButton: false });
        const result = await appSdk.readIdCard();
        this.code = result.card.no;
        this.searchTicket();
      } finally {
        this.$dialog.close();
      }
    },
    searchTicket() {
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

      ticketService
        .GetTicketFullInfoAsync(this.queryInput)
        .then(response => {
          this.ticketInfo = response.result;
          this.totalMoney = math
            .multiply(this.ticketInfo.surplusQuantity, this.ticketInfo.reaPrice)
            .toFixed(2);
          this.showInfo = true;
        })
        .catch(response => {
          this.$toast.fail(response.response.data.error.message);
        });
    },
    async gotoSaleDrawback() {
      if (!this.code) {
        this.$toast.fail("请输入票号");
        return;
      }

      if (this.totalMoney <= 0) {
        this.$toast.fail("退款总额为零");
        return;
      }

      this.refundInput.ticketCode = this.ticketInfo.ticketCode;
      ticketService
        .RefundFromHandsetAsync(this.refundInput)
        .then(() => {
          this.$router.push(`/drawback/success/${this.totalMoney}`);
        })
        .catch(result => {
          this.$toast.fail(result.response.data.error.message);
        });
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../variables";

// 底栏高度
$bottom-bar-height: 64px;

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

// 底栏
.bottom-bar {
  background: #333333;
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: $bottom-bar-height;
  line-height: $bottom-bar-height;
  color: #ffffff;
  z-index: 500;

  // 退款总额标题
  .title {
    display: inline-block;
    margin: 0 24px;
  }

  // 金额数字
  .summary-money {
    font-family: "DINOT-Medium", sans-serif;
    font-size: 28px;
    vertical-align: sub;
  }

  // 去结算按钮
  .checkout-button {
    background: $orange;
    font-size: 20px;
    position: absolute;
    right: 0;
    top: 0;
    bottom: 0;
    width: 32%;
    border: none;
  }

  // 无商品状态
  &.inactive {
    .cart-icon {
      background: #474747;
    }

    .cart-icon-badge {
      display: none;
    }

    .checkout-button {
      background: #606060;
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
.number {
  font-size: 20px;
  letter-spacing: -0.5px;
  font-family: "DINOT-Medium", sans-serif;
  color: #fe5906;
}
</style>
