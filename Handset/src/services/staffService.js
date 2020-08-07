import ajax from "./../utils/ajax.js";
import tokenService from "./tokenService.js";

export default {
  async loginAsync(input) {
    const response = await ajax.post("/Staff/LoginFromHandsetAsync", input);

    let staffInfo = response.result.staff;
    sessionStorage.setItem("staffInfo", JSON.stringify(staffInfo));
    sessionStorage.setItem("staffId", staffInfo.id);
    sessionStorage.setItem("employeeName", staffInfo.name);
    //sessionStorage.setItem("parkId", staffInfo.parkId);
    sessionStorage.setItem("userName", input.userName);
    tokenService.setToken(response.result.token);
    let permissions = JSON.stringify(
      response.result.permissions
    ).toLocaleLowerCase();
    sessionStorage.setItem("permissions", permissions);
    return response;
  },
  async editPasswordAsync(input) {
    const response = await ajax.post("/Staff/EditPasswordAsync", input);
    return response;
  },
  logout() {
    sessionStorage.removeItem("staffInfo");
    tokenService.deleteToken();
    sessionStorage.removeItem("permissions");
  }
};
