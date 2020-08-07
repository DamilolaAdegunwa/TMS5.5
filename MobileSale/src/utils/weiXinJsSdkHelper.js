export default {
  jsApiPay(args) {
    if (typeof args == "string") {
      args = JSON.parse(args);
    }

    return new Promise((resolve, reject) => {
      window.WeixinJSBridge.invoke("getBrandWCPayRequest", args, res => {
        if (res.err_msg == "get_brand_wcpay_request:ok") {
          resolve(res);
        } else {
          reject(res);
        }
      });
    });
  },
  config(signature, jsApiList) {
    return new Promise((resolve, reject) => {
      window.wx.config({
        debug: process.env.NODE_ENV == "development",
        appId: signature.appId,
        timestamp: signature.timestamp,
        nonceStr: signature.nonceStr,
        signature: signature.signature,
        jsApiList: jsApiList
      });

      window.wx.ready(() => resolve());

      window.wx.error(res => reject(res.errMsg));
    });
  },
  scanQRCode() {
    return new Promise((resolve, reject) => {
      window.wx.scanQRCode({
        needResult: 1,
        scanType: ["qrCode", "barCode"],
        success: res => resolve(res.resultStr),
        fail: res => reject(res.errMsg),
        cancel: () => reject("cancel")
      });
    });
  },
  updateAppMessageShareData(title, url, imgUrl) {
    return new Promise(resolve => {
      if (window.wx.updateAppMessageShareData) {
        window.wx.updateAppMessageShareData({
          title: title,
          desc: title,
          link: url,
          imgUrl: imgUrl,
          success: () => resolve()
        });
      } else {
        window.wx.onMenuShareAppMessage({
          title: title,
          desc: title,
          link: url,
          imgUrl: imgUrl,
          type: "",
          dataUrl: "",
          success: () => resolve()
        });
      }
    });
  },
  updateTimelineShareData(title, url, imgUrl) {
    return new Promise(resolve => {
      if (window.wx.updateTimelineShareData) {
        window.wx.updateTimelineShareData({
          title: title,
          link: url,
          imgUrl: imgUrl,
          success: () => resolve()
        });
      } else {
        window.wx.onMenuShareTimeline({
          title: title,
          link: url,
          imgUrl: imgUrl,
          success: () => resolve()
        });
      }
    });
  },
  chooseImage() {
    return new Promise((resolve, reject) => {
      window.wx.chooseImage({
        count: 1,
        sizeType: ["compressed"],
        sourceType: ["camera"],
        success: function(res) {
          const localId = res.localIds[0];
          resolve(localId);
        },
        fail: function(error) {
          reject(error);
        },
        cancel: function() {
          reject("cancel");
        }
      });
    });
  },
  getLocalImgData(localId) {
    return new Promise((resolve, reject) => {
      window.wx.getLocalImgData({
        localId: localId,
        success: function(res) {
          if (res.localData.substr(0, 5) == "data:") {
            resolve(res.localData.replace("jgp", "jpeg"));
          } else {
            resolve(`data:image/jpeg;base64,${res.localData}`);
          }
        },
        fail: function(error) {
          reject(error);
        }
      });
    });
  }
};
