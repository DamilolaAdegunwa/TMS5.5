<template>
  <div class="tabs">
    <div :class="{ active: active === 0 }" @click="gotoSale">
      <font-awesome-icon icon="yen-sign" />
      <span>售票</span>
    </div>
    <div :class="{ active: active === 1 }" @click="gotoSaleDrawback">
      <font-awesome-icon icon="undo-alt" />
      <span>退票</span>
    </div>
    <div :class="{ active: active === 2 }" @click="gotoSaleVerify">
      <font-awesome-icon icon="search" />
      <span>查票</span>
    </div>
  </div>
</template>

<script>
export default {
  name: "SaleTabs",
  props: {
    active: {
      type: Number,
      default: 0
    }
  },
  methods: {
    gotoSale() {
      this.$router.push("/sale");
    },
    gotoSaleDrawback() {
      let temp = sessionStorage.getItem("permissions");
      let permissions = JSON.parse(temp);
      if (
        permissions.includes(
          "726CB969-620D-4B3A-96E1-2AE04C5FB3AA".toLocaleLowerCase()
        )
      ) {
        this.$router.push("/sale/drawback");
      } else {
        this.$toast("没有访问权限");
      }
    },
    gotoSaleVerify() {
      this.$router.push("/sale/verify");
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
