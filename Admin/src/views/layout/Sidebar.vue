<template>
  <el-scrollbar>
    <el-menu
      :show-timeout="200"
      :default-active="$route.path"
      :collapse="isSidebarCollapse"
      mode="vertical"
      background-color="#304156"
      :router="true"
      unique-opened
      class="side-menu"
    >
      <div v-if="!isSidebarCollapse" class="logo">票务后台管理系统</div>
      <sidebar-item
        v-for="route in routes"
        :key="route.path"
        :route="route"
        :base-path="route.path"
      />
    </el-menu>
  </el-scrollbar>
</template>

<script>
import { mapState } from "vuex";
import SidebarItem from "./SidebarItem.vue";
import routeJs from "./../../router/router.js";
import { filterAuthorisedRoutes } from "./../../router/permission.js";
import { modules, app, staff } from "./../../store/types.js";
import scenicService from "./../../services/scenicService.js";

export default {
  name: "sidebar",
  components: { SidebarItem },
  data() {
    return {
      scenicName: "深圳市易高科技有限公司"
    };
  },
  computed: {
    routes() {
      return filterAuthorisedRoutes(routeJs.getRoutes(), this.permissions).filter(
        item => !item.hidden && item.children
      );
    },
    permissions() {
      return JSON.parse(sessionStorage.getItem(staff.permissions));
    },
    ...mapState(modules.app, [app.isSidebarCollapse])
  },
  created() {
    scenicService.getScenicOptions().then(data => {
      if (data && data.scenicName) {
        this.scenicName = data.scenicName;
      }
    });
  }
};
</script>

<style lang="scss">
@import "./../../styles/variables.scss";
$color: #bfcbd9 !important;
@mixin nest {
  background-color: #1f2d3d !important;
  color: $color;
  &:hover {
    background-color: #001528 !important;
    color: $color;
  }
}
@mixin active {
  background-color: $blue !important;
  color: #fff !important;

  .icon {
    color: #fff !important;
  }
}

.sidebar-container {
  /deep/ .el-scrollbar {
    height: 100%;
  }

  /deep/ .el-scrollbar__wrap {
    overflow-x: hidden;
  }
}

.side-menu {
  min-height: 100vh;

  &:not(.el-menu--collapse) {
    width: $sidebar-width;
  }

  .logo {
    height: $nav-height;
    line-height: $nav-height;
    border-bottom: 1px solid #002140;
    color: #fff !important;
    font-size: 16px;
    font-weight: 600;
    white-space: nowrap;
    padding-left: 20px;
  }
}

.side-menu,
.el-menu--popup {
  .el-submenu__title,
  .el-menu-item {
    display: flex;
    align-items: center;
    height: 44px;
    line-height: 44px;
    color: $color;

    &:hover {
      background-color: #263445 !important;
      color: $color;
    }
  }

  .icon {
    color: $color;
    margin-right: 5px;
    width: 24px;
    text-align: center;
    font-size: 18px;
    vertical-align: middle;
  }

  .is-nest {
    @include nest;
    .el-submenu__title {
      @include nest;
    }

    &.el-menu-item {
      height: 40px;
      line-height: 40px;
    }
  }

  .is-active {
    @include active;
    &:hover {
      @include active;
    }
  }
}

.el-menu--popup {
  padding: 0;
}
</style>
