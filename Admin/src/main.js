import Vue from "vue";
import ElementUI from "element-ui";
import VCharts from "v-charts";
import App from "./App.vue";
import routerJs from "./router/router.js";
import store from "./store/store.js";
import DatetimeRange from "./components/DatetimeRange.vue";
import ShortcutDatetimePicker from "./components/ShortcutDatetimePicker.vue";
import TinyMce from "./components/TinyMce.vue";
import FileUpload from "./components/FileUpload.vue";
import scenicService from "@/services/scenicService.js";

Vue.config.productionTip = false;

Vue.use(ElementUI, {
  size: "mini",
  zIndex: 3000
});

Vue.use(VCharts);

Vue.component(DatetimeRange.name, DatetimeRange);
Vue.component(ShortcutDatetimePicker.name, ShortcutDatetimePicker);
Vue.component(TinyMce.name, TinyMce);
Vue.component(FileUpload.name, FileUpload);

let letVue = Vue;
routerJs.getRouter().then((router) => {
  const pageLabelMainText = scenicService.getPageLabelMainText();
  for (let i = 0; i < router.options.routes.length; i++) {
    let route = router.options.routes[i];
    route.meta.title = route.meta.title.replace("景区", pageLabelMainText);
    if (route.children) {
      for (let j = 0; j < route.children.length; j++) {
        let childrenRoute = route.children[j];
        childrenRoute.meta.title = childrenRoute.meta.title.replace("景区", pageLabelMainText);
      }
    }
  }

  new letVue({
    router,
    store,
    render: h => h(App)
  }).$mount("#app");
});
