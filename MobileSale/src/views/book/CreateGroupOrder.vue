<template>
  <div class="group-order">
    <van-panel title="参观日期" class="margin-bottom-10">
      <date-selector
        v-model="orderInput.travelDate"
        :dates="options.dates"
        :disabled-weeks="options.disabledWeeks"
      ></date-selector>
    </van-panel>

    <van-panel title="参观信息" class="margin-bottom-10">
      <van-cell-group>
        <van-cell title="单位名称" class="van-field">{{ member.customerName }}</van-cell>
        <div class="group-order-person">
          <van-cell title="年龄段" class="van-field">参观人数</van-cell>
          <van-cell
            v-for="(ageRange, index) in ageRanges"
            :key="index"
            :title="ageRange.displayText"
            class="van-field"
          >
            <van-stepper
              v-model="ageRange.personNum"
              :min="0"
              :max="groupOrderMaxQuantity"
              integer
            />
          </van-cell>
        </div>
        <van-row type="flex" justify="center">
          <van-col span="22">
            <div class="group-order-tips" v-if="options.groupOrderMaxQuantity > 0">
              <van-icon name="info-o" />
              <span>每单最多{{ options.groupOrderMaxQuantity }}人</span>
            </div>
          </van-col>
        </van-row>
        <multi-picker-field
          label="客源类型"
          placeholder="必填，选择客源类型"
          v-model="orderInput.keYuanTypeId"
          :list="keYuanTypes"
          class="van-hairline--top"
        />
        <van-cell title="客源地" class="van-field">
          <van-row class="inline-cell">
            <van-col :span="showArea ? 12 : 24">
              <multi-picker-field placeholder="选择客源地" v-model="regionId" :list="regions" />
            </van-col>
            <van-col span="12" v-if="showArea">
              <multi-picker-field
                placeholder="选择客源地"
                v-model="orderInput.areaId"
                :list="areas"
              />
            </van-col>
          </van-row>
        </van-cell>
        <van-field
          label="带队人员"
          :maxlength="10"
          placeholder="输入带队人员姓名"
          v-model="orderInput.jidiaoName"
          clearable
        ></van-field>
        <date-field
          label="入馆时间"
          v-model="orderInput.arrivalTime"
          type="time"
          :min-hour="minHour"
          :max-hour="maxHour"
        />
        <van-field
          label="车牌号"
          placeholder="输入车牌号"
          :maxlength="50"
          v-model="orderInput.licensePlateNumber"
          clearable
        ></van-field>
        <multi-picker-field
          label="讲解场次"
          placeholder="选择讲解场次"
          v-model="orderInput.explainerTimePointId"
          :loading="loadingExplainer"
          :list="explainers"
          empty-text="所有场次已约满"
        />
        <van-field
          label="备注"
          type="textarea"
          placeholder="输入备注"
          rows="1"
          autosize
          :maxlength="200"
          v-model="orderInput.memo"
          clearable
        ></van-field>
      </van-cell-group>
    </van-panel>

    <van-panel title="联系人信息">
      <van-cell-group>
        <van-field
          label="姓名"
          :maxlength="10"
          placeholder="必填，输入联系人姓名"
          v-model="orderInput.contactName"
          clearable
        ></van-field>
        <van-field
          label="手机号"
          :maxlength="11"
          placeholder="必填，输入联系人手机号"
          type="tel"
          v-model="orderInput.contactMobile"
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
import MultiPickerField from "@/components/MultiPickerField.vue";
import dayjs from "dayjs";
import validate from "@/utils/validator.js";
import orderService from "@/services/orderService.js";
import { mapState, createNamespacedHelpers } from "vuex";
const { mapActions } = createNamespacedHelpers("orderModule");
import appConsts from "@/store/consts.js";
import explainerService from "@/services/explainerService.js";
import commonService from "@/services/commonService.js";

let defaultMinHour = 8;
let defaultArrivalTime = "08:00";

export default {
  components: {
    DateSelector,
    DateField,
    BottomButton,
    MultiPickerField
  },
  data() {
    return {
      loading: false,
      loadingExplainer: false,
      minHour: defaultMinHour,
      maxHour: 18,
      orderInput: {
        travelDate: "",
        personNum: 1,
        arrivalTime: defaultArrivalTime,
        licensePlateNumber: "",
        explainerTimePointId: "",
        contactName: "",
        contactMobile: "",
        keYuanTypeId: "",
        areaId: "",
        jidiaoName: "",
        memo: ""
      },
      ageRanges: [],
      explainers: [],
      keYuanTypes: [],
      areaItems: [],
      regions: [],
      regionId: "",
      areas: []
    };
  },
  computed: {
    groupOrderMaxQuantity() {
      if (this.options.groupOrderMaxQuantity > 0) {
        return this.options.groupOrderMaxQuantity;
      }
      return 1000;
    },
    showArea() {
      return this.areas.length > 0 && this.areas[0].children.length > 0;
    },
    ...mapState({
      member: state => state.memberModule.member,
      options: state => state.orderModule.options
    })
  },
  methods: {
    setRegions(keYuanTypeId) {
      this.regions = this.areaItems.filter(a => a.value == keYuanTypeId);
    },
    async onSubmit() {
      let selectedAgeRanges = this.ageRanges.filter(a => a.personNum > 0);
      if (selectedAgeRanges.length <= 0) {
        this.$toast("请输入参观人数");
        return;
      }
      this.orderInput.personNum = selectedAgeRanges.map(a => a.personNum).reduce((f, s) => s + f);
      if (this.orderInput.personNum > this.groupOrderMaxQuantity) {
        this.$toast(`每单最多${this.groupOrderMaxQuantity}人`);
        return;
      }
      this.orderInput.ageRanges = selectedAgeRanges.map(a => {
        return { ageRangeId: a.value, personNum: a.personNum };
      });

      if (!this.orderInput.areaId && this.regionId) {
        this.orderInput.areaId = this.regionId;
      }

      let validateRules = [
        {
          value: this.orderInput.keYuanTypeId,
          name: "客源类型",
          rules: [{ rule: "required" }]
        },
        {
          value: this.orderInput.areaId,
          name: "客源地",
          rules: [{ rule: "required" }]
        },
        {
          value: this.orderInput.contactName,
          name: "姓名",
          rules: [{ rule: "required" }]
        },
        {
          value: this.orderInput.contactMobile,
          name: "手机号",
          rules: [{ rule: "required" }, { rule: "isMobile" }]
        }
      ];
      let result = validate(validateRules);
      if (!result.success) {
        this.$toast(result.message);
        return;
      }

      try {
        this.loading = true;
        let response = await orderService.createGroupOrderAsync(this.orderInput);
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
    initTime() {
      let now = dayjs();
      this.minHour = now.hour();
      this.orderInput.arrivalTime = now.format("HH:mm");
    },
    ...mapActions([appConsts.loadOrderOptionsAsync])
  },
  watch: {
    "orderInput.travelDate": async function() {
      if (this.orderInput.travelDate == dayjs().toDateString()) {
        this.initTime();
      } else {
        this.minHour = defaultMinHour;
        this.orderInput.arrivalTime = defaultArrivalTime;
      }

      try {
        this.loadingExplainer = true;
        this.explainers = await explainerService.getSchedulingsAsync({
          Date: this.orderInput.travelDate
        });
      } finally {
        this.loadingExplainer = false;
      }
    },
    "orderInput.keYuanTypeId": function(val) {
      this.regionId = "";
      if (val == "") {
        this.regions = [
          {
            text: "",
            children: []
          }
        ];
      } else {
        this.setRegions(val);
      }
    },
    regionId: function(val) {
      this.orderInput.areaId = "";
      if (val == "") {
        this.areas = [
          {
            text: "",
            children: []
          }
        ];
      } else {
        this.areas = this.regions[0].children.filter(r => r.value == val);
      }
    }
  },
  created() {
    this.initTime();

    this.loadOrderOptionsAsync().then(() => {
      if (!this.orderInput.travelDate) {
        this.orderInput.travelDate = this.options.dates.filter(d => !d.disable)[0].date;
      }
    });

    commonService.getAgeRangeComboboxItemsAsync().then(items => {
      this.ageRanges = items.map(item => {
        return {
          value: item.value,
          displayText: item.displayText,
          personNum: 0
        };
      });
    });

    commonService.getTouristOriginTreeAsync().then(treeNode => {
      let keYuanTypeItems = treeNode.nodes.map(item => {
        return {
          value: item.value,
          text: item.displayText,
          disabled: item.disabled
        };
      });
      this.keYuanTypes = [
        {
          text: "客源类型",
          children: keYuanTypeItems
        }
      ];

      this.areaItems = treeNode.nodes.map(keYuan => {
        return {
          value: keYuan.value,
          children: keYuan.nodes.map(region => {
            return {
              text: region.displayText,
              value: region.value,
              disabled: region.disabled,
              children: region.nodes.map(area => {
                return {
                  text: area.displayText,
                  value: area.value,
                  disabled: area.disabled
                };
              })
            };
          })
        };
      });

      this.setRegions(keYuanTypeItems[0].value);
    });
  }
};
</script>

<style lang="scss">
.group-order {
  .van-hairline--top-bottom::after {
    border-width: 0;
  }

  .van-cell__value {
    text-align: left;
  }

  &-person {
    .van-cell {
      padding: 3px 15px;
    }

    .van-cell:not(:last-child)::after {
      border: none;
    }
  }

  &-tips {
    padding: 5px 10px;
    margin-top: 7px;
    margin-bottom: 10px;
    font-size: 12px;
    background-color: rgb(245, 249, 248);

    span {
      padding-left: 5px;
    }
  }
}

.inline-cell {
  .van-cell {
    padding: 0;
  }

  .van-hairline--bottom::after {
    border-bottom-width: 0;
  }

  .van-cell:not(:last-child)::after {
    border-bottom-width: 0;
  }
}
</style>
