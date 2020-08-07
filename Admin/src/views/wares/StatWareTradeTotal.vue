<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="销售时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']" />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="商店">
              <el-select v-model="input.shopId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in wareShops"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select> </el-form-item
            >
          </el-col>
        </el-row>
        <div class="button-box">
          <div></div>
          <el-form-item label-width="20px">
            <el-button type="primary" @click="stat" :loading="loading">统计</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </el-form-item>
        </div>
      </el-form>
    </div>
    <iframe ref="iframe" class="iframe" :src="reportUrl" @load="onload"></iframe>
    <iframe class="hidden-iframe" :src="exportUrl" @load="onload"></iframe>
  </div>
</template>

<script>
import { reportViewMixin, ReportInputDto } from "@/mixins/reportViewMixin.js";
import wareService from "@/services/wareService.js";
import { startTime, endTime } from "@/utils/datetime.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.sCTime = startTime;
    this.eCTime = endTime;
    this.shopId = "";
    this.shopName = "";
  }
}

export default {
  name: "StatWareTradeTotal",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      wareShops: []
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.sCTime = val[0];
        this.input.eCTime = val[1];
      }
    },
    "input.shopId": function(val) {
      if (val) {
        const wareShop = this.wareShops.filter(s => s.value == val)[0];
        this.input.shopName = wareShop.displayText;
      } else {
        this.input.shopName = "";
      }
    }
  },
  async created() {
    this.wareShops = await wareService.getShopComboBoxItemsAsync();
  },
  methods: {
    async stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/Wares/StatWareTradeTotal");
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.clear();
    },
    async exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.exportReport("/Stat/Wares/StatWareTradeTotal");
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择统计时间");
        return false;
      }

      return true;
    }
  }
};
</script>
