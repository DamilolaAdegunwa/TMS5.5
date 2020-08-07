<template>
  <div class="page">
    <nav-bar title="交易凭证重印" back="/account" />
    <div class="table-body">
      <van-cell-group>
        <van-cell title="交易单号：" :value="receiptInfo.orderCode" />
        <van-cell title="交易时间：" :value="receiptInfo.time" />
        <van-cell title="票数：" :value="receiptInfo.ticketsNum" />
        <van-cell title="交易金额：" :value="'￥' + receiptInfo.money" />
        <van-cell title="付款方式：" :value="receiptInfo.payType" />
        <van-cell>
          <van-row>
            <van-col span="12">收银员：</van-col>
            <van-col span="12" style="text-align: right; color:gray;">{{
              receiptInfo.Cashier
            }}</van-col>
          </van-row>
        </van-cell>
      </van-cell-group>
      <van-cell-group
        v-for="ticket in receiptInfo.tickets"
        :key="ticket.ticketCode"
      >
        <van-cell title="票类名称：" :value="ticket.ticketTypeName" />
        <van-cell title="人次：" :value="ticket.num" />
        <van-cell title="有效期至：" :value="ticket.etime" />
        <van-cell>
          <div style="text-align:center">
            <img :src="ticket.ticketCodeDataUrl" />
          </div>
        </van-cell>
        <van-cell>
          <div style="text-align:center">
            <span>{{ ticket.ticketCode }}</span>
          </div>
        </van-cell>
      </van-cell-group>
    </div>
    <van-button
      type="primary"
      size="large"
      :loading="loading"
      @click="printReceipt"
      >重印凭证</van-button
    >
  </div>
</template>

<script>
import { Toast } from "vant";
import NavBar from "../../components/NavBar";
import { App, Printer } from "../../handset-sdk";
import printerHelper from "../../utils/printerHelper.js";
import qrcodeHelper from "../../utils/qrcodeHelper.js";
import orderService from "../../services/orderService.js";
import ticketService from "../../services/ticketService.js";
import { isNullOrUndefined } from "util";

class OrderInfo {
  constructor() {
    this.orderCode = "";
    this.time = "";
    this.ticketsNum = "";
    this.money = 0.0;
    this.payType = "";
    this.tickets = [
      {
        ticketTypeName: "",
        num: 0,
        etime: "",
        ticketCode: "",
        ticketCodeDataUrl: ""
      }
    ];
  }
}
export default {
  name: "AccountReprintReceipt",
  components: { NavBar },
  data() {
    return {
      loading: false,
      receiptInfo: new OrderInfo()
    };
  },
  async created() {
    let input = {
      cashierId: this.getStaffId(),
      cashPcid: this.getPcId(),
      salePointId: this.getSalePointId(),
      parkId: this.getParkId()
    };
    orderService
      .getLastOrderFullInfoAsync(input)
      .then(async result => {
        let receiptInfo = {};
        receiptInfo.orderCode = result.listNo;
        receiptInfo.time = result.cTime;
        receiptInfo.money = result.totalMoney;
        receiptInfo.payType = result.payTypeName;
        receiptInfo.Cashier = sessionStorage.getItem("employeeName");
        receiptInfo.tickets = [];
        let ticketsNum = 0;
        for (let i = 0; i < result.details.length; i++) {
          let detail = result.details[i];
          ticketsNum += detail.tickets.length;
          for (let j = 0; j < detail.tickets.length; j++) {
            let ticket = detail.tickets[j];
            receiptInfo.tickets.push({
              ticketTypeName: ticket.ticketTypeName,
              num: ticket.totalNum,
              etime: ticket.etime.split(" ")[0], //有效期至只要显示年月日就ok了，时分秒不用显示。
              ticketCode: ticket.ticketCode
            });
          }
        }
        receiptInfo.ticketsNum = ticketsNum;

        for (let k = 0; k < receiptInfo.tickets.length; k++) {
          let dataUrl = await qrcodeHelper.createQRCodeAsync(
            receiptInfo.tickets[k].ticketCode
          );
          receiptInfo.tickets[k].ticketCodeDataUrl = dataUrl;
        }
        receiptInfo.title = "门票销售交易凭证(重印)";
        this.receiptInfo = receiptInfo;
      })
      .catch(result => {
        Toast.fail(result.response.data.error.message);
      });
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
    print() {
      if (Printer) {
        // 调用SDK功能例子
        Printer.addString("打印测试");

        // 注意此处图片是使用URL指定的(建议使用绝对路径)
        // webpack打包的文件是无法找到的
        // 来自服务端使用路径指定的图片应该可以
        Printer.printImage("/logo.png");
        Printer.printString();
        Printer.walkPaper(10);

        // 安卓原生风格Toast, 或者用Vant的Toast, 都可以
        App.toast("打印成功");
      } else {
        Toast.fail("没有打印机");
      }
    },
    async printReceipt() {
      this.loading = true;
      if (this.receiptInfo.orderCode == "") {
        this.$toast.fail("暂无数据");
        this.loading = false;
      } else if (this.receiptInfo.payType == "") {
        this.$toast.fail("订单未支付");
        this.loading = false;
      } else {
        try {
          let rePrintInput = {
            listNo: this.receiptInfo.orderCode,
            ticketCode: ""
          };
          //更新订单打印次数
          await ticketService.rePrintAsync(rePrintInput);

          printerHelper.printReceipt(this.receiptInfo);
        } catch (err) {
          this.$toast.fail(err.message);
        } finally {
          this.loading = false;
        }
      }
    },
    gotoAccount() {
      this.$router.push("/account");
    }
  }
};
</script>

<style lang="scss" scoped>
.van-cell-group {
  margin-top: 12px;
}

.van-button {
  margin: 12px 8px 0;
  width: calc(100% - 16px);
}

.button {
  margin: 12px 8px 0;
  margin-bottom: 12px;
  width: calc(100% - 16px);
}

.table-body {
  // 总高度减去其它部分
  max-height: calc(100vh - 120px);
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
}
</style>
