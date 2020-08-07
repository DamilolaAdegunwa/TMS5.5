<template>
  <div class="bind-staff">
    <van-row type="flex" justify="center" align="center" class="bind-staff-header">
      <van-col>员工绑定</van-col>
    </van-row>

    <van-cell-group>
      <van-field label="用户名" v-model="input.userName" clearable placeholder="请输入用户名" />
      <van-field
        label="密码"
        type="password"
        v-model="input.password"
        clearable
        placeholder="请输入密码"
      />
    </van-cell-group>

    <div class="bind-staff-button">
      <van-button type="primary" size="large" :loading="loading" @click="onClick">绑定</van-button>
    </div>
  </div>
</template>

<script>
import memberService from "@/services/memberService.js";
import validate from "@/utils/validator.js";

export default {
  data() {
    return {
      loading: false,
      input: {
        userName: "",
        password: ""
      }
    };
  },
  methods: {
    async onClick() {
      try {
        this.loading = true;

        await this.login();
      } finally {
        this.loading = false;
      }
    },
    async login() {
      let validateRules = [
        {
          value: this.input.userName,
          name: "用户名",
          rules: [{ rule: "required" }]
        },
        {
          value: this.input.password,
          name: "密码",
          rules: [{ rule: "required" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }

      await memberService.bindStaffAsync(this.input);

      this.$router.replace("/my");
    }
  }
};
</script>

<style lang="scss">
.bind-staff {
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
}
</style>
