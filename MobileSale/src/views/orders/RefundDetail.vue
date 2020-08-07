<template>
  <div>
    <div class="refund-top">{{ topText }}</div>
    <div v-for="(refundApply, index) in refundApplys" :key="index" class="refund-item">
      <div class="refund-head">
        <div class="refund-status">{{ refundApply.refundStatusName }}</div>
        <div class="refund-time">
          <span>{{ refundApply.refundTimeDescription }}</span>
        </div>
        <div class="refund-more">
          <span>退款金额</span>
          <span class="refund-more-price">￥{{ refundApply.refundMoney }}</span>
          <span>（{{ refundApply.refundQuantity }}张）</span>
          <span @click="change(refundApply)" class="refund-more-unfold">
            <span>{{ refundApply.isOpen ? "收起" : "展开" }}</span>
            <van-icon :name="refundApply.isOpen ? 'arrow-up' : 'arrow-down'" />
          </span>
        </div>
      </div>
      <div v-if="refundApply.isOpen" class="refund-step">
        <div class="refund-info">
          <div class="refund-info-label">退回账户</div>
          <div class="refund-info-content">
            {{ refundApply.refundRecvAccount }}
          </div>
        </div>
        <div class="refund-info">
          <div class="refund-info-label">退款单号</div>
          <div class="refund-info-content">{{ refundApply.refundListNo }}</div>
        </div>
        <div class="refund-dashed"></div>
        <van-steps
          direction="vertical"
          :active="refundApply.statusDetails.length - 1"
          active-color="#2a99f1"
        >
          <van-step v-for="(statusDetail, index) in refundApply.statusDetails" :key="index">
            <h3 class="refund-step-name">{{ statusDetail.title }}</h3>
            <p
              v-for="(detail, index) in statusDetail.details"
              :key="index"
              class="refund-step-label"
            >
              {{ detail }}
            </p>
          </van-step>
        </van-steps>
      </div>
    </div>
  </div>
</template>

<script>
import orderService from "./../../services/orderService.js";

export default {
  name: "RefundDetail",
  props: {
    listNo: {
      type: String
    }
  },
  data() {
    return {
      topText: "",
      refundApplys: []
    };
  },
  async created() {
    try {
      this.$toast.loading({
        duration: 0,
        message: "加载中..."
      });

      const refundApplys = await orderService.getRefundApplysWithStatusDetailAsync(this.listNo);
      refundApplys.forEach(item => {
        item.isOpen = refundApplys.length === 1;
      });
      const totalMoney = refundApplys.map(r => r.refundMoney).reduce((s, j) => s + j);
      this.topText = `您有${refundApplys.length}笔退款，共￥${totalMoney}`;
      this.refundApplys = refundApplys;
    } catch (err) {
      return;
    } finally {
      this.$toast.clear();
    }
  },
  methods: {
    change(refundApply) {
      if (refundApply.isOpen) {
        refundApply.isOpen = false;
      } else {
        this.refundApplys.forEach(item => (item.isOpen = false));
        refundApply.isOpen = true;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.refund {
  &-top {
    border-radius: 5px;
    padding: 10px 15px;
    font-size: 12px;
    background-color: #fff;
    color: #ff6230;
    margin: 12px 6px;
  }

  &-item {
    border-radius: 5px;
    padding: 15px 15px;
    background-color: #fff;
    margin: 12px 6px;
    line-height: 13px;
    font-size: 13px;
    font-weight: 300;
    color: #0b0d0f;
  }

  &-status {
    height: 19px;
    line-height: 19px;
    font-size: 19px;
    font-weight: 700;
    color: #0b0d0f;
    margin-bottom: 10px;
  }

  &-time {
    padding-bottom: 20px;
    border-bottom: 1px solid #dfe2e5;
  }

  &-more {
    padding: 18px 0 0;

    &-price {
      font-weight: 700;
      color: #f9504c;
      margin-left: 25px;
    }

    &-unfold {
      float: right;
      color: #168ae6;

      i {
        vertical-align: middle;
      }
    }
  }

  &-step {
    &-name,
    &-label {
      line-height: 13px;
      font-size: 13px;
      font-weight: 300;
    }

    &-label {
      margin-top: 8px;
      color: #b5b6b7;
    }

    &-name {
      color: #0b0d0f;
    }

    /deep/ .van-step__line,
    /deep/ .van-step__circle {
      background-color: #168ae6;
    }
  }

  &-info {
    padding-top: 8px;

    &-label,
    &-content {
      display: inline-block;
      opacity: 0.8;
      font-size: 13px;
      font-weight: 300;
      line-height: 13px;
      color: #0b0d0f;
    }

    &-content {
      margin-left: 25px;
    }
  }

  &-dashed {
    margin-top: 18px;
    margin-bottom: 8px;
    height: 1px;
    background: linear-gradient(90deg, #dfe2e5, #dfe2e5 5px, transparent 0, transparent);
    background-size: 10px 100%;
  }
}
.van-step--process {
  .refund-step-name {
    color: #2a99f1;
    font-weight: 700;
  }
  .refund-step-label {
    color: #0b0d0f;
  }
}
</style>
