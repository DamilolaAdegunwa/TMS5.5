<template>
  <van-nav-bar @click-left="clickLeft" :title="title">
    <span slot="left" class="arrow">
      <font-awesome-icon icon="chevron-left" size="lg" />
    </span>
  </van-nav-bar>
</template>

<script>
export default {
  name: "NavBar",
  props: {
    title: {
      type: String,
      default: ""
    },
    back: {
      type: String,
      default: undefined
    }
  },
  methods: {
    clickLeft() {
      this.$router.push(this.back);
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../variables";

.arrow {
  color: $blue;
}
</style>
