<template>
  <div class="main-content search-content">
    <div class="search-box">
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
            <el-form-item label="已交班">
              <el-select v-model="input.hasShift" filterable clearable placeholder="请选择">
                <el-option label="是" :value="true"></el-option>
                <el-option label="否" :value="false"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item>
              <el-checkbox v-model="input.statTicketByPayType">门票销售按付款方式</el-checkbox>
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item>
              <el-checkbox v-model="input.includeWareDetail">包含商品明细</el-checkbox>
            </el-form-item>
          </el-col>
          <el-col
            :sm="sm"
            :md="md"
            :lg="{ span: lg, offset: 18 }"
            :xl="{ span: 5, offset: 0 }"
            class="button-box"
          >
            <div v-if="isLg"></div>
            <el-form-item label-width="20px">
              <el-button type="primary" @click="stat" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportToExcel" :loading="loading">导出</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <iframe ref="iframe" class="iframe" :src="reportUrl" @load="onload"></iframe>
    <iframe class="hidden-iframe" :src="exportUrl" @load="onload"></iframe>
  </div>
</template>

<script>
import moment from "moment";
import { reportViewMixin, ReportInputDto } from "./../../../mixins/reportViewMixin.js";
import staffService from "./../../../services/staffService.js";
import scenicService from "./../../../services/scenicService.js";
import { startTime, endTime, datetimeFormat } from "./../../../utils/datetime.js";

let startCTime = startTime;
let endCTime = endTime;

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.startCTime = startCTime;
    this.endCTime = endCTime;
    this.parkId = "";
    this.parkName = "";
    this.salePointId = "";
    this.salePointName = "";
    this.cashierId = "";
    this.cashierName = "";
    this.hasShift = "";
    this.statTicketByPayType = false;
    this.includeWareDetail = false;
  }
}

export default {
  name: "StatShift",
  mixins: [reportViewMixin],
  data() {
    return {
      input: new StatInput(),
      parks: [],
      salePoints: [],
      cashiers: [],
      sm: 12,
      md: 8,
      lg: 6,
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
  watch: {
    "input.parkId": function(val) {
      if (val) {
        const park = this.parks.filter(s => s.value == val)[0];
        this.input.parkName = park.displayText;
      } else {
        this.input.parkName = "";
      }
    },
    "input.salePointId": function(val) {
      if (val) {
        const salePoint = this.salePoints.filter(s => s.value == val)[0];
        this.input.salePointName = salePoint.displayText;
      } else {
        this.input.salePointName = "";
      }
    },
    "input.cashierId": function(val) {
      if (val) {
        const cashier = this.cashiers.filter(s => s.value == val)[0];
        this.input.cashierName = cashier.displayText;
      } else {
        this.input.cashierName = "";
      }
    }
  },
  async created() {
    scenicService.getParkComboboxItemsAsync().then(data => {
      this.parks = data;
    });
    scenicService.getSalePointComboboxItemsAsync().then(data => {
      this.salePoints = data;
    });
    staffService.getCashierComboboxItemsAsync().then(data => {
      this.cashiers = data;
    });

    const scenicOptions = await scenicService.getScenicOptions();
    startCTime = moment(startTime)
      .add(scenicOptions.tradeTimeOffset, "minutes")
      .format(datetimeFormat);
    endCTime = moment(endTime)
      .add(scenicOptions.tradeTimeOffset, "minutes")
      .format(datetimeFormat);
    this.input.startCTime = startCTime;
    this.input.endCTime = endCTime;
    this.pageLabelMainText = scenicService.getPageLabelMainText();
  },
  methods: {
    stat() {
      this.submit(() => {
        this.getReport("/Stat/Shift/StatShift");
      });
    },
    reset() {
      this.input = new StatInput();
      this.clear();
    },
    exportToExcel() {
      this.submit(() => {
        this.exportReport("/Stat/Shift/StatShift");
      });
    }
  }
};
</script>
