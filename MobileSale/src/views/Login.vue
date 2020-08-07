<template>
  <div class="main"></div>
</template>

<script>
import combineURLs from "axios/lib/helpers/combineURLs.js";
import dayjs from "dayjs";
import { mapMutations } from "vuex";
import { mobileMixin } from "./../mixins/mobileMixin.js";
import memberService from "@/services/memberService.js";
import settingService from "@/services/settingService.js";

export default {
  mixins: [mobileMixin],
  async created() {
    try {
      this.loading();

      await memberService.loginFromWeChatAsync({
        code: this.$route.query.code,
        state: this.$route.query.state
      });

      await settingService.getSettingsFromWeChatAsync();

      if (this.isIphone()) {
        await settingService.configWxJsApi();
      }

      this.setPromoter();

      let path = "/";
      let query = {};
      const redirect = this.$route.query.redirect;
      if (redirect) {
        path = combineURLs(path, redirect);

        query = { ...this.$route.query };
        delete query.code;
        delete query.state;
        delete query.redirect;
      }

      this.$router.replace({
        path: path,
        query: query
      });
    } finally {
      this.loaded();
    }
  },
  methods: {
    setPromoter() {
      let promoterId = this.$route.query.promoterId;
      if (promoterId) {
        const promoter = {
          id: promoterId,
          expires: dayjs()
            .add(5, "minute")
            .toISOString()
        };
        localStorage.setItem("promoter", JSON.stringify(promoter));
      } else {
        let promoter = localStorage.getItem("promoter");
        if (promoter) {
          promoter = JSON.parse(promoter);
          if (dayjs(promoter.expires).isAfter(dayjs())) {
            promoterId = promoter.id;
          }
        }
      }
      this.setPromoterId(promoterId);
    },
    ...mapMutations(["setPromoterId"])
  }
};
</script>

<style lang="scss" scoped>
.main {
  height: 100vh;
}
</style>
