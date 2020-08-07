<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" :rules="rules" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始时间" prop="sCTime">
              <shortcut-datetime-picker v-model="input.sCTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止时间" prop="eCTime">
              <shortcut-datetime-picker v-model="input.eCTime" default-time="23:59:59" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="单号">
              <el-input v-model="input.listNo" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="备注">
              <el-input v-model="input.memo" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="统计类型" prop="statTypeId">
              <el-select v-model="input.statTypeId" filterable clearable placeholder="请选择">
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
            <el-form-item label="交易大类型">
              <el-select v-model="input.tradeTypeTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in tradeTypeTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="交易类型">
              <el-select v-model="input.tradeTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in tradeTypes"
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
              <el-select v-model="input.cashPcId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in cashPcs"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="商家">
              <el-select v-model="input.merchantId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in merchants"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="商店类型">
              <el-select v-model="input.shopTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in shopTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="商店">
              <el-select v-model="input.shopId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in shops"
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
            <el-button type="primary" @click="getData" :loading="loading">查询</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </div>
        </div>
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
import { statViewMixin } from "@/mixins/statViewMixin.js";
import { startTime, endTime } from "./../../utils/datetime.js";
import wareService from "./../../services/wareService.js";
import tradeService from "./../../services/tradeService.js";
import scenicService from "./../../services/scenicService.js";
import staffService from "./../../services/staffService.js";

class QueryInput {
  constructor() {
    this.sCTime = startTime;
    this.eCTime = endTime;
    this.listNo = "";
    this.memo = "";
    this.tradeTypeTypeId = "";
    this.tradeTypeId = "";
    this.cashiersId = "";
    this.cashPcId = "";
    this.merchantId = "";
    this.shopTypeId = "";
    this.shopId = "";
    this.statTypeId = null;
  }
}

export default {
  name: "StatWareTrade",
  mixins: [statViewMixin],
  data() {
    return {
      input: new QueryInput(),
      tradeTypeTypes: [],
      tradeTypes: [],
      cashiers: [],
      cashPcs: [],
      merchants: [],
      shopTypes: [],
      shops: [],
      statTypes: [],
      rules: {
        sCTime: [
          {
            required: true,
            message: "请选择起始时间",
            trigger: "change"
          }
        ],
        eCTime: [
          {
            required: true,
            message: "请选择截止时间",
            trigger: "change"
          }
        ],
        statTypeId: [{ required: true, message: "请选择统计类型", trigger: "change" }]
      },
      showAdvanced: false
    };
  },
  async created() {
    this.tradeTypeTypes = await tradeService.getTradeTypeTypeComboboxItemsAsync();
    this.tradeTypeTypes = this.tradeTypeTypes.filter(t => t.displayText == "商品");
    this.input.tradeTypeTypeId = this.tradeTypeTypes[0].value;
    this.tradeTypes = await tradeService.getTradeTypeComboboxItemsAsync(this.input.tradeTypeTypeId);
    this.cashiers = await staffService.getCashierComboboxItemsAsync();
    this.cashPcs = await scenicService.getSalePointComboboxItemsAsync();
    this.merchants = await wareService.getMerchantComboBoxItemsAsync();
    this.shopTypes = await wareService.getShopTypeComboBoxItemsAsync();
    this.shops = await wareService.getShopComboBoxItemsAsync();
    this.statTypes = await wareService.getPayDetailStatTypeComboBoxItems();
    this.input.statTypeId = this.statTypes[0].value;
  },
  methods: {
    async getData() {
      await this.$refs.searchForm.validate();
      const result = await wareService.statWareTradeAsync(this.input);
      this.tableData.columns = result.columns;
      this.tableData.rows = result.data;
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
      this.input.statTypeId = this.statTypes[0].value;
    },
    async exportToExcel() {
      await this.$refs.searchForm.validate();
      await wareService.statWareTradeToExcelAsync(this.input);
    }
  }
};
</script>
