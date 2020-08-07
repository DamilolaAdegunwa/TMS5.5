<template>
  <div class="page">
    <nav-bar title="支付" back="/sale" />

    <div class="title">
      支付金额：
      <span class="amount">￥{{ $route.params.summaryMoney }}</span>
      <br />
      <span style="font-size: 16px;" @click="showOrderDetail"
        >订单号：{{ $route.params.listNo }}</span
      >
      <br />
      <span style="font-size: 16px;" @click="showOrderDetail">订单详情 ></span>
    </div>
    <van-radio-group v-model="payTypeId">
      <van-cell-group>
        <van-cell>请选择支付方式</van-cell>
        <van-cell title="现金支付" clickable @click="payTypeId = '1'">
          <img
            slot="icon"
            src="../../assets/imgs/payments/cash.png"
            width="35"
            height="30"
          />
          <van-radio name="1" />
        </van-cell>
        <van-cell title="微信支付" clickable @click="payTypeId = '8'">
          <img
            slot="icon"
            src="../../assets/imgs/payments/wechat.png"
            width="35"
            height="30"
          />
          <van-radio name="8" />
        </van-cell>
        <!-- <van-cell title="银行卡" clickable @click="payTypeId = '2'">
          <img slot="icon" src="../../assets/imgs/payments/unipay.png" width="35" height="30">
          <van-radio name="2"/>
        </van-cell>
        <van-cell title="储值卡支付" clickable @click="payTypeId = '40000'">
          <img slot="icon" src="../../assets/imgs/payments/card.png" width="35" height="30">
          <van-radio name="40000"/>
        </van-cell>
        <van-cell title="支付宝支付" clickable @click="payTypeId = '9'">
          <img slot="icon" src="../../assets/imgs/payments/alipay.png" width="35" height="30">
          <van-radio name="9"/>
        </van-cell>-->
      </van-cell-group>
    </van-radio-group>

    <van-cell-group>
      <van-button
        type="danger"
        class="checkout-button"
        :loading="loading"
        size="large"
        @click="checkOut"
        >确认</van-button
      >
    </van-cell-group>

    <van-actionsheet v-model="show" title="订单详情">
      <van-cell-group>
        <van-cell>
          <van-row class="van-row">
            <van-col span="12">名称</van-col>
            <van-col class="item-price" span="8">单价</van-col>
            <van-col span="4">人数</van-col>
          </van-row>
        </van-cell>
        <div class="table_detail">
          <van-cell
            v-for="item in orderInfo.details"
            :key="item.ticketTypeName"
          >
            <van-row class="van-row">
              <van-col span="12">{{ item.ticketTypeName }}</van-col>
              <van-col class="item-price" span="8">{{
                `￥${item.reaPrice.toFixed(2)}`
              }}</van-col>
              <van-col span="4">{{ item.totalNum }}</van-col>
            </van-row>
          </van-cell>
        </div>
      </van-cell-group>
    </van-actionsheet>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import { App, Barcode, Printer } from "../../handset-sdk";
import paymentService from "../../services/paymentService.js";
import payTypeService from "../../services/payTypeService.js";
import printerHelper from "../../utils/printerHelper.js";

import orderService from "../../services/orderService.js";
import ticketService from "../../services/ticketService.js";

class PayInput {
  constructor() {
    this.listNo = "";
    this.payTypeId = 0;
    this.authCode = "";
  }
}

export default {
  name: "SaleCheckout",
  components: { NavBar },
  data() {
    return {
      //authCode: "",
      show: false,
      loading: false,
      listNoInput: {},
      payTypeId: 0,
      payTypeName: "",
      orderInfo: {},
      payInput: new PayInput()
    };
  },
  beforeRouteLeave(to, from, next) {
    let orderInput = { listNo: this.$route.params.listNo };
    if (to.path == "/sale" && this.payInput.authCode != "") {
      this.$dialog
        .confirm({
          title: "",
          message: "支付订单已经提交，确定要取消吗？",
          confirmButtonText: "取消",
          cancelButtonText: "确认"
        })
        .then(() => {
          // on confirm
        })
        .catch(() => {
          // on cancel
          orderService
            .cancelOrderAsync(orderInput)
            .then(() => {
              this.$toast.success("订单已取消");
            })
            .catch(response => {
              this.$toast.fail(response.response.data.error.message);
            });
          next();
        });
    } else if (to.path == "/sale") {
      orderService
        .cancelOrderAsync(orderInput)
        .then(() => {})
        .catch(response => {
          this.$toast.fail(response.response.data.error.message);
        });
      next();
    } else {
      next();
    }
  },
  created() {
    this.getOrderInfo();
  },
  watch: {
    payTypeId() {
      this.payInput.payTypeId = Number(this.payTypeId);
      this.payTypeName = payTypeService.getPayTypeName(Number(this.payTypeId));
    }
  },
  methods: {
    gotoCash() {
      paymentService
        .cashPayAsync(this.$route.params.listNo)
        .then(async response => {
          if (response.result.success) {
            this.loading = false;
            this.payInput.authCode = "";
            this.$router.push(
              `/sale/checkoutCash/${this.payTypeName}/${
                this.$route.params.summaryMoney
              }/${this.$route.params.listNo}`
            );
            await this.printReceipt();
          }
        })
        .catch(response => {
          this.loading = false;
          this.payInput.authCode = "";
          this.$toast.fail(response.response.data.error.message);
        });
    },
    gotoMicroPay() {
      if (Barcode) {
        Barcode.read(
          authCode => {
            // 这里传入了取到的条码内容，在这里做处理
            // ...
            App.beep();
            this.microPay(authCode);
          },
          () => {
            // 这里代表扫码不成功，例如超时或手动取消等
            this.loading = false;
            this.payInput.authCode = "";
          }
        );
      } else {
        this.$toast("此功能未实现");
        this.loading = false;
        this.payInput.authCode = "";
      }
    },
    async microPay(authCode) {
      this.payInput.authCode = authCode;
      this.payInput.listNo = this.$route.params.listNo;
      this.payInput.payTypeId = 8;
      let payMoney = 0.0;
      //调用微信支付接口

      paymentService
        .microPayAsync(this.payInput)
        .then(async microPayResponse => {
          if (microPayResponse.result.success) {
            //微信支付已成功
            //设置支付状态:支付成功
            payMoney = this.$route.params.summaryMoney;

            this.$toast.success("支付成功");
            this.loading = false;
            this.payInput.authCode = "";
            this.$router.push(
              `/sale/checkoutReceipt/${this.payInput.payTypeId}/${payMoney}/${
                this.$route.params.listNo
              }`
            );
            await this.printReceipt();
          } else if (microPayResponse.result.isPaying) {
            //微信支付还没有成功，正在支付中，循环查询订单支付结果
            //提示“支付中...”
            this.$toast.loading({
              duration: 0, // 持续展示 toast
              forbidClick: true, // 禁用背景点击
              loadingType: "spinner",
              message: "支付中..."
            });

            //设置timer来控制循环
            let second = 60;
            const timer = setInterval(async () => {
              second -= 3; //3秒一循环
              if (second) {
                //进入循环体
                paymentService
                  .getNetPayOrderAsync(this.$route.params.listNo)
                  .then(async netPayOrderResponse => {
                    if (netPayOrderResponse.result.paySuccess) {
                      //订单支付成功
                      //设置支付状态:支付成功
                      payMoney = netPayOrderResponse.result.payMoney;
                      //清除timer\清除Toast
                      clearInterval(timer);

                      this.$toast.success("支付成功！");
                      this.loading = false;
                      this.payInput.authCode = "";
                      this.$router.push(
                        `/sale/checkoutReceipt/${
                          this.payInput.payTypeId
                        }/${payMoney}/${this.$route.params.listNo}`
                      );
                      await this.printReceipt();
                    } else if (!netPayOrderResponse.result.isPaying) {
                      clearInterval(timer);
                      this.loading = false;
                      this.payInput.authCode = "";
                      this.$toast.success("支付失败！");
                    }
                  })
                  .catch(() => {
                    //清除timer
                    clearInterval(timer);
                    //提示“支付失败”
                    this.loading = false;
                    this.payInput.authCode = "";
                  });
              } else {
                //清除timer
                clearInterval(timer);
                //提示“超时”
                this.loading = false;
                this.payInput.authCode = "";
                this.$toast.fail("超时");
              }
            }, 3000);
          } else {
            this.loading = false;
            this.payInput.authCode = "";
            //提示“支付失败”
            this.$toast.fail("支付失败");
          }
        })
        .catch(microPayResponse => {
          //提示“支付失败”
          this.loading = false;
          this.payInput.authCode = "";
          this.$toast.fail(
            "支付异常:" + microPayResponse.response.data.error.message
          );
        });
    },

    gotoReceipt() {
      if (Printer) {
        Printer.addString("打印示例");
        Printer.addString(String(new Date()));
        Printer.printString();
        Printer.walkPaper(10);
      }
      this.$router.push("/sale/checkoutReceipt/888.88/Test1234567890");
    },
    async printReceipt() {
      let payTypeName = payTypeService.getPayTypeName(
        Number(this.payInput.payTypeId)
      );
      let listNo = this.$route.params.listNo;
      try {
        await printerHelper.printReceiptByListNo(
          "门票销售交易凭证",
          payTypeName,
          listNo
        );

        //更新订单打印次数
        let rePrintInput = {
          listNo: listNo,
          ticketCode: ""
        };
        await ticketService.printAsync(rePrintInput);
      } catch (err) {
        this.$toast.fail(err.message);
      } finally {
        this.loading = false;
      }
    },
    showOrderDetail() {
      this.show = true;
    },
    async getOrderInfo() {
      let listNoInput = { listNo: this.$route.params.listNo };
      let result = await orderService.getOrderFullInfoAsync(listNoInput);
      this.orderInfo.listNo = result.listNo;
      this.orderInfo.totalMoney = result.totalMoney;
      this.orderInfo.details = [];
      result.details.forEach(detail => {
        this.orderInfo.details.push({
          ticketTypeName: detail.ticketTypeName,
          totalNum: detail.totalNum,
          reaPrice: detail.reaPrice
        });
      });
    },

    checkOut() {
      this.loading = true;
      if (printerHelper.checkStatus()) {
        switch (this.payTypeId) {
          case "1":
            this.gotoCash();
            break;
          case "8":
            this.gotoMicroPay();
            break;
          default:
            this.loading = false;
            this.payInput.authCode = "";
            this.$toast("请选择支付方式");
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
  margin-bottom: 12px;
  font-size: 20px;

  .amount {
    font-family: "DINOT-Medium", sans-serif;
    font-size: 24px;
    color: $orange;
  }
}

.van-cell__title {
  display: flex;
  align-items: center;
  padding-left: 12px;
}
.van-button {
  margin: 32px 8px 0;
  width: calc(100% - 16px);
}
.checkout-button {
  background: $orange;
}
.van-actionsheet {
  height: 400px;

  position: fixed;
  .table_detail {
    max-height: 300px;
    overflow-y: auto;
    padding-top: 0px;
    padding-bottom: 24px;
  }
}
</style>
