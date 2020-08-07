<template>
  <div v-if="pageLoaded">
    <div class="scenic">
      <div class="scenic-screen">
        <van-swipe :autoplay="3000" :height="swipeHeight">
          <van-swipe-item v-for="(image, index) in images" :key="index">
            <img :src="image" />
          </van-swipe-item>
        </van-swipe>
      </div>
      <div v-if="scenic.noticeTitle" @click="showNotice = true" class="scenic-event">
        <div>
          <van-icon name="gonggao" />
          <span>{{ scenic.noticeTitle }}</span>
        </div>
        <div class="scenic-more">
          <van-icon name="arrow" />
        </div>
      </div>
      <div @click="showScenic = true" class="scenic-ext">
        <div>
          <div class="scenic-ext-title">
            <span v-if="scenic.openText">开放时间</span>
            <span>{{pageLabelMainText}}特色</span>
            <span>{{pageLabelMainText}}简介</span>
          </div>
          <div v-if="scenic.openText" class="scenic-ext-word">
            {{ scenic.openText }}
          </div>
        </div>
        <div class="scenic-more">
          <span>详情</span>
          <van-icon name="arrow" />
        </div>
      </div>
    </div>
    <div class="ticket">
      <div class="ticket-title">
        <van-icon name="menpiao-y" />
        <span>{{pageLabelMainText}}门票</span>
      </div>
      <div class="ticket-ul">
        <div v-for="ticketType in ticketTypes" :key="ticketType.id" class="ticket-li">
          <div style="flex:1;">
            <div class="ticket-li-title">{{ ticketType.name }}</div>
            <div class="ticket-li-timebox">
              <span>{{ ticketType.travelDateText }}</span>
              <span :style="{ color: getRefundColor(ticketType) }">{{
                ticketType.refundText
              }}</span>
              <span>无需取票</span>
            </div>
            <div class="ticket-li-detail" @click="onShowDescription(ticketType)">
              <span>新品</span>
              <span class="ticket-li-detail-split"></span>
              <span>购买须知></span>
            </div>
          </div>
          <div>
            <div class="ticket-li-price">
              <span>¥</span>
              <i>{{ ticketType.price }}</i>
            </div>
            <div class="ticket-li-btn">
              <van-button size="small" @click="onBuy(ticketType)">立即预订</van-button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <scenic-notice v-model="showNotice" :scenic="scenic" />
    <scenic-description v-model="showScenic" :scenic="scenic" />
    <ticket-type-description
      v-model="showDescription"
      :show-buy="true"
      :ticket-type-id="selectedTicketType.id"
      :ticket-type-name="selectedTicketType.name"
      :should-read="selectedTicketType.shouldReadDescription"
      :price="selectedTicketType.price"
    />
  </div>
</template>

<script>
import dayjs from "dayjs";
import { mapState, mapMutations } from "vuex";
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import ticketTypeService from "./../../services/ticketTypeService.js";
import scenicService from "./../../services/scenicService.js";
import settingService from "./../../services/settingService.js";
import TicketTypeDescription from "./TicketTypeDescription.vue";
import ScenicDescription from "./ScenicDescription.vue";
import ScenicNotice from "./ScenicNotice.vue";
import scenicImage1 from "./../../assets/scenic1.jpg";
import scenicImage2 from "./../../assets/scenic2.jpg";

const today = dayjs();
const tomorrow = dayjs().addDays(1);

export default {
  mixins: [mobileMixin],
  components: {
    TicketTypeDescription,
    ScenicDescription,
    ScenicNotice
  },
  props: {
    publicSaleFlag: {
      type: [Boolean, String],
      default: true
    }
  },
  data() {
    return {
      images: [scenicImage1, scenicImage2],
      scenic: {},
      showNotice: false,
      showScenic: false,
      ticketTypes: [],
      showDescription: false,
      selectedTicketType: {},
      shareImgUrl: `${location.origin}${scenicImage1}`
    };
  },
  computed: {
    swipeHeight() {
      return (document.documentElement.clientWidth / 640) * 360;
    },
    ...mapState(["groundId", "pageLabelMainText"])
  },
  async created() {
    this.loading();

    if (this.$route.query.groundId && !this.groundId) {
      this.setGroundId(this.$route.query.groundId);
    }
    const groundId = this.$route.query.groundId || this.groundId;

    const getTicketTypeTask = ticketTypeService
      .getTicketTypesForWeiXinSaleAsync({
        publicSaleFlag: this.publicSaleFlag,
        groundId: groundId
      })
      .then(ticketTypes => {
        for (const ticketType of ticketTypes) {
          ticketType.travelDateText = this.getTravelDateText(ticketType);
          ticketType.refundText = this.getRefundText(ticketType);
        }

        this.ticketTypes = ticketTypes;
      });

    const getScenicTask = scenicService
      .getScenicAsync()
      .then(scenic => {
        if (scenic.photoList && scenic.photoList.length > 0) {
          this.images = scenic.photoList.map(p => p.url);
          this.shareImgUrl = this.images[0];
        }

        if (scenic.openTime && scenic.closeTime) {
          const today = dayjs().toDateString();
          const openTime = dayjs(`${today} ${scenic.openTime}:00`);
          const closeTime = dayjs(`${today} ${scenic.closeTime}:00`);
          const now = dayjs();
          if (now.isBefore(openTime)) {
            scenic.openText = `未开放 ${scenic.openTime}开放`;
          } else if (now.isBetween(openTime, closeTime)) {
            scenic.openText = `开放中 ${scenic.closeTime}关闭`;
          } else {
            scenic.openText = `已关闭 明日${scenic.openTime}开放`;
          }
        }

        this.scenic = scenic;
        this.setPageLabelMainText(this.scenic.pageLabelMainText);
      })
      .then(() => {
        let shareUrl = `${location.origin}/tickettype/${this.publicSaleFlag}`;
        if (groundId) {
          shareUrl += `?groundId=${groundId}`;
        }
        settingService.configWxJsApi().then(() => {
          settingService.configWxShareUrl(shareUrl, this.shareImgUrl);
        });
      });

    try {
      await Promise.all([getTicketTypeTask, getScenicTask]);
      this.pageLoaded = true;
    } finally {
      this.loaded();
    }
  },
  methods: {
    onBuy(ticketType) {
      if (ticketType.shouldReadDescription) {
        this.onShowDescription(ticketType);
      } else {
        this.$router.push({
          name: "buyticket",
          params: { ticketTypeId: ticketType.id }
        });
      }
    },
    onShowDescription(ticketType) {
      this.selectedTicketType = ticketType;
      this.showDescription = true;
    },
    getTravelDateText(ticketType) {
      const startTravelDate = dayjs(ticketType.startTravelDate);
      let travelDateText = "";
      if (startTravelDate.isSameOrBefore(today)) {
        travelDateText = "今日";
      } else if (startTravelDate.isSameOrBefore(tomorrow)) {
        travelDateText = "明日";
      } else {
        travelDateText = startTravelDate.format("MM月DD日");
      }

      return `最早可订${travelDateText}票`;
    },
    getRefundText(ticketType) {
      if (ticketType.allowRefund === false) {
        return "不可退";
      }

      return ticketType.refundLimited ? "有条件退" : "随时退";
    },
    getRefundColor(ticketType) {
      if (ticketType.allowRefund === false) {
        return "#ff2f39";
      }
      return ticketType.refundLimited ? "#ffae13" : "#099fde";
    },
    ...mapMutations(["setGroundId", "setPageLabelMainText"])
  }
};
</script>

<style lang="scss" scoped>
.scenic {
  margin-bottom: 10px;
  background-color: #fff;

  &-screen {
    img {
      width: 100%;
      height: 100%;
    }
  }

  &-event {
    padding: 7px 15px;
    color: #ff2f39;
    border-bottom: 1px solid #dbdbdb;
    display: flex;
    justify-content: space-between;
    align-items: center;

    .van-icon-gonggao {
      margin-right: 5px;
    }
  }

  &-ext {
    padding: 13px 15px;
    display: flex;
    justify-content: space-between;
    align-items: center;

    &-title {
      font-size: 15px;

      span {
        margin-right: 5px;
      }
    }

    &-word {
      margin-top: 8px;
      line-height: 13px;
      font-size: 12px;
      color: #999;
    }
  }

  &-more {
    text-align: right;
    line-height: 24px;
    color: #999;

    span {
      display: inline-block;
      vertical-align: middle;
      margin: 0 6px;
      height: 18px;
      line-height: 18px;
      font-size: 13px;
    }

    i {
      vertical-align: middle;
    }
  }
}

.ticket {
  background-color: #fff;

  &-title {
    padding: 8px 15px;
    font-size: 17px;
    color: #000;
    height: 24px;
    line-height: 24px;
    border-bottom: 1px solid #dbdbdb;

    i {
      color: #ff7d13;
      margin-right: 5px;
      font-size: 24px;
      vertical-align: top;
      margin-top: -3px;
    }

    span {
      display: inline-block;
      vertical-align: top;
    }
  }

  &-ul {
    padding-left: 15px;
  }

  &-li {
    padding: 10px 0;
    border-bottom: 1px solid #dbdbdb;
    display: flex;
    justify-content: space-between;
    align-items: center;

    &-title {
      margin-bottom: 7px;
      padding-right: 0;
      line-height: 18px;
      font-size: 14px;
      color: #000;
      text-overflow: ellipsis;
      overflow: hidden;
    }

    &-timebox {
      overflow: hidden;
      color: #099fde;

      span {
        display: inline-block;
        overflow: hidden;
        height: 15px;
        max-height: 15px;
        line-height: 15px;
        font-size: 11px;
        margin-right: 5px;
        margin-bottom: 2px;
      }
    }

    &-detail {
      font-size: 12px;
      color: #999;
      height: 21px;
      line-height: 21px;

      &-split {
        background: #999;
        height: 11px;
        width: 1px;
        margin: -2px 5px;
      }

      span {
        display: inline-block;
        vertical-align: middle;
      }
    }

    &-price {
      color: #f40;
      text-align: right;
      overflow: hidden;
      word-break: break-all;
      font-weight: 400;
      padding-right: 15px;
      margin-bottom: 5px;

      span {
        font-size: 15px;
        font-family: Arial;
        margin: 0 1px;
      }

      i {
        font-size: 20px;
        font-style: normal;
        line-height: 21px;
        margin: 0 1px;
      }
    }

    &-btn {
      padding-right: 15px;
      text-align: right;

      button {
        background: -webkit-linear-gradient(left, #ff6034, #ee0a24);
        background: linear-gradient(to right, #ff6034, #ee0a24);
        border: 0;
        font-size: 14px;
        color: #fff;
        padding: 0 6px;
      }
    }
  }
}
</style>
