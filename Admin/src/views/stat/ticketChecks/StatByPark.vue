<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg * 2" class="col-2xl">
            <el-form-item label="检票时间">
              <datetime-range v-model="queryTime" :default-time="['00:00:00', '23:59:59']" />
            </el-form-item>
          </el-col>
          <el-col :md="11" :lg="15" :xl="18" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="getData" :loading="loading">统计</el-button>
              <el-button @click="reset">重置</el-button>
            </el-form-item>
          </el-col>
        </el-row>
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
import ticketService from "./../../../services/ticketService.js";
import { startTime, endTime } from "./../../../utils/datetime.js";

class StatInput {
  constructor() {
    this.startCTime = "";
    this.endCTime = "";
  }
}

export default {
  name: "StatTicketCheckByPark",
  mixins: [statViewMixin],
  data() {
    return {
      queryTime: [startTime, endTime],
      input: new StatInput()
    };
  },
  methods: {
    async getData() {
      if (!this.queryTime || this.queryTime.length != 2) {
        this.$message.error("请选择检票时间");
        return;
      }

      this.input.startCTime = this.queryTime[0];
      this.input.endCTime = this.queryTime[1];

      try {
        this.loading = true;
        const result = await ticketService.statTicketCheckInByDateAndParkAsync(this.input);
        this.tableData.columns = result.columns;
        this.tableData.rows = result.data;
      } catch (error) {
        return;
      } finally {
        this.loading = false;
      }
    },
    reset() {
      this.input = new StatInput();
      this.queryTime = [startTime, endTime];
      this.clear();
    }
  }
};
</script>
