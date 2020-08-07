<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" :rules="rules" label-width="110px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始检票时间">
              <shortcut-datetime-picker v-model="input.startCheckTime" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止检票时间">
              <shortcut-datetime-picker
                v-model="input.endCheckTime"
                clearable
                default-time="23:59:59"
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始核销时间">
              <shortcut-datetime-picker v-model="input.startConsumeTime" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止核销时间">
              <shortcut-datetime-picker
                v-model="input.endConsumeTime"
                clearable
                default-time="23:59:59"
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="单号">
              <el-input v-model="input.listNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="票号">
              <el-input v-model="input.ticketCode" clearable></el-input>
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
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="核销通知">
              <el-select v-model="input.hasNoticed" filterable clearable placeholder="请选择">
                <el-option label="已通知" :value="true"></el-option>
                <el-option label="未通知" :value="false"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始游玩日期">
              <shortcut-datetime-picker type="date" clearable v-model="input.startTravelDate" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止游玩日期">
              <shortcut-datetime-picker type="date" clearable v-model="input.endTravelDate" />
            </el-form-item>
          </el-col>
        </el-row>
        <div class="button-box">
          <div>
            <div>
              <span class="text" @click="showAdvanced = !showAdvanced"
                >{{ showAdvanced ? "隐藏" : "显示" }}高级过滤</span
              >
              <i
                class="arrow"
                :class="{ 'el-icon-arrow-up': showAdvanced, 'el-icon-arrow-down': !showAdvanced }"
              ></i>
            </div>
          </div>
          <div>
            <el-button type="primary" @click="query" :loading="loading">查询</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportData" :loading="loading">导出</el-button>
          </div>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData" border stripe :height="tableHeight" v-loading="loading">
        <el-table-column fixed prop="rowNum" min-width="60" label="序号"></el-table-column>
        <el-table-column
          fixed
          prop="consumeTime"
          min-width="140"
          label="检票时间"
        ></el-table-column>
        <el-table-column prop="ticketTypeName" min-width="180" label="票类"></el-table-column>
        <el-table-column prop="ticketCode" min-width="150" label="票号"></el-table-column>
        <el-table-column prop="price" min-width="80" label="单价"></el-table-column>
        <el-table-column prop="consumeNum" min-width="80" label="核销数量"></el-table-column>
        <el-table-column prop="consumeMoney" min-width="80" label="核销金额"></el-table-column>
        <el-table-column prop="totalNum" min-width="80" label="总数量"></el-table-column>
        <el-table-column prop="consumeTypeName" min-width="100" label="核销类型"></el-table-column>
        <el-table-column prop="customerName" min-width="170" label="客户"></el-table-column>
        <el-table-column prop="listNo" min-width="150" label="单号"></el-table-column>
        <el-table-column prop="thirdOrderId" min-width="190" label="第三方单号"></el-table-column>
        <el-table-column prop="lastNoticeTime" min-width="140" label="核销时间"></el-table-column>
      </el-table>
    </div>
    <div class="page-box">
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="pageSizes"
        :page-size="input.maxResultCount"
        layout="total, sizes, prev, pager, next, jumper"
        :total="totalResultCount"
      ></el-pagination>
    </div>
  </div>
</template>

<script>
import { pagedViewMixin, PagedInputDto } from "./../../mixins/pagedViewMixin.js";
import { startTime, endTime } from "./../../utils/datetime.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import customerService from "./../../services/customerService.js";
import ticketService from "./../../services/ticketService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startCheckTime = startTime;
    this.endCheckTime = endTime;
    this.startConsumeTime = "";
    this.endConsumeTime = "";
    this.startTravelDate = "";
    this.endTravelDate = "";
    this.listNo = "";
    this.ticketCode = "";
    this.ticketTypeId = "";
    this.customerId = "";
    this.consumeType = "";
    this.hasNoticed = "";
  }
}

export default {
  name: "SearchTicketConsume",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      ticketTypes: [],
      customers: [],
      consumeTypes: [],
      rules: {}
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
    async getData(input) {
      return await ticketService.queryTicketConsumesAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await ticketService.queryTicketConsumesToExcelAsync(this.input);
    }
  }
};
</script>
