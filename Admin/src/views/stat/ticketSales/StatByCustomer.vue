<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="销售时间">
              <datetime-range
                v-model="queryTime"
                clearable
                :default-time="['00:00:00', '23:59:59']"
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="客户名称">
              <el-select v-model="input.customerId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in customers"
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
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="购买类型">
              <el-select v-model="input.tradeSource" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in tradeSources"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="游玩日期">
              <datetime-range v-model="travelDate" clearable type="daterange" />
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
import customerService from "./../../../services/customerService.js";
import tradeService from "./../../../services/tradeService.js";
import ticketTypeService from "./../../../services/ticketTypeService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.startTravelDate = startTime;
    this.endTravelDate = endTime;
    this.customerId = "";
    this.tradeSource = "";
    this.ticketTypeId = "";
  }
}

export default {
  name: "StatTicketSaleByCustomer",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      travelDate: [startTime, endTime],
      input: new StatInput(),
      customers: [],
      tradeSources: [],
      ticketTypes: []
    };
  },
  computed: {
    isQueryTimeEmpty() {
      return !this.queryTime || this.queryTime.length != 2;
    },
    isTravelDateEmpty() {
      return !this.travelDate || this.travelDate.length != 2;
    }
  },
  watch: {
    queryTime(val) {
      if (val && val.length === 2) {
        this.input.startCTime = val[0];
        this.input.endCTime = val[1];
      } else {
        this.input.startCTime = "";
        this.input.endCTime = "";
      }
    },
    travelDate(val) {
      if (val && val.length === 2) {
        this.input.startTravelDate = val[0];
        this.input.endTravelDate = val[1];
      } else {
        this.input.startTravelDate = "";
        this.input.endTravelDate = "";
      }
    }
  },
  created() {
    customerService.getCustomerComboboxItemsAsync().then(customers => {
      this.customers = customers;
    });
    tradeService.getTradeSourceComboboxItems().then(data => {
      this.tradeSources = data;
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
        this.getReport("/Stat/TicketSales/StatTicketSaleByCustomer");
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.travelDate = [startTime, endTime];
      this.clear();
    },
    exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.exportReport("/Stat/TicketSales/StatTicketSaleByCustomer");
      });
    },
    checkInput() {
      if (this.isQueryTimeEmpty && this.isTravelDateEmpty) {
        this.$message.error("请选择销售时间或游玩日期");
        return false;
      }

      return true;
    }
  }
};
</script>
