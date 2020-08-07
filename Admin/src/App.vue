<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
import "normalize.css/normalize.css";
import "element-ui/lib/theme-chalk/index.css";
import "./styles/iconfont.scss";
import "./styles/index.scss";

export default {
  name: "app"
};
</script>
