import Vue from "vue";
import Router from "vue-router";
import Account from "../views/account/Account";
import AccountChangePassword from "../views/account/AccountChangePassword";
//import AccountCheckStat from "../views/account/AccountCheckStat";
import AccountReprintReceipt from "../views/account/AccountReprintReceipt";
import AccountReprintCheckIn from "../views/account/AccountReprintCheckIn";
import AccountSaleStat from "../views/account/AccountSaleStat";
import AccountCheckStat from "../views/account/AccountCheckStat";
import Check from "../views/check/Check";
import CheckConsume from "../views/check/CheckConsume";
import CheckFailed from "../views/check/CheckFailed";
import CheckSuccess from "../views/check/CheckSuccess";
import Home from "../views/Home";
import Login from "../views/Login";
import Sale from "../views/sale/Sale";
import SaleVerify from "../views/sale/SaleVerify";
import SaleDrawback from "../views/sale/SaleDrawback";
import SaleCheckout from "../views/sale/SaleCheckout";
import SaleCheckoutCash from "../views/sale/SaleCheckoutCash";
import SaleCheckoutReceipt from "../views/sale/SaleCheckoutReceipt";
import SaleCheckoutFree from "../views/sale/SaleCheckoutFree";
import SaleDrawbackFinish from "../views/sale/SaleDrawbackFinish";
import Setup from "../views/Setup";

Vue.use(Router);

const routes = [
  { path: "/", redirect: "/login" },
  { path: "/login", component: Login },
  { path: "/setup/:set", component: Setup },
  { path: "/home", component: Home },
  { path: "/sale", component: Sale },
  { path: "/sale/drawback", component: SaleDrawback },
  { path: "/sale/verify", component: SaleVerify },
  { path: "/sale/checkout/:summaryMoney/:listNo", component: SaleCheckout },
  {
    path: "/sale/checkoutCash/:payTypeName/:payMoney/:listNo",
    component: SaleCheckoutCash
  },
  {
    path: "/sale/checkoutReceipt/:payTypeId/:payMoney/:listNo",
    component: SaleCheckoutReceipt
  },
  {
    path: "/sale/checkoutFree/:payMoney/:listNo",
    component: SaleCheckoutFree
  },
  { path: "/check", component: Check },
  { path: "/check/consume", component: CheckConsume },
  {
    path: "/check/success/:ticketId/:ticketTypeName/:checkinNum",
    component: CheckSuccess
  },
  { path: "/check/failed/:ticketId/:errMsg", component: CheckFailed },
  { path: "/drawback/success/:drawbackMoney", component: SaleDrawbackFinish },
  { path: "/account", component: Account },
  { path: "/account/change-password", component: AccountChangePassword },
  { path: "/account/reprint-checkIn", component: AccountReprintCheckIn },
  { path: "/account/reprint-receipt", component: AccountReprintReceipt },
  { path: "/account/check-stat", component: AccountCheckStat },
  { path: "/account/sale-stat", component: AccountSaleStat }
];

const routers = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: routes
});

export default routers;
routers.beforeEach((to, from, next) => {
  if (to.path != "/login") {
    if (!window.sessionStorage.getItem("permissions")) {
      next({
        path: "/login"
      });
    } else {
      next();
    }
  } else {
    next();
  }
});
