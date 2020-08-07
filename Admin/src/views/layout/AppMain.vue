<template>
  <section class="app-main">
    <transition name="fade-transform" mode="out-in">
      <keep-alive :include="cachedViews">
        <router-view :key="key"/>
      </keep-alive>
    </transition>
  </section>
</template>

<script>
import { mapState } from "vuex";
import { modules, tagsView } from "./../../store/types.js";

export default {
  name: "app-main",
  computed: {
    key() {
      return this.$route.fullPath;
    },
    ...mapState(modules.tagsView, [tagsView.cachedViews])
  }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";
.app-main {
  height: calc(100vh - #{$header-height});
  margin-top: $header-height;
  width: 100%;
  //background: #f0f2f5;
  position: relative;
  overflow-y: auto;
}
</style>
