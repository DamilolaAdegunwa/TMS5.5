<template>
  <div class="memberCard">
    <van-panel
      :title="memberCard.ticketTypeName"
      :status="etime"
      class="memberCard-card margin-bottom-10"
    >
      <div style="text-align: center;">
        <img v-if="!memberCard.isExpired && memberCard.qrcode" :src="memberCard.qrcode" />
      </div>
      <div v-if="memberCard.isExpired" slot="footer" style="text-align: right;">
        <van-button size="small" :loading="loading" @click="renew">延期</van-button>
      </div>
    </van-panel>

    <van-panel title="个人资料">
      <van-cell-group :border="false">
        <van-cell title="姓名" :value="memberCard.memberName" class="van-field" />
        <van-cell title="手机号码" :value="memberCard.mobile" class="van-field" />
        <van-cell title="身份证" :value="memberCard.idCard" class="van-field" />
        <van-cell title="学历" :value="memberCard.education" class="van-field" />
        <van-cell title="性别" :value="memberCard.sex" class="van-field" />
        <van-cell title="民族" :value="memberCard.nation" class="van-field" />
        <van-cell title="住址" :value="memberCard.address" class="van-field" />
      </van-cell-group>
    </van-panel>
  </div>
</template>

<script>
import QRCode from "qrcode";
import dayjs from "dayjs";
import memberService from "@/services/memberService.js";

export default {
  data() {
    return {
      loading: false,
      memberCard: {
        ticketTypeName: "",
        etime: "",
        qrcode: ""
      }
    };
  },
  computed: {
    etime() {
      if (this.memberCard.etime) {
        return "有效期至：" + this.memberCard.etime;
      }
      return "";
    }
  },
  methods: {
    async generateQRCode(text) {
      try {
        return await QRCode.toDataURL(text, {
          width: 200
        });
      } catch (err) {
        this.$toast(err);
      }
    },
    async renew() {
      try {
        this.loading = true;
        let etime = dayjs()
          .addDays(this.memberCard.days - 1)
          .toDateString();
        await this.$dialog.confirm({ title: "延期至" + etime + "?" });
        let response = await memberService.renewMemberCardAsync(this.memberCard.id);
        if (response.success) {
          this.$toast("延期成功");
          await this.getMemberCard();
        }
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async getMemberCard() {
      let memberCard = await memberService.getElectronicMemberCardAsync();
      if (!memberCard.isExpired) {
        memberCard.qrcode = await this.generateQRCode(memberCard.ticketCode);
      }
      this.memberCard = memberCard;
    }
  },
  async created() {
    await this.getMemberCard();
  }
};
</script>

<style lang="scss">
.memberCard {
  &-card {
    .van-cell__value {
      color: rgb(51, 51, 51);
    }
  }
}
</style>
