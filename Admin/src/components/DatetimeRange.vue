<template>
  <el-date-picker
    :value="selectedValue"
    :type="type"
    :picker-options="pickerOptions"
    range-separator="至"
    :start-placeholder="startPlaceholder"
    :end-placeholder="endPlaceholder"
    align="right"
    :value-format="valueFormat"
    :clearable="clearable"
    v-bind="$attrs"
    @input="onInput"
  ></el-date-picker>
</template>

<script>
export default {
  name: "datetime-range",
  props: {
    value: {},
    type: {
      type: String,
      default: "datetimerange"
    },
    clearable: {
      type: Boolean,
      default: false
    }
  },
  data() {
    const now = new Date();
    const end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59);
    return {
      pickerOptions: {
        shortcuts: [
          {
            text: "最近一周",
            onClick(picker) {
              const start = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 7);
              picker.$emit("pick", [start, end]);
            }
          },
          {
            text: "最近一个月",
            onClick(picker) {
              const start = new Date(now.getFullYear(), now.getMonth() - 1, now.getDate());
              picker.$emit("pick", [start, end]);
            }
          },
          {
            text: "最近三个月",
            onClick(picker) {
              const start = new Date(now.getFullYear(), now.getMonth() - 3, now.getDate());
              picker.$emit("pick", [start, end]);
            }
          },
          {
            text: "最近半年",
            onClick(picker) {
              const start = new Date(now.getFullYear(), now.getMonth() - 6, now.getDate());
              picker.$emit("pick", [start, end]);
            }
          },
          {
            text: "最近一年",
            onClick(picker) {
              const start = new Date(now.getFullYear() - 1, now.getMonth(), now.getDate());
              picker.$emit("pick", [start, end]);
            }
          }
        ]
      },
      selectedValue: this.value
    };
  },
  computed: {
    startPlaceholder() {
      if (this.type === "daterange") {
        return "起始日期";
      }
      return "起始时间";
    },
    endPlaceholder() {
      if (this.type === "daterange") {
        return "截止日期";
      }
      return "截止时间";
    },
    valueFormat() {
      if (this.type === "daterange") {
        return "yyyy-MM-dd";
      }
      return "yyyy-MM-dd HH:mm:ss";
    }
  },
  watch: {
    value(val) {
      this.selectedValue = val;
    }
  },
  methods: {
    onInput(val) {
      this.selectedValue = val;
      this.$emit("input", val);
    }
  }
};
</script>
