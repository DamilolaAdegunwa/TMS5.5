import QRCode from "qrcode";
import JsBarCode from "jsbarcode";
import ajax from "./ajax.js";

export default {
  async createQRCodeAsync(value) {
    try {
      return await QRCode.toDataURL(value, {
        width: 200
      });
    } catch {
      const response = await ajax.get(`/common/CreateQRCodeAsync?value=${value}`);
      return response.result;
    }
  },
  async createBarCodeAsync(valueRef, value, param) {
    try {
      if(!param){
        param =  {
          displayValue: false,
          width: 2,
          height: 120,
          margin: 5
        };
      }
      return JsBarCode(valueRef, value, param);
    } catch {
      const response = await ajax.get(`/common/CreateQRCodeAsync?value=${value}`);
      return response.result;
    }
  }
};
