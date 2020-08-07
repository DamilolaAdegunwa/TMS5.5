<template>
  <div class="page">
    <nav-bar title="检票管理" back="/home" />
    <check-tabs :active="0" />
    <div class="groundId">
      <span>当前项目:{{ groundIdStr }}</span>
    </div>
    <div class="content">
      <img src="../../assets/imgs/check.png" />
      <van-button size="large" @click="readBarcode">
        <font-awesome-icon icon="qrcode" color="#009DDC" fixed-width />扫码
      </van-button>
      <van-button size="large" @click="readCard">
        <font-awesome-icon
          icon="credit-card"
          color="rgb(225, 148, 37)"
          fixed-width
        />刷卡
      </van-button>
      <van-checkbox v-model="withFlash" class="content-flash"
        >手电筒</van-checkbox
      >
    </div>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import { App } from "../../handset-sdk.js";
import appSdk from "./../../appSdk.js";
import CheckTabs from "./CheckTabs";
import ticketService from "../../services/ticketService.js";
import { isNullOrUndefined } from "util";
import printerHelper from "../../utils/printerHelper.js";

class Input {
  constructor() {
    this.ticketCode = "";
    this.certNo = "";
    this.groundId = 0;
    this.gateGroupId = 0;
    this.gateId = 0;
  }
}

class CheckInInfo {
  constructor() {
    this.title = "";
    this.ticketCode = "";
    this.ticketTypeName = "";
    this.checkinNum = 0;
    this.ticketCollector = "";
    this.checkingTime = "";
  }
}

export default {
  name: "Check",
  components: { CheckTabs, NavBar },
  data() {
    return {
      groundIdStr: "",
      input: new Input(),
      checkInInfo: new CheckInInfo(),
      withFlash: false
    };
  },
  created() {
    this.loadData();
  },
  beforeDestroy() {
    this.$dialog.close();
  },
  methods: {
    loadData() {
      this.groundIdStr = this.getGroundIdStr();
      this.input.groundId = this.getGroundId();
      this.input.gateGroupId = this.getGateGroupId();
      this.input.gateId = this.getGateId();

      this.checkInInfo.ticketCollector = sessionStorage.getItem("employeeName");
    },
    getGroundId() {
      if (App) {
        let result = App.getValue("groundId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else if (window.localStorage) {
        let result = localStorage.getItem("groundId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getGroundIdStr() {
      if (App) {
        let result = App.getValue("groundIdStr");
        return result;
      } else if (window.localStorage) {
        let result = localStorage.getItem("groundIdStr");
        return result;
      } else {
        return "";
      }
    },
    getGateGroupId() {
      if (App) {
        let result = App.getValue("gateGroupId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else if (window.localStorage) {
        let result = localStorage.getItem("gateGroupId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getSalePointId() {
      if (App) {
        let result = App.getValue("salePointId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
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
    getGateId() {
      if (App) {
        let result = App.getValue("gateId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
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
    },
    async readBarcode() {
      const ticketCode = await appSdk.scanBarcode(this.withFlash);
      this.checkTicket(ticketCode, "");
    },
    async readCard() {
      try {
        this.$dialog.alert({ message: "请刷卡...", showConfirmButton: false });
        const result = await appSdk.readIdCard();
        this.checkTicket("", result.card.no);
      } finally {
        this.$dialog.close();
      }
    },
    checkTicket(ticketCode, certNo) {
      this.input.ticketCode = ticketCode;
      this.input.certNo = certNo;

      let checkNo = ticketCode || certNo;

      ticketService
        .checkTicketAsync(this.input)
        .then(result => {
          let ticket = result;

          if (ticket.success) {
            this.$router.push(
              `/check/success/${ticket.result.cardNo}/${
                ticket.result.ticketTypeName
              }/${ticket.result.checkNum}`
            );
            if (ticket.result.shouldPrintAfterCheck) {
              this.checkInInfo.title = "检票凭证";
              this.checkInInfo.ticketCode = ticket.result.cardNo;
              this.checkInInfo.ticketTypeName = ticket.result.ticketTypeName;
              // this.checkInInfo.checkinNum = ticket.result.totalNum;
              this.checkInInfo.checkinNum = ticket.result.checkNum;
              this.checkInInfo.checkinPoint = this.groundIdStr; //
              this.checkInInfo.checkingTime = ticket.result.checkTime;

              printerHelper.printCheckIn(this.checkInInfo);
            }
          } else {
            this.$router.push(
              `/check/failed/${checkNo}/${ticket.error.message}`
            );
          }
        })
        .catch(result => {
          let ticket = result.response.data;
          this.$router.push(`/check/failed/${checkNo}/${ticket.error.message}`);
        });
    }
  }
};
</script>

<style lang="scss" scoped>
.content {
  margin: 12px;
  text-align: center;

  img {
    max-width: 65%;
    margin-top: 24px;
    margin-bottom: 36px;
  }

  .field-label {
    font-weight: normal;
    height: 48px;
    line-height: 48px;
    margin-bottom: 12px;
  }

  .van-button {
    margin-bottom: 12px;
    height: 56px;
  }

  &-flash {
    text-align: right;
  }
}
.groundId {
  text-align: center;
  font-size: 20px;
}
</style>
