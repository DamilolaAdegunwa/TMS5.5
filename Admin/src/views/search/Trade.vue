<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" :rules="rules" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始时间" prop="startSaleTime">
              <shortcut-datetime-picker v-model="input.startSaleTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止时间" prop="endSaleTime">
              <shortcut-datetime-picker v-model="input.endSaleTime" default-time="23:59:59" />
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
            <el-form-item label="单号">
              <el-input v-model="input.listNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="付款方式">
              <el-select v-model="input.payTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in payTypes"
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
            <el-form-item label="会员">
              <el-select v-model="input.memberId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in members"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item :label="'景点'">
              <el-select v-model="input.parkId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in parks"
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
            <el-form-item label="推广员">
              <el-select v-model="input.promoterId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in promoters"
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
          </div>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData" border stripe :height="tableHeight" v-loading="loading">
        <el-table-column fixed prop="rowNum" min-width="60" label="序号"></el-table-column>
        <el-table-column fixed prop="listNo" min-width="150" label="单号"></el-table-column>
        <el-table-column prop="ctime" min-width="140" label="售票时间"></el-table-column>
        <el-table-column prop="tradeTypeName" min-width="100" label="交易类型"></el-table-column>
        <el-table-column prop="payTypeName" min-width="100" label="付款方式"></el-table-column>
        <el-table-column prop="totalMoney" min-width="100" label="总金额"></el-table-column>
        <el-table-column prop="customerName" min-width="140" label="客户"></el-table-column>
        <el-table-column prop="memberName" min-width="100" label="会员"></el-table-column>
        <el-table-column prop="guiderName" min-width="80" label="导游"></el-table-column>
        <el-table-column prop="promoterName" min-width="90" label="推广员"></el-table-column>
        <el-table-column prop="cashierName" min-width="80" label="收银员"></el-table-column>
        <el-table-column prop="cashPCName" min-width="130" label="收银机"></el-table-column>
        <el-table-column prop="salePointName" min-width="160" label="售票点"></el-table-column>
        <el-table-column prop="parkName" min-width="100" :label="'景点'"></el-table-column>
        <el-table-column prop="memo" min-width="100" label="备注"></el-table-column>
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
import customerService from "./../../services/customerService.js";
import memberService from "./../../services/memberService.js";
import promoterService from "./../../services/promoterService.js";
import staffService from "./../../services/staffService.js";
import pcService from "./../../services/pcService.js";
import tradeService from "./../../services/tradeService.js";
import payTypeService from "./../../services/payTypeService.js";
import scenicService from "./../../services/scenicService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startSaleTime = startTime;
    this.endSaleTime = endTime;
    this.tradeTypeTypeId = "";
    this.tradeTypeId = "";
    this.listNo = "";
    this.thirdPartyPlatformOrderId = "";
    this.tradeSource = "";
    this.memberId = "";
    this.customerId = "";
    this.guiderId = "";
    this.promoterId = "";
    this.payTypeId = "";
    this.parkId = "";
    this.salePointId = "";
    this.cashierId = "";
    this.cashpcId = "";
    this.salePointId = "";
    this.parkId = "";
  }
}

export default {
  name: "SearchTrade",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      tradeTypeTypes: [],
      tradeTypes: [],
      customers: [],
      members: [],
      promoters: [],
      parks: [],
      salePoints: [],
      cashiers: [],
      cashpcs: [],
      tradeSources: [],
      payTypes: [],
      rules: {
        startSaleTime: [
          {
            required: true,
            message: "请选择起始时间",
            trigger: "change"
          }
        ],
        endSaleTime: [
          {
            required: true,
            message: "请选择截止时间",
            trigger: "change"
          }
        ]
      },
      pageLabelMainText: '景区'
    };
  },
  watch: {
    "input.tradeTypeTypeId": function(val) {
      this.input.tradeTypeId = "";
      this.getTradeTypes(val);
    }
  },
  created() {
    tradeService.getTradeTypeTypeComboboxItemsAsync().then(data => {
      this.tradeTypeTypes = data;
    });
    this.getTradeTypes(this.input.tradeTypeTypeId);
    customerService.getCustomerComboboxItemsAsync().then(data => {
      this.customers = data;
    });
    promoterService.getPromoterComboboxItemsAsync().then(items => {
      this.promoters = items;
    });
    memberService.getMemberComboboxItemsAsync().then(data => {
      this.members = data;
    });
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    pcService.getCashPcComboboxItemsAsync().then(data => {
      this.cashpcs = data;
    });
    tradeService.getTradeSourceComboboxItems().then(data => {
      this.tradeSources = data;
    });
    payTypeService.getPayTypeComboboxItemsAsync().then(data => {
      this.payTypes = data;
    });
    scenicService.getParkComboboxItemsAsync().then(data => {
      this.parks = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    getTradeTypes(tradeTypeTypeId) {
      tradeService.getTradeTypeComboboxItemsAsync(tradeTypeTypeId).then(data => {
        this.tradeTypes = data;
      });
    },
    async getData(input) {
      return await tradeService.queryTradesAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    }
  }
};
</script>
