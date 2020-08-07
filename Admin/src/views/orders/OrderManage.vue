<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" label-width="110px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始下单时间">
              <shortcut-datetime-picker v-model="input.startCTime" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止下单时间">
              <shortcut-datetime-picker
                v-model="input.endCTime"
                default-time="23:59:59"
                clearable
              />
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
            <el-form-item label="单号">
              <el-input v-model="input.listNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="订单状态">
              <el-select v-model="input.orderStatus" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in orderStatus"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="消费状态">
              <el-select v-model="input.consumeStatus" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in consumeStatus"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="退款状态">
              <el-select v-model="input.refundStatus" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in refundStatus"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="订单类型">
              <el-select v-model="input.orderType" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in orderTypes"
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
            <el-form-item label="姓名">
              <el-input v-model="input.contactName" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="手机号码">
              <el-input v-model="input.contactMobile" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="证件号码">
              <el-input v-model="input.contactCertNo" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="备注">
              <el-input v-model="input.remark" clearable></el-input>
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
        <el-table-column fixed prop="listNo" min-width="140" label="单号"></el-table-column>
        <el-table-column prop="cTime" min-width="140" label="下单时间"></el-table-column>
        <el-table-column prop="totalMoney" min-width="90" label="总金额"></el-table-column>
        <el-table-column prop="totalNum" min-width="80" label="总数量"></el-table-column>
        <el-table-column prop="collectNum" min-width="80" label="已出票"></el-table-column>
        <el-table-column prop="returnNum" min-width="80" label="已退"></el-table-column>
        <el-table-column prop="usedNum" min-width="80" label="已用"></el-table-column>
        <el-table-column prop="surplusNum" min-width="80" label="未用"></el-table-column>
        <el-table-column prop="travelDate" min-width="100" label="游玩日期"></el-table-column>
        <el-table-column prop="ydrName" min-width="80" label="联系人"></el-table-column>
        <el-table-column prop="mobile" min-width="100" label="联系人手机"></el-table-column>
        <el-table-column prop="certNo" min-width="150" label="联系人证件" />
        <el-table-column prop="orderStatusName" min-width="80" label="订单状态"></el-table-column>
        <el-table-column prop="consumeStatusName" min-width="80" label="消费状态"></el-table-column>
        <el-table-column prop="refundStatusName" min-width="80" label="退款状态"></el-table-column>
        <el-table-column prop="orderTypeName" min-width="90" label="订单类型"></el-table-column>
        <el-table-column prop="customerName" min-width="140" label="客户"></el-table-column>
        <el-table-column prop="memberName" min-width="190" label="会员"></el-table-column>
        <el-table-column prop="guiderName" min-width="80" label="导游"></el-table-column>
        <el-table-column prop="promoterName" min-width="90" label="推广员"></el-table-column>
        <el-table-column min-width="80" label="是否付款">
          <template slot-scope="scope">
            <el-tag size="medium" v-if="scope.row.payFlag">是</el-tag>
            <el-tag size="medium" v-else type="danger">否</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="thirdListNo" min-width="200" label="第三方单号"></el-table-column>
        <el-table-column prop="memo" min-width="150" label="备注"></el-table-column>
        <el-table-column fixed="right" label="操作" width="50">
          <template slot-scope="scope">
            <el-button @click="onDetail(scope.row.listNo)" type="text">详情</el-button>
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
    <order-detail v-model="showDetail" :list-no="listNo" />
  </div>
</template>

<script>
import { pagedViewMixin, PagedInputDto } from "./../../mixins/pagedViewMixin.js";
import { startTime, endTime } from "./../../utils/datetime.js";
import customerService from "./../../services/customerService.js";
import promoterService from "./../../services/promoterService.js";
import orderService from "./../../services/orderService.js";
import OrderDetail from "./OrderDetail.vue";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.startTravelDate = "";
    this.endTravelDate = "";
    this.customerId = "";
    this.promoterId = "";
    this.listNo = "";
    this.orderStatus = "";
    this.consumeStatus = "";
    this.refundStatus = "";
    this.orderType = "";
    this.contactName = "";
    this.contactMobile = "";
    this.contactCertNo = "";
    this.remark = "";
  }
}

export default {
  name: "OrderManage",
  mixins: [pagedViewMixin],
  components: { OrderDetail },
  data() {
    return {
      input: new QueryInput(),
      customers: [],
      promoters: [],
      orderStatus: [],
      consumeStatus: [],
      refundStatus: [],
      orderTypes: [],
      showDetail: false,
      listNo: ""
    };
  },
  created() {
    customerService.getCustomerComboboxItemsAsync().then(data => {
      this.customers = data;
    });
    promoterService.getPromoterComboboxItemsAsync().then(items => {
      this.promoters = items;
    });
    orderService.getOrderStatusComboboxItems().then(data => {
      this.orderStatus = data;
    });
    orderService.getConsumeStatusComboboxItems().then(data => {
      this.consumeStatus = data;
    });
    orderService.getRefundStatusComboboxItems().then(data => {
      this.refundStatus = data;
    });
    orderService.getOrderTypeComboboxItems().then(data => {
      this.orderTypes = data;
    });
  },
  methods: {
    async getData(input) {
      return await orderService.getOrdersAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await orderService.getOrdersToExcelAsync(this.input);
    },
    onDetail(listNo) {
      this.listNo = listNo;
      this.showDetail = true;
    }
  }
};
</script>
