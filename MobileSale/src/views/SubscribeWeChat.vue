<template>
  <div class="container">
    <div v-if="subscribeQRCode" class="qrcode">
      <img :src="subscribeQRCode" />
      <div class="tips">长按二维码关注公众号</div>
    </div>
  </div>
</template>

<script>
import qrcodeHelper from "./../utils/qrcodeHelper.js";

export default {
  data() {
    return {
      subscribeQRCode: ""
    };
  },
  async created() {
    this.subscribeQRCode = await qrcodeHelper.createQRCodeAsync(this.$route.params.subscribeUrl);
  }
};
</script>

<style lang="scss" scoped>
.container {
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 1fr;
  justify-items: center;
  min-height: calc(100vh - 46px);
}
.qrcode {
  margin-top: 30%;
}
.tips {
  text-align: center;
}
</style>
