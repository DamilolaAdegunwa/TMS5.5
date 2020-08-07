<template>
  <div class="page">
    <nav-bar title="检票凭证重印" back="/account" />

    <van-cell-group>
      <van-cell title="票类：" :value="checkInInfo.ticketTypeName" />
      <van-cell title="票号：" :value="checkInInfo.ticketCode" />
      <van-cell title="检票人数：" :value="checkInInfo.checkinNum" />
      <van-cell>
        <van-row>
          <van-col span="12">检&nbsp;&nbsp;票&nbsp;&nbsp;员：</van-col>
          <van-col span="12" style="text-align: right; color:gray;">{{
            checkInInfo.ticketCollector
          }}</van-col>
        </van-row>
      </van-cell>
      <van-cell title="检票时间：" :value="checkInInfo.checkingTime" />
      <van-cell title="检票项目：" :value="checkInInfo.checkinPoint" />
    </van-cell-group>

    <van-button
      type="primary"
      size="large"
      :loading="loading"
      @click="printCheckIn"
      >重印凭证</van-button
    >
  </div>
</template>

<script>
import { Toast } from "vant";
import NavBar from "../../components/NavBar";
import { App, Printer } from "../../handset-sdk";
import printerHelper from "../../utils/printerHelper.js";
import ticketService from "../../services/ticketService.js";
import { isNullOrUndefined } from "util";

class CheckInInfo {
  constructor() {
    this.ticketCode = "";
    this.ticketTypeName = "";
    this.checkinNum = 0;
    this.ticketCollector = "";
    this.checkingTime = "";
  }
}
export default {
  name: "AccountReprintCheckIn",
  components: { NavBar },
  data() {
    return {
      loading: false,
      checkInInfo: new CheckInInfo()
    };
  },
  created() {
    this.checkInInfo.title = "检票凭证(重印)";
    let input = {
      staffId: this.getStaffId(),
      gateId: this.getGateId()
    };

    ticketService
      .getLastCheckTicketInfoAsync(input)
      .then(result => {
        this.checkInInfo.ticketCode = result.cardNo;
        this.checkInInfo.ticketTypeName = result.ticketTypeName;
        this.checkInInfo.ticketCollector = result.checkerName;
        this.checkInInfo.checkinPoint = this.getGroundIdStr();
        //this.checkInInfo.checkinNum = result.totalNum;
        this.checkInInfo.checkinNum = result.checkNum;
        this.checkInInfo.checkingTime = result.checkTime;
        this.checkInInfo.title = "检票凭证(重印)";
      })
      .catch(result => {
        Toast.fail(result.response.data.error.message);
      });
  },
  methods: {
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
    },
    print() {
      if (Printer) {
        // 调用SDK功能例子
        Printer.addString("打印测试");

        // 注意此处图片是使用URL指定的(建议使用绝对路径)
        // webpack打包的文件是无法找到的
        // 来自服务端使用路径指定的图片应该可以
        Printer.printImage("/logo.png");
        //Printer.printImage("../../public/logo.png");
        Printer.printString();
        Printer.walkPaper(10);

        // 安卓原生风格Toast, 或者用Vant的Toast, 都可以
        App.toast("打印成功");
      } else {
        Toast.fail("没有打印机");
      }
    },
    printCheckIn() {
      this.loading = true;
      try {
        if (this.checkInInfo.ticketCode == "") {
          this.$toast.fail("暂无数据");
        } else {
          printerHelper.printCheckIn(this.checkInInfo);
        }
      } catch (err) {
        this.$toast.fail(err.message);
      } finally {
        this.loading = false;
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
</style>
