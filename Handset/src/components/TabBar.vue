<template>
  <van-tabbar v-model="active1">
    <van-tabbar-item @click="gotoHome">
      <font-awesome-icon slot="icon" icon="home" />首页
    </van-tabbar-item>
    <van-tabbar-item @click="gotoAccount">
      <font-awesome-icon slot="icon" icon="user" />我的
    </van-tabbar-item>
  </van-tabbar>
</template>

<script>
export default {
  name: "TabBar",
  props: {
    active: {
      type: Number,
      default: 0
    }
  },
  data() {
    return {
      active1: this.$props.active
    };
  },
  methods: {
    gotoHome() {
      this.$router.push("/home");
    },
    gotoAccount() {
      this.$router.push("/account");
    }
  }
};
</script>

<style scoped></style>
