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
            <el-form-item label="商品名称">
              <el-input v-model="input.wareName" clearable />
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
            <el-form-item label="商品类型">
              <el-select v-model="input.wareTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in wareTypes"
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
            <el-form-item label="商店">
              <el-select v-model="input.wareShopId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in wareShops"
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
        <el-table-column fixed prop="wareName" min-width="110" label="商品名称"></el-table-column>
        <el-table-column prop="listNo" min-width="140" label="单号"></el-table-column>
        <el-table-column prop="tradeTypeName" min-width="80" label="交易类型"></el-table-column>
        <el-table-column prop="costPrice" min-width="60" label="成本价"></el-table-column>
        <el-table-column prop="retailPrice" min-width="60" label="零售价"></el-table-column>
        <el-table-column prop="rentPrice" min-width="60" label="租金"></el-table-column>
        <el-table-column prop="yaJin" min-width="60" label="押金"></el-table-column>
        <el-table-column prop="ionum" min-width="80" label="出入数量"></el-table-column>
        <el-table-column prop="reaMoney" min-width="60" label="金额"></el-table-column>
        <el-table-column prop="discountTypeName" min-width="80" label="折扣类型"></el-table-column>
        <el-table-column prop="cashierName" min-width="60" label="收银员"></el-table-column>
        <el-table-column prop="cashPcname" min-width="100" label="收银机"></el-table-column>
        <el-table-column prop="wareShopName" min-width="80" label="商店"></el-table-column>
        <el-table-column prop="ctime" min-width="140" label="创建时间"></el-table-column>
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
    this.wareName = "";
    this.listNo = "";
    this.czkCardNo = "";
    this.wareTypeId = "";
    this.tradeTypeId = "";
    this.merchantId = "";
    this.wareShopId = "";
    this.cashierId = "";
    this.cashPcId = "";
  }
}

export default {
  name: "SearchWareIODetail",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      wareTypes: [],
      tradeTypes: [],
      merchants: [],
      wareShops: [],
      cashiers: [],
      cashPcs: [],
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
    this.wareTypes = await wareService.getWareTypeComboBoxItemsAsync();
    this.merchants = await wareService.getMerchantComboBoxItemsAsync();
    this.tradeTypes = await tradeService.getTradeTypeComboboxItemsAsync(6);
    this.wareShops = await wareService.getShopComboBoxItemsAsync();
    this.cashiers = await staffService.getCashierComboboxItemsAsync();
    this.cashPcs = await scenicService.getSalePointComboboxItemsAsync();
  },
  methods: {
    async getData(input) {
      const data = await wareService.queryWareIODetailAsync(input);
      return data;
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await wareService.queryWareIODetailToExcelAsync(this.input);
    }
  }
};
</script>
