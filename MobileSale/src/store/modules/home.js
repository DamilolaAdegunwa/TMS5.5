import settingService from "./../../services/settingService.js";
import appConsts from "./../consts.js";

export default {
  namespaced: true,
  state() {
    return {
      notice: {
        scenicName: "",
        timeNotice: "",
        orderNotice: "",
        contactNotice: "",
        shouldLoad: true
      }
    };
  },
  mutations: {
    setNotice(state, notice) {
      state.notice = notice;
    }
  },
  actions: {
    async [appConsts.loadOrderNoticeAsync](context) {
      if (context.state.notice.shouldLoad) {
        let notice = await settingService.getOrderNoticeAsync();
        notice.shouldLoad = false;

        context.commit("setNotice", notice);
      }
    }
  }
};
