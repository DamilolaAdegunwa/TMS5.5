import axios from "axios";
import combineURLs from "axios/lib/helpers/combineURLs.js";
import { Notify } from "vant";
import router from "@/routes/router.js";
import tokenService from "@/services/tokenService.js";

if(window.baseURL){
  localStorage.setItem("wechatBaseUrl", window.baseURL);
} else {
  window.baseURL = localStorage.getItem("wechatBaseUrl");
}
const ajax = axios.create({
  baseURL: combineURLs(window.baseURL, "/Api"),
  timeout: 10000
});

ajax.interceptors.request.use(
  function(config) {
    const token = tokenService.getToken();
    if (token) {
      config.headers.common["Authorization"] = `${token.token_type} ${token.access_token}`;
    }

    return config;
  },
  function(error) {
    return Promise.reject(error);
  }
);

ajax.interceptors.response.use(
  response => {
    if (response.data.success !== true && response.data.error) {
      showError(response.data.error.message, response.config.hideError);
    }
    if (response.data.unAuthorizedRequest === true) {
      redirectToErrorPage("登录失败，请关闭页面后重试。", false);
    }

    return response.data;
  },
  error => {
    if (error.response) {
      if (error.response.status === 401) {
        redirectToErrorPage("登录失败，请关闭页面后重试。", false);
      } else if (error.response.status === 403) {
        showError("无权进行此操作", error.config.hideError);
        return Promise.reject("无权进行此操作");
      } else if (!!error.response.data.error && !!error.response.data.error.details) {
        showError(error.response.data.error.details, error.config.hideError);
        return Promise.reject(error.response.data.error.details);
      } else if (!!error.response.data.error && !!error.response.data.error.message) {
        showError(error.response.data.error.message, error.config.hideError);
        return Promise.reject(error.response.data.error.message);
      } else {
        showError(error.message, error.config.hideError);
        return Promise.reject(error.message);
      }
    } else if (error.code === "ECONNABORTED" || error.message === "Network Error") {
      const isLoginPage = router.currentRoute.name == "login";
      redirectToErrorPage("网络访问失败，请稍后重试。", !isLoginPage, !isLoginPage);
    } else {
      showError(error.message, error.config.hideError);
      return Promise.reject(error.message);
    }

    return Promise.reject(error);
  }
);

function showError(message, hideError) {
  if (router.currentRoute.name == "login") {
    redirectToErrorPage(message, false, false);
  } else {
    if (hideError) return;

    Notify(message);
  }
}

function redirectToErrorPage(message, retry, allowBack = true) {
  const route = { name: "errorinfo", params: { text: message, retry: retry } };
  if (allowBack) {
    router.push(route);
  } else {
    router.replace(route);
  }
}

export default ajax;
