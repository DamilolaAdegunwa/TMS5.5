<template>
  <div class="page">
    <nav-bar v-if="$route.params.set == 'reset'" title="设置" back="/account" />
    <img class="banner" src="../assets/imgs/banner.png" />

    <div class="content">
      <van-cell-group>
        <van-cell title="请选择检票项目" />
        <picker-field
          v-model="groundId"
          placeholder
          :columns="groundItems"
        ></picker-field>
      </van-cell-group>
      <van-cell-group>
        <van-cell title="请选择检票点" />
        <picker-field
          v-model="gateGroupId"
          placeholder
          :columns="gateGroupItems"
        ></picker-field>
      </van-cell-group>
      <van-cell-group>
        <van-cell title="请选择售票点" />
        <picker-field
          v-model="salePointId"
          placeholder
          :columns="salePointItems"
        ></picker-field>
      </van-cell-group>
      <van-button
        size="large"
        :loading="loading"
        type="primary"
        @click="gotoHome"
        >确认</van-button
      >
    </div>
  </div>
</template>

<script>
import NavBar from "../components/NavBar";
import PickerField from "../components/PickerField.vue";
import scenicService from "../services/scenicService.js";
import gateService from "../services/gateService.js";
import pcService from "../services/pcService.js";
import { App } from "../handset-sdk.js";
import { isNullOrUndefined } from "util";
import payTypeService from "../services/payTypeService.js";

class GateInput {
  constructor() {
    this.groundId = 0;
    this.gateGroupId = 0;
    this.id = 0;
  }
}

class PcInput {
  constructor() {
    this.salePointId = 0;
    this.id = 0;
  }
}

export default {
  name: "Setup",
  components: { NavBar, PickerField },

  data() {
    return {
      loading: false,
      groundId: this.getGroundId(),
      groundItems: [],
      gateGroupId: this.getGateGroupId(),
      gateGroupItems: [],
      salePointId: this.getSalePointId(),
      salePointItems: [],
      gateId: this.getGateId(),
      pcId: this.getPcId(),
      gateInput: new GateInput(),
      pcInput: new PcInput()
    };
  },
  async created() {
    await this.loadData();
  },
  watch: {
    groundId: async function() {
      this.gateGroupId = 0;
      this.gateGroupItems = [];
      await this.getGateGroupItems(this.groundId);
    }
  },
  methods: {
    getGroundId() {
      if (App) {
        let result = App.getValue("groundId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("groundId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getGateGroupId() {
      if (App) {
        let result = App.getValue("gateGroupId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("gateGroupId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getSalePointId() {
      if (App) {
        let result = App.getValue("salePointId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("salePointId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getGateId() {
      if (App) {
        let result = App.getValue("gateId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("gateId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getPcId() {
      if (App) {
        let result = App.getValue("pcId", "0");
        return Number(result);
      } else if (window.localStorage) {
        let result = localStorage.getItem("pcId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    async gotoHome() {
      this.loading = true;

      let groundItem = this.groundItems.filter(
        c => c["value"] == this.groundId
      )[0];

      let gateGroupItem = this.gateGroupItems.filter(
        c => c["value"] == this.gateGroupId
      )[0];

      let salePointItem = this.salePointItems.filter(
        c => c["value"] == this.salePointId
      )[0];

      if (!groundItem) {
        this.$toast("请选择检票项目");
        this.loading = false;
        return;
      }
      if (!gateGroupItem) {
        this.$toast("请选择检票点");
        this.loading = false;
        return;
      }
      if (!salePointItem) {
        this.$toast("请选择售票点");
        this.loading = false;
        return;
      }

      if (App) {
        App.setValue("groundId", this.groundId);
        App.setValue("gateGroupId", this.gateGroupId);
        App.setValue("salePointId", this.salePointId);

        App.setValue("groundIdStr", groundItem.displayText);
      } else if (window.localStorage) {
        let storage = window.localStorage;
        storage.setItem("groundId", this.groundId);
        storage.setItem("gateGroupId", this.gateGroupId);
        storage.setItem("salePointId", this.salePointId);

        storage.setItem("groundIdStr", groundItem.displayText);
      }

      await this.changeLocation();
      this.loading = false;
      if (this.$route.params.set == "set") {
        this.$router.push("/home");
      } else {
        this.$toast.success("设置成功");
        this.$router.push("/home");
      }
    },

    async loadData() {
      this.groundItems = [];
      this.gateGroupItems = [];
      this.salePointItems = [];
      this.getGroundItems();
      this.getSalePointItems();
      this.getGateGroupItems(this.groundId);
      await payTypeService.getPayTypeComboboxItemsAsync();
    },

    async getGroundItems() {
      let result = await scenicService.getGroundComboboxItemsAsync();
      //console.log(JSON.stringify(result));
      result.forEach(item => {
        this.groundItems.push(item);
      });
    },
    async getSalePointItems() {
      let result = await scenicService.getSalePointComboboxItemsAsync();
      //console.log(JSON.stringify(result));
      result.forEach(item => {
        this.salePointItems.push(item);
      });
    },
    async getGateGroupItems(groundId) {
      let result = await scenicService.getGateGroupComboboxItemsAsync(groundId);
      result.forEach(item => {
        this.gateGroupItems.push(item);
      });
    },

    async changeLocation() {
      this.gateInput.groundId = this.groundId;
      this.gateInput.gateGroupId = this.gateGroupId;
      this.gateInput.id = this.gateId;

      this.pcInput.salePointId = this.salePointId;
      this.pcInput.id = this.pcId;

      //console.log(JSON.stringify(this.pcInput));
      await gateService.changeLocationAsync(this.gateInput);
      await pcService.changeLocationAsync(this.pcInput);
      // console.log("方法：changeLocationAsync 执行完毕");

      // let parkId = sessionStorage.getItem("parkId");
      // console.log(
      //   "groundId=" +
      //     this.groundId +
      //     " gateGroupId=" +
      //     this.gateGroupId +
      //     " salePointId=" +
      //     this.salePointId +
      //     " gateId=" +
      //     this.gateId +
      //     " pcId=" +
      //     this.pcId +
      //     " parkId=" +
      //     parkId
      // );
    }
  }
};
</script>

<style lang="scss" scoped>
.banner {
  width: 100%;
}

.van-cell-group {
  margin-bottom: 8px;
}

.selected-value {
  color: #999999;
}

.content {
  padding: 8px;
}
</style>
