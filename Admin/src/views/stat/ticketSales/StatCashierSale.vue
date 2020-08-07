<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="销售时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="收银员">
              <el-select v-model="input.cashierId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in cashiers"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="售票点">
              <el-select v-model="input.salePointId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in salePoints"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="统计类型">
              <el-select v-model="input.statTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in statTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <div class="button-box">
          <div></div>
          <div>
            <el-button type="primary" @click="stat" :loading="loading">统计</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </div>
        </div>
      </el-form>
    </div>
    <iframe ref="iframe" class="iframe" :src="reportUrl" @load="onload"></iframe>
    <iframe class="hidden-iframe" :src="exportUrl" @load="onload"></iframe>
  </div>
</template>

<script>
import { reportViewMixin, ReportInputDto } from "./../../../mixins/reportViewMixin.js";
import staffService from "./../../../services/staffService.js";
import scenicService from "./../../../services/scenicService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.cashierId = "";
    this.cashierName = "";
    this.salePointId = "";
    this.salePointName = "";
  }
}

export default {
  name: "StatCashierSale",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      cashiers: [],
      salePoints: [],
      statTypes: [],
      staffReportUrl: undefined
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.startCTime = val[0];
        this.input.endCTime = val[1];
      }
    },
    "input.cashierId": function(val) {
      if (val) {
        const cashier = this.cashiers.filter(s => s.value == val)[0];
        this.input.cashierName = cashier.displayText;
      } else {
        this.input.cashierName = "";
      }
    },
    "input.salePointId": function(val) {
      if (val) {
        const salePoint = this.salePoints.filter(s => s.value == val)[0];
        this.input.salePointName = salePoint.displayText;
      } else {
        this.input.salePointName = "";
      }
    }
  },
  created() {
    this.staffReportUrl = "/Stat/TicketSales/StatCashierSale";
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    this.statTypes = [
      {
        value: 1,
        displayText: "按金额"
      },
      {
        value: 2,
        displayText: "按付款方式"
      }
    ];
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport(this.staffReportUrl);
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.clear();
    },
    exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.exportReport(this.staffReportUrl);
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择销售时间");
        return false;
      }

      return true;
    }
  }
};
</script>
