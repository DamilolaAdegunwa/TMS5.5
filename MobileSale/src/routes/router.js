import Vue from "vue";
import Router from "vue-router";
import dayjs from "dayjs";
import Store from "@/store/index.js";
import AppConsts from "@/store/consts.js";
import Permissions from "./../permissions.js";

import ErrorInfo from "@/views/ErrorInfo.vue";
import Login from "@/views/Login.vue";
import Home from "./../views/Home.vue";
import Distribution from "./../views/Distribution.vue";
import SubscribeWeChat from "./../views/SubscribeWeChat.vue";

import bookRoutes from "./book.js";
import groupRoutes from "./groups.js";
import memberRoutes from "./members.js";
import orderRoutes from "./orders.js";
import payRoutes from "./payment.js";
import reportRoutes from "./reports.js";
import staffRoutes from "./staffs.js";
import ticketRoutes from "./tickets.js";

import tokenService from "./../services/tokenService.js";
import settingService from "./../services/settingService.js";
import memberService from "./../services/memberService.js";

Vue.use(Router);

const router = new Router({
  mode: "history",
  routes: [
    {
      path: "/",
      name: "home",
      component: Home,
      meta: {
        title: "首页"
      }
    },
    {
      path: "/errorinfo",
      name: "errorinfo",
      component: ErrorInfo,
      props: true,
      meta: {
        title: "操作失败",
        showNavbar: false,
        shouldNotConfirm: true,
        allowAnonymous: true
      }
    },
    {
      path: "/login",
      name: "login",
      component: Login,
      meta: {
        title: "登录",
        showNavbar: false,
        allowAnonymous: true
      }
    },
    {
      path: "/SubscribeWeChat",
      name: "SubscribeWeChat",
      component: SubscribeWeChat,
      meta: {
        title: "关注微信公众号",
        allowAnonymous: true
      }
    },
    {
      path: "/Distribution",
      name: "Distribution",
      component: Distribution,
      meta: {
        title: "分销平台",
        showNavbar: false,
        permission: Permissions.TMSWeChat_Distribution
      }
    },
    ...bookRoutes,
    ...groupRoutes,
    ...memberRoutes,
    ...orderRoutes,
    ...payRoutes,
    ...reportRoutes,
    ...staffRoutes,
    ...ticketRoutes
  ],
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      const position = {
        x: 0,
        y: 0
      };
      if (to.hash) {
        position.selector = to.hash;
      }
      return position;
    }
  }
});

const tabbarRoutes = ["tickettype", "order", "my"];

router.beforeEach(async (to, from, next) => {
  if (!(await ensureLogined(to, from, next))) return;

  const member = memberService.getMember();
  let def = "H5";
  if (jWeixin && jWeixin.miniProgram && jWeixin.miniProgram.getEnv) {
    console.log(window.__wxjs_environment);
    if (window.__wxjs_environment === 'miniprogram') {
      def = "MP";
    }
  }
  memberService.setDef(def);
  if (def == "MP") {
    alert("小程序环境");
  } else {
    if (!(await ensureWeChatSubscribed(member, to, next))) return;
  }

  if (to.meta.permission) {
    const permissions = member && member.permissions ? member.permissions : [];
    const hasPermission = permissions.some(
      p => p.toLowerCase() === to.meta.permission.toLowerCase()
    );
    if (!hasPermission) {
      next(false);
      return;
    }
  }

  window.document.title = to.meta.title;
  Store.commit(AppConsts.setTitle, to.meta.title);

  let showNavbar = to.meta.showNavbar;
  if (showNavbar === undefined) {
    showNavbar = true;
  }
  Store.commit(AppConsts.setShowNavbar, showNavbar);

  let navbarColor = to.meta.navbarColor;
  if (navbarColor === undefined) {
    navbarColor = "white";
  }
  Store.commit(AppConsts.setNavbarColor, navbarColor);

  let showTabbar = to.meta.showTabbar;
  if (showTabbar === undefined) {
    showTabbar = false;
  }
  Store.commit(AppConsts.setShowTabbar, showTabbar);

  const tabbarActiveIndex = tabbarRoutes.indexOf(to.name);
  if (tabbarActiveIndex >= 0) {
    Store.commit(AppConsts.activeTabbar, tabbarActiveIndex);
  }

  next();
});

async function ensureLogined(to, from, next) {
  if (to.meta.allowAnonymous) return true;
  if (process.env.NODE_ENV == "development") return true;

  const token = tokenService.getToken();
  if (!token || dayjs.unix(token.expires_in).isBefore(dayjs())) {
    let loginUrl = `${location.origin}/Login?redirect=${location.pathname}&date=${new Date().getTime()}`;
    if (location.search) {
      loginUrl += location.search.replace("?", "&");
    }
    const wxLoginUrl = await settingService.getWxLoginUrl(loginUrl);
    if (wxLoginUrl) {
      next(false);
      location.href = wxLoginUrl + `&urldate=${new Date().getTime() + 1234}`;
      return false;
    }
  }

  return true;
}

async function ensureWeChatSubscribed(member, to, next) {
  if (to.meta.allowAnonymous) return true;

  if (process.env.NODE_ENV == "development") return true;

  if (!member || member.isWeChatSubscribed === false) {
    const options = await settingService.getOptionsAsync();
    if (options && options.WxSubscribeUrl) {
      next({
        name: "SubscribeWeChat",
        params: { subscribeUrl: options.WxSubscribeUrl }
      });
      return false;
    }
  }

  return true;
}

export default router;
