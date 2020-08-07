import ajax from "./ajax.js";

export default {
  async downloadExcelAsync(url, input) {
    const response = await ajax.post(url, input, {
      responseType: "blob"
    });

    if (!response) {
      return;
    }
    const blob = new Blob([response], {
      type: response.type
    });
    const fileUrl = window.URL.createObjectURL(blob);
    const aLink = document.createElement("a");
    aLink.style.display = "none";
    aLink.href = fileUrl;
    aLink.setAttribute("download", `excel${new Date().getMilliseconds()}.xlsx`);
    document.body.appendChild(aLink);
    aLink.click();
    document.body.removeChild(aLink);
    window.URL.revokeObjectURL(fileUrl);
  }
};
