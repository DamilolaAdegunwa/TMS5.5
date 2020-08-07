<template>
  <iframe :src="distributionUrl" class="iframe" @load="onLoad" />
</template>

<script>
import { mobileMixin } from "./../mixins/mobileMixin.js";
import settingService from "./../services/settingService.js";
import memberService from "./../services/memberService.js";
import weiXinJsSdkHelper from "./../utils/weiXinJsSdkHelper.js";

export default {
  mixins: [mobileMixin],
  data() {
    return {
      distributionUrl: ""
    };
  },
  async created() {
    this.loading();

    const options = await settingService.getOptionsAsync();
    if (options) {
      const member = memberService.getMember();
      this.distributionUrl = `${options.DistributionUrl}?openId=${member.openId}`;
    }
  },
  mounted() {
    window.onmessage = e => {
      weiXinJsSdkHelper.jsApiPay(e.data).then(() => {
        e.source.postMessage("success", "*");
      });
    };
  },
  beforeDestroy() {
    window.onmessage = null;
  },
  methods: {
    onLoad() {
      this.loaded();
    }
  }
};
</script>

<style lang="scss" scoped>
.iframe {
  border: none;
  width: 100%;
  min-height: 100vh;
  margin-bottom: -6px;
}
</style>
