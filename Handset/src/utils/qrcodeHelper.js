import QRCode from "qrcode";
import ajax from "./ajax.js";

export default {
  async createQRCodeAsync(value) {
    try {
      return await QRCode.toDataURL(value, {
        width: 200
      });
    } catch {
      const response = await ajax.get(
        `/common/CreateQRCodeAsync?value=${value}`
      );
      return response.result;
    }
  }
};
