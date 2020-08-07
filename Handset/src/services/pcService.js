import ajax from "./../utils/ajax.js";
import { App } from "../handset-sdk.js";

export default {
  async registHandsetAsync(input) {
    const response = await ajax.post("/Pc/RegistHandsetAsync", input);

    if (App) {
      App.setValue("pcId", response.result.id);
    } else {
      window.localStorage.setItem("pcId", response.result.id);
    }
    return response;
  },
  async changeLocationAsync(input) {
    const response = await ajax.put("/Pc/ChangeLocationAsync", input);
    sessionStorage.setItem("parkId", response.result.parkId);
  }
};
