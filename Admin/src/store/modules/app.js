import { app as types } from "./../types.js";

const app = {
  namespaced: true,
  state: {
    [types.isSidebarCollapse]: false
  },
  mutations: {
    [types.toggleSidebar](state) {
      state.isSidebarCollapse = !state.isSidebarCollapse;
    }
  }
};

export default app;
