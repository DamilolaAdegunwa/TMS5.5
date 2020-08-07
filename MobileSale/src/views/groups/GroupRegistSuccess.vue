<template>
  <div class="regist-success" :style="{ height: contentHeight + 'px' }">
    <van-row class="regist-success-icon" type="flex" justify="center" align="center">
      <van-col>
        <van-icon name="certificate" />
        <div class="text">注册成功</div>
      </van-col>
    </van-row>

    <van-row class="regist-success-tip" type="flex" justify="center" align="center">
      <van-col>
        <span>请等待馆方审核，信息审核通过，才能自助下单。</span>
      </van-col>
    </van-row>

    <div class="regist-success-button">
      <van-button type="primary" size="large" @click="onClick">关闭</van-button>
    </div>
  </div>
</template>

<script>
export default {
  computed: {
    contentHeight() {
      return document.documentElement.clientHeight - 46;
    }
  },
  methods: {
    onClick() {
      this.$router.push("/my");
    }
  }
};
</script>

<style lang="scss">
.regist-success {
  background-color: white;

  &-icon {
    height: 150px;
    text-align: center;
    color: #19a0f0;
    font-size: 50px;
    padding-top: 20px;

    .text {
      font-size: 30px;
      font-weight: bold;
    }
  }

  &-tip {
    height: 100px;
    font-size: 14px;
  }

  &-button {
    margin-top: 50px;
    padding: 0 20px;
  }
}
</style>
