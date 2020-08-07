<template>
  <div class="page">
    <nav-bar title="支付成功" back="/sale" />

    <div class="title">支付成功</div>
    <van-cell-group>
      <van-cell title="订单号" :value="$route.params.listNo" />
    </van-cell-group>
    <van-cell-group>
      <van-button
        type="primary"
        :loading="loading"
        :disabled="disabled"
        size="large"
        @click="printReceipt"
        >打印</van-button
      >
      <van-button type="default" size="large" @click="gotoSale"
        >返回</van-button
      >
    </van-cell-group>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import ticketService from "../../services/ticketService.js";
import printerHelper from "../../utils/printerHelper.js";
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
        ticketCode: "",
        ticketCodeDataUrl: ""
      }
    ];
  }
}
export default {
  name: "SaleCheckoutCash",
  components: { NavBar },
  data() {
    return {
      loading: false,
      disabled: false,
      payTypeName: payTypeService.getPayTypeName(1),
      listNoInput: new ListNoInput(),
      orderInfo: new OrderInfo()
    };
  },
  methods: {
    gotoSale() {
      this.$router.push("/sale");
    },
    async printReceipt() {
      this.loading = true;
      if (printerHelper.checkStatus()) {
        try {
          let listNo = this.$route.params.listNo;
          await printerHelper.printReceiptByListNo(
            "门票销售交易凭证",
            this.payTypeName,
            listNo
          );
          //更新订单打印次数
          let rePrintInput = {
            listNo: listNo,
            ticketCode: ""
          };
          await ticketService.printAsync(rePrintInput);
          this.loading = false;
          this.disabled = true;
        } catch (err) {
          this.$toast.fail(err.message);
        } finally {
          this.loading = false;
        }
      } else {
        this.loading = false;
      }
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
