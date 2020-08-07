export default {
  normalize(obj) {
    const keys = Object.keys(obj);
    keys.forEach(key => {
      let value = obj[key];
      if (typeof value == "string" && value) {
        obj[key] = value.replace(/\n+/g, "<br>");
      }
    });
  }
};
