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
            <el-form-item :label="'景点'">
              <el-select v-model="input.parkId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in parks"
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
            <el-form-item label="票类">
              <el-select v-model="input.ticketTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in ticketTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <div :lg="{ span: 6, offset: 18 }" :xl="{ span: 5, offset: 0 }" class="button-box">
          <div></div>
          <el-form-item label-width="20px">
            <el-button type="primary" @click="stat" :loading="loading">统计</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </el-form-item>
        </div>
      </el-form>
    </div>
    <iframe ref="iframe" class="iframe" :src="reportUrl" @load="onload"></iframe>
    <iframe class="hidden-iframe" :src="exportUrl" @load="onload"></iframe>
  </div>
</template>

<script>
import { reportViewMixin, ReportInputDto } from "./../../../mixins/reportViewMixin.js";
import ticketTypeService from "./../../../services/ticketTypeService.js";
import scenicService from "./../../../services/scenicService.js";
import staffService from "./../../../services/staffService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.parkId = "";
    this.parkName = "";
    this.salePointId = "";
    this.salePointName = "";
    this.ticketTypeId = "";
    this.ticketTypeName = "";
  }
}

export default {
  name: "StatTicketSaleBySalePoint",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      parks: [],
      salePoints: [],
      ticketTypes: [],
      pageLabelMainText: '景区'
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.startCTime = val[0];
        this.input.endCTime = val[1];
      }
    },
    "input.parkId": function(val) {
      if (val) {
        const park = this.parks.filter(s => s.value == val)[0];
        this.input.parkName = park.displayText;
      } else {
        this.input.parkName = "";
      }
    },
    "input.salePointId": function(val) {
      if (val) {
        const salePoint = this.salePoints.filter(s => s.value == val)[0];
        this.input.salePointName = salePoint.displayText;
      } else {
        this.input.salePointName = "";
      }
    },
    "input.ticketTypeId": function(val) {
      if (val) {
        const ticketType = this.ticketTypes.filter(s => s.value == val)[0];
        this.input.ticketTypeName = ticketType.displayText;
      } else {
        this.input.ticketTypeName = "";
      }
    }
  },
  created() {
    scenicService.getParkComboboxItemsAsync().then(data => {
      this.parks = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    ticketTypeService.getTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/TicketSales/StatTicketSaleBySalePoint");
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
        this.exportReport("/Stat/TicketSales/StatTicketSaleBySalePoint");
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
