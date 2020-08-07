import qs from "qs";
import ipHelper from "./../utils/ipHelper.js";
import combineURLs from "axios/lib/helpers/combineURLs.js";
import tokenService from "./../services/tokenService.js";
import scenicService from "./../services/scenicService.js";
import staffService from "./../services/staffService.js";

const reportViewMixin = {
  data() {
    return {
      input: new ReportInputDto(),
      scenicName: "",
      staffName: "",
      reportUrl: "",
      exportUrl: "",
      sm: 12,
      md: 8,
      lg: 6,
      loading: false
    };
  },
  computed: {
    isLg() {
      const width = document.documentElement.clientWidth;
      return width >= 1200 && width < 1920;
    }
  },
  created() {
    scenicService.getScenicOptions().then(data => {
      this.scenicName = data.scenicName;
    });
    const staffInfo = staffService.getStaffInfo();
    this.staffName = staffInfo.name;
  },
  methods: {
    submit(action) {
      this.$refs.searchForm.validate(valid => {
        if (!valid) {
          return false;
        }

        this.input.token = tokenService.getToken();
        this.input.random = new Date().getMilliseconds();
        this.input.scenicName = this.scenicName;
        this.input.staffName = this.staffName;

        this.loading = true;

        action();
      });
    },
    onload() {
      this.loading = false;
    },
    getReport(url) {
      this.input.height = this.$refs.iframe.clientHeight - 3;
      this.input.isExport = false;

      this.reportUrl = combineURLs(this.getBaseUrl(), `${url}?${qs.stringify(this.input)}`);
    },
    clear() {
      this.reportUrl = "";
    },
    exportReport(url) {
      this.input.isExport = true;

      this.exportUrl = combineURLs(this.getBaseUrl(), `${url}?${qs.stringify(this.input)}`);

      setTimeout(() => {
        this.exportUrl = "";
      }, 1000);
    },
    getBaseUrl() {
      if (ipHelper.isIntranetIp(window.location.href)) {
        return window.reportIntranetUrl;
      }
      return window.reportOuterNetUrl;
    }
  }
};

class ReportInputDto {
  constructor() {
    this.token = "";
    this.random = 0;
    this.height = 0;
    this.isExport = false;
    this.scenicName = "";
    this.staffName = "";
  }
}

export { reportViewMixin, ReportInputDto };
