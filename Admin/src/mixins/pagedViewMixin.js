const pagedViewMixin = {
  data() {
    return {
      input: new PagedInputDto(),
      showAdvanced: false,
      sm: 12,
      md: 8,
      lg: 6,
      tableData: [],
      pageSizes: [30, 50, 100],
      currentPage: 1,
      totalResultCount: 0,
      loading: false,
      tableHeight: 600
    };
  },
  computed: {
    skipCount() {
      return (this.currentPage - 1) * this.input.maxResultCount;
    }
  },
  watch: {
    input: {
      handler: function() {
        this.clearTable();
      },
      deep: true
    },
    showAdvanced() {
      this.setTableHeight();
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
    handleSizeChange(val) {
      this.input.maxResultCount = val;
      this.query();
      this.resetTableScroll();
    },
    handleCurrentChange(val) {
      this.currentPage = val;
      this.query();
      this.resetTableScroll();
    },
    query() {
      this.$refs.searchForm.validate(async valid => {
        if (!valid) {
          return false;
        }

        try {
          this.loading = true;
          let queryInput = { ...this.input };
          queryInput.skipCount = this.skipCount;
          const result = await this.getData(queryInput);
          this.tableData = result.items;
          this.totalResultCount = result.totalCount;
        } catch (error) {
          return;
        } finally {
          this.loading = false;
        }
      });
    },
    async getData() {},
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
      this.clearTable();
    },
    clearTable() {
      this.currentPage = 1;
      this.totalResultCount = 0;
      this.tableData = [];
      this.resetTableScroll();
    },
    resetTableScroll() {
      this.$nextTick(() => {
        document.querySelector(".el-table__body-wrapper").scrollTo(0, 0);
      });
    }
  }
};

class PagedInputDto {
  constructor() {
    this.maxResultCount = 30;
    this.skipCount = 0;
  }
}

export { pagedViewMixin, PagedInputDto };
