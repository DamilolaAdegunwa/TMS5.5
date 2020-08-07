<template>
  <div>
    <van-panel title="参观日期" class="margin-bottom-10">
      <date-selector
        v-model="orderInput.travelDate"
        :dates="options.dates"
        :disabled-weeks="options.disabledWeeks"
      ></date-selector>
    </van-panel>

    <van-panel title="成人信息" class="margin-bottom-10">
      <van-cell-group>
        <van-cell title="参观人数" class="van-field">
          <van-stepper
            v-model="orderInput.adultQuantity"
            :defaultValue="0"
            :min="0"
            :max="maxAdultQuantity"
            integer
          />
        </van-cell>
      </van-cell-group>
      <van-cell-group v-for="(adult, index) in orderInput.adults" :key="index">
        <div class="tourist-card">
          <van-field
            ref="adultName"
            label="姓名"
            :maxlength="10"
            placeholder="必填，输入证件上的姓名"
            v-model="adult.name.value"
            :error="adult.name.error"
            :error-message="adult.name.errorMessage"
            clearable
          ></van-field>
          <idcard-field
            label="身份证"
            v-model="adult.certNo.value"
            :error="adult.certNo.error"
            :error-message="adult.certNo.errorMessage"
            :delay="isKeyboardPopup"
            :z-index="101"
            @showPopupChange="onSubPopupChange"
            class="cell-value-left"
          />
        </div>
      </van-cell-group>
    </van-panel>

    <van-panel v-if="addChildren" title="儿童信息" class="margin-bottom-10">
      <van-cell-group class="tourist-quantity">
        <van-cell title="参观人数" class="van-field">
          <van-stepper
            v-model="orderInput.childrenQuantity"
            :defaultValue="0"
            :min="0"
            :max="maxChildrenQuantity"
          />
        </van-cell>
        <div v-if="options.perAdultMaxChildrenQuantity > 0" class="tourist-tips">
          <van-icon name="info-o" />
          <span>每个成人最多携带{{ options.perAdultMaxChildrenQuantity }}名儿童</span>
        </div>
        <van-row></van-row>
      </van-cell-group>
      <van-cell-group v-for="(child, index) in orderInput.children" :key="index">
        <div class="tourist-card">
          <van-field
            ref="childrenName"
            label="姓名"
            :maxlength="10"
            placeholder="必填，输入证件上的姓名"
            v-model="child.name.value"
            :error="child.name.error"
            :error-message="child.name.errorMessage"
            clearable
          ></van-field>
          <date-field
            label="出生日期"
            placeholder="必填，选择出生日期"
            v-model="child.birthday"
            :min-date="childrenMinDate"
            :max-date="new Date()"
          />
        </div>
      </van-cell-group>
    </van-panel>

    <van-panel title="联系人信息" :class="{ 'tourist-contact': !addChildren }">
      <van-cell-group :border="false" class="van-hairline--bottom">
        <van-field
          ref="mobile"
          label="手机号"
          placeholder="必填，输入联系人手机号"
          type="tel"
          v-model="orderInput.contact.mobile"
          clearable
        ></van-field>
      </van-cell-group>
    </van-panel>
    <bottom-button text="提交" :loading="loading" @click="onSubmit"></bottom-button>
  </div>
</template>

<script>
import DateSelector from "@/components/DateSelector.vue";
import DateField from "@/components/DateField.vue";
import BottomButton from "@/components/BottomButton.vue";
import IdcardField from "@/components/IdCardField.vue";
import dayjs from "dayjs";
import validate from "@/utils/validator.js";
import { mapState, mapMutations, mapActions } from "vuex";
import appConsts from "@/store/consts.js";
import orderService from "@/services/orderService";
import Vue from "vue";
import { keyboardPopupMixin } from "./../../mixins/KeyboardPopupMixin.js";

function Tourist() {
  this.type = "成人";
  this.name = {
    value: "",
    error: false,
    errorMessage: ""
  };
  this.certNo = {
    value: "",
    error: false,
    errorMessage: ""
  };
  this.birthday = "";
}

export default {
  mixins: [keyboardPopupMixin],
  components: {
    DateSelector,
    DateField,
    BottomButton,
    IdcardField
  },
  data() {
    return {
      orderInput: new orderService.CreateOrderInput(),
      loading: false,
      isKeyboardPopup: false
    };
  },
  computed: {
    addChildren() {
      return this.$route.query.addChildren;
    },
    childrenMinDate() {
      return dayjs()
        .addYears(-18)
        .toDate();
    },
    maxAdultQuantity() {
      if (this.options.individualOrderMaxAdultQuantity <= 0) {
        return 100;
      }
      return this.options.individualOrderMaxAdultQuantity;
    },
    maxChildrenQuantity() {
      if (
        this.options.perAdultMaxChildrenQuantity <= 0 &&
        this.options.individualOrderMaxChildrenQuantity <= 0
      ) {
        return 100;
      }
      if (this.options.perAdultMaxChildrenQuantity > 0) {
        let adultQuantity = this.orderInput.adultQuantity;
        if (adultQuantity <= 0) {
          adultQuantity = 1;
        }
        let maxQuantity = adultQuantity * this.options.perAdultMaxChildrenQuantity;
        if (
          this.options.individualOrderMaxChildrenQuantity > 0 &&
          maxQuantity > this.options.individualOrderMaxChildrenQuantity
        ) {
          maxQuantity = this.options.individualOrderMaxChildrenQuantity;
        }
        return maxQuantity;
      } else {
        return this.options.individualOrderMaxChildrenQuantity;
      }
    },
    totalPerson() {
      return this.orderInput.adults.length + this.orderInput.children.length;
    },
    ...mapState({
      options: state => state.orderModule.options,
      member: state => state.memberModule.member
    })
  },
  methods: {
    async onSubmit() {
      if (this.totalPerson === 0) {
        this.$toast("请添加参观人信息");
        return;
      }

      if (this.orderInput.adults.length > 0) {
        for (let i = 0; i < this.orderInput.adults.length; i++) {
          let adult = this.orderInput.adults[i];
          let result = validate([
            {
              value: adult.name.value,
              name: "姓名",
              rules: [
                {
                  rule: "required"
                }
              ]
            }
          ]);
          Vue.set(adult.name, "error", !result.success);
          Vue.set(adult.name, "errorMessage", result.message);
          if (!result.success) {
            this.$refs.adultName[i].$refs.input.focus();
            this.$toast(result.message);
            return;
          }

          result = validate([
            {
              value: adult.certNo.value,
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
          ]);
          Vue.set(adult.certNo, "error", !result.success);
          Vue.set(adult.certNo, "errorMessage", result.message);
          if (!result.success) {
            this.$toast(result.message);
            return;
          }
        }
      }

      if (this.orderInput.children.length > 0) {
        for (let i = 0; i < this.orderInput.children.length; i++) {
          let children = this.orderInput.children[i];
          let result = validate([
            {
              value: children.name.value,
              name: "姓名",
              rules: [
                {
                  rule: "required"
                }
              ]
            }
          ]);
          Vue.set(children.name, "error", !result.success);
          Vue.set(children.name, "errorMessage", result.message);
          if (!result.success) {
            this.$refs.childrenName[i].$refs.input.focus();
            this.$toast(result.message);
            return;
          }
        }
      }

      let adultQuantity = this.orderInput.adultQuantity;
      if (this.member.hasElectronicMemberCard) {
        adultQuantity++;
      }
      if (adultQuantity === 0 && this.orderInput.childrenQuantity > 0) {
        this.$toast("儿童必须有成人陪同");
        return;
      }
      if (
        this.options.individualOrderMaxAdultQuantity > 0 &&
        this.orderInput.adultQuantity > this.options.individualOrderMaxAdultQuantity
      ) {
        this.$toast("每单最多" + this.options.individualOrderMaxAdultQuantity + "个成人");
        return;
      }
      if (
        this.options.individualOrderMaxChildrenQuantity > 0 &&
        this.orderInput.childrenQuantity > this.options.individualOrderMaxChildrenQuantity
      ) {
        this.$toast("每单最多" + this.options.individualOrderMaxChildrenQuantity + "个儿童");
        return;
      }
      if (
        this.options.perAdultMaxChildrenQuantity > 0 &&
        adultQuantity * this.options.perAdultMaxChildrenQuantity < this.orderInput.childrenQuantity
      ) {
        this.$toast("每个成人最多携带" + this.options.perAdultMaxChildrenQuantity + "个儿童");
        return;
      }

      let result = validate([
        {
          value: this.orderInput.contact.mobile,
          name: "手机号",
          rules: [{ rule: "required" }, { rule: "isMobile" }]
        }
      ]);
      if (!result.success) {
        this.$refs.mobile.$refs.input.focus();
        this.$toast(result.message);
        return;
      }

      try {
        let tourists = this.orderInput.adults.concat(this.orderInput.children);
        tourists = tourists.map(t => {
          return {
            type: t.type,
            name: t.name.value,
            certNo: t.certNo.value,
            birthday: t.birthday
          };
        });
        this.loading = true;
        let response = await orderService.createIndividualOrderAsync({
          travelDate: this.orderInput.travelDate,
          tourists: tourists,
          contact: this.orderInput.contact
        });
        if (response.success) {
          this.$router.replace({
            name: "orderinfo",
            params: {
              listNo: response.result
            }
          });
        }
      } catch (err) {
        return;
      } finally {
        this.loading = false;
      }
    },
    onSubPopupChange(show, height, top) {
      if (show) {
        window.scrollTo(0, top - 100);
      }
    },
    ...mapMutations([appConsts.setShowNavbar]),
    ...mapActions("orderModule", [appConsts.loadOrderOptionsAsync])
  },
  watch: {
    "orderInput.adultQuantity": function(newVal, oldVal) {
      let diff = newVal - oldVal;
      if (diff > 0) {
        for (let i = 0; i < diff; i++) {
          let adult = new Tourist();
          adult.type = "成人";
          this.orderInput.adults.push(adult);
        }
      } else {
        this.orderInput.adults.length += diff;
        if (this.orderInput.childrenQuantity > this.maxChildrenQuantity) {
          this.orderInput.childrenQuantity = this.maxChildrenQuantity;
        }
      }
    },
    "orderInput.childrenQuantity": function(newVal, oldVal) {
      let diff = newVal - oldVal;
      if (diff > 0) {
        for (let i = 0; i < diff; i++) {
          let child = new Tourist();
          child.type = "儿童";
          child.birthday = dayjs().toDateString();
          this.orderInput.children.push(child);
        }
      } else {
        this.orderInput.children.length += diff;
      }
    }
  },
  async created() {
    await this.loadOrderOptionsAsync();
    if (!this.orderInput.travelDate) {
      this.orderInput.travelDate = this.options.dates.filter(d => !d.disable)[0].date;
    }
  }
};
</script>

<style lang="scss" scoped>
.tourist {
  &-quantity {
    .van-cell:not(:last-child)::after {
      border: none;
    }
  }

  &-tips {
    font-size: 12px;
    margin: 0 15px;
    margin-bottom: 10px;
    padding: 5px 5px;
    background-color: #f5f9f8;
    border-radius: 3px;

    span {
      padding-left: 5px;
    }
  }

  &-card {
    &::after {
      content: "";
      display: block;
      height: 2px;
      background-color: #e5e5e5;
    }
  }

  &-contact {
    height: 186px;
  }
}
</style>
