<template>
  <div class="face">
    <div class="face-tip margin-bottom-10">
      温馨提示：登记人脸信息后无法取消订单，如需退款，请先删除人脸信息
    </div>
    <template v-for="(ticket, index) in tickets">
      <section
        v-if="ticket.allowEnrollFace || ticket.photos.length > 0"
        :key="index"
        :class="{ 'margin-bottom-10': index != tickets.length - 1 }"
        class="face-panel"
      >
        <h3 class="face-panel-hd">{{ ticket.ticketTypeName }}</h3>
        <van-cell-group :border="false" class="field-cell">
          <van-cell title="可用数量" :value="ticket.surplusQuantity" />
          <van-cell title="入园凭证" :value="ticket.ticketCode" />
          <van-cell title="己登人脸" :value="ticket.photos.length" />
        </van-cell-group>
        <div class="face-panel-upload">
          <h3 class="face-panel-upload-hd">人脸信息</h3>
          <div
            v-if="ticket.maxPhotoQuantity > 0"
            class="face-panel-upload-images"
            :style="{
              'grid-template-rows': `${previewHeight}px`,
              'grid-auto-rows': `${previewHeight}px`
            }"
          >
            <div
              v-for="(photo, index) in ticket.photos"
              :key="photo.id"
              class="face-panel-upload-preview"
            >
              <div class="face-panel-upload-preview-image" @click="preview(ticket.photos, index)">
                <img v-lazy="photo.url" />
              </div>
              <van-icon
                v-if="ticket.ticketSaleStatusName == '已售'"
                name="delete"
                class="face-panel-upload-preview-delete"
                @click="deletePhoto(ticket, photo)"
              />
            </div>
            <div
              v-if="ticket.allowEnrollFace && ticket.photos.length < ticket.maxPhotoQuantity"
              class="face-panel-upload-uploader"
              @click="uploadPhoto(ticket)"
            >
              <van-icon name="plus" class="face-panel-upload-uploader-plus" />
            </div>
          </div>
        </div>
      </section>
    </template>
    <van-image-preview
      v-model="showPreview"
      :images="previewPhotos"
      :start-position="previewIndex"
      @change="onPreviewChange"
    >
      <template v-slot:index>
        <div class="face-preview">
          <span>{{ `${previewIndex + 1}/${previewPhotos.length}` }}</span>
          <van-icon name="cross" @click="showPreview = false" />
        </div>
      </template>
    </van-image-preview>
  </div>
</template>

<script>
import imageHelper from "./../../utils/imageHelper.js";
import settingService from "./../../services/settingService.js";
import ticketService from "./../../services/ticketService.js";
import weiXinJsSdkHelper from "./../../utils/weiXinJsSdkHelper.js";
import { mapState } from "vuex";

export default {
  props: {
    listNo: {
      type: String
    }
  },
  data() {
    return {
      tickets: [],
      previewHeight: 80,
      showPreview: false,
      previewPhotos: [],
      previewIndex: 0
    };
  },
  async created() {
    try {
      this.$toast.loading({
        duration: 0,
        message: "加载中..."
      });

      await settingService.configWxJsApi();

      this.tickets = await ticketService.getTicketSalePhotosByListNoAsync(this.listNo);

      this.$nextTick(() => {
        this.getPreviewHeight();
      });
    } finally {
      this.$toast.clear();
    }
  },
  beforeRouteLeave(to, from, next) {
    if (this.showPreview) {
      this.showPreview = false;
      next(false);
      return;
    }
    next();
  },
  computed: {
    ...mapState(["pageLabelMainText"])
  },
  methods: {
    getPreviewHeight() {
      let preview = document.querySelector(".face-panel-upload-uploader");
      if (preview) {
        this.previewHeight = preview.clientWidth;
      } else {
        preview = document.querySelector(".face-panel-upload-preview");
        if (preview) {
          this.previewHeight = preview.clientWidth;
        }
      }
    },
    async uploadPhoto(ticket) {
      try {
        const localId = await weiXinJsSdkHelper.chooseImage();

        this.$toast.loading({
          duration: 0,
          message: "加载中..."
        });

        const url = await weiXinJsSdkHelper.getLocalImgData(localId);

        const formData = new FormData();
        formData.append("TicketId", ticket.ticketId);
        formData.append("RegSource", "3");
        formData.append("Photo", imageHelper.dataURLToBlob(url));
        const id = await ticketService.enrollFaceAsync(formData);

        ticket.photos.push({ id: id, url: url });
      } finally {
        this.$toast.clear();
      }
    },
    async deletePhoto(ticket, photo) {
      try {
        await this.$dialog.confirm({ message: "确定删除人脸？" });

        this.$toast.loading({
          duration: 0,
          message: "加载中..."
        });

        await ticketService.deleteFaceAsync(photo.id);

        const index = ticket.photos.indexOf(photo);
        ticket.photos.splice(index, 1);
      } catch {
        return;
      } finally {
        this.$toast.clear();
      }
    },
    preview(photos, index) {
      this.previewPhotos = photos.map(p => p.url);
      this.previewIndex = index;
      this.showPreview = true;
    },
    onPreviewChange(index) {
      this.previewIndex = index;
    }
  }
};
</script>

<style lang="scss" scoped>
.face {
  background-color: #c0c7cf;
  padding: 10px;
  min-height: calc(100vh - 66px);

  &-tip {
    position: relative;
    padding: 7px 15px;
    background-color: #fff7e0;
    font-size: 13px;
    color: #481a03;
    line-height: 18px;
    border-radius: 5px;
  }

  &-panel {
    &-hd {
      padding: 10px 38px 8px 15px;
      font-size: 14px;
      line-height: 1.3;
      color: #808080;
      background: #eff0f2;
      border-top-left-radius: 5px;
      border-top-right-radius: 5px;
    }

    &-upload {
      padding: 10px 15px;
      background-color: #fff;
      border-top: 1px solid gray;
      border-bottom-left-radius: 5px;
      border-bottom-right-radius: 5px;

      &-hd {
        font-size: 14px;
        margin-bottom: 10px;
      }

      &-images {
        display: grid;
        grid-template-columns: repeat(4, 1fr);
        grid-row-gap: 8px;
        grid-column-gap: 8px;
      }

      &-preview {
        position: relative;

        &-image {
          width: 100%;
          height: 100%;

          img {
            width: 100%;
            height: 100%;
            object-fit: cover;
          }
        }

        &-delete {
          position: absolute;
          bottom: 0;
          right: 0;
          padding: 1px;
          color: #fff;
          background-color: rgba(0, 0, 0, 0.45);
        }
      }

      &-uploader {
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: #fff;
        border: 1px dashed #e5e5e5;

        &-plus {
          color: #969799;
          font-size: 24px;
        }
      }
    }
  }

  &-preview {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 90vw;
  }

  .upload-explain {
    padding: 25px 0px 5px 0px;
  }
}
</style>
