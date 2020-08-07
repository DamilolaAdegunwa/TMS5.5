import axios from "axios";
import combineURLs from "axios/lib/helpers/combineURLs.js";
import { Toast } from "vant";
import router from "./../routes/router.js";
import tokenService from "@/services/tokenService.js";

const ajax = axios.create({
  baseURL: combineURLs(window.baseURL, "/api"),
  timeout: 30000
});

ajax.interceptors.request.use(
  function(config) {
    let token = tokenService.getToken();
    if (token) {
      config.headers.common["Authorization"] = `Bearer ${token}`;
    }
    Toast.loading({
      duration: 0, // 持续展示 toast
      forbidClick: true, // 禁用背景点击
      loadingType: "spinner",
      message: "加载中..."
    });
    return config;
  },
  function(error) {
    return Promise.reject(error);
  }
);

ajax.interceptors.response.use(
  respon => {
    Toast.clear();
    if (respon.data.success !== true && respon.data.error) {
      showError(respon.data.error);
    }
    if (respon.data.unAuthorizedRequest === true) {
      router.push({
        name: "errorinfo",
        query: {
          retry: false,
          text: "登录失败，请关闭页面后重试。"
        }
      });
    }

    return respon.data;
  },
  error => {
    if (error.response) {
      if (error.response.status === 401) {
        router.push({
          name: "errorinfo",
          query: {
            retry: false,
            text: "登录失败，请关闭页面后重试。"
          }
        });
      } else if (error.response.status === 403) {
        showError("无权进行此操作");
      } else if (
        !!error.response.data.error &&
        !!error.response.data.error.message &&
        error.response.data.error.details
      ) {
        showError(error.response.data.error.details);
      } else if (
        !!error.response.data.error &&
        !!error.response.data.error.message
      ) {
        showError(error.response.data.error.message);
      } else {
        showError(error.message);
      }
    } else if (
      error.code === "ECONNABORTED" ||
      error.message === "Network Error"
    ) {
      // router.push({
      //   name: "errorinfo",
      //   query: {
      //     retry: router.currentRoute.name != "login",
      //     text: "网络访问失败，请稍后重试。"
      //   }
      // });
      Toast.fail("网络访问失败，请稍后重试。");
    } else {
      showError(error.message);
    }

    return Promise.reject(error);
  }
);

function showError(message) {
  if (router.currentRoute.name == "login") {
    router.replace({
      name: "errorinfo",
      query: {
        retry: false,
        text: message
      }
    });
  } else {
    //Toast(message);
    Toast.clear();
  }
}

export default ajax;
