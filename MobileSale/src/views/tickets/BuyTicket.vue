<template>
  <div v-if="pageLoaded" ref="grbody" :class="{'booking': !showTourist}">
    <!-- <van-popup v-if="ticketType.touristInfoType == 3" v-model="showTourist" position="right"> -->
    <div v-if="ticketType.touristInfoType == 3 && showTourist" class="booking-tourist-edit">
      <van-nav-bar
        :title="`${editTouristTitle}出行人`"
        left-text="返回"
        left-arrow
        @click-left="showTourist = false"
      />
      <div class="booking-tourist-edit-content">
        <van-cell-group :border="false" style="border-radius: 5px;">
          <van-field
            v-if="ticketType.needTouristName"
            v-model="editTourist.name"
            clearable
            label="姓名"
            placeholder="须与证件上的名字一致"
          />
          <van-field
            v-if="ticketType.needTouristMobile"
            v-model="editTourist.mobile"
            type="tel"
            clearable
            label="手机号码"
            placeholder="请填写手机号码"
          />
          <picker-field
            v-if="ticketType.needCert && wxTouristNeedCertTypeFlag"
            :columns="certTypes"
            label="证件类型"
            v-model="editTourist.certTypeId"
            placeholder="请选择证件类型"
            class="changCi"
          />
          <van-field
            v-if="ticketType.needCert"
            v-model="editTourist.certNo"
            clearable
            label="证件号码"
            placeholder="请填写证件号码"
          />
        </van-cell-group>
        <div class="booking-tourist-edit-btnbox">
          <van-button @click="onTouristSave">完成</van-button>
        </div>
      </div>
    </div>
    <!-- </van-popup> -->
    
    <div v-if="!showTourist">
      <div class="booking-mod">
        <div class="booking-mod-hd">
          <span class="booking-mod-hd-title">使用日期</span>
        </div>
        <div class="booking-mod-bd" style="padding: 10px 15px;">
          <date-selector v-model="input.travelDate" :dates="ticketType.dailyPrices"></date-selector>
        </div>
      </div>

      <div class="booking-mod">
        <div class="booking-mod-around">
          <div class="booking-multi-item">
            <div class="booking-multi-item-auto">
              <h3>{{ ticketType.name }}</h3>
              <ul class="booking-mod-around-label clearfix">
                <li :class="refundClass">
                  <van-icon :name="refundIcon" />
                  <span>{{ refundText }}</span>
                </li>
                <li class="info">
                  <van-icon name="passed" />
                  <span>入园保障</span>
                </li>
              </ul>
              <p @click="showDescription = true" class="booking-mod-around-resinfo">购买须知></p>
              <p
                v-if="ticketType.minBuyNum > 1"
                class="booking-mod-around-limited"
              >至少需购买{{ ticketType.minBuyNum }}份</p>
            </div>
            <div>
              <div class="booking-mod-around-price">
                <dfn>¥</dfn>
                <i>{{ price }}</i>
              </div>
              <div class="booking-mod-around-num">
                <van-stepper
                  v-model="quantity"
                  :min="minBuyNum"
                  :max="maxBuyNum"
                  integer
                  @change="onQuantityChange"
                  @overlimit="onOverlimit"
                />
              </div>
              <div v-if="stockNum" class="booking-stock">余：{{stockNum}}</div>
            </div>
          </div>
          <p
            v-if="false"
            class="booking-mod-around-tips"
          >{{pageLabelMainText}}统一限购，同一手机号1天内在所有网络平台最多只能预订5份。如您需要重复购买，请使用其他手机号购买。</p>
        </div>
      </div>

      <div v-if="ticketType.groundChangCis.length > 0" class="booking-mod">
        <div class="booking-mod-hd">
          <span class="booking-mod-hd-title">场次信息</span>
        </div>
        <div class="booking-mod-bd">
          <van-cell-group>
            <!-- <picker-field
            v-for="groundChangCi in ticketType.groundChangCis"
            :key="groundChangCi.groundId"
            :columns="groundChangCi.changCis"
            :label="groundChangCi.groundName"
            v-model="groundChangCi.changCiId"
            placeholder="请选择场次"
            class="changCi"
            />-->
            <div v-for="groundChangCi in ticketType.groundChangCis" :key="groundChangCi.groundId">
              <select-list
                :groundChangCi="groundChangCi"
                v-model="groundChangCi.changCiId"
                @change="onChangCiChange()"
              ></select-list>
            </div>
          </van-cell-group>
        </div>
      </div>

      <div v-if="ticketType.touristInfoType == 3" class="booking-mod">
        <div class="booking-mod-hd">
          <span class="booking-mod-hd-title">出行人信息</span>
          <span class="booking-mod-hd-tips">
            需填写
            <em>{{ quantity }}个</em>出行人
          </span>
        </div>
        <div class="booking-mod-bd">
          <van-cell-group v-show="quantity == 1">
            <van-field
              v-if="ticketType.needTouristName"
              v-model="firstTourist.name"
              clearable
              label="姓名"
              placeholder="须与证件上的名字一致"
            />
            <van-field
              v-if="ticketType.needTouristMobile"
              v-model="firstTourist.mobile"
              clearable
              label="手机号码"
              placeholder="请填写手机号码"
            />
            <picker-field
              v-if="ticketType.needCert && wxTouristNeedCertTypeFlag"
              :columns="certTypes"
              label="证件类型"
              v-model="firstTourist.certTypeId"
              placeholder="请选择证件类型"
              class="changCi"
            />
            <van-field
              v-if="ticketType.needCert"
              v-model="firstTourist.certNo"
              clearable
              label="证件号码"
              placeholder="请填写证件号码"
            />
          </van-cell-group>
          <div v-show="quantity > 1" class="booking-tourist">
            <div style="width:75px;">出行人</div>
            <div style="flex:1;">
              <van-cell-group :border="false">
                <template v-for="(tourist, index) in tourists">
                  <van-cell
                    v-if="tourist.name || tourist.mobile || tourist.certNo"
                    :key="index"
                    :class="{
                    'booking-tourist-error': errorTouristIndex == index
                  }"
                    style="align-items: center;"
                    @click="onTouristEdit(index)"
                  >
                    <p v-show="tourist.name" class="booking-tourist-item">{{ tourist.name }}</p>
                    <p
                      v-show="tourist.mobile"
                      class="booking-tourist-item"
                    >手机号码 {{ tourist.mobile }}</p>
                    <p
                      v-show="tourist.certNo"
                      class="booking-tourist-item"
                    >{{computCertTypeName(tourist.certTypeId)}} {{ tourist.certNo }}</p>
                    <van-icon slot="right-icon" name="edit" />
                  </van-cell>
                  <van-cell
                    v-else
                    :key="index"
                    :title="`点击添加出行人${index + 1}`"
                    :class="{
                    'booking-tourist-error': errorTouristIndex == index
                  }"
                    @click="onTouristEdit(index)"
                  >
                    <van-icon slot="right-icon" name="edit" />
                  </van-cell>
                </template>
              </van-cell-group>
            </div>
          </div>
        </div>
      </div>

      <div v-if="ticketType.touristInfoType == 3 && !ticketType.needTouristMobile">
        <div class="booking-mod-hd">
          <span class="booking-mod-hd-title">联系信息</span>
        </div>
        <div class="booking-mod-bd">
          <van-cell-group>
            <van-field
              v-model="input.contactMobile"
              type="tel"
              clearable
              label="联系手机"
              placeholder="请填写手机号码"
            />
          </van-cell-group>
        </div>
      </div>

      <div v-if="ticketType.touristInfoType == 2" class="booking-mod">
        <div class="booking-mod-hd">
          <span class="booking-mod-hd-title">联系信息</span>
          <span class="booking-mod-hd-tips">
            需填写
            <em>1个</em>联系人
          </span>
        </div>
        <div class="booking-mod-bd">
          <van-cell-group>
            <van-field
              v-if="ticketType.needTouristName"
              v-model="input.contactName"
              clearable
              label="姓名"
              placeholder="请填写姓名"
            />
            <van-field
              v-if="ticketType.needTouristMobile"
              v-model="input.contactMobile"
              type="tel"
              clearable
              label="联系手机"
              placeholder="请填写手机号码"
            />
            <picker-field
              v-if="ticketType.needCert && wxTouristNeedCertTypeFlag"
              :columns="certTypes"
              label="证件类型"
              v-model="input.contactCertTypeId"
              placeholder="请选择证件类型"
              class="changCi"
            />
            <van-field
              v-if="ticketType.needCert"
              v-model="input.contactCert"
              clearable
              label="证件号码"
              placeholder="请填写证件号码"
            />
          </van-cell-group>
        </div>
      </div>

      <van-submit-bar
        :loading="saving"
        :price="totalMoney"
        label="总额："
        :button-text="submitText"
        @submit="onSubmit"
      />

      <ticket-type-description
        v-model="showDescription"
        :show-buy="false"
        :ticket-type-id="ticketType.id"
        :ticket-type-name="ticketType.name"
        :price="price"
      />

      <van-popup v-if="shouldRegistMember" v-model="showMember" position="right">
        <div class="booking-tourist-edit">
          <van-nav-bar title="注册会员" left-text="返回" left-arrow @click-left="showMember = false" />
          <div class="booking-tourist-edit-content">
            <van-cell-group :border="false" style="border-radius: 5px;">
              <van-field v-model="member.name" clearable label="姓名" placeholder="请输入" />
              <van-field
                v-model="member.mobile"
                type="tel"
                clearable
                label="手机号码"
                placeholder="请输入"
              />
            </van-cell-group>
            <div class="booking-tourist-edit-btnbox">
              <van-button @click="onRegistMember">注册</van-button>
            </div>
          </div>
        </div>
      </van-popup>
    </div>
  </div>
</template>

<script>
import { mapMutations, mapState } from "vuex";
import md5 from "md5";
import DateSelector from "@/components/DateSelector.vue";
import TicketTypeDescription from "./TicketTypeDescription.vue";
import memberService from "../../services/memberService.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import orderService from "./../../services/orderService.js";
import scenicService from "@/services/scenicService.js";
import commonService from "@/services/commonService.js";
import validate from "@/utils/validator.js";
import appConsts from "@/store/consts.js";
import { keyboardPopupMixin } from "./../../mixins/KeyboardPopupMixin.js";
import { mobileMixin } from "./../../mixins/mobileMixin.js";

class Tourist {
  constructor() {
    this.name = "";
    this.mobile = "";
    this.certNo = "";
    this.certTypeId = 5;
  }
}

export default {
  mixins: [keyboardPopupMixin, mobileMixin],
  components: {
    DateSelector,
    TicketTypeDescription
  },
  props: {
    ticketTypeId: {
      type: [Number, String],
      default: 0
    }
  },
  data() {
    return {
      input: {
        items: [
          {
            ticketTypeId: this.ticketTypeId,
            quantity: 1
          }
        ],
        travelDate: "",
        contactName: "",
        contactMobile: "",
        contactCert: "",
        contactCertTypeId: 5
      },
      quantity: 1,
      tourists: [],
      ticketType: {
        dailyPrices: [],
        groundChangCis: []
      },
      minBuyNum: 1,
      maxBuyNum: 1000,
      showDescription: false,
      saving: false,
      submited: false,
      showTourist: false,
      firstTourist: new Tourist(),
      currentTouristIndex: -1,
      editTourist: {},
      errorTouristIndex: -1,
      showMember: false,
      member: new Tourist(),
      stockNum: "",
      certTypes: [],
      wxTouristNeedCertTypeFlag: false
    };
  },
  computed: {
    refundClass() {
      let className = "info";
      if (this.ticketType.allowRefund === false) {
        className = "error";
      } else if (this.ticketType.refundLimited) {
        className = "warning";
      }
      let classObj = {};
      classObj[className] = true;
      return classObj;
    },
    refundIcon() {
      if (this.ticketType.allowRefund === false) {
        return "close";
      }
      return this.ticketType.refundLimited ? "info-o" : "passed";
    },
    refundText() {
      if (this.ticketType.allowRefund === false) {
        return "不可退";
      }
      return this.ticketType.refundLimited ? "有条件退" : "随时退";
    },
    price() {
      const priceObj = this.ticketType.dailyPrices.find(p => p.date == this.input.travelDate);
      return priceObj ? priceObj.price : 0;
    },
    totalMoney() {
      return this.price * this.quantity * 100;
    },
    submitText() {
      return this.totalMoney > 0 ? "去支付" : "提交订单";
    },
    editTouristTitle() {
      if (this.editTourist.name || this.editTourist.mobile || this.editTourist.certNo) {
        return "编辑";
      }
      return "新增";
    },
    shouldRegistMember() {
      const member = memberService.getMember();
      return this.ticketType.ticketTypeType == 3 && !member.mobile;
    },
    ...mapState(["promoterId", "pageLabelMainText"]),
    ...mapState("orderModule", ["signKey"])
  },
  watch: {
    "input.travelDate": async function(val, oldVal) {
      if (!oldVal) {
        return;
      }

      this.getTicketTypeMaxBuyNum();

      if (!this.ticketType.hasGroundChangCi) {
        return;
      }

      try {
        this.loading();

        this.ticketType.groundChangCis = await ticketTypeService.getGroundChangCisDtosVariedAsync(
          this.ticketTypeId,
          val
        );
        this.setGroundChangCiDefaultValue();
      } finally {
        this.loaded();
      }
    },
    quantity: function(val, oldVal) {
      if (this.ticketType.touristInfoType != 3) return;

      const diff = val - oldVal;
      if (diff > 0) {
        for (let i = 0; i < diff; i++) {
          this.tourists.push(new Tourist());
        }
        if (val == 1) {
          this.firstTourist = this.tourists[0];
        }
      } else {
        this.tourists.length += diff;
      }
    },
    showTourist(val){
      if(val){
        this.setShowNavbar(false);
      } else {
        this.setShowNavbar(true);
      }
    }
  },
  async created() {
    try {
      this.loading();

      this.ticketType = await ticketTypeService.getTicketTypeForWeiXinSaleAsync(this.ticketTypeId);
      if (!this.ticketType.minBuyNum) {
        this.ticketType.minBuyNum = 1;
      }
      if (!this.ticketType.maxBuyNum) {
        this.ticketType.maxBuyNum = 1000;
      }
      this.certTypes = await commonService.getCertTypeComboBoxItemsAsync();
      this.certTypes = this.certTypes.filter(a => a.value != 0);
      this.wxTouristNeedCertTypeFlag = await scenicService.getWxTouristNeedCertTypeFlag();

      this.minBuyNum = this.ticketType.minBuyNum;
      this.maxBuyNum = this.ticketType.maxBuyNum;
      this.quantity = this.minBuyNum;
      const priceObj = this.ticketType.dailyPrices.find(p => !p.disable);
      if (priceObj) {
        this.input.travelDate = priceObj.date;
        if (priceObj.stock) {
          this.maxBuyNum = Math.min(priceObj.stock, priceObj.memberSurplusNum);
          this.stockNum = priceObj.stock;
        }
      }

      this.setGroundChangCiDefaultValue();

      if (this.ticketType.touristInfoType == 3) {
        this.tourists.push(this.firstTourist);
      }

      this.pageLoaded = true;
    } finally {
      this.loaded();
    }
  },
  beforeRouteLeave(to, from, next) {
    if (this.showTourist) {
      this.showTourist = false;
      next(false);
      return;
    }

    if (this.submited || to.meta.shouldNotConfirm) {
      this.$toast.clear();
      next();
      return;
    }

    this.$dialog
      .confirm({
        title: "确认离开订单填写页？",
        showCancelButton: true,
        confirmButtonText: "离开",
        cancelButtonText: "取消"
      })
      .then(() => {
        this.$toast.clear();
        next();
      })
      .catch(() => {
        next(false);
      });
  },
  methods: {
    onQuantityChange(val) {
      if (val && val < this.minBuyNum) {
        this.$nextTick(() => {
          this.quantity = this.minBuyNum;
          this.$toast(`至少需购买${this.minBuyNum}份`);
        });
      } else if (val > this.maxBuyNum) {
        this.$nextTick(() => {
          this.quantity = this.maxBuyNum;
          this.$toast(`最多购买${this.maxBuyNum}份`);
        });
      }
      this.setChangCiDisabled();
    },
    onOverlimit(val) {
      if (val == "plus") {
        const priceObj = this.ticketType.dailyPrices.find(p => p.date == this.input.travelDate);
        if (priceObj && !priceObj.disable && priceObj.stock) {
          if (priceObj.stock > priceObj.memberSurplusNum) {
            this.$toast(`您当前剩余可购数量为${priceObj.memberSurplusNum}`);
          }
        }
      }
    },
    onTouristEdit(index) {
      this.currentTouristIndex = index;
      const tourist = this.tourists[index];
      this.editTourist = { ...tourist };
      this.showTourist = true;
    },
    onTouristSave() {
      if (!this.validateTourist(this.editTourist)) return;

      if (this.ticketType.needCert) {
        const index = this.tourists.findIndex(t => t.certNo == this.editTourist.certNo);
        if (index >= 0 && index != this.currentTouristIndex) {
          this.$toast("证件号码已添加");
          return;
        }
      }

      this.tourists[this.currentTouristIndex] = { ...this.editTourist };
      if (this.currentTouristIndex == 0) {
        this.firstTourist = this.tourists[0];
      }
      this.showTourist = false;
    },
    async onRegistMember() {
      const rules = [
        {
          value: this.member.name,
          name: "姓名",
          rules: [{ rule: "required" }]
        },
        {
          value: this.member.mobile,
          name: "手机号码",
          rules: [{ rule: "required" }, { rule: "isMobile" }]
        }
      ];
      const validateResult = validate(rules);
      if (!validateResult.success) {
        this.$toast(validateResult.message);
        return;
      }

      await memberService.registWechatMemberAsync(this.member);
      let member = memberService.getMember();
      member.mobile = this.member.mobile;
      localStorage.setItem("member", JSON.stringify(member));
      this.showMember = false;
      await this.createOrder();
    },
    async onSubmit() {
      let errorMsg = "";
      if (this.ticketType.groundChangCis.length > 0) {
        for (const groundChangCi of this.ticketType.groundChangCis) {
          if (!groundChangCi.changCiId) {
            if (groundChangCi.changCiDtos.length === 1 && groundChangCi.changCiDtos[0].disabled) {
              errorMsg += `${groundChangCi.groundName}暂无可售场次，`;
            } else {
              errorMsg += `请选择${groundChangCi.groundName}场次，`;
            }
          } else {
            let changCiDto = groundChangCi.changCiDtos.find(a => a.id == groundChangCi.changCiId);
            if (changCiDto.surplusNum < this.quantity) {
              errorMsg += `${groundChangCi.groundName}场次剩余数量不足，`;
            }
          }
        }
      }

      if (errorMsg) {
        this.$toast(errorMsg);
        return;
      }

      if (this.ticketType.touristInfoType == 3) {
        for (let i = 0; i < this.tourists.length; i++) {
          const tourist = this.tourists[i];
          if (!this.validateTourist(tourist)) {
            this.errorTouristIndex = i;
            return;
          }
        }
        this.errorTouristIndex = -1;

        if (!this.ticketType.needTouristMobile) {
          const validateRules = [
            {
              value: this.input.contactMobile,
              name: "联系手机",
              rules: [{ rule: "required" }, { rule: "isMobile" }]
            }
          ];
          let validateResult = validate(validateRules);
          if (!validateResult.success) {
            this.$toast(validateResult.message);
            return;
          }
        }
      }

      if (this.ticketType.touristInfoType == 2) {
        let errorMsg = "";
        const validateRules = [];
        if (this.ticketType.needTouristName) {
          validateRules.push({
            value: this.input.contactName,
            name: "姓名",
            rules: [{ rule: "required" }]
          });
        }
        if (this.ticketType.needTouristMobile) {
          validateRules.push({
            value: this.input.contactMobile,
            name: "联系手机",
            rules: [{ rule: "required" }, { rule: "isMobile" }]
          });
        }
        if (this.ticketType.needCert) {
          if (this.wxTouristNeedCertTypeFlag) {
            validateRules.push({
              value: this.input.contactCertTypeId,
              name: "证件类型",
              rules: [{ rule: "required" }]
            });
          }
          let certNoRules = [{ rule: "required" }];
          if (
            !this.wxTouristNeedCertTypeFlag ||
            (this.wxTouristNeedCertTypeFlag && this.input.contactCertTypeId == 5)
          ) {
            certNoRules.push({ rule: "isIdCard" });
          }
          validateRules.push({
            value: this.input.contactCert,
            name: "证件号码",
            rules: certNoRules
          });
          if (this.input.contactCert.length < 7) {
            errorMsg += "证件号码格式不正确";
          }
        }
        if (!errorMsg) {
          let validateResult = validate(validateRules);
          if (!validateResult.success) {
            errorMsg += validateResult.message;
          }
        }

        if (errorMsg) {
          this.$toast(errorMsg);
          return;
        }
      }

      this.quantity = Math.max(this.minBuyNum, this.quantity);
      this.quantity = Math.min(this.maxBuyNum, this.quantity);

      if (this.shouldRegistMember) {
        this.showMember = true;
      } else {
        await this.createOrder();
      }
    },
    setGroundChangCiDefaultValue() {
      if (!this.ticketType.groundChangCis || this.ticketType.groundChangCis.length <= 0) {
        return;
      }

      let changeChangCi = false;
      this.ticketType.groundChangCis.forEach(groundChangCi => {
        if (groundChangCi.changCis.length == 0) {
          groundChangCi.changCis.push({
            value: "",
            displayText: "暂无可售场次",
            disabled: true
          });
        } else if (groundChangCi.changCis.length == 1) {
          groundChangCi.changCiId = groundChangCi.changCis[0].value;
          changeChangCi = true;
        }
      });

      if(changeChangCi){
        this.onChangCiChange();
      }
    },
    validateTourist(tourist) {
      let errorMsg = "";
      const rules = [];
      if (this.ticketType.needTouristName) {
        rules.push({
          value: tourist.name,
          name: "姓名",
          rules: [{ rule: "required" }]
        });
      }
      if (this.ticketType.needTouristMobile) {
        rules.push({
          value: tourist.mobile,
          name: "手机号码",
          rules: [{ rule: "required" }, { rule: "isMobile" }]
        });
      }
      if (this.ticketType.needCert) {
        if (this.wxTouristNeedCertTypeFlag) {
          rules.push({
            value: tourist.certTypeId,
            name: "证件类型",
            rules: [{ rule: "required" }]
          });
        }
        let certNoRules = [{ rule: "required" }];
        if (
          !this.wxTouristNeedCertTypeFlag ||
          (this.wxTouristNeedCertTypeFlag && tourist.certTypeId == 5)
        ) {
          certNoRules.push({ rule: "isIdCard" });
        }
        rules.push({
          value: tourist.certNo,
          name: "证件号码",
          rules: certNoRules
        });
        if (tourist.certNo.length < 7) {
          errorMsg += "证件号码格式不正确";
        }
      }
      if (!errorMsg) {
        let validateResult = validate(rules);
        if (!validateResult.success) {
          errorMsg += validateResult.message;
        }
      }

      if (errorMsg) {
        this.$toast(errorMsg);
        return false;
      }

      return true;
    },
    async createOrder() {
      try {
        this.input.items[0].quantity = this.quantity;
        this.input.items[0].tourists = this.tourists;
        this.input.items[0].groundChangCis = this.ticketType.groundChangCis.map(g => {
          return { groundId: g.groundId, changCiId: g.changCiId };
        });
        this.input.promoterId = this.promoterId;
        this.input.sign = md5(
          `${this.input.travelDate}${this.input.items[0].ticketTypeId}${this.input.items[0].quantity}${this.signKey}`
        );

        this.saving = true;
        const result = await orderService.createWeiXinOrderAsync(this.input);
        this.submited = true;
        if (result.shouldPay) {
          localStorage.setItem("Pay:Product", this.ticketType.name);
        }
        const routeName = result.shouldPay ? "wxjspay" : "orderdetail";
        this.$router.push({
          name: routeName,
          params: { listNo: result.listNo }
        });
      } finally {
        this.saving = false;
      }
    },
    computCertTypeName(certTypeId) {
      const certType = this.certTypes.find(a => a.value == certTypeId);
      const certTypeName = certType ? certType.displayText : "";
      return certTypeName;
    },
    onChangCiChange() {
      let changCiMinSurplusNum = 100000;
      this.ticketType.groundChangCis.forEach((groundChangCi) => {
        let changCiDto = groundChangCi.changCiDtos.find(a=>a.id == groundChangCi.changCiId);
        if(changCiDto && changCiDto.surplusNum < changCiMinSurplusNum){
          changCiMinSurplusNum = changCiDto.surplusNum;
        }
      })
      
      if(changCiMinSurplusNum < 100){
       this.maxBuyNum = changCiMinSurplusNum;
      } else {
        this.getTicketTypeMaxBuyNum();
      }
    },
    getTicketTypeMaxBuyNum(){
      this.maxBuyNum = 1000;
      const priceObj = this.ticketType.dailyPrices.find(p => p.date == this.input.travelDate);
      if (priceObj && !priceObj.disable && priceObj.stock) {
        this.maxBuyNum = Math.min(priceObj.stock, priceObj.memberSurplusNum);
        this.quantity = Math.min(this.quantity, this.maxBuyNum);
        this.stockNum = priceObj.stock;
      }
    },
    setChangCiDisabled(){
      this.ticketType.groundChangCis.forEach((groundChangCi) => {
        groundChangCi.changCiDtos.forEach((changCiDto) => {
          if(changCiDto.surplusNum < this.quantity){
            changCiDto.disabled = true;
          } else {
            changCiDto.disabled = false;
          }
        })
      })
    },
    ...mapMutations([appConsts.setShowNavbar]),
    ...mapState(["showNavbar"])
  }
};
</script>

<style lang="scss" scoped>
.booking {
  padding: 10px 6px 70px;
  background-color: #c0c7cf;
  min-height: calc(100vh - 46px);
  box-sizing: border-box;

  &-mod {
    margin-bottom: 10px;

    &-hd {
      padding: 10px 12px;
      border-radius: 5px 5px 0 0;
      background: #eff0f2;

      &-title {
        margin-right: 5px;
        font-size: 16px;
        line-height: 20px;
        color: #000;
      }

      &-tips {
        display: inline-block;
        vertical-align: middle;
        line-height: 18px;
        font-size: 13px;
        color: #999;
        word-break: break-all;

        em {
          color: #f40;
          font-style: normal;
          font-weight: 400;
        }
      }
    }

    &-bd {
      border-radius: 0 0 5px 5px;
      border-bottom: solid 1px #a3aab2;
      overflow: hidden;
      background: #fff;

      .van-cell {
        padding: 13px 15px;
      }
    }

    &-around {
      border-radius: 5px;
      background-color: #fff;
      padding: 15px;

      h3 {
        font-size: 16px;
        color: #333;
        font-weight: 700;
      }

      &-label {
        margin-top: 3px;
        font-size: 12px;
        height: 20px;
        overflow: hidden;

        li {
          float: left;
          margin-right: 14px;
          line-height: 20px;

          i {
            vertical-align: middle;
            margin-right: 2px;
          }

          span {
            display: inline-block;
            vertical-align: middle;
          }
        }

        .warning {
          color: #ffae13;
          i {
            color: #ffae13;
          }
        }

        .info {
          color: #999;
          i {
            color: #19a0f0;
          }
        }

        .error {
          color: #ff2f39;
          i {
            color: #ff2f39;
          }
        }
      }

      &-resinfo {
        font-size: 12px;
        color: #19a0f0;
        line-height: 20px;
      }

      &-limited {
        font-size: 12px;
        color: #999;
        line-height: 20px;
      }

      &-price {
        text-align: right;
        color: #f40;

        dfn {
          font-size: 15px;
        }

        i {
          font-size: 21px;
          margin-right: 1px;
          line-height: 21px;
          font-style: normal;
          font-weight: 400;
        }
      }

      &-num {
        .van-stepper {
          display: flex;
        }

        /deep/ .van-stepper__input {
          margin: 0;
          height: 33px;
        }
      }

      &-tips {
        margin: 5px -15px 0 -15px;
        padding: 10px 15px;
        background-color: #fff7e0;
        font-size: 12px;
        color: #481a03;
      }
    }
  }

  &-multi {
    &-item {
      display: flex;
      justify-content: space-between;
      align-items: center;

      &-auto {
        flex: 1;
      }
    }
  }

  .changCi {
    /deep/ .van-field .van-cell__title {
      max-width: 40%;
    }
  }

  &-tourist {
    display: flex;
    align-items: center;
    margin-left: 15px;

    .van-cell__title {
      color: #999;
    }

    .van-cell__value {
      line-height: 16px;
    }

    .van-icon {
      color: #19a0f0;
    }

    &-error {
      background-color: #fffdf2;

      .booking-tourist-item,
      .van-cell__title {
        color: #ff2f39;
      }
    }

    &-item {
      display: inline-block;
      vertical-align: middle;
      width: 100%;
      font-size: 14px;
      color: #333;
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    }

    &-edit {
      width: 100vw;
      height: 100vh;
      background-color: #c0c7cf;
      // position: absolute;
      // z-index: 1001;
      // top: 0;
      // left: 0;

      &-content {
        padding: 10px 6px;
        overflow-y: auto;
        height: calc(100vh - 46px);
      }

      &-btnbox {
        padding: 10px;

        .van-button {
          width: 100%;
          line-height: 44px;
          font-size: 18px;
          font-weight: 700;
          color: #fff;
          background-color: #ff9a14;
          border: 1px solid #ff9a14;
          letter-spacing: 1px;
          border-radius: 5px;
        }
      }
    }
  }
}

.top-radius {
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}

.bottom-radius {
  border-bottom-left-radius: 5px;
  border-bottom-right-radius: 5px;
}

.booking-stock {
  text-align: right;
  color: #999;
  font-size: 10px;
}
</style>
