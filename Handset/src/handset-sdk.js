/**
 * 易高手持机 JavaScript API ES6 Module
 *
 * @copyright Egoal
 * @version v1.1.1 @2020/05/11
 */

/**
 * 将相对路径转换为绝对路径
 * @param {string} url
 * @returns {string}
 */
export function toAbsoluteUrl(url) {
  const a = document.createElement("a");
  a.href = url;
  return a.href;
}

/**
 * 空函数
 */
function noop() {}

const SERVER_ADDRESS_KEY = "server-address";
const UPDATE_ADDRESS_KEY = "update-address";

const __App__ = window.__App__;
const __Barcode__ = window.__Barcode__;
const __Printer__ = window.__Printer__;
const __IdCard__ = window.__IdCard__;
const __Nfc__ = window.__Nfc__;
const __Socket = window.__Socket__;

/**
 * App自身相关功能
 */
export const App = window.__App__ !== undefined && {
  /**
   * @type {string} 手持机机型, 天波定义的型号名称
   */
  model: __App__.getModel(),

  /**
   * @type {string} 手持机机型ID, 由天波定义的ID
   */
  modelId: __App__.getModelId(),

  /**
   * @type {string} 手持机IMEI码
   */
  IMEI: __App__.getImei(),

  /**
   * @type {string} 手持机序列号
   */
  SN: __App__.getSerialNumber(),

  /**
   * @type {string} APK版本名称
   */
  versionName: __App__.versionName(),

  /**
   * @type {number} APK版本编号
   */
  versionCode: __App__.versionCode(),

  /**
   * 弹出提示窗, 使用Android的弹窗实现
   * @param {string} message
   */
  alert(message) {
    return window.alert(message);
  },

  /**
   * 弹出确认窗, 使用Android的弹窗实现
   * @param {string} message
   * @returns {boolean}
   */
  confirm(message) {
    return window.confirm(message);
  },

  /**
   * 弹出输入窗, 使用Android的弹窗实现
   * @param {string} message
   * @param {?string} _default - 默认值
   * @returns {string}
   */
  prompt(message, _default = undefined) {
    return window.prompt(message, _default);
  },

  /**
   * 播放短的蜂鸣声
   */
  beep() {
    return __App__.beep();
  },

  /**
   * 弹出Android自带的底部消息提示 (toast)
   * @param {string} message
   */
  toast(message) {
    return __App__.toast(message);
  },

  /**
   * 关闭当前显示的底部提示消息 (toast)
   */
  dismissToast: function() {
    __App__.dismissToast();
  },

  /**
   * 重新加载Activity, 重启整个Webview环境
   */
  restart() {
    return __App__.restart();
  },

  /**
   * 重新加载当前页 (刷新)
   */
  refresh() {
    return window.location.reload();
  },

  /**
   * 进入固定屏幕模式，会在界面上弹出确认对话框
   * (!) 由于某些不明原因，经测试并不能正常通过长按退出该模式，慎用
   */
  startLockTask() {
    return __App__.startLockTask();
  },

  /**
   * 解除固定屏幕模式
   */
  stopLockTask() {
    return __App__.stopLockTask();
  },

  /**
   * 跳转到页面
   * @param {string} url
   * @param {boolean} clearHistory - 是否在打开后清除记录(即无法返回)
   */
  openUrl(url, clearHistory) {
    return __App__.openUrl(url, clearHistory);
  },

  /**
   * 清除浏览器返回历史 (清除浏览器返回的队列)
   */
  clearHistory() {
    return __App__.clearHistory();
  },

  /**
   * 跳转到在android_assets目录中的页面
   * @param {string} filename
   */
  openAsset(filename) {
    return __App__.openAsset(filename);
  },

  /**
   * 打开App中的启动页面
   */
  openLandingPage() {
    return __App__.openLandingPage();
  },

  /**
   * 打开App中的设置页面
   */
  openSettingPage() {
    return __App__.openSettingPage();
  },

  /**
   * 打开App中的功能演示页面
   */
  openDemoPage() {
    return __App__.openDemoPage();
  },

  /**
   * 获取本地储存的数据(字符串)
   * @param {string} key
   * @param {string} _default - 默认值
   * @returns {?string}
   */
  getValue(key, _default) {
    const value = __App__.getValue(key);
    return value ? value : _default;
  },

  /**
   * 写入本地存储的数据(字符串)
   * @param {string} key
   * @param {string} value
   */
  setValue(key, value) {
    return __App__.setValue(key, value);
  },

  /**
   * 清除本地存储的数据
   * @param {string} key
   */
  clearValue(key) {
    return __App__.clearValue(key);
  },

  /**
   * 删除所有本地储存的数据
   */
  clearValues() {
    return __App__.clearValues();
  },

  /**
   * 获取检票服务器地址
   * @param {?string} _default - 默认值
   * @returns {?string}
   */
  getServerAddress(_default = undefined) {
    return App.getValue(SERVER_ADDRESS_KEY, _default);
  },

  /**
   * 设置检票服务器地址
   * @param {string} address
   */
  setServerAddress(address) {
    return App.setValue(SERVER_ADDRESS_KEY, address);
  },

  /**
   * 获取更新服务器地址
   * @param {?string} _default - 默认值
   * @returns {?string}
   */
  getUpdateAddress(_default = undefined) {
    return App.getValue(UPDATE_ADDRESS_KEY, _default);
  },

  /**
   * 设置更新服务器地址
   * @param {string} address
   */
  setUpdateAddress(address) {
    return App.setValue(UPDATE_ADDRESS_KEY, address);
  }
};

/**
 * 二维码 / 条码读取相关功能
 */
export const Barcode = window.__Barcode__ !== undefined && {
  __onSuccess__: undefined,
  __onFailed__: undefined,

  /**
   * 扫描二维码 / 条码, 完成或失败后会调用相应的回调函数
   * @param {function} onSuccess - 成功回调 (result: string) => Any
   * @param {function} onFailed - 失败回调 () => Any
   * @param {boolean} withFlash - 如果为真，在开启扫描时同时打开闪光灯
   */
  read(onSuccess, onFailed = noop, withFlash = false) {
    Barcode.__onSuccess__ = onSuccess || noop;
    Barcode.__onFailed__ = onFailed || noop;
    return __Barcode__.read(
      "Barcode.__onSuccess__",
      "Barcode.__onFailed__",
      withFlash
    );
  }
};

/**
 * 热敏打印机相关功能
 */
export const Printer = window.__Printer__ !== undefined && {
  ALIGN_LEFT: __Printer__.ALGIN_LEFT(),
  ALIGN_MIDDLE: __Printer__.ALGIN_MIDDLE(),
  ALIGN_RIGHT: __Printer__.ALGIN_RIGHT(),
  DIRECTION_BACK: __Printer__.DIRECTION_BACK(),
  DIRECTION_FORWORD: __Printer__.DIRECTION_FORWORD(),
  STATUS_NO_PAPER: __Printer__.STATUS_NO_PAPER(),
  STATUS_OK: __Printer__.STATUS_OK(),
  STATUS_OVER_FLOW: __Printer__.STATUS_OVER_FLOW(),
  STATUS_OVER_HEAT: __Printer__.STATUS_OVER_HEAT(),
  STATUS_UNKNOWN: __Printer__.STATUS_UNKNOWN(),
  WALK_DOTLINE: __Printer__.WALK_DOTLINE(),
  WALK_LINE: __Printer__.WALK_LINE(),

  /**
   * 启动打印机 (!) 使用打印机功能前必须先启动
   * @param {number} mode - 0: 普通打印模式, 1: 大电流快速打印模式(慎用)
   */
  start(mode) {
    return __Printer__.start(mode);
  },

  /**
   * 停止打印机
   */
  stop() {
    return __Printer__.stop();
  },

  /**
   * 重置打印机, 恢复默认设置, 清空打印缓存
   */
  reset() {
    return __Printer__.reset();
  },

  /**
   * 打印机走纸
   * @param {number} line - 走纸行数
   */
  walkPaper(line) {
    return __Printer__.walkPaper(line);
  },

  /**
   * 检查打印机状态, 返回值见常量部分STATUS_*
   * @returns {number}
   */
  checkStatus() {
    return __Printer__.checkStatus();
  },

  /**
   * 设置打印字体大小 (取值: 1-2)
   * @param {number} type
   */
  setFontSize(type) {
    return __Printer__.setFontSize(type);
  },

  /**
   * 按倍数放大打印字体宽高 (取值: 1-2)
   * @param {number} widthMultiple
   * @param {number} heightMultiple
   */
  enlargeFontSize(widthMultiple, heightMultiple) {
    return __Printer__.enlargeFontSize(widthMultiple, heightMultiple);
  },

  /**
   * 设置是否反白打印
   * @param {boolean} mode
   */
  setHighlight(mode) {
    return __Printer__.setHighlight(mode);
  },

  /**
   * 设置打印灰度值 (取值: 0-7)
   * @param {number} level
   */
  setGray(level) {
    return __Printer__.setGray(level);
  },

  /**
   * 设置打印对齐方式, 取值见常量部分ALIGN_*
   * @param {number} mode
   */
  setAlign(mode) {
    return __Printer__.setAlgin(mode);
  },

  /**
   * 添加打印内容
   * @param {string} content
   */
  addString(content) {
    return __Printer__.addString(content);
  },

  /**
   * 清除已添加的打印内容
   */
  clearString() {
    return __Printer__.clearString();
  },

  /**
   * 启动打印, (!) 注意打印机必须是启动状态
   */
  printString() {
    return __Printer__.printString();
  },

  /**
   * 启动打印并走纸
   * @param {number} direction - 走纸方向, 见常量部分DIRECTION_*
   * @param {number} mode - 走纸模式, 见常量部分WALK_*
   * @param {number} lines - 走纸距离, 取值: 1-255
   */
  printStringAndWalk(direction, mode, lines) {
    return __Printer__.printStringAndWalk(direction, mode, lines);
  },

  /**
   * 设置行间距
   * @param {number} lineSpace - 行间距, 取值: 0-255
   */
  setLineSpace(lineSpace) {
    return __Printer__.setLineSpace(lineSpace);
  },

  /**
   * 打印图片, 通过路径指定 (可以为dataUrl)
   * @param {string} url - 图片路径, 宽需小于384像素点且为8的整数倍, 高需为8的整数倍
   * @param {boolean} isBuffer - 是否加入缓冲队列, 使得在printString()时一同打印
   */
  printImage(url, isBuffer) {
    return __Printer__.printImage(toAbsoluteUrl(url), isBuffer);
  },

  /**
   * 搜索打印纸黑标
   * @param {number} searchDistance - 最大走纸行数, 取值: 0-255
   * @param {number} walkDistance - 查找到黑标后的走纸行数, 取值: 0-255
   */
  searchMark(searchDistance, walkDistance) {
    return __Printer__.searchMark(searchDistance, walkDistance);
  },

  /**
   * 设置字体是否加粗
   * @param {boolean} isBold
   */
  setBold(isBold) {
    return __Printer__.setBold(isBold);
  },

  /**
   * 设置字体大小, 取值: 8-64
   * @param {number} size
   */
  setTextSize(size) {
    return __Printer__.setTextSize(size);
  },

  /**
   * 设置是否下划线
   * @param {boolean} mode
   */
  setUnderline(mode) {
    return __Printer__.setUnderline(mode);
  },

  /**
   * 获取输入文本的总宽度
   * @param {string} test
   * @returns {number}
   */
  measureText(test) {
    return __Printer__.measureText(test);
  },

  /**
   * 添加带行内偏移打印内容
   * @param {number} offset - 偏移, 取值: 8的整数倍
   * @param {string} context
   */
  addStringOffset(offset, context) {
    return __Printer__.addStringOffset(offset, context);
  },

  /**
   * 结束当前行
   */
  endLine() {
    return __Printer__.endLine();
  },

  /**
   * 设置是否使用智能排版
   * @param {boolean} isAutoBreak
   */
  autoBreakSet(isAutoBreak) {
    return __Printer__.autoBreakSet(isAutoBreak);
  }
};

/**
 * 二代身份证读取相关功能
 */
export const IdCard = window.__IdCard__ !== undefined && {
  __onSuccess__: undefined,
  __onFailed__: undefined,
  __onTimeout__: undefined,

  /**
   * 读取二代身份证, 结束后调用相应回调函数
   * @param {number} timeout - 超时(ms)
   * @param {function} onSuccess - 成功回调 (data: JSONString, image: Base64String) => Any
   * @param {function} onFailed - 失败回调 (message?: string) => Any
   * @param {function} onTimeout - 超时回调 () => Any
   */
  read(timeout, onSuccess, onFailed = noop, onTimeout = noop) {
    IdCard.__onSuccess__ = function(json, image) {
      onSuccess(JSON.parse(json), image);
    };
    IdCard.__onFailed__ = onFailed || noop;
    IdCard.__onTimeout__ = onTimeout || noop;
    return __IdCard__.checkIdCard(
      timeout,
      "IdCard.__onSuccess__",
      "IdCard.__onFailed__",
      "IdCard.__onTimeout__"
    );
  }
};

/**
 * IC卡功能 TODO
 */
export const Nfc = window.__Nfc__ !== undefined && {};

/**
 * Socket功能 TODO
 */
export const Socket = window.__Socket__ !== undefined && {};

/**
 * 赋值到window中:
 * 1. 不需要import也可以访问这些功能接口(不推荐)
 * 2. 用于在App中调用回调函数(必须)
 */
window.App = App;
window.Barcode = Barcode;
window.Printer = Printer;
window.IdCard = IdCard;
