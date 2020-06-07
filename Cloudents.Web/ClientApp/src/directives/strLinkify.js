export const strLinkify = {
   bind: function(el, binding) {
      let str = el.textContent
      let urlPattern = /\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]/gim;
      let pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
      let emailAddressPattern = /[\w.]+@[a-zA-Z_-]+?(?:\.[a-zA-Z]{2,6})+/gim;

      let modifiedText = str;
      let matchedResults = modifiedText.match(urlPattern) || modifiedText.match(pseudoUrlPattern) || modifiedText.match(emailAddressPattern) ;
      if(matchedResults){
      let linkClassName = binding.value;
         matchedResults.forEach(result=>{
            let prefix = result.toLowerCase().indexOf('http') === -1 && result.toLowerCase().indexOf('ftp') === -1 ? '//' : '';
            if(result.match(urlPattern)?.length){
               modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="${prefix + result.trim()}" target="_blank">${result}</a>`)
            }
            if(result.match(pseudoUrlPattern)?.length){
               modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="${prefix + result.trim()}" target="_blank">${result}</a>`)
            }
            if(result.match(emailAddressPattern)?.length){
               modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="mailto:${result}">${result}</a>`)
            }
         }); 
      }
      el.innerHTML = modifiedText
   },
   unbind: function() {
   },
}