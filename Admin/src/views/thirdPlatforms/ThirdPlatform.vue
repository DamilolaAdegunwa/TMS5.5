<template>
  <div class="main-content">
    <div class="search-box">
      <el-form ref="searchForm" :model="input" label-width="90px">
        <el-row type="flex">
          <el-col :md="13" :lg="9" :xl="6">
            <el-form-item label="用户名">
              <el-input v-model="input.uid" clearable></el-input>
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
        <el-table-column prop="name" min-width="150" label="名称"></el-table-column>
        <el-table-column prop="uid" min-width="150" label="用户名"></el-table-column>
        <el-table-column prop="orderCheckUrl" min-width="150" label="消费通知地址"></el-table-column>
        <el-table-column prop="platformTypeName" min-width="150" label="接口类型"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button @click="update(scope.row.id)" type="text">修改</el-button>
            <el-button @click="remove(scope.row.id)" type="text">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
    <edit-third-platform v-model="showEditDialog" :id="currentId" :is-add="isAdd" @saved="query"/>
  </div>
</template>

<script>
import { pagedAndEditViewMixin } from "./../../mixins/pagedAndEditViewMixin.js";
import thirdPlatformService from "./../../services/thirdPlatformService.js";
import EditThirdPlatform from "./EditThirdPlatform.vue";

class QueryInput {
  constructor() {
    this.uid = "";
  }
}

export default {
  name: "ThirdPlatform",
  mixins: [pagedAndEditViewMixin],
  components: { EditThirdPlatform },
  data() {
    return {
      input: new QueryInput()
    };
  },
  methods: {
    async getData(input) {
      const items = await thirdPlatformService.getThirdPlatformsAsync(input.uid);
      return { items: items, totalCount: items.length };
    },
    reset() {
      this.input = new QueryInput();
      this.clear();
    },
    async delete(id) {
      await thirdPlatformService.deleteAsync(id);
    }
  }
};
</script>
