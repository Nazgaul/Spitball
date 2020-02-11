<template>
   <div id="div-gpt-ad-1581424574316-0" class="dfpSlot">22222</div>
</template>

<script>
export default {
   name: "DfpSlot",
  data() {
     return {
        DfpJsReady:false,
     }
  },
  watch: {
    DfpJsReady(val){
       if(val){
          googletag.cmd.push(function() { googletag.display('div-gpt-ad-1581424574316-0'); });
       }
    } 
  },   
   beforeCreate() {

      let path = '/107474526/test-feed-top-left';
      let size = [300, 250];
      let id = 'div-gpt-ad-1581424574316-0';
      let self = this;
      let DfpJs = document.createElement('script');
      DfpJs.async = true;
      DfpJs.src = 'https://securepubads.g.doubleclick.net/tag/js/gpt.js'
      document.getElementsByTagName('head')[0].appendChild(DfpJs);
      DfpJs.onload = () => {
         window.googletag = window.googletag || {cmd: []};
         googletag.cmd.push(function() {
            // debugger
            googletag.defineSlot(path, size, id).addService(googletag.pubads());
            googletag.pubads().enableSingleRequest();
            googletag.enableServices();
            self.DfpJsReady = true;
         });
      }
   },
}

</script>
<style lang="less">
// .dfpSlot{
//    width: 300px;
//    height: 250px;
// }
</style>