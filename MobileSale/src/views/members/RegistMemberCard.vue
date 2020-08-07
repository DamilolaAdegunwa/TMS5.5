<template>
  <div class="regist-membercard">
    <div class="regist-membercard-tips">注册长期电子会员卡，可多次检票入园</div>

    <van-cell-group>
      <van-field label="姓名" v-model="input.name" clearable placeholder="请输入姓名" />
      <van-field
        label="手机号码"
        type="tel"
        v-model="input.mobile"
        clearable
        placeholder="请输入手机号码"
      />
      <idcard-field
        label="身份证"
        v-model="input.idCard"
        placeholder="请输入二代身份证号码"
        :z-index="101"
      />
      <multi-picker-field
        label="学历"
        placeholder="选择学历"
        v-model="input.education"
        :list="educations"
      />
      <van-cell title="性别" class="van-field cell-value-left">
        <van-radio-group v-model="input.sex">
          <van-row>
            <van-col span="6">
              <van-radio name="男">男</van-radio>
            </van-col>
            <van-col span="6">
              <van-radio name="女">女</van-radio>
            </van-col>
          </van-row>
        </van-radio-group>
      </van-cell>
      <multi-picker-field
        label="民族"
        placeholder="选择民族"
        v-model="input.nation"
        :list="nations"
      />
      <van-field label="住址" v-model="input.address" clearable placeholder="请输入居住地址" />
    </van-cell-group>

    <div class="regist-membercard-button">
      <van-button type="primary" size="large" :loading="loading" @click="onClick">提交</van-button>
    </div>
  </div>
</template>

<script>
import IdcardField from "@/components/IdCardField.vue";
import MultiPickerField from "@/components/MultiPickerField.vue";
import memberService from "@/services/memberService.js";
import commonService from "@/services/commonService.js";
import validate from "@/utils/validator.js";
import { createNamespacedHelpers } from "vuex";
const { mapMutations } = createNamespacedHelpers("memberModule");

export default {
  components: {
    IdcardField,
    MultiPickerField
  },
  data() {
    return {
      loading: false,
      input: {
        name: "",
        mobile: "",
        idCard: "",
        sex: "",
        nation: "",
        education: "",
        address: ""
      },
      educations: [],
      nations: []
    };
  },
  methods: {
    async onClick() {
      let validateRules = [
        {
          value: this.input.name,
          name: "姓名",
          rules: [
            {
              rule: "required"
            }
          ]
        },
        {
          value: this.input.mobile,
          name: "手机号码",
          rules: [
            {
              rule: "required"
            },
            {
              rule: "isMobile"
            }
          ]
        },
        {
          value: this.input.idCard,
          name: "身份证",
          rules: [
            {
              rule: "required"
            },
            {
              rule: "isIdCard"
            }
          ]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }

      try {
        this.loading = true;
        let response = await memberService.registMemberCardAsync(this.input);
        if (response.success) {
          this.registMemberCard();
          this.$router.replace("/membercard");
        }
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    ...mapMutations(["registMemberCard"])
  },
  created() {
    commonService.getEducationComboboxItems().then(items => {
      let pickerItems = items.map(item => {
        return { value: item.value, text: item.displayText };
      });
      this.educations = [{ text: "学历", children: pickerItems }];
    });

    commonService.getNationComboboxItems().then(items => {
      let pickerItems = items.map(item => {
        return { value: item.value, text: item.displayText };
      });
      this.nations = [{ text: "民族", children: pickerItems }];
    });
  }
};
</script>

<style lang="scss">
.regist-membercard {
  &-tips {
    margin: 15px 0px;
    text-align: center;
    font-size: 13px;
  }

  &-button {
    margin-top: 20px;
    padding: 0 20px;
  }
}
</style>
