<template>
  <el-dialog
    title="门票详情"
    :visible.sync="showDialog"
    :close-on-click-modal="false"
    @closed="onClosed"
    :width="width"
    class="dialog"
  >
    <div v-loading="loading">
      <div class="border">
        <div class="title">门票信息</div>
        <el-form label-width="90px" class="info-form">
          <el-form-item label="票号">{{ ticket.ticketCode }}</el-form-item>
          <el-form-item label="单号">{{ ticket.listNo }}</el-form-item>
          <el-form-item label="票类">{{ ticket.ticketTypeName }}</el-form-item>
          <el-form-item label="状态">{{ ticket.ticketStatusName }}</el-form-item>
          <el-form-item label="单价">{{ ticket.reaPrice }}</el-form-item>
          <el-form-item label="人数">{{ ticket.quantity }}</el-form-item>
          <el-form-item label="实售金额">{{ ticket.realMoney }}</el-form-item>
          <el-form-item label="付款方式">{{ ticket.payTypeName }}</el-form-item>
          <el-form-item label="起始有效期">{{ ticket.stime }}</el-form-item>
          <el-form-item label="最晚有效期">{{ ticket.etime }}</el-form-item>
          <el-form-item label="剩余数量">{{ ticket.surplusQuantity }}</el-form-item>
          <el-form-item label="售票时间">{{ ticket.ctime }}</el-form-item>
          <el-form-item v-show="ticket.contactName" label="联系人">{{
            ticket.contactName
          }}</el-form-item>
          <el-form-item v-show="ticket.contactMobile" label="联系人手机">{{
            ticket.contactMobile
          }}</el-form-item>
          <el-form-item v-show="ticket.contactCertNo" label="联系人证件">{{
            ticket.contactCertNo
          }}</el-form-item>
        </el-form>
      </div>
      <div v-show="ticket.grounds && ticket.grounds.length > 0">
        <div class="title">可使用项目</div>
        <el-table :data="ticket.grounds" style="width: 100%">
          <el-table-column prop="groundName" label="项目" />
          <el-table-column prop="surplusNum" label="剩余次数" />
          <el-table-column v-if="showGroundChangCi" prop="changCiName" label="场次" />
          <el-table-column prop="stime" label="起始有效期" />
          <el-table-column prop="etime" label="最晚有效期" />
        </el-table>
      </div>
      <div v-show="ticket.seats && ticket.seats.length > 0">
        <div class="title">座位信息</div>
        <el-table :data="ticket.seats" style="width: 100%">
          <el-table-column prop="sdate" label="日期" />
          <el-table-column prop="changCiName" label="场次" />
          <el-table-column prop="stadiumName" label="场馆" />
          <el-table-column prop="regionName" label="片区" />
          <el-table-column prop="seatName" label="座位" />
        </el-table>
      </div>
      <div v-show="ticket.tourists && ticket.tourists.length > 0">
        <div class="title">游客信息</div>
        <el-table :data="ticket.tourists" style="width: 100%">
          <el-table-column prop="name" label="姓名" />
          <el-table-column v-if="showTouristMobile" prop="mobile" label="手机号码" />
          <el-table-column prop="certNo" label="证件号码" />
        </el-table>
      </div>
      <div v-show="ticket.ticketChecks && ticket.ticketChecks.length > 0">
        <div class="title">检票记录</div>
        <el-table :data="ticket.ticketChecks" style="width: 100%">
          <el-table-column prop="groundName" label="项目" />
          <el-table-column prop="gateGroupName" label="检票点" />
          <el-table-column prop="gateName" label="通道" />
          <el-table-column prop="checkNum" label="检票次数" />
          <el-table-column prop="inOutFlagName" label="出入标志" />
          <el-table-column prop="ctime" label="检票时间" min-width="140" />
          <el-table-column min-width="80" :label="'是否二次入' + '园'">
            <template slot-scope="scope">
              <el-tag size="medium" v-if="scope.row.isSecondInFlag">是</el-tag>
              <el-tag size="medium" v-else type="danger">否</el-tag>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
    <span slot="footer" class="dialog-footer">
      <el-button type="primary" @click="showDialog = false">确 定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import ticketService from "./../../services/ticketService.js";
import scenicService from "@/services/scenicService.js";

export default {
  name: "TicketSaleDetail",
  props: {
    value: {
      type: Boolean,
      default: false
    },
    ticketId: {
      type: Number,
      default: -1
    }
  },
  data() {
    return {
      showDialog: false,
      width: "60%",
      ticket: {},
      loading: false,
      pageLabelMainText: '景区'
    };
  },
  computed: {
    showGroundChangCi() {
      if (this.ticket.grounds) {
        return this.ticket.grounds.some(g => g.changCiName);
      }
      return false;
    },
    showTouristMobile() {
      if (this.ticket.tourists) {
        return this.ticket.tourists.some(t => t.mobile);
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
  created(){
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    onClosed() {
      this.showDialog = false;
      this.$emit("input", this.showDialog);
    },
    async load() {
      try {
        this.loading = true;

        this.ticket = {};
        this.ticket = await ticketService.getTicketFullInfoAsync({ id: this.ticketId });
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
