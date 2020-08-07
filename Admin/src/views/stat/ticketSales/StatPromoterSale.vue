<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="销售时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="推广员">
              <el-select v-model="input.promoterId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in promoters"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="6" :xl="8" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="stat" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportToExcel" :loading="loading">导出</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <iframe ref="iframe" class="iframe" :src="reportUrl" @load="onload"></iframe>
    <iframe class="hidden-iframe" :src="exportUrl" @load="onload"></iframe>
  </div>
</template>

<script>
import { reportViewMixin, ReportInputDto } from "./../../../mixins/reportViewMixin.js";
import promoterService from "./../../../services/promoterService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.promoterId = "";
    this.promoterName = "";
  }
}

export default {
  name: "StatPromoterSale",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      promoters: []
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.startCTime = val[0];
        this.input.endCTime = val[1];
      }
    },
    "input.promoterId": function(val) {
      if (val) {
        const item = this.promoters.filter(s => s.value == val)[0];
        this.input.promoterName = item.displayText;
      } else {
        this.input.promoterName = "";
      }
    }
  },
  created() {
    promoterService.getPromoterComboboxItemsAsync().then(items => {
      this.promoters = items;
    });
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/TicketSales/StatPromoterSale");
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
        this.exportReport("/Stat/TicketSales/StatPromoterSale");
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
