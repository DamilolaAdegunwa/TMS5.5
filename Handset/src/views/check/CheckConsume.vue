<template>
  <div class="page">
    <nav-bar title="检票管理" back="/home" />
    <check-tabs :active="1" />

    <div class="content">
      <img src="../../assets/imgs/check.png" />
      <van-row>
        <van-col span="8">
          <h3 class="field-label">扣费金额</h3>
        </van-col>
        <van-col span="16">
          <van-field placeholder="请输入扣费金额" />
        </van-col>
      </van-row>
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
    </div>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import CheckTabs from "./CheckTabs";

export default {
  name: "CheckFreeConsume",
  components: { CheckTabs, NavBar },
  methods: {
    readBarcode() {
      this.$toast("此功能未开放");
    },
    readCard() {
      this.$toast("此功能未开放");
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
}
</style>
