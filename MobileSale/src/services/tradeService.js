import ajax from "@/utils/ajax.js";
import qs from "qs";

export default {
  async statTradeAsync(input) {
    const response = await ajax.post("/trade/StatTradeAsync", qs.stringify(input));
    return response.result;
  },
  async statPayDetailJbAsync(input) {
    const response = await ajax.post("/trade/StatPayDetailJbAsync", qs.stringify(input));
    return response.result;
  }
};
