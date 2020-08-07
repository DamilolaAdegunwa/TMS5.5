<template>
  <div class="van-hairline--bottom">
    <van-field
      :label="label"
      :value="currentText"
      :placeholder="placeholder"
      readonly
      @focus="onFocus"
      @click="onClick"
    />
    <van-popup v-model="showPopup" position="bottom">
      <van-picker
        show-toolbar
        :columns="columns"
        :loading="loading"
        @cancel="onCancel"
        @confirm="onConfirm"
        @change="onChange"
      />
    </van-popup>
  </div>
</template>

<script>
export default {
  name: "multi-picker-field",
  props: {
    label: {
      type: String
    },
    placeholder: {
      type: String,
      default: ""
    },
    value: {
      type: String
    },
    loading: {
      type: Boolean,
      default: false
    },
    columnNum: {
      type: Number,
      default: 1
    },
    list: {
      type: Array,
      default() {
        return [];
      }
    },
    emptyText: {
      type: String,
      default: ""
    }
  },
  data() {
    return {
      currentText: "",
      showPopup: false
    };
  },
  computed: {
    columns() {
      if (!this.list || this.list.length === 0) {
        if (this.emptyText) {
          return [{ text: this.emptyText, disabled: true }];
        }
        return this.list;
      }

      if (this.columnNum === 1) {
        if (!this.list[0].children || this.list[0].children.length === 0) {
          return [{ text: this.emptyText, disabled: true }];
        }
        let column = { values: this.list[0].children, defaultIndex: 0 };
        if (this.value) {
          for (let i = 0; i < column.values.length; i++) {
            if (this.value == column.values[i].value) {
              column.defaultIndex = i;
            }
          }
        }

        return [column];
      } else if (this.columnNum === 2) {
        let defaultValues = [];
        if (this.value) {
          defaultValues = this.value.split(",");
        }

        let column1 = { values: [], defaultIndex: 0 };
        for (let i = 0; i < this.list.length; i++) {
          column1.values.push({
            text: this.list[i].text,
            value: this.list[i].value,
            disabled: this.list[i].disabled
          });

          if (defaultValues[0] == this.list[i].value) {
            column1.defaultIndex = i;
          }
        }

        let column2 = {
          values: this.list[column1.defaultIndex].children,
          defaultIndex: 0
        };

        for (let i = 0; i < column2.values.length; i++) {
          if (defaultValues[1] == column2.values[i].value) {
            column2.defaultIndex = i;
          }
        }

        return [column1, column2];
      }

      return [];
    }
  },
  watch: {
    value: function(val) {
      if (!val) {
        this.currentText = "";
      }
    }
  },
  methods: {
    onFocus() {
      document.activeElement.blur();
    },
    onClick() {
      this.showPopup = true;
    },
    onCancel() {
      this.showPopup = false;
    },
    onConfirm(values) {
      if (!values) {
        return;
      }
      for (let i = 0; i < values.length; i++) {
        if (values[i] == undefined) {
          return;
        }
      }

      this.currentText = "";
      let value = "";

      for (let i = 0; i < values.length; i++) {
        if (this.currentText.length > 0) {
          this.currentText += "，";
        }
        this.currentText += values[i].text;

        if (value.length > 0) {
          value += ",";
        }
        value += values[i].value;
      }

      this.$emit("input", value);

      this.showPopup = false;
    },
    onChange(picker, values) {
      if (values.length === 2) {
        let index = -1;
        for (let i = 0; i < this.columns[0].values.length; i++) {
          if (values[0].value == this.columns[0].values[i].value) {
            index = i;
            break;
          }
        }
        if (index !== -1) {
          picker.setColumnValues(1, this.list[index].children);
        }
      }
    }
  },
  created() {
    if (this.value) {
      for (let i = 0; i < this.columns.length; i++) {
        if (this.currentText.length > 0) {
          this.currentText += "，";
        }
        this.currentText += this.columns[i].values[this.columns[i].defaultIndex].text;
      }
    }
  }
};
</script>
