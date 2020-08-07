<template>
  <div class="van-hairline--bottom">
    <van-field
      :label="label"
      :value="currentValue"
      :placeholder="placeholder"
      readonly
      @focus="onFocus"
      @click="onClick"
    />
    <van-popup v-model="showPopup" position="bottom">
      <van-area
        :area-list="list"
        :columns-num="2"
        :value="value"
        @cancel="onCancel"
        @confirm="onConfirm"
      />
    </van-popup>
  </div>
</template>

<script>
export default {
  name: "area-field",
  props: {
    label: {
      type: String
    },
    placeholder: {
      type: String,
      default: ""
    },
    value: {},
    list: {
      type: Object
    }
  },
  data() {
    return {
      currentValue: this.value,
      showPopup: false
    };
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
      let value = "";
      for (let i = 0; i < values.length; i++) {
        if (value.length > 0) {
          value += "|";
        }
        value += values[i].name;
      }
      this.currentValue = value;

      this.$emit("input", values[values.length - 1].code);

      this.showPopup = false;
    }
  }
};
</script>
