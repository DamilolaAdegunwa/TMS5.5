<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="付款时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']"/>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="客户名称">
              <el-select
                v-model="input.customerId"
                filterable
                clearable
                placeholder="请选择"
              >
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
            <el-form-item label="购买类型">
              <el-select
                v-model="input.tradeSource"
                filterable
                clearable
                placeholder="请选择"
              >
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
            <el-form-item label="付款方式">
              <el-select
                v-model="input.payTypeId"
                filterable
                clearable
                placeholder="请选择"
              >
                <el-option
                  v-for="item in payTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="8" :xl="4" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="getData" :loading="loading"
                >统计</el-button
              >
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportToExcel" :loading="loading"
                >导出</el-button
              >
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table
        v-if="hasTableColumn"
        :data="tableData.rows"
        border
        stripe
        :max-height="tableHeight"
        v-loading="loading"
      >
        <el-table-column
          v-for="column in tableData.columns"
          :key="column"
          :prop="column"
          :label="column"
          :formatter="formatter"
          :min-width="90"
        ></el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "./../../../mixins/statViewMixin.js";
import customerService from "./../../../services/customerService.js";
import tradeService from "./../../../services/tradeService.js";
import payTypeService from "./../../../services/payTypeService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput {
  constructor() {
    this.startCTime = "";
    this.endCTime = "";
    this.customerId = "";
    this.tradeSource = "";
    this.payTypeId = "";
  }
}

export default {
  name: "StatPayDetailByCustomer",
  mixins: [statViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      customers: [],
      tradeSources: [],
      payTypes: []
    };
  },
  created() {
    customerService.getCustomerComboboxItemsAsync().then(customers => {
      this.customers = customers;
    });
    tradeService.getTradeSourceComboboxItems().then(tradeSources => {
      this.tradeSources = tradeSources;
    });
    payTypeService.getPayTypeComboboxItemsAsync().then(paytypes => {
      this.payTypes = paytypes;
    });
  },
  methods: {
    async getData() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        const result = await tradeService.statPayDetailByCustomerAsync(this.input);
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.clear();
    },
    async exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        await tradeService.statPayDetailByCustomerToExcelAsync(this.input);
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择付款时间");
        return false;
      }

      this.input.startCTime = this.queryTime[0];
      this.input.endCTime = this.queryTime[1];

      return true;
    }
  }
};
</script>
