export default {
  //设置cookie
  setCookie(cname, cvalue, exdays) {
    let d = new Date();
    d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000);
    //d.setTime(d.getTime() + exdays * 1000);
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
  },
  //获取cookie
  getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(";");

    for (let i = 0; i < ca.length; i++) {
      let c = ca[i].trim();
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  },
  //清除cookie
  clearCookie(cname) {
    this.setCookie(cname, "", -1);
  },
  //
  checkCookie(cname, exdays) {
    let temp = this.getCookie(cname);
    if (temp != "") {
      this.setCookie(cname, temp, exdays);
      return true;
    } else {
      return false;
    }
  }
};
