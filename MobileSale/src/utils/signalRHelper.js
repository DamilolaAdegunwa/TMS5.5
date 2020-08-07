import * as signalR from "@aspnet/signalr";
import * as combineURLs from "axios/lib/helpers/combineURLs.js";

export default {
  buildExplainerNotificationConnection() {
    const url = combineURLs(window.baseURL, "/notification?Group=Explainer");
    const connection = new signalR.HubConnectionBuilder().withUrl(url).build();
    return connection;
  },
  buildCheckerNotificationConnection() {
    const url = combineURLs(window.baseURL, "/notification?Group=Checker");
    const connection = new signalR.HubConnectionBuilder().withUrl(url).build();
    return connection;
  }
};
