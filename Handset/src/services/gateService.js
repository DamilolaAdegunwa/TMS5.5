import ajax from "./../utils/ajax.js";
import { App } from "../handset-sdk.js";

export default {
  async registHandsetAsync(input) {
    const response = await ajax.post("/Gate/RegistHandsetAsync", input);
    if (App) {
      App.setValue("gateId", response.result.id);
    } else {
      window.localStorage.setItem("gateId", response.result.id);
    }

    return response;
  },
  async changeLocationAsync(input) {
    await ajax.put("/Gate/ChangeLocationAsync", input);
  }
};
