<template>
  <div class="error">
    <van-row type="flex" justify="center" align="center" class="error-main">
      <van-col>
        <van-row class="error-icon" type="flex" justify="center" align="center">
          <van-col>
            <van-icon name="fail" />
          </van-col>
        </van-row>

        <van-row class="error-tip" type="flex" justify="center" align="center">
          <van-col>
            <span>{{ text }}</span>
          </van-col>
        </van-row>

        <van-row v-if="retry" class="error-button" type="flex" justify="center" align="center">
          <van-col>
            <van-button size="small" @click="onClick">重试</van-button>
          </van-col>
        </van-row>
      </van-col>
    </van-row>
  </div>
</template>

<script>
export default {
  props: {
    text: {
      type: String,
      default: "操作失败，请稍后重试。"
    },
    retry: {
      type: Boolean,
      default: true
    }
  },
  methods: {
    onClick() {
      this.$router.go(-1);
    }
  }
};
</script>

<style lang="scss" scoped>
.error {
  background-color: white;

  &-main {
    height: 100vh;
  }

  &-icon {
    text-align: center;
    color: #19a0f0;
    font-size: 50px;
  }

  &-tip {
    height: 50px;
    font-size: 14px;
  }

  &-button {
    padding: 0 20px;
  }
}
</style>
