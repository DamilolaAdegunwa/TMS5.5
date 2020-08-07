import moment from "moment";

const tokenKey = "token";

export default {
  getToken() {
    const token = window.sessionStorage.getItem(tokenKey);
    if (!token) {
      return "";
    }
    const tokenObj = JSON.parse(token);
    if (moment.unix(tokenObj.expires_in).isBefore(moment())) {
      return "";
    }

    return tokenObj.access_token;
  },
  setToken(value) {
    window.sessionStorage.setItem(tokenKey, value);
  },
  deleteToken() {
    window.sessionStorage.removeItem(tokenKey);
  }
};
