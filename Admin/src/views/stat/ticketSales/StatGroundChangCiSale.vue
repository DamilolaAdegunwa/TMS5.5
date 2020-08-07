<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :md="10" :lg="7" :xl="4">
            <el-form-item label="游玩日期">
              <shortcut-datetime-picker v-model="input.travelDate" type="date" />
            </el-form-item>
          </el-col>
          <el-col :md="14" :lg="17" :xl="20" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button @click="exportToExcel" :loading="loading">导出</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table
        v-if="hasTableColumn"
        :data="tableData.rows"
        :max-height="tableHeight"
        border
        stripe
        v-loading="loading"
      >
        <el-table-column prop="groundName" min-width="90" label="项目" />
        <el-table-column prop="changCiName" min-width="90" label="场次" />
        <el-table-column prop="sTime" min-width="90" label="起始时间" />
        <el-table-column prop="eTime" min-width="90" label="截止时间" />
        <el-table-column prop="totalNum" min-width="90" label="总数量" />
        <el-table-column prop="saleNum" min-width="90" label="已售数量" :formatter="formatter" />
        <el-table-column prop="surplusNum" min-width="90" label="剩余数量" />
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "./../../../mixins/statViewMixin.js";
import ticketService from "./../../../services/ticketService.js";
import { startDate } from "./../../../utils/datetime.js";

class StatInput {
  constructor() {
    this.travelDate = startDate;
  }
}

export default {
  name: "StatGroundChangCiSale",
  mixins: [statViewMixin],
  data() {
    return {
      input: new StatInput()
    };
  },
  methods: {
    async getData() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        const result = await ticketService.statGroundChangCiSaleAsync(this.input);
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;
      });
    },
    reset() {
      this.input = new StatInput();
      this.clear();
    },
    async exportToExcel() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        await ticketService.statGroundChangCiSaleToExcelAsync(this.input);
      });
    },
    checkInput() {
      if (!this.input.travelDate) {
        this.$message.error("请选择游玩日期");
        return false;
      }

      return true;
    }
  }
};
</script>
