import Pica from "pica";

export default {
  resize(dataUrl, maxWidth, maxHeight, type = "DataURL") {
    return new Promise(function(resolve, reject) {
      try {
        const srcImg = new Image();
        srcImg.src = dataUrl;
        srcImg.onload = function() {
          let width = srcImg.width;
          let height = srcImg.height;
          if (srcImg.width > srcImg.height) {
            width = width > maxWidth ? maxWidth : width;
            height = (width / srcImg.width) * srcImg.height;
          } else {
            height = height > maxHeight ? maxHeight : height;
            width = (height / srcImg.height) * srcImg.width;
          }
          const dstCanvas = document.createElement("canvas");
          dstCanvas.width = width;
          dstCanvas.height = height;
          const pica = Pica();
          pica
            .resize(srcImg, dstCanvas, {
              unsharpAmount: 80,
              unsharpRadius: 0.6,
              unsharpThreshold: 2
            })
            .then(result => {
              if (type == "DataURL") {
                return result.toDataURL("image/jpeg");
              }
              if (type == "Blob") {
                return pica.toBlob(result, "image/jpeg", 0.9);
              }
            })
            .then(photo => {
              const file = {
                width: dstCanvas.width,
                height: dstCanvas.height,
                photo: photo
              };
              resolve(file);
            })
            .catch(error => {
              reject(error);
            });
        };
      } catch (error) {
        reject(error);
      }
    });
  },
  dataURLToBlob(dataurl) {
    var arr = dataurl.split(",");
    var mime = arr[0].match(/:(.*?);/)[1];
    var bstr = atob(arr[1]);
    var n = bstr.length;
    var u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }

    return new Blob([u8arr], { type: mime });
  }
};
