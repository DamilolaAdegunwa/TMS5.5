<template>
  <el-date-picker
    :value="selectedValue"
    :default-value="defaultValue"
    :type="type"
    :placeholder="placeholder"
    align="right"
    :picker-options="pickerOptions"
    :value-format="valueFormat"
    :clearable="clearable"
    v-bind="$attrs"
    @input="onInput"
  ></el-date-picker>
</template>

<script>
export default {
  name: "shortcut-datetime-picker",
  props: {
    value: {},
    defaultValue: {},
    type: {
      type: String,
      default: "datetime"
    },
    placeholder: String,
    clearable: {
      type: Boolean,
      default: false
    }
  },
  data() {
    const now = new Date();
    return {
      pickerOptions: {
        shortcuts: [
          {
            text: "昨天",
            onClick(picker) {
              const date = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1);
              picker.$emit("pick", date);
            }
          },
          {
            text: "一周前",
            onClick(picker) {
              const date = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 7);
              picker.$emit("pick", date);
            }
          },
          {
            text: "一个月前",
            onClick(picker) {
              const date = new Date(now.getFullYear(), now.getMonth() - 1, now.getDate());
              picker.$emit("pick", date);
            }
          },
          {
            text: "三个月前",
            onClick(picker) {
              const date = new Date(now.getFullYear(), now.getMonth() - 3, now.getDate());
              picker.$emit("pick", date);
            }
          },
          {
            text: "半年前",
            onClick(picker) {
              const date = new Date(now.getFullYear(), now.getMonth() - 6, now.getDate());
              picker.$emit("pick", date);
            }
          },
          {
            text: "一年前",
            onClick(picker) {
              const date = new Date(now.getFullYear() - 1, now.getMonth(), now.getDate());
              picker.$emit("pick", date);
            }
          }
        ]
      },
      selectedValue: this.value
    };
  },
  computed: {
    valueFormat() {
      if (this.type == "date") {
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
