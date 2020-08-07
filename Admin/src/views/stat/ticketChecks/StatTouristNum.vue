<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="过馆时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']" />
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
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="通道">
              <el-select v-model="input.gateId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in gates"
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
import scenicService from "./../../../services/scenicService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.sSDate = startTime;
    this.eSDate = endTime;
    this.gateId = "";
    this.gateName = "";
    this.gateGroupId = "";
    this.gateGroupName = "";
  }
}

export default {
  name: "StatTouristNum",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      gates: [],
      gateGroups: []
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.sSDate = val[0];
        this.input.eSDate = val[1];
      }
    },
    "input.gateId": function(val) {
      if (val) {
        const gate = this.gates.filter(s => s.value == val)[0];
        this.input.gateName = gate.displayText;
      } else {
        this.input.gateName = "";
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
    scenicService.getGateComboBoxItemsAsync().then(data => {
      this.gates = data;
    });
    scenicService.getGateGroupComboboxItemsAsync().then(data => {
      this.gateGroups = data;
    });
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/TicketChecks/StatTouristNum");
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
        this.exportReport("/Stat/TicketChecks/StatTouristNum");
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择过馆时间");
        return false;
      }

      return true;
    }
  }
};
</script>
