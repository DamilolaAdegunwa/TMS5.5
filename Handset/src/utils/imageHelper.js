import CanvasToBMP from "./CanvasToBMP.js";
export default {
  resizeToBmp(dataUrl, maxWidth, maxHeight) {
    return new Promise(function(resolve, reject) {
      try {
        let srcImg = new Image();
        srcImg.src = dataUrl;
        srcImg.onload = function() {
          let dstCanvas = document.createElement("canvas");
          let width = srcImg.width;
          let height = srcImg.height;
          if (srcImg.width > srcImg.height) {
            width = width > maxWidth ? maxWidth : width;
            height = (width / srcImg.width) * srcImg.height;
          } else {
            height = height > maxHeight ? maxHeight : height;
            width = (height / srcImg.height) * srcImg.width;
          }
          dstCanvas.width = width;
          dstCanvas.height = height;
          let ctx = dstCanvas.getContext("2d");
          ctx.drawImage(srcImg, 0, 0, dstCanvas.width, dstCanvas.height);
          let photo = CanvasToBMP.toDataURL(dstCanvas);
          resolve({
            width: dstCanvas.width,
            height: dstCanvas.height,
            photo: photo
          });
        };
      } catch (error) {
        reject(error);
      }
    });
  },
  resize(dataUrl, maxWidth, maxHeight) {
    return new Promise(function(resolve, reject) {
      try {
        let srcImg = new Image();
        srcImg.src = dataUrl;
        srcImg.onload = function() {
          let dstCanvas = document.createElement("canvas");
          let width = srcImg.width;
          let height = srcImg.height;
          if (srcImg.width > srcImg.height) {
            width = width > maxWidth ? maxWidth : width;
            height = (width / srcImg.width) * srcImg.height;
          } else {
            height = height > maxHeight ? maxHeight : height;
            width = (height / srcImg.height) * srcImg.width;
          }
          dstCanvas.width = width;
          dstCanvas.height = height;
          let ctx = dstCanvas.getContext("2d");
          ctx.drawImage(srcImg, 0, 0, dstCanvas.width, dstCanvas.height);
          let photo = dstCanvas.toDataURL("image/jpeg");
          resolve({
            width: dstCanvas.width,
            height: dstCanvas.height,
            photo: photo
          });
        };
      } catch (error) {
        reject(error);
      }
    });
  }
};
