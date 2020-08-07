<template>
  <div class="order-info">
    <van-panel
      v-if="orderInfo.orderStatusName"
      :title="orderInfo.orderStatusName"
      class="order-info-status"
      :class="{ 'margin-top-nav': showNavbar }"
    ></van-panel>

    <van-panel v-if="orderInfo.tickets.length > 0" title="门票信息" class="margin-bottom-10">
      <van-collapse v-model="activeTicketName" accordion @change="onAccordionChange">
        <van-collapse-item
          v-for="(ticket, index) in orderInfo.tickets"
          :key="index"
          :name="index"
          :class="{ 'order-info-qrcode-hide-arrow': ticket.completed }"
          class="order-info-qrcode"
        >
          <div slot="title">
            <van-row type="flex" justify="space-between" align="center">
              <van-col>票号：{{ ticket.ticketCode }}</van-col>
              <van-col v-if="!ticket.completed && activeTicketName !== index" style="color: #38f;">
                <van-icon name="qr" style="margin-right: 8px;" />
                <span>查看二维码</span>
              </van-col>
              <van-col v-if="ticket.completed" style="color: #38f;">{{
                ticket.ticketStatusName
              }}</van-col>
            </van-row>
          </div>
          <img v-if="!ticket.completed" :src="ticket.qrcode" />
        </van-collapse-item>
      </van-collapse>
    </van-panel>

    <van-panel title="订单信息" class="margin-bottom-10">
      <van-cell-group :border="false">
        <van-cell title="参观日期" :value="orderInfo.travelDate" class="van-field" />
        <van-cell
          v-if="orderInfo.personNum"
          title="参观人数"
          :value="orderInfo.personNum"
          class="van-field"
        />
        <van-cell
          v-if="orderInfo.arrivalTime"
          title="入馆时间"
          :value="orderInfo.arrivalTime"
          class="van-field"
        />
        <van-cell title="预约单号" :value="orderInfo.listNo" class="van-field" />
        <van-cell
          v-if="orderInfo.mobile"
          title="手机号"
          :value="orderInfo.mobile"
          class="van-field"
        />
        <van-cell v-if="orderInfo.keYuanTypeName" title="客源" :value="keYuan" class="van-field" />
        <van-cell
          v-if="orderInfo.jidiaoName"
          title="带队人员"
          :value="orderInfo.jidiaoName"
          class="van-field"
        />
        <van-cell
          v-if="orderInfo.licensePlateNumber"
          title="车牌号"
          :value="orderInfo.licensePlateNumber"
          class="van-field"
        />
        <van-cell
          v-if="orderInfo.explainerName"
          title="讲解员"
          :value="orderInfo.explainerName"
          class="van-field"
        />
        <van-cell
          v-if="orderInfo.explainerTime"
          title="讲解时段"
          :value="orderInfo.explainerTime"
          class="van-field"
        />
        <van-cell title="预约时间" :value="orderInfo.cTime" class="van-field" />
        <van-cell v-if="orderInfo.memo" title="备注" :value="orderInfo.memo" class="van-field" />
      </van-cell-group>
      <div v-if="orderInfo.allowCancel" slot="footer" class="order-info-cancel">
        <van-button size="small" :loading="loading" @click="onCancel">取消预约</van-button>
      </div>
    </van-panel>

    <van-panel v-if="orderInfo.tourists.length > 0" title="参观人信息" class="margin-bottom-10">
      <div
        v-for="(tourist, index) in orderInfo.tourists"
        :key="index"
        class="order-info-tourist van-hairline--bottom"
      >
        <span>{{ tourist.name }}，</span>
        <span v-if="tourist.certNo">{{ tourist.certNo }}</span>
        <span v-if="!tourist.certNo">{{ tourist.birthday }}</span>
      </div>
    </van-panel>

    <van-panel v-if="orderInfo.contact" title="联系人信息">
      <van-cell-group :border="false">
        <van-cell
          v-if="orderInfo.contact.contactName"
          title="姓名"
          :value="orderInfo.contact.contactName"
          class="van-field"
        />
        <van-cell title="手机号" :value="orderInfo.contact.contactMobile" class="van-field" />
      </van-cell-group>
    </van-panel>
  </div>
</template>

<script>
import qrcodeHelper from "./../../utils/qrcodeHelper.js";
import orderService from "@/services/orderService.js";

export default {
  name: "order-info",
  props: {
    listNo: {
      type: String
    },
    showNavbar: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      loading: false,
      activeTicketName: 0,
      orderInfo: {
        tickets: [],
        tourists: []
      }
    };
  },
  computed: {
    keYuan() {
      return `${this.orderInfo.keYuanTypeName}，${this.orderInfo.areaName}`;
    }
  },
  methods: {
    async onAccordionChange(index) {
      if (index === "" || this.orderInfo.tickets[index].qrcode) {
        return;
      }

      let ticket = this.orderInfo.tickets[index];
      if (!ticket.completed) {
        ticket.qrcode = await qrcodeHelper.createQRCodeAsync(ticket.ticketCode);
      }
    },
    async onCancel() {
      try {
        this.loading = true;
        await this.$dialog.confirm({ title: "确定取消预约？" });
        await this.cancelOrder();
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async cancelOrder() {
      let response = await orderService.cancelOrderAsync(this.listNo);
      if (response.success) {
        this.$toast("取消成功");
        await this.getOrderInfo();
      }
    },
    async getOrderInfo() {
      let order = await orderService.getOrderInfoForMobileAsync(this.listNo);
      if (order) {
        let ticket = order.tickets[0];
        if (!ticket.completed) {
          ticket.qrcode = await qrcodeHelper.createQRCodeAsync(ticket.ticketCode);
        }
        this.orderInfo = order;
      }
    }
  },
  watch: {
    listNo: async function() {
      await this.getOrderInfo();
    }
  },
  async created() {
    await this.getOrderInfo();
  }
};
</script>

<style lang="scss">
.order-info {
  .van-panel {
    .van-panel__content {
      .van-cell {
        font-size: 13px;

        .van-cell__title {
          color: rgb(152, 152, 152);
        }

        .van-cell__value {
          text-align: left;
        }
      }
    }
  }

  &-status {
    .van-panel__header {
      padding-top: 25px;
      background-color: #19a0f0;

      .van-cell__title {
        color: white;
      }
    }
  }

  &-cancel {
    text-align: right;
  }

  &-qrcode {
    text-align: center;

    &-hide-arrow {
      i {
        display: none;
      }
    }

    .van-collapse-item__content {
      padding: 0 15px;
    }
  }

  &-tourist {
    padding: 10px 15px;
    font-size: 14px;
  }
}
</style>
