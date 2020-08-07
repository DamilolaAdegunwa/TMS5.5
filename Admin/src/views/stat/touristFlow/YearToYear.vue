<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :md="12" :lg="9" :xl="6">
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
          <el-col :md="12" :lg="15" :xl="18" class="button-box">
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
      <ve-histogram :data="chartData" :extend="chartOptions" height="400px"></ve-histogram>
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

class StatInput {
  constructor() {
    this.statType = 1;
  }
}

export default {
  name: "YearToYear",
  mixins: [statViewMixin],
  data() {
    return {
      input: new StatInput(),
      statTypes: [
        { value: 1, displayText: "前一年" },
        { value: 2, displayText: "前两年" },
        { value: 3, displayText: "前三年" }
      ],
      chartData: {
        columns: [],
        rows: []
      },
      chartOptions: {
        grid: {
          bottom: 0
        },
        series: {
          barMaxWidth: 50,
          label: { show: true, position: "top" },
          barGap: "0"
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
        const result = await ticketService.statTicketCheckInYearOverYearComparisonAsync(
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
        await ticketService.statTicketCheckInYearOverYearComparisonToExcelAsync(
          this.input
        );
      });
    },
    checkInput() {
      return true;
    }
  }
};
</script>
