<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :md="14" :lg="11" :xl="8">
            <el-form-item label="时间段">
              <datetime-range v-model="queryTime" style="width:320px;"/>
            </el-form-item>
          </el-col>
          <el-col :md="10" :lg="6" :xl="6">
            <el-form-item label-width="20px">
              <el-radio-group v-model="input.statType">
                <el-radio
                  v-for="(item,index) in statTypes"
                  :key="index"
                  :label="item.value"
                >{{item.displayText}}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="7" :xl="10" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportToExcel" :loading="loading">导出</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div v-if="tableData.rows.length>0" style="margin-top:20px;">
      <ve-line :data="chartData" :extend="chartOptions" height="400px"></ve-line>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table
        v-if="hasTableColumn"
        :data="tableData.rows"
        border
        stripe
        :max-height="tableHeight"
        v-loading="loading"
      >
        <el-table-column
          v-for="column in tableData.columns"
          :key="column"
          :prop="column"
          :label="column"
          :formatter="formatter"
          :min-width="90"
        ></el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "./../../../mixins/statViewMixin.js";
import ticketService from "./../../../services/ticketService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput {
  constructor() {
    this.startCTime = "";
    this.endCTime = "";
    this.statType = 1;
  }
}

export default {
  name: "TradeSource",
  mixins: [statViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      statTypes: [
        { value: 1, displayText: "按日" },
        { value: 2, displayText: "按月" },
        { value: 3, displayText: "按年" }
      ],
      chartData: {
        columns: [],
        rows: []
      },
      chartOptions: {
        legend: {
          orient: "vertical",
          top: "20",
          right: "20"
        }
      }
    };
  },
  created() {},
  methods: {
    async getData() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        const result = await ticketService.statTicketCheckByTradeSourceAsync(
          this.input
        );
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;
        this.chartData.columns = result.columns;
        this.chartData.rows = result.data;
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.chartData.columns = [];
      this.chartData.rows = [];
      this.clear();
    },
    async exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        await ticketService.statTicketCheckByTradeSourceToExcelAsync(this.input);
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择时间段");
        return false;
      }

      this.input.startCTime = this.queryTime[0];
      this.input.endCTime = this.queryTime[1];

      return true;
    }
  }
};
</script>
