<template>
  <div>
    <van-field
      :label="label"
      :value="currentText"
      :placeholder="placeholder"
      readonly
      is-link
      @focus="onFocus"
      @click="onClick"
    />
    <van-popup v-model="showPopup" position="bottom">
      <van-picker
        show-toolbar
        :columns="columns"
        :loading="loading"
        :value-key="textKey"
        @cancel="onCancel"
        @confirm="onConfirm"
      />
    </van-popup>
  </div>
</template>

<script>
export default {
  name: "picker-field",
  props: {
    label: {
      type: String
    },
    placeholder: {
      type: String,
      default: ""
    },
    value: {
      type: [String, Number]
    },
    textKey: {
      type: String,
      default: "displayText"
    },
    valueKey: {
      type: String,
      default: "value"
    },
    loading: {
      type: Boolean,
      default: false
    },
    columns: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      showPopup: false
    };
  },
  computed: {
    currentText() {
      if (this.value) {
        const currentItem = this.columns.filter(
          c => c[this.valueKey] == this.value
        )[0];
        if (currentItem) {
          return currentItem[this.textKey];
        }
      }
      return "";
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
    onConfirm(value) {
      this.$emit("input", value[this.valueKey]);
      this.showPopup = false;
    }
  }
};
</script>
