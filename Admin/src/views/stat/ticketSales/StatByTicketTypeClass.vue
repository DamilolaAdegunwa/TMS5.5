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
            <el-form-item label="票种">
              <el-select
                v-model="input.ticketTypeClassId"
                filterable
                clearable
                placeholder="请选择"
              >
                <el-option
                  v-for="item in ticketTypeClasses"
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
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.ticketTypeClassId = "";
    this.ticketTypeClassName = "";
    this.ticketTypeId = "";
    this.ticketTypeName = "";
  }
}

export default {
  name: "StatTicketSaleByTicketTypeClass",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      ticketTypeClasses: [],
      ticketTypes: []
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.startCTime = val[0];
        this.input.endCTime = val[1];
      }
    },
    "input.ticketTypeClassId": function(val) {
      if (val) {
        const ticketTypeClass = this.ticketTypeClasses.filter(s => s.value == val)[0];
        this.input.ticketTypeClassName = ticketTypeClass.displayText;
      } else {
        this.input.ticketTypeClassName = "";
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
    ticketTypeService.getTicketTypeClassComboboxItemsAsync().then(data => {
      this.ticketTypeClasses = data;
    });
    ticketTypeService.getTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
  },
  methods: {
    stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/TicketSales/StatTicketSaleByTicketTypeClass");
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
        this.exportReport("/Stat/TicketSales/StatTicketSaleByTicketTypeClass");
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
