<template>
  <router-view></router-view>
</template>

<script>
export default {
  name: "App"
};
</script>

<style lang="scss">
html,
body {
  height: 100vh;
  width: 100vw;
  font-family: "Source Han Sans Regular", sans-serif;
}

.page {
  background-attachment: fixed;
  background: #f5f5f5;
  min-height: 100%;
  width: 100%;
}

.van-cell {
  padding: 12px 16px;
  font-size: 16px;
}

@font-face {
  font-family: "DINOT-Medium";
  src: url("./assets/fonts/DINOT-Medium.otf");
}

@font-face {
  font-family: "Roboto";
  src: url("./assets/fonts/Roboto-Regular.ttf");
}

@font-face {
  font-family: "Source Han Sans Medium";
  src: url("./assets/fonts/SourceHanSansCN-Medium.otf");
}

@font-face {
  font-family: "Source Han Sans Regular";
  src: url("./assets/fonts/SourceHanSansCN-Regular.otf");
}

.font-dinot {
  font-family: "DINOT-Medium", sans-serif;
}

.font-roboto {
  font-family: "Roboto", sans-serif;
}

.font-source {
  font-family: "Source Han Sans Regular", sans-serif;
}

.font-source-medium {
  font-family: "Source Han Sans Medium", sans-serif;
}
</style>
