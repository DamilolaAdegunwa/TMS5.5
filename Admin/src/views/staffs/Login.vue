<template>
  <div class="login">
    <div class="login-title">
      <img class="title-img" src="\login_logo.png" />
      <div class="title-seperate"></div>
      <div class="div-place-name">{{scenicName}}</div>
    </div>
    <div class="login-main">
      <div class="login-box">
        <div class="login-box-title">
          <div class="title-div">账户登录</div>
        </div>
        <div class="login-box-content">
          <el-form ref="form" :rules="rules" :model="input">
            <el-form-item prop="userName" class="login-item">
              <el-input
                v-model="input.userName"
                size="medium"
                placeholder="账号"
                prefix-icon="al-icon-yonghu"
              ></el-input>
            </el-form-item>
            <el-form-item prop="password" class="login-item">
              <el-input
                v-model="input.password"
                type="password"
                size="medium"
                placeholder="密码"
                prefix-icon="al-icon-wuquanxian"
                @keyup.enter.native="onSubmit"
              ></el-input>
            </el-form-item>
            <el-form-item class="div-button">
              <el-button
                type="primary"
                size="medium"
                :loading="loading"
                @click="onSubmit"
                class="login-box-button"
              >登录</el-button>
            </el-form-item>
          </el-form>
          <div v-if="false" class="login-box-footer">
            <a href="javascript:void(0)">忘记密码</a>
          </div>
        </div>
      </div>
    </div>
    <div class="login-bottom">
      <div class="div-footer">{{copyright}}</div>
    </div>
  </div>
</template>

<script>
import staffService from "./../../services/staffService.js";
import scenicService from "./../../services/scenicService.js";

export default {
  name: "Login",
  data() {
    return {
      input: {
        userName: "",
        password: ""
      },
      rules: {
        userName: [{ required: true, message: "账号不能为空", trigger: "blur" }],
        password: [{ required: true, message: "密码不能为空", trigger: "blur" }]
      },
      loading: false,
      scenicName: '深圳市易高科技有限公司',
      copyright: '版权所有 深圳市易高科技有限公司'
    };
  },
  created() {
    let scenicName = scenicService.getScenicName();
    if(scenicName){
      this.scenicName = scenicName;
    }
    let copyright = scenicService.getCopyright();
    if(copyright){
      this.copyright = this.copyright;
    }
  },
  methods: {
    onSubmit() {
      this.$refs.form.validate(async valid => {
        if (!valid) {
          return false;
        }
        await this.login();
      });
    },
    async login() {
      try {
        this.loading = true;
        await staffService.loginAsync(this.input);
        this.$router.push("/");
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";

.login {
  height: 100vh;
  width: 100%;
  background-color: #ffffff;

  &-main {
    width: 100%;
    margin: 0 auto;
    display: flex;
    justify-content: center;
    align-items: center;
    background: url("./../../assets/img/login_background.jpg");
    background-size: 100% 100%;
    height: 60%;
  }

  &-box {
    background-color: #fff;
    padding: 5px 22px 55px 22px;
    margin-left: 44%;

    &-title {
      font-size: 22px;
      text-align: center;
      color: #414242;
      padding: 30px 0 15px 0px;
      .title-div {
        width: 103px;
        margin: 0 auto;
        padding: 0px 0px 10px 0px;
        border-bottom: 3px solid #419efd;
      }
    }

    &-content {
      padding: 10px 20px 12px;
    }

    &-button {
      width: 311px;
      background: #419efd;
      border-color: #419efd;
      height: 50px;
      font-size: 16px;
    }

    &-footer {
      text-align: right;
      font-size: 12px;

      a {
        color: $blue;
      }
    }
  }
}
.login-title {
  height: 100px;
  padding: 0px 0px 0px 330px;
  display: flex;
  align-items: center;
  .title-seperate {
    border: 1px solid #808080;
    margin: 8px 0px 0px 10px;
    height: 21px;
  }
  .div-place-name {
    font-size: 24px;
    color: #808080;
    padding: 8px 0px 0px 10px;
  }
}
.main-img-div {
  margin-right: 9%;
}
.login-bottom {
  .div-footer {
    color: #808080;
    font-size: 14px;
    text-align: center;
    width: 100%;
    margin-top: 8%;
    position: absolute;
    margin-top: 130px;
    font-family: Microsoft YaHei;
  }
}
.login-item {
  padding: 3px 0px 3px 0px;
  /deep/ .el-input__inner {
    height: 45px;
    line-height: 45px;
    background-color: #ffffff !important;
  }
  /deep/ .el-input__inner:hover {
    border-color: #808080;
  }
  /deep/ input:-internal-autofill-selected {
    background-color: #ff0000 !important;
  }
}
.div-button {
  padding: 16px 0px 0px 0px;
}
</style>
