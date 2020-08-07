import moment from "moment";

const startTime = moment().format("YYYY-MM-DD 00:00:00");
const endTime = moment().format("YYYY-MM-DD 23:59:59");

const startDate = startTime.substr(0, 10);
const endDate = endTime.substr(0, 10);

const datetimeFormat = "YYYY-MM-DD HH:mm:ss";
const dateFormat = "YYYY-MM-DD";

function getEndDate(date) {
  return moment(date)
    .add(1, "days")
    .format(dateFormat);
}

export { startTime, endTime, startDate, endDate, datetimeFormat, dateFormat, getEndDate };
