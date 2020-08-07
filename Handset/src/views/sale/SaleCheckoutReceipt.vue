<template>
  <div class="page">
    <nav-bar title="支付成功" back="/sale" />

    <div class="content">
      <img src="../../assets/imgs/receipt.png" width="127" height="112" />
      <p class="title">订单支付成功</p>
      <p class="amount">￥{{ $route.params.payMoney }}</p>
    </div>
    <van-button type="primary" size="large" @click="gotoSale">返回</van-button>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import payTypeService from "../../services/payTypeService.js";

class ListNoInput {
  constructor() {
    this.listNo = "";
  }
}
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
        ticketCode: ""
      }
    ];
  }
}
export default {
  name: "SaleCheckoutReceipt",
  components: { NavBar },

  data() {
    return {
      payTypeName: payTypeService.getPayTypeName(8),
      listNoInput: new ListNoInput(),
      orderInfo: new OrderInfo()
    };
  },
  methods: {
    gotoSale() {
      this.$router.push("/sale");
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
