<template>
  <section v-if="pageLoaded">
    <form action="/" class="search">
      <van-search
        v-model="ticketCode"
        placeholder="请输入票号或扫码"
        show-action
        shape="round"
        background="#fff"
        @search="onSearch"
      >
        <div slot="action">
          <span @click="onSearch">搜索</span>
          <span class="scan" @click="onScan">扫码</span>
        </div>
      </van-search>
    </form>

    <template v-if="ticketInfo && !isLoading">
      <van-panel title="门票信息" class="panel">
        <van-cell-group :border="false">
          <van-cell title="票类名称" :value="ticketInfo.ticketTypeName" />
          <van-cell title="游玩日期" :value="ticketInfo.stime.substr(0, 10)" />
          <van-cell title="人数" :value="ticketInfo.quantity" />
          <van-cell title="金额" :value="ticketInfo.realMoney" />
          <van-cell title="票号" :value="ticketInfo.ticketCode" />
          <van-cell title="单号" :value="ticketInfo.listNo" />
          <van-cell title="票状态" :value="ticketInfo.ticketStatusName" />
          <van-cell title="售票时间" :value="ticketInfo.ctime" />
        </van-cell-group>
      </van-panel>

      <van-panel v-if="ticketInfo.grounds" title="检票信息" class="panel">
        <table class="table">
          <tr class="th">
            <td>检票区域</td>
            <td>剩余次数</td>
          </tr>
          <tr v-for="ground in ticketInfo.grounds" :key="ground.id">
            <td>{{ ground.groundName }}</td>
            <td>{{ ground.surplusNum }}</td>
          </tr>
        </table>
      </van-panel>
    </template>
  </section>
</template>

<script>
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import weiXinJsSdkHelper from "./../../utils/weiXinJsSdkHelper.js";
import ticketService from "./../../services/ticketService.js";
import settingService from "./../../services/settingService.js";

export default {
  mixins: [mobileMixin],
  data() {
    return {
      ticketCode: "",
      ticketInfo: null
    };
  },
  async created() {
    try {
      this.loading();

      await settingService.configWxJsApi();

      if (this.$route.params.ticketCode) {
        this.ticketCode = this.$route.params.ticketCode;
        await this.getTicketInfo();
      }

      this.pageLoaded = true;
    } finally {
      this.loaded();
    }
  },
  methods: {
    async onSearch() {
      await this.getTicketInfo();
    },
    async onScan() {
      this.ticketCode = await weiXinJsSdkHelper.scanQRCode();
      await this.getTicketInfo();
    },
    async getTicketInfo() {
      if (!this.ticketCode) {
        this.$notify("请输入票号或扫码");
        return;
      }

      try {
        this.loading();

        this.ticketInfo = await ticketService.getTicketFullInfoAsync({
          ticketCode: this.ticketCode
        });
      } catch {
        this.ticketInfo = null;
      } finally {
        this.loaded();
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.search {
  /deep/ .van-cell {
    background-color: #f2f2f2;
  }

  .scan {
    margin-left: 10px;
  }
}

.panel {
  margin-top: 10px;

  /deep/ .van-cell__title {
    max-width: 90px;
  }

  /deep/ .van-cell__value {
    text-align: left;
  }

  .table {
    padding: 10px 15px;
    width: 100%;
    text-align: center;

    .th {
      font-weight: bold;
    }
  }
}
</style>
