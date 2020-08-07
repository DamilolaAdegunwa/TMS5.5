<template>
  <div ref="grbody" class="group-regist">
    <van-panel title="注册信息" class="margin-bottom-10">
      <van-cell-group>
        <van-field
          label="用户名"
          v-model="group.userName"
          clearable
          placeholder="4-16个字符，支持字母、数字及符号"
        />
        <van-field
          label="密码"
          type="password"
          v-model="group.password"
          clearable
          placeholder="6-12个字符，支持字母、数字及符号"
        />
        <van-field
          label="确认密码"
          type="password"
          v-model="group.confirmPassword"
          clearable
          placeholder="请再次输入密码"
        />
      </van-cell-group>
    </van-panel>

    <van-panel title="机构信息" class="margin-bottom-10">
      <van-cell-group>
        <multi-picker-field
          label="机构类型"
          placeholder="必填，选择机构类型"
          v-model="group.customerTypeId"
          :list="customerTypes"
        />
        <van-field label="机构名称" v-model="group.name" clearable placeholder="请输入机构名称" />
        <van-field label="机构代码" v-model="group.code" clearable placeholder="社会统一信息代码" />
        <van-cell title="机构照片" center class="van-field">
          <van-uploader :after-read="onRead">
            <van-icon name="photograph" />
          </van-uploader>
          <van-button v-if="group.photo" size="mini" @click="preview" class="group-regist-preview"
            >预览</van-button
          >
        </van-cell>
      </van-cell-group>
    </van-panel>

    <van-panel title="联系人信息">
      <van-field label="姓名" v-model="group.contactName" clearable placeholder="请输入姓名" />
      <van-field
        label="手机号码"
        type="tel"
        v-model="group.contactMobile"
        clearable
        placeholder="请输入手机号码"
      />
      <idcard-field
        label="身份证"
        v-model="group.contactCertNo"
        placeholder="请输入二代身份证号码"
        :z-index="101"
        @showPopupChange="onSubPopupChange"
      />
    </van-panel>

    <bottom-button text="注册" :loading="loading" @click="onClick"></bottom-button>
  </div>
</template>

<script>
import { ImagePreview } from "vant";
import BottomButton from "@/components/BottomButton.vue";
import IdcardField from "@/components/IdCardField.vue";
import MultiPickerField from "@/components/MultiPickerField.vue";
import imageHelper from "@/utils/imageHelper.js";
import validate from "@/utils/validator.js";
import customerService from "@/services/customerService.js";
import { keyboardPopupMixin } from "./../../mixins/KeyboardPopupMixin.js";

export default {
  mixins: [keyboardPopupMixin],
  components: {
    BottomButton,
    IdcardField,
    MultiPickerField
  },
  data() {
    return {
      loading: false,
      group: {
        photo: ""
      },
      customerTypes: []
    };
  },
  methods: {
    async onRead(file) {
      try {
        let img = await imageHelper.resize(file.content, 700, 700);
        this.group.photo = img.photo;
      } catch (error) {
        this.group.photo = "";
        return;
      }
    },
    async onClick() {
      try {
        this.loading = true;
        await this.regist();
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    async regist() {
      let validateRules = [
        {
          value: this.group.userName,
          name: "用户名",
          rules: [{ rule: "required" }, { rule: "minLength:4" }, { rule: "maxLength:16" }]
        },
        {
          value: this.group.password,
          name: "密码",
          rules: [{ rule: "required" }, { rule: "minLength:6" }, { rule: "maxLength:12" }]
        },
        {
          value: this.group.customerTypeId,
          name: "机构类型",
          rules: [{ rule: "required" }]
        },
        {
          value: this.group.name,
          name: "机构名称",
          rules: [{ rule: "required" }]
        },
        {
          value: this.group.code,
          name: "机构代码",
          rules: [{ rule: "required" }]
        },
        {
          value: this.group.photo,
          name: "机构照片",
          rules: [{ rule: "required" }]
        },
        {
          value: this.group.contactName,
          name: "姓名",
          rules: [{ rule: "required" }]
        },
        {
          value: this.group.contactMobile,
          name: "手机号码",
          rules: [{ rule: "required" }, { rule: "isMobile" }]
        },
        {
          value: this.group.contactCertNo,
          name: "身份证",
          rules: [{ rule: "required" }, { rule: "isIdCard" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }
      if (this.group.password !== this.group.confirmPassword) {
        this.$toast("两次输入的密码不一致");
        return;
      }
      const uidPattern = /^[a-zA-Z0-9_-]{4,16}$/;
      if (!uidPattern.test(this.group.userName)) {
        this.$toast("用户名格式不正确");
        return;
      }
      const pwdPattern = /^[a-zA-Z0-9_-]{6,12}$/;
      if (!pwdPattern.test(this.group.password)) {
        this.$toast("密码格式不正确");
        return;
      }

      await customerService.registAsync(this.group);
      this.$router.push("/groupregistsuccess");
    },
    preview() {
      ImagePreview([this.group.photo]);
    }
  },
  async created() {
    let customerTypes = await customerService.getCustomerTypeComboboxItemsAsync();
    this.customerTypes = [
      {
        text: "客户类型",
        children: customerTypes.map(c => {
          return {
            text: c.displayText,
            value: c.value,
            disabled: c.disabled
          };
        })
      }
    ];
  }
};
</script>

<style lang="scss">
.group-regist {
  .van-cell__value {
    text-align: left;
  }

  &-preview {
    margin-left: 20px;
  }
}
</style>
