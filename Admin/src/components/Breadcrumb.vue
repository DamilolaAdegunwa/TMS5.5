<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item,index) in levelList" :key="item.path">
        <span
          v-if="item.redirect==='noredirect'||index==levelList.length-1"
          class="no-redirect"
        >{{ item.meta.title }}</span>
        <a v-else @click.prevent="handleLink(item)">{{ item.meta.title }}</a>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script>
import pathToRegexp from "path-to-regexp";
import routeJs from "./../router/router.js";
import { filterAuthorisedRoutes } from "./../router/permission.js";
import { staff } from "./../store/types.js";

export default {
  name: "breadcrumb",
  data() {
    return {
      levelList: [],
    };
  },
  computed: {
    permissions() {
      return JSON.parse(sessionStorage.getItem(staff.permissions));
    },
  },
  watch: {
    $route() {
      this.getBreadcrumb();
    },
  },
  created() {
    this.getBreadcrumb();
  },
  methods: {
    getBreadcrumb() {
      let matched = this.$route.matched.filter((item) => item.name);
      const first = matched[0];
      if (first && first.name.trim().toLocaleLowerCase() !== "home".toLocaleLowerCase()) {
        matched = [{ path: "/", meta: { title: "首页" } }].concat(matched);
      }
      this.levelList = matched;
    },
    handleLink(item) {
      const { redirect, path } = item;
      if (redirect) {
        this.$router.push(redirect);
        return;
      }

      let routes = routeJs.getRoutes();
      const route = routes.filter((r) => r.name === item.name)[0];
      if (route && route.children) {
        const routes = filterAuthorisedRoutes(route.children, this.permissions);
        if (routes && routes.length > 0) {
          this.$router.push(routes[0].path);
          return;
        }
        this.$router.push("/");
        return;
      }

      this.$router.push(this.pathCompile(path));
    },
    pathCompile(path) {
      const { params } = this.$route;
      var toPath = pathToRegexp.compile(path);
      return toPath(params);
    },
  },
};
</script>

<style lang="scss" scoped>
.app-breadcrumb.el-breadcrumb {
  display: inline-block;
  font-size: 14px;
  margin-left: 10px;
  .no-redirect {
    color: #97a8be;
    cursor: text;
  }
}
</style>
