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
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label-width="20px">
              <el-radio-group v-model="input.statType">
                <el-radio v-for="(item, index) in statTypes" :key="index" :label="item.value">{{
                  item.displayText
                }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <div :lg="{ span: 6, offset: 18 }" :xl="{ span: 5, offset: 0 }" class="button-box">
          <div></div>
          <el-form-item label-width="20px">
            <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
            <el-button @click="reset">重置</el-button>
            <el-button @click="exportToExcel" :loading="loading">导出</el-button>
          </el-form-item>
        </div>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table
        v-if="hasTableColumn"
        :data="tableData.rows"
        border
        stripe
        :max-height="tableHeight"
        v-loading="loading"
      >
        <el-table-column
          v-for="column in tableData.columns"
          :key="column"
          :prop="column"
          :label="column"
          :formatter="formatter"
          :min-width="90"
        ></el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { statViewMixin } from "./../../../mixins/statViewMixin.js";
import tradeService from "./../../../services/tradeService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput {
  constructor() {
    this.startCTime = "";
    this.endCTime = "";
    this.statType = 1;
  }
}

export default {
  name: "StatPayDetail",
  mixins: [statViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput(),
      statTypes: []
    };
  },
  created() {
    tradeService.getPayDetailStatTypeComboboxItems().then(data => {
      this.statTypes = data;
    });
  },
  methods: {
    async getData() {
      if (!this.checkInput()) {
        return;
      }

      await this.submit(async () => {
        const result = await tradeService.statPayDetailAsync(this.input);
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;
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

      await this.submit(async () => {
        await tradeService.statPayDetailToExcelAsync(this.input);
      });
    },
    checkInput() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择销售时间");
        return false;
      }

      this.input.startCTime = this.queryTime[0];
      this.input.endCTime = this.queryTime[1];

      return true;
    }
  }
};
</script>
