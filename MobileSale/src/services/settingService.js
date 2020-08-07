import ajax from "@/utils/ajax.js";
import weiXinJsSdkHelper from "./../utils/weiXinJsSdkHelper.js";
import store from "./../store/index.js";

export default {
  async getOrderNoticeAsync() {
    let response = await ajax.get("setting/GetOrderNoticeAsync");
    return response.result;
  },
  async getOptionsAsync() {
    let options = localStorage.getItem("options");
    if (options) {
      return JSON.parse(options);
    }
    return await this.getSettingsFromWeChatAsync();
  },
  async getSettingsFromWeChatAsync() {
    const response = await ajax.get("/setting/GetSettingsFromWeChatAsync");
    localStorage.setItem("options", JSON.stringify(response.result));
    return response.result;
  },
  async configWxJsApi() {
    const url = location.href.split("#")[0];

    if (store.state.wxJsApiConfigedPages.includes(url)) return;

    const signature = await this.getWxJsApiSignatureAsync(encodeURIComponent(url));
    await weiXinJsSdkHelper.config(signature, [
      "scanQRCode",
      "onMenuShareAppMessage",
      "updateAppMessageShareData",
      "onMenuShareTimeline",
      "updateTimelineShareData",
      "chooseImage",
      "getLocalImgData"
    ]);

    store.commit("setPageConfiged", url);
  },
  configWxShareUrl(url, imgUrl) {
    const title = "微信购票";
    weiXinJsSdkHelper.updateAppMessageShareData(title, url, imgUrl);
    weiXinJsSdkHelper.updateTimelineShareData(title, url, imgUrl);
  },
  async getWxJsApiSignatureAsync(url) {
    const response = await ajax.get(`setting/GetWxJsApiSignature?url=${url}`);
    return response.result;
  },
  async getWxLoginUrl(url) {
    const response = await ajax.get(`setting/GetWxLoginUrl?url=${encodeURIComponent(url)}`);
    return response.result;
  }
};
