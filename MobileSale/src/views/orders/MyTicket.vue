<template>
  <div class="ticket">
    <no-data v-if="dataLoaded && tickets.length === 0" :content-height="contentHeight"></no-data>
    <van-list v-else v-model="loading" :finished="finished" @load="getTickets">
      <van-panel
        v-for="(ticket, index) in tickets"
        :key="index"
        :title="ticket.wxShowQrCode ? ticket.ticketCode : ticket.listNo"
        :status="ticket.statusName"
        class="margin-bottom-10"
      >
        <van-cell-group>
          <van-cell :title="ticket.ticketTypeName" />
          <van-cell title="有效期" class="ticket-time">
            <span>{{ ticket.startDate }}</span>
            <span v-if="ticket.endDate !== ticket.startDate">至{{ ticket.endDate }}</span>
          </van-cell>
          <van-cell title="下单时间" :value="ticket.cTime" />
        </van-cell-group>
        <div
          v-for="(groundChangCi, index) in ticket.groundChangCis"
          :key="index"
          :title="groundChangCi.groundName"
          class="ticket-ground van-hairline--bottom"
        >
          <span>{{ groundChangCi.groundName }}</span>
          <span>，{{ ticket.startDate }}日{{ groundChangCi.changCiName }}</span>
          <span v-for="(seat, seatIndex) in groundChangCi.seats" :key="seatIndex"
            >，{{ seat }}</span
          >
        </div>
        <div class="ticket-qrcode">
          <img v-if="ticket.wxShowQrCode" :src="ticket.qrcode" />
          <svg v-else :ref="ticket.ticketCode" class="listno-barcode-img" />
        </div>
      </van-panel>
    </van-list>
  </div>
</template>

<script>
import qrcodeHelper from "./../../utils/qrcodeHelper.js";
import ticketService from "@/services/ticketService.js";
import NoData from "@/components/NoData.vue";

export default {
  components: {
    NoData
  },
  data() {
    return {
      loading: false,
      dataLoaded: false,
      finished: false,
      queryInput: {
        skipCount: 0,
        maxResultCount: 10
      },
      tickets: []
    };
  },
  computed: {
    contentHeight() {
      return this.$store.state.clientHeight - 46;
    }
  },
  watch: {
    async finished(val){
      if(val){
        for (let i = 0; i < this.tickets.length; i++) {
          let ticket = this.tickets[i];
          await qrcodeHelper.createBarCodeAsync(
            this.$refs[ticket.ticketCode],
            ticket.listNo
          );
        }
      }
    }
  },
  methods: {
    async getTickets() {
      try {
        this.loading = true;
        this.dataLoaded = false;
        let result = await ticketService.getMemberTicketsForMobileAsync(this.queryInput);
        for (let i = 0; i < result.items.length; i++) {
          let ticket = result.items[i];
          ticket.qrcode = await qrcodeHelper.createQRCodeAsync(ticket.ticketCode);
          this.tickets.push(ticket);
        }

        this.queryInput.skipCount = this.queryInput.skipCount + result.items.length;

        this.finished = this.tickets.length === result.items.length;
      } catch (err) {
        return;
      } finally {
        this.loading = false;
        this.dataLoaded = true;
      }
    }
  }
};
</script>

<style lang="scss">
.ticket {
  &-qrcode {
    width: 100%;
    text-align: center;
  }

  &-time {
    /deep/ .van-cell__value {
      flex: 2;
    }
  }

  &-ground {
    padding: 10px 16px;
  }
}
</style>
