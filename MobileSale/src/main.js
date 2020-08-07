import Vue from "vue";
import App from "./App.vue";
import router from "./routes/router.js";
import store from "./store";

import "./assets/index.scss";
import "./assets/iconfont.css";

import {
  Tabbar,
  TabbarItem,
  NavBar,
  Dialog,
  Toast,
  Loading,
  Tab,
  Tabs,
  List,
  Row,
  Col,
  Cell,
  CellGroup,
  Button,
  Icon,
  Panel,
  Popup,
  DatetimePicker,
  Field,
  RadioGroup,
  Radio,
  Collapse,
  CollapseItem,
  Uploader,
  Stepper,
  Picker,
  Area,
  NumberKeyboard,
  Search,
  ActionSheet,
  Swipe,
  SwipeItem,
  Lazyload,
  SubmitBar,
  PullRefresh,
  Step,
  Steps,
  Notify,
  ImagePreview,
  Checkbox,
  Calendar
} from "vant";

Vue.use(Tabbar)
  .use(TabbarItem)
  .use(NavBar)
  .use(Dialog)
  .use(Toast)
  .use(Loading)
  .use(Tab)
  .use(Tabs)
  .use(List)
  .use(Row)
  .use(Col)
  .use(Cell)
  .use(CellGroup)
  .use(Button)
  .use(Icon)
  .use(Panel)
  .use(Popup)
  .use(DatetimePicker)
  .use(Field)
  .use(RadioGroup)
  .use(Radio)
  .use(Collapse)
  .use(CollapseItem)
  .use(Uploader)
  .use(Stepper)
  .use(Picker)
  .use(Area)
  .use(NumberKeyboard)
  .use(Search)
  .use(ActionSheet)
  .use(Swipe)
  .use(SwipeItem)
  .use(Lazyload)
  .use(SubmitBar)
  .use(PullRefresh)
  .use(Step)
  .use(Steps)
  .use(Notify)
  .use(ImagePreview)
  .use(Checkbox)
  .use(Calendar);

import PickerField from "./components/PickerField.vue";
Vue.component(PickerField.name, PickerField);
import SelectList from "./components/SelectList.vue";
Vue.component(SelectList.name, SelectList);

import Permission from "./directives/permission.js";
Vue.directive("permission", Permission);

if (process.env.NODE_ENV === "development") {
  const VConsole = require("vconsole");
  new VConsole();
}

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
