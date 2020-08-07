<template>
  <el-dialog title="修改密码" :visible.sync="showDialog" @closed="onClosed" width="400px">
    <el-form ref="form" :model="input" :rules="rules" status-icon label-width="100px">
      <el-form-item prop="oldPassword" label="原密码">
        <el-input v-model="input.oldPassword" type="password" placeholder="请输入原密码"></el-input>
      </el-form-item>
      <el-form-item prop="password" label="新密码">
        <el-input v-model="input.password" type="password" placeholder="请输入新密码"></el-input>
      </el-form-item>
      <el-form-item prop="confimPassword" label="确认新密码">
        <el-input v-model="input.confimPassword" type="password" placeholder="请再次输入新密码"></el-input>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="close">取 消</el-button>
      <el-button type="primary" :loading="saving" @click="save">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import staffService from "./../../services/staffService.js";

class EditPasswordInput {
  constructor() {
    this.oldPassword = "";
    this.password = "";
    this.confimPassword = "";
  }
}

export default {
  name: "EditPassword",
  props: {
    value: {
      type: Boolean,
      default: false
    }
  },
  data() {
    const validateConfimPassword = (rule, value, callback) => {
      if (value !== this.input.password) {
        callback(new Error("两次输入密码不一致!"));
      } else {
        callback();
      }
    };

    return {
      showDialog: this.value,
      input: new EditPasswordInput(),
      rules: {
        oldPassword: [{ required: true, message: "请输入原密码", trigger: "blur" }],
        password: [{ required: true, message: "请输入新密码", trigger: "blur" }],
        confimPassword: [
          { required: true, message: "请再次输入新密码", trigger: "blur" },
          { validator: validateConfimPassword, trigger: "blur" }
        ]
      },
      saving: false
    };
  },
  watch: {
    value(val) {
      this.showDialog = val;
    }
  },
  methods: {
    save() {
      this.$refs.form.validate(async valid => {
        if (!valid) {
          return false;
        }

        try {
          this.saving = true;
          const result = await staffService.editPasswordAsync(this.input);
          if (result.success) {
            this.close();
            this.$message.success("修改成功");
            setTimeout(() => {
              staffService.logout();
              this.$router.push({ name: "Login" });
            }, 1500);
          }
        } catch (err) {
          return;
        } finally {
          this.saving = false;
        }
      });
    },
    close() {
      this.showDialog = false;
    },
    onClosed() {
      this.input = new EditPasswordInput();
      this.$refs.form.resetFields();
      this.$emit("input", this.showDialog);
    }
  }
};
</script>
