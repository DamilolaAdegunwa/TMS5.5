<template>
  <div class="app-wrapper">
    <div class="sidebar-container">
      <sidebar/>
    </div>
    <div class="main-container">
      <div class="header-container">
        <navbar/>
        <tags-view/>
      </div>
      <app-main/>
    </div>
  </div>
</template>

<script>
import Sidebar from "./Sidebar.vue";
import Navbar from "./Navbar.vue";
import TagsView from "./TagsView.vue";
import AppMain from "./AppMain.vue";

export default {
  name: "layout",
  components: { Sidebar, Navbar, TagsView, AppMain }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";

.app-wrapper {
  display: flex;
  height: 100vh;
}
.main-container {
  flex: 1;
  position: relative;
  width: calc(100vw - #{$sidebar-width + $scrollbar-width});
}
.header-container {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
}
</style>
