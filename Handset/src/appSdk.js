import { App, Barcode, IdCard } from "./handset-sdk.js";

export default {
  scanBarcode(withFlash = false) {
    return new Promise((resolve, reject) => {
      Barcode.read(
        code => {
          App.beep();
          resolve(code);
        },
        () => {
          reject(new Error("扫描失败"));
        },
        withFlash
      );
    });
  },
  readIdCard() {
    return new Promise((resolve, reject) => {
      IdCard.read(
        1000 * 10,
        (card, image) => {
          App.beep();
          resolve({ card: card, image: image });
        },
        () => {
          reject(new Error("读卡失败"));
        },
        () => {
          reject(new Error("读卡超时"));
        }
      );
    });
  }
};
