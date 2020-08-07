import { Printer } from "../handset-sdk.js";
import orderService from "../services/orderService.js";
import qrcodeHelper from "./qrcodeHelper.js";
import { Toast } from "vant";

class ReceiptInfo {
  constructor() {
    this.title = "";
    this.orderCode = "";
    this.time = "";
    this.ticketsNum = "";
    this.money = 0.0;
    this.payType = "";
    this.Cashier = "";
    this.tickets = [
      {
        ticketTypeName: "",
        num: 0,
        etime: "",
        ticketCode: "",
        ticketCodeDataUrl: ""
      }
    ];
  }
}

// class CheckInInfo {
//   constructor() {
//     this.title = "";
//     this.ticketCode = "";
//     this.ticketTypeName = "";
//     this.checkinNum = 0;
//     this.ticketCollector = "";
//     this.checkingTime = "";
//     this.checkinPoint = "";
//   }
// }

export default {
  checkStatus() {
    if (Printer) {
      let status = Printer.checkStatus();
      if (status != 0) {
        let msg = "";
        switch (status) {
          case 1:
            msg = "打印机缺纸";
            break;
          case 2:
            msg = "打印机过热";
            break;
          case 3:
            msg = "打印机栈溢出";
            break;
          default:
            msg = "打印机错误";
        }
        Toast.fail(msg);
        return false;
      } else {
        return true;
      }
    } else {
      Toast.fail("此设备不支持打印功能");
      return false;
    }
  },
  //打印检票凭证
  printCheckIn(CheckInInfo) {
    try {
      if (this.checkStatus()) {
        Printer.start(0); //启动打印机
        Printer.reset();

        Printer.setGray(3); //设置打印灰度值 (取值: 0-7)
        //Printer.setBold(true);//设置字体是否加粗

        Printer.setLineSpace(5);
        Printer.setAlign(1);
        // Printer.addString("检票凭证");
        Printer.addString(CheckInInfo.title);
        Printer.addString("—————————————————————————————");

        Printer.printString();
        Printer.clearString();

        Printer.setLineSpace(3);
        Printer.setAlign(0);
        Printer.addString("票　　类：" + CheckInInfo.ticketTypeName);
        Printer.addString("票　　号：" + CheckInInfo.ticketCode);
        Printer.addString("检票人数：" + CheckInInfo.checkinNum);
        Printer.printString();
        Printer.clearString();
        Printer.addString("检  票 员：" + CheckInInfo.ticketCollector);
        Printer.addString("检票时间：" + CheckInInfo.checkingTime);
        Printer.addString("检票项目：" + CheckInInfo.checkinPoint);

        Printer.printString();
        Printer.clearString();

        Printer.walkPaper(20); //打印机走纸
        Printer.stop(); //停止打印机
      }
    } catch (err) {
      throw new Error("打印机错误");
    }
  },

  //打印门票售销交易凭证
  printReceipt(ReceiptInfo) {
    try {
      if (this.checkStatus()) {
        Printer.start(0); //启动打印机
        Printer.reset(); //重置打印机, 恢复默认设置, 清空打印缓存
        Printer.setGray(3); //设置打印灰度值 (取值: 0-7)
        //Printer.setBold(true);//设置字体是否加粗
        Printer.setLineSpace(5);
        Printer.setAlign(1); //设置打印对齐方式 居中
        // Printer.addString("门票售销交易凭证");
        Printer.addString(ReceiptInfo.title);
        Printer.addString("—————————————————————————————");
        Printer.printString();

        Printer.clearString();
        Printer.setLineSpace(3);
        Printer.setAlign(0);
        Printer.addString("单　　号：" + ReceiptInfo.orderCode);
        Printer.addString("时　　间：" + ReceiptInfo.time);
        Printer.addString("票　　数：" + ReceiptInfo.ticketsNum);
        Printer.addString("金　　额：" + ReceiptInfo.money);
        Printer.addString("付款方式：" + ReceiptInfo.payType);
        Printer.addString("收  银 员：" + ReceiptInfo.Cashier);

        Printer.printString();

        ReceiptInfo.tickets.forEach(async ticket => {
          Printer.walkPaper(5);
          Printer.clearString();
          Printer.setAlign(1); //设置打印对齐方式 居中
          Printer.addString("—————————————————————————————");
          Printer.printString();

          Printer.walkPaper(5);
          Printer.clearString();
          Printer.setLineSpace(3);
          Printer.setAlign(0);
          Printer.addString("票类名称：" + ticket.ticketTypeName);
          Printer.addString("人　　次：" + ticket.num);
          Printer.addString("有效期至：" + ticket.etime);
          Printer.printString();

          Printer.clearString();
          Printer.setLineSpace(3);
          Printer.setAlign(1);

          Printer.printImage(ticket.ticketCodeDataUrl);

          //Printer.printImage("/logo.png");
          Printer.addString(ticket.ticketCode);

          Printer.printString();
        });

        Printer.walkPaper(20); //打印机走纸\
        Printer.stop(); //停止打印机
      }
    } catch (err) {
      throw new Error("打印机错误");
    }
  },
  //通过订单号获取门票售销交易凭证
  async getReceiptInfo(title, payTypeName, listNo) {
    let receiptInfo = new ReceiptInfo();
    let listNoInput = { listNo: listNo };
    await orderService
      .getOrderFullInfoAsync(listNoInput)
      .then(result => {
        receiptInfo.title = title; //"门票售销交易凭证";
        receiptInfo.orderCode = result.listNo;
        receiptInfo.time = result.cTime;
        receiptInfo.money = result.totalMoney;
        receiptInfo.payType = payTypeName;
        receiptInfo.Cashier = sessionStorage.getItem("employeeName");

        receiptInfo.tickets = [];
        let ticketsNum = 0;
        result.details.forEach(detail => {
          ticketsNum += detail.tickets.length;
          detail.tickets.forEach(ticket => {
            receiptInfo.tickets.push({
              ticketTypeName: ticket.ticketTypeName,
              num: ticket.totalNum,
              etime: ticket.etime.split(" ")[0], //有效期至只要显示年月日就ok了，时分秒不用显示。
              ticketCode: ticket.ticketCode
            });
          });
        });
        receiptInfo.ticketsNum = ticketsNum;
      })
      .catch(result => {
        throw new Error(result.response.data.error.message);
      });
    return receiptInfo;
  },
  //打印门票售销交易凭证
  async printReceiptByListNo(title, payTypeName, listNo) {
    try {
      let receiptInfo = await this.getReceiptInfo(title, payTypeName, listNo);
      for (var i = 0; i < receiptInfo.tickets.length; i++) {
        let dataUrl = await qrcodeHelper.createQRCodeAsync(
          receiptInfo.tickets[i].ticketCode
        );
        receiptInfo.tickets[i].ticketCodeDataUrl = dataUrl;
      }
      //打印门票售销交易凭证
      this.printReceipt(receiptInfo);
    } catch (err) {
      throw new Error(err.message);
      //Toast.fail(err.message);
    }
  }
};
