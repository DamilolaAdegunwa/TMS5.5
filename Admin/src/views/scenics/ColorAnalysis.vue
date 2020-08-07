<template>
  <div class="div-color-analysis">
    <div class="header">
      <div class="title">颜色分析器</div>
      <div class="sub-title">color analysis，图片颜色分析，传入图片获取图片主颜色</div>
    </div>
    <div class="div-upload">
      <el-upload
        class="avatar-uploader"
        action="https://jsonplaceholder.typicode.com/posts/"
        :show-file-list="false"
        :on-success="handleAvatarSuccess"
        :before-upload="beforeAvatarUpload"
      >
        <img ref="previewImg" v-if="imgUrl" :src="imgUrl" class="avatar" />
        <i v-else class="el-icon-plus avatar-uploader-icon"></i>
      </el-upload>
    </div>
    <div v-if="colors.length > 4" class="div-colors">
      <div
        v-for="(color,colorIndex) in colors"
        :key="colorIndex"
        :style="{background: color}"
        class="colors-div"
        @click="setColor(color)"
      >{{color}}</div>
    </div>
    <div
      v-if="selectColors.length > 1  "
      class="div-gradient"
      :style="{background: `linear-gradient(to bottom right, ${selectColors[selectColors.length - 2]}, ${selectColors[selectColors.length - 1]})`}"
    >
      <div class="div-style">
        <img class="style-img" :src="indexHeaderStyleUrl" />
      </div>
      <div class="gradient-div">
        <img class="gradient-img" :src="imgUrl" />
      </div>
    </div>
  </div>
</template>

<script>
import ColorThief from "colorthief";
import iIndexHeaderStyle from "@/assets/img/IndexHeaderStyle.png";

export default {
  data() {
    return {
      imgUrl: "",
      canvasWidth: 200,
      canvasHeight: 200,
      colorAnalysis: null,
      winWidth: 375,
      maxColors: 5,
      colors: [],
      rbgColors: [],
      input: {
        photoList: []
      },
      selectColors: [],
      indexHeaderStyleUrl: iIndexHeaderStyle
    };
  },
  methods: {
    handleAvatarSuccess(res, file) {
      console.log(file);
      this.imgUrl = URL.createObjectURL(file.raw);
    },
    beforeAvatarUpload(file) {
      let reader = new FileReader();
      let self = this;
      reader.onload = function(e) {
        self.imgUrl = e.target.result;
        let srcImg = new Image();
        srcImg.src = e.target.result;
        srcImg.onload = function() {
          let colorThiefObj = new ColorThief();
          let colorPalettes = colorThiefObj.getPalette(srcImg, 5);
          self.colors = [];
          for (let i = 0; i < colorPalettes.length; i++) {
            let colorPalette = colorPalettes[i];
            self.colors.push(
              "#" +
                colorPalette[0].toString(16).toUpperCase() +
                colorPalette[1].toString(16).toUpperCase() +
                colorPalette[2].toString(16).toUpperCase()
            );
          }
        };
      };
      reader.readAsDataURL(file);
    },
    setColor(color) {
      this.selectColors.push(color);
    },
    getFile(event) {
      var file = event.target.files;
      for (var i = 0; i < file.length; i++) {
        //    上传类型判断
        let reader = new FileReader();
        let self = this;
        reader.onload = function(e) {
          self.imgUrl = e.target.result;
          let srcImg = new Image();
          srcImg.src = e.target.result;
          srcImg.onload = function() {
            let colorThiefObj = new ColorThief();
            let colorPalette = colorThiefObj.getPalette(srcImg, 5);
            console.log(colorPalette);
            colorPalette.foreach(a => {
              console.log(a);
              self.colors.push("#" + a[0].toString(16) + a[1].toString(16) + a[2].toString(16));
            });
          };
        };
        reader.readAsDataURL(file[i]);
      }
    }
  }
};
</script>

<style lang="scss">
.div-color-analysis {
  padding: 10px 0 60px 0;
  box-sizing: border-box;
  background-color: #fafafa;
  .header {
    padding: 40px 45px 30px 45px;
    box-sizing: border-box;
    text-align: center;
    .title {
      font-size: 17px;
      color: #333;
      font-weight: 500;
    }
    .sub-title {
      font-size: 12px;
      color: #7a7a7a;
      padding-top: 9px;
    }
  }

  .div-upload {
    width: 300px;
    height: 300px;
    padding: 0 15px;
    box-sizing: border-box;
    display: -webkit-box;
    display: -webkit-flex;
    display: flex;
    -webkit-box-align: center;
    -webkit-align-items: center;
    align-items: center;
    -webkit-box-pack: center;
    -webkit-justify-content: center;
    justify-content: center;
    background-color: #fff;
    margin: 0 auto;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    overflow: hidden;

    .avatar-uploader .el-upload {
      border: 1px dashed #d9d9d9;
      border-radius: 6px;
      cursor: pointer;
      position: relative;
      overflow: hidden;
    }
    .avatar-uploader .el-upload:hover {
      border-color: #409eff;
    }
    .avatar-uploader-icon {
      font-size: 28px;
      color: #8c939d;
      width: 178px;
      height: 178px;
      line-height: 178px;
      text-align: center;
    }
    .avatar {
      width: 178px;
      height: 178px;
      display: block;
    }
  }

  .div-colors {
    width: 300px;
    height: 80px;
    display: -webkit-box;
    display: -webkit-flex;
    display: flex;
    -webkit-box-align: center;
    -webkit-align-items: center;
    align-items: center;
    -webkit-box-pack: center;
    -webkit-justify-content: center;
    justify-content: center;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    border-radius: 6px;
    overflow: hidden;
    margin: 20px auto 0;
    box-sizing: border-box;
    .colors-div {
      -webkit-box-flex: 1;
      -webkit-flex: 1;
      flex: 1;
      -webkit-flex-shrink: 0;
      flex-shrink: 0;
      height: 100%;
      font-size: 12px;
      color: #fff;
      display: -webkit-box;
      display: -webkit-flex;
      display: flex;
      -webkit-box-orient: vertical;
      -webkit-box-direction: normal;
      -webkit-flex-direction: column;
      flex-direction: column;
      -webkit-box-align: center;
      -webkit-align-items: center;
      align-items: center;
      -webkit-box-pack: center;
      -webkit-justify-content: center;
      justify-content: center;
      word-break: break-all;
    }
  }

  .div-gradient {
    height: 163px;
    width: 280px;
    margin: 20px auto 0;
    padding: 0px 10px 0px 10px;
    border-bottom-right-radius: 20px;
    border-bottom-left-radius: 20px;
    .div-style {
      overflow: hidden;
      height: 80px;
      .style-img {
        width: 100%;
      }
    }
    .gradient-div {
      width: 280px;
      height: 120px;
      border-radius: 6px;
      overflow: hidden;
      margin-top: -21px;
      .gradient-img {
        width: 100%;
        height: 100%;
      }
    }
  }
}
</style>
