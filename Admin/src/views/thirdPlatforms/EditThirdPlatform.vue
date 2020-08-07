<template>
  <el-dialog
    :title="title"
    :visible.sync="showDialog"
    :close-on-click-modal="false"
    @closed="onClosed"
    width="30%"
  >
    <el-form ref="form" :model="input" :rules="rules" status-icon label-width="100px">
      <el-form-item prop="name" label="名称">
        <el-input v-model="input.name" clearable></el-input>
      </el-form-item>
      <el-form-item prop="uid" label="用户名">
        <el-input v-model="input.uid" clearable></el-input>
      </el-form-item>
      <el-form-item prop="pwd" label="密码">
        <el-input v-model="input.pwd" type="password" clearable></el-input>
      </el-form-item>
      <el-form-item prop="confimPassword" label="确认密码">
        <el-input v-model="input.confimPassword" type="password" clearable></el-input>
      </el-form-item>
      <el-form-item label="消费通知地址">
        <el-input v-model="input.orderCheckUrl" clearable></el-input>
      </el-form-item>
      <el-form-item prop="platformType" label="接口类型">
        <el-select v-model="input.platformType" filterable placeholder="请选择">
          <el-option
            v-for="item in platformTypes"
            :key="item.value"
            :label="item.displayText"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="close">取 消</el-button>
      <el-button type="primary" :loading="saving" @click="save">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { editViewMixin } from "./../../mixins/editViewMixin.js";
import thirdPlatformService from "./../../services/thirdPlatformService.js";

class EditInput {
  constructor() {
    this.name = "";
    this.uid = "";
    this.pwd = "";
    this.confimPassword = "";
    this.orderCheckUrl = "";
    this.platformType = "";
  }
}

export default {
  name: "EditThirdPlatform",
  mixins: [editViewMixin],
  data() {
    const validateConfimPassword = (rule, value, callback) => {
      if (value !== this.input.pwd) {
        callback(new Error("两次输入密码不一致!"));
      } else {
        callback();
      }
    };

    return {
      input: new EditInput(),
      platformTypes: [],
      rules: {
        name: [{ required: true, message: "请输入名称", trigger: "blur" }],
        uid: [{ required: true, message: "请输入用户名", trigger: "blur" }],
        pwd: [{ required: true, message: "请输入密码", trigger: "blur" }],
        confimPassword: [
          { required: true, message: "请再次输入密码", trigger: "blur" },
          { validator: validateConfimPassword, trigger: "blur" }
        ],
        platformType: [{ required: true, message: "请选择接口类型", trigger: "change" }]
      }
    };
  },
  computed: {
    title() {
      return `${this.editText}第三方平台`;
    }
  },
  async created() {
    this.platformTypes = await thirdPlatformService.getPlatformTypeComboboxItemsAsync();
  },
  methods: {
    getDataForEdit() {
      thirdPlatformService.getThirdPlatformForEditAsync(this.id).then(data => {
        this.input = data;
        this.input.confimPassword = data.pwd;
      });
    },
    async create() {
      return await thirdPlatformService.createAsync(this.input);
    },
    async update() {
      return await thirdPlatformService.updateAsync(this.input);
    },
    clear() {
      this.input = new EditInput();
    }
  }
};
</script>
