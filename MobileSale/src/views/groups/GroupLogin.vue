<template>
  <div class="group-login">
    <van-row type="flex" justify="center" align="center" class="group-login-header">
      <van-col>{{ title }}</van-col>
    </van-row>

    <van-cell-group>
      <van-field
        label="用户名"
        v-model="loginInput.userName"
        clearable
        placeholder="请输入用户名"
      />
      <van-field
        label="密码"
        type="password"
        v-model="loginInput.password"
        clearable
        placeholder="请输入密码"
      />
    </van-cell-group>

    <div class="group-login-button">
      <van-button type="primary" size="large" :loading="loading" @click="onClick">{{
        shouldBindMember ? "绑定" : "登录"
      }}</van-button>
    </div>

    <van-row v-if="!shouldBindMember" type="flex" justify="center" class="group-login-regist">
      <van-col>
        <router-link to="/groupregist">团队注册</router-link>
      </van-col>
    </van-row>
  </div>
</template>

<script>
import store from "@/store/index.js";
import appConsts from "@/store/consts.js";
import customerService from "@/services/customerService.js";
import validate from "@/utils/validator.js";

export default {
  data() {
    return {
      title: "团队登录",
      loading: false,
      loginInput: {
        userName: "",
        password: ""
      }
    };
  },
  props: {
    shouldBind: {
      type: String,
      default: "0"
    },
    nextPath: {
      type: String,
      default: ""
    }
  },
  computed: {
    shouldBindMember() {
      return this.shouldBind === "1";
    }
  },
  methods: {
    async onClick() {
      try {
        this.loading = true;

        await this.login();
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async login() {
      let validateRules = [
        {
          value: this.loginInput.userName,
          name: "用户名",
          rules: [
            {
              rule: "required"
            }
          ]
        },
        {
          value: this.loginInput.password,
          name: "密码",
          rules: [
            {
              rule: "required"
            }
          ]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }

      this.loginInput.shouldBindMember = this.shouldBindMember;
      await customerService.loginFromWeChatAsync(this.loginInput);

      if (this.nextPath) {
        this.$router.replace(`/${this.nextPath}`);
      } else {
        this.$router.go(-1);
      }
    }
  },
  created() {
    this.title = this.shouldBindMember ? "团队绑定" : "团队登录";
    window.document.title = this.title;
    store.commit(appConsts.setTitle, this.title);
  }
};
</script>

<style lang="scss">
.group-login {
  &-header {
    height: 100px;
    background-color: white;
    font-size: 20px;
    font-weight: bold;
  }

  &-button {
    margin-top: 50px;
    padding: 0 20px;
  }

  &-regist {
    margin-top: 20px;
    padding: 0 80px;
    font-size: 13px;

    a {
      color: #19a0f0;
    }
  }
}
</style>
