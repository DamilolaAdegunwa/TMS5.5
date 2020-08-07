<template>
  <el-dialog
    title="订单详情"
    :visible.sync="showDialog"
    :close-on-click-modal="false"
    @closed="onClosed"
    :width="width"
    class="dialog"
  >
    <div v-loading="loading">
      <div class="border">
        <div class="title">订单信息</div>
        <el-form label-width="90px" class="info-form">
          <el-form-item label="单号">{{ order.listNo }}</el-form-item>
          <el-form-item label="游玩日期">{{ order.travelDate }}</el-form-item>
          <el-form-item label="总金额">{{ order.totalMoney }}</el-form-item>
          <el-form-item v-show="order.payTypeName" label="付款方式">{{
            order.payTypeName
          }}</el-form-item>
          <el-form-item label="订单状态">{{ order.orderStatusName }}</el-form-item>
          <el-form-item label="消费状态">{{ order.consumeStatusName }}</el-form-item>
          <el-form-item label="订单类型">{{ order.orderTypeName }}</el-form-item>
          <el-form-item v-if="order.contact && order.contact.contactName" label="联系人">{{
            order.contact.contactName
          }}</el-form-item>
          <el-form-item v-if="order.contact && order.contact.contactMobile" label="联系人手机">{{
            order.contact.contactMobile
          }}</el-form-item>
          <el-form-item v-if="order.contact && order.contact.contactCertNo" label="联系人证件">{{
            order.contact.contactCertNo
          }}</el-form-item>
          <el-form-item v-show="order.customerName" label="客户">{{
            order.customerName
          }}</el-form-item>
          <el-form-item v-show="order.memberName" label="会员">{{ order.memberName }}</el-form-item>
          <el-form-item v-show="order.guiderName" label="导游">{{ order.guiderName }}</el-form-item>
          <el-form-item v-show="order.promoterName" label="推广员">{{
            order.promoterName
          }}</el-form-item>
          <el-form-item v-show="order.licensePlateNumber" label="车牌号">{{
            order.licensePlateNumber
          }}</el-form-item>
          <el-form-item v-show="order.keYuanTypeName" label="客源类型">{{
            order.keYuanTypeName
          }}</el-form-item>
          <el-form-item v-show="order.areaName" label="客源地">{{ order.areaName }}</el-form-item>
          <el-form-item v-show="order.thirdListNo" label="第三方单号">{{
            order.thirdListNo
          }}</el-form-item>
          <el-form-item label="下单时间">{{ order.cTime }}</el-form-item>
          <el-form-item v-show="order.memo" label="备注">{{ order.memo }}</el-form-item>
        </el-form>
      </div>
      <div v-show="order.details && order.details.length > 0">
        <div class="title">订单明细</div>
        <el-table :data="order.details" style="width: 100%">
          <el-table-column prop="ticketTypeName" label="票类" />
          <el-table-column prop="totalNum" label="总数量" />
          <el-table-column prop="usableQuantity" label="使用数量" />
          <el-table-column prop="refundQuantity" label="退票数量" />
          <el-table-column prop="surplusNum" label="剩余数量" />
          <el-table-column prop="reaPrice" label="单价" />
        </el-table>
      </div>
      <div v-show="order.tickets && order.tickets.length > 0">
        <div class="title">门票信息</div>
        <el-table :data="order.tickets" style="width: 100%">
          <el-table-column prop="ticketTypeName" label="票类" />
          <el-table-column prop="ticketCode" label="票号" />
          <el-table-column prop="ticketStatusName" label="票状态" />
          <el-table-column prop="quantity" label="总数量" />
          <el-table-column prop="surplusQuantity" label="剩余数量" />
          <el-table-column prop="stime" label="起始有效期" />
          <el-table-column prop="etime" label="最晚有效期" />
        </el-table>
      </div>
      <div v-show="order.changCis && order.changCis.length > 0">
        <div class="title">场次信息</div>
        <el-table :data="order.changCis" style="width: 100%">
          <el-table-column prop="groundName" label="项目" />
          <el-table-column prop="changCiName" label="场次" />
          <el-table-column v-if="showChangCiSeat" prop="seatName" label="座位" />
        </el-table>
      </div>
      <div v-show="order.tourists && order.tourists.length > 0">
        <div class="title">游客信息</div>
        <el-table :data="order.tourists" style="width: 100%">
          <el-table-column prop="name" label="姓名" />
          <el-table-column v-if="showTouristMobile" prop="mobile" label="手机号码" />
          <el-table-column prop="certNo" label="证件号码" />
        </el-table>
      </div>
    </div>
    <span slot="footer" class="dialog-footer">
      <el-button type="primary" @click="showDialog = false">确 定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import orderService from "./../../services/orderService.js";

export default {
  name: "OrderDetail",
  props: {
    value: {
      type: Boolean,
      default: false
    },
    listNo: {
      type: String,
      default: ""
    }
  },
  data() {
    return {
      showDialog: false,
      width: "60%",
      order: {},
      loading: false
    };
  },
  computed: {
    showChangCiSeat() {
      if (this.order.changCis) {
        return this.order.changCis.some(c => c.seatName);
      }
      return false;
    },
    showTouristMobile() {
      if (this.order.tourists) {
        return this.order.tourists.some(t => t.mobile);
      }
      return false;
    }
  },
  watch: {
    async value(val) {
      this.showDialog = val;
      if (val) {
        await this.load();
      }
    }
  },
  mounted() {
    if (document.body.clientWidth <= 1440) {
      this.width = "72%";
    }
  },
  methods: {
    onClosed() {
      this.showDialog = false;
      this.$emit("input", this.showDialog);
    },
    async load() {
      try {
        this.loading = true;

        this.order = {};
        const order = await orderService.getOrderFullInfoAsync({ listNo: this.listNo });

        const tickets = [];
        const changCis = [];
        for (const detail of order.details) {
          for (const ticket of detail.tickets) {
            tickets.push(ticket);
          }

          if (detail.groundChangCis) {
            for (const changCi of detail.groundChangCis) {
              changCis.push(changCi);

              let seatName = "";
              if (changCi.seats) {
                for (const seat of changCi.seats) {
                  seatName += `${seat.seatName}，`;
                }
                seatName = seatName.substr(0, seatName.length - 1);
              }
              changCi.seatName = seatName;
            }
          }
        }
        order.tickets = tickets;
        order.changCis = changCis;

        this.order = order;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.dialog {
  /deep/ .el-dialog__header {
    display: none;
  }

  /deep/ .el-dialog__body {
    padding-top: 10px;
  }
}

.title {
  font-size: 18px;
  font-weight: 700;
  padding: 20px 0;
}

.border {
  border-bottom: 1px solid #ebeef5;
}

.info-form {
  display: flex;
  flex-wrap: wrap;

  .el-form-item {
    width: 25%;
  }

  /deep/ .el-form-item__content {
    word-break: break-all;
  }
}
</style>
