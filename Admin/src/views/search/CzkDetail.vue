<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" :rules="rules" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始时间" prop="startCTime">
              <shortcut-datetime-picker v-model="input.startCTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止时间" prop="endCTime">
              <shortcut-datetime-picker v-model="input.endCTime" default-time="23:59:59" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="操作类型">
              <el-select v-model="input.czkOpTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in czkOpTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="充值类型">
              <el-select
                v-model="input.czkRechargeTypeId"
                filterable
                clearable
                placeholder="请选择"
              >
                <el-option
                  v-for="item in czkRechargeTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="充值套餐">
              <el-select v-model="input.czkCztcId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in czkCztcs"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="消费类型">
              <el-select v-model="input.czkConsumeTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in czkConsumeTypes"
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
        <el-table-column prop="ctime" min-width="140" label="交易时间" />
        <el-table-column prop="czkOpTypeName" min-width="80" label="操作类型" />
        <el-table-column prop="ticketCode" min-width="150" label="票号" />
        <el-table-column prop="cardNo" min-width="150" label="卡号" />
        <el-table-column prop="ticketTypeName" min-width="180" label="卡类" />
        <el-table-column prop="czkRechargeTypeName" min-width="100" label="充值类型" />
        <el-table-column prop="czkCztcName" min-width="100" label="充值套餐" />
        <el-table-column prop="czkConsumeTypeName" min-width="100" label="消费类型" />
        <el-table-column prop="oldCardMoney" min-width="90" label="原本金" />
        <el-table-column prop="oldFreeMoney" min-width="90" label="原赠送金" />
        <el-table-column prop="oldGameMoney" min-width="90" label="原体验金" />
        <el-table-column prop="oldTotalMoney" min-width="90" label="原总金额" />
        <el-table-column prop="rechargeCardMoney" min-width="90" label="充值本金" />
        <el-table-column prop="rechargeFreeMoney" min-width="90" label="充值赠送金" />
        <el-table-column prop="rechargeGameMoney" min-width="90" label="充值体验金" />
        <el-table-column prop="rechargeTotalMoney" min-width="90" label="充值总金额" />
        <el-table-column prop="useCouponNum" min-width="90" label="优惠券面额" />
        <el-table-column prop="consumeCardMoney" min-width="90" label="消费本金" />
        <el-table-column prop="consumeFreeMoney" min-width="90" label="消费赠送金" />
        <el-table-column prop="consumeGameMoney" min-width="90" label="消费体验金" />
        <el-table-column prop="consumeTotalMoney" min-width="90" label="消费总金额" />
        <el-table-column prop="newCardMoney" min-width="90" label="新本金" />
        <el-table-column prop="newFreeMoney" min-width="90" label="新赠送金" />
        <el-table-column prop="newGameMoney" min-width="90" label="新体验金" />
        <el-table-column prop="newTotalMoney" min-width="90" label="新总金额" />
        <el-table-column prop="yaJin" min-width="90" label="押金" />
        <el-table-column prop="memberName" min-width="80" label="会员" />
        <el-table-column prop="payTypeName" min-width="100" label="付款方式" />
        <el-table-column prop="cashierName" min-width="120" label="收银" />
        <el-table-column prop="memo" min-width="200" label="备注" />
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
import memberService from "./../../services/memberService.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import staffService from "./../../services/staffService.js";
import payTypeService from "./../../services/payTypeService.js";
import valueCardService from "./../../services/valueCardService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.czkOpTypeId = "";
    this.czkRechargeTypeId = "";
    this.czkCztcId = "";
    this.czkConsumeTypeId = "";
    this.ticketTypeId = "";
    this.cashierId = "";
    this.memberId = "";
    this.ticketCode = "";
    this.cardNo = "";
    this.payTypeId = "";
  }
}

export default {
  name: "QueryCzkDetail",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      czkOpTypes: [],
      czkRechargeTypes: [],
      czkCztcs: [],
      czkConsumeTypes: [],
      ticketTypes: [],
      cashiers: [],
      members: [],
      payTypes: [],
      rules: {
        startCTime: [
          {
            required: true,
            message: "请选择起始时间",
            trigger: "change"
          }
        ],
        endCTime: [
          {
            required: true,
            message: "请选择截止时间",
            trigger: "change"
          }
        ]
      }
    };
  },
  created() {
    valueCardService.getCzkOpTypeComboboxItems().then(items => {
      this.czkOpTypes = items;
    });
    valueCardService.getCzkRechargeTypeComboboxItems().then(items => {
      this.czkRechargeTypes = items;
    });
    valueCardService.getCzkCztcComboboxItemsAsync().then(items => {
      this.czkCztcs = items;
    });
    valueCardService.getCzkConsumeTypeComboboxItems().then(items => {
      this.czkConsumeTypes = items;
    });
    ticketTypeService.getTicketTypeComboboxItemsAsync(8).then(data => {
      this.ticketTypes = data;
    });
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    memberService.getMemberComboboxItemsAsync().then(data => {
      this.members = data;
    });
    payTypeService.getPayTypeComboboxItemsAsync().then(data => {
      this.payTypes = data;
    });
  },
  methods: {
    async getData(input) {
      return await valueCardService.queryCzkDetailsAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await valueCardService.queryCzkDetailsToExcelAsync(this.input);
    }
  }
};
</script>
