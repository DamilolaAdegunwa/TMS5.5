<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" label-width="110px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始销售时间">
              <shortcut-datetime-picker v-model="input.startSaleTime" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止销售时间">
              <shortcut-datetime-picker
                v-model="input.endSaleTime"
                default-time="23:59:59"
                clearable
              />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="票号">
              <el-input v-model="input.ticketCode" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="卡号">
              <el-input v-model="input.cardNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="单号">
              <el-input v-model="input.listNo" clearable></el-input>
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
            <el-form-item label="已过期">
              <el-select v-model="input.isExpired" filterable clearable placeholder="请选择">
                <el-option label="是" :value="true"></el-option>
                <el-option label="否" :value="false"></el-option>
              </el-select>
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
            <el-form-item label="订单单号">
              <el-input v-model="input.orderListNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="第三方单号">
              <el-input v-model="input.thirdListNo" clearable></el-input>
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
            <el-form-item label="业务员">
              <el-select v-model="input.salesManId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in salesMans"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="手机号码">
              <el-input v-model="input.mobile" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="证件号码">
              <el-input v-model="input.certNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="指纹状态">
              <el-select v-model="input.hasFingerprint" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in bindStatus"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="人像状态">
              <el-select v-model="input.hasFaceImage" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in bindStatus"
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
        <el-table-column fixed prop="rowNum" min-width="60" label="序号" />
        <el-table-column fixed prop="listNo" min-width="150" label="单号" />
        <el-table-column prop="cTime" min-width="140" label="售票时间" />
        <el-table-column prop="ticketTypeName" min-width="180" label="票类" />
        <el-table-column prop="ticketCode" min-width="150" label="票号" />
        <el-table-column prop="cardNo" min-width="150" label="卡号" />
        <el-table-column prop="ticketStatusName" min-width="50" label="状态" />
        <el-table-column prop="validFlagName" min-width="80" label="是否有效" />
        <el-table-column prop="realPrice" min-width="60" label="单价" />
        <el-table-column prop="personNum" min-width="60" label="人数" />
        <el-table-column prop="realMoney" min-width="80" label="实售金额" />
        <el-table-column prop="payTypeName" min-width="100" label="付款方式" />
        <el-table-column prop="tradeSourceName" min-width="80" label="购买类型" />
        <el-table-column prop="certTypeName" min-width="90" label="证件类型" />
        <el-table-column prop="certNo" min-width="150" label="证件号码" />
        <el-table-column prop="fingerprintNum" min-width="100" label="指纹登记数量" />
        <el-table-column prop="unBindFingerprintNum" min-width="100" label="指纹未登数量" />
        <el-table-column prop="photoBindFlagName" min-width="100" label="人像登记状态" />
        <el-table-column prop="photoBindTime" min-width="140" label="人像登记时间" />
        <el-table-column prop="customerName" min-width="140" label="客户" />
        <el-table-column prop="memberName" min-width="190" label="会员" />
        <el-table-column prop="guiderName" min-width="80" label="导游" />
        <el-table-column prop="promoterName" min-width="90" label="推广员" />
        <el-table-column prop="contactName" min-width="80" label="联系人" />
        <el-table-column prop="contactMobile" min-width="100" label="联系人手机" />
        <el-table-column prop="contactCertNo" min-width="150" label="联系人证件" />
        <el-table-column prop="cashierName" min-width="80" label="收银员" />
        <el-table-column prop="cashPCName" min-width="150" label="收银机" />
        <el-table-column prop="salePointName" min-width="160" label="售票点" />
        <el-table-column prop="parkName" min-width="160" :label="`售票${'景点'}`" />
        <el-table-column prop="totalNum" min-width="60" label="总次数" />
        <el-table-column prop="ticPrice" min-width="80" label="原始单价" />
        <el-table-column prop="ticMoney" min-width="80" label="原始金额" />
        <el-table-column prop="discountTypeName" min-width="80" label="折扣类型" />
        <el-table-column prop="discountRate" min-width="60" label="折扣率" />
        <el-table-column prop="sTime" min-width="140" label="起始有效期" />
        <el-table-column prop="eTime" min-width="140" label="最晚有效期" />
        <el-table-column prop="returnTypeName" min-width="80" label="退票类型" />
        <el-table-column prop="returnRate" min-width="90" label="退票折扣率" />
        <el-table-column prop="orderListNo" min-width="150" label="订单单号" />
        <el-table-column prop="thirdPartyPlatformOrderID" min-width="200" label="第三方单号" />
        <el-table-column prop="memo" min-width="200" label="备注" />
        <el-table-column min-width="100" label="票流水号">
          <template slot-scope="scope">{{ scope.row.id == 0 ? "" : scope.row.id }}</template>
        </el-table-column>
        <el-table-column fixed="right" label="操作" width="50">
          <template slot-scope="scope">
            <el-button
              v-if="scope.row.id && scope.row.ticketStatusName != '已退'"
              @click="onDetail(scope.row.id)"
              type="text"
              >详情</el-button
            >
          </template>
        </el-table-column>
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
    <ticket-sale-detail v-model="showDetail" :ticket-id="ticketId" />
  </div>
</template>

<script>
import { pagedViewMixin, PagedInputDto } from "./../../mixins/pagedViewMixin.js";
import { startTime, endTime } from "./../../utils/datetime.js";
import ticketService from "./../../services/ticketService.js";
import customerService from "./../../services/customerService.js";
import memberService from "./../../services/memberService.js";
import promoterService from "./../../services/promoterService.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import staffService from "./../../services/staffService.js";
import pcService from "./../../services/pcService.js";
import tradeService from "./../../services/tradeService.js";
import payTypeService from "./../../services/payTypeService.js";
import scenicService from "./../../services/scenicService.js";
import TicketSaleDetail from "./TicketSaleDetail.vue";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startSaleTime = startTime;
    this.endSaleTime = endTime;
    this.startTravelDate = "";
    this.endTravelDate = "";
    this.ticketCode = "";
    this.cardNo = "";
    this.listNo = "";
    this.ticketStatusId = "";
    this.isExpired = "";
    this.ticketTypeTypeId = "";
    this.ticketTypeId = "";
    this.payTypeId = "";
    this.customerId = "";
    this.memberId = "";
    this.promoterId = "";
    this.parkId = "";
    this.salePointId = "";
    this.cashierId = "";
    this.cashpcId = "";
    this.orderListNo = "";
    this.thirdListNo = "";
    this.tradeSource = "";
    this.salesManId = "";
    this.mobile = "";
    this.certNo = "";
    this.hasFingerprint = "";
    this.hasFaceImage = "";
  }
}

export default {
  name: "SearchTicketSale",
  mixins: [pagedViewMixin],
  components: { TicketSaleDetail },
  data() {
    return {
      input: new QueryInput(),
      ticketStatus: [],
      ticketTypeTypes: [],
      ticketTypes: [],
      payTypes: [],
      customers: [],
      members: [],
      promoters: [],
      parks: [],
      salePoints: [],
      cashiers: [],
      cashpcs: [],
      tradeSources: [],
      salesMans: [],
      bindStatus: [{ value: false, displayText: "未登记" }, { value: true, displayText: "已登记" }],
      showDetail: false,
      ticketId: -1
    };
  },
  computed: {
    pageLabelMainText() {
      return scenicService.getPageLabelMainText();
    }
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
    customerService.getCustomerComboboxItemsAsync().then(data => {
      this.customers = data;
    });
    memberService.getMemberComboboxItemsAsync().then(data => {
      this.members = data;
    });
    promoterService.getPromoterComboboxItemsAsync().then(items => {
      this.promoters = items;
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
    staffService.getSalesManComboboxItemsAsync().then(date => {
      this.salesMans = date;
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
  },
  methods: {
    getTicketTypes(ticketTypeTypeId) {
      ticketTypeService.getTicketTypeComboboxItemsAsync(ticketTypeTypeId).then(data => {
        this.ticketTypes = data;
      });
    },
    async getData(input) {
      return await ticketService.queryTicketSalesAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await ticketService.queryTicketSalesToExcelAsync(this.input);
    },
    onDetail(ticketId) {
      this.ticketId = ticketId;
      this.showDetail = true;
    }
  }
};
</script>
