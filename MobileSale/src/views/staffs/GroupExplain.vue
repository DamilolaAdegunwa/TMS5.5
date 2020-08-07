<template>
  <div class="list">
    <form action="/">
      <van-search
        v-model="customerName"
        placeholder="请输入团队名称"
        show-action
        @search="loadOrdersAsync"
        class="fixed-top"
      >
        <div slot="action" @click="loadOrdersAsync">搜索</div>
      </van-search>
      <div style="height:44px;"></div>
    </form>
    <no-data v-if="dataLoaded && orders.length === 0" :content-height="contentHeight"></no-data>
    <div v-else v-for="(order, index) in orders" :key="index" class="list-panel">
      <van-row type="flex" align="center">
        <van-col span="19">
          <div class="list-panel-title">{{ order.customerName }}</div>
          <div v-if="order.hasCheckIn && order.editable" class="list-panel-item list-panel-status">
            团队已入场
          </div>
          <div class="list-panel-item">单号：{{ order.listNo }}</div>
          <div class="list-panel-item">参观日期：{{ order.travelDate }}</div>
          <div v-if="order.ageRanges.length === 0" class="list-panel-item">
            人数：{{ order.totalNum }}
          </div>
          <div
            v-else
            v-for="(ageRange, index) in order.ageRanges"
            :key="index"
            class="list-panel-item"
          >
            {{ ageRange.ageRangeName }}：{{ ageRange.personNum }}人
          </div>
          <div v-if="order.keYuanTypeName" class="list-panel-item">
            客源：{{ order.keYuanTypeName }}，{{ order.areaName }}
          </div>
          <div v-if="order.jidiaoName" class="list-panel-item">
            带队人员：{{ order.jidiaoName }}
          </div>
          <div class="list-panel-item">场次：{{ order.timeslotName }}</div>
          <div v-if="order.explainerName" class="list-panel-item">
            讲解员：{{ order.explainerName }}
          </div>
          <div v-if="order.memo" class="list-panel-item">备注：{{ order.memo }}</div>
        </van-col>
        <van-col span="5">
          <van-button
            v-if="order.editable"
            type="primary"
            :loading="loading && currentIndex === index"
            @click="onClick(index)"
            >{{ order.beginTime ? "结束" : "开始" }}</van-button
          >
          <div v-else class="list-panel-status">
            {{ order.completeTime ? "已讲解" : "讲解中" }}
          </div>
        </van-col>
      </van-row>
    </div>
  </div>
</template>

<script>
import NoData from "@/components/NoData.vue";
import orderService from "@/services/orderService.js";
import explainerService from "@/services/explainerService.js";
import signalRHelper from "@/utils/signalRHelper.js";

const connection = signalRHelper.buildExplainerNotificationConnection();

export default {
  components: {
    NoData
  },
  data() {
    return {
      loading: false,
      dataLoaded: false,
      currentIndex: -1,
      customerName: "",
      orders: []
    };
  },
  computed: {
    contentHeight() {
      return this.$store.state.clientHeight - 90;
    }
  },
  methods: {
    async onClick(index) {
      this.currentIndex = index;
      let order = this.orders[index];
      try {
        this.loading = true;
        if (order.beginTime) {
          await this.$dialog.confirm({
            title: "确定结束讲解？"
          });
          await explainerService.completeExplainAsync({
            listNo: order.listNo
          });
        } else {
          await this.$dialog.confirm({
            title: "确定开始讲解？"
          });
          await explainerService.beginExplainAsync({
            listNo: order.listNo,
            timeslotId: order.explainerTimeId
          });
        }
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async loadOrdersAsync() {
      this.orders = await orderService.getOrdersForExplainAsync(this.customerName);
      this.dataLoaded = true;
    },
    layzeLoadOrder() {
      setTimeout(this.loadOrdersAsync, 300);
    }
  },
  async created() {
    await this.loadOrdersAsync();

    connection.on("BeginExplain", this.layzeLoadOrder);
    connection.on("CompleteExplain", this.layzeLoadOrder);
    connection.on("CheckIn", this.layzeLoadOrder);
    connection.start();
  },
  beforeDestroy() {
    connection.stop();
    connection.off("BeginExplain");
    connection.off("CompleteExplain");
    connection.off("CheckIn");
  }
};
</script>
