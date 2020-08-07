<template>
  <div class="dashboard">
    <div class="background-color">
      <div class="div-row div-peak">
        <div class="div-row-panel panel-yester">
          <div class="yester-div yester-one">
            <div class="yester-title">昨日销售数</div>
            <div class="yester-content">{{ yesterSaleNum }}</div>
          </div>
          <div class="yester-div yester-two">
            <div class="yester-title">昨日检票数</div>
            <div class="yester-content">{{ yesterCheckNum }}</div>
          </div>
          <div class="yester-div yester-three">
            <div class="yester-title">本月售票数</div>
            <div class="yester-content">{{ thisMonthSaleNum }}</div>
          </div>
          <div class="yester-div yester-four">
            <div class="yester-title">本月检票数</div>
            <div class="yester-content">{{ thisMonthCheckNum }}</div>
          </div>
        </div>
        <div class="div-row-panel panel-peak">
          <div class="chart-title">日高峰期统计</div>
          <div class="div-line">
            <ve-line :data="peakChartData" height="240px" :settings="peakChartSettings" />
          </div>
        </div>
      </div>
      <div class="div-row div-progress">
        <div class="div-row-panel panel-today">
          <div class="today-div today-first" style="margin-left: unset">
            <div class="chart-title today-title">当天销售数</div>
            <div class="today-num">
              <el-progress
                type="circle"
                :percentage="todaySaleNumPercentage"
                color="#3fbb5d"
                :stroke-width="strokeWidth"
                :width="progressWidth"
                :show-text="false"
              />
              <div class="today-sale-num">{{ todaySaleNum }}</div>
            </div>
            <div class="today-bottom-sale">本日售票量</div>
          </div>
          <div class="today-div">
            <div class="chart-title today-title">当天检票数</div>
            <div class="today-num">
              <el-progress
                type="circle"
                :percentage="todayCheckNumPercentage"
                color="#fe762a"
                :stroke-width="strokeWidth"
                :width="progressWidth"
                :show-text="false"
              />
              <div class="today-sale-num">{{ todayCheckNum }}</div>
            </div>
            <div class="today-bottom-check">本日检票量</div>
          </div>
          <div class="today-div">
            <div class="chart-title today-title">当天退票数</div>
            <div class="today-num">
              <el-progress
                type="circle"
                :percentage="todayRefundNumPercentage"
                color="#18a9e2"
                :stroke-width="strokeWidth"
                :width="progressWidth"
                :show-text="false"
              />
              <div class="today-sale-num">{{ todayRefundNum }}</div>
            </div>
            <div class="today-bottom-refund">本日退单量</div>
          </div>
        </div>
        <div class="div-row-panel panel-type">
          <div class="chart-title">当天票种销售TOP5</div>
          <div v-if="typeChartData.rows.length > 0" style="margin-top: -23px;">
            <ve-bar
              :data="typeChartData"
              height="300px"
              :settings="typeChartSettings"
              :extend="typeExtend"
            />
          </div>
          <div v-else class="type-empty">暂无数据</div>
        </div>
      </div>
      <div class="div-row div-trade">
        <div class="div-row-panel panel-trade">
          <div class="trade-header">
            <div class="trade-title">本月销售渠道占比</div>
            <div class="trade-month">{{ thisMonth }}</div>
          </div>
          <div class="trade-pie">
            <ve-pie :data="tradeChartData" height="320px" :settings="pieSetting" />
          </div>
        </div>
        <div class="div-row-panel panel-week">
          <div class="chart-title">近七日{{'景点'}}检票趋势图</div>
          <div style="width: 95%; margin-left: 2.5%">
            <ve-line :data="weekChartData" height="330px" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import ticketService from "./../../services/ticketService.js";
import scenicService from "@/services/scenicService.js";
const weekArray = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];

export default {
  name: "Dashboard",
  data() {
    return {
      yesterSaleNum: 0,
      yesterCheckNum: 0,
      thisMonthSaleNum: 0,
      thisMonthCheckNum: 0,
      todaySaleNum: 0,
      todaySaleNumPercentage: 0,
      todayCheckNum: 0,
      todayCheckNumPercentage: 0,
      todayRefundNum: 0,
      todayRefundNumPercentage: 0,
      thisMonth: "",
      peakChartData: {
        columns: ["时间", "人数"],
        rows: [
          { 时间: "08:00", 人数: 0 },
          { 时间: "10:00", 人数: 0 },
          { 时间: "12:00", 人数: 0 },
          { 时间: "14:00", 人数: 0 },
          { 时间: "16:00", 人数: 0 },
          { 时间: "18:00", 人数: 0 }
        ]
      },
      typeChartSettings: {
        metrics: ["realNum"],
        dataOrder: {
          label: "realNum",
          order: "desc"
        },
        labelMap: {
          realNum: "销售数量"
        }
      },
      typeChartExtends: {
        yAxis: {
          type: "value",
          min: 0,
          minInterval: 1
        }
      },
      peakChartSettings: {
        min: [5, 5],
        yAxis: {
          type: "value",
          min: 1,
          minInterval: 1
        }
      },
      typeExtend: {
        series: {
          barMaxWidth: 30
        },
        xAxis: {
          type: "value",
          min: 0,
          minInterval: 1
        }
      },
      typeChartData: {
        columns: ["ticketTypeName", "realNum"],
        rows: [
          // { 票类: "成人票", 销售数量: 0 }
          // {'票类': '优惠票', '销售数量': 15},
          // {'票类': '儿童票', '销售数量': 8},
          // {'票类': '夜场票', '销售数量': 4},
          // {'票类': '学生票', '销售数量': 3},
          // {'票类': '军人票', '销售数量': 2},
          // {'票类': '外国人', '销售数量': 1}
        ]
      },
      tradeChartData: {
        columns: ["渠道", "销售数量"],
        rows: [
          { 渠道: "微信", 销售数量: 0 }
          // {'渠道': '自助机', '销售数量': 500},
          // {'渠道': '微官网', '销售数量': 400},
          // {'渠道': '直接入园', '销售数量': 100}
        ]
      },
      weekChartData: {
        columns: ["日期", "景点1"],
        rows: [
          {
            日期: "星期三",
            景点1: 15,
            "3D电影院": 20,
            "4D电影院": 25,
            儿童乐园: 30
          },
          {
            星期: "星期四",
            景点1: 20,
            "3D电影院": 25,
            "4D电影院": 30,
            儿童乐园: 15
          },
          {
            星期: "星期五",
            景点1: 25,
            "3D电影院": 30,
            "4D电影院": 15,
            儿童乐园: 20
          },
          {
            星期: "星期六",
            景点1: 30,
            "3D电影院": 15,
            "4D电影院": 20,
            儿童乐园: 25
          },
          {
            星期: "星期日",
            景点1: 10,
            "3D电影院": 20,
            "4D电影院": 30,
            儿童乐园: 40
          },
          {
            星期: "星期一",
            景点1: 20,
            "3D电影院": 30,
            "4D电影院": 40,
            儿童乐园: 10
          },
          {
            星期: "星期二",
            景点1: 30,
            "3D电影院": 40,
            "4D电影院": 10,
            儿童乐园: 20
          }
        ]
      },
      strokeWidth: 18,
      progressWidth: 120,
      pieSetting: {
        emphasis: {
          itemStyle: {
            color: ["#0693c8"],
            borderColor: "#ff0000",
            borderWidth: "2",
            borderType: "solid"
          }
        }
      },
      pageLabelMainText: '景区'
    };
  },
  methods: {
    async getDayStat() {
      let yesterDate = new Date();
      let todayDate = new Date(
        yesterDate.getFullYear() +
          "/" +
          (yesterDate.getMonth() + 1) +
          "/" +
          yesterDate.getDate() +
          " 23:59:59"
      );
      yesterDate.setTime(todayDate.getTime() - 24 * 60 * 60 * 1000);
      let yesterResult = await this.getDaySaleStat(yesterDate);
      let self = this;
      if (yesterResult.length > 0) {
        self.yesterSaleNum = yesterResult[0]["realNum"];
      }

      let typeResult = await this.getDaySaleStat(todayDate, undefined, 6);
      if (typeResult.length > 0) {
        self.getTypeChartData(typeResult);
      }

      yesterResult = await this.getDayCheckStat(yesterDate);
      if (yesterResult.data.length > 0) {
        self.yesterCheckNum = yesterResult.data[0]["人数"];
      }

      let todayResult = await this.getDayCheckStat(todayDate);
      if (todayResult.data.length > 0) {
        self.todayCheckNum = todayResult.data[0]["人数"];
      }

      let monthCTime = todayDate.getFullYear() + "-" + (todayDate.getMonth() + 1) + "-01";
      let tradeResult = await this.getDaySaleStat(todayDate, monthCTime, 7);
      if (tradeResult.length > 0) {
        self.getTradeChartData(tradeResult);
      }

      todayResult = await this.getDayCheckStat(todayDate, monthCTime);
      if (todayResult.data.length > 0) {
        self.thisMonthCheckNum = todayResult.data[0]["人数"];
      }

      let peakResult = await this.getDayCheckStat(todayDate, undefined, 1);
      if (peakResult.data.length > 0) {
        self.getPeakChartData(peakResult.data);
      }

      let weekDate = new Date();
      weekDate.setTime(weekDate.getTime() - 6 * 24 * 60 * 60 * 1000);
      let weekStartCTime =
        weekDate.getFullYear() + "-" + (weekDate.getMonth() + 1) + "-" + weekDate.getDate();
      let weekResult = await this.getDayCheckStat(todayDate, weekStartCTime, 2);
      // if (weekResult.data.length > 0) {
      self.getWeekChartData(weekResult.data);
      // }
      this.todayRefundNumPercentage =
        this.todayRefundNum > 0
          ? (this.todayRefundNum / (this.todaySaleNum + this.todayCheckNum + this.todayRefundNum)) *
            100
          : 0;
      this.todayCheckNumPercentage =
        this.todayCheckNum > 0
          ? (this.todayCheckNum / (this.todaySaleNum + this.todayCheckNum + this.todayRefundNum)) *
            100
          : 0;
      this.todaySaleNumPercentage =
        this.todaySaleNum > 0
          ? (this.todaySaleNum / (this.todaySaleNum + this.todayCheckNum + this.todayRefundNum)) *
            100
          : 0;
    },
    async getDaySaleStat(date, startCTime, statType) {
      if (startCTime == undefined) {
        startCTime = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
      }
      if (statType == undefined) {
        statType = 3;
      }
      let endCTime =
        date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate() + " 23:59:59";
      let result = await ticketService.statTicketSaleAsync({
        StartCTime: startCTime,
        EndCTime: endCTime,
        StatType: statType
      });
      return result;
    },
    async getDayCheckStat(date, startCTime, statType) {
      if (startCTime == undefined) {
        startCTime = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
      }
      if (statType == undefined) {
        statType = 3;
      }
      let endCTime =
        date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate() + " 23:59:59";
      let result = await ticketService.statTicketCheckInAsync({
        StartCTime: startCTime,
        EndCTime: endCTime,
        StatType: statType
      });
      return result;
    },
    getPeakChartData(data) {
      let rows = [];
      rows.push({ 时间: "08:00", 人数: this.getIntNum(data[0]["8点前"]) });
      rows.push({
        时间: "10:00",
        人数: this.getIntNum(data[0]["08-09"]) + this.getIntNum(data[0]["09-10"])
      });
      for (let i = 12; i < 17; i += 2) {
        let dataTen = data[0][i - 2 + "-" + (i - 1)];
        let dataEle = data[0][i - 1 + "-" + i];
        rows.push({
          时间: i + ":00",
          人数: this.getIntNum(dataTen) + this.getIntNum(dataEle)
        });
      }
      rows.push({
        时间: "18:00",
        人数:
          this.getIntNum(data[0]["16-17"]) +
          this.getIntNum(data[0]["17-18"]) +
          this.getIntNum(data[0]["18-19"]) +
          this.getIntNum(data[0]["19-20"]) +
          this.getIntNum(data[0]["20点后"])
      });
      this.peakChartData.rows = rows;
    },
    getIntNum(string) {
      let num = 0;
      if (string != null) {
        num = parseInt(string);
      }
      return num;
    },
    getTradeChartData(data) {
      this.thisMonthSaleNum = data[data.length - 1]["realNum"];
      let rows = [];
      for (let i = 0; i < data.length - 1; i++) {
        rows.push({
          渠道: data[i]["statType"],
          销售数量: this.getIntNum(data[i]["realNum"])
        });
      }
      this.tradeChartData.rows = rows;
    },
    getTypeChartData(data) {
      this.todaySaleNum = data[data.length - 1]["realNum"];
      this.todayRefundNum = data[data.length - 1]["returnNum"];
      if (data.length > 0) {
        data.splice(data.length - 1, 1);
      }
      this.typeChartData.rows = [];
      for (let i = 0; i < data.length; i++) {
        let ticketType = data[i];
        if (ticketType.realNum && ticketType.realNum > 0) {
          this.typeChartData.rows.push({
            ticketTypeName: ticketType.statType,
            realNum: ticketType.realNum
          });
        }
      }

      // let rows = [];
      // for (let i = 0; i < data.length - 1; i++) {
      //   rows.push({
      //     票类: data[i]["ticketTypeName"],
      //     销售数量: this.getIntNum(data[i]["realNum"])
      //   });
      // }
      // let dataRows = rows;
      // for (let i = 0; i < dataRows.length; i++) {
      //   let maxRow = i;
      //   for (let j = i + 1; j < dataRows.length; j++) {
      //     if (
      //       this.getIntNum(dataRows[j]["销售数量"]) >
      //       this.getIntNum(dataRows[maxRow]["销售数量"])
      //     ) {
      //       maxRow = j;
      //     }
      //   }
      //   let copyRow = dataRows[i];
      //   dataRows[i] = dataRows[maxRow];
      //   dataRows[maxRow] = copyRow;
      // }
      // this.typeChartData.rows = [];
      // for (let i = 0; i < dataRows.length && i < 5; i++) {
      //   this.typeChartData.rows.push(dataRows[i]);
      // }
    },
    getWeekChartData(data) {
      let todayDate = new Date();
      let rows = [];
      let weekDate = new Date();
      for (let i = 6; i > -1; i--) {
        weekDate.setTime(todayDate.getTime() - i * 24 * 60 * 60 * 1000);
        let weekName = weekArray[weekDate.getDay()];
        let dataI = data.find(d => d["星期"] == weekName);
        rows.push({
          日期: weekDate.getMonth() + 1 + "-" + weekDate.getDate(),
          景点1: dataI ? this.getIntNum(dataI["人数"]) : 0,
          "3D电影院": 25 + i,
          "4D电影院": 30 + i,
          儿童乐园: 15 + i
        });
      }
      this.weekChartData.rows = rows;
    },
    loopQuery() {
      this.queryTimer = setInterval(async () => {
        await this.getDayStat();
      }, 150000);
    }
  },
  async created() {
    this.pageLabelMainText = scenicService.getPageLabelMainText();
    this.thisMonth = new Date().getMonth() + 1 + "月";
    await this.getDayStat();
    this.loopQuery();
  }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";
.dashboard {
  min-height: 900px;
  height: calc(100vh - #{$header-height});
  background-color: #cccccc;
  display: flex;
  flex-direction: column;
  .background-color {
    background-color: #cccccc;
  }
  .div-row {
    padding: 10px 10px 10px 10px;
    width: 100%;
    display: flex;
    .div-row-panel {
      background-color: #ffffff;
      padding: 0px 0px;
    }
    .panel-yester {
      width: 60%;
      float: left;
      display: flex;
      align-items: center;
      height: 210px;
      .yester-div {
        float: left;
        width: 20.5%;
        margin-left: 2%;
        padding: 30px 0px 10px 10px;
        text-align: left;
        height: 70px;
        border-radius: 7px;
        color: #ffffff;
      }
      .yester-content {
        font-size: 32px;
        margin-top: 10px;
      }
      .yester-one {
        border: 1px solid #488fdf;
        background-color: #488fdf;
      }
      .yester-two {
        border: 1px solid #b78c24;
        background-color: #b78c24;
      }
      .yester-three {
        border: 1px solid #05bdc7;
        background-color: #05bdc7;
      }
      .yester-four {
        border: 1px solid #0693c8;
        background-color: #0693c8;
      }
    }
    .panel-type {
      float: left;
      width: 35%;
      height: 260px;
      margin-left: 2%;
      padding: 10px 0px 0px 10px;
    }
    .type-empty {
      margin: 0 auto;
      display: flex;
      align-items: center;
      text-align: center;
      align-content: center;
      height: 242px;
      width: 65px;
    }
    .panel-today {
      width: 60%;
      display: flex;
      justify-content: space-between;
      background-color: unset;
      .today-div {
        width: 31.3%;
        background-color: #ffffff;
        height: 270px;
      }
      .today-num {
        padding: 30px 0px 30px 0px;
        text-align: center;
      }
      .today-num-div {
        border: 15px solid #ffa500;
        border-radius: 105px;
        width: 70px;
        height: 46px;
        text-align: center;
        padding: 24px 0px 0px 0px;
        margin: 0 auto;
      }
      .today-title {
        padding: 10px 0px 5px 10px;
        border-bottom: 1px solid #ebedf0;
      }
      .today-num-sale {
        font-size: 20px;
      }
      .today-bottom-sale {
        color: #ffa500;
        text-align: center;
      }
      .today-num-check {
        border-color: #00d0ff;
        font-size: 20px;
      }
      .today-bottom-check {
        color: #00d0ff;
        text-align: center;
      }
      .today-num-refund {
        border-color: #7bb901;
        font-size: 20px;
      }
      .today-bottom-refund {
        color: #7bb901;
        text-align: center;
      }

      .today-sale-num {
        margin-top: -82px;
        height: 70px;
        font-size: 32px;
      }
    }
    .panel-peak {
      float: left;
      width: 35%;
      margin-left: 2%;
      padding: 10px 0px 0px 10px;
      height: 200px;
      .div-line {
        height: 200px;
        margin-top: -20px;
      }
    }
    .panel-trade {
      width: 35%;
      float: left;
      padding: 0px 0px 0px 10px;
      overflow: hidden;
      height: 340px;

      .trade-header {
        margin-top: -20px;
        margin-bottom: -100px;
      }
      .trade-title {
        width: 50%;
        float: left;
        margin-bottom: -80px;
        margin-top: 30px;
        text-align: left;
      }
      .trade-month {
        width: 40%;
        float: right;
        margin-top: 30px;
        margin-bottom: -77px;
        font-size: 20px;
      }
      .trade-pie {
        margin-top: -90px;
      }
    }
    .panel-week {
      float: left;
      width: 60%;
      margin-left: 2%;
      height: 340px;
      overflow: hidden;
      .chart-title {
        height: 20px;
        padding-top: 10px;
        margin-left: 2%;
      }
    }
  }
  .div-peak {
    height: 210px;
  }
  .div-progress {
    height: 270px;
  }
  .div-trade {
    height: 230px;
  }
}
</style>
