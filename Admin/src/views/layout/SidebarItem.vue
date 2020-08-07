<template>
  <el-menu-item
    v-if="hasOnlyOneShowingChild(route.children,route) && (!onlyOneChild.children||onlyOneChild.noShowingChildren)&&!route.alwaysShow"
    :index="resolvePath(onlyOneChild.path)"
    :class="{'is-nest':isNest}"
  >
    <i
      v-if="hasIcon(onlyOneChild)||hasIcon(route)"
      :class="onlyOneChild.meta.icon||route.meta.icon"
      class="icon"
    ></i>
    <span slot="title">{{onlyOneChild.meta.title}}</span>
  </el-menu-item>
  <el-submenu v-else-if="showingRoutes.length>0" :index="resolvePath(route.path)" :class="{'is-nest':isNest}">
    <template slot="title">
      <i v-if="hasIcon(route)" :class="route.meta.icon" class="icon"></i>
      <span slot="title">{{route.meta.title}}</span>
    </template>
    <sidebar-item
      v-for="child in showingRoutes"
      :key="child.path"
      :route="child"
      :is-nest="true"
      :base-path="route.path"
    />
  </el-submenu>
</template>

<script>
import path from "path";

export default {
  name: "sidebar-item",
  props: {
    route: {
      type: Object,
      required: true
    },
    isNest: {
      type: Boolean,
      default: false
    },
    basePath: {
      type: String,
      default: ""
    }
  },
  data() {
    return {
      onlyOneChild: null
    };
  },
  computed: {
    showingRoutes() {
      if (!this.route.children) {
        return [];
      }
      return this.route.children.filter(item => !item.hidden);
    }
  },
  methods: {
    hasOnlyOneShowingChild(children, parent) {
      if (!children) {
        this.onlyOneChild = parent;
        return true;
      }

      const showingChildren = children.filter(item => !item.hidden);

      if (showingChildren.length === 1) {
        this.onlyOneChild = showingChildren[0];
        return true;
      }

      if (showingChildren.length === 0) {
        this.onlyOneChild = { ...parent, path: "", noShowingChildren: true };
        return true;
      }

      return false;
    },
    hasIcon(route) {
      if (route && route.meta && route.meta.icon) {
        return true;
      }
      return false;
    },
    resolvePath(routePath) {
      return path.resolve(this.basePath, routePath);
    }
  }
};
</script>
