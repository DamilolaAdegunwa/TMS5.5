<template>
  <div class="page">
    <nav-bar title="修改密码" back="/account" />

    <van-cell-group>
      <van-field
        label="旧密码"
        :type="fieldType[0].type"
        placeholder="请输入旧密码"
        v-model="input.oldPassword"
        @click-right-icon="eyeSlash(0)"
      >
        <font-awesome-icon slot="right-icon" :icon="fieldType[0].icon" />
      </van-field>
      <van-field
        label="新密码"
        :type="fieldType[1].type"
        placeholder="请输入6-12以内的字符"
        v-model="input.password"
        @click-right-icon="eyeSlash(1)"
      >
        <font-awesome-icon slot="right-icon" :icon="fieldType[1].icon" />
      </van-field>
      <van-field
        label="确认密码"
        :type="fieldType[2].type"
        placeholder="请重复新密码"
        v-model="confirmPassword"
        @click-right-icon="eyeSlash(2)"
      >
        <font-awesome-icon slot="right-icon" :icon="fieldType[2].icon" />
      </van-field>
    </van-cell-group>

    <van-button
      type="primary"
      :loading="loading"
      size="large"
      @click="ChangePassword"
      >确认修改</van-button
    >

    <tab-bar :active="1" />
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import TabBar from "../../components/TabBar";
import staffService from "../../services/staffService.js";
import validate from "../../utils/validator.js";
class Input {
  constructor() {
    this.oldPassword = "";
    this.password = "";
  }
}

export default {
  name: "AccountChangePassword",
  components: { TabBar, NavBar },
  data() {
    return {
      loading: false,
      fieldType: [
        {
          icon: "eye-slash",
          type: "password"
        },
        {
          icon: "eye-slash",
          type: "password"
        },
        {
          icon: "eye-slash",
          type: "password"
        }
      ],
      input: new Input(),
      confirmPassword: ""
    };
  },
  methods: {
    eyeSlash(value) {
      if (this.fieldType[value].icon == "eye-slash") {
        this.fieldType[value].icon = "eye";
        this.fieldType[value].type = "";
      } else {
        this.fieldType[value].icon = "eye-slash";
        this.fieldType[value].type = "password";
      }
    },
    async ChangePassword() {
      this.loading = true;
      let validateRules = [
        {
          value: this.input.oldPassword,
          name: "旧密码",
          rules: [{ rule: "required" }]
        },
        {
          value: this.input.password,
          name: "新密码",
          rules: [
            { rule: "required" },
            { rule: "minLength", type: 6 },
            { rule: "maxLength", type: 12 }
          ]
        },
        {
          value: this.confirmPassword,
          name: "确认密码",
          rules: [{ rule: "required" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast.fail(result.message);
        this.loading = false;
        return;
      }

      if (this.input.password == this.input.oldPassword) {
        this.$toast.fail("新密码不能与旧密码相同");
        this.loading = false;
        return;
      }

      if (this.input.password !== this.confirmPassword) {
        this.$toast.fail("新密码与确认密码不一致");
        this.loading = false;
        return;
      }

      staffService
        .editPasswordAsync(this.input)
        .then(result => {
          this.loading = false;
          if (result.success) {
            this.$toast.success("密码修改成功");
            this.input = new Input();
            this.confirmPassword = "";
          }
        })
        .catch(result => {
          this.loading = false;
          this.$toast.fail(result.response.data.error.message);
        });
    },
    gotoAccount() {
      this.$router.push("/account");
    }
  }
};
</script>

<style lang="scss" scoped>
.van-nav-bar {
  margin-bottom: 12px;
}

.van-button {
  margin: 12px 8px 0;
  width: calc(100% - 16px);
}
</style>
