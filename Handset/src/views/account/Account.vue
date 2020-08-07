<template>
  <div class="page">
    <div class="header">
      <h1>我的</h1>
      <van-row>
        <van-col class="avatar-col" span="8">
          <img class="avatar" src="../../assets/imgs/avatar1.png" />
        </van-col>

        <van-col class="info-col" span="16">
          <h5>{{ employeeName }}</h5>
          <p>登录账号：{{ userName }}</p>
        </van-col>

        <a class="logout-button" @click="gotoLogin">
          退出
          <font-awesome-icon icon="sign-out-alt" />
        </a>
      </van-row>
    </div>
    <van-cell-group>
      <van-cell is-link @click="gotoAccountChangePassword">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="sign-out-alt"
            color="rgb(225, 148, 37)"
            fixed-width
          />修改密码
        </span>
      </van-cell>
      <van-cell is-link @click="gotoAccountReprintReceipt">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="receipt"
            color="rgb(243, 177, 10)"
            fixed-width
          />交易凭证重印
        </span>
      </van-cell>
      <van-cell is-link @click="gotoAccountReprintCheckIn">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="receipt"
            color="rgb(243, 177, 10)"
            fixed-width
          />检票凭证重印
        </span>
      </van-cell>
      <van-cell is-link @click="gotoAccountCheckStat">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="ticket-alt"
            color="rgb(0, 206, 162)"
            fixed-width
          />检票统计
        </span>
      </van-cell>
      <van-cell is-link @click="gotoAccountSaleStat">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="chart-line"
            color="#009DDC"
            fixed-width
          />售票统计
        </span>
      </van-cell>
      <van-cell is-link @click="gotoSetup">
        <span slot="title">
          <font-awesome-icon
            slot="icon"
            icon="chart-line"
            color="#009DDC"
            fixed-width
          />项目设置
        </span>
      </van-cell>
    </van-cell-group>

    <tab-bar :active="1" />
  </div>
</template>

<script>
import TabBar from "../../components/TabBar";
import staffService from "../../services/staffService.js";
import { App } from "../../handset-sdk.js";

export default {
  name: "Account",
  components: { TabBar },
  data() {
    return {
      userName: "",
      employeeName: ""
    };
  },
  created() {
    this.loadData();
  },
  methods: {
    loadData() {
      this.employeeName = sessionStorage.getItem("employeeName");
      this.userName = sessionStorage.getItem("userName");
    },
    gotoLogin() {
      staffService.logout();

      this.$router.push("/login");
    },
    gotoAccountChangePassword() {
      this.$router.push("/account/change-password");
    },
    gotoAccountReprintReceipt() {
      this.$router.push("/account/reprint-receipt");
    },
    gotoAccountReprintCheckIn() {
      this.$router.push("/account/reprint-checkIn");
    },
    gotoAccountCheckStat() {
      this.$router.push("/account/check-stat");
    },
    gotoAccountSaleStat() {
      this.$router.push("/account/sale-stat");
    },
    gotoSetup() {
      this.$router.push("/Setup/reset");
    },
    gotoSettingLoginUrl() {
      if (App) {
        App.openSettingPage();
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.header {
  background: url("../../assets/imgs/account-bg.png") center no-repeat;
  color: #ffffff;

  h1 {
    font-weight: normal;
    font-size: 16px;
    text-align: center;
    height: 46px;
    line-height: 46px;
    margin-bottom: 12px;
  }

  .van-row {
    padding-bottom: 24px;
    position: relative;

    .avatar-col {
      text-align: center;

      .avatar {
        width: 90px;
        height: 90px;
      }
    }

    .info-col {
      padding-top: 16px;

      h5 {
        font-size: 18px;
        margin-bottom: 0;
        font-weight: normal;
      }

      p {
        font-size: 15px;
        margin-bottom: 0;
      }
    }

    .logout-button {
      text-align: center;
      position: absolute;
      right: 0;
      top: 16px;
      border: white solid 1px;
      height: 36px;
      width: 82px;
      padding-left: 9px;
      line-height: 36px;
      border-right: none;
      border-bottom-left-radius: 18px;
      border-top-left-radius: 18px;
      background: rgba(0, 0, 0, 0.1);
    }
  }
}
</style>
