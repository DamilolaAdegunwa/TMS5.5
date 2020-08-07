import ajax from "@/utils/ajax.js";
import tokenService from "@/services/tokenService.js";

export default {
  async loginFromWeChatAsync(input) {
    const response = await ajax.post("/customer/LoginFromWeChatAsync", input);
    this.setMember(response);
  },
  async registAsync(input) {
    return await ajax.post("/customer/RegistFromMobileAsync", input);
  },
  async changePasswordAsync(input) {
    return await ajax.post("/customer/ChangePasswordAsync", input);
  },
  async getCustomerTypeComboboxItemsAsync() {
    const response = await ajax.get("/customer/GetCustomerTypeComboboxItemsAsync");
    return response.result;
  },
  setMember(response) {
    let member = response.result.member;
    member.permissions = response.result.permissions;
    localStorage.setItem("member", JSON.stringify(member));

    tokenService.setToken(response.result.token);
  }
};
