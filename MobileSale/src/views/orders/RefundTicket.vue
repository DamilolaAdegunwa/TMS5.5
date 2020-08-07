<template>
  <div class="refund">
    <div class="refund-tip margin-bottom-10">
      <p>
        经核实如未使用且符合退改规则，退款将于2-15个工作日退至您的支付账户。
      </p>
    </div>
    <div class="refund-list margin-bottom-10">
      <h2 class="refund-money">
        <span class="refund-money-pay">
          订单实付：
          <span class="refund-money-pay-fee">¥{{ order.totalMoney }}</span>
        </span>
        <span class="refund-money-amount">
          退款金额：
          <span class="refund-money-amount-fee">¥{{ refundMoney }}</span>
        </span>
      </h2>
    </div>
    <div class="refund-list margin-bottom-10">
      <h3 class="refund-list-title">退款明细</h3>
      <ul>
        <li v-for="(detail, index) in order.details" :key="index">
          <div class="refund-list-item margin-bottom-10">
            <span class="refund-ticket-name">{{ detail.ticketTypeName }}</span>
            <div class="refund-copies">
              <van-stepper
                v-model="detail.refundQuantity"
                :min="detail.minQuantity"
                :max="detail.refundableQuantity"
                integer
                @change="onQuantityChange(detail)"
              />
            </div>
          </div>
          <div class="refund-list-item">
            <div @click="showRefundDescription(detail.ticketTypeId)" class="refund-rules">
              <span>退改规则</span>
              <van-icon name="arrow" />
            </div>
            <!-- <span class="refund-price">退 ¥{{detail.reaPrice}}</span> -->
          </div>
        </li>
      </ul>
    </div>
    <div class="refund-list margin-bottom-10">
      <h3 class="refund-list-title">
        取消原因
        <span class="refund-list-title-tips">(必选)</span>
      </h3>
      <ul>
        <li style="padding:0;">
          <van-radio-group v-model="reasonIndex">
            <div
              v-for="(reason, index) in reasons"
              :key="index"
              class="refund-list-item refund-reason"
              @click="reasonIndex = index"
            >
              <van-radio :name="index">{{ reason }}</van-radio>
            </div>
          </van-radio-group>
        </li>
      </ul>
    </div>
    <bottom-button text="申请取消" type="price" :loading="loading" @click="refund"></bottom-button>
    <refund-description v-model="showDescription" :ticket-type-id="currentTicketTypeId" />
  </div>
</template>

<script>
import dayjs from "dayjs";
import BottomButton from "@/components/BottomButton.vue";
import RefundDescription from "./RefundDescription.vue";
import orderService from "@/services/orderService.js";

export default {
  components: {
    BottomButton,
    RefundDescription
  },
  props: {
    listNo: {
      type: String
    }
  },
  data() {
    return {
      order: {},
      showDescription: false,
      currentTicketTypeId: 0,
      reasonIndex: -1,
      reasons: ["行程不确定", "价格原因", "需要更改出行日期", "评价不好", "其他原因"],
      loading: false
    };
  },
  computed: {
    refundMoney() {
      let totalMoney = 0;
      if (this.order.details) {
        this.order.details.forEach(d => {
          totalMoney += d.reaPrice * d.refundQuantity;
        });
      }

      return totalMoney;
    }
  },
  async created() {
    const order = await orderService.getOrderInfoForMobileAsync(this.listNo);
    for (let detail of order.details) {
      detail.refundQuantity = detail.refundableQuantity;
      detail.minQuantity = detail.allowPartialRefund === false ? detail.refundableQuantity : 1;
    }
    this.order = order;
  },
  methods: {
    showRefundDescription(ticketTypeId) {
      this.currentTicketTypeId = ticketTypeId;
      this.showDescription = true;
    },
    onQuantityChange(detail) {
      if (detail.refundQuantity > detail.refundableQuantity) {
        this.$nextTick(() => {
          detail.refundQuantity = detail.refundableQuantity;
        });
      }
    },
    async refund() {
      try {
        this.loading = true;

        if (this.reasonIndex === -1) {
          this.$toast("取消原因不能为空");
          return;
        }

        let input = {
          listNo: this.listNo,
          reason: this.reasons[this.reasonIndex],
          details: this.order.details
            .filter(d => d.refundQuantity > 0)
            .map(d => {
              return { id: d.id, refundQuantity: d.refundQuantity };
            })
        };
        await orderService.applyRefundAsync(input);

        const lastTime = dayjs()
          .addDays(2)
          .format("YYYY-MM-DD 23:00");
        await this.$dialog.alert({
          message: `退订申请已提交,我们最晚在北京时间${lastTime}前为您处理`,
          confirmButtonText: "知道了"
        });
        this.$router.go(-1);
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style lang="scss">
.refund {
  &-tip {
    position: relative;
    padding: 7px 15px;
    background-color: #fff7e0;
    font-size: 13px;
    color: #481a03;
    line-height: 18px;
  }

  &-list {
    background-color: #fff;

    &-title {
      position: relative;
      height: 35px;
      line-height: 35px;
      font-size: 16px;
      color: #333;
      font-weight: 700;
      padding-left: 15px;
      border-bottom: 1px solid #ececec;

      &-tips {
        font-size: 12px;
        font-weight: normal;
        color: #999;
      }
    }

    ul {
      padding-left: 15px;
    }

    li {
      padding: 10px 0;
      list-style: none;
    }

    &-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-right: 15px;
    }
  }

  &-money {
    position: relative;
    min-height: 50px;
    line-height: 50px;
    font-size: 14px;
    font-weight: 400;
    margin-left: 15px;

    &-pay {
      &-fee {
        vertical-align: -1px;
        font-size: 16px;
        font-style: normal;
      }
    }

    &-amount {
      position: absolute;
      right: 0;
      padding: 0 15px;
      color: #333;

      &-fee {
        font-size: 16px;
        vertical-align: -1px;
        color: #f40;
      }
    }
  }

  &-ticket-name {
    font-size: 14px;
    flex: 1;
  }

  &-copies {
    padding-left: 20px;
    font-size: 14px;
  }

  &-rules {
    margin-top: 3px;
    line-height: 12px;
    font-size: 12px;
    color: #19a0f0;
    display: flex;
    align-items: center;
  }

  &-price {
    padding-left: 10px;
    font-size: 12px;
    text-align: right;
    color: #f40;
  }

  &-reason {
    height: 45px;
    align-items: center;
    border-bottom: 1px solid #ececec;

    .van-radio__input {
      margin-top: -5px;
    }
  }
}
</style>
