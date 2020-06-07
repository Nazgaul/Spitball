const init = function(){
    HTMLTextAreaElement.prototype.insertAtCaret = function (text) {
        text = text || '';
        if (document.selection) {
          // IE
          this.focus();
          let sel = document.selection.createRange();
          sel.text = text;
        } else if (this.selectionStart || this.selectionStart === 0) {
          // Others
          let startPos = this.selectionStart;
          let endPos = this.selectionEnd;
          this.value = this.value.substring(0, startPos) +
            text +
            this.value.substring(endPos, this.value.length);
          this.selectionStart = startPos + text.length;
          this.selectionEnd = startPos + text.length;
        } else {
          this.value += text;
        }
        this.focus();
        return this.value;
      };
};

const proccessImageUrl = function(url, width, height, mode){
  let usedMode = mode ? mode : 'crop';
  if(url){
      let returnedUrl = `${url}?&width=${width}&height=${height}&mode=${usedMode}`;
      return returnedUrl;
  }else{
      return '';
  }
};


const dateFormater = function(dateTime){
  let lang = `${global.lang}-${global.country}`;
  let date = new Date(dateTime);
  let options = { year: "numeric", month: "short", day: "2-digit" };
  return new Intl.DateTimeFormat(lang, options).format(date);
};

// function isoStringDateWithOffset(days){
//   let ourDate = new Date();
//   let pastDate = ourDate.getDate() + days;
//   ourDate.setDate(pastDate);
//   return ourDate.toISOString();
// }
export default {
    init,
    proccessImageURL: proccessImageUrl,
    dateFormater,
    //IsoStringDateWithOffset: isoStringDateWithOffset
}