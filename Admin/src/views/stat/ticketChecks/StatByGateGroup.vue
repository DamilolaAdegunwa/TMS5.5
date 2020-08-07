<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="检票时间">
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
            <el-form-item label="检票点">
              <el-select v-model="input.gateGroupId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in gateGroups"
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
    this.gateGroupId = "";
    this.gateGroupName = "";
  }
}

export default {
  name: "StatTicketCheckByGateGroup",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      parks: [],
      gateGroups: [],
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
    "input.gateGroupId": function(val) {
      if (val) {
        const gateGroup = this.gateGroups.filter(s => s.value == val)[0];
        this.input.gateGroupName = gateGroup.displayText;
      } else {
        this.input.gateGroupName = "";
      }
    }
  },
  created() {
    scenicService.getParkComboboxItemsAsync().then(data => {
      this.parks = data;
    });
    scenicService.getGateGroupComboboxItemsAsync().then(data => {
      this.gateGroups = data;
    });
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/TicketChecks/StatTicketCheckByGateGroup");
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
        this.exportReport("/Stat/TicketChecks/StatTicketCheckByGateGroup");
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择检票时间");
        return false;
      }

      return true;
    }
  }
};
</script>
