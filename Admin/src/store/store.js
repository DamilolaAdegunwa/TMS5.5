import Vue from "vue";
import Vuex from "vuex";
import app from "./modules/app.js";
import tagsView from "./modules/tagsView.js";

Vue.use(Vuex);

export default new Vuex.Store({
  modules: { app, tagsView }
});
