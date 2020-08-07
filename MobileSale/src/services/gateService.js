import ajax from "@/utils/ajax.js";

export default {
  async registMobileAsync(input) {
    const response = await ajax.post("/gate/RegistMobileAsync", input);
    return response.result;
  },
  async changeLocationAsync(input) {
    const response = await ajax.put("/gate/ChangeLocationAsync", input);
    return response.result;
  }
};
