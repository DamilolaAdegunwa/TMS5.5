import ajax from "../utils/ajax.js";

export default {
  async createHandsetOrderAsync(input) {
    const response = await ajax.post("/Order/CreateHandsetOrderAsync", input);
    return response;
  },

  async getOrderFullInfoAsync(input) {
    const response = await ajax.post("/Order/GetOrderFullInfoAsync", input);
    return response.result;
  },
  async getLastOrderFullInfoAsync(input) {
    const response = await ajax.post("/Order/GetLastOrderFullInfoAsync", input);
    return response.result;
  },
  async cancelOrderAsync(input) {
    const response = await ajax.post("/Order/CancelOrderAsync", input);
    return response;
  }
};
