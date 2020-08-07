import axios from "axios";
import combineURLs from "axios/lib/helpers/combineURLs.js";
import { Message } from "element-ui";
import router from "./../router/router.js";
import tokenService from "@/services/tokenService.js";
import ipHelper from "./ipHelper.js";

const baseURL = ipHelper.isIntranetIp(window.location.href)
  ? window.webApiIntranetUrl
  : window.webApiOuterNetUrl;

let ajax = axios.create({
  baseURL: combineURLs(baseURL, "/api"),
  timeout: 300000
});

ajax.interceptors.request.use(
  config => {
    const token = tokenService.getToken();
    if (token) {
      config.headers.common["Authorization"] = `Bearer ${token}`;
    }

    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

ajax.interceptors.response.use(
  respon => {
    if (respon.data.success !== true && respon.data.error) {
      Message.error(respon.data.error);
    }
    if (respon.data.unAuthorizedRequest === true) {
      router.push({ name: "login" });
    }

    return respon.data;
  },
  error => {
    if (error.response) {
      const status = error.response.status;
      if (status === 401 || status === 403) {
        router.push({ name: "login" });
      } else if (
        !!error.response.data.error &&
        !!error.response.data.error.message &&
        error.response.data.error.details
      ) {
        Message.error(error.response.data.error.details);
      } else if (!!error.response.data.error && !!error.response.data.error.message) {
        Message.error(error.response.data.error.message);
      } else {
        Message.error(error.message);
      }
    } else if (error.code === "ECONNABORTED" || error.message === "Network Error") {
      Message.error("网络访问失败，请稍后重试。");
    } else {
      Message.error(error.message);
    }

    return Promise.reject(error);
  }
);

export default ajax;

export { baseURL };
