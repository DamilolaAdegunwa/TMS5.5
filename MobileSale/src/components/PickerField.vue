<template>
  <div>
    <van-cell
      v-if="isCell"
      :title="label"
      is-link
      :value="currentText ? currentText : placeholder"
      @click="onClick"
    />
    <van-field
      v-else
      :label="label"
      :value="currentText"
      :placeholder="placeholder"
      right-icon="arrow"
      readonly
      @focus="onFocus"
      @click="onClick"
    />
    <div></div>
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
    },
    isCell: {
      type: Boolean,
      default: false
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
        const currentItem = this.columns.filter(c => c[this.valueKey] == this.value)[0];
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

<style lang="scss" scoped>
.custom-van-field{
  border-bottom: 1px solid #ebedf0;
  margin: 0px 0px 0px 16px;
}
</style>