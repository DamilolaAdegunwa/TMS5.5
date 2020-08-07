<template>
  <div>
    <div class="in-order">
      <div class="in-order-countdown">
        支付剩余时间
        <span>{{ countdownText }}</span>
      </div>
      <div class="in-order-fee">
        <dfn>¥</dfn>
        <span>{{ totalMoney }}</span>
      </div>
      <div class="in-order-title">{{ product }}</div>
    </div>
    <div class="in-pay-method">
      <div class="in-pay-method-pay-type">
        <van-icon name="wechat" />
        <span>微信支付</span>
      </div>
    </div>
    <div class="in-button">
      <van-button type="primary" size="large" :loading="saving" @click="pay">确认支付</van-button>
    </div>
  </div>
</template>

<script>
import dayjs from "dayjs";
import weiXinJsSdkHelper from "./../../utils/weiXinJsSdkHelper.js";
import paymentService from "./../../services/paymentService.js";
import commonService from "./../../services/commonService.js";
import memberService from "@/services/memberService.js";

export default {
  props: {
    listNo: {
      type: String,
    },
  },
  data() {
    return {
      countdownText: "",
      totalMoney: 0,
      timer: -1,
      queryTimer: -1,
      saving: false,
      shouldConfirm: true,
    };
  },
  computed: {
    product() {
      return localStorage.getItem("Pay:Product");
    },
  },
  async created() {
    const order = await paymentService.getNetPayOrderAsync(this.listNo);
    this.totalMoney = order.payMoney;

    if (order.expireSeconds <= 0) {
      this.shouldConfirm = false;
      this.$router.push({ name: "tickettype" });
      return;
    }

    const endTime = dayjs("2018-01-01 00:00:00");
    let countdown = endTime.clone().addSeconds(order.expireSeconds);
    const formatStr = order.expireSeconds > 3600 ? "HH:mm:ss" : "mm:ss";
    this.countdownText = countdown.format(formatStr);
    this.timer = setInterval(() => {
      countdown = countdown.addSeconds(-1);
      this.countdownText = countdown.format(formatStr);
      if (countdown.isSame(endTime)) {
        paymentService.getNetPayOrderAsync(this.listNo).then((result) => {
          this.clear();
          this.shouldConfirm = false;
          if (result.paySuccess) {
            this.$router.replace({
              name: "orderdetail",
              params: { listNo: this.listNo },
            });
          } else {
            this.$dialog
              .alert({
                title: "支付超时订单已取消",
              })
              .then(() => {
                this.$router.push({ name: "tickettype" });
              });
          }
        });
      }
    }, 1000);
  },
  beforeRouteLeave(to, from, next) {
    if (!this.shouldConfirm || to.meta.shouldNotConfirm) {
      this.clear();
      next();
      return;
    }

    this.$dialog
      .confirm({
        title: "你的支付尚未完成，是否取消支付？",
        showCancelButton: true,
        confirmButtonText: "继续支付",
        cancelButtonText: "取消支付",
      })
      .then(() => {
        next(false);
      })
      .catch(() => {
        this.clear();
        this.shouldConfirm = false;
        next({
          name: "orderdetail",
          params: { listNo: this.listNo },
          replace: true,
        });
      });
  },
  methods: {
    async pay() {
      try {
        this.saving = true;
        let payArgs;
        let def = memberService.getDef();
        if (def == "MP") {
          payArgs = await paymentService.miniProgramPayAsync(this.listNo);
          this.miniPay(payArgs);
        } else {
          payArgs = await paymentService.jsApiPayAsync(this.listNo);
          await weiXinJsSdkHelper.jsApiPay(payArgs);
        }

        this.$toast.loading({
          duration: 0,
          message: "查询支付结果...",
          className: "van-toast-big",
        });

        this.queryTimer = setInterval(() => {
          paymentService.getNetPayOrderAsync(this.listNo).then((result) => {
            if (result.paySuccess) {
              this.clear();
              this.shouldConfirm = false;
              this.$router.replace({
                name: "orderdetail",
                params: { listNo: this.listNo },
              });
            }
          });
        }, 3000);
      } catch (err) {
        if (err.err_msg == "get_brand_wcpay_request:cancel") return;

        commonService.logError(err);
      } finally {
        this.saving = false;
      }
    },
    miniPay(payArgs) {
      let resultMap = JSON.parse(payArgs);
      //点击微信支付后，调取统一下单接口生成微信小程序支付需要的支付参数
      var params =
        "?appId=" +
        resultMap.appId +
        "&timeStamp=" +
        resultMap.timeStamp +
        "&nonceStr=" +
        resultMap.nonceStr +
        "&package=" +
        resultMap.package.replace("=", "%3D") +
        "&signType=" +
        resultMap.signType +
        "&paySign=" +
        resultMap.paySign;
      //定义path 与小程序的支付页面的路径相对应
      var path = "/pagesTicket/mp-pay" + params;

      //通过JSSDK的api使小程序跳转到指定的小程序页面
      jWeixin.miniProgram.navigateTo({ url: path });
    },
    clear() {
      clearInterval(this.timer);
      clearInterval(this.queryTimer);
      this.$toast.clear();
    },
  },
};
</script>

<style lang="scss" scoped>
.in-order {
  background-color: #fff;
  text-align: center;
  padding: 15px 15px;

  &-countdown {
    font-size: 13px;
    color: #999;
    line-height: 1;
  }

  &-fee {
    margin-top: 10px;
    line-height: 1;
    font-size: 30px;
    letter-spacing: -0.44px;

    dfn {
      font-size: 20px;
      margin-right: 8px;
    }

    span {
      font-weight: 700;
    }
  }

  &-title {
    margin-top: 3px;
    font-size: 11px;
    color: #999;
  }
}

.in-pay-method {
  margin: 15px 0;
  background-color: #fff;

  &-pay-type {
    padding: 0 20px;
    height: 70px;
    line-height: 70px;

    i {
      vertical-align: middle;
      font-size: 25px;
      margin-right: 20px;
      color: rgb(9, 187, 7);
    }

    span {
      display: inline-block;
      vertical-align: middle;
      font-size: 18px;
      font-weight: 600;
    }
  }
}

.in-button {
  padding: 0 15px;

  button {
    border-radius: 5px;
  }
}
</style>
