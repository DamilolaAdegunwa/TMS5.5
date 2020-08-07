<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="检票时间">
              <datetime-range v-model="checkTime" :default-time="['00:00:00', '23:59:59']" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="项目">
              <el-select v-model="input.groundId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in grounds"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :md="14" :lg="10" :xl="7">
            <el-form-item label-width="20px">
              <el-radio-group v-model="input.statType">
                <el-radio :label="1">按时间段</el-radio>
                <el-radio :label="3">按月</el-radio>
                <el-radio :label="5">按年</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div class="div-button">
      <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
      <el-button @click="reset">重置</el-button>
      <el-button @click="exportToExcel">导出</el-button>
    </div>
    <div class="chart-box">
      <ve-chart :data="chartData" :settings="chartSettings" :extend="chartOptions" height="400px"></ve-chart>
    </div>
    <div ref="tableBox" v-show="hasTableData" class="table-box">
      <el-table :data="tableData.rows" border stripe v-loading="loading">
        <el-table-column
          v-for="column in tableData.columns"
          :key="column"
          :prop="column"
          :label="column"
          :formatter="formatter"
          :min-width="95"
        ></el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin, StatResult } from "./../../../mixins/statViewMixin.js";
import ticketService from "./../../../services/ticketService.js";
import { rowConvertToColumn } from "./../../../utils/data.js";
import { startTime, endTime } from "./../../../utils/datetime.js";
import scenicService from "@/services/scenicService.js";

class StatInput {
  constructor() {
    this.startCTime = "";
    this.endCTime = "";
    this.statType = 1;
    this.ifByGround = true;
    this.groundId = "";
  }
}

export default {
  name: "StatTicketCheckDayIn",
  mixins: [statViewMixin],
  data() {
    return {
      checkTime: [startTime, endTime],
      input: new StatInput(),
      chartSettings: {
        type: "line",
        radius: 170,
        offsetY: 200,
        label: {
          formatter: "{b}：{d}%"
        }
      },
      chartOptions: {
        grid: {
          top: 10,
          bottom: 0
        },
        legend: {
          orient: "vertical",
          top: "20",
          right: "20"
        }
      },
      maxLines: 10,
      grounds: []
    };
  },
  created() {
    scenicService.getGroundComboboxItemsAsync().then(data => {
      this.grounds = data;
    });
  },
  methods: {
    async getData() {
      if (!this.checkTime || this.checkTime.length != 2) {
        this.$message.error("请选择检票时间");
        return;
      }

      this.input.startCTime = this.checkTime[0];
      this.input.endCTime = this.checkTime[1];

      try {
        this.loading = true;
        const result = await ticketService.statTicketCheckInAsync(this.input);
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;

        if (result.columns.length > 3) {
          this.chartSettings.type = "line";
          this.chartData = new StatResult();
          if (!this.hasTableData) {
            return;
          }

          if (result.data.length <= this.maxLines) {
            let rows = rowConvertToColumn(
              result.data,
              result.columns,
              result.columns[1],
              "xAxisName",
              ["合计", "项目"]
            );
            this.chartData.columns = Object.keys(rows[0]);
            this.chartData.rows = rows;
          } else {
            this.chartData.columns = ["timeslot", "合计"];
            const columns = result.columns.slice(2, result.columns.length - 1);
            columns.forEach(key => {
              this.chartData.rows.push({
                timeslot: key,
                合计: result.data.map(item => item[key]).reduce((s, j) => s + j)
              });
            });
          }
        } else {
          this.chartSettings.type = "pie";
          this.chartData.columns = result.columns.slice(1, result.columns.length);
          this.chartData.rows = result.data;
        }
      } catch (error) {
        return;
      } finally {
        this.loading = false;
      }
    },
    reset() {
      this.input = new StatInput();
      this.checkTime = [startTime, endTime];
      this.clear();
    },
    async exportToExcel() {
      await this.submit(async () => {
        await ticketService.statTicketCheckInToExcelAsync(this.input);
      });
    }
  }
};
</script>

<style lang="scss">
.div-button {
  text-align: right;
}
</style>