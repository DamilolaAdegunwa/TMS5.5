import appConsts from "./../consts.js";
import orderService from "./../../services/orderService.js";

export default {
  namespaced: true,
  state() {
    return {
      signKey: "B885EA36-A238-44BB-A375-040500431DDB",
      options: {
        shouldLoad: true,
        dates: undefined,
        disabledWeeks: undefined,
        groupOrderMaxQuantity: 0,
        individualOrderMaxAdultQuantity: 0,
        individualOrderMaxChildrenQuantity: 0,
        perAdultMaxChildrenQuantity: 0
      }
    };
  },
  mutations: {
    setOptions(state, input) {
      state.options = input;
    }
  },
  actions: {
    async [appConsts.loadOrderOptionsAsync](context) {
      if (context.state.options.shouldLoad) {
        let options = await orderService.getOrderOptionsAsync();
        options.shouldLoad = false;

        context.commit("setOptions", options);
      }
    }
  }
};
