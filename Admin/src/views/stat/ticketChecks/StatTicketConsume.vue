<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="110px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始检票时间">
              <shortcut-datetime-picker clearable v-model="input.startCheckTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止检票时间">
              <shortcut-datetime-picker
                clearable
                v-model="input.endCheckTime"
                default-time="23:59:59"
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始核销时间">
              <shortcut-datetime-picker clearable v-model="input.startConsumeTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止核销时间">
              <shortcut-datetime-picker
                clearable
                v-model="input.endConsumeTime"
                default-time="23:59:59"
              />
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
            <el-form-item label="客户">
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
            <el-form-item label="核销类型">
              <el-select v-model="input.consumeType" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in consumeTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="button-box col-xl">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportData" :loading="loading">导出</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData.rows" border stripe :max-height="tableHeight" v-loading="loading">
        <el-table-column prop="customerName" min-width="140" label="客户"></el-table-column>
        <el-table-column prop="ticketTypeName" min-width="180" label="票类"></el-table-column>
        <el-table-column prop="price" min-width="80" label="单价"></el-table-column>
        <el-table-column prop="checkNum" min-width="80" label="检票数量"></el-table-column>
        <el-table-column prop="checkMoney" min-width="80" label="检票金额"></el-table-column>
        <el-table-column prop="consumeNum" min-width="80" label="核销数量"></el-table-column>
        <el-table-column prop="consumeMoney" min-width="80" label="核销金额"></el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "./../../../mixins/statViewMixin.js";
import { startTime, endTime } from "./../../../utils/datetime.js";
import ticketService from "./../../../services/ticketService.js";
import ticketTypeService from "./../../../services/ticketTypeService.js";
import customerService from "./../../../services/customerService.js";

class StatInput {
  constructor() {
    this.startCheckTime = startTime;
    this.endCheckTime = endTime;
    this.startConsumeTime = "";
    this.endConsumeTime = "";
    this.ticketTypeId = "";
    this.customerId = "";
    this.consumeType = "";
  }
}

export default {
  name: "StatTicketConsume",
  mixins: [statViewMixin],
  data() {
    return {
      input: new StatInput(),
      ticketTypes: [],
      customers: [],
      consumeTypes: []
    };
  },
  created() {
    ticketTypeService.getTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
    customerService.getConsumeCustomerComboBoxItemsAsync().then(data => {
      this.customers = data;
    });
    ticketService.getConsumeTypeComboboxItemsAsync().then(data => {
      this.consumeTypes = data;
    });
  },
  methods: {
    async getData() {
      try {
        this.loading = true;
        const result = await ticketService.statTicketConsumeAsync(this.input);
        this.tableData.rows = result;
      } catch (error) {
        return;
      } finally {
        this.loading = false;
      }
    },
    reset() {
      this.input = new StatInput();
      this.clear();
    },
    async exportToExcel() {
      await ticketService.statTicketConsumeToExcelAsync(this.input);
    }
  }
};
</script>
