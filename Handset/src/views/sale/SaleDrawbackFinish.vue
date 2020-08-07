<template>
  <div class="page">
    <nav-bar title="退票成功" back="/sale/drawback" />

    <div class="content">
      <img src="../../assets/imgs/drawback.png" width="127" height="112" />
      <p class="title">退票成功</p>
      <p class="amount">￥{{ $route.params.drawbackMoney }}</p>
      <!-- <p class="message">正在打印交易小票...</p> -->
    </div>

    <van-button type="primary" size="large" @click="gotoSaleDrawback"
      >返回</van-button
    >
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";

export default {
  name: "SaleDrawbackFinish",
  components: { NavBar },
  methods: {
    gotoSaleDrawback() {
      this.$router.push("/sale/drawback");
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../variables";

.content {
  margin: 8px;
  padding: 32px 0;
  text-align: center;
  border-radius: 4px;
  background: white;

  img {
    margin-bottom: 24px;
  }

  .title {
    font-size: 20px;
    margin-bottom: 0;
  }

  .amount {
    font-family: "DINOT-Medium", sans-serif;
    color: $orange;
    font-size: 28px;
    margin-bottom: 32px;
  }

  .message {
    font-size: 20px;
    color: #999999;
  }
}

.van-button {
  margin: 32px 8px 0;
  width: calc(100% - 16px);
}
</style>
