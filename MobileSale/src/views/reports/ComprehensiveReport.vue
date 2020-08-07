<template>
  <div class="comprehensive-report">
    <van-panel title="客流情况" class="margin-bottom-10">
      <div class="passenger-flow">
        <van-row>
          <van-col span="8" class="passenger-flow-realtime">
            <div>园内实时人数</div>
            <div class="passenger-flow-realtime-num">{{ realTimeNum }}</div>
          </van-col>
          <van-col span="16">
            <ve-bar
              :data="stadiumPersonData"
              :settings="stadiumPersonChartSettings"
              :extend="stadiumPersonChartOptions"
              :tooltip-visible="false"
              :legend-visible="false"
              :data-empty="stadiumPersonData.rows.length === 0"
              height="100px"
            ></ve-bar>
          </van-col>
        </van-row>
        <van-row class="passenger-flow-stat">
          <van-col span="8" class="van-hairline--right">
            <div>今日入馆人数</div>
            <div class="passenger-flow-stat-today">{{ todayCheckInNum }}</div>
          </van-col>
          <van-col span="8" class="van-hairline--right">
            <div>今日接待团队数</div>
            <div class="passenger-flow-stat-group">{{ groupNum }}</div>
          </van-col>
          <van-col span="8">
            <div>本月入园人数</div>
            <div class="passenger-flow-stat-month">{{ monthCheckInNum }}</div>
          </van-col>
        </van-row>
      </div>
    </van-panel>
    <van-panel title="团队情况">
      <div class="group">
        <div>
          <div class="group-title">当前接待团队</div>
          <div class="group-content">
            <div v-for="order in groupData.visitingGroups" :key="order.listNo">
              {{ order.customerName }}{{ order.totalNum }}人
              <span v-if="order.explainerName">，讲解员：{{ order.explainerName }}</span>
            </div>
          </div>
        </div>
        <div>
          <div class="group-title">已离馆团队</div>
          <div class="group-content">
            <div v-for="order in groupData.leavedGroups" :key="order.listNo">
              {{ order.customerName }}{{ order.totalNum }}人
              <span v-if="order.explainerName">，讲解员：{{ order.explainerName }}</span>
            </div>
          </div>
        </div>
        <div>
          <div class="group-title">未到馆团队</div>
          <div class="group-content">
            <div v-for="order in groupData.unCheckInGroups" :key="order.listNo">
              {{ order.customerName }}{{ order.totalNum }}人
              <span v-if="order.explainerTimeslotName"
                >，讲解场次：{{ order.explainerTimeslotName }}</span
              >
            </div>
          </div>
        </div>
      </div>
    </van-panel>
  </div>
</template>

<script>
import dayjs from "dayjs";
import VeBar from "v-charts/lib/bar.common.js";
import "v-charts/lib/style.css";
import orderService from "@/services/orderService.js";
import ticketService from "@/services/ticketService.js";

const today = dayjs().toDateString();

export default {
  components: {
    VeBar
  },
  data() {
    return {
      realTimeNum: 0,
      stadiumPersonChartSettings: {
        itemStyle: {
          color: "#5AB1EF"
        },
        label: {
          show: true,
          position: "right"
        }
      },
      stadiumPersonChartOptions: {
        grid: {
          top: 10,
          bottom: 0,
          right: 50
        },
        xAxis: {
          show: false
        }
      },
      stadiumPersonData: {
        columns: ["gateName", "realTime"],
        rows: []
      },
      todayCheckInNum: 0,
      groupNum: 0,
      monthCheckInNum: 0,
      groupData: {
        visitingGroups: [],
        leavedGroups: [],
        unCheckInGroups: []
      }
    };
  },
  created() {
    orderService
      .getOrdersAsync({
        startTravelDate: today,
        endTravelDate: today,
        hasCustomer: true,
        needCheckTime: true,
        shouldPage: false
      })
      .then(orders => {
        this.groupNum = orders.totalCount;
        this.groupData.visitingGroups = orders.items.filter(
          order => order.checkInTime && !order.checkOutTime
        );
        this.groupData.leavedGroups = orders.items.filter(order => order.checkOutTime);
        this.groupData.unCheckInGroups = orders.items.filter(order => !order.checkInTime);
      });

    ticketService.getTicketCheckOverviewAsync(today).then(response => {
      this.realTimeNum = response.scenicRealTimeQuantity;
      this.todayCheckInNum = response.scenicCheckInQuantity;
      this.stadiumPersonData.rows = response.stadiumOverview;
    });

    ticketService
      .getScenicCheckInQuantityAsync({
        StartDate: dayjs().format("YYYY-MM-01"),
        EndDate: today
      })
      .then(response => {
        this.monthCheckInNum = response;
      });
  }
};
</script>

<style lang="scss">
.comprehensive-report {
  .passenger-flow {
    text-align: center;
    font-size: 14px;
    font-weight: bolder;
    color: dimgray;

    &-realtime {
      margin-top: 20px;
      font-size: 16px;

      &-num {
        margin-top: 10px;
        color: red;
      }
    }

    &-stat {
      height: 60px;

      &-today {
        margin-top: 10px;
        color: rgb(255, 106, 69);
      }

      &-group {
        margin-top: 10px;
        color: rgb(69, 181, 183);
      }

      &-month {
        margin-top: 10px;
        color: rgb(0, 102, 255);
      }
    }
  }

  .group {
    padding: 0 15px;
    padding-bottom: 20px;
    font-size: 14px;

    &-title {
      font-weight: bolder;
      color: dimgray;
      margin: 10px 0;
    }

    &-content {
      background-color: #f2f2f2;
      padding: 10px 10px;
      border-radius: 5px;
      font-size: 13px;
    }
  }
}
</style>
