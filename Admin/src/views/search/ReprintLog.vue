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
        </el-row>
        <div class="button-box">
          <div>
            <div v-if="showAdvancedPanel">
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
          </div>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData" border stripe :height="tableHeight" v-loading="loading">
        <el-table-column fixed prop="rowNum" min-width="60" label="序号"></el-table-column>
        <el-table-column fixed prop="ctime" min-width="140" label="操作时间"></el-table-column>
        <el-table-column prop="ticketId" min-width="100" label="票流水号"></el-table-column>
        <el-table-column prop="ticketTypeName" min-width="180" label="票类"></el-table-column>
        <el-table-column prop="ticketCode" min-width="150" label="票号"></el-table-column>
        <el-table-column prop="cardNo" min-width="150" label="卡号"></el-table-column>
        <el-table-column prop="cashierName" min-width="80" label="收银员"></el-table-column>
        <el-table-column prop="cashPcname" min-width="150" label="收银机"></el-table-column>
        <el-table-column prop="salePointName" min-width="160" label="售票点"></el-table-column>
        <el-table-column prop="parkName" min-width="90" :label="'景点'"></el-table-column>
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
import staffService from "./../../services/staffService.js";
import pcService from "./../../services/pcService.js";
import scenicService from "./../../services/scenicService.js";
import ticketService from "./../../services/ticketService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.startCTime = startTime;
    this.endCTime = endTime;
    this.ticketCode = "";
    this.cardNo = "";
    this.ticketTypeId = "";
    this.cashierId = "";
    this.cashpcId = "";
    this.salePointId = "";
  }
}

export default {
  name: "SearchReprintLog",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      ticketTypes: [],
      cashiers: [],
      cashpcs: [],
      salePoints: [],
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
      },
      pageLabelMainText: '景区'
    };
  },
  computed: {
    showAdvancedPanel() {
      return document.documentElement.clientWidth < 1200;
    }
  },
  created() {
    ticketTypeService.getTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });
    pcService.getCashPcComboboxItemsAsync().then(data => {
      this.cashpcs = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    async getData(input) {
      return await ticketService.queryReprintLogAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    }
  }
};
</script>
