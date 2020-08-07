import Vue from "vue";
import Router from "vue-router";
import scenicService from "@/services/scenicService.js";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { registPermissionCheck, Permission } from "./permission.js";
import Login from "./../views/staffs/Login.vue";
import Layout from "./../views/layout/Layout.vue";
import ScenicSetting from "@/views/scenics/ScenicSetting.vue";
import ScenicSettingLiangYuan from "@/views/scenics/ScenicSettingLiangYuan.vue";

Vue.use(Router);

let routes = [];
export default {
  async getRouter() {
    await scenicService.setScenicObject();

    let iScenicSetting = ScenicSetting;
    switch (scenicService.getScenicObject()) {
      case "梁园": iScenicSetting = ScenicSettingLiangYuan; break;
    }

    NProgress.configure({ showSpinner: false });

    routes = [
      {
        path: "/Login",
        name: "Login",
        component: Login,
        meta: { title: "登录", allowAnonymous: true, noCache: true }
      },
      {
        path: "/",
        name: "Home",
        redirect: "/Dashboard",
        component: Layout,
        meta: { title: "首页", icon: "al-icon-home", noCache: false },
        children: [
          {
            path: "Dashboard",
            name: "Dashboard",
            component: () => import("@/views/dashboards/Dashboard.vue"),
            meta: { title: "首页", icon: "al-icon-home", noCache: false }
          }
        ]
      },
      {
        path: "/TicketTypes",
        name: "TicketTypes",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "票务管理",
          icon: "al-icon-chanpin",
          noCache: false
        },
        children: [
          {
            path: "Description",
            name: "TicketTypeDescription",
            component: () => import("@/views/ticketTypes/Description.vue"),
            meta: {
              title: "票类说明",
              noCache: false,
              permission: Permission.TMSAdmin_TicketTypeDescription
            }
          }
        ]
      },
      {
        path: "/Scenics",
        name: "Scenics",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "景区管理",
          icon: "al-icon-jingqu",
          noCache: false
        },
        children: [
          {
            path: "ScenicSetting",
            name: "ScenicSetting",
            component: iScenicSetting,
            meta: {
              title: "景区设置",
              noCache: false,
              permission: Permission.TMSAdmin_ScenicSetting
            }
          }
        ]
      },
      {
        path: "/Orders",
        name: "Orders",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "预订管理",
          icon: "al-icon-dingdan",
          noCache: false
        },
        children: [
          {
            path: "OrderManage",
            name: "OrderManage",
            component: () => import("@/views/orders/OrderManage.vue"),
            meta: {
              title: "订单查询",
              noCache: false,
              permission: Permission.TMSAdmin_OrderManage
            }
          }
        ]
      },
      {
        path: "/Search",
        name: "Search",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "查询",
          icon: "el-icon-search",
          noCache: false
        },
        children: [
          {
            path: "TicketSale",
            name: "SearchTicketSale",
            component: () => import("@/views/search/TicketSale.vue"),
            meta: {
              title: "售票查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchTicketSale
            }
          },
          {
            path: "Trade",
            name: "SearchTrade",
            component: () => import("@/views/search/Trade.vue"),
            meta: {
              title: "交易查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchTrade
            }
          },
          {
            path: "TicketCheck",
            name: "SearchTicketCheck",
            component: () => import("@/views/search/TicketCheck.vue"),
            meta: {
              title: "检票查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchTicketCheck
            }
          },
          {
            path: "TicketConsume",
            name: "SearchTicketConsume",
            component: () => import("@/views/search/TicketConsume.vue"),
            meta: {
              title: "核销查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchTicketConsume
            }
          },
          {
            path: "ReprintLog",
            name: "SearchReprintLog",
            component: () => import("@/views/search/ReprintLog.vue"),
            meta: {
              title: "重印查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchReprintLog
            }
          },
          {
            path: "ExchangeHistory",
            name: "SearchExchangeHistory",
            component: () => import("@/views/search/ExchangeHistory.vue"),
            meta: {
              title: "换票查询",
              noCache: false,
              permission: Permission.TMSAdmin_SearchExchangeHistory
            }
          },
          {
            path: "CzkDetail",
            name: "QueryCzkDetail",
            component: () => import("@/views/search/CzkDetail.vue"),
            meta: {
              title: "储值卡明细查询",
              noCache: false,
              permission: Permission.TMSAdmin_QueryCzkDetail
            }
          }
        ]
      },
      {
        path: "/StatTicketSales",
        name: "StatTicketSales",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "售票统计",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "StatTicketSale",
            name: "StatTicketSale",
            component: () => import("@/views/stat/ticketSales/StatTicketSale.vue"),
            meta: {
              title: "售票统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSale
            }
          },
          {
            path: "StatCashierSale",
            name: "StatCashierSale",
            component: () => import("@/views/stat/ticketSales/StatCashierSale.vue"),
            meta: {
              title: "售票员销售汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatCashierSale
            }
          },
          {
            path: "StatByTradeSource",
            name: "StatTicketSaleByTradeSource",
            component: () => import("@/views/stat/ticketSales/StatByTradeSource.vue"),
            meta: {
              title: "售票按购买类型统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleByTradeSource
            }
          },
          {
            path: "StatByTicketTypeClass",
            name: "StatTicketSaleByTicketTypeClass",
            component: () => import("@/views/stat/ticketSales/StatByTicketTypeClass.vue"),
            meta: {
              title: "售票按票种统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleByTicketTypeClass
            }
          },
          {
            path: "StatBySalePoint",
            name: "StatTicketSaleBySalePoint",
            component: () => import("@/views/stat/ticketSales/StatBySalePoint.vue"),
            meta: {
              title: "售票按售票点统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleBySalePoint
            }
          },
          {
            path: "StatByCustomer",
            name: "StatTicketSaleByCustomer",
            component: () => import("@/views/stat/ticketSales/StatByCustomer.vue"),
            meta: {
              title: "客户门票销售统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleByCustomer
            }
          },
          {
            path: "StatGroundSharing",
            name: "StatTicketSaleGroundSharing",
            component: () => import("@/views/stat/ticketSales/StatGroundSharing.vue"),
            meta: {
              title: "项目分成统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleGroundSharing
            }
          },
          {
            path: "StatGroundChangCiSale",
            name: "StatGroundChangCiSale",
            component: () => import("@/views/stat/ticketSales/StatGroundChangCiSale.vue"),
            meta: {
              title: "场次座位销售统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatGroundChangCiSale
            }
          },
          {
            path: "StatPromoterSale",
            name: "StatPromoterSale",
            component: () => import("@/views/stat/ticketSales/StatPromoterSale.vue"),
            meta: {
              title: "营销推广员销售汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatPromoterSale
            }
          }, {
            path: "StatCzkSale",
            name: "StatCzkSale",
            component: () => import("@/views/stat/ticketSales/StatCzkSale.vue"),
            meta: {
              title: "储值卡销售统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatCzkSale
            }
          }
        ]
      },
      {
        path: "/StatTicketChecks",
        name: "StatTicketChecks",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "检票统计",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "StatCheckIn",
            name: "StatTicketCheckIn",
            component: () => import("@/views/stat/ticketChecks/StatCheckIn.vue"),
            meta: {
              title: "检票入园统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketCheckIn
            }
          },
          {
            path: "StatByPark",
            name: "StatTicketCheckByPark",
            component: () => import("@/views/stat/ticketChecks/StatByPark.vue"),
            meta: {
              title: "检票按景区统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketCheckByPark
            }
          },
          {
            path: "StatByGateGroup",
            name: "StatTicketCheckByGateGroup",
            component: () => import("@/views/stat/ticketChecks/StatByGateGroup.vue"),
            meta: {
              title: "检票按检票点统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketCheckByGateGroup
            }
          },
          {
            path: "StatTicketConsume",
            name: "StatTicketConsume",
            component: () => import("@/views/stat/ticketChecks/StatTicketConsume.vue"),
            meta: {
              title: "核销统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketConsume
            }
          },
          {
            path: "StatTouristNum",
            name: "StatTouristNum",
            component: () => import("@/views/stat/ticketChecks/StatTouristNum.vue"),
            meta: {
              title: "客流统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTouristNum
            }
          },
          {
            path: "StatTicketCheckDayIn",
            name: "StatTicketCheckDayIn",
            component: () => import("@/views/stat/ticketChecks/StatCheckDayIn.vue"),
            meta: {
              title: "客流按日期统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketCheckDayIn
            }
          }
        ]
      },
      {
        path: "/StatPayment",
        name: "StatPayment",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "收款统计",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "StatPayDetail",
            name: "StatPayDetail",
            component: () => import("@/views/stat/payment/StatPayDetail.vue"),
            meta: {
              title: "收款汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatPayDetail
            }
          },
          {
            path: "StatTradeByPayType",
            name: "StatTradeByPayType",
            component: () => import("@/views/stat/payment/StatTradeByPayType.vue"),
            meta: {
              title: "交易收款汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatTradeByPayType
            }
          },
          {
            path: "StatTicketSaleByPayType",
            name: "StatTicketSaleByPayType",
            component: () => import("@/views/stat/payment/StatTicketSaleByPayType.vue"),
            meta: {
              title: "门票收款汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketSaleByPayType
            }
          },
          {
            path: "StatPayDetailByCustomer",
            name: "StatPayDetailByCustomer",
            component: () => import("@/views/stat/payment/StatPayDetailByCustomer.vue"),
            meta: {
              title: "客户收款汇总",
              noCache: false,
              permission: Permission.TMSAdmin_StatPayDetailByCustomer
            }
          }
        ]
      },
      {
        path: "/WareManage",
        name: "WareManage",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "商品报表",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "SearchWare",
            name: "SearchWare",
            component: () => import("@/views/wares/SearchWare.vue"),
            meta: {
              title: "商品查询",
              noCache: false,
              permission: Permission.TMSAdmin_WareSearchWare
            }
          },
          {
            path: "SearchWareIODetail",
            name: "SearchWareIODetail",
            component: () => import("@/views/wares/SearchWareIODetail.vue"),
            meta: {
              title: "商品租售查询",
              noCache: false,
              permission: Permission.TMSAdmin_WareSearchWareIODetail
            }
          },
          {
            path: "SearchWareTrade",
            name: "SearchWareTrade",
            component: () => import("@/views/wares/SearchWareTrade.vue"),
            meta: {
              title: "商品交易查询",
              noCache: false,
              permission: Permission.TMSAdmin_WareSearchWareTrade
            }
          },
          {
            path: "StatWareTrade",
            name: "StatWareTrade",
            component: () => import("@/views/wares/StatWareTrade.vue"),
            meta: {
              title: "商品交易统计",
              noCache: false,
              permission: Permission.TMSAdmin_WareStatWareTrade
            }
          },
          {
            path: "StatWareSaleByWareType",
            name: "StatWareSaleByWareType",
            component: () => import("@/views/wares/StatWareSaleByWareType.vue"),
            meta: {
              title: "商品销售按类型统计",
              noCache: false,
              permission: Permission.TMSAdmin_WareStatWareSaleByWareType
            }
          },
          {
            path: "SataWareRentSale",
            name: "StatWareRentSale",
            component: () => import("@/views/wares/StatWareRentSale.vue"),
            meta: {
              title: "商品租售统计",
              noCache: false,
              permission: Permission.TMSAdmin_WareStatWareRentSale
            }
          },
          {
            path: "StatWareSale",
            name: "StatWareSale",
            component: () => import("@/views/wares/StatWareSale.vue"),
            meta: {
              title: "商店销售明细统计",
              noCache: false,
              permission: Permission.TMSAdmin_WareStatWareSale
            }
          },
          {
            path: "SataWareTradeTotal",
            name: "StatWareTradeTotal",
            component: () => import("@/views/wares/StatWareTradeTotal.vue"),
            meta: {
              title: "商店销售金额统计",
              noCache: false,
              permission: Permission.TMSAdmin_WareStatWareTradeTotal
            }
          }
        ]
      },
      {
        path: "/StatTouristFlow",
        name: "StatTouristFlow",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "客流统计",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "StatTicketCheckInAverage",
            name: "StatTicketCheckInAverage",
            component: () => import("@/views/stat/touristFlow/StatTicketCheckInAverage.vue"),
            meta: {
              title: "高峰期统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatTicketCheckInAverage
            }
          },
          {
            path: "TradeSource",
            name: "TradeSource",
            component: () => import("@/views/stat/touristFlow/TradeSource.vue"),
            meta: {
              title: "客流来源统计",
              noCache: false,
              permission: Permission.TMSAdmin_TradeSource
            }
          },
          {
            path: "Exposition",
            name: "Exposition",
            component: () => import("@/views/stat/touristFlow/Exposition.vue"),
            meta: {
              title: "展厅客流统计",
              noCache: false,
              permission: Permission.TMSAdmin_Exposition
            }
          },
          {
            path: "YearToYear",
            name: "YearToYear",
            component: () => import("@/views/stat/touristFlow/YearToYear.vue"),
            meta: {
              title: "年度对比统计",
              noCache: false,
              permission: Permission.TMSAdmin_YearToYear
            }
          }
        ]
      },
      {
        path: "/StatTouristInfo",
        name: "StatTouristInfo",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "游客信息统计",
          icon: "al-icon-tongji",
          noCache: false
        },
        children: [
          {
            path: "StatByAgeRange",
            name: "StatByAgeRange",
            component: () => import("@/views/stat/touristInfo/StatByAgeRange.vue"),
            meta: {
              title: "游客年龄段统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatByAgeRange
            }
          },
          {
            path: "StatByArea",
            name: "StatByArea",
            component: () => import("@/views/stat/touristInfo/StatByArea.vue"),
            meta: {
              title: "游客地区统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatByArea
            }
          },
          {
            path: "StatBySex",
            name: "StatBySex",
            component: () => import("@/views/stat/touristInfo/StatBySex.vue"),
            meta: {
              title: "游客性别统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatBySex
            }
          }
        ]
      },
      {
        path: "/ShiftManage",
        name: "ShiftManage",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "交班管理",
          icon: "al-icon-workclock",
          noCache: false
        },
        children: [
          {
            path: "StatShift",
            name: "StatShift",
            component: () => import("@/views/stat/shift/StatShift.vue"),
            meta: {
              title: "交班统计",
              noCache: false,
              permission: Permission.TMSAdmin_StatShift
            }
          }
        ]
      },
      {
        path: "/RightManage",
        name: "RightManage",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "权限管理",
          icon: "el-icon-setting",
          noCache: false
        },
        children: [
          {
            path: "Staff",
            name: "Staff",
            component: () => import("@/views/staffs/Staff.vue"),
            meta: {
              title: "员工设置",
              noCache: false,
              permission: Permission.TMSAdmin_Staff
            }
          }
        ]
      },
      {
        path: "/SystemSetting",
        name: "SystemSetting",
        component: Layout,
        alwaysShow: true,
        meta: {
          title: "系统设置",
          icon: "el-icon-setting",
          noCache: false
        },
        children: [
          {
            path: "ThirdPlatform",
            name: "ThirdPlatform",
            component: () => import("@/views/thirdPlatforms/ThirdPlatform.vue"),
            meta: {
              title: "第三方平台设置",
              noCache: false,
              permission: Permission.TMSAdmin_ThirdPlatformSetting
            }
          }
        ]
      }
    ];

    let myRouter = new Router({
      mode: "history",
      base: process.env.BASE_URL,
      routes: routes,
      scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
          return savedPosition;
        } else {
          return { x: 0, y: 0 };
        }
      }
    });

    myRouter.beforeEach((to, from, next) => {
      NProgress.start();
      document.title = to.meta.title;

      next();
    });
    myRouter.afterEach(() => {
      NProgress.done();
    });

    registPermissionCheck(myRouter);
    return myRouter;
  },
  getRoutes() {
    return routes
  }
}

// export { routes };
