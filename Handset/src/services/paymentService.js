import ajax from "./../utils/ajax.js";

export default {
  async microPayAsync(input) {
    const response = await ajax.post("/Payment/MicroPayAsync", input);
    return response;
  },
  async cashPayAsync(listNo) {
    const response = await ajax.post(`/Payment/CashPayAsync?listNo=${listNo}`);
    return response;
  },
  async getNetPayOrderAsync(listNo) {
    const response = await ajax.get(
      `/Payment/GetNetPayOrderAsync?listNo=${listNo}`
    );
    return response;
  }
};
