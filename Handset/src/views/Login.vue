<template>
  <div class="page">
    <div class="login-avatar">
      <img src="../assets/imgs/login.png" />
      <h5>系统登录</h5>
    </div>

    <form class="login-form">
      <van-field v-model="input.userName" placeholder="账号">
        <span slot="left-icon" class="field-icon">
          <font-awesome-icon icon="user" />
        </span>
      </van-field>
      <van-field
        v-model="input.passWord"
        left-icon="contact"
        type="password"
        placeholder="密码"
      >
        <span slot="left-icon" class="field-icon">
          <font-awesome-icon slot="left-icon" icon="lock" />
        </span>
      </van-field>

      <van-button
        size="large"
        type="primary"
        :disabled="disabled"
        v-on:click.prevent="login"
        >登录</van-button
      >
      <span class="errMsg" @click="checkDevice">{{ deviceUseFlagMsg }}</span>
    </form>
  </div>
</template>

<script>
import staffService from "./../services/staffService.js";
import validate from "./../utils/validator.js";
import { App } from "./../handset-sdk.js";
import gateService from "./../services/gateService.js";
import pcService from "./../services/pcService.js";

class Input {
  constructor() {
    this.userName = "";
    this.passWord = "";
    this.pcId = 0;
  }
}

class GateInput {
  constructor() {
    this.name = "";
    this.tcpIp = "";
    this.tcpMac = "";
    this.identifyCode = "";
  }
}

class PcInput {
  constructor() {
    this.name = "";
    this.ip = "";
    this.mac = "";
    this.identifyCode = "";
  }
}

export default {
  name: "Login",
  data() {
    return {
      disabled: false,
      input: new Input(),
      gateInput: new GateInput(),
      pcInput: new PcInput(),
      deviceUseFlagMsg: ""
    };
  },
  created() {
    this.checkDevice();
  },
  methods: {
    async login() {
      let validateRules = [
        {
          value: this.input.userName,
          name: "用户名",
          rules: [{ rule: "required" }]
        },
        {
          value: this.input.passWord,
          name: "密码",
          rules: [{ rule: "required" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }

      staffService
        .loginAsync(this.input)
        .then(() => {
          this.$router.push("/setup/set");
        })
        .catch(result => {
          this.$toast(result.response.data.error.message);
        });
    },

    checkDevice() {
      this.disabled = true;
      this.deviceUseFlagMsg = "";

      let model = "";
      let SN = "";
      if (App) {
        model = App.model;
        SN = App.SN;
      } else {
        if (process.env.NODE_ENV === "production") {
          this.deviceUseFlagMsg = "设备注册问题!";
          return;
        } else {
          model = "TPS900";
          SN = "867188038412594";
        }
      }

      this.gateInput.name = model;
      this.gateInput.tcpIp = "";
      this.gateInput.tcpMac = "";
      this.gateInput.identifyCode = SN;

      this.pcInput.name = model;
      this.pcInput.ip = "";
      this.pcInput.mac = "";
      this.pcInput.identifyCode = SN;

      let gateUseFlagMsg = "通道未注册";
      let pcUseFlagMsg = "主机未注册";

      const registGate = gateService
        .registHandsetAsync(this.gateInput)
        .then(gateResult => {
          if (gateResult.result.useFlag) {
            gateUseFlagMsg = "";
          }
        });

      const registPc = pcService
        .registHandsetAsync(this.pcInput)
        .then(pcResult => {
          if (pcResult.result.permitFlag) {
            pcUseFlagMsg = "";
            this.input.pcId = pcResult.result.id;
          }
        });

      Promise.all([registGate, registPc])
        .then(() => {
          if (gateUseFlagMsg != "" || pcUseFlagMsg != "") {
            this.deviceUseFlagMsg = `${gateUseFlagMsg}\n${pcUseFlagMsg}\n设备号:${SN}`;
          } else {
            this.disabled = false;
          }
          this.$toast.clear();
        })
        .catch(() => {
          this.deviceUseFlagMsg = "设备注册问题";
          this.$toast.clear();
        });
    }
  }
};
</script>

<style lang="scss" scoped>
.page {
  background-image: url("../assets/imgs/login-bg.png");
}
.errMsg {
  color: red;
  white-space: pre-line;
}
.login-avatar {
  text-align: center;
  padding-top: 80px;

  img {
    width: 123px;
    height: 123px;
    margin-bottom: 12px;
  }

  h5 {
    font-weight: normal;
    font-size: 18px;
  }
}

.login-form {
  padding: 40px;

  .van-field {
    margin-bottom: 8px;
    border-radius: 3px;
  }

  .field-icon {
    color: #999999;
  }

  .van-button {
    border-radius: 3px;
    margin-top: 20px;
  }
}
</style>
