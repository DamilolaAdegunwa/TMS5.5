import ajax from "@/utils/ajax.js";

export default {
  async getMenuAsync() {
    let response = await ajax.get("/navigation/GetMobileSaleMenuAsync");
    return response.result;
  }
};
