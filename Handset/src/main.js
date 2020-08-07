import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faAngleRight,
  faChartLine,
  faChevronLeft,
  faClipboardCheck,
  faCreditCard,
  faEye,
  faEyeSlash,
  faHome,
  faLock,
  faMinus,
  faPlus,
  faQrcode,
  faReceipt,
  faSearch,
  faShoppingCart,
  faSignOutAlt,
  faTicketAlt,
  faTrashAlt,
  faUndoAlt,
  faUser,
  faYenSign
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import "bootstrap/dist/css/bootstrap-reboot.css";
import {
  Actionsheet,
  Button,
  Cell,
  CellGroup,
  Col,
  Field,
  GoodsAction,
  GoodsActionBigBtn,
  GoodsActionMiniBtn,
  NavBar,
  Panel,
  Row,
  Tab,
  Tabbar,
  TabbarItem,
  Tabs,
  Toast,
  TreeSelect,
  Popup,
  Picker,
  RadioGroup,
  Radio,
  Dialog,
  Checkbox
} from "vant";
import Vue from "vue";
import App from "./App.vue";
import router from "./routes/router";
import store from "./store";

// 加载 Fontawesome 图标
library.add(faAngleRight);
library.add(faChartLine);
library.add(faChevronLeft);
library.add(faClipboardCheck);
library.add(faCreditCard);
library.add(faEye);
library.add(faEyeSlash);
library.add(faHome);
library.add(faLock);
library.add(faMinus);
library.add(faPlus);
library.add(faQrcode);
library.add(faReceipt);
library.add(faSearch);
library.add(faShoppingCart);
library.add(faSignOutAlt);
library.add(faTicketAlt);
library.add(faTrashAlt);
library.add(faUndoAlt);
library.add(faUser);
library.add(faYenSign);
Vue.component("font-awesome-icon", FontAwesomeIcon);

// 加载组件
Vue.use(Actionsheet);
Vue.use(Button);
Vue.use(Cell);
Vue.use(CellGroup);
Vue.use(Col);
Vue.use(Field);
Vue.use(GoodsAction);
Vue.use(GoodsActionBigBtn);
Vue.use(GoodsActionMiniBtn);
Vue.use(NavBar);
Vue.use(Panel);
Vue.use(Row);
Vue.use(Tab);
Vue.use(Tabbar);
Vue.use(TabbarItem);
Vue.use(Tabs);
Vue.use(TreeSelect);
Vue.use(Toast);
Vue.use(Popup);
Vue.use(Picker);
Vue.use(Radio);
Vue.use(RadioGroup);
Vue.use(Dialog);
Vue.use(Checkbox);
Vue.config.productionTip = false;

if (process.env.NODE_ENV === "development") {
  const VConsole = require("vconsole");
  new VConsole();
}

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
