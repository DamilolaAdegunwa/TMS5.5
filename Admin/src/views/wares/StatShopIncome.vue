<template>
  <div class="main-content search-content">
    <div class="search-box" :class="{'search-box-collapse':!showAdvanced}">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="名称" prop="startSaleTime">
              <el-input v-model="input.name" clearable />
            </el-form-item>
          </el-col>
          <el-col :sm="sm" :md="md" :lg="lg" class="col-xl">
            <el-form-item label="助记符" prop="endSaleTime">
              <el-input v-model="input.zjf" clearable />
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
        </el-row>
        <div class="button-box">
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
        <el-table-column fixed prop="rowNum" min-width="60" label="序号"></el-table-column>
        <el-table-column fixed prop="name" min-width="190" label="商品名称"></el-table-column>
        <el-table-column prop="zjf" min-width="80" label="助记符"></el-table-column>
        <el-table-column prop="wareTypeName" min-width="100" label="商品类型"></el-table-column>
        <el-table-column prop="wareUnit" min-width="80" label="单位"></el-table-column>
        <el-table-column prop="costPrice" min-width="80" label="成本价"></el-table-column>
        <el-table-column prop="retailPrice" min-width="80" label="零售价"></el-table-column>
        <el-table-column prop="merchantName" min-width="170" label="商家"></el-table-column>
        <el-table-column prop="memo" min-width="160" label="备注"></el-table-column>
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
import wareService from "./../../services/wareService.js";
import ticketService from "./../../services/ticketService.js";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.name = "";
    this.zjf = "";
    this.wareTypeId = "";
    this.merchantId = "";
  }
}

export default {
  name: "StatShopIncome",
  mixins: [pagedViewMixin],
  data() {
    return {
      input: new QueryInput(),
      wareTypes: [],
      merchants: []
    };
  },
  async created() {
    this.wareTypes = await wareService.getWareTypeComboBoxItemsAsync();
    this.merchants = await wareService.getMerchantComboBoxItemsAsync();
  },
  methods: {
    async getData(input) {
      const data = await wareService.queryWareAsync(input);
      return data;
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async exportToExcel() {
      await ticketService.queryTicketSalesToExcelAsync(this.input);
    }
  }
};
</script>
