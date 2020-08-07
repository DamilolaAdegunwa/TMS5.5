<template>
  <div>
    <van-field
      ref="input"
      :label="label"
      v-model="certNo"
      :placeholder="placeholder"
      :error="error"
      :error-message="errorMessage"
      readonly
      clearable
      @focus="onFocus"
      @click="onClick"
    />
    <van-number-keyboard
      :show="showPopup"
      extra-key="X"
      :z-index="ZIndex"
      close-button-text="完成"
      @input="onInput"
      @delete="onDelete"
      @blur="onBlur"
    />
  </div>
</template>

<script>
export default {
  name: "idcard-field",
  data() {
    return {
      showPopup: false,
      certNo: this.value
    };
  },
  props: {
    label: {
      type: String,
      default: "身份证"
    },
    value: {
      type: String,
      default: ""
    },
    placeholder: {
      type: String,
      default: "必填，输入证件号"
    },
    ZIndex: {
      type: Number,
      default: 100
    },
    delay: {
      type: Boolean,
      default: false
    },
    error: {
      type: Boolean,
      default: false
    },
    errorMessage: {
      type: String
    }
  },
  methods: {
    onFocus() {
      document.activeElement.blur();
    },
    onClick() {
      if (this.delay) {
        setTimeout(() => {
          this.showPopup = true;
        }, 300);
      } else {
        this.showPopup = true;
      }
    },
    onInput(val) {
      this.certNo += val;
      if (this.certNo.length === 18) {
        this.showPopup = false;
      }
      this.$emit("input", this.certNo);
    },
    onDelete() {
      this.certNo = this.certNo.substr(0, this.certNo.length - 1);
      this.$emit("input", this.certNo);
    },
    onBlur() {
      this.showPopup = false;
    }
  },
  watch: {
    value(newVal) {
      this.certNo = newVal;
    },
    showPopup(val) {
      let top = 0;
      let element = this.$refs.input.$refs.input;
      while (element) {
        top += element.offsetTop;

        element = element.offsetParent;
      }
      this.$emit("showPopupChange", val, 246, top);
    }
  }
};
</script>
