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
            <el-form-item label="商品大类">
              <el-select v-model="input.wareTypeTypeId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in wareTypeTypes"
                  :key="item.value"
                  :label="item.displayText"
                  :value="item.value"
                ></el-option>
              </el-select>
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
            <el-form-item label="供应商">
              <el-select v-model="input.supplierId" filterable clearable placeholder="请选择">
                <el-option
                  v-for="item in suppliers"
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
        <div :lg="{ span: 6, offset: 18 }" :xl="{ span: 5, offset: 0 }" class="button-box">
          <div v-if="isLg"></div>
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
import scenicService from "./../../services/scenicService.js";
import { startTime, endTime } from "@/utils/datetime.js";
import staffService from "./../../services/staffService.js";

class StatInput extends ReportInputDto {
  constructor() {
    super();
    this.sCTime = startTime;
    this.eCTime = endTime;
    this.wareName = "";
    this.listNo = "";
    this.wareTypeTypeId = "";
    this.wareTypeTypeName = "";
    this.wareTypeId = "";
    this.wareTypeName = "";
    this.merchantId = "";
    this.merchantName = "";
    this.shopTypeId = "";
    this.shopTypeName = "";
    this.wareShopId = "";
    this.wareShopName = "";
    this.supplierId = "";
    this.supplierName = "";
    this.cashierId = "";
    this.cashierName = "";
    this.cashPcId = "";
    this.cashPcName = "";
  }
}

export default {
  name: "StatWareRentSale",
  mixins: [reportViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      wareTypeTypes: [],
      wareTypes: [],
      merchants: [],
      shopTypes: [],
      wareShops: [],
      suppliers: [],
      cashiers: [],
      cashPcs: []
    };
  },
  watch: {
    queryTime(val) {
      if (val.length === 2) {
        this.input.sCTime = val[0];
        this.input.eCTime = val[1];
      }
    },
    "input.wareTypeTypeId": function(val) {
      if (val) {
        const wareTypeType = this.wareTypeTypes.filter(s => s.value == val)[0];
        this.input.wareTypeTypeName = wareTypeType.displayText;
      } else {
        this.input.wareTypeTypeName = "";
      }
    },
    "input.wareTypeId": function(val) {
      if (val) {
        const wareType = this.wareTypes.filter(s => s.value == val)[0];
        this.input.wareTypeName = wareType.displayText;
      } else {
        this.input.wareTypeName = "";
      }
    },
    "input.merchantId": function(val) {
      if (val) {
        const merchant = this.merchants.filter(s => s.value == val)[0];
        this.input.merchantName = merchant.displayText;
      } else {
        this.input.merchantName = "";
      }
    },
    "input.shopTypeId": function(val) {
      if (val) {
        const shopType = this.shopTypes.filter(s => s.value == val)[0];
        this.input.shopTypeName = shopType.displayText;
      } else {
        this.input.shopTypeName = "";
      }
    },
    "input.wareShopId": function(val) {
      if (val) {
        const wareShop = this.wareShops.filter(s => s.value == val)[0];
        this.input.wareShopName = wareShop.displayText;
      } else {
        this.input.wareShopName = "";
      }
    },
    "input.supplierId": function(val) {
      if (val) {
        const supplier = this.suppliers.filter(s => s.value == val)[0];
        this.input.supplierName = supplier.displayText;
      } else {
        this.input.supplierName = "";
      }
    },
    "input.cashierId": function(val) {
      if (val) {
        const cashier = this.cashiers.filter(s => s.value == val)[0];
        this.input.cashierName = cashier.displayText;
      } else {
        this.input.cashierName = "";
      }
    },
    "input.cashPcId": function(val) {
      if (val) {
        const cashPc = this.cashPcs.filter(s => s.value == val)[0];
        this.input.cashPcName = cashPc.displayText;
      } else {
        this.input.cashPcName = "";
      }
    }
  },
  async created() {
    this.wareTypeTypes = await wareService.getWareTypeTypeComboBoxItemsAsync();
    this.wareTypes = await wareService.getWareTypeComboBoxItemsAsync();
    this.merchants = await wareService.getMerchantComboBoxItemsAsync();
    this.shopTypes = await wareService.getShopTypeComboBoxItemsAsync();
    this.wareShops = await wareService.getShopComboBoxItemsAsync();
    this.suppliers = await wareService.getSupplierComboBoxItemsAsync();
    this.cashiers = await staffService.getCashierComboboxItemsAsync();
    this.cashPcs = await scenicService.getSalePointComboboxItemsAsync();
  },
  methods: {
    async stat() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.getReport("/Stat/Wares/StatWareRentSale");
      });
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.clear();
    },
    exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      this.submit(() => {
        this.exportReport("/Stat/Wares/StatWareRentSale");
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

<style lang="scss" scoped>
.button-box {
  display: flex;
  justify-content: right;
  flex-direction: row-reverse;
}
</style>
