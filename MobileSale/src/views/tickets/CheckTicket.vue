<template>
  <div v-if="pageLoaded">
    <van-cell-group>
      <picker-field
        v-model="input.groundId"
        label="检票区域"
        placeholder="选择检票区域"
        :columns="grounds"
        isCell
      />
      <picker-field
        v-model="input.gateGroupId"
        label="检票点"
        placeholder="选择检票点"
        :columns="gateGroups"
        isCell
      />
    </van-cell-group>
    <div class="content">
      <section v-show="isValidTicket === true" class="section">
        <img :src="successImage" />
        <van-row class="row">
          <van-col span="8" class="label">票类：</van-col>
          <van-col span="16" class="value">{{ checkResult.ticketTypeName }}</van-col>
        </van-row>
        <van-row class="row">
          <van-col span="8" class="label">票号：</van-col>
          <van-col span="16" class="value">{{ checkResult.cardNo }}</van-col>
        </van-row>
        <van-row class="row">
          <van-col span="8" class="label">剩余数量：</van-col>
          <van-col span="16" class="value">{{
            `${checkResult.surplusNum}/${checkResult.totalNum}`
          }}</van-col>
        </van-row>
        <van-row v-if="checkResult.lastCheckInTime" class="row">
          <van-col span="8" class="label">上次检票：</van-col>
          <van-col span="16" class="value">{{ checkResult.lastCheckInTime }}</van-col>
        </van-row>
        <h1>检票成功</h1>
      </section>
      <section v-show="isValidTicket === false" class="section">
        <img :src="failImage" />
        <van-row class="row">
          <van-col span="8" class="label">票号：</van-col>
          <van-col span="16" class="value">{{ input.ticketCode }}</van-col>
        </van-row>
        <h1>{{ errorMsg }}</h1>
      </section>
      <img v-show="isValidTicket === ''" :src="checkImage" />
      <van-button type="primary" size="large" :loading="isLoading" @click="onScan"
        >扫码检票</van-button
      >
      <van-button type="default" size="large" class="search" @click="onSearch">门票查询</van-button>
    </div>
  </div>
</template>

<script>
import checkImage from "./../../assets/check.png";
import successImage from "./../../assets/check-success.png";
import failImage from "./../../assets/check-failed.png";
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import weiXinJsSdkHelper from "./../../utils/weiXinJsSdkHelper.js";
import scenicService from "./../../services/scenicService.js";
import gateService from "./../../services/gateService.js";
import settingService from "./../../services/settingService.js";
import ticketService from "./../../services/ticketService.js";
import memberService from "./../../services/memberService.js";

export default {
  mixins: [mobileMixin],
  data() {
    return {
      grounds: [],
      gateGroups: [],
      checkImage: checkImage,
      successImage: successImage,
      failImage: failImage,
      input: {
        ticketCode: "",
        groundId: localStorage.getItem("GroundId") || "",
        gateGroupId: localStorage.getItem("GateGroupId") || "",
        gateId: localStorage.getItem("GateId") || ""
      },
      isValidTicket: "",
      checkResult: {},
      errorMsg: "",
      timer: -1
    };
  },
  computed: {
    member() {
      return memberService.getMember();
    }
  },
  watch: {
    "input.groundId": async function(val) {
      localStorage.setItem("GroundId", val);

      this.gateGroups = await scenicService.getGateGroupComboboxItemsAsync(val);
      this.input.gateGroupId = "";
    },
    "input.gateGroupId": async function(val) {
      localStorage.setItem("GateGroupId", val);

      if (val) {
        await this.changeLocation();
      }
    }
  },
  async created() {
    try {
      this.loading();

      const getGroundsTask = scenicService.getGroundComboboxItemsAsync().then(grounds => {
        this.grounds = grounds;
      });

      const getGateGroupsTask = scenicService
        .getGateGroupComboboxItemsAsync(this.input.groundId)
        .then(gateGroups => {
          this.gateGroups = gateGroups;
        });

      const registGateTask = this.registGate();

      const configWxJsApiTask = settingService.configWxJsApi();

      await Promise.all([getGroundsTask, getGateGroupsTask, registGateTask, configWxJsApiTask]);

      this.pageLoaded = true;
    } finally {
      this.loaded();
    }
  },
  methods: {
    async registGate() {
      if (this.input.gateId) return;

      const gate = {
        name: `${this.member.name}的手机`,
        identifyCode: this.member.id
      };
      const result = await gateService.registMobileAsync(gate);
      this.input.gateId = result.id;
      localStorage.setItem("GateId", this.input.gateId);
    },
    async changeLocation() {
      const gate = {
        id: this.input.gateId,
        groundId: this.input.groundId,
        gateGroupId: this.input.gateGroupId
      };
      await gateService.changeLocationAsync(gate);
    },
    async onScan() {
      if (!this.input.groundId) {
        this.$notify("请选择检票区域");
        return;
      }
      if (!this.input.gateGroupId) {
        this.$notify("请选择检票点");
        return;
      }

      try {
        this.input.ticketCode = await weiXinJsSdkHelper.scanQRCode();

        this.loading();
        this.checkResult = await ticketService.checkTicketFromMobileAsync(this.input, {
          hideError: true
        });
        this.isValidTicket = true;
      } catch (error) {
        if (error == "cancel") return;

        this.errorMsg = typeof error == "string" ? error : JSON.stringify(error);
        this.isValidTicket = false;
      } finally {
        this.loaded();
        this.clearResult();
      }
    },
    clearResult() {
      clearTimeout(this.timer);
      this.timer = setTimeout(() => {
        this.isValidTicket = "";
      }, 10000);
    },
    async onSearch() {
      const ticketCode = await weiXinJsSdkHelper.scanQRCode();

      this.$router.push({
        name: "QueryTicket",
        params: { ticketCode: ticketCode }
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.content {
  background-color: white;
  padding: 0 30px;
  text-align: center;
  min-height: calc(100vh - 134px);

  img {
    max-width: 70%;
    margin: 20px 0;
  }

  .section {
    img {
      max-width: 130px;
    }

    h1 {
      margin: 20px 0;
    }

    .row {
      margin: 10px 0;
    }

    .label {
      text-align: right;
    }

    .value {
      text-align: left;
    }
  }

  .search {
    margin-top: 20px;
  }
}
</style>
