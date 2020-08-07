const statViewMixin = {
  data() {
    return {
      showAdvanced: false,
      sm: 12,
      md: 8,
      lg: 6,
      chartData: new StatResult(),
      tableData: new StatResult(),
      loading: false,
      tableHeight: 600
    };
  },
  computed: {
    hasChartData() {
      return this.chartData.rows.length > 0;
    },
    hasTableColumn() {
      return this.tableData.columns.length > 0;
    },
    hasTableData() {
      return this.tableData.rows.length > 0;
    }
  },
  mounted() {
    this.setTableHeight();
  },
  methods: {
    setTableHeight() {
      this.$nextTick(() => {
        this.tableHeight = this.$refs.tableBox.clientHeight;
      });
    },
    async submit(action) {
      try {
        this.loading = true;
        await action();
      } catch (error) {
        return;
      } finally {
        this.loading = false;
      }
    },
    exportData() {
      this.$refs.searchForm.validate(async valid => {
        if (!valid) {
          return false;
        }

        try {
          this.loading = true;
          await this.exportToExcel();
        } catch (error) {
          return;
        } finally {
          this.loading = false;
        }
      });
    },
    async exportToExcel() {},
    clear() {
      this.showAdvanced = false;
      this.chartData = new StatResult();
      this.tableData = new StatResult();
    },
    formatter(row, column, cellValue) {
      if (cellValue === null || cellValue === undefined || cellValue === "") {
        return "0";
      }
      return cellValue;
    }
  }
};

class StatResult {
  constructor() {
    this.columns = [];
    this.rows = [];
  }
}

export { statViewMixin, StatResult };
