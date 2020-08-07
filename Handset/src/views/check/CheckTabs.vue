<template>
  <div class="tabs">
    <div :class="{ active: active === 0 }" @click="gotoCheck">
      <font-awesome-icon icon="clipboard-check" />
      <span>检票</span>
    </div>
    <div :class="{ active: active === 1 }" @click="gotoCheckConsume">
      <font-awesome-icon icon="shopping-cart" />
      <span>自由消费</span>
    </div>
  </div>
</template>

<script>
export default {
  name: "CheckTabs",
  props: {
    active: {
      type: Number,
      default: 0
    }
  },
  methods: {
    gotoCheck() {
      this.$router.push("/check");
    },
    gotoCheckConsume() {
      this.$toast("此功能未开放");
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../variables";

.tabs {
  $height: 36px;

  margin: 12px;
  display: flex;
  border: 1px solid $blue;
  border-radius: 3px;
  background: white;
  color: $blue;
  font-weight: normal;
  height: $height;
  line-height: $height - 2px;

  div {
    display: block;
    width: 50%;
    text-align: center;
    border-right: 1px solid $blue;

    span {
      margin-left: 8px;
    }

    &:last-of-type {
      border-right: none;
    }
  }

  div.active {
    background: $blue;
    color: white;
  }
}
</style>
