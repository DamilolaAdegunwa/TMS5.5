<template>
  <div class="page">
    <nav-bar title="售票管理" back="/home" />
    <sale-tabs :active="0" />

    <ul class="category-list">
      <li
        v-for="(category, index) in tickets"
        :key="category.name"
        :class="{ active: selectedCategory === index }"
        @click="selectCategory(index)"
      >
        {{ category.name }}
      </li>
    </ul>

    <ul class="item-list" v-if="tickets[selectedCategory]">
      <li v-for="item in tickets[selectedCategory].items" :key="item.name">
        <van-row gutter="0" type="flex" align="center" justify="space-between">
          <div>
            <p class="item-name">{{ item.name }}</p>
            <p class="item-price">{{ `￥${item.price.toFixed(2)}` }}</p>
          </div>
          <stepper-button
            v-model="item.quantity"
            :defaultValue="0"
            :minValue="0"
            :maxValue="100"
          />
        </van-row>
      </li>
    </ul>

    <!-- 用 "inactive" class 切换至无商品灰色状态 -->
    <div class="bottom-bar">
      <div class="cart-icon" @click="toggleCart">
        <font-awesome-icon icon="shopping-cart" fixed-width />
        <span class="cart-icon-badge" v-if="!showCart">{{ sumQuantity }}</span>
      </div>
      <span class="summary-money">¥ {{ summaryMoney }}</span>
      <button class="checkout-button" @click="gotoCheckout">去结算</button>
    </div>

    <div class="cart" :class="{ show: showCart }">
      <div class="cart-title">
        <span>已选商品：{{ sumQuantity }}</span>
        <button class="clear-button" @click="emptyCart">
          <font-awesome-icon icon="trash-alt" />清空购物车
        </button>
      </div>
      <div class="ul">
        <van-cell-group>
          <van-cell v-for="item in cart" :key="item.name">
            <van-row class="van-row">
              <van-col span="12">{{ item.name }}</van-col>
              <van-col class="item-price" span="6">{{
                `￥${item.price.toFixed(2)}`
              }}</van-col>
              <van-col span="6">
                <stepper-button
                  v-model="item.quantity"
                  :defaultValue="0"
                  :minValue="0"
                  :maxValue="100"
                />
              </van-col>
            </van-row>
          </van-cell>
        </van-cell-group>
      </div>
    </div>

    <div class="cart-overlay" :class="{ show: showCart }" @click="toggleCart" />
  </div>
</template>

<script>
import NavBar from "../../components/NavBar";
import SaleTabs from "./SaleTabs";
import StepperButton from "../../components/StepperButton";
import ticketTypeService from "../../services/ticketTypeService.js";
import orderService from "../../services/orderService.js";
import math from "mathjs";
import { App } from "../../handset-sdk.js";
import { isNullOrUndefined } from "util";
import moment from "moment";
import md5 from "md5";
import { mapState } from "vuex";

class Input {
  constructor() {
    this.keyWord = "";
    this.staffId = 0;
    this.salePointId = 0;
  }
}

class OrderInput {
  constructor() {
    this.items = [
      {
        ticketTypeId: 0,
        quantity: 0,
        groundChangCis: []
      }
    ];
    this.travelDate = "";
    this.cashierId = 0;
    this.cashPcid = 0;
    this.salePointId = 0;
    this.parkId = 0;
  }
}

export default {
  name: "Sale",
  components: { SaleTabs, NavBar, StepperButton },
  created() {
    this.loadData();
  },
  data() {
    return {
      input: new Input(),
      orderInput: new OrderInput(),
      selectedCategory: 0,
      tickets: [],
      ticketsSources: [],
      showCart: false,
      cart: [],
      sumQuantity: 0,
      order: {},
      orderList: [],
      orderItem: {}
    };
  },
  computed: {
    summaryMoney: {
      // getter
      get: function() {
        let sumMoney = 0.0;
        this.ticketsSources.forEach(item => {
          item.money = Number((item.price * item.quantity).toFixed(2));
          sumMoney = math.add(sumMoney, item.money);
        });

        return sumMoney.toFixed(2);
      }
    },
    ...mapState(["signKey"])
  },
  watch: {
    tickets: {
      handler: function() {
        this.addCart();
      },
      deep: true
    }
  },
  methods: {
    loadData() {
      let staffId = this.getStaffId();
      let parkId = this.getParkId();
      let salePointId = this.getSalePointId();
      let pcId = this.getPcId();

      this.input.keyWord = "";
      this.input.staffId = staffId;
      this.input.salePointId = salePointId;

      this.orderInput.cashierId = staffId;
      this.orderInput.cashPcid = pcId;
      this.orderInput.salePointId = salePointId;
      this.orderInput.parkId = parkId;

      this.getTicketTypes();
      this.cart = [];
    },
    getParkId() {
      let parkId = sessionStorage.getItem("parkId");
      return Number(parkId);
    },
    getStaffId() {
      let staffId = sessionStorage.getItem("staffId");
      return Number(staffId);
    },
    getSalePointId() {
      if (App) {
        let result = App.getValue("salePointId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else if (window.localStorage) {
        let result = localStorage.getItem("salePointId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    getPcId() {
      if (App) {
        let result = App.getValue("pcId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else if (window.localStorage) {
        let result = localStorage.getItem("pcId");
        if (isNullOrUndefined(result)) {
          return 0;
        } else {
          return Number(result == "" ? "0" : result);
        }
      } else {
        return 0;
      }
    },
    async getTicketTypes() {
      try {
        let response = await ticketTypeService.getTicketTypesForLocalSaleAsync(
          this.input
        );
        this.ticketsSources = response.result;
        let ticketTypes = [];

        this.ticketsSources.forEach(ticket => {
          this.$set(ticket, "quantity", 0);
          this.$set(ticket, "money", 0.0);

          if (ticket.classes.length == 0) {
            ticket.classes.push("其它");
          }

          ticket.classes.forEach(classe => {
            let pushed = false;
            try {
              ticketTypes.forEach(ticketType => {
                if (ticketType.name == classe) {
                  pushed = true;
                  ticketType.items.push(ticket);
                  throw new Error("StopIteration");
                }
              });
            } catch (error) {
              if (error.message != "StopIteration") throw error;
            }

            if (!pushed) {
              ticketTypes.push({ name: classe, items: [ticket] });
            }
          });
        });
        this.tickets = ticketTypes;
      } catch (err) {
        this.$toast.clear();
        this.$toast.fail(err.message);
      }
    },

    addCart() {
      this.cart = [];
      this.sumQuantity = 0;
      this.ticketsSources.forEach(item => {
        if (item.quantity > 0) {
          this.sumQuantity = math.add(this.sumQuantity, item.quantity);
          this.cart.push(item);
        }
      });
    },
    emptyCart() {
      this.ticketsSources.forEach(item => {
        item.quantity = 0;
      });
    },
    selectCategory(index) {
      this.selectedCategory = index;
    },
    toggleCart() {
      this.showCart = !this.showCart;
    },
    createOrder() {},
    async gotoCheckout() {
      if (this.sumQuantity > 0) {
        //获取订单信息
        this.orderInput.travelDate = moment().format("YYYY-MM-DD");
        this.orderInput.items = [];
        this.cart.forEach(ticket => {
          this.orderInput.items.push({
            ticketTypeId: ticket.id,
            quantity: ticket.quantity,
            groundChangCis: []
          });
        });
        this.orderInput.sign = md5(
          `${this.orderInput.travelDate}${
            this.orderInput.items[0].ticketTypeId
          }${this.orderInput.items[0].quantity}${this.signKey}`
        );

        //创建订单
        await orderService
          .createHandsetOrderAsync(this.orderInput)
          .then(response => {
            if (response.result.shouldPay) {
              this.$router.push(
                `/sale/checkout/${this.summaryMoney}/${response.result.listNo}`
              );
            } else {
              this.$router.push(
                `/sale/checkoutFree/${this.summaryMoney}/${
                  response.result.listNo
                }`
              );
            }
          })
          .catch(response => {
            this.$toast.clear();
            this.$toast.fail(response.response.data.error.message);
          });
      } else {
        this.$toast("请选择商品");
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../variables";

// 底栏高度
$bottom-bar-height: 64px;

// 左边售票大类标签栏宽度(百分比)
$category-list-width: 25%;

// 购物车弹出列表高度
$cart-height: 35vh;

.category-list,
.item-list {
  position: fixed;
  top: 106px;
  bottom: $bottom-bar-height;
  overflow-y: auto;
  margin: 0;
  background: white;
}

// "+", "-", 数字选择按钮, 可以考虑做成单独component
.button-group {
  $button-size: 24px;

  display: flex;
  align-items: center;
  justify-content: flex-end;

  // 当前数量
  .quantity {
    display: inline-block;
    text-align: center;
    width: 24px;
    vertical-align: center;
  }
}

// 售票大类选择区域
.category-list {
  background: #f5f5f5;
  left: 0;
  right: 100% - $category-list-width;

  li {
    position: relative;
    padding: 12px;
  }

  li.active {
    background: white;
    color: $blue;

    &::before {
      content: "";
      position: absolute;
      left: 2px;
      top: 8px;
      bottom: 8px;
      width: 2px;
      background: $blue;
    }
  }
}

// 售票项目选择区域
.item-list {
  left: $category-list-width;
  right: 0;

  li {
    border-bottom: #eeeeee solid 1px;
    padding: 12px 16px;

    .item-name {
      margin-bottom: 0;
    }

    .item-price {
      color: $orange;
      font-family: "DINOT-Medium", sans-serif;
      margin-bottom: 0;
    }
  }
}

// 底栏
.bottom-bar {
  background: #333333;
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: $bottom-bar-height;
  line-height: $bottom-bar-height;
  color: #ffffff;
  z-index: 500;

  // 购物车图标
  .cart-icon {
    $size: 64px;
    font-size: 32px;
    position: absolute;
    left: 20px;
    bottom: 20px;
    padding-right: 4px;
    background: $orange;
    width: $size;
    height: $size;
    line-height: $size + 4px;
    text-align: center;
    border-radius: $size / 2;
  }

  // 购物车右上数字
  .cart-icon-badge {
    $size: 28px;
    font-size: 18px;
    position: absolute;
    display: block;
    right: -5px;
    top: -5px;
    width: $size;
    height: $size;
    line-height: $size - 4px;
    border-radius: $size / 2;
    border: #ffffff 2px solid;
    background: $blue;
  }

  // 金额数字
  .summary-money {
    font-family: "DINOT-Medium", sans-serif;
    font-size: 28px;
    vertical-align: sub;
    margin-left: 28%;
  }

  // 去结算按钮
  .checkout-button {
    background: $orange;
    font-size: 20px;
    position: absolute;
    right: 0;
    top: 0;
    bottom: 0;
    width: 32%;
    border: none;
  }

  // 无商品状态
  &.inactive {
    .cart-icon {
      background: #474747;
    }

    .cart-icon-badge {
      display: none;
    }

    .checkout-button {
      background: #606060;
    }
  }
}

// 购物车底部弹出列表
.cart {
  $title-height: 36px;

  z-index: 499;
  height: $cart-height;
  width: 100vw;
  position: fixed;
  transition: top ease 0.25s;
  top: calc(100vh - #{$bottom-bar-height});

  background: white;
  border-top: 4px $blue solid;

  .cart-title {
    height: $title-height + 4px;
    line-height: $title-height;
    padding: 0 16px;
    border-bottom: #eeeeee solid 1px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    color: #999999;
    font-size: 14px;
  }

  .clear-button {
    border: none;
    background: none;
  }

  .ul {
    max-height: calc(100% - #{$title-height});
    overflow-y: auto;
    padding-top: 0px;
    padding-bottom: 24px;
  }

  li {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px 16px;

    .item-price {
      font-family: "DINOT-Medium", sans-serif;
    }
  }

  &.show {
    top: calc(100vh - #{$cart-height} - #{$bottom-bar-height});
  }
}

// 购物车底部弹出时背景覆盖
.cart-overlay {
  background: transparent;
  transition: background-color ease 0.25s;

  &.show {
    z-index: 498;
    position: fixed;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    background: rgba(0, 0, 0, 0.5);
  }
}
</style>
