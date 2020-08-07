<template>
  <div class="page">
    <nav-bar title="现金支付" back="/sale" />

    <div class="title">
      收款金额：
      <span class="amount">￥{{ $route.params.payMoney }}</span>
    </div>

    <van-cell-group>
      <van-cell title="订单号" :value="$route.params.listNo" />
      <van-cell title="付款方式" :value="$route.params.payTypeName" />
    </van-cell-group>
    <van-cell-group>
      <van-button type="primary" size="large" @click="gotoSale"
        >返回</van-button
      >
    </van-cell-group>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";

export default {
  name: "SaleCheckoutCash",
  components: { NavBar },
  methods: {
    gotoSale() {
      this.$router.push("/sale");
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../variables";

.title {
  padding: 24px 0;
  text-align: center;
  background: white;
  font-size: 20px;

  .amount {
    font-family: "DINOT-Medium", sans-serif;
    font-size: 24px;
    color: $orange;
  }
}

.van-button {
  margin: 32px 8px 0;
  width: calc(100% - 16px);
}
</style>
