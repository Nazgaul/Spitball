export default {
   name:'settingMixin',
   methods: {
      MIXIN_addStream(streamsArray,stream){
         if(!streamsArray) return
         streamsArray.push(stream)
      },
      MIXIN_cleanStreams(streamsArray){
         if(!streamsArray) return
         streamsArray.forEach(t => {
            t.getTracks()[0].stop()
         });
      },
      MIXIN_getMediaTrack(params){
         return navigator.mediaDevices.getUserMedia(params)
                  .then(stream=>{
                     return Promise.resolve(stream)
                  })
                  .catch(err=>{
                     return Promise.reject(err)
                  })
      }
   },
}