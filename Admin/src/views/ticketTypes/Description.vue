<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :md="13" :lg="9" :xl="6">
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
          <el-col :md="11" :lg="15" :xl="18" class="button-box">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="query" :loading="loading">查询</el-button>
              <el-button @click="reset">重置</el-button>
              <el-button type="primary" @click="add">添加</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
    <div ref="tableBox" class="table-box">
      <el-table :data="tableData" border stripe :height="tableHeight" v-loading="loading">
        <el-table-column prop="ticketTypeName" min-width="180" label="票类"></el-table-column>
        <el-table-column prop="bookDescription" min-width="150" label="预订说明"></el-table-column>
        <el-table-column prop="feeDescription" min-width="150" label="费用说明"></el-table-column>
        <el-table-column prop="usageDescription" min-width="150" label="使用说明"></el-table-column>
        <el-table-column prop="refundDescription" min-width="150" label="退改说明"></el-table-column>
        <el-table-column prop="otherDescription" min-width="150" label="其他说明"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button @click="update(scope.row.ticketTypeId)" type="text">修改</el-button>
            <el-button @click="remove(scope.row.ticketTypeId)" type="text">删除</el-button>
          </template>
        </el-table-column>
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
    <edit-description
      v-model="showEditDialog"
      :ticket-type-id="currentId"
      :is-add="isAdd"
      @saved="query"
    />
  </div>
</template>

<script>
import { PagedInputDto } from "./../../mixins/pagedViewMixin.js";
import { pagedAndEditViewMixin } from "./../../mixins/pagedAndEditViewMixin.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import EditDescription from "./EditDescription.vue";

class QueryInput extends PagedInputDto {
  constructor() {
    super();
    this.ticketTypeId = "";
  }
}

export default {
  name: "TicketTypeDescription",
  mixins: [pagedAndEditViewMixin],
  components: { EditDescription },
  data() {
    return {
      input: new QueryInput(),
      ticketTypes: []
    };
  },
  created() {
    ticketTypeService.getNetSaleTicketTypeComboboxItemsAsync().then(data => {
      this.ticketTypes = data;
    });
  },
  methods: {
    async getData(input) {
      return await ticketTypeService.getTicketTypeDescriptionsAsync(input);
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async delete(id) {
      await ticketTypeService.deleteDescriptionAsync(id);
    }
  }
};
</script>
