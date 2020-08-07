import tokenService from "./../services/tokenService.js";
import { staff } from "./../store/types.js";

const Permission = {
  TMSAdmin_TicketTypeDescription: "202F33F2-83F5-484F-AB51-5E03D9F1CFE0",
  TMSAdmin_ScenicSetting: "5C9A2210-361C-47D8-BC96-EF913464B1C0",
  TMSAdmin_OrderManage: "E324F6C0-9939-46AB-889F-8C7E3C0B092F",
  TMSAdmin_SearchTicketSale: "468F9E8A-A1AE-469A-9A9C-16EB953377D0",
  TMSAdmin_SearchTrade: "0AD0191D-CDEE-4F57-8769-A3B7E8A1ACD2",
  TMSAdmin_SearchTicketCheck: "421D70CD-DFB1-45C9-B60C-327B34D23FC6",
  TMSAdmin_SearchTicketConsume: "9255D0F1-CFD3-4313-AF5D-CA1200D637FD",
  TMSAdmin_SearchReprintLog: "48F41D57-BDA8-4760-BBF8-BF668C6E59AB",
  TMSAdmin_SearchExchangeHistory: "4FF585ED-393E-4B98-ABE7-505EFBE57A5A",
  TMSAdmin_QueryCzkDetail: "E8F9C0B5-E443-4088-91A0-83A7D3761EEB",
  TMSAdmin_StatTicketSale: "60548314-A7D5-47DB-A13D-54211EE916C0",
  TMSAdmin_StatCashierSale: "DEFC725C-6D04-406B-A718-6E72AB1ABB82",
  TMSAdmin_StatPromoterSale: "CDA6D301-CAB6-44F1-8A71-E1F38D684582",
  TMSAdmin_StatTicketSaleByTradeSource: "E528B265-4197-4285-BC37-853E49E9CF0C",
  TMSAdmin_StatTicketSaleByTicketTypeClass: "E2FA39C9-FE7E-4146-A653-FEDBA9B6B441",
  TMSAdmin_StatTicketSaleBySalePoint: "BA984D2C-D896-4C6A-A6AA-AAFEE0D68A80",
  TMSAdmin_StatTicketSaleByCustomer: "6135E9C3-2AE6-4855-9136-887D7FD8BAF0",
  TMSAdmin_StatTicketSaleGroundSharing: "754FEF70-4DA3-428C-9A49-26DB204AFB70",
  TMSAdmin_StatGroundChangCiSale: "96FB052B-E4F2-49EE-B8B5-E54FE8B6F160",
  TMSAdmin_StatTicketCheckIn: "C6EE7CDD-C596-4ACA-915D-817C19DD77EE",
  TMSAdmin_StatTicketCheckDayIn: "84E66A6D-426B-4459-B04B-3190A2F5FE7B",
  TMSAdmin_StatTicketCheckByPark: "F5DACB0F-7F4A-4231-ACA3-60C8B47D8568",
  TMSAdmin_StatTicketCheckByGateGroup: "C83820AC-2DFC-42AE-946B-79EBC89B7294",
  TMSAdmin_StatTicketConsume: "DA4CE80B-DB56-4ECC-B3B6-D894E9765AE9",
  TMSAdmin_StatPayDetail: "2016E20E-4FEC-44C2-B465-C9A7E084CB90",
  TMSAdmin_StatPayDetailByCustomer: "62C96462-0E14-426F-8DE0-A120FAAE2AB2",
  TMSAdmin_StatTradeByPayType: "9DC6F782-5E7C-4C10-9987-740B5767BC21",
  TMSAdmin_StatTicketSaleByPayType: "A63E8A2C-4962-4F85-A736-83DB30E60D21",
  TMSAdmin_StatShift: "0CD6324C-FD4C-4D13-B192-0413B348AED4",
  TMSAdmin_StatTouristNum: "BD155F6F-99F0-46F4-9EE8-E9D2ABA578A5",
  TMSAdmin_StatCzkSale: "FAE1DF5A-7038-4793-9F5E-28F2FE616C96",
  TMSAdmin_Staff: "00D62451-EC01-43D2-A448-8F1C6FB0C8B3",
  TMSAdmin_ThirdPlatformSetting: "81F20535-1C7C-48C8-9DE3-B8485B415EB9",
  TMSAdmin_WareSearchWare: "3768B463-16BF-4324-B0AF-7D73F2E7835B",
  TMSAdmin_WareSearchWareIODetail: "8BFF05FE-5B65-4EE2-96C0-AE22AF1F654E",
  TMSAdmin_WareStatWareRentSale: "A23E44D4-1C43-44CC-8ABE-27558B70C484",
  TMSAdmin_WareSearchWareTrade: "370936C3-2413-4320-9CB5-C54E59F3046D",
  TMSAdmin_WareStatWareTrade: "21151729-B191-4A09-9203-674DB5743A69",
  TMSAdmin_WareStatWareSale: "769A700B-A882-46F1-B44F-D557B914677F",
  TMSAdmin_WareStatWareTradeTotal: "19D768DF-FDDA-4D98-BF2B-CAC879FF2A48",
  TMSAdmin_WareStatShopIncome: "292B6448-5566-445B-A23F-1BB15E1B62A3",
  TMSAdmin_WareStatWareSaleByWareType: "774D668D-8C6F-4EC3-B1F2-1DBE0D3E657F",

  //---------------------------------------------------------------------
  
  

  TMSAdmin_StatTicketCheckInAverage: "D3903586-D329-4474-8EA4-EC7399F49D7C",
  TMSAdmin_Exposition: "0FE377CA-DF90-4DE7-839C-3B94C9C9E238",
  TMSAdmin_TradeSource: "BAD043A7-D82C-4720-B752-C1F2259AC9AC",
  TMSAdmin_YearToYear: "5F49D4B1-0A1F-46F8-84D9-7235C65840C9",

  TMSAdmin_StatByAgeRange: "9148AA4B-4FDE-4E5A-9C19-AF7BE5413A35",
  TMSAdmin_StatByArea: "3665FF2F-5B99-4FA5-A9A0-D99D7A172F72",
  TMSAdmin_StatBySex: "5D1CD290-293F-4AD7-A38C-69F25121DEA6",
};

function registPermissionCheck(router) {
  router.beforeEach((to, from, next) => {
    if (to.matched.length === 0) {
      next({ name: "Login" });
      return;
    }

    if (to.meta && to.meta.allowAnonymous) {
      next();
      return;
    }

    const token = tokenService.getToken();
    if (!token) {
      next({ name: "Login" });
      return;
    }

    if (to.meta && to.meta.permission) {
      const permissionStr = sessionStorage.getItem(staff.permissions);
      if (!permissionStr) {
        next({ name: "Login" });
        return;
      }
      const permissions = JSON.parse(permissionStr);
      if (!hasPermission(to, permissions)) {
        next({ name: "Login" });
        return;
      }
    }

    next();
  });
}

function filterAuthorisedRoutes(routes, permissions) {
  let authorisedRoutes = [];
  routes.forEach(route => {
    if (hasPermission(route, permissions)) {
      let tempRoute = { ...route };
      if (tempRoute.children) {
        tempRoute.children = filterAuthorisedRoutes(tempRoute.children, permissions);
      }
      authorisedRoutes.push(tempRoute);
    }
  });

  return authorisedRoutes;
}

function hasPermission(route, permissions) {
  if (route.meta && route.meta.permission) {
    return permissions.some(
      permission => permission.toLowerCase() === route.meta.permission.toLowerCase()
    );
  }

  return true;
}

export { registPermissionCheck, filterAuthorisedRoutes, Permission };
