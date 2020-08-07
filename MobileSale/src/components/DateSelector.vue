<template>
  <div>
    <van-row type="flex" gutter="8">
      <van-col span="6" v-for="day in showDates" :key="day.date">
        <div
          class="date"
          :class="{ 'date-selected': selectedDate == day.date, 'date-disable': day.disable }"
          @click="onDateChange(day)"
        >
          <div>
            <div class="date-text">{{ day.displayText }}</div>
            <div v-if="day.disable">不可订</div>
            <div v-else-if="day.price" class="date-price">
              <dfn class="dfn">¥</dfn>
              <span>{{ day.price }}</span>
            </div>
          </div>
          <div v-if="selectedDate === day.date" class="triangle-right-bottom">
            <van-icon name="xuanzhong" />
          </div>
        </div>
      </van-col>
      <van-col v-if="showMoreDate" span="6">
        <div class="date date-text" @click="showCalendar = true">
          <span>更多日期</span>
          <van-icon name="arrow" />
        </div>
      </van-col>
    </van-row>
    <van-calendar
      v-model="showCalendar"
      :min-date="minDate"
      :max-date="maxDate"
      :formatter="formatter"
      :show-confirm="false"
      color="#19a0f0"
      @confirm="confirm"
    />
  </div>
</template>

<script>
import dayjs from "dayjs";

const weekNames = ["周日", "周一", "周二", "周三", "周四", "周五", "周六"];

export default {
  name: "DateSelector1",
  props: {
    value: {
      type: String,
      default: dayjs().toDateString()
    },
    dates: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      selectedDate: this.value,
      showCalendar: false
    };
  },
  computed: {
    showDates() {
      const dates = [];
      dates.push(this.dates[0]);
      dates.push(...this.dates.filter(d => d.date != this.dates[0].date && !d.disable).slice(0, 2));
      if (this.selectedDate && !dates.some(d => d.date == this.selectedDate)) {
        const selectedDay = this.dates.find(d => d.date == this.selectedDate);
        if (selectedDay) {
          dates[dates.length - 1] = selectedDay;
        }
      }
      const today = dayjs().toDateString();
      const tomorrow = dayjs()
        .addDays(1)
        .toDateString();
      for (const date of dates) {
        if (date.displayText) continue;
        if (date.date === today) {
          date.displayText = `今天${date.date.substr(5, 5)}`;
        } else if (date.date === tomorrow) {
          date.displayText = `明天${date.date.substr(5, 5)}`;
        } else {
          date.displayText = `${date.date.substr(5, 5)}${weekNames[dayjs(date.date).day()]}`;
        }
      }
      return dates;
    },
    showMoreDate() {
      return this.dates.filter(d => d.date != this.dates[0].date && !d.disable).length > 2;
    },
    minDate() {
      return dayjs(this.dates[0].date).toDate();
    },
    maxDate() {
      return dayjs(this.dates[this.dates.length - 1].date).toDate();
    }
  },
  watch: {
    value(val) {
      this.selectedDate = val;
    }
  },
  methods: {
    onDateChange(day) {
      if (!day.disable) {
        this.selectedDate = day.date;
        this.$emit("input", this.selectedDate);
      }
    },
    formatter(day) {
      const dateObj = this.dates.find(d => d.date == dayjs(day.date).toDateString());
      if (dateObj) {
        if (dateObj.disable) {
          day.type = "disabled";
        } else {
          if (dateObj.date == this.selectedDate) {
            day.type = "selected";
          } else {
            day.type = "";
          }
          if (dateObj.price) {
            day.bottomInfo = dateObj.price;
            if (day.type != "selected") {
              day.className = "calendar-price";
            }
          }
        }
      }
      return day;
    },
    confirm(date) {
      this.showCalendar = false;
      this.selectedDate = dayjs(date).toDateString();
      this.$emit("input", this.selectedDate);
    }
  }
};
</script>

<style lang="scss" scoped>
.date {
  height: 45px;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
  border: #999 solid 1px;
  border-radius: 5px;

  &-text {
    font-size: 12px;
    line-height: 14px;
    color: #19a0f0;
  }

  &-price {
    font-weight: 400;
    line-height: 20px;
    font-size: 13px;
    color: #f40;

    .dfn {
      margin-right: 2px;
    }
  }

  &-selected {
    border: #19a0f0 solid 1px;
    background-size: 17px;
    position: relative;
  }

  &-disable {
    color: #bbb;

    .date-text {
      color: #bbb;
    }
  }
}
</style>

<style lang="scss">
.calendar-price {
  .van-calendar__bottom-info {
    color: #f40;
  }
}
</style>
