<template>
  <div>
    <van-cell-group class="fixed-top">
      <van-cell :title="timeslotName" icon="search" is-link @click="showTimeslot = true" />
    </van-cell-group>
    <van-action-sheet v-model="showTimeslot" :actions="actions" @select="onTimeslotSelected" />
    <div style="height:44px;"></div>
  </div>
</template>

<script>
import dayjs from "dayjs";

export default {
  name: "search-time-range",
  props: {
    value: String
  },
  data() {
    return {
      timeslotName: "今天",
      showTimeslot: false,
      actions: [
        {
          name: "今天"
        },
        {
          name: "最近一周"
        },
        {
          name: "最近一个月"
        },
        {
          name: "最近三个月"
        }
      ],
      startDate: dayjs().toDateString()
    };
  },
  methods: {
    onTimeslotSelected(item) {
      this.timeslotName = item.name;
      if (item.name === "今天") {
        this.startDate = dayjs().toDateString();
      } else if (item.name === "最近一周") {
        this.startDate = dayjs()
          .addWeeks(-1)
          .toDateString();
      } else if (item.name === "最近一个月") {
        this.startDate = dayjs()
          .addMonths(-1)
          .toDateString();
      } else if (item.name === "最近三个月") {
        this.startDate = dayjs()
          .addMonths(-3)
          .toDateString();
      }
      this.showTimeslot = false;
      this.$emit("input", this.startDate);
    }
  }
};
</script>
