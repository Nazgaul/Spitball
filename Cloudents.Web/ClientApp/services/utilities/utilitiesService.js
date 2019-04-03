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
}

const proccessImageURL = function(url, width, height, mode){
  let usedMode = mode ? mode : 'crop';
  let returnedUrl = `${url}?&width=${width}&height=${height}&mode=${usedMode}`;
  return returnedUrl;
}


export default {
    init,
    proccessImageURL
}