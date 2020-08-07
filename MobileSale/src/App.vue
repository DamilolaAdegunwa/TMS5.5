<template>
  <div id="app">
    <div v-if="showNavbar">
      <van-nav-bar
        :title="title"
        :left-text="leftText"
        :right-text="rightText"
        :z-index="1000"
        :left-arrow="showNavbarLeft"
        fixed
        :class="{ 'van-nav-bar-blue': navbarColor === 'blue' }"
        @click-left="onClickLeft"
        @click-right="onClickRight"
      />
      <div style="height:46px;"></div>
    </div>

    <keep-alive include="order">
      <router-view />
    </keep-alive>

    <div v-if="showTabbar">
      <van-tabbar v-model="tabbarActiveIndex">
        <van-tabbar-item icon="wap-home" to="/">首页</van-tabbar-item>
        <van-tabbar-item icon="orders-o" to="/order">订单</van-tabbar-item>
        <van-tabbar-item icon="contact" to="/my">我的</van-tabbar-item>
      </van-tabbar>
      <div style="height:50px;"></div>
    </div>
  </div>
</template>

<script>
import "@/utils/dayjs.js";
import { mapState } from "vuex";
import appConsts from "./store/consts.js";

export default {
  data() {
    return {};
  },
  computed: {
    tabbarActiveIndex: {
      get() {
        return this.$store.state.tabbarActiveIndex;
      },
      set(value) {
        this.$store.commit(appConsts.activeTabbar, value);
      }
    },
    leftText() {
      return this.showNavbarLeft ? "返回" : "";
    },
    rightText() {
      return this.showNavbarRight ? "首页" : "";
    },
    ...mapState([
      "title",
      "showNavbar",
      "showNavbarLeft",
      "showNavbarRight",
      "navbarColor",
      "showTabbar"
    ])
  },
  methods: {
    onClickLeft() {
      if (this.showNavbarLeft) {
        this.$router.go(-1);
      }
    },
    onClickRight() {
      if (this.showNavbarRight) {
        this.$router.push("/");
      }
    }
  },
  mounted() {
    this.$store.commit("setClientHeight", document.documentElement.clientHeight);
  }
};
</script>
