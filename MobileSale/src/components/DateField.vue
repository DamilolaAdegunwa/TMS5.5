<template>
  <div class="van-hairline--bottom">
    <van-field
      :label="label"
      :value="value"
      :placeholder="placeholder"
      readonly
      @focus="onFocus"
      @click="onClick"
    />
    <van-popup v-model="showDatePicker" position="bottom">
      <van-datetime-picker
        :type="type"
        :value="defaultValue"
        :min-date="minDate"
        :max-date="maxDate"
        :min-hour="minHour"
        :max-hour="maxHour"
        @confirm="confirm"
        @cancel="cancel"
      />
    </van-popup>
  </div>
</template>

<script>
import dayjs from "dayjs";

export default {
  name: "date-field",
  data() {
    return {
      showDatePicker: false
    };
  },
  props: {
    type: {
      type: String,
      default: "date"
    },
    value: {
      type: String,
      default: function() {
        return dayjs().toDateString();
      }
    },
    label: {
      type: String,
      default: "日期"
    },
    placeholder: {
      type: String,
      default: "必填，选择日期"
    },
    minDate: {
      type: Date
    },
    maxDate: {
      type: Date
    },
    minHour: {
      type: Number,
      default: 0
    },
    maxHour: {
      type: Number,
      default: 23
    }
  },
  computed: {
    defaultValue() {
      if (this.type === "time") {
        return this.value;
      } else {
        return new Date(this.value);
      }
    }
  },
  methods: {
    onFocus() {
      document.activeElement.blur();
    },
    onClick() {
      this.showDatePicker = true;
    },
    confirm(value) {
      this.showDatePicker = false;

      if (this.type === "date") {
        this.$emit("input", dayjs(value).toDateString());
      } else {
        this.$emit("input", value);
      }
    },
    cancel() {
      this.showDatePicker = false;
    }
  },
  watch: {
    showDatePicker(val) {
      this.$emit("showPopupChange", val);
    }
  }
};
</script>
