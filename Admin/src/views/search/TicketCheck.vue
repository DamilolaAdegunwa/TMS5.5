<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{ 'search-box-collapse': !showAdvanced }">
      <el-form ref="searchForm" :model="input" :rules="rules" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="起始时间" prop="startCheckTime">
              <shortcut-datetime-picker v-model="input.startCheckTime" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="截止时间" prop="endCheckTime">
              <shortcut-datetime-picker v-model="input.endCheckTime" default-time="23:59:59" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="项目">
              <el-select v-model="input.groundId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in grounds"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="检票点">
              <el-select v-model="input.gateGroupId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in gateGroups"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="检票通道">
              <el-select v-model="input.gateId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in gates"
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
        </el-row>
        <div class="button-box">
          <div>
            <span
              class="text"
              @click="showAdvanced = !showAdvanced"
            >{{ showAdvanced ? "隐藏" : "显示" }}高级过滤</span>
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
        <el-table-column fixed prop="cardNo" min-width="150" label="卡号"></el-table-column>
        <el-table-column prop="ctime" min-width="140" label="检票时间"></el-table-column>
        <el-table-column prop="parkName" min-width="100" :label="'检票' + '景点'"></el-table-column>
        <el-table-column prop="groundName" min-width="130" label="项目"></el-table-column>
        <el-table-column prop="gateGroupName" min-width="130" label="检票点"></el-table-column>
        <el-table-column prop="gateName" min-width="160" label="通道"></el-table-column>
        <el-table-column prop="ticketTypeName" min-width="180" label="票类"></el-table-column>
        <el-table-column prop="checkTypeName" min-width="100" label="检票类型"></el-table-column>
        <el-table-column prop="totalNum" min-width="60" label="总次数"></el-table-column>
        <el-table-column prop="surplusNum" min-width="80" label="剩余次数"></el-table-column>
        <el-table-column prop="checkNum" min-width="80" label="检票次数"></el-table-column>
        <el-table-column prop="inOutFlagName" min-width="80" label="出入标志"></el-table-column>
        <el-table-column prop="checkerName" min-width="80" label="检票员"></el-table-column>
        <el-table-column prop="glkOwnerName" min-width="80" label="放行人"></el-table-column>
        <el-table-column prop="fxCardNo" min-width="80" label="放行卡号"></el-table-column>
        <el-table-column prop="recycleFlagName" min-width="80" label="是否回收"></el-table-column>
        <el-table-column prop="listNo" min-width="150" label="单号"></el-table-column>
        <el-table-column prop="ticketCode" min-width="150" label="票号"></el-table-column>
        <el-table-column prop="stime" min-width="140" label="起始有效期"></el-table-column>
        <el-table-column prop="etime" min-width="140" label="最晚有效期"></el-table-column>
        <el-table-column prop="cashierName" min-width="80" label="收银员"></el-table-column>
        <el-table-column prop="cashPCName" min-width="100" label="收银机"></el-table-column>
        <el-table-column prop="salePointName" min-width="160" label="售票点"></el-table-column>
        <el-table-column prop="saleParkName" min-width="100" :label="'售票' + '景点'"></el-table-column>
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
import staffService from "./../../services/staffService.js";
import pcService from "./../../services/pcService.js";
import ticketService from "./../../services/ticketService.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import scenicService from "./../../services/scenicService.js";
import gateService from "./../../services/gateService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startCheckTime = startTime;
    this.endCheckTime = endTime;
    this.groundId = "";
    this.gateGroupId = "";
    this.gateId = "";
    this.listNo = "";
    this.ticketCode = "";
    this.cardNo = "";
    this.ticketTypeId = "";
    this.cashierId = "";
    this.cashpcId = "";
    this.parkId = "";
  }
}

export default {
  name: "SearchTicketCheck",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      grounds: [],
      gateGroups: [],
      gates: [],
      ticketTypes: [],
      cashiers: [],
      cashpcs: [],
      rules: {
        startCheckTime: [
          {
            required: true,
            message: "请选择起始时间",
            trigger: "change"
          }
        ],
        endCheckTime: [
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
    "input.groundId": function(val) {
      this.input.gateGroupId = "";
      this.getGateGroups(val);
    },
    "input.gateGroupId": function(val) {
      this.input.gateId = "";
      this.getGates(val);
    }
  },
  created() {
    scenicService.getGroundComboboxItemsAsync().then(data => {
      this.grounds = data;
    });
    this.getGateGroups(this.input.groundId);
    this.getGates(this.input.gateGroupId);
    ticketTypeService.getTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    pcService.getCashPcComboboxItemsAsync().then(data => {
      this.cashpcs = data;
    });
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    getGateGroups(groundId) {
      scenicService.getGateGroupComboboxItemsAsync(groundId).then(data => {
        this.gateGroups = data;
      });
    },
    getGates(gateGroupId) {
      gateService.getGateComboboxItemsAsync(gateGroupId).then(data => {
        this.gates = data;
      });
    },
    async getData(input) {
      return await ticketService.queryTicketChecksAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    }
  }
};
</script>
