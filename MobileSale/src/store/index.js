import Vue from "vue";
import Vuex from "vuex";
import appConsts from "./consts.js";
import orderModule from "./modules/order.js";
import homeModule from "./modules/home.js";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    title: "首页",
    showNavbar: false,
    showNavbarLeft: true,
    showNavbarRight: true,
    navbarColor: "white",
    showTabbar: false,
    tabbarActiveIndex: -1,
    clientHeight: 500,
    groundId: "",
    wxJsApiConfigedPages: [],
    promoterId: "",
    pageLabelMainText: '景区'
  },
  mutations: {
    [appConsts.setTitle](state, value) {
      state.title = value;
    },
    [appConsts.setShowNavbar](state, value) {
      state.showNavbar = value;
    },
    setShowNavbarLeft(state, value) {
      state.showNavbarLeft = value;
    },
    setShowNavbarRight(state, value) {
      state.showNavbarRight = value;
    },
    [appConsts.setNavbarColor](state, value) {
      state.navbarColor = value;
    },
    [appConsts.setShowTabbar](state, value) {
      state.showTabbar = value;
    },
    [appConsts.activeTabbar](state, index) {
      state.tabbarActiveIndex = index;
    },
    setClientHeight(state, value) {
      state.clientHeight = value;
    },
    setGroundId(state, groundId) {
      state.groundId = groundId;
    },
    setPageConfiged(state, pageName) {
      if (state.wxJsApiConfigedPages.includes(pageName)) return;
      state.wxJsApiConfigedPages.push(pageName);
    },
    setPromoterId(state, promoterId) {
      state.promoterId = promoterId;
    },
    setPageLabelMainText(state, value){
      state.pageLabelMainText = value;
    }
  },
  modules: {
    homeModule: homeModule,
    orderModule: orderModule
  }
});
