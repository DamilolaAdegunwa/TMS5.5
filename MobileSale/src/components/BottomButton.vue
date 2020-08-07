<template>
  <div>
    <div style="height:50px"></div>
    <div class="bottom-button">
      <div class="bottom-button-bar">
        <van-button :type="type" :loading="loading" @click="onClick">{{ text }}</van-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "bottom-button",
  props: {
    text: {
      type: String
    },
    loading: {
      type: Boolean
    },
    type: {
      type: String,
      default: "primary"
    }
  },
  methods: {
    onClick() {
      this.$emit("click");
    }
  }
};
</script>

<style lang="scss" scoped>
.bottom-button {
  left: 0;
  bottom: 0;
  width: 100%;
  z-index: 100;
  position: fixed;
  user-select: none;

  &-bar {
    height: 50px;
    display: flex;
    font-size: 16px;
    align-items: center;
  }

  .van-button {
    width: 100%;
    height: 100%;
    border-radius: 0;
    font-size: 16px;

    &--disabled {
      border: none;
    }
  }
}
</style>
