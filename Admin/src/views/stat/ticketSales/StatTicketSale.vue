<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" label-width="110px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始销售时间">
              <shortcut-datetime-picker v-model="input.startCTime" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止销售时间">
              <shortcut-datetime-picker
                v-model="input.endCTime"
                default-time="23:59:59"
                clearable
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="票大类">
              <el-select v-model="input.ticketTypeTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in ticketTypeTypes"
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
            <el-form-item label="起始游玩日期">
              <shortcut-datetime-picker v-model="input.startTravelDate" type="date" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止游玩日期">
              <shortcut-datetime-picker v-model="input.endTravelDate" type="date" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="票状态">
              <el-select v-model="input.ticketStatusId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in ticketStatus"
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
              <el-select v-model="input.statType" filterable placeholder="请选择">
                <el-option
                  v-for="item in statTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
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
            <el-form-item label="收银机">
              <el-select v-model="input.cashpcId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in cashpcs"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <div class="button-box">
          <div>
            <span class="text" @click="showAdvanced = !showAdvanced"
              >{{ showAdvanced ? "隐藏" : "显示" }}高级过滤</span
            >
            <i
              class="arrow"
              :class="{ 'el-icon-arrow-up': showAdvanced, 'el-icon-arrow-down': !showAdvanced }"
            ></i>
          </div>
          <div>
            <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </div>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table
        v-if="tableData.rows.length > 0"
        :data="tableData.rows"
        :max-height="tableHeight"
        border
        stripe
        v-loading="loading"
      >
        <el-table-column prop="statType" min-width="90" :label="statTypeName" />
        <el-table-column prop="saleNum" min-width="90" label="售票张数" />
        <el-table-column prop="salePersonNum" min-width="90" label="售票人数" />
        <el-table-column prop="saleMoney" min-width="90" label="售票金额" />
        <el-table-column prop="returnNum" min-width="90" label="退票张数" />
        <el-table-column prop="returnPersonNum" min-width="90" label="退票人数" />
        <el-table-column prop="returnMoney" min-width="90" label="退票金额" />
        <el-table-column prop="realNum" min-width="90" label="实售张数" />
        <el-table-column prop="realPersonNum" min-width="90" label="实售人数" />
        <el-table-column prop="realMoney" min-width="90" label="实售金额" />
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "@/mixins/statViewMixin.js";
import { startTime, endTime } from "@/utils/datetime.js";
import ticketService from "@/services/ticketService.js";
import ticketTypeService from "@/services/ticketTypeService.js";
import staffService from "@/services/staffService.js";
import pcService from "@/services/pcService.js";
import tradeService from "@/services/tradeService.js";
import scenicService from "@/services/scenicService.js";

class StatInput {
  constructor() {
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.startTravelDate = "";
    this.endTravelDate = "";
    this.ticketStatusId = "";
    this.ticketTypeTypeId = "";
    this.ticketTypeId = "";
    this.salePointId = "";
    this.cashierId = "";
    this.cashpcId = "";
    this.tradeSource = "";
    this.statType = 1;
  }
}

export default {
  name: "StatTicketSale",
  mixins: [statViewMixin],
  data() {
    return {
      input: new StatInput(),
      ticketStatus: [],
      ticketTypeTypes: [],
      ticketTypes: [],
      salePoints: [],
      cashiers: [],
      cashpcs: [],
      tradeSources: [],
      statTypes: [],
      statTypeName: ""
    };
  },
  watch: {
    "input.ticketTypeTypeId": function(val) {
      this.input.ticketTypeId = "";
      this.getTicketTypes(val);
    }
  },
  created() {
    ticketService.getTicketStatusComboboxItems().then(data => {
      this.ticketStatus = data;
    });
    ticketTypeService.getTicketTypeTypeComboboxItemsAsync().then(data => {
      this.ticketTypeTypes = data;
    });
    this.getTicketTypes(this.input.ticketTypeTypeId);
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    pcService.getCashPcComboboxItemsAsync().then(data => {
      this.cashpcs = data;
    });
    tradeService.getTradeSourceComboboxItems().then(data => {
      this.tradeSources = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    ticketService.getTicketSaleStatTypeComboboxItems().then(data => {
      this.statTypes = data;
    });
  },
  methods: {
    getStatTypeName() {
      const statType = this.statTypes.find(s => s.value == this.input.statType);
      if (statType) {
        return statType.displayText;
      }
      return "统计类型";
    },
    getTicketTypes(ticketTypeTypeId) {
      ticketTypeService.getTicketTypeComboboxItemsAsync(ticketTypeTypeId).then(data => {
        this.ticketTypes = data;
      });
    },
    async getData() {
      await this.submit(async () => {
        const items = await ticketService.statTicketSaleAsync(this.input);
        this.tableData.rows = items;
        this.statTypeName = this.getStatTypeName();
      });
    },
    reset() {
      this.input = new StatInput();
      this.clear();
    },
    async exportToExcel() {
      await this.submit(async () => {
        await ticketService.statTicketSaleToExcelAsync(this.input);
      });
    }
  }
};
</script>
