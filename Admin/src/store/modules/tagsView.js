import { tagsView as types } from "./../types.js";

const tagsView = {
  namespaced: true,
  state: {
    [types.visitedViews]: [],
    [types.cachedViews]: []
  },
  mutations: {
    [types.addVisitedView]: (state, view) => {
      if (state.visitedViews.some(v => v.path === view.path)) return;
      state.visitedViews.push(
        Object.assign({}, view, {
          title: view.meta.title || "no-name"
        })
      );
    },
    [types.addCachedView]: (state, view) => {
      if (state.cachedViews.includes(view.name)) return;
      if (!view.meta.noCache) {
        state.cachedViews.push(view.name);
      }
    },
    [types.deleteVisitedView]: (state, view) => {
      for (const [i, v] of state.visitedViews.entries()) {
        if (v.path === view.path) {
          state.visitedViews.splice(i, 1);
          break;
        }
      }
    },
    [types.deleteCachedView]: (state, view) => {
      for (const i of state.cachedViews) {
        if (i === view.name) {
          const index = state.cachedViews.indexOf(i);
          state.cachedViews.splice(index, 1);
          break;
        }
      }
    },
    [types.deleteOthersVisitedView]: (state, view) => {
      for (const [i, v] of state.visitedViews.entries()) {
        if (v.path === view.path) {
          state.visitedViews = state.visitedViews.slice(i, i + 1);
          break;
        }
      }
    },
    [types.deleteOthersCachedView]: (state, view) => {
      for (const i of state.cachedViews) {
        if (i === view.name) {
          const index = state.cachedViews.indexOf(i);
          state.cachedViews = state.cachedViews.slice(index, index + 1);
          break;
        }
      }
    },
    [types.deleteAllVisitedView]: state => {
      state.visitedViews = [];
    },
    [types.deleteAllCachedView]: state => {
      state.cachedViews = [];
    },
    [types.updateVisitedView]: (state, view) => {
      for (let v of state.visitedViews) {
        if (v.path === view.path) {
          v = Object.assign(v, view);
          break;
        }
      }
    }
  },
  actions: {
    [types.addView]({ commit }, view) {
      commit(types.addVisitedView, view);
      commit(types.addCachedView, view);
    },
    [types.deleteView]({ commit }, view) {
      commit(types.deleteVisitedView, view);
      commit(types.deleteCachedView, view);
    },
    [types.deleteOthersView]({ commit }, view) {
      commit(types.deleteOthersVisitedView, view);
      commit(types.deleteOthersCachedView, view);
    },
    [types.deleteAllView]({ commit }) {
      commit(types.deleteAllVisitedView);
      commit(types.deleteAllCachedView);
    }
  }
};

export default tagsView;
