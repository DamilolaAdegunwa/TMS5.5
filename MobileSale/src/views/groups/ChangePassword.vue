<template>
  <div>
    <div class="big-title">修改密码</div>

    <van-cell-group>
      <van-field
        label="原密码"
        type="password"
        v-model="input.originalPassword"
        clearable
        placeholder="请输入原密码"
      />
      <van-field
        label="新密码"
        type="password"
        v-model="input.newPassword"
        clearable
        placeholder="请输入新密码"
      />
      <van-field
        label="确认新密码"
        type="password"
        v-model="confirmNewPassword"
        clearable
        placeholder="请输入新密码"
      />
    </van-cell-group>

    <div class="big-button">
      <van-button type="primary" size="large" :loading="loading" @click="onClick">确定</van-button>
    </div>
  </div>
</template>

<script>
import validate from "@/utils/validator.js";
import customerService from "@/services/customerService.js";
import memberService from "@/services/memberService.js";

export default {
  data() {
    return {
      input: {
        originalPassword: "",
        newPassword: ""
      },
      confirmNewPassword: "",
      loading: false
    };
  },
  methods: {
    async onClick() {
      const validateRules = [
        {
          value: this.input.originalPassword,
          name: "原密码",
          rules: [{ rule: "required" }]
        },
        {
          value: this.input.newPassword,
          name: "新密码",
          rules: [{ rule: "required" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }
      if (!this.confirmNewPassword) {
        this.$toast("请再次输入新密码");
        return;
      }
      if (this.input.newPassword !== this.confirmNewPassword) {
        this.$toast("两次输入的密码不一致");
        return;
      }
      const pwdPattern = /^[a-zA-Z0-9_-]{6,12}$/;
      if (!pwdPattern.test(this.input.newPassword)) {
        this.$toast("密码格式不正确");
        return;
      }

      try {
        this.loading = true;
        await customerService.changePasswordAsync(this.input);
        await memberService.customerLogoutAsync();
        this.$router.push("/my");
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style lang="scss">
.big-title {
  height: 100px;
  line-height: 100px;
  background-color: white;
  text-align: center;
  font-size: 20px;
  font-weight: bold;
}
.big-button {
  margin-top: 50px;
  padding: 0 20px;
}
</style>
