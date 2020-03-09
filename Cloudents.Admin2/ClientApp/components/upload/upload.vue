<template>


<div>
    <h1 align="center">Upload Files</h1>
     <v-layout align-center justify-center column fill-height>
        <file-upload
          class="btn btn-primary"
          post-action="api/adminUpload/upload"
          
          :chunk-enabled="true"
          :chunk="{
            action: 'api/adminUpload/upload',
            minSize: chunkMinSize,
            maxActive: chunkMaxActive,
            maxRetries: chunkMaxRetries,
            progress: progressHandler
          }"
          extensions="gif,jpg,jpeg,png,webp,mp4"
          accept="image/png,image/gif,image/jpeg,image/webp,video/mp4"
          :size="1024 * 1024 * 10"
          v-model="files"
          @input-file="inputFile"
          ref="upload">
          <v-btn color="blue">Select files</v-btn>
        </file-upload>

        <div v-if="loader" class="uf-uploading-container">
            <div class="uf-uploading-text">
                <span class="uf-bold">Uploading...</span>
                <span>it may take a few minutes</span>
                <span>{{files[0].progress}}%</span>
            </div>
            <v-progress-linear color="success" v-model="files[0].progress"></v-progress-linear>
        </div>
         
        <ul>
          <li v-for="item in uploadedUrl" :key="item">
            {{ item }}
          </li>
        </ul>
        </v-layout>
   </div>
</template>

<style lang="less">
 .uf-uploading-container{
        .uf-uploading-text{
            color: #000;
            display: flex;
            flex-direction: column;
            align-items: center;
            .uf-bold{
                font-weight: bold;
                font-size:18px;
            }
        }
 }
</style>


<script>
import FileUpload from 'vue-upload-component'
import { getBlobs } from './uploadService.js'

export default {
  components: {
    FileUpload,
  },
  data() {
    return {
      files: [],
      // 0MB by default
      chunkMinSize: 0,
      chunkMaxActive: 3,
      chunkMaxRetries: 5,
      uploadedUrl: [],
      // progress: 0,
      showUrl: false,
      loader : false
    }
  },
  methods: {
    inputFile(newFile, oldFile) {
      if (newFile && !oldFile) {
        // add
        console.log('add', newFile)
        this.$refs.upload.active = true
        this.loader = true
      }
      if (newFile && oldFile) {
        // update
        if(newFile.response.hasOwnProperty("url")) {
        this.uploadedUrl.unshift(newFile.response.url)
        this.showUrl = true
        this.loader = false
        }
        console.log('update', newFile)
      }
      if (!newFile && oldFile) {
        // remove
        console.log('remove', oldFile)
      }
    },
    progressHandler(progress){
            this.progress = progress;            
        }
  },
  created() {
    getBlobs().then((list) => {
      this.uploadedUrl = list
    })
  }
}
</script>

