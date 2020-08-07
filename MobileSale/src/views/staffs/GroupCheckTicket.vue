<template>
  <div class="list">
    <form action="/">
      <van-search
        v-model="queryText"
        placeholder="请输入车牌号/团队名称"
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
          <div class="list-panel-item">车牌号：{{ order.licensePlateNumber }}</div>
          <div v-if="order.hasCheckIn" class="list-panel-item">
            入场时间：{{ order.checkInTime }}
          </div>
          <div v-if="order.hasCheckOut" class="list-panel-item">
            出场时间：{{ order.checkOutTime }}
          </div>
          <div v-if="order.memo" class="list-panel-item">备注：{{ order.memo }}</div>
        </van-col>
        <van-col span="5">
          <van-button
            v-if="!order.hasCheckOut"
            type="primary"
            :loading="loading && currentIndex === index"
            @click="onClick(index)"
            >{{ order.hasCheckIn ? "出场" : "入场" }}</van-button
          >
          <div v-else class="list-panel-status">已完成</div>
        </van-col>
      </van-row>
    </div>
  </div>
</template>

<script>
import dayjs from "dayjs";
import NoData from "@/components/NoData.vue";
import orderService from "@/services/orderService.js";
import signalRHelper from "@/utils/signalRHelper.js";

const connection = signalRHelper.buildCheckerNotificationConnection();

export default {
  components: {
    NoData
  },
  data() {
    return {
      loading: false,
      dataLoaded: false,
      currentIndex: -1,
      queryText: "",
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
        if (!order.hasCheckIn) {
          await this.$dialog.confirm({
            title: "确定入场？"
          });
          await orderService.consumeOrderFromMobileAsync(order.listNo);
        } else {
          await this.$dialog.confirm({
            title: "确定出场？"
          });
          await orderService.checkOutOrderFromMobileAsync(order.listNo);
        }
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async loadOrdersAsync() {
      let orders = await orderService.getGroupOrdersForConsumeAsync(this.queryText);
      orders.forEach(order => {
        if (order.checkInTime) {
          order.checkInTime = dayjs(order.checkInTime).toDateTimeString();
        }
        if (order.checkOutTime) {
          order.checkOutTime = dayjs(order.checkOutTime).toDateTimeString();
        }
      });
      this.orders = orders;
      this.dataLoaded = true;
    },
    layzeLoadOrder() {
      setTimeout(this.loadOrdersAsync, 300);
    }
  },
  async created() {
    await this.loadOrdersAsync();

    connection.on("CheckIn", this.layzeLoadOrder);
    connection.on("CheckOut", this.layzeLoadOrder);
    connection.start();
  },
  beforeDestroy() {
    connection.stop();
    connection.off("CheckIn");
    connection.off("CheckOut");
  }
};
</script>
