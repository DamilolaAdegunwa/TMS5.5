<template>
  <div class="page">
    <nav-bar title="检票失败" back="/check" />

    <div class="content">
      <img src="../../assets/imgs/check-failed.png" />
      <h3>票号：{{ $route.params.ticketId }}</h3>
      <h1>{{ $route.params.errMsg }}</h1>
    </div>
    <van-button size="large" type="primary" @click="gotoCheck">返回</van-button>
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";

export default {
  name: "CheckFailed",
  components: { NavBar },
  data() {
    return {
      timer: -1
    };
  },
  created() {
    let second = 3;
    this.timer = setInterval(() => {
      second--;
      if (!second) {
        this.gotoCheck();
      }
    }, 1000);
  },
  beforeDestroy() {
    clearInterval(this.timer);
  },
  methods: {
    gotoCheck() {
      this.$router.push("/check");
    }
  }
};
</script>

<style lang="scss" scoped>
.content {
  margin: 12px;
  background: white;
  text-align: center;
  padding: 64px 0;

  img {
    width: 96px;
    height: 96px;
    margin-bottom: 24px;
  }

  h3 {
    color: #999999;
    font-weight: normal;
    margin-bottom: 0;
  }

  h1 {
    margin-top: 12px;
    font-weight: normal;
  }
}

.van-button {
  width: calc(100% - 24px);
  margin: 0 12px;
}
</style>
