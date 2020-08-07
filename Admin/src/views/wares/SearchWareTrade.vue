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
            <el-form-item label="卡号">
              <el-input v-model="input.czkCardNo" clearable />
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
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="备注">
              <el-input v-model="input.memo" clearable />
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
            <el-button type="primary" @click="query" :loading="loading">查询</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportData" :loading="loading">导出</el-button>
          </div>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData" border stripe :height="tableHeight" v-loading="loading">
        <el-table-column fixed prop="rowNum" min-width="50" label="序号"></el-table-column>
        <el-table-column fixed prop="ctime" min-width="110" label="交易时间"></el-table-column>
        <el-table-column prop="czkCardNo" min-width="140" label="卡号"></el-table-column>
        <el-table-column prop="payMoney" min-width="80" label="总金额"></el-table-column>
        <el-table-column prop="tradeTypeName" min-width="60" label="交易类型"></el-table-column>
        <el-table-column prop="payTypeName" min-width="60" label="付款方式"></el-table-column>
        <el-table-column prop="customerName" min-width="60" label="客户名称"></el-table-column>
        <el-table-column prop="cashierName" min-width="60" label="收银员"></el-table-column>
        <el-table-column prop="cashPcname" min-width="100" label="收银机"></el-table-column>
        <el-table-column prop="shopName" min-width="80" label="商店"></el-table-column>
        <el-table-column prop="listNo" min-width="140" label="单号"></el-table-column>
        <el-table-column prop="memo" min-width="50" label="备注"></el-table-column>
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
import wareService from "./../../services/wareService.js";
import tradeService from "./../../services/tradeService.js";
import scenicService from "./../../services/scenicService.js";
import staffService from "./../../services/staffService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.sCTime = startTime;
    this.eCTime = endTime;
    this.listNo = "";
    this.czkCardNo = "";
    this.tradeTypeTypeId = "";
    this.tradeTypeId = "";
    this.cashiersId = "";
    this.cashPcId = "";
    this.merchantId = "";
    this.shopTypeId = "";
    this.shopId = "";
    this.memo = "";
  }
}

export default {
  name: "SearchWareTrade",
  mixins: [pagedViewMixin],
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
        ]
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
  },
  methods: {
    async getData(input) {
      const data = await wareService.queryWareTradeAsync(input);
      return data;
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
      this.input.tradeTypeTypeId = this.tradeTypeTypes[0].value;
    },
    async exportToExcel() {
      await wareService.queryWareTradeToExcelAsync(this.input);
    }
  }
};
</script>
